using UnityEngine;

public class TouchReaction : MonoBehaviour
{
    // 在 Inspector 視窗選取這個觸碰動作對應的事件類型
    public EventType reactionEvent = EventType.GazeBowlCloseSuccess;

/* * ============================================================
     * 【各階段設定指南】請在 Inspector 的 reactionEvent 下拉選單選擇：
     * ============================================================
     * * 階段 4 [任務成功]：PutBottleIntoBowlSuccess
     * -> 物件：請掛在「乾隆手中的溫碗」感應區。
     * -> 功能：偵測玩家是否將酒瓶放入碗中。
     * * 階段 5 [任務成功]：GazeBowlCloseSuccess
     * -> 物件：請掛在「實體展櫃前方 50cm」的隱形區域。
     * -> 功能：偵測玩家是否靠近欣賞（乾隆會接著解說底部）。
     * * 階段 7 [玩家完成]：PutBowlBackSuccess
     * -> 物件：請掛在「貨櫃內原本放置溫碗」的位置。
     * -> 功能：偵測玩家是否將文物放回原位（結束體驗）。
     * * ============================================================
     */


    // 當有物件碰撞到此物件時執行
    private void OnTriggerEnter(Collider other)
    {
        // 檢查碰撞到的物件是否標記為 Player
        if (other.CompareTag("Player"))
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