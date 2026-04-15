using UnityEngine;

[RequireComponent(typeof(Animator))] // 確保此腳本掛在有 Animator 的乾隆身上
public class BowlFlyIn : MonoBehaviour
{
    [Header("物件指派")]
    public Transform bowl;         // 碗 (635)
    public Transform handAnchor;  // 右手裡的 Bowl_Anchor
    private Animator characterAnim; 
    private Rigidbody bowlRigidbody; // 取得碗的剛體

    [Header("飛行設定")]
    public float flySpeed = 2f;
    public float delayTime = 0.5f; // 動畫播多久後碗才開始飛

    private bool isSequenceStarted = false;
    private bool isAttached = false;
    private float timer = 0f;
    private Quaternion fixedRotation = Quaternion.Euler(0, 0, 0);

    void Start()
    {
        characterAnim = GetComponent<Animator>();
        
        // 自動獲取碗的剛體，確保飛行模式正確
        if (bowl != null)
        {
            bowlRigidbody = bowl.GetComponent<Rigidbody>();
            if (bowlRigidbody == null)
            {
                Debug.LogError("碗 (635) 物件上找不到 Rigidbody 組件！");
            }
        }
    }

    void Update()
    {
        if (isSequenceStarted && !isAttached)
        {
            timer += Time.deltaTime;
            
            if (timer >= delayTime)
            {
                // 【核心修改】：Kinematic 模式不使用 bowl.position = ...，改用 MovePosition 更穩定
                Vector3 newPos = Vector3.Lerp(bowl.position, handAnchor.position, flySpeed * Time.deltaTime);
                Quaternion newRot = Quaternion.Slerp(bowl.rotation, handAnchor.rotation, flySpeed * Time.deltaTime);
                
                // bowlRigidbody.MovePosition(newPos); // 如果 Lerp 卡卡可以用這個
                bowl.position = newPos; 
                bowl.rotation = newRot;

                if (Vector3.Distance(bowl.position, handAnchor.position) < 0.02f)
                {
                    isAttached = true;
                    bowl.SetParent(handAnchor); // 正式綁定
                    Debug.Log("碗已吸附");
                }
            }
        }

        if (isAttached)
        {
            bowl.position = handAnchor.position;
            bowl.rotation = fixedRotation;
        }
    }

    // 【外部呼叫這個方法】
    public void StartBowlSequence()
    {
        if (isSequenceStarted) return;
        
        isSequenceStarted = true;
        isAttached = false;
        timer = 0f;
        
        // 【核心修改】：在飛行前，徹底關閉碗的物理影響
        if (bowlRigidbody != null)
        {
            bowlRigidbody.isKinematic = true; // 強制開啟 Kinematic，防止彈跳
            bowlRigidbody.velocity = Vector3.zero; // 清除可能殘留的速度
            bowlRigidbody.angularVelocity = Vector3.zero;
        }

        // 1. 先播放動畫
        if (characterAnim != null)
        {
            characterAnim.SetTrigger("16-17");
            Debug.Log("第一步：播動畫，等 " + delayTime + " 秒後碗會起飛");
        }

        // 碗目前不用 SetParent(null)，讓它在原地 Kinematic 等待
    }
}