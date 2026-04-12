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

    void Start()
    {
        // 初始狀態
        confirmButton.SetActive(false);
        finalMessageText.gameObject.SetActive(false);
        UpdateConfirmText(0);
    }

    // 綁定在 Slider 的 OnValueChanged 事件
    public void OnSliderValueChanged(float value)
    {
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