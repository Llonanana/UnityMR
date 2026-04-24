using UnityEngine;
using System.Collections;
using System;

public class Talker : MonoBehaviour
{
    public TextToSpeech ttsManager;
    // 在你的 Talker 或 TextToSpeech 腳本中
    private bool isSpeaking = false;
    private void Start()
    {
        if (ttsManager != null)
        {
            // 當 Azure 說出一個詞時，直接交給 SubtitleDisplayManager 的 UpdateLiveSubtitle 處理
            ttsManager.OnWordSpoken += SubtitleDisplayManager.Instance.UpdateLiveSubtitle;
        }
    }
    private void OnDestroy()
    {
        if (ttsManager != null && SubtitleDisplayManager.Instance != null)
        {
            ttsManager.OnWordSpoken -= SubtitleDisplayManager.Instance.UpdateLiveSubtitle;
        }
    }
    // 舊方法（維持原本功能）
    public void Speak(string category, string type)
    {
        if (isSpeaking) return; // 如果正在說話，就直接跳過這次請求
        StartCoroutine(SpeakCoroutine(category, type));
    }

    // 新方法（可以等待語音完成）
    public IEnumerator SpeakCoroutine(string category, string type)
    {
        if (isSpeaking) yield break;
        isSpeaking = true;

        string path = string.IsNullOrEmpty(category) ? $"Dialogues/{type}" : $"Dialogues/{category}/{type}";
        TextAsset txt = Resources.Load<TextAsset>(path);
        if (txt == null) { isSpeaking = false; yield break; }

        // 將文字按行拆分
        string[] lines = txt.text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

        foreach (string line in lines)
        {
            SubtitleDisplayManager.Instance.StartLiveSubtitle();

            bool finished = false;
            Action handleFinished = null;
            handleFinished = () => {
                finished = true;
                ttsManager.OnSpeechCompleted -= handleFinished;
            };
            ttsManager.OnSpeechCompleted += handleFinished;

            // 一次只唸一行
            ttsManager.ConvertTextToSpeech(line);

            // 等這一行唸完
            yield return new WaitUntil(() => finished);

            // 每行之間的停頓感
            yield return new WaitForSeconds(0.5f);
        }

        SubtitleDisplayManager.Instance.HideSubtitle();
        isSpeaking = false;
    }
}