using UnityEngine;
using System.Collections;

public class Talker : MonoBehaviour
{
    public TextToSpeech ttsManager;
    // 在你的 Talker 或 TextToSpeech 腳本中
    private bool isSpeaking = false;

    // 舊方法（維持原本功能）
    public void Speak(string category, string type)
    {
        if (isSpeaking) return; // 如果正在說話，就直接跳過這次請求
        StartCoroutine(SpeakCoroutine(category, type));
    }

    // 新方法（可以等待語音完成）
    public IEnumerator SpeakCoroutine(string category, string type)
    {
        isSpeaking = true;
        // 如果 category 是空字串，直接從 Dialogues 根目錄載入（例如 LianHuaWan.txt）
        string path = string.IsNullOrEmpty(category)
            ? $"Dialogues/{type}"
            : $"Dialogues/{category}/{type}";

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
        isSpeaking = false;
    }
}