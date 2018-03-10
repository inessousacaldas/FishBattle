// **********************************************************************
// Copyright (c) 2013 Baoyugame. All rights reserved.
// File     :  GeneralUnit.cs
// Author   : SK
// Created  : 2013/2/4
// Purpose  : 
// **********************************************************************
using UnityEngine;
using AppDto;

public class GeneralUnit : TriggerNpcUnit
{
	private JSTimer.TimerTask _animateTimer;

	protected override bool NeedTrigger ()
	{
		return true;
	}

	protected override void AfterInit ()
	{
		base.AfterInit ();

		InitPlayerName ();

		if (IsMainCharactorModel () == false) {
			_animateTimer = JSTimer.Instance.SetupTimer ("GeneralUnit_" + _unitGo.GetInstanceID(), NPCDoAction, GetAnimateRandomTime ());
		}
	}

	private bool IsMainCharactorModel ()
	{
		return _npcInfo.npcStateDto.npc.modelId < 100;
	}

	public static float GetAnimateRandomTime ()
	{
		return Random.Range (10f, 20f);
	}

	public override void DoTrigger ()
	{
	    base.DoTrigger();
        var npcGeneral = _npcInfo.npcStateDto.npc as NpcGeneral;
        if(!npcGeneral.needDialog && npcGeneral.dialogFunctionIds.Count == 1)
        {
            var missionOptionList = MissionDataMgr.DataMgr.GetMissionOptionListByNpcInternal(npcGeneral);
            if(missionOptionList.Count == 0)
            {
                OpenDefaultDialogueFunction();
                return;
            }
            if(missionOptionList.Count == 1)
            {
                if(missionOptionList[0].isExis)
                {
                    var sumbitDto = MissionHelper.GetSubmitDtoByMission(missionOptionList[0].mission);
                    if((sumbitDto is CollectionItemSubmitDto &&
                         (sumbitDto as CollectionItemSubmitDto).acceptNpc.npc.id == npcGeneral.id))
                    {
                        OpenDefaultDialogueFunction();
                        return;
                    }
                    else {
                        ProxyNpcDialogueView.Open(_npcInfo);
                    }
                }
            }
        }
        else
        {
            ProxyNpcDialogueView.Open(_npcInfo);
        }
        GameDebuger.TODO(@"if (NewBieGuideManager.Instance.IsForceGuideRunning())
        {
            GameDebuger.LogError('存在强制类型的引导， 所以不能跟NPC对话');
            return;
        }

        var npcGeneral = _npcInfo.npcStateDto.npc as NpcGeneral;
        //必须要显示对话框
        if (!npcGeneral.needDialog && npcGeneral.dialogFunctionIds.Count == 1)
        {
            var missionOptionList = ModelManager.MissionNpc.GetMissionOptionListByNpcInternal(npcGeneral);
            if (missionOptionList.Count == 0)
            {
                OpenDefaultDialogueFunction();
                return;
            }
            if (missionOptionList.Count == 1)
            {
                if (missionOptionList[0].isExist)
                {
                    var sumbitDto = MissionHelper.GetSubmitDtoByMission(missionOptionList[0].mission);
                    if ((sumbitDto is CollectionItemSubmitDto &&
                         (sumbitDto as CollectionItemSubmitDto).acceptNpc.npc.id == npcGeneral.id)
                        ||
                        (sumbitDto is CollectionItemCategorySubmitDto &&
                         (sumbitDto as CollectionItemCategorySubmitDto).acceptNpc.npc.id == npcGeneral.id)
                        ||
                        (sumbitDto is CollectionPetSubmitDto &&
                         (sumbitDto as CollectionPetSubmitDto).acceptNpc.npc.id == npcGeneral.id))
                    {
                        OpenDefaultDialogueFunction();
                        return;
                    }
                }
            }
        }

        //节日仙子/红娘先请求数据再打开对话框
        if (_npcInfo.npcStateDto.npcId == ProxyDialogueModule.FESTIVAL_NPC_ID)
        {
            ModelManager.Player.GetFestivalInfoFromServer(() => { ProxyManager.Dialogue.OpenNpcDialogue(_npcInfo); });
        }
        else if (_npcInfo.npcStateDto.npcId == ProxyDialogueModule.Matchmaker_NPC_ID)
        {
            ModelManager.Marry.GetMyMarryInfoDtoFromServer(() => { ProxyManager.Dialogue.OpenNpcDialogue(_npcInfo); });
        }
           else if(_npcInfo.npcStateDto.npcId == ProxyDialogueModule.SWORNBROTHER_NPC_ID)
           {
               ModelManager.BrotherPupil.GetSwornInfo( ()=> { ProxyManager.Dialogue.OpenNpcDialogue(_npcInfo); });                                                                             /// 结拜信息
           }
        else if (_npcInfo.npcStateDto.npcId == ModelManager.DuelData.speciteNpcID) {
        ModelManager.DuelData.CleanDuelInfoDto();
        ProxyManager.Dialogue.OpenNpcDialogue(_npcInfo);
        }else if (_npcInfo.npcStateDto.npcId == ModelManager.CSPK.EntranceNPCId) {
        ModelManager.CSPK.ShowDialogue (_npcInfo);
        }
           else if(_npcInfo.npcStateDto.npcId==ProxyDialogueModule.SHOWSTAGEVIEW)
           {
               ProxyManager.TalentShow.GetShowMatchDate(_npcInfo);
           }
        else
        {
            ProxyManager.Dialogue.OpenNpcDialogue(_npcInfo);
        }");
    }

    private void OpenDefaultDialogueFunction ()
	{
        //ProxyNpcDialogueView.OpenNpcDialogue(_npcInfo);
        NpcGeneral npcGeneral = _npcInfo.npcStateDto.npc as NpcGeneral;
        if(npcGeneral.dialogFunctionIds.Count > 0) {
            DialogFunction dialogFunction = DataCache.getDtoByCls<DialogFunction>(npcGeneral.dialogFunctionIds[0]);
            DialogueHelper.OpenDialogueFunction(_npcInfo.npcStateDto,dialogFunction);
        }

    }

	private void NPCDoAction ()
	{
		if (_animateTimer != null) {
			_animateTimer.Reset (NPCDoAction, GetAnimateRandomTime (), false);
		}
        if (CheckHaveRide() == false)            //有坐骑的时候 没有show 动作
            DoAction (ModelHelper.AnimType.show, true);
	}

	public override void Destroy ()
	{
		if (_animateTimer != null) {
			_animateTimer.Cancel ();
			_animateTimer = null;
		}
		base.Destroy ();
	}
}

