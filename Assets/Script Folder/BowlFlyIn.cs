using UnityEngine;

public class BowlFlyIn : MonoBehaviour
{
    [Header("物件指派")]
    public Transform bowl;         // 碗 (635)
    public Transform handAnchor;  // 右手裡的 Bowl_Anchor
    public Animator characterAnim; // 乾隆的 Animator

    [Header("飛行設定")]
    public float flySpeed = 6f;
    public float delayTime = 0.5f; // 【新增】收到指令後，過幾秒才開始飛

    private bool isSequenceStarted = false;
    private bool isAttached = false;
    private float timer = 0f;

    private Quaternion fixedRotation = Quaternion.Euler(0, 0, 0);

    void Update()
    {
        // 測試用：按下空白鍵
        if (Input.GetKeyDown(KeyCode.Space) && !isSequenceStarted)
        {
            StartBowlSequence();
        }

        // --- 核心邏輯修改 ---
        if (isSequenceStarted && !isAttached)
        {
            // 增加計時器，等到 delayTime 到了才開始動
            timer += Time.deltaTime;

            if (timer >= delayTime)
            {
                // 執行飛行
                bowl.position = Vector3.Lerp(bowl.position, handAnchor.position, flySpeed * Time.deltaTime);
                bowl.rotation = Quaternion.Slerp(bowl.rotation, handAnchor.rotation, flySpeed * Time.deltaTime);

                if (Vector3.Distance(bowl.position, handAnchor.position) < 0.02f)
                {
                    isAttached = true;
                    bowl.SetParent(handAnchor);
                    Debug.Log("碗已吸附在手上");
                }
            }
        }

        if (isAttached)
        {
            bowl.position = handAnchor.position;
            bowl.rotation = fixedRotation;
        }
    }

    // 【外部呼叫這個】
    public void StartBowlSequence()
    {
        if (isSequenceStarted) return; // 防止重複觸發

        isSequenceStarted = true;
        isAttached = false;
        timer = 0f; // 重置計時器

        // 1. 先播放動畫
        if (characterAnim != null)
        {
            characterAnim.SetTrigger("16-17");
            Debug.Log("第一步：開始播放 16-17 動畫");
        }

        // 2. 準備飛行 (此時碗還在原地，因為 Update 會等 delayTime)
        bowl.SetParent(null);
    }
}