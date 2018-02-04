using UnityEngine;
using System.Collections;
using System;
using AppDto;
using GamePlot;

public class DefaultMissionDelegate :BaseMissionDelegate, IMissionDelegate
{
    public DefaultMissionDelegate(MissionDataMgr.MissionData model,int missionType) : base(model,missionType){

    }

    public bool AcceptMission() {
        return true;
    }

    public void UpdateSubmitDtoByStateNotify(MissionSubmitStateNotify notify,SubmitDto submitDto) {

    }
    public void FinishMission(PlayerMissionDto dto,SubmitDto submitDto) {

    }

    /// <summary>
    /// 这里会处理 没有在 MissionModel.RegisterMissionDelegate 的任务类型
    /// 主要是有些任务会通过服务器下发通知来接受,而不是 Acceptxxx 接口返回
    /// 当然如果比较复杂还是推荐新的一个 MissionDelegate
    /// 例如 TrialMissionDelegate
    /// </summary>
    /// <param name="dto"></param>
    public void AcceptMissionFinish(PlayerMissionDto dto)
    {
        if(dto.mission.quickFindWay && (_model.GetLastFindMission() != null
          && _model.GetLastFindMission().type == dto.mission.type)
          || (_model.GetLastFindMission() == null && dto.mission.quickFindWay))
        {
            _model.WaitFindToMissionNpc(dto.mission);
        }
    }

    public bool FindToNpc(Mission mission) {
        // 不能寻路任务拦截
        return false;
    }

    public bool DropMission(Mission mission,out string winTips) {
        winTips = "确认是否放弃该任务";
        if(MissionHelper.IsMainMissionType(mission)) {
            TipManager.AddTip(string.Format("不能放弃主线任务"));
            return false;
        }
        return true;
    }

    public void Dispose()
    {

    }
}
