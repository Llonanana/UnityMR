using UnityEngine;
using System.Collections.Generic;

public class TableTrigger : MonoBehaviour
{
    public Talker npc;

    // 記錄每個 TableItem 上次觸發時間
    private Dictionary<TableItem, float> lastTriggerTime = new Dictionary<TableItem, float>();

    public float triggerCooldown = 2f; // 冷卻時間，秒

    private void OnTriggerEnter(Collider other)
    {
        TableItem item = other.GetComponent<TableItem>();
        if (item == null) return;

        // 檢查冷卻時間
        if (lastTriggerTime.TryGetValue(item, out float lastTime))
        {
            if (Time.time - lastTime < triggerCooldown)
            {
                // 還沒超過冷卻，忽略這次碰撞
                return;
            }
        }

        // 更新上次觸發時間
        lastTriggerTime[item] = Time.time;

        // 觸發事件
        switch (item.itemType)
        {
            case ItemType.LianHuaWan:
                Debug.Log("蓮花碗放到桌上了！");
                npc.Speak("LianHuaWan");
                break;

            case ItemType.ZhiChuiPing:
                Debug.Log("紙槌瓶放到桌上了！");
                npc.Speak("ZhiChuiPing");
                break;
        }
    }
}
