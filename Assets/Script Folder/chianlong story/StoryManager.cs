using UnityEngine;

public class StoryManager : MonoBehaviour
{
    public static StoryManager Instance;

    [Header("Current State")]
    public StoryState currentState;

    [Header("System References")]
    public SubtitleDisplayManager subtitleDisplayManager;
    public Talker npc;
    public Animator animator;
    public string animation; //會刪掉，下面animation改成觸發動畫名稱

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartStory();
    }

    // 觸發劇情開始
    void StartStory()
    {
        Debug.Log("Story Start");

        currentState = StoryState.WaitEnterZone;

        // subtitleDisplayManager.ShowHint("請靠近桌子");
    }

    // 所有 Trigger 都會呼叫這裡
    public void Notify(EventType eventType)
    {
        Debug.Log("Event Received: " + eventType);

        switch (currentState)
        {
            case StoryState.WaitEnterZone:

                if (eventType == EventType.EnterStoryZone)
                {
                    // 1.開場-純劇情動畫
                    GoToNPCIntro();
                }

                break;

            case StoryState.WaitPlaceBowl:

                // if (eventType == EventType.PutBowlFailed)
                // {
                //     GoToPlaceBowl();
                // }
                if (eventType == EventType.PutBowlTimeout)
                {
                    GoToPlaceBowl();
                }
                if (eventType == EventType.PutBowlSuccess)
                {
                    GoToPlaceBowl();
                }

                break;

            case StoryState.WaitLookBowl:

                if (eventType == EventType.LookBowlSuccess)
                {
                    GoToPlaceBowl();
                }
                if (eventType == EventType.LookBowlTimeout)
                {
                    GoToPlaceBowl();
                }
                if (eventType == EventType.LookBowlFailed)
                {
                    GoToPlaceBowl();
                }

                break;

            case StoryState.WaitBottleIntoBowl:

                if (eventType == EventType.PutBottleIntoBowlSuccess)
                {
                    GoToPutBottle();
                }
                if (eventType == EventType.PutBottleIntoBowlFailed)
                {
                    GoToPutBottle();
                }

                break;

            case StoryState.WaitBowlToNPC:

                if (eventType == EventType.GiveBowlToNPCSuccess)
                {
                    FinishStory();
                }
                if (eventType == EventType.GiveBowlToNPCFailed)
                {
                    FinishStory();
                }

                break;

            case StoryState.WaitGazeBowlClose:

                if (eventType == EventType.GazeBowlCloseSuccess)
                {
                    FinishStory();
                }
                if (eventType == EventType.GazeBowlCloseFailed)
                {
                    FinishStory();
                }

                break;

            case StoryState.WaitBowlBack:

                if (eventType == EventType.PutBowlBackSuccess)
                {
                    FinishStory();
                }
                if (eventType == EventType.PutBowlBackFailed)
                {
                    FinishStory();
                }

                break;
        }
    }

    // =========================
    // 各劇情步驟(功能會放這裡?)
    // =========================

    void GoToNPCIntro() //之後要改，會改成一連串動畫，不會分三段，作為之後Coroutine的範例
    {
        StartCoroutine(NPCIntroSequence());
    }
    IEnumerator NPCIntroSequence()
    {
        currentState = StoryState.NPCIntroTalking;

        // ===== 第一段 =====

        SubtitleDisplayManager.Instance.DisplayStory("story1-1");

        animator.SetTrigger("Action1");

        yield return npc.SpeakCoroutine("intro_01");


        // ===== 第二段 =====

        SubtitleDisplayManager.Instance.DisplayStory("story1-2");

        animator.SetTrigger("Action2");

        yield return npc.SpeakCoroutine("intro_02");


        // ===== 第三段 =====

        SubtitleDisplayManager.Instance.DisplayStory("story1-3");

        animator.SetTrigger("Action3");

        yield return npc.SpeakCoroutine("intro_03");
        // 下一步
        StartCoroutine(GoToPlaceBowlSequence());
    }
    public IEnumerator GoToPlaceBowlSequence()
    {
        currentState = StoryState.WaitPlaceBowl;

        // 顯示劇情與提示
        SubtitleDisplayManager.Instance.DisplayStory("story2-1");
        SubtitleDisplayManager.Instance.DisplayHint("hint2-1");

        // 播放 NPC 台詞並等待完成
        yield return npc.SpeakCoroutine("story2-1");

        // 等待trigger
    }

    void GoToPutBottle()
    {
        currentState = StoryState.WaitPutBottle;

        SubtitleDisplayManager.Instance.DisplayHint("hint4-1");

    }

    void FinishStory()
    {
        currentState = StoryState.Finish;

        subtitleDisplayManager.ShowHint("完成！");

        animator.SetTrigger(animation);
    }
}