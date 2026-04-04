using UnityEngine;
using TMPro;

public class SubtitleDisplayManager : MonoBehaviour
{
    [Header("文字螢幕連連看")]
    public TextMeshProUGUI subtitleText;
    public TextMeshProUGUI hintText;
    public TextMeshProUGUI taskText; // 👈 新增任務文字螢幕

    [Header("面板抽屜連連看")]
    public GameObject subtitlePanel;
    public GameObject hintPanel;
    public GameObject taskPanel; // 👈 新增任務面板抽屜

    void Start()
    {
        HideAll(); 
    }

    public void HideAll()
    {
        if (subtitlePanel != null) subtitlePanel.SetActive(false);
        if (hintPanel != null) hintPanel.SetActive(false);
        if (taskPanel != null) taskPanel.SetActive(false); // 👈 關閉任務面板
    }

    // --- 這是給同學（StoryManager）呼叫的神祕門 ---

    public void DisplaySubtitle(string content)
    {
        HideAll();
        subtitlePanel.SetActive(true);
        subtitleText.text = content;
    }

    public void DisplayHint(string content)
    {
        HideAll();
        hintPanel.SetActive(true);
        hintText.text = content;
    }

    // 👈 當同學讀到「任務內容」時，呼叫這扇門
    public void DisplayTask(string content)
    {
        HideAll();
        taskPanel.SetActive(true);
        taskText.text = content;
    }
}