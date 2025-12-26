using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using Microsoft.CognitiveServices.Speech;

public class APIRequestController : MonoBehaviour
{
    public LunarcomController lunarcomController;
    public Text responseText;

    // 設定本機 Docker 網址
    private string apiUrl = "http://localhost:5050/api/npc/ask";
    private string role = "白起";
    public string language = "zh_TW"; 

    // Azure 設定
    public string subscriptionKey = "YourAzureSubscriptionKey";
    public string region = "YourServiceRegion";
    public string voiceName = "zh-TW-HsiaoChenNeural"; 

    public UserInteractionRecorder interactionRecorder;

    // 💥 修正重點：把 void 改回 IEnumerator，讓 Lunarcom 可以正常呼叫！
    public IEnumerator SendRequestToAPI(string query)
    {
        Debug.Log("貓貓修正版 V2：正在發送訊息給 Docker... " + query);

        // 準備資料
        var json = new JObject
        {
            { "query", query },
            { "lang", language },
            { "npc_role", role },
            { "personality", "introverted" },
            { "is_rag", true }
        };

        string jsonData = json.ToString();
        byte[] body = Encoding.UTF8.GetBytes(jsonData);

        // 發送請求
        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(body);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.timeout = 30;

            // 等待伺服器回應 (這就是 Lunarcom 需要等待的部分)
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("貓貓大成功 Response: " + request.downloadHandler.text);
                ProcessResponse(request.downloadHandler.text);
                
                if (interactionRecorder != null)
                    interactionRecorder.RecordInteraction(query);
            }
            else
            {
                Debug.LogError("貓貓連線失敗: " + request.error);
                Debug.LogError("錯誤網址: " + request.url);
                ProcessResponse("伺服器連線失敗，請檢查 Docker 綠燈。");
            }
        }
    }

    // 處理回應並唸出來
    async void ProcessResponse(string response)
    {
        try 
        {
            JObject jsonResponse = JObject.Parse(response);
            if(jsonResponse.ContainsKey("response"))
            {
                response = jsonResponse["response"].ToString();
            }
        }
        catch { }

        Debug.Log("準備說話: " + response);
        if(responseText != null) responseText.text = response;
        await ConvertTextToSpeech(response);
    }

    private async System.Threading.Tasks.Task ConvertTextToSpeech(string text)
    {
        if(string.IsNullOrEmpty(subscriptionKey) || subscriptionKey == "YourAzureSubscriptionKey") 
        {
            return;
        }

        var config = SpeechConfig.FromSubscription(subscriptionKey, region);
        config.SpeechSynthesisVoiceName = voiceName;

        using (var synthesizer = new SpeechSynthesizer(config))
        {
            await synthesizer.SpeakTextAsync(text);
        }
    }
}