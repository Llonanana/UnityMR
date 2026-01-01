using UnityEngine;


public class NPCController : MonoBehaviour
{
    public string configFileName; // qianlong.json / huizong.json

    private NPCRequestManager requestManager;

    void Awake()
    {
        requestManager = FindObjectOfType<NPCRequestManager>();
    }

    public void SendQuery(string query)
    {
        var npcInfo = requestManager.GetNPCInfoByConfig(configFileName);
        requestManager.SendNPCRequest(query, npcInfo);
    }
}
