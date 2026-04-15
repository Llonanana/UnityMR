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
    public string tableTag = "Table"; // 👈 新增：桌子的 Tag 標籤

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

        grab.selectEntered.AddListener(OnGrab);
        grab.selectExited.AddListener(OnRelease);

        renderers = GetComponentsInChildren<Renderer>();
        //HideItem();
    }

    void Update()
    {
        if (!isFloating)
            return;

        // 使用 Sin 波讓物件在 startPos 周圍上下漂浮
        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        rb.MovePosition(new Vector3(startPos.x, newY, startPos.z));
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        StopFloating();
    }

    void OnRelease(SelectExitEventArgs args)
    {
        // 👈 修改：放手後不要開啟物理，而是讓它在「目前放手的位置」繼續漂浮
        ResumeFloatingAtCurrentPosition();
    }

    // 👈 新增：偵測碰撞
    private void OnCollisionEnter(Collision collision)
    {
        // 如果碰到了標籤為 Table 的物件，才恢復正常物理
        if (collision.gameObject.CompareTag(tableTag))
        {
            StopFloating();
            EnablePhysics();
            Debug.Log("[FloatingItem] 碰到桌子，恢復物理狀態");
        }
    }

    void ResumeFloatingAtCurrentPosition()
    {
        // 更新漂浮的出發點為當前位置，並重新開啟 Update 裡的位移邏輯
        startPos = transform.position;
        isFloating = true;
        rb.isKinematic = true;
        rb.useGravity = false;
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
    public void UnablePhysics()
    {
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    public void ShowItem()
    {
        foreach (Renderer r in renderers) r.enabled = true;
    }

    public void HideItem()
    {
        foreach (Renderer r in renderers) r.enabled = false;
    }
}