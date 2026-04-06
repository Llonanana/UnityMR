using UnityEngine;
using System.Collections;

public class StoryManager : MonoBehaviour
{
    public static StoryManager Instance;

    [Header("Current State")]
    public StoryState currentState;

    [Header("System References")]
    public SubtitleDisplayManager subtitleDisplayManager;
    public Talker npc;
    public Animator animator;
    // public string animation; //下面animation改成觸發動畫名稱
    private bool allowLookBowlSuccessOnly = false;
    private bool allowGazeBowlSuccessOnly = false;
    private Coroutine lookLongerCoroutine;
    private Coroutine gazeLongerCoroutine;
    
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartStory();
    }

    // 延遲指令
    IEnumerator Delay(float time, IEnumerator routine)
    {
        yield return new WaitForSeconds(time);
        yield return routine;
    }

    // 觸發劇情開始
    void StartStory()
    {
        Debug.Log("Story Start");

        currentState = StoryState.WaitEnterZone;

        // subtitleDisplayManager.ShowHint("請靠近桌子");
    }

    //!!!!!!!!!!!記得加上npc talking state!!!!!!!!!
    // npc talking包含上一階段的task failed/success
    // 改成一律npc talking 不分階段因為npc講話不會有其他觸發事件
    // 所有 Trigger 都會呼叫這裡
    public void Notify(EventType eventType)
    {
        Debug.Log("Event Received: " + eventType);

        // 如果進入「只允許成功」模式
        if (allowLookBowlSuccessOnly)
        {
            if (eventType == EventType.LookBowlSuccess)
            {
                StartCoroutine(GoToPutBottle());
            }
            return; // 擋掉所有其他事件
        }
        if (allowGazeBowlSuccessOnly)
        {
            if (eventType == EventType.GazeBowlCloseSuccess)
            {
                StartCoroutine(ContinueAppreciate());

            }
            return; // 擋掉所有其他事件
        }

        switch (currentState)
        {
            case StoryState.WaitEnterZone:
                switch (eventType)
                {
                    case EventType.EnterStoryZone:
                        GoToNPCIntro();
                        break;
                }
                break;

            case StoryState.WaitPlaceBowl:
                switch (eventType)
                {
                    case EventType.PutBowlTimeout:
                       StartCoroutine(FindBowlTimeout());
                        break;

                    case EventType.PutBowlSuccess:
                       StartCoroutine(FindBowlSuccess());
                        break;
                }
                break;

            case StoryState.WaitLookBowl:
                switch (eventType)
                {
                    case EventType.LookBowlSuccess:
                        OnLookBowlSuccess();
                        break;
                    // case EventType.LookBowlTimeout:
                    //     StartCoroutine(Delay(5f, GoToPutBottle()));
                    //     break;
                    case EventType.LookBowlFailed:
                        OnLookBowlFailed();
                        break;
                }
                break;

            case StoryState.WaitBottleIntoBowl:
                switch (eventType)
                {
                    case EventType.PutBottleIntoBowlSuccess:
                        StartCoroutine(NPCDrinking());
                        break;
                    case EventType.PutBottleIntoBowlFailed:
                        // animator.SetTrigger("flyingBottle");
                        StartCoroutine(NPCDrinking());
                        break;
                }
                break;

            case StoryState.WaitBowlToNPC:
                switch (eventType)
                {
                    case EventType.GiveBowlToNPCSuccess:
                        StartCoroutine(NPCStartAppreciate());
                        break;
                    case EventType.GiveBowlToNPCFailed:
                        // animator.SetTrigger("flyingBowlAgain");
                        StartCoroutine(NPCStartAppreciate());
                        break;
                }
                break;

            case StoryState.WaitGazeBowlClose:
                switch (eventType)
                {
                    case EventType.GazeBowlCloseSuccess:
                        OnGazeBowlSuccess();
                        break;
                    case EventType.GazeBowlCloseFailed:
                        OnGazeBowlFailed();
                        break;
                }
                break;

            case StoryState.WaitBowlBack:
                switch (eventType)
                {
                    case EventType.PutBowlBackSuccess:
                    case EventType.PutBowlBackFailed:
                        StoryEnding();
                        break;
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
        currentState = StoryState.NPCTalking;

        // ===== 第一段 =====

        SubtitleDisplayManager.Instance.DisplayStory("story1-1");

        // animator.SetTrigger("breathing");

        yield return npc.SpeakCoroutine("story1-1");


        // ===== 第二段 =====

        SubtitleDisplayManager.Instance.DisplayStory("story1-2");

        animator.SetTrigger("look around1-2");

        yield return npc.SpeakCoroutine("story1-2");


        // ===== 第三段 =====

        SubtitleDisplayManager.Instance.DisplayStory("story1-3");

        animator.SetTrigger("looking exhibition1-3");

        yield return npc.SpeakCoroutine("story1-3");
        // 下一步
        StartCoroutine(GoToPlaceBowlSequence());
    }
    public IEnumerator GoToPlaceBowlSequence()
    {
        // currentState = StoryState.NPCTalking;

        SubtitleDisplayManager.Instance.DisplayStory("story2-1");
        SubtitleDisplayManager.Instance.DisplayHint("hint2-1");

        // 播放 NPC 台詞並等待完成
        yield return npc.SpeakCoroutine("story2-1");

        currentState = StoryState.WaitPlaceBowl;
        // 等待放碗trigger
    }
    public IEnumerator GoToLookBowlSequence()
    {
        // currentState = StoryState.NPCTalking;
        // 顯示劇情與提示
        SubtitleDisplayManager.Instance.DisplayStory("story3-1");
        animator.SetTrigger("near to far3-1");

        // 播放 NPC 台詞並等待完成
        yield return npc.SpeakCoroutine("story3-1");

        currentState = StoryState.WaitLookBowl;
        // 等待視線trigger
    }
        public IEnumerator GoToPutBottle()
    {
        currentState = StoryState.NPCTalking;
        
        SubtitleDisplayManager.Instance.DisplayStory("story4-1");
        yield return npc.SpeakCoroutine("story4-1");

        SubtitleDisplayManager.Instance.DisplayStory("story4-2");
        yield return npc.SpeakCoroutine("story4-2");
        SubtitleDisplayManager.Instance.DisplayHint("hint4-1");

        currentState = StoryState.WaitBottleIntoBowl;
        // 等待放酒壺trigger
    }
        public IEnumerator NPCDrinking()
    {
        currentState = StoryState.NPCTalking;

        SubtitleDisplayManager.Instance.DisplayStory("story4-3");
        animator.SetTrigger("drinking4-3");
        yield return npc.SpeakCoroutine("story4-3");

        // 到第五階段
        StartCoroutine(BowlAppreciate());
    }
        public IEnumerator BowlAppreciate()
    {
        SubtitleDisplayManager.Instance.DisplayStory("story5-1");
        yield return npc.SpeakCoroutine("story5-1");

        SubtitleDisplayManager.Instance.DisplayStory("story5-2");
        yield return npc.SpeakCoroutine("story5-2");
        animator.SetTrigger("taking5-1");
        SubtitleDisplayManager.Instance.DisplayHint("hint5-1");

        currentState = StoryState.WaitBowlToNPC;
        // 等待把碗給NPC trigger
    }
    public IEnumerator NPCStartAppreciate()
    {
        currentState = StoryState.NPCTalking;

        SubtitleDisplayManager.Instance.DisplayStory("story5-3");
        animator.SetTrigger("appreciating5-3");
        yield return npc.SpeakCoroutine("story5-3");

        currentState = StoryState.WaitGazeBowlClose;
        // 等待trigger：凝視溫碗
    }
    public IEnumerator ContinueAppreciate()
    {
        currentState = StoryState.NPCTalking;

        SubtitleDisplayManager.Instance.DisplayStory("story5-4");
        yield return npc.SpeakCoroutine("story5-4");

        // 到第六階段
        StartCoroutine(TurningBowl());
    }
    public IEnumerator TurningBowl()
    {
        SubtitleDisplayManager.Instance.DisplayStory("story6-1");
        animator.SetTrigger("taking6-1");
        yield return npc.SpeakCoroutine("story6-1");

        SubtitleDisplayManager.Instance.DisplayHint("hint6-1");

        // 可能需要寫trigger
        // yield return new WaitForSeconds(5f);
        SubtitleDisplayManager.Instance.DisplayHint("hint6-2");

        SubtitleDisplayManager.Instance.DisplayStory("story6-2");
        animator.SetTrigger("point fake exhibit6-2");
        yield return npc.SpeakCoroutine("story6-2");

        // 到第七階段
        StartCoroutine(FinishStory());
    }
    public IEnumerator FinishStory()
    {
        SubtitleDisplayManager.Instance.DisplayStory("story7-1");
        animator.SetTrigger("standstill7-1");
        yield return npc.SpeakCoroutine("story7-1");
        SubtitleDisplayManager.Instance.DisplayHint("hint7-1");

        currentState = StoryState.WaitGazeBowlClose;
        // 等待trigger：玩家把溫碗放回原位
    }
    public IEnumerator StoryEnding()
    {
        SubtitleDisplayManager.Instance.DisplayStory("story7-2");
        yield return npc.SpeakCoroutine("story7-2");

        // 劇情結束，這裡可以放一些結束後的處理，例如顯示結局畫面、重置劇情等
        Debug.Log("Story Ended");
    }




    // task success/failed methods
        public IEnumerator FindBowlTimeout()
    {
        currentState = StoryState.NPCTalking;
        
        SubtitleDisplayManager.Instance.DisplayTask("task2_overtime");
        animator.SetTrigger("task 2 overtime");
        // 播放 NPC 台詞並等待完成
        yield return npc.SpeakCoroutine("task2_overtime");
        // animator.SetTrigger("flyingBowl");
        // 到story3
        StartCoroutine(GoToLookBowlSequence());
    }
    public IEnumerator FindBowlSuccess()
    {
        currentState = StoryState.NPCTalking;

        // 顯示劇情與提示
        SubtitleDisplayManager.Instance.DisplayTask("task2_success");
        animator.SetTrigger("task 2 success");

        // 播放 NPC 台詞並等待完成
        yield return npc.SpeakCoroutine("task2_success");
        // animator.SetTrigger("flyingBowl");
        // 到story3
        StartCoroutine(GoToLookBowlSequence());
    }
    private IEnumerator LookLongerCoroutine()
    {
        allowLookBowlSuccessOnly = true; // 進入「只允許成功」

        SubtitleDisplayManager.Instance.DisplayTask("task3_failed");

        yield return new WaitForSeconds(5f);

        allowLookBowlSuccessOnly = false;

        StartCoroutine(GoToPutBottle());
    }
    void OnLookBowlSuccess()
    {
        allowLookBowlSuccessOnly = false;

        if (lookLongerCoroutine != null)
        {
            StopCoroutine(lookLongerCoroutine);
            lookLongerCoroutine = null;
        }

        StartCoroutine(GoToPutBottle());
    }
    void OnLookBowlFailed()
    {
        if (lookLongerCoroutine != null)
            StopCoroutine(lookLongerCoroutine);

        lookLongerCoroutine = StartCoroutine(LookLongerCoroutine());
    }

    private IEnumerator GazeLongerCoroutine()
    {
        allowGazeBowlSuccessOnly = true; // 進入「只允許成功」

        SubtitleDisplayManager.Instance.DisplayTask("task5_failed");

        yield return new WaitForSeconds(5f);

        allowGazeBowlSuccessOnly = false;

        StartCoroutine(ContinueAppreciate());
    }
    void OnGazeBowlSuccess()
    {
        allowGazeBowlSuccessOnly = false;

        if (gazeLongerCoroutine != null)
        {
            StopCoroutine(gazeLongerCoroutine);
            gazeLongerCoroutine = null;
        }

        StartCoroutine(ContinueAppreciate());
    }
    void OnGazeBowlFailed()
    {
        if (gazeLongerCoroutine != null)
            StopCoroutine(gazeLongerCoroutine);

        gazeLongerCoroutine = StartCoroutine(GazeLongerCoroutine());
    }
}