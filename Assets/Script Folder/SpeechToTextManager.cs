using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.CognitiveServices.Speech;
using TMPro;

public class SpeechToTextManager : MonoBehaviour
{
    private string subscriptionKey = "4968672a35e040c182e965c879351d64";
    private string region = "eastasia";
    public string fromLanguage;
    private SpeechRecognizer recognizer;
    // public TextMeshProUGUI resultText;
    public NPCRequestManager npcRequestManager; // Reference to the NPCRequestManager

    void Start()
    {
        UnityMainThreadDispatcher.Initialize();
        InitializeRecognizer();
    }

    private void InitializeRecognizer()
    {
        if (recognizer != null)
        {
            recognizer.Dispose();
        }

        var config = SpeechConfig.FromSubscription(subscriptionKey, region);
        config.SpeechRecognitionLanguage = fromLanguage;
        recognizer = new SpeechRecognizer(config);
    }

    public async void StartRecognition()
    {
        InitializeRecognizer(); // Ensure the recognizer is initialized with the latest language setting

        var result = await recognizer.RecognizeOnceAsync().ConfigureAwait(false);

        if (result.Reason == ResultReason.RecognizedSpeech)
        {
            Debug.Log($"Recognized: {result.Text}");
            // resultText.text = result.Text;
            UnityMainThreadDispatcher.Instance().Enqueue(() => npcRequestManager.SendNPCRequest(result.Text)); // Send the recognized text to NPC API
        }
        else if (result.Reason == ResultReason.NoMatch)
        {
            Debug.Log("No speech could be recognized.");
            // resultText.text = "No speech could be recognized.";
        }
        else if (result.Reason == ResultReason.Canceled)
        {
            var cancellation = CancellationDetails.FromResult(result);
            Debug.Log($"CANCELED: Reason={cancellation.Reason}");

            if (cancellation.Reason == CancellationReason.Error)
            {
                Debug.Log($"CANCELED: ErrorDetails={cancellation.ErrorDetails}");
                // resultText.text = $"Error: {cancellation.ErrorDetails}";
            }
        }
    }

    void OnDestroy()
    {
        if (recognizer != null)
        {
            recognizer.Dispose();
        }
    }
}
