using UnityEngine;

public class NPCController : MonoBehaviour
{
    public string npc_role;           // 對應 npc_config.json 的角色名
    public string configFileName;     // 對應 NPC 詳細設定 JSON

    private NPCRequestManager requestManager;

    void Awake()
    {
        requestManager = FindObjectOfType<NPCRequestManager>();
    }

    public void SendQuery(string query)
    {
        // 先讀取該 NPC 的設定
        var npcInfo = requestManager.GetNPCInfoByConfig(configFileName);
        requestManager.SendNPCRequest(query, npcInfo);
    }
}
