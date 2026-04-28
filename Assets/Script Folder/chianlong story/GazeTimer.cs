using UnityEngine;
using UnityEngine.UI;

public class GazeTimer : MonoBehaviour
{
    public float requiredTime = 5.0f;
    private float timer = 0f;
    private bool isGazing = false;

    [Header("UI 視覺回饋")]
    public Image fillImage; // 拖入一個 Circle Image (Filled)

    void Update()
    {
        if (isGazing)
        {
            timer += Time.deltaTime;
            
            if (fillImage != null)
                fillImage.fillAmount = timer / requiredTime;

            if (timer >= requiredTime)
            {
                TriggerSuccess();
            }
        }
    }

    // 由 MRTK3 的事件系統呼叫
    public void StartGaze()
    {
        isGazing = true;
    }

    // 由 MRTK3 的事件系統呼叫
    public void StopGaze()
    {
        isGazing = false;
        timer = 0;
        if (fillImage != null) fillImage.fillAmount = 0;
    }

    private void TriggerSuccess()
    {
        isGazing = false;
        if (StoryManager.Instance != null) StoryManager.Instance.Notify(EventType.LookBowlSuccess);
        
        Debug.Log("凝視成功！");
        // 成功後關閉腳本，避免重複觸發
        this.enabled = false;
    }
}