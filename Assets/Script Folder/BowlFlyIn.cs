using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Animator))]
public class BowlFlyIn : MonoBehaviour
{
    [Header("物件指派")]
    public Transform bowl;         
    public Transform handAnchor;  
    private Animator characterAnim; 
    private Rigidbody bowlRigidbody; 

    [Header("飛行設定")]
    public float flySpeed = 2f;
    public float delayTime = 0.5f; 

    [Header("漂浮設定")]
    public float floatAmplitude = 0.05f; // 上下漂浮的幅度
    public float floatFrequency = 1f;    // 漂浮的速度
    private Vector3 floatBasePosition;   

    private bool isSequenceStarted = false;
    private bool isAttached = false;
    private bool isFloating = false;     
    private float timer = 0f;
    private Quaternion fixedRotation = Quaternion.Euler(0, 0, 0);
    XRGrabInteractable grab;

    void Start()
    {
        characterAnim = GetComponent<Animator>();
        if (bowl != null)
        {
            bowlRigidbody = bowl.GetComponent<Rigidbody>();
        }
        grab = bowl.GetComponent<XRGrabInteractable>();

        if (grab != null)
        {
            grab.selectEntered.AddListener(OnGrab);
        }
    }

    void Update()
    {
        if (grab != null && grab.isSelected)
            return;

        // --- 鍵盤測試邏輯 ---
        
        // 按下空白鍵：開始飛行流程
        if (Input.GetKeyDown(KeyCode.Space) && !isSequenceStarted)
        {
            StartBowlSequence();
        }

        // 按下 M 鍵：解除吸附並開始漂浮 (測試用)
        if (Input.GetKeyDown(KeyCode.M) && isAttached)
        {
            DetachAndFloat();
        }

        // --- 核心運作邏輯 ---

        // 1. 飛行中
        if (isSequenceStarted && !isAttached && !isFloating)
        {
            timer += Time.deltaTime;
            if (timer >= delayTime)
            {
                bowl.position = Vector3.Lerp(bowl.position, handAnchor.position, flySpeed * Time.deltaTime);
                bowl.rotation = Quaternion.Slerp(bowl.rotation, handAnchor.rotation, flySpeed * Time.deltaTime);

                if (Vector3.Distance(bowl.position, handAnchor.position) < 0.02f)
                {
                    AttachToHand();
                }
            }
        }

        // 2. 吸附於手上
        if (isAttached)
        {
            bowl.position = handAnchor.position;
            bowl.rotation = fixedRotation;
        }

        // 3. 空中漂浮
        if (isFloating)
        {
            float newY = floatBasePosition.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
            bowl.position = new Vector3(floatBasePosition.x, newY, floatBasePosition.z);
            bowl.rotation = fixedRotation;
        }
    }

    // --- 功能方法 ---

    public void StartBowlSequence()
    {
        if (isSequenceStarted) return;
        isSequenceStarted = true;
        isAttached = false;
        isFloating = false; 
        timer = 0f;
        
        if (bowlRigidbody != null)
        {
            bowlRigidbody.isKinematic = true; 
            bowlRigidbody.velocity = Vector3.zero; 
        }

        if (characterAnim != null)
        {
            characterAnim.SetTrigger("16-17");
        }
    }

    void AttachToHand()
    {
        isAttached = true;
        isFloating = false;
        bowl.SetParent(handAnchor);
        Debug.Log("碗已吸附");
    }

    public void DetachAndFloat()
    {
        // 只有在吸附狀態下才能解除
        if (!isAttached) return;

        isAttached = false;
        isFloating = true;
        isSequenceStarted = false; // 重置流程開關，讓下次按空白鍵能重新開始

        bowl.SetParent(null); 
        floatBasePosition = bowl.position; 

        Debug.Log("【測試】碗已解除吸附，進入漂浮模式");
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        // ❗完全停止所有腳本控制
        isFloating = false;
        isAttached = false;
        isSequenceStarted = false;

        // ❗解除父物件（讓 XR 控制）
        bowl.SetParent(null);

        // ❗開啟物理（讓手能拿）
        if (bowlRigidbody != null)
        {
            bowlRigidbody.isKinematic = false;
        }

        Debug.Log("玩家抓取碗 → 停止漂浮");
    }
}