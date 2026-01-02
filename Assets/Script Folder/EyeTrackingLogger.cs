using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text;


public class EyeTrackingLogger : MonoBehaviour
{
    [Header("Gaze Source")]
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor gazeInteractor;
    [Header("Fixation Settings")]
    public float fixationThreshold = 0.3f;

    [Header("Ray Settings")]
    public float rayMaxDistance = 10f;
    public float gazeTriggerDuration = 3f; // 你可以用來判斷凝視觸發事件（選用）

    private string currentAOI = "None";
    private float currentFixationTime = 0f;

    private Dictionary<string, int> fixationCounts = new();
    private Dictionary<string, float> fixationDurations = new();

    private List<string> csvLines = new();

    // Start is called before the first frame update
    void Start()
    {
        fixationCounts["AOI_NPC"] = 0;
        fixationCounts["AOI_Physical"] = 0;
        fixationCounts["AOI_Virtual"] = 0;

        fixationDurations["AOI_NPC"] = 0f;
        fixationDurations["AOI_Physical"] = 0f;
        fixationDurations["AOI_Virtual"] = 0f;

        csvLines.Add("timestamp,AOI,fixationDuration");
    }

    // Update is called once per frame
    void Update()
    {
        Transform origin = gazeInteractor.rayOriginTransform;
        Ray ray = new Ray(origin.position, origin.forward);

        string hitAOI = "None";

        if (Physics.Raycast(ray, out RaycastHit hit, rayMaxDistance))
        {
            hitAOI = hit.collider.tag;
        }

        // Same AOI → accumulate fixation time
        if (hitAOI == currentAOI)
        {
            currentFixationTime += Time.deltaTime;
        }
        else
        {
            // AOI changed → finalize previous fixation
            FinalizeFixation();

            currentAOI = hitAOI;
            currentFixationTime = 0f;
        }
    }

    void FinalizeFixation()
    {
        if (currentFixationTime >= fixationThreshold &&
            fixationCounts.ContainsKey(currentAOI))
        {
            fixationCounts[currentAOI]++;
            fixationDurations[currentAOI] += currentFixationTime;

            csvLines.Add(
                $"{Time.time:F3},{currentAOI},{currentFixationTime:F3}"
            );
        }
    }

    void OnApplicationQuit()
    {
        FinalizeFixation();

        StringBuilder sb = new();
        sb.AppendLine("AOI,FixationCount,TotalFixationDuration");

        foreach (var aoi in fixationCounts.Keys)
        {
            sb.AppendLine(
                $"{aoi},{fixationCounts[aoi]},{fixationDurations[aoi]:F3}"
            );
        }

        string summaryPath = Path.Combine(
            Application.persistentDataPath,
            "EyeTracking_FixationSummary.csv"
        );

        string rawPath = Path.Combine(
            Application.persistentDataPath,
            "EyeTracking_FixationRaw.csv"
        );

        File.WriteAllText(summaryPath, sb.ToString());
        File.WriteAllLines(rawPath, csvLines);

        Debug.Log("Eye-tracking CSV saved to:");
        Debug.Log(summaryPath);
        Debug.Log(rawPath);
    }
}
