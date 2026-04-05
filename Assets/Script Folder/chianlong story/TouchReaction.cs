using UnityEngine;

public class TouchReaction : MonoBehaviour
{
    // 當有物件碰撞到此物件時執行
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("碰到了：" + gameObject.name);
            // 範例：改變顏色或播放特效
        }
    }
}