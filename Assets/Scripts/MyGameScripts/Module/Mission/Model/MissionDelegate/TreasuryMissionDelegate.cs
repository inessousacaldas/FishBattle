using UnityEngine;
using System.Collections;
using AppDto;
using System;

public class TreasuryMissionDelegate : BaseMissionDelegate,IMissionDelegate {
    private int TreasureMaxCount = DataCache.GetStaticConfigValue(AppStaticConfigs.TREASURY_MISSION_REWARD_MAX_COUNT);
    public TreasuryMissionDelegate(MissionDataMgr.MissionData model,int missionType) : base(model,missionType) {

    }

    public bool AcceptMission() {
        return true;
    }

    public void AcceptMissionFinish(PlayerMissionDto dto) {
        if(dto.mission.quickFindWay && (_model.GetLastFindMission() != null
         && _model.GetLastFindMission().type == dto.mission.type)
         || _model.GetLastFindMission() == null) { 
            _model.FindToMissionNpc(dto.mission);
        }
    }

    public void Dispose()
    {
        
    }

    public bool DropMission(Mission mission,out string winTips)
    {
        winTips = "确认是否放弃该任务";
        return true;
    }

    public bool FindToNpc(Mission mission)
    {
        return false;
    }

    public void FinishMission(PlayerMissionDto dto,SubmitDto submitDto)
    {
        if(_model.GetMissionStatDto(dto.mission.type).daily >= TreasureMaxCount)
            TipManager.AddTip(string.Format("今天已获得{0}张藏宝图，无法继续宝图任务",TreasureMaxCount));
    }

    public void ReqEnterMission(Mission mission,SubmitDto submitDto)
    {
        
    }

    public void UpdateSubmitDtoByStateNotify(MissionSubmitStateNotify notify,SubmitDto submitDto)
    {
        
    }
}
