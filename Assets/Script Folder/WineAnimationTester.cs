using UnityEngine;

public class WineAnimationTester : MonoBehaviour
{
    [Header("物件指派")]
    public Transform bowl;
    public Transform bottle;
    public Transform handAnchor;
    public Transform bowlTarget;
    public Transform drinkTarget;

    [Header("飛行設定")]
    public float flySpeed = 3f;
    public Vector3 fixedBowlRotation = new Vector3(0, 0, 0);

    [Header("偏移設定")]
    public Vector3 bottleBowlOffset = new Vector3(0, 0.05f, 0);
    public Vector3 bottleHandOffset = Vector3.zero;

    private bool shouldBowlFly = false;
    private bool isBowlInHand = false;

    private bool shouldBottleToBowlFly = false;
    private bool isBottleInBowl = false;

    private bool shouldBottleToHandFly = false;
    private bool isBottleInHand = false;

    Rigidbody bottleRb;

    void Start()
    {
        bottleRb = bottle.GetComponent<Rigidbody>();
    }

    // =========================
    // 外部呼叫
    // =========================

    public void TriggerBowlFly()
    {
        if (!isBowlInHand)
            shouldBowlFly = true;
    }

    public void TriggerBottleToBowl()
    {
        if (!isBottleInBowl)
        {
            PrepareBottleForAnimation();
            shouldBottleToBowlFly = true;
        }
    }

    public void TriggerBottleToHand()
    {
        if (!isBottleInHand)
        {
            isBottleInBowl = false;
            shouldBottleToBowlFly = false;

            bottle.SetParent(null);

            PrepareBottleForAnimation();

            shouldBottleToHandFly = true;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) TriggerBowlFly();
        if (Input.GetKeyDown(KeyCode.B)) TriggerBottleToBowl();
        if (Input.GetKeyDown(KeyCode.N)) TriggerBottleToHand();

        // =====================
        // 碗飛到手
        // =====================
        if (shouldBowlFly && !isBowlInHand)
        {
            MoveAndAttach(bowl, handAnchor, Vector3.zero, () =>
            {
                isBowlInHand = true;
                shouldBowlFly = false;
                bowl.SetParent(handAnchor);
                Debug.Log("碗已就位");
            });
        }

        // =====================
        // 酒瓶進碗
        // =====================
        if (shouldBottleToBowlFly && !isBottleInBowl)
        {
            MoveAndAttach(bottle, bowlTarget, bottleBowlOffset, () =>
            {
                isBottleInBowl = true;
                shouldBottleToBowlFly = false;

                bottle.SetParent(bowlTarget);
                Debug.Log("酒瓶已入碗");
            });
        }

        // =====================
        // 酒瓶到手
        // =====================
        if (shouldBottleToHandFly && !isBottleInHand)
        {
            MoveAndAttach(bottle, drinkTarget, bottleHandOffset, () =>
            {
                isBottleInHand = true;
                shouldBottleToHandFly = false;

                bottle.SetParent(drinkTarget);
                Debug.Log("酒瓶已到手");
            });
        }

        if (isBowlInHand)
        {
            bowl.rotation = Quaternion.Euler(fixedBowlRotation);
        }
    }

    // =========================
    // ⭐ 4參數版本（核心）
    // =========================
    void MoveAndAttach(Transform obj, Transform target, Vector3 offset, System.Action onComplete)
    {
        if (obj == null || target == null) return;

        Vector3 targetPos = target.position + offset;

        obj.position = Vector3.Lerp(obj.position, targetPos, flySpeed * Time.deltaTime);
        obj.rotation = Quaternion.Slerp(obj.rotation, target.rotation, flySpeed * Time.deltaTime);

        if (Vector3.Distance(obj.position, targetPos) < 0.02f)
        {
            obj.position = targetPos;
            obj.rotation = target.rotation;
            onComplete?.Invoke();
        }
    }

    // =========================
    // ⭐ 2參數版本（避免舊錯誤）
    // =========================
    void MoveAndAttach(Transform obj, Transform target, System.Action onComplete)
    {
        MoveAndAttach(obj, target, Vector3.zero, onComplete);
    }

    // =========================
    // 關閉物理
    // =========================
    void PrepareBottleForAnimation()
    {
        if (bottleRb != null)
        {
            bottleRb.isKinematic = true;
            bottleRb.useGravity = false;

            bottleRb.velocity = Vector3.zero;
            bottleRb.angularVelocity = Vector3.zero;
        }
    }
}