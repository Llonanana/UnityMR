using UnityEngine;
using TMPro;
using UnityEngine.UI;
// using Microsoft.MixedReality.Toolkit.UX; // MRTK3 UX 命名空間

public class MuseumSurveyController : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI promptText;
    public TextMeshProUGUI confirmText;
    public Slider waitTimeSlider;
    public GameObject confirmButton;
    public TextMeshProUGUI finalMessageText;
    public static int selectedSeconds;

    void Start()
    {
        // 等同學來呼叫
        gameObject.SetActive(false); 
    }

    // ==========================================================
    //  Public Function
    // ==========================================================
    public void StartPhase8Survey()
    {
        // 1. 讓整個問卷面板現身！
        gameObject.SetActive(true);

        // 2. 確保每次打開時，東西都有乖乖歸位（重置狀態）
        promptText.gameObject.SetActive(true);
        waitTimeSlider.gameObject.SetActive(true);
        confirmText.gameObject.SetActive(true);
        
        confirmButton.SetActive(false); // 確認按鈕先藏起來
        finalMessageText.gameObject.SetActive(false); // 感謝詞先藏起來
        
        // 3. 把滑桿歸零
        waitTimeSlider.value = 0;
        UpdateConfirmText(0);

        Debug.Log("同學呼叫成功！第八階段問卷正式啟動！");
    }
    // ==========================================================

    // 綁定在 Slider 的 OnValueChanged 事件
    public void OnSliderValueChanged(float value)
    {
        value = waitTimeSlider.value; // 確保讀到的是滑桿的實際值
        int seconds = Mathf.RoundToInt(value);
        UpdateConfirmText(seconds);
        
        
        // 只要拉動過，就可以顯示確認按鈕
        if (!confirmButton.activeSelf) confirmButton.SetActive(true);
    }

    void UpdateConfirmText(int sec)
    {
        confirmText.text = $"請確認，您選擇了 <color=#FFFF00>{sec}</color> 秒鐘！";
    }

    // 綁定在 ConfirmButton 的 OnClick 事件
    public void OnConfirmClicked()
    {
        // 取得目前秒數
        int seconds = Mathf.RoundToInt(waitTimeSlider.value);

        // 存給 EyeTrackLog 用
        selectedSeconds = seconds;

        Debug.Log("輸出的秒數：" + selectedSeconds);
        // 隱藏所有調查介面
        promptText.gameObject.SetActive(false);
        waitTimeSlider.gameObject.SetActive(false);
        confirmText.gameObject.SetActive(false);
        confirmButton.SetActive(false);

        // 顯示最終感謝詞
        finalMessageText.text = "謝謝您的配合！請您馬上前往入口區通知實驗工作人員並暫時歸還裝置！";
        finalMessageText.gameObject.SetActive(true);
    }
}