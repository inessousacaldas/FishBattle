using UnityEngine;
using System.Collections;



/// <summary>
/// 任务常用对话和环数判断
/// </summary>
public class MissionDialogue {
    #region 安全巡查
    //	巡查队长总环数
    static public int maxBuffyRingNum = 120;
    //巡查队员限制
    static public int minTeamNum = 1;
    //巡查人员不够的时候快速组队
    static public string quickTime = "魔兽凶狠残暴，安全起见，至少需要{0}人组队才能领取任务。";
    //巡查队员里面等级不够30队员显示
    static public string TeamWithMinLv = "安全巡查危险重重，需要达到{0}级才可领取，您队伍中{1}等级不足{0}级";
    //如果玩家自身等级不够30级
    static public string playerWithMinLv = "安全巡查会遇到危险的魔兽，你还是先升到{0}级再来吧。";
    //玩家当天已用完安全巡查环数
    static public string RindCycles = "安全巡查奖励已经领完了，继续巡查将无法获得奖励，是否确认领取？";
    //安全巡查最低等级
    static public int minLevel = 30;
    //战斗前检测队友人数如果不足巡查人员显示
    static public string BattleMinTeam = "魔兽众多，找多个帮手再来吧。";
    //不是队长提示
    static public string NotLeaderTip = "你不是队长，请找齐队员再来。";
    static public string NotRindCyclesTip ="你没有安全巡查奖励次数，无法获得奖励。";
    //不能带队了
    static public string NotGroupLeader = "巡逻点数{0}个已经消耗完毕，无法继续带队了。";
    //已经领取了
    static public string ReceivedGhostDelegate = "你已经领取安全巡查任务了，快去完成吧。";
    //点数不够的时候
    static public string GhostNoMaxRing = "你已经没有安全巡查奖励次数，继续巡查将不会获得奖励，是否继续？";
    //标题
    static public string GhostTitle = "安全巡查";
    #endregion


    #region 公会任务提示
    //超过今天可做公会任务环数提示
    static public string GuildRingNum = "今天不能再接受公会任务";
    //身上已经公会任务了
    static public string GuildAccepEd = "交给你的任务还没有完成吗？";

    #endregion
}
