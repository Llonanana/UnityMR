// using UnityEngine;

// public class Talker : MonoBehaviour
// {
//     public TextToSpeech ttsManager;
//     public void Speak(string type)
//     {
//         // 動態產生路徑
//         string path = "Dialogues/" + type; // 對應 Resources/Dialogues/A_Type1.txt

//         TextAsset txt = Resources.Load<TextAsset>(path);

//         if (txt != null)
//         {
//             Debug.Log(txt.text);
//             ttsManager.ConvertTextToSpeech(txt.text);
//         }
//         else
//         {
//             Debug.LogWarning("找不到對應的 txt: " + path);
//             ttsManager.ConvertTextToSpeech("找不到對應的對話內容");
//         }
//     }
// }

using UnityEngine;
using System.Collections;

public class Talker : MonoBehaviour
{
    public TextToSpeech ttsManager;

    // 舊方法（維持原本功能）
    public void Speak(string type)
    {
        StartCoroutine(SpeakCoroutine(type));
    }

    // 新方法（可以等待語音完成）
    public IEnumerator SpeakCoroutine(string type)
    {
        // 讀取 txt
        string path = "Dialogues/" + type;

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

        // ⭐ 訂閱語音完成事件
        void OnSpeechDone()
        {
            finished = true;
            ttsManager.OnSpeechCompleted -= OnSpeechDone;
        }

        ttsManager.OnSpeechCompleted += OnSpeechDone;

        // 開始說話
        ttsManager.ConvertTextToSpeech(content);

        // ⭐ 等語音播完
        yield return new WaitUntil(() => finished);
    }
}