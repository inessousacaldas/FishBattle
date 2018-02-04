using UnityEngine;
using System.Collections;
using AppDto;
using System;

public class UrgentMissionDelegate:BaseMissionDelegate, IMissionDelegate
{
    public UrgentMissionDelegate(MissionDataMgr.MissionData model,int missionType) : base(model,missionType) {

    }
    public bool AcceptMission()
    {
        return false;
    }

    //紧急委托接受任务后，需要显示警告等字样
    public void AcceptMissionFinish(PlayerMissionDto dto)
    {
        DelegateMissionEffectDataMgr.DelegateMissionEffectPanelLogic.Open();
    }

    public void Dispose()
    {
        
    }

    public bool FindToNpc(Mission mission)
    {
        return false;
    }

    public void FinishMission(PlayerMissionDto dto,SubmitDto submitDto)
    {
        
    }

    public void UpdateSubmitDtoByStateNotify(MissionSubmitStateNotify notify,SubmitDto submitDto)
    {
       
    }

    public bool DropMission(Mission mission,out string winTips)
    {
        winTips = "确认是否放弃该任务?";
        return true;
    }
}
