using UnityEngine;

public class TouchReaction : MonoBehaviour
{
    // 在 Inspector 視窗選取這個觸碰動作對應的事件類型
    public EventType reactionEvent = EventType.GazeBowlCloseSuccess;

    // 當有物件碰撞到此物件時執行
    private void OnCollisionEnter(Collision collision)
    {
        // 檢查碰撞到的物件是否標記為 Player
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("碰到了：" + gameObject.name + "，觸發事件：" + reactionEvent);

            // --- 關鍵呼叫：通知劇本管理器 ---
            if (StoryManager.Instance != null)
            {
                StoryManager.Instance.Notify(reactionEvent);
            }
            else
            {
                Debug.LogError("找不到 StoryManager 實例！請確認場景中有 StorySystem。");
            }
            
            // 範例：改變顏色代表觸發成功
            GetComponent<Renderer>().material.color = Color.green;
        }
    }
}