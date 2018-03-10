using UnityEngine;
using System.Collections;
using AppDto;
using System;

public class GuildMissionDelegate : BaseMissionDelegate, IMissionDelegate
{
    public GuildMissionDelegate(MissionDataMgr.MissionData model,int missionType) : base(model,missionType) {

    }

    public bool AcceptMission()
    {
        //检查是否满足接受条件
        return MissionTip();
    }

    public void AcceptMissionFinish(PlayerMissionDto dto)
    {
        if(dto.mission.quickFindWay && (_model.GetLastFindMission() != null
          && _model.GetLastFindMission().type == dto.mission.type)
          || _model.GetLastFindMission() == null)
        {
            _model.FindToMissionNpc(dto.mission);
        }
    }

    public void Dispose()
    {
        
    }

    public bool DropMission(Mission mission,out string winTips)
    {
        winTips = "确认放弃公会任务吗？";
        return true;
    }

    public bool FindToNpc(Mission mission)
    {
        return false;
    }

    public void FinishMission(PlayerMissionDto dto,SubmitDto submitDto)
    {
        ModelManager.Player.StopAutoFram();
    }

    public void ReqEnterMission(Mission mission,SubmitDto submitDto)
    {
        
    }

    public void UpdateSubmitDtoByStateNotify(MissionSubmitStateNotify notify,SubmitDto submitDto)
    {
        
    }

    private bool MissionTip() {
        string tip = "";
        //默认需要飘字
        bool tipSta = true;
        //超过今天可做公会任务环数提示
        MissionStatDto tMissionStatDto = _model.GetMissionStatDto((int)MissionType.MissionTypeEnum.Guild);
        if(tMissionStatDto.daily >= DataCache.GetStaticConfigValue(AppStaticConfigs.GUILD_MISSION_DAILY_FINISH_COUNT))
        {
            tip = MissionDialogue.GuildRingNum;
        }
        _model.GetMissionListDto.ForEach(e =>
        {
            if(e.mission.type == (int)MissionType.MissionTypeEnum.Guild)
            {
                tip = MissionDialogue.GuildAccepEd;
                Npc tBuffyNpc = e.mission.acceptNpc;
                ProxyNpcDialogueView.Close();
                ProxyNpcDialogueView.OpenCustomPanel(tBuffyNpc,tip,null,null);
                tipSta = false;
            }
        });
        bool tSta = string.IsNullOrEmpty(tip);
        if(!tSta && tipSta)
        {
            TipManager.AddTip(tip);
        }
        return tSta;
        //身上已经有任务了
    }
}
