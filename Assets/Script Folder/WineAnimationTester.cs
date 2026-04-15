using UnityEngine;

public class WineAnimationTester : MonoBehaviour
{
    [Header("物件指派")]
    public Transform bowl;          // 碗 (635)
    public Transform bottle;        // 酒瓶
    public Transform handAnchor;    // 乾隆右手掌的掛載點 (Bowl_Anchor)
    public Transform bowlTarget;    // 碗裡面的掛載點 (溫酒)
    public Transform drinkTarget;   // 喝酒位置 (右手特定點)

    [Header("飛行設定")]
    public float flySpeed = 8f;
    public Vector3 fixedBowlRotation = new Vector3(0, 0, 0);

    // 狀態開關 (設為 private 避免外部誤改)
    private bool shouldBowlFly = false;
    private bool isBowlInHand = false;
    private bool shouldBottleToBowlFly = false;
    private bool isBottleInBowl = false;
    private bool shouldBottleToHandFly = false;
    private bool isBottleInHand = false;

    // --- 【新增：供外部腳本呼叫的方法】 ---

    // 呼叫此方法讓碗飛向右手
    public void TriggerBowlFly()
    {
        if (!isBowlInHand) shouldBowlFly = true;
    }

    // 呼叫此方法讓酒瓶飛入碗中
    public void TriggerBottleToBowl()
    {
        if (!isBottleInBowl) shouldBottleToBowlFly = true;
    }

    // 呼叫此方法讓酒瓶飛向 drinkTarget (喝酒)
    public void TriggerBottleToHand()
    {
        if (!isBottleInHand) shouldBottleToHandFly = true;
    }

    void Update()
    {
        // --- 鍵盤測試 (保留原始功能，方便除錯) ---
        if (Input.GetKeyDown(KeyCode.Space)) TriggerBowlFly();
        if (Input.GetKeyDown(KeyCode.B)) TriggerBottleToBowl();
        if (Input.GetKeyDown(KeyCode.N)) TriggerBottleToHand();

        // --- 執行飛行邏輯 ---

        // 碗飛向右手掌
        if (shouldBowlFly && !isBowlInHand)
        {
            MoveAndAttach(bowl, handAnchor, () => {
                isBowlInHand = true;
                shouldBowlFly = false;
                bowl.SetParent(handAnchor);
                Debug.Log("碗已就位");
            });
        }

        // 酒瓶飛向碗裡 (溫酒)
        if (shouldBottleToBowlFly && !isBottleInBowl)
        {
            MoveAndAttach(bottle, bowlTarget, () => {
                isBottleInBowl = true;
                shouldBottleToBowlFly = false;
                bottle.SetParent(bowlTarget);
                Debug.Log("酒瓶已入碗");
            });
        }

        // 酒瓶飛向右手 (喝酒)
        if (shouldBottleToHandFly && !isBottleInHand)
        {
            MoveAndAttach(bottle, drinkTarget, () => {
                isBottleInHand = true;
                shouldBottleToHandFly = false;
                bottle.SetParent(drinkTarget);
                Debug.Log("酒瓶已到達 drinkTarget");
            });
        }

        // 碗的旋轉鎖定
        if (isBowlInHand)
        {
            bowl.rotation = Quaternion.Euler(fixedBowlRotation);
        }
    }

    void MoveAndAttach(Transform obj, Transform target, System.Action onComplete)
    {
        if (obj == null || target == null) return;
        obj.position = Vector3.Lerp(obj.position, target.position, flySpeed * Time.deltaTime);
        obj.rotation = Quaternion.Slerp(obj.rotation, target.rotation, flySpeed * Time.deltaTime);

        if (Vector3.Distance(obj.position, target.position) < 0.02f)
        {
            obj.position = target.position;
            obj.rotation = target.rotation;
            onComplete?.Invoke();
        }
    }
}