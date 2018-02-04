using UnityEngine;
using System.Collections;
using AppDto;
using System;

public class CopyMissionDelegate:BaseMissionDelegate, IMissionDelegate
{
    public CopyMissionDelegate(MissionDataMgr.MissionData model,int missionType):base(model,missionType)
    {

    }
    public bool AcceptMission()
    {
        return true;
    }

    public void AcceptMissionFinish(PlayerMissionDto dto)
    {
        if(dto.mission.quickFindWay && (_model.GetLastFindMission() != null
        && _model.GetLastFindMission().type == dto.mission.type)
        || _model.GetLastFindMission() == null)
        {
            _model.WaitFindToMissionNpc(dto.mission);
        }
    }

    public void Dispose()
    {
        
    }

    public bool DropMission(Mission mission,out string winTips)
    {
        winTips = "确认是否放弃该任务";
        if(MissionHelper.IsMainMissionType(mission))
        {
            TipManager.AddTip(string.Format("不能放弃主线任务"));
            return false;
        }
        return true;
    }

    public bool FindToNpc(Mission mission)
    {
        return false;
    }

    public void FinishMission(PlayerMissionDto dto,SubmitDto submitDto)
    {
        PlayerCopyMissionDto mPlayerCopyMissionDto = dto as PlayerCopyMissionDto;
        if(mPlayerCopyMissionDto != null && mPlayerCopyMissionDto.finish) {
            TipManager.AddTip("已经获得过该关卡奖励");
        }
    }

    public void UpdateSubmitDtoByStateNotify(MissionSubmitStateNotify notify,SubmitDto submitDto)
    {

    }
}
