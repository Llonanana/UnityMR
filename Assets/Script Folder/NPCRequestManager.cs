using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;

public class NPCRequestManager : MonoBehaviour
{
    private string apiUrl = "http://192.168.0.76:5050/api/npc/ask";
    public string language = "zh_TW";
    public string role = "白起";
    public string personality = "introvert";
    public bool is_rag = true;
    public TextToSpeech ttsManager;
    public TextManager textManager;

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

        using (UnityWebRequest www = UnityWebRequest.PostWwwForm(apiUrl, "POST"))
        {
            byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(jsonData);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("accept", "application/json");

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Error: " + www.error);
                ttsManager.ConvertTextToSpeech("Server Error");
            }
            else
            {
                Debug.Log("Response: " + www.downloadHandler.text);
                // Send the response text to the TTS manager
                var json = JObject.Parse(www.downloadHandler.text);
                var npcResponse = json["response"]?.ToString();
                
                // textManager.UpdateText(npcResponse);

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
