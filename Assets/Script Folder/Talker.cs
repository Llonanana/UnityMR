using UnityEngine;
using System.Collections;
using System;

public class Talker : MonoBehaviour
{
    public TextToSpeech ttsManager;
    // 在你的 Talker 或 TextToSpeech 腳本中
    // private bool isSpeaking = false;
    private static bool isGlobalSpeaking = false;
    // private void Start()
    // {
    //     if (ttsManager != null)
    //     {
    //         // 當 Azure 說出一個詞時，直接交給 SubtitleDisplayManager 的 UpdateLiveSubtitle 處理
    //         ttsManager.OnWordSpoken += SubtitleDisplayManager.Instance.UpdateLiveSubtitle;
    //     }
    // }
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
        if (isGlobalSpeaking) 
        {
            Debug.LogWarning("[Talker] 正在說話中，拒絕新的請求：" + type);
            return;
        }
        StartCoroutine(SpeakCoroutine(category, type));
    }

    // 新方法（可以等待語音完成）
    public IEnumerator SpeakCoroutine(string category, string type)
    {
        isGlobalSpeaking = true;

        string path = string.IsNullOrEmpty(category) ? $"Dialogues/{type}" : $"Dialogues/{category}/{type}";
        TextAsset txt = Resources.Load<TextAsset>(path);
        if (txt == null) { isGlobalSpeaking = false; yield break; }
        
        string[] lines = txt.text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

        foreach (string line in lines)
        {
            string cleanLine = line.Trim();

            // --- 重點修復：根據種類決定顯示在哪個面板 ---
            if (category == "tasks") {
                // 如果是任務，只顯示任務面板，關閉對話面板
                SubtitleDisplayManager.Instance.HideSubtitle();
                SubtitleDisplayManager.Instance.taskPanel.SetActive(true);
                SubtitleDisplayManager.Instance.taskText.text = cleanLine;
            } else {
                // 如果是普通對話
                SubtitleDisplayManager.Instance.subtitlePanel.SetActive(true);
                SubtitleDisplayManager.Instance.subtitleText.text = cleanLine;
            }

            bool currentLineFinished = false;
            Action onDone = null;
            onDone = () => {
                currentLineFinished = true;
                ttsManager.OnSpeechCompleted -= onDone;
            };
            ttsManager.OnSpeechCompleted += onDone;

            ttsManager.ConvertTextToSpeech(cleanLine);

            while (!currentLineFinished) { yield return null; }
            yield return new WaitForSeconds(0.4f);
        }

        // 只有非任務類的字幕才自動隱藏，任務面板通常建議留著
        if (category != "tasks") {
            SubtitleDisplayManager.Instance.HideSubtitle();
        }
        
        isGlobalSpeaking = false; 
    }
}