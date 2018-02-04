using UnityEngine;
using System.Collections.Generic;
using AppDto;
using System;

public class GhostMissionDelegate : BaseMissionDelegate,IMissionDelegate {
    private Dictionary<int,MissionType> _missionTypeDic = new Dictionary<int, MissionType>();
    public GhostMissionDelegate(MissionDataMgr.MissionData model,int missionType) :base(model,missionType) {
        _missionTypeDic = DataCache.getDicByCls<MissionType>();
    }

    public bool AcceptMission() {
        //检查是否满足捉鬼条件
        if(MissionTip(true,true)) {
            return false;
        }
        CanAccepMission();
        return false;
    }

    /// <summary>
    /// 接受安全巡逻任务
    /// </summary>
    void CanAccepMission() {
        bool tAccepMission = true;
        Mission tMission = null;
        var tMissionListDto = _model.GetMissionListDto.ToList();
        for(int i = 0; i < tMissionListDto.Count;i++) {
            if(MissionHelper.IsBuffyMissionType(tMissionListDto[i].mission)) {
                tAccepMission = false;
                tMission = tMissionListDto[i].mission;
                break;
            }
        }
        if(tAccepMission)
            MissionDataMgr.MissionNetMsg.AccepGhostMisson();
        if(tMission != null) {
            _model.WaitFindToMissionNpc(tMission);
            TipManager.AddTip(MissionDialogue.ReceivedGhostDelegate);
        }
        ProxyNpcDialogueView.Close();
    }

    public void AcceptMissionFinish(PlayerMissionDto dto) {
        if(dto.mission.quickFindWay && (_model.GetLastFindMission() != null
         && _model.GetLastFindMission().type == dto.mission.type)
         || _model.GetLastFindMission() == null)
        {
            _model.FindToMissionNpc(dto.mission);
        }
        //_model.WaitFindToMissionNpc(dto.mission);
    }

    public void UpdateSubmitDtoByStateNotify(MissionSubmitStateNotify notify,SubmitDto submitDto) {

    }

    public void FinishMission(PlayerMissionDto dto,SubmitDto submitDto) {
        PlayerGhostMissionDto tPlayerGhostMissionDto = dto as PlayerGhostMissionDto;
        if(tPlayerGhostMissionDto.mission.next == null)
        {
            MissionDataMgr.MissionData.mGhostRingCount--;
            WorldManager.Instance.FlyToByNpc(tPlayerGhostMissionDto.mission.missionType.acceptNpc);
        }
        else
        {
            //FindToAcceptNpc(tPlayerGhostMissionDto);
        }
        //GameDebuger.LogError(tGhostMissionDto.curRings + "," + tGhostMissionDto.ghostRingCount + "," + tGhostMissionDto.toDayFinishCount);
    }

    public bool FindToNpc(Mission mission)
    {
        return false;
    }

    public void Dispose()
    {
        _missionTypeDic.Clear();
    }

    public bool MissionTip(bool tipSta,bool acceptSta) {
        string tip = "";
        int minLv = MissionDialogue.minLevel;
        bool isMaxBuffRing = IsSpecifiMaxRing();
        bool isTeamLeader = TeamDataMgr.DataMgr.IsLeader();
        int memberCount = GetTeamDtoWithMinLvCount(minLv);
        if(isTeamLeader && !string.IsNullOrEmpty(GetTeamDtoListWithMinLvStr(minLv)))
        {
            tip = string.Format(MissionDialogue.TeamWithMinLv,minLv,GetTeamDtoListWithMinLvStr(minLv));
        }
        else if(isTeamLeader && memberCount >= MissionDialogue.minTeamNum) {
            if(isMaxBuffRing) {
                tip = string.Format(MissionDialogue.NotGroupLeader,MissionDialogue.maxBuffyRingNum.ToString());
                if(acceptSta)
                {
                    tipSta = false;
                    ProxyWindowModule.OpenConfirmWindow(MissionDialogue.GhostNoMaxRing,MissionDialogue.GhostTitle,delegate
                    {
                        CanAccepMission();
                    });
                }
                else {
                    TipManager.AddTip(tip);
                }
            }
        }
        else if(!isTeamLeader) {
            tip = MissionDialogue.NotLeaderTip;
            if(acceptSta) {
                tipSta = false;
                ProxyNpcDialogueView.Close();
                //不是队长
                Npc tBuffyNpc = _missionTypeDic[(int)MissionType.MissionTypeEnum.Ghost].acceptNpc;
                ProxyNpcDialogueView.OpenCustomPanel(tBuffyNpc,tip,null,null);
            }
            else
            {
                TipManager.AddTip(tip);
            }
        }
        else if(isTeamLeader && memberCount < MissionDialogue.minTeamNum) {
            tip = MissionDialogue.BattleMinTeam;
            if(acceptSta) {
                tipSta = false;
                Npc tBuffyNpc = _missionTypeDic[(int)MissionType.MissionTypeEnum.Ghost].acceptNpc;
                ProxyNpcDialogueView.Close();
                ProxyNpcDialogueView.OpenCustomPanel(tBuffyNpc,tip,null,null);
            }
            else
            {
                ProxyWindowModule.OpenConfirmWindow(tip,"安全巡查",delegate() {
                    DialogFunction dialogFunction=DataCache.getDtoByCls<DialogFunction>(14);
                    var teamAction = DataCache.getDtoByCls<TeamActionTarget>(dialogFunction.logicId);
                    var teamTarget = TeamMatchTargetData.Create(dialogFunction.logicId, teamAction.maxGrade, teamAction.minGrade, true);
                    TeamDataMgr.DataMgr.GetCurTeamMatchTargetData = teamTarget;
                    ProxyTeamModule.OpenMainView(TeamMainViewTab.CreateTeam);
                },null,UIWidget.Pivot.Left,null,null);
            }
        }
        else {
            if(isMaxBuffRing) {
                tip = string.Format(MissionDialogue.NotGroupLeader,MissionDialogue.maxBuffyRingNum);
                if(acceptSta)
                {
                    ProxyWindowModule.OpenConfirmWindow(MissionDialogue.GhostNoMaxRing,MissionDialogue.GhostTitle,delegate
                    {
                        CanAccepMission();
                    });
                }
                else {
                    TipManager.AddTip(tip);
                }
            }
        }
        bool tSta = !string.IsNullOrEmpty(tip);
        if(tSta && tipSta) {
            TipManager.AddTip(tip);
        }
        return tSta;
    }

    //检查剩余点数
    private bool IsSpecifiMaxRing() {
        if(MissionDataMgr.MissionData.mGhostRingCount < 0)
            return true;
        return false;
    }

    #region 查找队伍列表里面符合捉鬼等级的人员
    /// <summary>
    /// 如果查找符合最小等级人数，如果符合条件人数+1；
    /// </summary>
    /// <param name="minLv">符合的人数</param>
    /// <returns></returns>
    public int GetTeamDtoWithMinLvCount(int minLv) {
        TeamDto tTeamDto = TeamDataMgr.DataMgr.GetSelfTeamDto();
        if(tTeamDto == null) return 0;
        List<TeamMemberDto> mTeamMemberDto = tTeamDto.members;
        int result = 0;
        for(int i = 0;i < mTeamMemberDto.Count;i++) {
            TeamMemberDto memberDto = mTeamMemberDto[i];
            if(memberDto.memberStatus == (int)TeamMemberDto.TeamMemberStatus.Member ||
                memberDto.memberStatus == (int)TeamMemberDto.TeamMemberStatus.Leader) {
                if(minLv == 0 || memberDto.grade >= minLv)
                    result++;
            }
        }
        return result;
    }
    #endregion

    #region 返回等级参加队员的信息
    /// <summary>
    /// 返回不够等级参加队员的信息
    /// </summary>
    /// <param name="minLv"></param>
    /// <returns></returns>
    public string GetTeamDtoListWithMinLvStr(int minLv) {
        TeamDto tTeamDto = TeamDataMgr.DataMgr.GetSelfTeamDto();
        if(tTeamDto == null) return "";
        List<TeamMemberDto> mTeamMemberDto = tTeamDto.members;
        System.Text.StringBuilder strBuilder = new System.Text.StringBuilder();
        for(int i = 0, max = mTeamMemberDto.Count;i < max;i++) {
            TeamMemberDto memberDto = mTeamMemberDto[i];
            if(memberDto.memberStatus == (int)TeamMemberDto.TeamMemberStatus.Member
                || memberDto.memberStatus == (int)TeamMemberDto.TeamMemberStatus.Leader) {
                if(minLv == 0 || memberDto.grade < minLv) {
                    strBuilder.Append(memberDto.nickname.WrapColor(ColorConstantV3.Color_ChatNameBlue));
                    strBuilder.Append(",");
                }
            }
        }
        if(strBuilder.Length > 0)
            strBuilder.Remove(strBuilder.Length - 1,1);
        return strBuilder.ToString();
    }

    public bool DropMission(Mission mission,out string winTips)
    {
        winTips = "确认是否放弃该任务";
        return true;
    }
    #endregion
}
