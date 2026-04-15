using UnityEngine;

public class BowlFlyIn : MonoBehaviour
{
    [Header("物件指派")]
    public Transform bowl;         // 碗 (635)
    public Transform handAnchor;  // 右手裡的 Bowl_Anchor
    public Animator characterAnim; // 乾隆的 Animator

    [Header("飛行設定")]
    public float flySpeed = 6f;
    private bool isSequenceStarted = false;
    private bool isAttached = false;

    // 用來儲存「正面朝上」的旋轉值
    private Quaternion fixedRotation = Quaternion.Euler(0, 0, 0);

    void Update()
    {
        // 測試用：按下空白鍵開始整個流程
        if (Input.GetKeyDown(KeyCode.Space) && !isSequenceStarted)
        {
            StartBowlSequence();
        }

        // 飛行中邏輯
        if (isSequenceStarted && !isAttached)
        {
            // 1. 碗朝向手心飛行
            bowl.position = Vector3.Lerp(bowl.position, handAnchor.position, flySpeed * Time.deltaTime);
            bowl.rotation = Quaternion.Slerp(bowl.rotation, handAnchor.rotation, flySpeed * Time.deltaTime);

            // 2. 當碗足夠接近手心時
            if (Vector3.Distance(bowl.position, handAnchor.position) < 0.02f)
            {
                AttachAndAnimate();
            }
        }

        // 【新增邏輯】一旦吸附成功，強制碗永遠保持正面朝上，不受手掌旋轉影響
        if (isAttached)
        {
            // 位置依然跟隨手部掛載點
            bowl.position = handAnchor.position;
            // 旋轉則強制鎖定在世界座標的固定角度 (0,0,0)
            // 如果你的碗模型預設角度不同，可以手動修改上面的 fixedRotation 數值
            bowl.rotation = fixedRotation;
        }
    }

    public void StartBowlSequence()
    {
        isSequenceStarted = true;
        isAttached = false;
        bowl.SetParent(null); // 確保碗在飛行時不被其他東西影響
    }

    void AttachAndAnimate()
    {
        isAttached = true;

        // 3. 物理綁定（雖然 Update 裡強制同步了，但 SetParent 能確保層級結構正確）
        bowl.SetParent(handAnchor);

        // 4. 通知 Animator 開始播放動畫
        if (characterAnim != null)
        {
            characterAnim.SetTrigger("appreciating3-1");
            Debug.Log("碗已到位，開始播放 appreciating3-1");
        }
        else
        {
            Debug.LogError("尚未指派 Character Anim！");
        }
    }
}