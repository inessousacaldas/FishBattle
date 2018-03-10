using UnityEngine;
using System.Collections;
using AppDto;
using System;

public class CopyExtraMissionDelegate : BaseMissionDelegate,IMissionDelegate {
    public CopyExtraMissionDelegate(MissionDataMgr.MissionData model,int missionType) : base(model,missionType) {

    }

    public bool AcceptMission() {
        return false;
    }

    public void AcceptMissionFinish(PlayerMissionDto dto) {
        ProxySpecialCopy.Open(dto.missionId);
    }

    public void Dispose() {

    }

    public bool FindToNpc(Mission mission) {
        return false;
    }

    public void FinishMission(PlayerMissionDto dto,SubmitDto submitDto) {

    }

    public void UpdateSubmitDtoByStateNotify(MissionSubmitStateNotify notify,SubmitDto submitDto) {

    }

    public bool DropMission(Mission mission,out string winTips) {
        TipManager.AddTip("彩蛋任务不能放弃");
        winTips = "彩蛋任务不能放弃";
        return false;
    }

    public void ReqEnterMission(Mission mission,SubmitDto submitDto)
    {
        
    }
}
