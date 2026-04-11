using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class FloatingPickupItem : MonoBehaviour
{
    Rigidbody rb;
    Renderer[] renderers;
    XRGrabInteractable grab;

    [Header("漂浮設定")]
    public float floatSpeed;
    public float floatHeight;

    Vector3 startPos;
    bool isFloating = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        grab = GetComponent<XRGrabInteractable>();

        // 初始：漂浮
        rb.isKinematic = true;
        rb.useGravity = false;

        startPos = transform.position;

        // 監聽抓取
        grab.selectEntered.AddListener(OnGrab);

        // 監聽放手
        grab.selectExited.AddListener(OnRelease);

        renderers = GetComponentsInChildren<Renderer>();
        HideItem(); // 一開始隱形
    }

    void Update()
    {
        if (!isFloating)
            return;

        float newY = startPos.y +
            Mathf.Sin(Time.time * floatSpeed) * floatHeight;

        rb.MovePosition(new Vector3(startPos.x, newY, startPos.z));
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        // 停止漂浮（但還不開物理）
        StopFloating();
    }

    void OnRelease(SelectExitEventArgs args)
    {
        // 放手後才變正常物理物件
        EnablePhysics();
    }

    void StopFloating()
    {
        isFloating = false;
    }

    void EnablePhysics()
    {
        rb.isKinematic = false;
        rb.useGravity = true;
    }

    public void ShowItem()
    {
        foreach (Renderer r in renderers)
        {
            r.enabled = true;
        }
    }

    public void HideItem()
    {
        foreach (Renderer r in renderers)
        {
            r.enabled = false;
        }
    }
}