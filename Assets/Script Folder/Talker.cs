using UnityEngine;
using System.Collections;

public class Talker : MonoBehaviour
{
    public TextToSpeech ttsManager;

    // 舊方法（維持原本功能）
    public void Speak(string category, string type)
    {
        StartCoroutine(SpeakCoroutine(category, type));
    }

    // 新方法（可以等待語音完成）
    public IEnumerator SpeakCoroutine(string category, string type)
    {
        string path = $"Dialogues/{category}/{type}";

        TextAsset txt = Resources.Load<TextAsset>(path);

        string content;

        if (txt != null)
        {
            content = txt.text;
        }
        else
        {
            Debug.LogWarning("找不到對應 txt: " + path);
            content = "找不到對話內容";
        }

        bool finished = false;

        void OnSpeechDone()
        {
            finished = true;
            ttsManager.OnSpeechCompleted -= OnSpeechDone;
        }

        ttsManager.OnSpeechCompleted += OnSpeechDone;

        ttsManager.ConvertTextToSpeech(content);

        yield return new WaitUntil(() => finished);
    }
}