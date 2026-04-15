using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class FloatingPickupItem : MonoBehaviour
{
    Rigidbody rb;
    XRGrabInteractable grab;
    Renderer[] renderers;

    [Header("漂浮設定")]
    public float floatSpeed = 2f;
    public float floatHeight = 0.2f;

    Vector3 startPos;
    bool isFloating = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        grab = GetComponent<XRGrabInteractable>();

        renderers = GetComponentsInChildren<Renderer>();

        rb.isKinematic = true;
        rb.useGravity = false;

        startPos = transform.position;

        grab.selectEntered.AddListener(OnGrab);
        grab.selectExited.AddListener(OnRelease);

        HideItem();
    }

    void Update()
    {
        if (!isFloating) return;

        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        rb.MovePosition(new Vector3(startPos.x, newY, startPos.z));
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        StopFloating();

        // 🔥 關鍵：避免 XR 干擾動畫系統
        rb.isKinematic = false;
        rb.useGravity = true;
    }

    void OnRelease(SelectExitEventArgs args)
    {
        EnablePhysics();
    }

    public void StopFloating()
    {
        isFloating = false;
    }

    public void LockForAnimation()
    {
        rb.isKinematic = true;
        rb.useGravity = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // 🔥 關鍵：避免 XR 再控制 transform
        grab.enabled = false;
    }

    public void UnlockFromAnimation()
    {
        grab.enabled = true;
    }

    void EnablePhysics()
    {
        rb.isKinematic = false;
        rb.useGravity = true;
    }

    public void ShowItem()
    {
        foreach (Renderer r in renderers)
            r.enabled = true;
    }

    public void HideItem()
    {
        foreach (Renderer r in renderers)
            r.enabled = false;
    }
}