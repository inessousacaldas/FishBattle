using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AppDto;

public class NpcSceneCollectionUnit:TriggerNpcUnit
{
    public Mission _mission;
    public override void DoTrigger()
    {
        base.DoTrigger();
        //寻找当前NPC的身上的所有任务
        if(_mission != null)
        {
            waitingTrigger = false;
            touch = false;
            ShowOpening();
        }
        else
        {
            GameDebuger.LogError("该采集物身上没有任何任务，不应该出现在场景里面");
        }
    }

    private void ShowOpening()
    {
        //Mission mission = _missionOptionList[0].mission;
        SubmitDto tSubmitDto = MissionHelper.GetSubmitDtoByMission(_mission);
        PickItemSubmitInfoDto tApplyItemSubmitDto = tSubmitDto as PickItemSubmitInfoDto;
        GeneralItem tAppMissionItem = tApplyItemSubmitDto.item as GeneralItem;
        ProxyProgressBar.ShowProgressBar(tAppMissionItem.icon,"采集中...",null,"__PopupUseMissionProps",delegate
        {
            MissionDataMgr.MissionNetMsg.PickItemMission(_npcInfo.npcStateDto.id,_mission.id);
            //  【任务】使用道具的任务，在道具使用完成后，需要弹出一个飘字（客户端）
            //  http://oa.cilugame.com/redmine/issues/9116
            //TipManager.AddTip(tAppMissionItem.finishTip);
        },3f);
    }


    /// <summary>
    /// 设置生成采集物名字
    /// </summary>
    protected override void AfterInit()
    {
        base.AfterInit();
        InitPlayerName();
    }

    /// <summary>
    /// 设置采集物的碰撞体大小
    /// </summary>
    protected override void SetupBoxCollider()
    {
        _boxCollider = _unitGo.GetMissingComponent<BoxCollider>();
        _boxCollider.isTrigger = true;
        _boxCollider.center = new Vector3(0,0.35f,0f);
        _boxCollider.size = new Vector3(1f,0.7f,0.7f);
        _unitGo.tag = GameTag.Tag_Npc;
    }

    protected override bool NeedTrigger()
    {
        return true;
    }
}
