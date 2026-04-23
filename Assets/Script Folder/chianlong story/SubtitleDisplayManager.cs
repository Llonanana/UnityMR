using UnityEngine;
using TMPro;
using System.Collections;

public class SubtitleDisplayManager : MonoBehaviour
{
    public static SubtitleDisplayManager Instance;

    [Header("UI 引用")]
    public TextMeshProUGUI subtitleText;
    public TextMeshProUGUI hintText;
    public TextMeshProUGUI taskText;

    [Header("面板引用")]
    public GameObject subtitlePanel;
    public GameObject hintPanel;
    public GameObject taskPanel;

    private Coroutine subtitleCoroutine;
    private Coroutine hintCoroutine;

    void Awake() { Instance = this; }
    void Start() { HideAll(); }

    public void HideAll() {
        if (subtitlePanel != null) subtitlePanel.SetActive(false);
        if (hintPanel != null) hintPanel.SetActive(false);
        if (taskPanel != null) taskPanel.SetActive(false);
    }

    // 1. 故事字幕
    public void DisplayStory(string fileName, float totalAudioTime = -1f) {
        if (subtitleCoroutine != null) StopCoroutine(subtitleCoroutine);
        string content = LoadText("stories", fileName);
        subtitleCoroutine = StartCoroutine(ShowTextByLines(subtitlePanel, subtitleText, content, totalAudioTime));
    }

    // 2. 提示文字 (修正名字為 DisplayHintText，解決 999+ 紅字！)
    public void DisplayHintText(string fileName) {
        if (hintCoroutine != null) StopCoroutine(hintCoroutine);
        string content = LoadText("hints", fileName);
        hintCoroutine = StartCoroutine(ShowTextByLines(hintPanel, hintText, content, -1f));
    }

    // 為了預防萬一，留一個沒有 Text 結尾的版本
    public void DisplayHint(string fileName) { DisplayHintText(fileName); }

    // 3. 任務文字
    public void DisplayTask(string fileName) {
        if (taskPanel != null) taskPanel.SetActive(true);
        string content = LoadText("tasks", fileName);
        taskText.text = content;
    }

    IEnumerator ShowTextByLines(GameObject panel, TextMeshProUGUI textUI, string fullText, float totalTime) {
        panel.SetActive(true);
        string[] lines = fullText.Split(new[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.RemoveEmptyEntries);

        float waitTime = (totalTime > 0) ? (totalTime / lines.Length) : 3.0f;

        foreach (string line in lines) {
            textUI.text = line;
            yield return new WaitForSeconds(waitTime);
        }
    }

    public void HideSubtitle() { if (subtitlePanel != null) subtitlePanel.SetActive(false); }
    public void HideHint() { if (hintPanel != null) hintPanel.SetActive(false); }
    public void HideTask() { if (taskPanel != null) taskPanel.SetActive(false); }

    string LoadText(string folderName, string fileName) {
        string path = "Dialogues/" + folderName + "/" + fileName;
        TextAsset textFile = Resources.Load<TextAsset>(path);
        if (textFile == null) return "File missing: " + path;
        return textFile.text;
    }
}