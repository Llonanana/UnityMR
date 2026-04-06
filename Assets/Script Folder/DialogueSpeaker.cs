using UnityEngine;

public class DialogueSpeaker : MonoBehaviour
{
    [Header("Talker 組件")]
    public Talker talker;   // 在 Inspector 裡拖曳 Talker 進來

    [Header("要念的檔案名稱 (不含副檔名)")]
    public string fileName = "story1-1"; // 對應 Resources/Dialogues/story1-1.txt

    void Start()
    {
        if (talker != null)
        {
            talker.Speak(fileName);
        }
        else
        {
            Debug.LogError("DialogueSpeaker 沒有連結到 Talker！");
        }
    }
}
