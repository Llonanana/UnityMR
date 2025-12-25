using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NPCRequestManager : MonoBehaviour
{
    private string apiUrl = "http://140.119.19.21:3000/NPC";
    public string language = "en_US";
    public string role = "古代中國官員";
    public TextToSpeech ttsManager;
    public NPCInteractionRecorder npcInteractionRecoreder;

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
            npc_role = role
        };

        string jsonData = JsonUtility.ToJson(jsonBody);

        using UnityWebRequest www = UnityWebRequest.PostWwwForm(apiUrl, "POST");
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
            ttsManager.ConvertTextToSpeech(www.downloadHandler.text);
            //Record the Interaction
            if (npcInteractionRecoreder != null)
            {
                npcInteractionRecoreder.RecordInteraction(query);
            }
        }
    }

    [System.Serializable]
    public class NPCRequest
    {
        public string query;
        public string lang;
        public string npc_role;
    }
}
