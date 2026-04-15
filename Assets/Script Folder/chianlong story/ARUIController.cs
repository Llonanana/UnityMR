using UnityEngine;
using UnityEngine.UI;

public class ARUIController : MonoBehaviour
{
    public Button startButton;        // 階段 0
    public Button foundBowlButton;    // 階段 2
    public Button endExperienceButton; // 階段 7

    // 【對應原 ProximityTrigger】階段 0：開始體驗
    public void OnStartExperience()
    {
        if (ARStoryManager.Instance != null)
            ARStoryManager.Instance.Notify(EventType.EnterStoryZone);
        
        startButton.gameObject.SetActive(false);
    }

    // 【對應原 GrabDistanceChecker】階段 2：找到了
    public void OnFoundBowlClicked()
    {
        if (ARStoryManager.Instance != null)
            ARStoryManager.Instance.Notify(EventType.PutBowlSuccess);
        
        // 成功觸發後隱藏按鈕，避免重複點擊
        if (foundBowlButton != null)
            foundBowlButton.gameObject.SetActive(false);
    }

    // 【對應原 TouchReaction】階段 7：結束體驗
    public void OnEndExperienceClicked()
    {
        if (ARStoryManager.Instance != null)
            ARStoryManager.Instance.Notify(EventType.PutBowlBackSuccess);
        
        endExperienceButton.gameObject.SetActive(false);
    }
}