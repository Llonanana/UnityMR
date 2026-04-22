using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MuseumSurveyController : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI promptText;
    public TextMeshProUGUI confirmText;
    public GameObject confirmButton;
    public TextMeshProUGUI finalMessageText;
    
    // 用來放那 7 個按鈕的父物件 (就是你的 ButtonGroup)
    public GameObject buttonGroup; 

    // 儲存目前選中的秒數
    private int currentSelectedSeconds = 0;
    public static int selectedSeconds;

    void Start()
    {
        // 初始隱藏，等 StoryManager 呼叫
        gameObject.SetActive(false); 
    }

    // ==========================================================
    //  Public Function (供 StoryManager 呼叫)
    // ==========================================================
    public void StartPhase8Survey()
    {
        gameObject.SetActive(true);

        // 重置所有 UI 狀態
        promptText.gameObject.SetActive(true);
        if (buttonGroup != null) buttonGroup.SetActive(true);
        confirmText.gameObject.SetActive(true);
        
        confirmButton.SetActive(false); // 還沒選秒數前先藏起來
        finalMessageText.gameObject.SetActive(false);
        
        currentSelectedSeconds = 0;
        UpdateConfirmText(0);

        Debug.Log("第八階段問卷啟動：已切換為按鈕模式！");
    }

    // ==========================================================
    //  按鈕點擊事件 (請在 Unity 裡面設定參數 0, 30, 60...)
    // ==========================================================
    public void OnTimeButtonClicked(int seconds)
    {
        currentSelectedSeconds = seconds;
        UpdateConfirmText(seconds);
        
        // 只要有點選過秒數，就顯示確認按鈕
        if (!confirmButton.activeSelf) confirmButton.SetActive(true);
        
        Debug.Log($"使用者選了：{seconds} 秒");
    }

    void UpdateConfirmText(int sec)
    {
        confirmText.text = $"請確認，您選擇了 <color=#FFFF00>{sec}</color> 秒鐘！";
    }

    // 綁定在 ConfirmButton 的 OnClick 事件
    public void OnConfirmClicked()
    {
        // 將選擇的結果存入靜態變數供 Log 使用
        selectedSeconds = currentSelectedSeconds;

        Debug.Log("最終存入的秒數：" + selectedSeconds);

        // 隱藏問卷介面
        promptText.gameObject.SetActive(false);
        if (buttonGroup != null) buttonGroup.SetActive(false);
        confirmText.gameObject.SetActive(false);
        confirmButton.SetActive(false);

        // 顯示最終感謝詞
        finalMessageText.text = "謝謝您的配合！請您馬上前往入口區通知實驗工作人員並暫時歸還裝置！";
        finalMessageText.gameObject.SetActive(true);
    }
}