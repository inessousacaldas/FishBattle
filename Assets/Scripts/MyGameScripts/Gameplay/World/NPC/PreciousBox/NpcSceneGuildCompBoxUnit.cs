// **********************************************************************
// Copyright (c) 2016 cilugame. All rights reserved.
// File     : NpcSceneGuildCompBoxUnit.cs
// Author   : senkay <senkay@126.com>
// Created  : 4/18/2016 
// Porpuse  : 
// **********************************************************************
//

using UnityEngine;
using AppServices;
using AppDto;

public class NpcSceneGuildCompBoxUnit : TriggerNpcUnit
{
	public override void DoTrigger()
	{
		waitingTrigger = false;
		touch = false;

		ShowOpening();
	}

	private void ShowOpening()
	{
		GameDebuger.TODO(@"if (ModelManager.GuildCompetitionData.HaveOpenBox())
        {
            TipManager.AddTip(string.Format('你已开启了一个{0}，留给其他人吧',_npcInfo.name));
            return;
        }

        MainUIViewController.DataMgr.SetMissionUsePropsProgress(true, '正在开启宝箱……', CancelOpen);
        JSTimer.DataMgr.SetupCoolDown('OpenNpcSceneGuildCompBox', 3f, (remainTime) => {
            MainUIViewController.DataMgr.SetMissionUsePropsProgress(1 - remainTime / 3f);
        }, () => {
            MainUIViewController.DataMgr.SetMissionUsePropsProgress(false, "");

            WorldModel worldModel = WorldManager.DataMgr.GetModel();
            if (!worldModel.NpcsDic.ContainsKey(_npcInfo.npcStateDto.id))
            {
                TipManager.AddTip(string.Format('{0}已经消失了',_npcInfo.name));
                return;
            }

            ModelManager.GuildCompetitionData.GuildCompBox(_npcInfo.npcStateDto);
        }, 0);");
	}

	private void CancelOpen()
	{
		TipManager.AddTip("你放弃了宝箱");
		JSTimer.Instance.CancelCd("OpenNpcSceneGuildCompBox");
	}

	protected override bool NeedTrigger()
	{
		return true;
	}

	protected override void SetupBoxCollider()
	{
		if (_unitGo != null)
		{
			_boxCollider = _unitGo.GetMissingComponent<BoxCollider>();
			_boxCollider.isTrigger = true;
			_boxCollider.center = new Vector3(0f, 0.35f, 0f);
			_boxCollider.size = new Vector3(1f, 0.7f, 0.7f);
			_unitGo.tag = GameTag.Tag_Npc;
		}
		else
		{
			Debug.LogError("!!!!!! _unitGo = null");
		}
	}

	protected override void AfterInit()
	{
		base.AfterInit();

		InitPlayerName();
	}
}
