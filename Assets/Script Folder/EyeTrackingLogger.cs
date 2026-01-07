using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.XR;

public class EyeTrackingLogger : MonoBehaviour
{
    [Header("Fixation Settings")]
    public float fixationThreshold = 0.3f;

    [Header("Ray Settings")]
    public float rayMaxDistance = 10f;

    [Header("CSV Settings")]
    public float writeInterval = 5f; // 每隔多久 flush CSV
    public bool enableSummaryCSV = true; // 是否生成 Summary CSV

    private string currentAOI = "None";
    private string currentObject = "None";
    private float currentFixationTime = 0f;

    private Dictionary<string, int> fixationCounts = new();
    private Dictionary<string, float> fixationDurations = new();

    private List<string> csvLines = new();
    private bool csvInitialized = false;

    private string rawCSVPath;
    private string summaryCSVPath;

    void Start()
    {
        // 初始化 AOI 統計
        fixationCounts["AOI_NPC"] = 0;
        fixationCounts["AOI_Exhibit"] = 0;
        fixationCounts["AOI_Physical"] = 0;
        fixationCounts["AOI_Virtual"] = 0;
        fixationCounts["Untagged"] = 0;

        fixationDurations["AOI_NPC"] = 0f;
        fixationDurations["AOI_Exhibit"] = 0f;
        fixationDurations["AOI_Physical"] = 0f;
        fixationDurations["AOI_Virtual"] = 0f;
        fixationDurations["Untagged"] = 0f;

        // CSV 路徑
        rawCSVPath = Path.Combine(Application.persistentDataPath, "EyeTracking_Raw.csv");
        summaryCSVPath = Path.Combine(Application.persistentDataPath, "EyeTracking_Summary.csv");

        Debug.Log("Raw CSV path: " + rawCSVPath);
        if (enableSummaryCSV)
            Debug.Log("Summary CSV path: " + summaryCSVPath);

        // 初始化 Raw CSV（覆蓋舊檔）
        string csvHeader = "timestamp,AOI,ObjectName,fixationDuration";
        File.WriteAllText(rawCSVPath, csvHeader + "\n");
        csvInitialized = true;

        StartCoroutine(WriteCSVPeriodically());
    }

    void Update()
    {
        Vector3 gazeOrigin = Vector3.zero;
        Vector3 gazeDirection = Vector3.forward;

        // Hololens / MRTK3 Eye Tracking
        bool eyeDataValid = false;
        List<XRNodeState> nodeStates = new List<XRNodeState>();
        InputTracking.GetNodeStates(nodeStates);

        foreach (var nodeState in nodeStates)
        {
            if (nodeState.nodeType == XRNode.CenterEye)
            {
                if (nodeState.TryGetPosition(out Vector3 pos))
                    gazeOrigin = pos;
                if (nodeState.TryGetRotation(out Quaternion rot))
                    gazeDirection = rot * Vector3.forward;

                eyeDataValid = true;
            }
        }

        if (!eyeDataValid && Camera.main != null)
        {
            gazeOrigin = Camera.main.transform.position;
            gazeDirection = Camera.main.transform.forward;
        }

        Ray ray = new Ray(gazeOrigin, gazeDirection);
        string hitAOI = "None";
        string hitObject = "None";

        if (Physics.Raycast(ray, out RaycastHit hit, rayMaxDistance))
        {
            Transform t = hit.collider.transform;

            // 往上找 AOI Tag
            while (t != null)
            {
                if (t.tag.StartsWith("AOI_"))
                {
                    hitAOI = t.tag;
                    hitObject = t.root.name; // 使用 Root name
                    break;
                }
                t = t.parent;
            }

            if (hitAOI == "None")
            {
                hitAOI = hit.collider.tag;
                hitObject = hit.collider.transform.root.name;
            }
        }

        Debug.Log($"Hit AOI = {hitAOI}, Object = {hitObject}");

        // 同 AOI + 同物件 → 累積 fixation
        if (hitAOI == currentAOI && hitObject == currentObject)
        {
            currentFixationTime += Time.deltaTime;
        }
        else
        {
            FinalizeFixation();
            currentAOI = hitAOI;
            currentObject = hitObject;
            currentFixationTime = 0f;
        }
    }

    void FinalizeFixation()
    {
        if (currentFixationTime >= fixationThreshold && fixationCounts.ContainsKey(currentAOI))
        {
            fixationCounts[currentAOI]++;
            fixationDurations[currentAOI] += currentFixationTime;

            csvLines.Add($"{Time.time:F3},{currentAOI},{currentObject},{currentFixationTime:F3}");
        }
    }

    private IEnumerator WriteCSVPeriodically()
    {
        while (true)
        {
            yield return new WaitForSeconds(writeInterval);

            if (csvLines.Count > 0)
            {
                File.AppendAllLines(rawCSVPath, csvLines);
                csvLines.Clear();
                Debug.Log("Raw CSV updated: " + rawCSVPath);
            }

            if (enableSummaryCSV)
            {
                WriteSummaryCSV();
            }
        }
    }

    private void WriteSummaryCSV()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("AOI,FixationCount,TotalFixationDuration");

        foreach (var aoi in fixationCounts.Keys)
        {
            sb.AppendLine($"{aoi},{fixationCounts[aoi]},{fixationDurations[aoi]:F3}");
        }

        File.WriteAllText(summaryCSVPath, sb.ToString());
        Debug.Log("Summary CSV updated: " + summaryCSVPath);
    }

    private void OnDestroy()
    {
        FinalizeFixation();

        if (csvLines.Count > 0)
            File.AppendAllLines(rawCSVPath, csvLines);

        if (enableSummaryCSV)
            WriteSummaryCSV();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            FinalizeFixation();
            if (csvLines.Count > 0)
                File.AppendAllLines(rawCSVPath, csvLines);

            if (enableSummaryCSV)
                WriteSummaryCSV();
        }
    }
}


// using UnityEngine;
// using System.Collections.Generic;
// using System.IO;
// using System.Text;


// public class EyeTrackingLogger : MonoBehaviour
// {
//     [Header("Gaze Source")]
//     public UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor gazeInteractor;
//     [Header("Fixation Settings")]
//     public float fixationThreshold = 0.3f;

//     [Header("Ray Settings")]
//     public float rayMaxDistance = 10f;
//     public float gazeTriggerDuration = 3f; // 你可以用來判斷凝視觸發事件（選用）

//     private string currentAOI = "None";
//     private float currentFixationTime = 0f;

//     private Dictionary<string, int> fixationCounts = new();
//     private Dictionary<string, float> fixationDurations = new();

//     private List<string> csvLines = new();

//     // Start is called before the first frame update
//     void Start()
//     {
//         fixationCounts["AOI_NPC"] = 0;
//         fixationCounts["AOI_Physical"] = 0;
//         fixationCounts["AOI_Virtual"] = 0;

//         fixationDurations["AOI_NPC"] = 0f;
//         fixationDurations["AOI_Physical"] = 0f;
//         fixationDurations["AOI_Virtual"] = 0f;

//         csvLines.Add("timestamp,AOI,fixationDuration");
//     }

//     // Update is called once per frame
//     void Update()
//     {
//         Transform origin = gazeInteractor.rayOriginTransform;
//         Ray ray = new Ray(origin.position, origin.forward);

//         string hitAOI = "None";

//         if (Physics.Raycast(ray, out RaycastHit hit, rayMaxDistance))
//         {
//             hitAOI = hit.collider.tag;
//         }

//         // Same AOI → accumulate fixation time
//         if (hitAOI == currentAOI)
//         {
//             currentFixationTime += Time.deltaTime;
//         }
//         else
//         {
//             // AOI changed → finalize previous fixation
//             FinalizeFixation();

//             currentAOI = hitAOI;
//             currentFixationTime = 0f;
//         }
//     }

//     void FinalizeFixation()
//     {
//         if (currentFixationTime >= fixationThreshold &&
//             fixationCounts.ContainsKey(currentAOI))
//         {
//             fixationCounts[currentAOI]++;
//             fixationDurations[currentAOI] += currentFixationTime;

//             csvLines.Add(
//                 $"{Time.time:F3},{currentAOI},{currentFixationTime:F3}"
//             );
//         }
//     }

//     void OnApplicationQuit()
//     {
//         FinalizeFixation();

//         StringBuilder sb = new();
//         sb.AppendLine("AOI,FixationCount,TotalFixationDuration");

//         foreach (var aoi in fixationCounts.Keys)
//         {
//             sb.AppendLine(
//                 $"{aoi},{fixationCounts[aoi]},{fixationDurations[aoi]:F3}"
//             );
//         }

//         // string summaryPath = Path.Combine(
//         //     Application.persistentDataPath,
//         //     "EyeTracking_FixationSummary.csv"
//         // );

//         // string rawPath = Path.Combine(
//         //     Application.persistentDataPath,
//         //     "EyeTracking_FixationRaw.csv"
//         // );

//         string timeStamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");

//         string summaryPath = Path.Combine(
//             Application.persistentDataPath,
//             $"EyeTracking_FixationSummary_{timeStamp}.csv"
//         );

//         string rawPath = Path.Combine(
//             Application.persistentDataPath,
//             $"EyeTracking_FixationRaw_{timeStamp}.csv"
//         );


//         File.WriteAllText(summaryPath, sb.ToString());
//         File.WriteAllLines(rawPath, csvLines);

//         Debug.Log("Eye-tracking CSV saved to:");
//         Debug.Log(summaryPath);
//         Debug.Log(rawPath);
//     }
// }

// using System.Collections;
// using System.Collections.Generic;
// using System.IO;
// using System.Text;
// using UnityEngine;
// using UnityEngine.XR;

// public class EyeTrackingLogger : MonoBehaviour
// {
//     [Header("Fixation Settings")]
//     public float fixationThreshold = 0.3f;

//     [Header("Ray Settings")]
//     public float rayMaxDistance = 10f;

//     [Header("CSV Settings")]
//     public float writeInterval = 5f; // 每隔多久 flush CSV
//     public bool enableSummaryCSV = false; // 是否生成 Summary CSV

//     private string currentAOI = "None";
//     private string currentNPC = "None";
//     private float currentFixationTime = 0f;

//     private Dictionary<string, int> fixationCounts = new();
//     private Dictionary<string, float> fixationDurations = new();

//     private List<string> csvLines = new();
//     private bool csvInitialized = false;

//     private string rawCSVPath;
//     private string summaryCSVPath;

//     void Start()
//     {
//         // 初始化 AOI 統計
//         fixationCounts["AOI_NPC"] = 0;
//         fixationCounts["AOI_Physical"] = 0;
//         fixationCounts["AOI_Virtual"] = 0;
//         fixationCounts["Untagged"] = 0;

//         fixationDurations["AOI_NPC"] = 0f;
//         fixationDurations["AOI_Physical"] = 0f;
//         fixationDurations["AOI_Virtual"] = 0f;
//         fixationDurations["Untagged"] = 0f;

//         // CSV 路徑
//         rawCSVPath = Path.Combine(Application.persistentDataPath, "EyeTracking_Raw.csv");
//         summaryCSVPath = Path.Combine(Application.persistentDataPath, "EyeTracking_Summary.csv");

//         Debug.Log("Raw CSV path: " + rawCSVPath);
//         if (enableSummaryCSV)
//             Debug.Log("Summary CSV path: " + summaryCSVPath);

//         StartCoroutine(WriteCSVPeriodically());
//     }

//     void Update()
//     {
//         Vector3 gazeOrigin = Vector3.zero;
//         Vector3 gazeDirection = Vector3.forward;

//         // MRTK3 / Hololens 2 Eye Tracking
//         bool eyeDataValid = false;

//         List<XRNodeState> nodeStates = new List<XRNodeState>();
//         InputTracking.GetNodeStates(nodeStates);

//         foreach (var nodeState in nodeStates)
//         {
//             if (nodeState.nodeType == XRNode.CenterEye)
//             {
//                 if (nodeState.TryGetPosition(out Vector3 pos))
//                     gazeOrigin = pos;
//                 if (nodeState.TryGetRotation(out Quaternion rot))
//                     gazeDirection = rot * Vector3.forward;

//                 eyeDataValid = true;
//             }
//         }

//         if (!eyeDataValid && Camera.main != null)
//         {
//             // Editor 模擬
//             gazeOrigin = Camera.main.transform.position;
//             gazeDirection = Camera.main.transform.forward;
//         }

//         Ray ray = new Ray(gazeOrigin, gazeDirection);
//         string hitAOI = "None";
//         string hitNPC = "None";

//         if (Physics.Raycast(ray, out RaycastHit hit, rayMaxDistance))
//         {
//             // 確認碰到的 Collider
//             Transform t = hit.collider.transform;

//             // 嘗試往上找 Tag = AOI_NPC
//             while (t != null)
//             {
//                 if (t.tag == "AOI_NPC")
//                 {
//                     hitAOI = "AOI_NPC";
//                     hitNPC = t.root.name; // 直接用 Root 名稱
//                     break;
//                 }
//                 t = t.parent;
//             }

//             // 如果找不到 AOI_NPC Tag，就用 Collider 本身的 Tag
//             if (hitAOI == "None")
//             {
//                 hitAOI = hit.collider.tag;
//                 hitNPC = "None";
//             }
//         }

//         Debug.Log($"Hit AOI = {hitAOI}, NPC = {hitNPC}");

//         // 同 AOI + 同 NPC → 累積 fixation
//         if (hitAOI == currentAOI && hitNPC == currentNPC)
//         {
//             currentFixationTime += Time.deltaTime;
//         }
//         else
//         {
//             // AOI 或 NPC 變化 → Finalize previous fixation
//             FinalizeFixation();

//             currentAOI = hitAOI;
//             currentNPC = hitNPC;
//             currentFixationTime = 0f;
//         }
//     }

//     void FinalizeFixation()
//     {
//         if (currentFixationTime >= fixationThreshold && fixationCounts.ContainsKey(currentAOI))
//         {
//             fixationCounts[currentAOI]++;
//             fixationDurations[currentAOI] += currentFixationTime;

//             // 加入暫存 CSV，紀錄 NPC Root 名稱
//             csvLines.Add($"{Time.time:F3},{currentAOI},{currentNPC},{currentFixationTime:F3}");
//         }
//     }

//     private IEnumerator WriteCSVPeriodically()
//     {
//         // 一開始就初始化 Raw CSV（覆蓋舊檔）
//         string csvHeader = "timestamp,AOI,NPC,fixationDuration";
//         File.WriteAllText(rawCSVPath, csvHeader + "\n"); // 覆蓋舊檔
//         csvInitialized = true;
//         while (true)
//         {
//             yield return new WaitForSeconds(writeInterval);

//             // 初始化 Raw CSV
//             if (!csvInitialized)
//             {
//                 string header = "timestamp,AOI,NPC,fixationDuration";
//                 File.AppendAllText(rawCSVPath, header + "\n");
//                 csvInitialized = true;
//             }

//             // 寫入 Raw CSV
//             if (csvLines.Count > 0)
//             {
//                 File.AppendAllLines(rawCSVPath, csvLines);
//                 csvLines.Clear();
//                 Debug.Log("Raw CSV updated: " + rawCSVPath);
//             }

//             // 更新 Summary CSV
//             if (enableSummaryCSV)
//             {
//                 WriteSummaryCSV();
//             }
//         }
//     }

//     private void WriteSummaryCSV()
//     {
//         StringBuilder sb = new StringBuilder();
//         sb.AppendLine("AOI,FixationCount,TotalFixationDuration");

//         foreach (var aoi in fixationCounts.Keys)
//         {
//             sb.AppendLine($"{aoi},{fixationCounts[aoi]},{fixationDurations[aoi]:F3}");
//         }

//         File.WriteAllText(summaryCSVPath, sb.ToString());
//         Debug.Log("Summary CSV updated: " + summaryCSVPath);
//     }

//     private void OnDestroy()
//     {
//         FinalizeFixation();

//         if (csvLines.Count > 0)
//             File.AppendAllLines(rawCSVPath, csvLines);

//         if (enableSummaryCSV)
//             WriteSummaryCSV();
//     }

//     private void OnApplicationPause(bool pause)
//     {
//         if (pause)
//         {
//             FinalizeFixation();
//             if (csvLines.Count > 0)
//                 File.AppendAllLines(rawCSVPath, csvLines);

//             if (enableSummaryCSV)
//                 WriteSummaryCSV();
//         }
//     }
// }