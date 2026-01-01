using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using System.IO;

public class NPCRequestManager : MonoBehaviour
{
    public string apiUrl = "http://192.168.1.114:5050/api/npc/ask";
    public string language = "zh_TW"; 

    [System.Serializable]
    public class NPCListWrapper
    {
        public List<NPCInfo> npcs;
    }

    [System.Serializable]
    public class NPCInfo
    {
        public string npc_role;
        public string personality;
        public bool is_rag;
        public string configFileName;
    }

    [System.Serializable]
    public class NPCExtraData
    {
        public string npc_role;
        public string personality;
        public bool is_rag;
    }

    public List<NPCInfo> npcList = new List<NPCInfo>();
    public TextToSpeech ttsManager;

    void Start()
    {
        // 讀取簡化版 npc_config.json
        string path = Path.Combine(Application.streamingAssetsPath, "npc_config.json");
        string jsonText = File.ReadAllText(path);

        NPCListWrapper wrapper = JsonUtility.FromJson<NPCListWrapper>(jsonText);
        npcList = wrapper.npcs;

        // 讀取每個 NPC 的 JSON 更新 npc_role、personality、is_rag
        foreach (var npc in npcList)
        {
            LoadNPCConfig(npc);
        }

        Debug.Log($"已載入 {npcList.Count} 個 NPC");
    }

    public NPCInfo GetNPCInfoByConfig(string configFileName)
    {
        foreach (var npc in npcList)
        {
            if (npc.configFileName == configFileName)
                return npc;
        }
        Debug.LogWarning("找不到對應的 NPC JSON：" + configFileName);
        return null;
    }

    public void LoadNPCConfig(NPCInfo npc)
    {
        string path = Path.Combine(Application.streamingAssetsPath, npc.configFileName);
        if (!File.Exists(path))
        {
            Debug.LogError("找不到檔案：" + path);
            return;
        }

        string jsonText = File.ReadAllText(path);
        NPCExtraData extraData = JsonUtility.FromJson<NPCExtraData>(jsonText);

        npc.npc_role = extraData.npc_role;
        npc.personality = extraData.personality;
        npc.is_rag = extraData.is_rag;
    }

    public void SendNPCRequest(string query, NPCInfo npc)
    {
        StartCoroutine(PostRequest(query, npc));
    }

    IEnumerator PostRequest(string query, NPCInfo npc)
    {
        var jsonBody = new NPCRequest
        {
            query = query,
            lang = language,
            npc_role = npc.npc_role,
            personality = npc.personality,
            is_rag = npc.is_rag
        };

        string jsonData = JsonUtility.ToJson(jsonBody);

        using (UnityWebRequest www = UnityWebRequest.PostWwwForm(apiUrl, jsonData))
        {
            byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(jsonData);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("連線失敗: " + www.error);
                Debug.Log("錯誤網址: " + www.url); 
                if (ttsManager != null)
                    ttsManager.ConvertTextToSpeech("Server Error");
            }
            else
            {
                Debug.Log("連線成功 Response: " + www.downloadHandler.text);

                var json = JObject.Parse(www.downloadHandler.text);
                var npcResponse = json["response"]?.ToString();
                
                if (ttsManager != null)
                    ttsManager.ConvertTextToSpeech(npcResponse);
            }
        }
    }

    [System.Serializable]
    public class NPCRequest
    {
        public string query;
        public string lang;
        public string npc_role;
        public string personality;
        public bool is_rag;
    }
}
