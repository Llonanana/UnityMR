using UnityEngine;

public class ProximityTrigger : MonoBehaviour
{
    public GameObject uiPrompt; // 拖入你想顯示的 UI 物件

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StoryManager.Instance.Notify(EventType.EnterStoryZone);
            if (uiPrompt != null) uiPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            uiPrompt.SetActive(false); // 離開隱藏
        }
    }
}