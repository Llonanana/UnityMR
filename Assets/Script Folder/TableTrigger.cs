using UnityEngine;

public class TableTrigger : MonoBehaviour
{
    public Talker npc;
    private void OnTriggerEnter(Collider other)
    {
        TableItem item = other.GetComponent<TableItem>();
        if (item == null) return;

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
