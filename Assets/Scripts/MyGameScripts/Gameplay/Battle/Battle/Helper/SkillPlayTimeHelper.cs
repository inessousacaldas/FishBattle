// **********************************************************************
// Copyright (c) 2016 cilugame. All rights reserved.
// File     : SkillPlayTimeHelper.cs
// Author   : senkay <senkay@126.com>
// Created  : 7/9/2016 
// Porpuse  : 
// **********************************************************************
//
using System;
using AppDto;

//技能播放时长计算
using System.Collections.Generic;


public class SkillPlayTimeHelper
{
    public float GetVideoRoundPlayTime(VideoRound videoRound)
    {
        GameDebuger.TODO(@"if (!BattleDataManager.DEBUG)
        {
            return 0f;
        }");

        var _time = 0f;

        GameDebuger.TODO(@"_time += GetVideoRoundActionPlayTime(videoRound.readyAction);");

        for (int i = 0; i < videoRound.skillActions.Count; i++)
        {
            var videoSkillAction = videoRound.skillActions[i];
            _time += GetSkillVideoActionPlayTime(videoSkillAction);
        }

        GameDebuger.TODO(@"_time += GetVideoRoundActionPlayTime(videoRound.endAction);");

        //处理神罚
        GameDebuger.TODO(@"if (videoRound.afterEndAction != null && videoRound.afterEndAction.targetStateGroups.Count > 0)
        {
            _time += 1f;
            _time += GetVideoRoundActionPlayTime(videoRound.afterEndAction);
        }");

        return _time;
    }

    private static float GetVideoActionPlayTime(VideoAction videoAction)
    {
        float time = 0;

        for (var i = 0; i < videoAction.targetStateGroups.Count; i++)
        {
            var stateGroup = videoAction.targetStateGroups[i];
            if (stateGroup.strikeBackAction != null)
            {
                time += GetSkillVideoActionPlayTime(stateGroup.strikeBackAction);
            }
        }

        return time;
    }

    public static float GetSkillVideoActionPlayTime(VideoSkillAction videoSkillAction)
    {
        GameDebuger.TODO(@"if (!BattleDataManager.DEBUG)
        {
            return 0f;
        }");

        float time = GetVideoActionPlayTime(videoSkillAction);

        var skill = videoSkillAction.skill;
        if (skill == null)
        {
            return time;
        }
        if (videoSkillAction.actionSoldierId == 0)
        {
            return time;
        }
        if (skill.id == BattleDataManager.GetDefenseSkillId())
        {
            return time;
        }

        if (skill.id == BattleDataManager.GetProtectSkillId())
        {
            return time;
        }

        GameDebuger.TODO(@"if (videoSkillAction.skillStatusCode != Skill.SkillActionStatus_Ordinary)
        {
            return time;
        }");

        //释放技能的喊招时间
        if (skill.id >= 10)
        {
            time += 0.1f;
        }

        //开始攻击的准备时间
        time += skill.actionReadyPlayTime / 1000f;

        int totalWaitingRound = 0;

        bool waintingAttackFinish = false;

        List<long> injureIds = GetInjureIds(videoSkillAction, skill.atOnce);

        if (injureIds.Count == 0)
        {
            totalWaitingRound = 1;
        }
        else
        {
            if (skill.atOnce)
            {
                totalWaitingRound = 1; // Round 是主角行动多少次攻击行为
            }
            else
            {
                totalWaitingRound += injureIds.Count; // Round 是主角行动多少次攻击行为
            }
        }

        //攻击多人的施法时间
        time += (skill.singleActionPlayTime / 1000f * totalWaitingRound);

        //攻击多人的移动时间
        time += GetAttackMoveSpentTime(skill.clientSkillType) * totalWaitingRound;

        //行动结束耗时
        time += skill.actionEndPlayTime / 1000f;

        return time;
    }

    private static float GetAttackMoveSpentTime(int clientSkillType)
    {
        /*
		1 普攻
		2 远单
		3 远群
		4 远单飞
		5 远群飞
		6 近单
		7 近群
		8 己单
		9 己群
		10捕捉
		*/
        if (clientSkillType == 1
            || clientSkillType == 6
            || clientSkillType == 7
            || clientSkillType == 10)
        {
            return 0.1f;
        }
        else
        {
            return 0f;
        }
    }

    private static List<long> GetInjureIds(VideoSkillAction gameAction, bool atOnce)
    {
        var injurerIds = new List<long>();
        var _callSoldierIds = new List<long>();
        for (int i = 0, len = gameAction.targetStateGroups.Count; i < len; i++)
        {
            var group = gameAction.targetStateGroups[i];
            var injureId = GetInjureId(group, ref _callSoldierIds);
            if (injureId > 0)
            {
                injurerIds.Add(injureId);
            }
        }

        return injurerIds;
    }

    private static long GetInjureId(VideoTargetStateGroup group, ref List<long> callSoldierIds)
    {
        for (int i = 0, len = group.targetStates.Count; i < len; i++)
        {
            var state = group.targetStates[i];
            if (state is VideoBuffAddTargetState)
            {
                var videoBuffAddTargetState = state as VideoBuffAddTargetState;
                if (callSoldierIds.Contains(videoBuffAddTargetState.id))
                {
                    continue;
                }

                //当只有buff添加state的时候， 才考虑加入受击者，如果不是， 就不需要加入受击者
                if (group.targetStates.Count > 1)
                {
                    continue;
                }
            }
            if (state is VideoRetreatState)
            {
                continue;
            }
            GameDebuger.TODO(@"if (state is VideoRageTargetState)
            {
                continue;
            }

            if (state is VideoCallSoldierState)
            {
                VideoCallSoldierState videoCallSoldierState = state as VideoCallSoldierState;
                if (callSoldierIds.Contains(videoCallSoldierState.soldier.id) == false)
                {
                    callSoldierIds.Add(videoCallSoldierState.soldier.id);
                }
                continue;
            }");


            if (state.id > 0)
            {
                return state.id;
            }
        }

        return 0;
    }

    private float GetVideoRoundActionPlayTime(VideoRoundAction videoRoundAction)
    {
        if (videoRoundAction != null && videoRoundAction.targetStateGroups != null && videoRoundAction.targetStateGroups.Count > 0)
        {
            return 0.2f;
        }
        else
        {
            return 0f;
        }
    }
}

