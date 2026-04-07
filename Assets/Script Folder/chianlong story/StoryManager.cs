using UnityEngine;
using System.Collections;

public class StoryManager : MonoBehaviour
{
    public static StoryManager Instance;
    public ProximityTrigger proximityTrigger;
    
        // 定義劇情狀態

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
    private bool eventLocked = false;
    private bool isPlaying = false; // 用於防止重複執行同一個任務
    
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartStory();
    }

    // 延遲指令
    // IEnumerator Delay(float time, IEnumerator routine)
    // {
    //     yield return new WaitForSeconds(time);
    //     yield return routine;
    // }

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
        if (currentState == StoryState.NPCTalking || eventLocked)
            return;
        
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
                StartCoroutine(NPCStartAppreciate());

            }
            return; // 擋掉所有其他事件
        }

        switch (currentState)
        {
            case StoryState.NPCTalking:
                // NPC 講話階段，不接受任何事件
                Debug.Log("NPC is talking. Ignoring all events.");
                return; // 直接返回
            case StoryState.WaitEnterZone:
                switch (eventType)
                {
                    case EventType.EnterStoryZone:
                        eventLocked = true;
                        StartCoroutine(DelayedStartIntro());
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
                    case EventType.PutBowlFailed:
                        StartCoroutine(FindBowlTooFar());
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

            // case StoryState.WaitBowlToNPC:
            //     switch (eventType)
            //     {
            //         case EventType.GiveBowlToNPCSuccess:
            //             StartCoroutine(NPCStartAppreciate());
            //             break;
            //         case EventType.GiveBowlToNPCFailed:
            //             // animator.SetTrigger("flyingBowlAgain");
            //             StartCoroutine(NPCStartAppreciate());
            //             break;
            //     }
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
            case StoryState.Finish:
                switch (eventType)
                {
                    case EventType.ExitStoryZone:
                        Finish();
                        break;
                    case EventType.EnterStoryZone:
                        // 如果已經結束了，玩家又進來了，就重置劇情
                        StartStory();
                        break;
                }
                break;
        }
    }

    // =========================
    // 各劇情步驟(功能會放這裡?)
    // =========================

    
    IEnumerator DelayedStartIntro()
    {
        Debug.Log("Player Entered Zone");

        // ⭐ 確保乾隆先出現
        if (npc != null)
        {
            npc.gameObject.SetActive(true);
        }

        // ⭐ 等 1~2 秒（很重要）
        yield return new WaitForSeconds(1.5f);

        // ⭐ 再開始真正劇情
        GoToNPCIntro();
    }
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

        yield return npc.SpeakCoroutine("stories", "story1-1");


        // ===== 第二段 =====

        SubtitleDisplayManager.Instance.DisplayStory("story1-2");

        animator.SetTrigger("look around1-2");

        yield return npc.SpeakCoroutine("stories", "story1-2");


        // ===== 第三段 =====

        SubtitleDisplayManager.Instance.DisplayStory("story1-3");

        animator.SetTrigger("looking exhibition1-3");

        yield return npc.SpeakCoroutine("stories", "story1-3");
        // 下一步
        StartCoroutine(GoToPlaceBowlSequence());
    }
    public IEnumerator GoToPlaceBowlSequence()
    {
        // currentState = StoryState.NPCTalking;

        SubtitleDisplayManager.Instance.DisplayStory("story2-1");
        SubtitleDisplayManager.Instance.DisplayHint("hint2-1");

        // 播放 NPC 台詞並等待完成
        yield return npc.SpeakCoroutine("stories", "story2-1");

        currentState = StoryState.WaitPlaceBowl;
        eventLocked = false;
        // 等待放碗trigger
    }
    public IEnumerator GoToLookBowlSequence()
    {
        // currentState = StoryState.NPCTalking;
        // 顯示劇情與提示
        SubtitleDisplayManager.Instance.DisplayStory("story3-1");
        animator.SetTrigger("near to far3-1");

        // 播放 NPC 台詞並等待完成
        yield return npc.SpeakCoroutine("stories", "story3-1");

        currentState = StoryState.WaitLookBowl;
        eventLocked = false;
        // 等待視線trigger
    }
        public IEnumerator GoToPutBottle()
    {
        currentState = StoryState.NPCTalking;
        
        SubtitleDisplayManager.Instance.DisplayStory("story4-1");
        yield return npc.SpeakCoroutine("stories", "story4-1");

        SubtitleDisplayManager.Instance.DisplayStory("story4-2");
        yield return npc.SpeakCoroutine("stories", "story4-2");
        SubtitleDisplayManager.Instance.DisplayHint("hint4-1");

        currentState = StoryState.WaitBottleIntoBowl;
        eventLocked = false;
        // 等待放酒壺trigger
    }
        public IEnumerator NPCDrinking()
    {
        eventLocked = true;
        currentState = StoryState.NPCTalking;

        SubtitleDisplayManager.Instance.DisplayStory("story4-3");
        animator.SetTrigger("drinking4-3");
        yield return npc.SpeakCoroutine("stories", "story4-3");

        // 到第五階段
        StartCoroutine(BowlAppreciate());
    }
        public IEnumerator BowlAppreciate()
    {
        SubtitleDisplayManager.Instance.DisplayStory("story5-1");
        // animator.SetTrigger("taking5-1");
        yield return npc.SpeakCoroutine("stories", "story5-1");

        SubtitleDisplayManager.Instance.DisplayStory("story5-2");
        animator.SetTrigger("appreciating5-3");
        yield return npc.SpeakCoroutine("stories", "story5-2");

        currentState = StoryState.WaitGazeBowlClose;
        eventLocked = false;
        // 等待靠近欣賞trigger

    }
    public IEnumerator NPCStartAppreciate()
    {
        // eventLocked = true;
        currentState = StoryState.NPCTalking;

        SubtitleDisplayManager.Instance.DisplayStory("story5-3");
        // animator.SetTrigger("breathing");
        yield return npc.SpeakCoroutine("stories", "story5-3");

        // 到第六階段
        StartCoroutine(TurningBowl());
    }
    public IEnumerator TurningBowl()
    {
        SubtitleDisplayManager.Instance.DisplayStory("story6-1");
        animator.SetTrigger("taking6-1");
        yield return npc.SpeakCoroutine("stories", "story6-1");

        SubtitleDisplayManager.Instance.DisplayHint("hint6-1");

        // 可能需要寫trigger：
        // eventLocked = false;
        // 等待trigger：玩家把碗拿走
        // eventLocked = true;

        // 沒有trigger的話：
        // yield return new WaitForSeconds(5f);

        SubtitleDisplayManager.Instance.DisplayHint("hint6-2");

        SubtitleDisplayManager.Instance.DisplayStory("story6-2");
        animator.SetTrigger("point fake exhibit6-2");
        yield return npc.SpeakCoroutine("stories", "story6-2");

        // 到第七階段
        StartCoroutine(FinishStory());
    }
    public IEnumerator FinishStory()
    {
        SubtitleDisplayManager.Instance.DisplayStory("story7-1");
        animator.SetTrigger("standstill7-1");
        yield return npc.SpeakCoroutine("stories", "story7-1");
        SubtitleDisplayManager.Instance.DisplayHint("hint7-1");

        currentState = StoryState.WaitBowlBack;
        eventLocked = false;
        // 等待trigger：玩家把溫碗放回原位
    }
    public IEnumerator StoryEnding()
    {
        eventLocked = true;
        currentState = StoryState.NPCTalking;
        SubtitleDisplayManager.Instance.DisplayStory("story7-2");
        yield return npc.SpeakCoroutine("stories", "story7-2");

        currentState = StoryState.Finish;
    }
    public void Finish()
    {
        // 劇情結束 → 關閉乾隆場景
        if (proximityTrigger != null)
        {
            proximityTrigger.HidePrompt();
        }
        // 劇情結束，這裡可以放一些結束後的處理，例如顯示結局畫面、重置劇情等
        Debug.Log("Story Ended");
    }






    // task success/failed methods
    public IEnumerator FindBowlTooFar()
    {
        if (isPlaying) yield break; // 已在執行就直接退出

        // eventLocked = false;
        isPlaying = true;

        SubtitleDisplayManager.Instance.DisplayTask("task2_fail");
        yield return npc.SpeakCoroutine("tasks", "task2_fail");

        isPlaying = false; // 結束後解除鎖
        }
            
        public IEnumerator FindBowlTimeout()
    {
        eventLocked = true;
        currentState = StoryState.NPCTalking;
        
        SubtitleDisplayManager.Instance.DisplayTask("task2_overtime");
        animator.SetTrigger("task 2 overtime");
        // 播放 NPC 台詞並等待完成
        yield return npc.SpeakCoroutine("tasks", "task2_overtime");
        // animator.SetTrigger("flyingBowl");
        // 到story3
        StartCoroutine(GoToLookBowlSequence());
    }
    public IEnumerator FindBowlSuccess()
    {
        eventLocked = true;
        currentState = StoryState.NPCTalking;

        // 顯示劇情與提示
        SubtitleDisplayManager.Instance.DisplayTask("task2_success");
        animator.SetTrigger("task 2 success");

        // 播放 NPC 台詞並等待完成
        yield return npc.SpeakCoroutine("tasks", "task2_success");
        // animator.SetTrigger("flyingBowl");
        // 到story3
        StartCoroutine(GoToLookBowlSequence());
    }
    private IEnumerator LookLongerCoroutine()
    {
        // eventLocked = false;
        allowLookBowlSuccessOnly = true; // 進入「只允許成功」

        SubtitleDisplayManager.Instance.DisplayTask("task3_failed");

        yield return new WaitForSeconds(5f);

        eventLocked = true;
        allowLookBowlSuccessOnly = false;

        StartCoroutine(GoToPutBottle());
    }
    void OnLookBowlSuccess()
    {
        eventLocked = true;
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
        // eventLocked = false;
        if (lookLongerCoroutine != null)
            StopCoroutine(lookLongerCoroutine);

        lookLongerCoroutine = StartCoroutine(LookLongerCoroutine());
    }

    private IEnumerator GazeLongerCoroutine()
    {
        allowGazeBowlSuccessOnly = true; // 進入「只允許成功」

        SubtitleDisplayManager.Instance.DisplayTask("task5_failed");

        yield return new WaitForSeconds(5f);

        eventLocked = true;
        allowGazeBowlSuccessOnly = false;

        StartCoroutine(NPCStartAppreciate());
    }
    void OnGazeBowlSuccess()
    {
        eventLocked = true;
        allowGazeBowlSuccessOnly = false;

        if (gazeLongerCoroutine != null)
        {
            StopCoroutine(gazeLongerCoroutine);
            gazeLongerCoroutine = null;
        }

        StartCoroutine(NPCStartAppreciate());
    }
    void OnGazeBowlFailed()
    {
        // eventLocked = false;
        if (gazeLongerCoroutine != null)
            StopCoroutine(gazeLongerCoroutine);

        gazeLongerCoroutine = StartCoroutine(GazeLongerCoroutine());
    }
}