using UnityEngine;

public class Talker : MonoBehaviour
{
    public TextToSpeech ttsManager;
    public void Speak(string type)
    {
        // 動態產生路徑
        string path = "Dialogues/" + type; // 對應 Resources/Dialogues/A_Type1.txt

        TextAsset txt = Resources.Load<TextAsset>(path);

        if (txt != null)
        {
            Debug.Log(txt.text);
            ttsManager.ConvertTextToSpeech(txt.text);
        }
        else
        {
            Debug.LogWarning("找不到對應的 txt: " + path);
            ttsManager.ConvertTextToSpeech("找不到對應的對話內容");
        }
    }
}
