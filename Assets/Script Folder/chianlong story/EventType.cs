// 剛才瞬間發生了甚麼事件
public enum EventType
{
    // 1.玩家靠近觸發劇情
    EnterStoryZone,
    // 2.溫碗放展桌事件
    PutBowlFailed,
    PutBowlTimeout,
    PutBowlSuccess,
    // 3.欣賞溫碗事件
    LookBowlSuccess,
    // LookBowlTimeout,
    LookBowlFailed,
    // 4.放入酒瓶事件
    PutBottleIntoBowlSuccess,
    PutBottleIntoBowlFailed,
    // 5.把碗給NPC事件(已刪除)
    // GiveBowlToNPCSuccess,
    // GiveBowlToNPCFailed,
    // 6.靠近欣賞溫碗事件
    GazeBowlCloseSuccess,
    GazeBowlCloseFailed,
    // 7.把碗放回原位事件
    PutBowlBackSuccess,
    PutBowlBackFailed,
    // 8.玩家離開劇情區域事件
    ExitStoryZone
}