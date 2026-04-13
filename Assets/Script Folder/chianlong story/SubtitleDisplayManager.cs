using UnityEngine;
using TMPro;

public class SubtitleDisplayManager : MonoBehaviour
{
    public static SubtitleDisplayManager Instance;
    [Header("文字螢幕連連看")]
    public TextMeshProUGUI subtitleText;
    public TextMeshProUGUI hintText;
    public TextMeshProUGUI taskText; // 👈 新增任務文字螢幕

    [Header("面板抽屜連連看")]
    public GameObject subtitlePanel;
    public GameObject hintPanel;
    public GameObject taskPanel; // 👈 新增任務面板抽屜

    void Awake()
    {
        Instance = this;
    }
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
    // 用法範例：
    // SubtitleDisplayManager.Instance.DisplayTask("task_01");

    // AR組新增功能：借用hintPanel直接印出提示文字
    public void DisplayHintText(string content)
    {
        hintPanel.SetActive(true);
        hintText.text = content;
    }
    
    public void DisplayStory(string fileName)
    {
        subtitlePanel.SetActive(true);

        string content =
            LoadText("stories", fileName);

        subtitleText.text = content;
    }

    public void DisplayHint(string fileName)
    {
        hintPanel.SetActive(true);

        string content =
            LoadText("hints", fileName);

        hintText.text = content;
    }

    public void DisplayTask(string fileName)
    {
        taskPanel.SetActive(true);

        string content =
            LoadText("tasks", fileName);

        taskText.text = content;
    }
    public void HideSubtitle()
    {
        subtitlePanel.SetActive(false);
    }

    public void HideHint()
    {
        hintPanel.SetActive(false);
    }

    public void HideTask()
    {
        taskPanel.SetActive(false);
    }
    string LoadText(string folderName, string fileName)
    {
        string path = "Dialogues/" + folderName + "/" + fileName;

        TextAsset textFile =
            Resources.Load<TextAsset>(path);

        if (textFile == null)
        {
            Debug.LogError("找不到檔案: " + path);
            return "文字檔不存在";
        }

        return textFile.text;
    }
}