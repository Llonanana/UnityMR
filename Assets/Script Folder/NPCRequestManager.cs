using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CatNPCManager : MonoBehaviour
{
    // 預設改成正確的本機大腦 (Port 5050)
    public string apiUrl = "http://localhost:5050/api/npc/ask";
    public string language = "zh-TW"; 
    public string role = "白起"; 
    public string personality = "introverted"; 
    public bool is_rag = true; 

    public TextToSpeech ttsManager;
    // 注意：如果妳的專案裡沒有 NPCInteractionRecorder，這行可能會報錯，可以先註解掉
    // public NPCInteractionRecorder npcInteractionRecoreder; 

    void Start()
    {
        Debug.Log("貓貓新腳本啟動！連線目標：" + apiUrl);
        SendNPCRequest("你好，你是誰？");
    }

    public void SendNPCRequest(string query)
    {
        StartCoroutine(PostRequest(query));
    }

    IEnumerator PostRequest(string query)
    {
        var jsonBody = new NPCRequest
        {
            query = query,
            lang = language,
            npc_role = role,
            personality = personality,
            is_rag = is_rag
        };

        string jsonData = JsonUtility.ToJson(jsonBody);

        // 使用正確的 Post 方法
        using (UnityWebRequest www = UnityWebRequest.Post(apiUrl, jsonData))
        {
            byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(jsonData);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("貓貓連線失敗: " + www.error);
                // 這裡會把真實的錯誤印出來，而不是舊的 URL
                Debug.Log("錯誤網址: " + www.url); 
                
                if (ttsManager != null)
                    ttsManager.ConvertTextToSpeech("Server Error");
            }
            else
            {
                Debug.Log("貓貓連線成功 Response: " + www.downloadHandler.text);
                
                if (ttsManager != null)
                    ttsManager.ConvertTextToSpeech(www.downloadHandler.text);
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