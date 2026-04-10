using UnityEngine;

public class TouchReaction : MonoBehaviour
{
    [Header("偵測對象設定")]
    [Tooltip("勾選後：偵測標籤為 Player 的物件 (用於階段 5、7)\n不勾選：偵測名為 '酒壺' 的物件 (用於階段 4)")]
    public bool detectPlayer = true;

    [Header("階段事件設定")]
    [Tooltip("請根據掛載物件的階段選擇對應事件")]
    public EventType reactionEvent;

    /* * 階段標註說明：
     * ------------------------------------------------------------
     * 【階段 4】酒瓶入碗：選 PutBottleIntoBowlSuccess (detectPlayer 設為 false)
     * 【階段 5】靠近欣賞：選 GazeBowlCloseSuccess      (detectPlayer 設為 true)
     * 【階段 7】體驗結束：選 PutBowlBackSuccess       (detectPlayer 設為 true)
     * ------------------------------------------------------------
     */

    private void OnTriggerEnter(Collider other)
    {
        bool isTarget = false;

        if (detectPlayer)
        {
            // 模式 A：偵測玩家 (標籤為 Player)
            isTarget = other.CompareTag("Player");
        }
        else
        {
            // 模式 B：偵測酒瓶 (名稱為 酒壺 或 標籤為 Bottle)
            isTarget = (other.name == "酒壺" || other.CompareTag("Bottle"));
        }

        if (isTarget)
        {
            Debug.Log($"<color=cyan>【觸發成功】</color> {gameObject.name} 偵測到 {other.name}，執行：{reactionEvent}");

            if (StoryManager.Instance != null)
            {
                StoryManager.Instance.Notify(reactionEvent);
                
                // 成功回饋：變色
                Renderer rend = GetComponent<Renderer>();
                if (rend != null) rend.material.color = Color.green;
            }
            else
            {
                Debug.LogError("找不到 StoryManager！請確認場景中有 StorySystem。");
            }
        }
    }
}