// **********************************************************************
//	Copyright (C), 2011-2015, CILU Game Company Tech. Co., Ltd. All rights reserved
//	Work:		For H1 Project With .cs
//  FileName:	MonsterUnit.cs
//  Version:	Beat R&D

//  CreatedBy:	_Alot
//  Date:		2015.05.07
//	Modify:		__

//	Url:		http://www.cilugame.com/

//	Description:
//	This program files for detailed instructions to complete the main functions,
//	or functions with other modules interface, the output value of the range,
//	between meaning and parameter control, sequence, independence or dependence relations
// **********************************************************************

using UnityEngine;
using AppDto;

public class MonsterUnit : TriggerNpcUnit
{

	private JSTimer.TimerTask _animateTimer = null;

	protected override bool NeedTrigger ()
	{
		return true;
	}

	protected override void AfterInit ()
	{
		base.AfterInit ();

		InitPlayerName ();
		
		if (IsMainCharactorModel () == false) {
			if (_animateTimer == null) {
				_animateTimer = JSTimer.Instance.SetupTimer ("MonsterUnit_" + _unitGo.GetInstanceID(), NPCDoAction, GetAnimateRandomTime ());
			}
		}
	}

    protected override void SetupBoxCollider()
    {
        _boxCollider = _unitGo.GetMissingComponent<BoxCollider>();
        _boxCollider.isTrigger = true;
        _boxCollider.center = new Vector3(0f, 0.75f * GetNpcScale(), 0f);
        _boxCollider.size = new Vector3(GetNpcScale(), GetNpcScale(), GetNpcScale());

        GameDebuger.TODO(@"bool tIsDreamlandMonster = ModelManager.DreamlandData.IsInDreamlandScene() && _npcInfo.npcStateDto.npc is NpcSceneMythLandMonster;");
        bool tIsDreamlandMonster = false;
		_unitGo.tag = tIsDreamlandMonster? GameTag.Tag_DreamlandNpc : GameTag.Tag_Npc;
    }

	protected override void UpdateNpcPosition ()
	{
		GameDebuger.TODO(@"if (_npcInfo.npcStateDto.npc is NpcDynamicMonster || _npcInfo.npcStateDto.npc is NpcSceneMonster) {
            //  默认朝向玩家，策划需求加入随机值
            _npcInfo.npcStateDto.npc.rotateY = Random.rotation.eulerAngles.y;
        }");
		base.UpdateNpcPosition ();
	}

	private bool IsMainCharactorModel ()
	{
		return _npcInfo.npcStateDto.npc.modelId < 100;
	}

	private float GetAnimateRandomTime ()
	{
		return Random.Range (10f, 20f);
	}

	public override void Trigger ()
	{
        base.Trigger();
        //ProxyNpcDialogueView.Open(_npcInfo);


        GameDebuger.TODO(@"if (ModelManager.DreamlandData.IsInDreamlandScene() && _npcInfo.npcStateDto.npc is NpcSceneMythLandMonster) {
            //  怪物战斗请求（按场景怪物战斗请求方式）
            ModelManager.DreamlandData.DreamlandMonstert(_npcInfo.npcStateDto);
        } else {
            base.Trigger();
        }");
	}

	public override void DoTrigger ()
	{
		bool tNormalSta = false;
        base.DoTrigger();
        //	明雷怪物点击回调
        ProxyNpcDialogueView.Open(_npcInfo);
        GameDebuger.TODO(@"if (ModelManager.DreamlandData.IsInDreamlandScene()) {
            if (_npcInfo.npcStateDto.npc is NpcSceneMythLandMonster) {
                //  幻境怪物 不做操作
                return;
            } else if (_npcInfo.npcStateDto.npc is NpcSceneMythLandTransfer) {
                tNormalSta = true;
            } else if (_npcInfo.npcStateDto.npc is NpcSceneMythLandBox) {
                //  幻境宝箱开启处理
                base.DoTrigger();

                ModelManager.DreamlandData.DreamlandBox(_npcInfo.npcStateDto);
            }else if(_npcInfo.npcStateDto.npc is NpcSceneWorldGhostBox)
                  {
                      //世界Boss宝箱开启处理
                      base.DoTrigger();
                  }
                  else {
                tNormalSta = true;
            }
        }
        else {
            tNormalSta = true;
        }

        if (tNormalSta) {
            base.DoTrigger();
                  //    明雷怪物点击回调
                  ProxyManager.Dialogue.OpenNpcDialogue(_npcInfo);
        }");
	}

	void NPCDoAction ()
	{
		if (_animateTimer != null) {
			_animateTimer.Reset (NPCDoAction, GetAnimateRandomTime (), false);
		}
        if(CheckHaveRide() == false)            //有坐骑的时候 没有show 动作
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

