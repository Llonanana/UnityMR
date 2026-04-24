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
    private string _targetText = "";
    private bool _shouldUpdateUI = false;
    public float typeSpeed = 0.05f; // 每個字出現的間隔時間（秒）

    private Coroutine taskTypewriterCoroutine;
    void Awake() { Instance = this; }
    void Start() { HideAll(); }

    void Update()
    {
        // 在 Update 裡更新 UI，這保證是在 Unity 主執行緒執行
        if (_shouldUpdateUI)
        {
            subtitleText.text = _targetText;
            _shouldUpdateUI = false;
        }
    }
    public void HideAll() {
        if (subtitlePanel != null) subtitlePanel.SetActive(false);
        if (hintPanel != null) hintPanel.SetActive(false);
        if (taskPanel != null) taskPanel.SetActive(false);
    }

    // AR組新增功能：借用hintPanel直接印出提示文字
    public void DisplayHintText(string content)
    {
        hintPanel.SetActive(true);
        hintText.text = content;
    }

    // 2. 提示文字
    public void DisplayHint(string fileName)
    {
        hintPanel.SetActive(true);

        string content =
            LoadText("hints", fileName);

        hintText.text = content;
    }
    // 3. 任務文字
    public void DisplayTask(string fileName) {
        if (taskPanel != null) taskPanel.SetActive(true);
        
        // 1. 讀取文字
        string content = LoadText("tasks", fileName);
        
        // 2. 如果之前有正在跑的打字機，先停止它
        if (taskTypewriterCoroutine != null) StopCoroutine(taskTypewriterCoroutine);
        
        // 3. 開始逐字顯示
        taskTypewriterCoroutine = StartCoroutine(TypeText(taskText, content));
    }
    IEnumerator TypeText(TextMeshProUGUI uiText, string fullText) {
        uiText.text = ""; // 先清空文字
        
        // 逐個字元跑迴圈
        foreach (char letter in fullText.ToCharArray()) {
            uiText.text += letter; // 加上一個字
            
            // 這裡可以選擇是否跳過空白字元不等待
            yield return new WaitForSeconds(typeSpeed);
        }
        
        taskTypewriterCoroutine = null;
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



    // 新增：供 Azure API 調用的即時更新方法
    // 專供即時語音使用的顯示方法
    public void StartLiveSubtitle()
    {
        _targetText = "";
        subtitleText.text = "";
        subtitlePanel.SetActive(true);
    }

    // 提供給外部（Azure 事件）呼叫
    public void UpdateLiveSubtitle(string word)
    {
        // 直接累加，不要加 " "
        _targetText += word; 
        _shouldUpdateUI = true;
    }

    // 語音結束時呼叫
    public void OnSynthesisFinished()
    {
        StartCoroutine(FadeOutSubtitle(2.0f));
    }

    IEnumerator FadeOutSubtitle(float delay)
    {
        yield return new WaitForSeconds(delay);
        subtitlePanel.SetActive(false);
    }
}