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

    void Start()
    {
        // Initialize the Speech SDK
        speechConfig = SpeechConfig.FromSubscription(apiKey, region);
        speechConfig.SpeechSynthesisVoiceName = voiceName;
        synthesizer = new SpeechSynthesizer(speechConfig);
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
    }
}
