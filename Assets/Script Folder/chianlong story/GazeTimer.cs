using UnityEngine;
using UnityEngine.UI; // 如果要用 Image 做進度條可加此行

public class GazeTimer : MonoBehaviour
{
    public float requiredTime = 2.0f;
    private float timer = 0f;
    private bool isGazing = false;
    public Transform cameraTransform;

    void Update()
    {
        if (isGazing)
        {
            timer += Time.deltaTime;
            if (timer >= requiredTime)
            {
                TriggerAction();
                timer = 0;
            }
        }
    }

    // 這些方法會被 Camera 上的 Raycaster 呼叫
    public void StartGaze() { isGazing = true; }
    public void StopGaze() { isGazing = false; timer = 0; }

    void TriggerAction()
    {
        Debug.Log(gameObject.name + " 凝視觸發成功！");
    }
}