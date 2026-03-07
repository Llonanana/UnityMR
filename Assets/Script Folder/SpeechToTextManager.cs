using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.CognitiveServices.Speech;

public class SpeechToTextManager : MonoBehaviour
{
    private string subscriptionKey = "4968672a35e040c182e965c879351d64";
    private string region = "eastasia";
    private SpeechRecognizer recognizer;

    public NPCRequestManager npcRequestManager; // Reference to the NPCRequestManager

    void Start()
    {
        UnityMainThreadDispatcher.Initialize();
        InitializeRecognizer();
    }

    // 初始化 recognizer
    private void InitializeRecognizer()
    {
        if (recognizer != null)
            recognizer.Dispose();

        var config = SpeechConfig.FromSubscription(subscriptionKey, region);

        // 使用統一語言變數
        string lang = string.IsNullOrEmpty(LanguageState.SpeechLang) ? "en-US" : LanguageState.SpeechLang;
        config.SpeechRecognitionLanguage = lang;

        recognizer = new SpeechRecognizer(config);

        Debug.Log($"[STT] Recognizer initialized with language: {lang}");
    }

    // 語言切換
    public void SetLanguage(string newLanguage)
    {
        LanguageState.SpeechLang = newLanguage;
        InitializeRecognizer(); // 重新初始化 recognizer
        Debug.Log($"[STT] Language switched to: {newLanguage}");
    }

    // 按錄音按鈕才呼叫
    public async void StartRecognition()
    {
        if (recognizer == null)
        {
            Debug.LogError("[STT] Recognizer is null. Did you forget to initialize?");
            return;
        }

        Debug.Log("[STT] StartRecognition called");

        var result = await recognizer.RecognizeOnceAsync().ConfigureAwait(false);

        if (result.Reason == ResultReason.RecognizedSpeech)
        {
            Debug.Log($"[STT] Recognized: {result.Text}");
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                if (npcRequestManager != null)
                    npcRequestManager.SendNPCRequest(result.Text);
            });
        }
        else if (result.Reason == ResultReason.NoMatch)
        {
            Debug.Log("[STT] No speech could be recognized.");
        }
        else if (result.Reason == ResultReason.Canceled)
        {
            var cancellation = CancellationDetails.FromResult(result);
            Debug.LogError($"[STT] CANCELED: Reason={cancellation.Reason}");
            if (cancellation.Reason == CancellationReason.Error)
            {
                Debug.LogError($"[STT] CANCELED: ErrorDetails={cancellation.ErrorDetails}");
            }
        }
    }

    void OnDestroy()
    {
        if (recognizer != null)
            recognizer.Dispose();
    }
}
