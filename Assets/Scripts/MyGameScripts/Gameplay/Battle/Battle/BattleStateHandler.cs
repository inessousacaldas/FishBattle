//using AppDto;
//using AppDto;
using System.Collections.Generic;
using AppDto;
using MonsterManager = BattleDataManager.MonsterManager;

public static class BattleStateHandler
{

	public static void PlayVideoSkillAction(this MonsterController mc, VideoSkillAction action)
	{
		if (mc == null) return;
		mc.ClearMessageEffect(true);
		mc.modifyHP = action.hpSpent;
		mc.modifyEP = action.epSpent;
		mc.modifyCP = action.cpSpent;
		mc.lastHP = mc.videoSoldier.hp + action.hpSpent;
		mc.lastEP = mc.videoSoldier.ep + action.epSpent;
		mc.lastCP = mc.videoSoldier.cp + action.cpSpent;
		
		mc.PlayInjure();
	}

	public static void PlayState(this MonsterController mc, VideoTargetState bas, bool handleAll=false)
	{
		if (mc != null)
		{
			mc.ClearMessageEffect(true);
		}
		else
		{
			GameDebuger.TODO(@"if (bas is VideoCallSoldierState)
            {
                VideoCallSoldierState videoCallSoldierState = bas as VideoCallSoldierState;
                         BattleController.Instance.CallPet(videoCallSoldierState.soldier);
            }");
			return;
		}

        mc.UpdateHpEpCp(bas);
		if (bas is VideoActionTargetState)
		{
			var action = (VideoActionTargetState)bas;

			if ( action.crit )
				mc.AddMessageEffect( MonsterController.ShowMessageEffect.CRITICAL );
		}
		else if (bas is VideoDodgeTargetState)
		{
			mc.AddMessageEffect( MonsterController.ShowMessageEffect.DODGE );
            mc.PlaySkillName("躲闪");
        }
        else if (bas is VideoBuffAddTargetState)
        {
            mc.AddBuffState((VideoBuffAddTargetState) bas);
        }
        else if (bas is VideoBuffRemoveTargetState)
        {
            mc.RemoveBuffs( (VideoBuffRemoveTargetState)bas );
        }
		else if (bas is VideoSoldierSwtichState)
		{
			var tVideoSwtichPetState = bas as VideoSoldierSwtichState;
			var soldier = tVideoSwtichPetState.soldier;
			if (soldier != null)
			{
				MonsterManager.Instance.SwitchSolider(soldier);
			}
		}
		GameDebuger.TODO(@"else if (bas is VideoAntiSkillTargetState)
        {
            mc.AddMessageEffect( MonsterController.ShowMessageEffect.IMMUNE );
        }
       
        else if (bas is VideoRageTargetState)
        {
              }
        else if (bas is VideoTargetShoutState)
        {
            return;
        }
        else if (bas is VideoCallSoldierLeaveState)
        {
            VideoCallSoldierLeaveState rageTargetState = bas as VideoCallSoldierLeaveState;
            MonsterController leaveSoldier = MonsterManager.Instance.GetMonsterFromSoldierID (rageTargetState.id);
            if (leaveSoldier != null)
            {
                leaveSoldier.LeaveBattle();
            }
            return;
        }
        else if (bas is VideoTargetExceptionState)
        {
            //TODO@senkay 竞技场考虑对方打我的时候， 如果对方有异常状态， 也要显示
            if (mc.IsPlayerCtrlCharactor())
            {
                VideoTargetExceptionState targetExceptionState = bas as VideoTargetExceptionState;
                TipManager.AddTip(targetExceptionState.message);
            }
        }");

		if (handleAll)
		{
			if (bas is VideoRetreatState)
            {
                var retreatState = bas as VideoRetreatState;
                if (retreatState.success)
                {
                    mc.RetreatFromBattle(MonsterController.RetreatMode.Run, 1f);
                }
            }
		}

		mc.dead = bas.dead;
		mc.leave = bas.leave;
		if (bas is VideoDrivingTargetState)
		{
			var value = (bas as VideoDrivingTargetState).driving;
			if (mc.driving && !value)
				mc.ClearSkill(); // 被打清空技能选择
			mc.driving = value;
		}
		mc.PlayInjure();
	}

	public static void HanderOtherTargetState(List<long> checkList, List<VideoTargetState> targetStates/**, BattleController bc*/){//useless bc was deleted in 2017-01-20 17:25:2

		var states = new List<VideoTargetState>();

		for (int i=0,len=targetStates.Count; i<len; i++)
		{
			var state = targetStates[i];
			if(!checkList.Contains(state.id))
			{
				states.Add(state);
			}
		}

		for (int i=0,len=states.Count; i<len; i++)
		{
			var doState = states[i];
			PlayState(MonsterManager.Instance.GetMonsterFromSoldierID(doState.id), doState);
		}
	}

	public static void HandleBattleStateGroup(
		long petId, 
		List<VideoTargetStateGroup> targetStateGroups, 
		bool handleAll = false)
	{

		for (int i=0,len=targetStateGroups.Count; i<len; i++)
		{
			var group = targetStateGroups[i];
			HandleBattleState(petId, group.targetStates, handleAll);
		}
	}

//	static public void CheckDeadStates(List<VideoTargetState> targetStates)
//	{
//		foreach (VideoTargetState state in targetStates)
//		{
//			CheckDeadState(state);
//		}
//	}

	public static void CheckDeadState(MonsterController mc, VideoTargetState bas)
	{
		float modifyHP = 0;
		if (bas is VideoActionTargetState)
		{
			var action = (VideoActionTargetState)bas;
			
			modifyHP = action.hp;
		}

		if (mc == null) return;
		mc.dead = bas.dead;
		mc.leave = bas.leave;
	}

	public static void HandleBattleState(
		long petId, 
		List<VideoTargetState> targetStates, 
		bool handleAll = false)
	{
		HandleAllBattleState(targetStates, handleAll);
//		if (petId == 0) 
//		{
//			HandleAllBattleState(targetStates, bc);
//		}
//		else 
//		{
//			List<VideoTargetState> newStates = getTargetStates(targetStates, petId);
//			HandleAllBattleState(newStates, bc);
//		}
	}

	public static void HandleAllBattleState(
		List<VideoTargetState> targetStates, 
		bool handleAll = false)
	{
		targetStates.ForEach(state =>
		{
			if (state.id == 0)
			{
				GameDebuger.LogError("state.id = 0");
			}
			var mc = MonsterManager.Instance.GetMonsterFromSoldierID(state.id);
			mc.PlayState(state, handleAll);
		});
	}

	public static VideoTargetState getTargetState(List<VideoTargetState> arr, long petId)
	{
		VideoTargetState select = null;
		var count = 0;

		for (int i=0,len=arr.Count; i<len; i++)
		{
			var state = arr[i];
			if(state.id == petId)
			{
				if (select == null)
				{
					select = state;
				}
				count++;
			}
		}

		if (select != null && count > 1)
		{
			//这里因为要针对不同的攻击次数做状态处理， 所以把相同状态旧的删掉，只保留最后一个做后续的死亡判断
			arr.Remove(select);
		}

		return select;
	}
}
