using UnityEngine;
using MixedReality.Toolkit.SpatialManipulation; // 必須引用 MRTK 命名空間

public class GrabDistanceChecker : MonoBehaviour
{
    public Transform playerCamera; // 拖入 Main Camera

    // 這個方法會在被抓取時由 Event 呼叫
    public void CheckDistanceOnGrab()
    {
        float distance = Vector3.Distance(playerCamera.position, transform.position);
        
        if (distance > 0.5f) // 如果超過 50 公分
        {
            Debug.Log("太遠了！距離為: " + distance);
            StoryManager.Instance.Notify(EventType.PutBowlFailed);
        }
    }
}