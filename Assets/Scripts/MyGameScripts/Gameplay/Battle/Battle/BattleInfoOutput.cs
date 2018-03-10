// **********************************************************************
// Copyright (c) 2013 Baoyugame. All rights reserved.
// File     :  BattleInfoOutput.cs
// Author   : SK
// Created  : 2013/3/5
// Purpose  : 战斗信息输出
// **********************************************************************

using System.Collections.Generic;
using AppDto;
using MonsterManager = BattleDataManager.MonsterManager;
public static class BattleInfoOutput
{
    //private static float TotalTime = 0f;

    public static void ShowVideoAction(VideoAction gameAction)
    {
        if (!BattleDataManager.DEBUG) return;
        GameDebuger.Log("基础效果:");
        ShowGameVideoStateAction(gameAction);
    }

    private static void ShowGameVideoStateAction(VideoAction gameAction)
    {
        GameDebuger.Log(GetTargetStateGroupsInfo(gameAction));
    }

    public static void showVideoSkillAction(VideoSkillAction gameAction, bool strikeBack = false)
    {
        //TotalTime = 0f;

        if (gameAction.skill == null)
        {
            return;
        }

        var monster = MonsterManager.Instance.GetMonsterFromSoldierID(gameAction.actionSoldierId);
        if (monster == null)
        {
            GameDebuger.Log("Error: attacker not exist id=" + gameAction.actionSoldierId);
            return;
        }

        string desc = (strikeBack ? "[反击]" : "") + monster.GetDebugInfo() + " 使用【" + gameAction.skill.name + "】" + ",造成" +
                      GetTargetStateGroupsInfo(gameAction);
        GameDebuger.LogBattleInfo(desc);

        //GameDebuger.Log("播放时长:" + TotalTime);
    }

    private static string GetTargetStateGroupsInfo(VideoAction gameAction)
    {
        var effects = new List<string>();
        for (int i = 0, len = gameAction.targetStateGroups.Count; i < len; i++)
        {
            var targetStateGroup = gameAction.targetStateGroups[i];
            effects.Add(GetTargetStatesInfo(targetStateGroup));
        }
        return string.Join(";", effects.ToArray());
    }

    private static string GetTargetStatesInfo(VideoTargetStateGroup groups)
    {
        var targetList = new List<long>();
        for (int i = 0, len = groups.targetStates.Count; i < len; i++)
        {
            var targetState = groups.targetStates[i];
            if (targetList.Contains(targetState.id) == false)
            {
                targetList.Add(targetState.id);
            }
        }

        var effects = new List<string>();
        for (int i = 0, len = targetList.Count; i < len; i++)
        {
            long targetId = targetList[i];
            string effectInfo = GetTargetEffectInfo(targetId, groups.targetStates);
            if (!string.IsNullOrEmpty(effectInfo))
            {
                effects.Add(effectInfo);
            }
        }

        if (groups.strikeBackAction != null)
        {
            showVideoSkillAction(groups.strikeBackAction, true);
        }

        return string.Join(",", effects.ToArray());
    }

    private static List<VideoTargetState> getTargetStates(List<VideoTargetState> arr, long petId)
    {
        var states = new List<VideoTargetState>();
        for (int i = 0, len = arr.Count; i < len; i++)
        {
            var state = arr[i];
            if (petId == 0)
            {
                states.Add(state);
            }
            else
            {
                if (state.id == petId)
                {
                    states.Add(state);
                }
            }
        }
        return states;
    }

    private static string GetTargetEffectInfo(long targetId, List<VideoTargetState> stateList)
    {
        stateList = getTargetStates(stateList, targetId);

        string monsterInfo = "";
        var monster = MonsterManager.Instance.GetMonsterFromSoldierID(targetId);
        if (monster == null)
        {
            monsterInfo = "[" + targetId + "]";
            GameDebuger.Log("Error: target not exist! id=" + targetId);
        }
        else
        {
            monsterInfo = monster.GetDebugInfo();
        }

        var effect = new List<string>();
        bool dead = false;
        for (int i = 0, len = stateList.Count; i < len; i++)
        {
            var state = stateList[i];
            if (!dead)
            {
                dead = state.dead;
            }

            if (state is VideoActionTargetState)
            {
                effect.Add(GetActionTargetState(state as VideoActionTargetState));
            }

            if (state is VideoBuffAddTargetState)
            {
                effect.Add(GetBuffAddTargetState(state as VideoBuffAddTargetState));
            }

            if (state is VideoBuffRemoveTargetState)
            {
                effect.Add(GetBuffRemoveTargetState(state as VideoBuffRemoveTargetState));
            }

            if (state is VideoDodgeTargetState)
            {
                effect.Add(GetDodgeTargetState(state as VideoDodgeTargetState));
            }
            if (state is VideoRetreatState)
            {
                effect.Add(GetVideoRetreatState(state as VideoRetreatState));
            }
            
            GameDebuger.TODO(@"if (state is VideoAntiSkillTargetState)
            {
                effect.Add(GetAntiSkillTargetState(state as VideoAntiSkillTargetState));
            }");

            GameDebuger.TODO(@"if (state is VideoRageTargetState)
            {
                effect.Add(GetRageTargetState(state as VideoRageTargetState));
            }");

            GameDebuger.TODO(@"if (state is VideoCallSoldierState)
            {
                effect.Add(GetVideoCallSoldierState(state as VideoCallSoldierState));
            }");

            GameDebuger.TODO(@"if (state is VideoCallSoldierLeaveState)
            {
                effect.Add(GetVideoCallSoldierLeaveState(state as VideoCallSoldierLeaveState));
            }");

            GameDebuger.TODO(@"if (state is VideoSwtichPetState)
            {
                effect.Add(GetVideoSwtichPetState(state as VideoSwtichPetState));
            }");

            GameDebuger.TODO(@"if (state is VideoTargetExceptionState)
            {
                effect.Add(GetVideoTargetExceptionState(state as VideoTargetExceptionState));
            }");
            

            GameDebuger.TODO(@"if (state is VideoCaptureState)
            {
                effect.Add(GetVideoCaptureState(state as VideoCaptureState));
            }");
        }

        if (dead)
        {
            effect.Add("死亡");
        }
        if (effect.Count > 0)
        {
            return monsterInfo + " " + string.Join(",", effect.ToArray());
        }
        return "";
    }

    private static string GetActionTargetState(VideoActionTargetState state)
    {
        var effect = new List<string>();

        if (state.hp != 0)
        {
            effect.Add("HP" + state.hp);
        }

        return string.Join(",", effect.ToArray());
    }

    //
    //	private string getAbsorbState(AbsorbState state)
    //	{
    //		return "吸血"+state.hp+" 吸魔"+state.mp;
    //	}
    //
    private static string GetDodgeTargetState(VideoDodgeTargetState state)
    {
        return "闪避";
    }

    /**private static string GetAntiSkillTargetState(VideoAntiSkillTargetState state)
    {
        return "免疫";
    }*/

    //    private static string GetRageTargetState(VideoRageTargetState state)
    //    {
    //        return "怒气值:" + state.rage;
    //    }

    //    private static string GetVideoCallSoldierState(VideoCallSoldierState state)
    //    {
    //        return "召唤小怪:" + state.soldier.id;
    //    }

    //    private static string GetVideoCallSoldierLeaveState(VideoCallSoldierLeaveState state)
    //    {
    //        return "小怪撤离:" + state.id;
    //    }

    //    private static string GetVideoSwtichPetState(VideoSwtichPetState state)
    //    {
    //        return "召唤宠物:" + state.switchPetSoldier.name;
    //    }

    //    private static string GetVideoTargetExceptionState(VideoTargetExceptionState state)
    //    {
    //        return "异常状态:" + state.message;
    //    }

    private static string GetVideoRetreatState(VideoRetreatState state)
    {
        return "撤退:" + state.success;
    }

    
    //    private static string GetVideoCaptureState(VideoCaptureState state)
    //    {
    //        return "捕捉:" + state.success;
    //    }

    //
    //	private string getNumberEffectState(NumberEffectState state)
    //	{
    //		foreach(NumberEffectDto effect in state.numberEffectDtos){
    //			if (effect.numberEffectId == NumberEffectDto.NumberEffectType_Hp){
    //				return "HP"+effect.value;
    //			}else{
    //				return "MP"+effect.value;
    //			}
    //		}
    //		return "";
    //	}
    //
    //	private string getParrySuccessState(ParrySuccessState state)
    //	{
    //		return "招架";
    //	}
    //
    //	private string getReboundState(ReboundState state)
    //	{
    //		return "反震hp"+state.hp;
    //	}
    //
    //	private string getSquelchState(SquelchState state)
    //	{
    //		return "反击hp"+state.hp;
    //	}
    //
    //	private string getProtectState(ProtectState state)
    //	{
    //		return "保护";
    //	}
    //
    private static string GetBuffAddTargetState(VideoBuffAddTargetState state)
    {
//        GameDebuger.TODO(@"return 'Buff 剩余回合' + state.round + ' id:' + state.battleBuffId + ' 效果:' + state.battleBuff.name;");//legacy 2017-02-27 10:43:14
        return string.Format("Buff id：{0},效果：{1}，持续时间或次数：{2}", state.battleBuffId, state.battleBuff.name, state.durationTime);
    }

    	
    //    private string getBufferClearState(BufferClearState state)
    //    {
    //        return "Buff全清";
    //    }

    private static string GetBuffRemoveTargetState(VideoBuffRemoveTargetState state)
    {
        string ids = "";
        for (int index = 0; index < state.buffId.Count; index++)
        {
            ids += state.buffId[index] + ",";
        }
        return "Buff移除 id:" + ids;
    }

    //
    //	private string getBufferAiTypeClearState(BufferAiTypeClearState state){
    //		return "Buff移除aiType id:"+state.types.ToArray().ToString();
    //	}
    //
    //	private string getTrickState(TrickState state){
    //		return "特技 id:"+state.trickId + " name:"+state.trick.name+" 作用者:"+state.id;
    //	}
    //
    //	private string getAbsorbDamageState(AbsorbDamageState state){
    //		return "吸收伤害";
    //	}

    public static void OutputTargetStateGroups(List<VideoTargetStateGroup> groups)
    {
        string infos = "OutputTargetStateGroups=============";

        for (int i = 0, len = groups.Count; i < len; i++)
        {
            var group = groups[i];
            infos += OutputTargetStates(group.targetStates);
            if (group.strikeBackAction != null)
            {
                infos += "\n\t" + "[反击]";
                if (group.strikeBackAction.targetStateGroups != null &&
                    group.strikeBackAction.targetStateGroups.Count > 0)
                {
                    infos += OutputTargetStates(group.strikeBackAction.targetStateGroups[0].targetStates);
                }
            }
        }

        infos += "\n" + "===============OutputTargetStateGroups";
        GameDebuger.Log(infos);
    }

    private static string OutputTargetStates(List<VideoTargetState> targetStates)
    {
        string infos = "\n\t" + "OutputVideoTargetState-----------------";

        var outputs = GetVideoTargetStates(targetStates);

        for (int i = 0, len = outputs.Count; i < len; i++)
        {
            string info = outputs[i];
            infos += "\n\t" + info;
        }

        infos += "\n\t" + "-----------------OutputVideoTargetState";

        return infos;
    }

    private static List<string> GetVideoTargetStates(List<VideoTargetState> targetStates)
    {
        var outputs = new List<string>();
        for (int i = 0, len = targetStates.Count; i < len; i++)
        {
            var state = targetStates[i];
            outputs.Add(state.ToString());
        }

        return outputs;
    }
}