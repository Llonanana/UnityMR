// using UnityEngine;
// using UnityEngine.XR.Interaction.Toolkit;
// using UnityEngine.XR.Interaction.Toolkit.Interactables;

// [RequireComponent(typeof(Animator))]
// public class BowlFlyIn : MonoBehaviour
// {
//     [Header("物件指派")]
//     public Transform bowl;         
//     public Transform handAnchor;  
//     private Animator characterAnim; 
//     private Rigidbody bowlRigidbody; 

//     [Header("飛行設定")]
//     public float flySpeed = 2f;
//     public float delayTime = 0.5f; 

//     [Header("漂浮設定")]
//     public float floatAmplitude = 0.05f; 
//     public float floatFrequency = 1f;    
//     private Vector3 floatBasePosition;   

//     private bool isSequenceStarted = false;
//     private bool isAttached = false;
//     private bool isFloating = false;     
//     private float timer = 0f;
//     private Quaternion fixedRotation = Quaternion.Euler(0, 0, 0);
//     XRGrabInteractable grab;

//     void Start()
//     {
//         characterAnim = GetComponent<Animator>();
//         if (bowl != null)
//         {
//             bowlRigidbody = bowl.GetComponent<Rigidbody>();
//             grab = bowl.GetComponent<XRGrabInteractable>();
//         }

//         if (grab != null)
//         {
//             grab.selectEntered.AddListener(OnGrab);
//         }
//     }

//     void Update()
//     {
//         // 如果正在被抓取，不執行腳本位移邏輯
//         if (grab != null && grab.isSelected) return;

//         // 1. 飛行中邏輯... (保留你原本的代碼)
//         if (isSequenceStarted && !isAttached && !isFloating)
//         {
//             timer += Time.deltaTime;
//             if (timer >= delayTime)
//             {
//                 bowl.position = Vector3.Lerp(bowl.position, handAnchor.position, flySpeed * Time.deltaTime);
//                 bowl.rotation = Quaternion.Slerp(bowl.rotation, handAnchor.rotation, flySpeed * Time.deltaTime);
//                 if (Vector3.Distance(bowl.position, handAnchor.position) < 0.02f) AttachToHand();
//             }
//         }

//         // 2. 吸附於手上邏輯...
//         if (isAttached)
//         {
//             bowl.position = handAnchor.position;
//             bowl.rotation = fixedRotation;
//         }

//         // 3. 空中漂浮邏輯
//         if (isFloating)
//         {
//             float newY = floatBasePosition.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
//             bowl.position = new Vector3(floatBasePosition.x, newY, floatBasePosition.z);
//             bowl.rotation = fixedRotation;
//         }
//     }

//     // ==========================================
//     // 新增功能 1：使碗漂浮在空中且玩家不可以拿 (其他腳本呼叫)
//     // ==========================================
//     public void EnableFloatingLocked()
//     {
//         isFloating = true;
//         isAttached = false;
//         isSequenceStarted = false;
//         floatBasePosition = bowl.position;

//         // 鎖定物理與抓取
//         if (bowlRigidbody != null) bowlRigidbody.isKinematic = true;
//         if (grab != null) grab.enabled = false; // 禁用抓取功能

//         Debug.Log("碗進入「鎖定漂浮」模式：玩家無法抓取");
//     }

//     // ==========================================
//     // 功能 2：解除吸附並開始漂浮 (你原本的邏輯，但加上可抓取開關)
//     // ==========================================
//     public void DetachAndFloat()
//     {
//         isAttached = false;
//         isFloating = true;
//         isSequenceStarted = false;
//         bowl.SetParent(null); 
//         floatBasePosition = bowl.position; 

//         if (bowlRigidbody != null) bowlRigidbody.isKinematic = true;
//         if (grab != null) grab.enabled = true; // 確保漂浮時玩家可以去拿

//         Debug.Log("碗進入「自由漂浮」模式：玩家可以抓取");
//     }

//     // ==========================================
//     // 關鍵回調：玩家拿了之後的處理
//     // ==========================================
//     void OnGrab(SelectEnterEventArgs args)
//     {
//         // 1. 停止所有腳本位移（包含漂浮）
//         isFloating = false;
//         isAttached = false;
//         isSequenceStarted = false;
//         bowl.SetParent(null);

//         // 2. 滿足你的需求：取消重力，但關閉 Kinematic 讓手感正常，且會停在空中
//         if (bowlRigidbody != null)
//         {
//             bowlRigidbody.isKinematic = false; // 關閉 Kinematic 手感才不會死板
//             bowlRigidbody.useGravity = false;  // 關閉重力，放手後會停滯在空中
//         }

//         Debug.Log("玩家抓取：取消漂浮，關閉重力 (放手後將停滯)");
//     }

//     // --- 其他原有方法 ---
//     public void StartBowlSequence()
//     {
//         if (isSequenceStarted) return;
//         isSequenceStarted = true;
//         isAttached = false;
//         isFloating = false; 
//         timer = 0f;
//         if (bowlRigidbody != null) bowlRigidbody.isKinematic = true; 
//         // if (characterAnim != null) characterAnim.SetTrigger("16-17");
//     }

//     void AttachToHand()
//     {
//         isAttached = true;
//         isFloating = false;
//         bowl.SetParent(handAnchor);
//     }
// }

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Animator))]
public class BowlFlyIn : MonoBehaviour
{
    [Header("物件指派")]
    public Transform bowl;         
    public Transform handAnchor;  
    private Rigidbody bowlRigidbody; 
    private XRGrabInteractable grab;

    [Header("飛行設定")]
    public float flySpeed = 2f;
    public float delayTime = 0.5f; 
    private float timer = 0f;

    [Header("漂浮設定")]
    public float floatAmplitude = 0.05f; 
    public float floatFrequency = 1f;    
    private Vector3 floatBasePosition;   

    // 狀態管理
    private bool isSequenceStarted = false; // 正在飛向 NPC 手
    private bool isAttached = false;        // 黏在 NPC 手上
    private bool isFloating = false;        // 在空中自由漂浮
    private Quaternion fixedRotation = Quaternion.Euler(0, 0, 0);

    void Start()
    {
        grab.selectExited.AddListener(OnRelease);
        if (bowl != null)
        {
            bowlRigidbody = bowl.GetComponent<Rigidbody>();
            grab = bowl.GetComponent<XRGrabInteractable>();
            
            // 初始化：關閉重力，開啟 Kinematic
            bowlRigidbody.useGravity = false;
            bowlRigidbody.isKinematic = true;
        }

        if (grab != null)
        {
            grab.selectEntered.AddListener(OnGrab);
        }
    }

    void Update()
    {
        // 優先權 0：如果正在被玩家抓取，什麼都不做
        if (grab != null && grab.isSelected) return;

        // 優先權 1：飛向 NPC 手 (StartBowlSequence)
        if (isSequenceStarted && !isAttached)
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
            return; // 正在飛行時，跳過後面的邏輯
        }

        // 優先權 2：吸附於 NPC 手上(在late update)
        // if (isAttached)
        // {
        //     // 既然已經 SetParent 了，我們只需要確保它待在父物件的中心點
        //     // 不要再用全域座標 (bowl.position = ...) 
        //     bowl.localPosition = Vector3.zero; 
            
        //     // 這裡就是調整角度的地方！
        //     // 如果設為 Vector3.zero 碗口還是歪的，就在這裡改數字，例如 (90, 0, 0)
        //     bowl.localRotation = Quaternion.Euler(Vector3.zero); 
        //     return;
        // }

        // 優先權 3：空中漂浮 (DetachAndFloat 之後)
        if (isFloating)
        {
            float newY = floatBasePosition.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
            bowl.position = new Vector3(floatBasePosition.x, newY, floatBasePosition.z);
            // 漂浮時可以維持固定旋轉
            bowl.rotation = fixedRotation;
        }
    }
    void LateUpdate() // 改用 LateUpdate 確保在動畫執行後強制覆蓋
    {
        if (isAttached)
        {
            // 先強制吸附位置
            bowl.position = handAnchor.position;
            
            // 在這裡測試角度，例如 (90, 0, 0)
            // 因為是 LateUpdate，它會強行掰過 Animator 的旋轉
            bowl.rotation = handAnchor.rotation * Quaternion.Euler(72, 156, 109); 
        }
    }

    // --- 外部呼叫：開始劇情飛行 ---
    public void StartBowlSequence()
    {
        isSequenceStarted = true;
        isAttached = false;
        isFloating = false; 
        timer = 0f;
        
        if (grab != null) grab.enabled = false; // 飛行中不能抓
        if (bowlRigidbody != null) bowlRigidbody.isKinematic = true;
        Debug.Log("碗開始飛向 NPC");
    }

    private void AttachToHand()
    {
        isSequenceStarted = false;
        isAttached = true;
        
        bowl.SetParent(handAnchor);
        
        // 歸零，讓它瞬間對齊 handAnchor 的位置與旋轉
        bowl.localPosition = Vector3.zero;
        bowl.localRotation = Quaternion.identity; 

        Debug.Log("碗已成功吸附並歸零座標");
    }

    // --- 外部呼叫：解除吸附並開始可抓取的漂浮 ---
    public void DetachAndFloat()
    {
        // 1. 斷開所有劇情連結
        isAttached = false;
        isSequenceStarted = false;
        bowl.SetParent(null); 
        
        // 2. 物理重置：先關閉 Kinematic 讓系統重新計算，再交給腳本
        if (bowlRigidbody != null)
        {
            bowlRigidbody.isKinematic = true; 
            bowlRigidbody.useGravity = false;
            bowlRigidbody.velocity = Vector3.zero;
            bowlRigidbody.angularVelocity = Vector3.zero;
        }

        // 3. 更新基準位置
        floatBasePosition = bowl.position; 
        fixedRotation = bowl.rotation;

        // 4. 開啟漂浮與抓取
        isFloating = true; 
        if (grab != null)
        {
            grab.enabled = true;
            // 關鍵：確保抓取時的移動模式是 Kinematic 或是 Velocity Tracking
            // 如果設為 Instantaneous 有時會跟腳本衝突
            grab.movementType = XRBaseInteractable.MovementType.Kinematic;
        }

        Debug.Log("<color=orange>碗：強制解除鎖定並進入漂浮，抓取功能已激活</color>");
    }

    // --- 抓取回調 ---
    void OnGrab(SelectEnterEventArgs args)
    {
        isFloating = false;
        isAttached = false;
        
        if (bowlRigidbody != null)
        {
            // 抓取的瞬間，「一定要」關掉 Kinematic 才能讓手部平滑拖曳
            bowlRigidbody.isKinematic = false; 
            bowlRigidbody.useGravity = false;
            bowlRigidbody.WakeUp();
        }
    }

    // 在 Start 加入 grab.selectExited.AddListener(OnRelease);

    void OnRelease(SelectExitEventArgs args)
    {
        // 1. 紀錄放手的位置作為新起點
        floatBasePosition = bowl.position;
        fixedRotation = bowl.rotation;

        // 2. 重新鎖定物理，讓腳本 Update 接管
        if (bowlRigidbody != null)
        {
            bowlRigidbody.velocity = Vector3.zero;
            bowlRigidbody.angularVelocity = Vector3.zero;
            bowlRigidbody.isKinematic = true; 
        }

        // 3. 恢復漂浮
        isFloating = true;
    }
}

