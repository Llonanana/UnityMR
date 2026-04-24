using System;
using System.Collections;
using UnityEngine;
using Microsoft.CognitiveServices.Speech;
using System.Threading.Tasks;

public class TextToSpeech : MonoBehaviour
{
    private string apiKey = "4968672a35e040c182e965c879351d64";
    private string region = "eastasia";
    public string voiceName = "en-GB-RyanNeural";

    private SpeechConfig speechConfig;
    private SpeechSynthesizer synthesizer;
    public event Action OnSpeechCompleted; // 語音完成事件
    public event Action<string> OnWordSpoken; // 新增：當一個單詞（或片段）開始播放時觸發，傳出該單詞
    void Start()
    {
        // Initialize the Speech SDK
        speechConfig = SpeechConfig.FromSubscription(apiKey, region);
        speechConfig.SpeechSynthesisVoiceName = voiceName;
        synthesizer = new SpeechSynthesizer(speechConfig);
        
        // --- 重點：訂閱單詞邊界事件 ---
        synthesizer.WordBoundary += (s, e) =>
        {
            // e.Text 是當前正在說的那個單詞
            // 由於這是異步線程，我們需要傳回 Unity 主線程 (如果是簡單顯示可直接 Invoke)
            // 但為了安全，建議在實作中確保 UI 更新在主線程
            OnWordSpoken?.Invoke(e.Text);
        };
        foreach (var device in Microphone.devices)
            Debug.Log("可用麥克風: " + device);
    }

    public void ConvertTextToSpeech(string text)
    {
        StartCoroutine(SpeakText(text));
    }

    IEnumerator SpeakText(string text)
    {
        var task = Task.Run(async () => await synthesizer.SpeakTextAsync(text));
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.Result.Reason == ResultReason.SynthesizingAudioCompleted)
        {
            Debug.Log("Speech synthesis succeeded.");
        }
        else
        {
            Debug.LogError($"Speech synthesis failed. Reason: {task.Result.Reason}");
        }
        // 播放語音時在完成後呼叫：
        OnSpeechCompleted?.Invoke();
    }
}
