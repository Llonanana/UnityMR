using UnityEngine;

public class ProximityTrigger : MonoBehaviour
{
    public GameObject uiPrompt;

    void Start()
    {
        uiPrompt.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {// 階段 0 觸發：啟動開場劇情
            // Debug.Log("ENTER");

            StoryManager.Instance.Notify(EventType.EnterStoryZone);

            ShowPrompt();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Debug.Log("EXIT");

            StoryManager.Instance.Notify(EventType.ExitStoryZone);
        }
    }

    // 新增：讓別的 Script 可以呼叫
    public void ShowPrompt()
    {
        if (uiPrompt != null)
            uiPrompt.SetActive(true);
    }

    // 新增：讓別的 Script 可以呼叫
    public void HidePrompt()
    {
        if (uiPrompt != null)
            uiPrompt.SetActive(false);
    }
}