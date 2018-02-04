using UnityEngine;
using System;
using AppDto;

public class FactionMissionDelegate :BaseMissionDelegate,IMissionDelegate
{
    public FactionMissionDelegate(MissionDataMgr.MissionData model,int missionType) : base(model,missionType) {

    }

    public bool AcceptMission() {
        return true;
    }

    public void AcceptMissionFinish(PlayerMissionDto dto) {
        //if(_model.GetLastSubmitMission() != null && _model.GetLastSubmitMission().type == (int)MissionType.MissionTypeEnum.Faction || _model.GetLastSubmitMission() == null) {
        //    _model.FindToMissionNpc(dto.mission);
        //}
        //稍微修改完成任务自动寻路逻辑
        if(dto.mission.quickFindWay && (_model.GetLastFindMission() != null 
            && _model.GetLastFindMission().type == dto.mission.type)
            || _model.GetLastFindMission() == null)
        {
            _model.FindToMissionNpc(dto.mission);
        }
    }

    public void UpdateSubmitDtoByStateNotify(MissionSubmitStateNotify notify,SubmitDto submitDto) {

    }

    public void FinishMission(PlayerMissionDto dto,SubmitDto submitDto) {
        PlayerFactionMissionDto tPlayerFactionMissionDto = dto as PlayerFactionMissionDto;
        ModelManager.Player.StopAutoFram();
    }

    public bool FindToNpc(Mission mission) {
        return false;
    }

    public void Dispose() {

    }

    public bool DropMission(Mission mission,out string winTips)
    {
        winTips = "确认是否放弃该任务";
        return true;
    }
}
