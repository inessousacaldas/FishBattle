
using System;
using AppDto;
using UniRx;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace StaticInit
{
    using StaticDispose;
    public partial class StaticInit 
    {
        private StaticDelegateRunner initTeamDataMgr = new StaticDelegateRunner(
            ()=>{  var mgr = TeamDataMgr.DataMgr;});
    }
}

public sealed partial class TeamDataMgr
{
    public static readonly int TeamDefaultMemberCnt = 4;  // todo fish:用到的地方可能要改为最大队员数
    public static readonly int PlayerTeamCnt = 4;  // todo fish:玩家自己的武将队伍，也不知道这个值有没有改变
    public static readonly int ApplicationShowItemNum = 6;   //  申请玩家界面显示6个
    public static readonly int TeamDefaultShowInviteCnt = 5;
    public static readonly int TeamDefaultShowApplyCnt = 20;  // 显示20条
    public static readonly int TeamDefaultSaveApplyCnt = 50;  // 保存50条

    public static readonly int CreateTeamItemMax = 6;       //组队平台界面显示6个玩家item
    public static int CreateTeamRefreshTime = 10;  //刷新时间间隔10s
    public static readonly bool CreateTeamIsCanRefresh = true;

    private void LateInit()
    {
        _disposable.Add(NotifyListenerRegister.RegistListener<TeamDto>(HandleTeamDtoNotify));
        _disposable.Add(NotifyListenerRegister.RegistListener<TeamStatusNotify>(HandleTeamStatusNotify));
        _disposable.Add(NotifyListenerRegister.RegistListener<LeaveTeamNotify>(HandleLeaveTeamNotify));
        _disposable.Add(NotifyListenerRegister.RegistListener<JoinTeamNotify>(HandleJoinTeamNotify));
        _disposable.Add(NotifyListenerRegister.RegistListener<TeamInvitationNotify>(HandleTeamInvitationNotify));
        _disposable.Add(NotifyListenerRegister.RegistListener<TeamRequestNotify>(HandleTeamRequestNotify));
        
        _disposable.Add(NotifyListenerRegister.RegistListener<TeamNearbyDto>(HandlerNearbyPlayerNotify));
        _disposable.Add(NotifyListenerRegister.RegistListener<TeamsDto>(HandlerTeamPanelNotify));
        _disposable.Add(NotifyListenerRegister.RegistListener<TeamCommanderNotify>(HandlerTeamCommandNotify));
        _disposable.Add(NotifyListenerRegister.RegistListener<CallMemberNotify>(HandlerCallMemberNotify));
        _disposable.Add(NotifyListenerRegister.RegistListener<MatchBtnNotify>(HandlerMatchNotify));
        _disposable.Add(NotifyListenerRegister.RegistListener<TeamMemberDto>(HandlerTeamMemberNotify));
        _disposable.Add(NotifyListenerRegister.RegistListener<FormationInfoDto>(RefeshTeamFormationLv));
        _disposable.Add(NotifyListenerRegister.RegistListener<RefuseTeamRequestNotify>(RefuseTeamRequest));
        _disposable.Add(NotifyListenerRegister.RegistListener<RefuseTeamInvitationNotify>(RefuseTeamInvitation));
        stream.OnNext(_data);
    }

    public void OnDispose(){
    }

    private void HandleTeamDtoNotify(TeamDto notify)
    {
        _data.teamDto = notify;
        stream.OnNext(_data);
    }

    private void HandlerTeamMemberNotify(TeamMemberDto memberDto)
    {
        var member = _data.teamDto.members.Find(d => d.id == memberDto.id);
        member.grade = memberDto.grade;
        _data.teamDto.members.ReplaceOrAdd(d => d.id == member.id, member);
        stream.OnNext(_data);
    }

    private void RefuseTeamRequest(RefuseTeamRequestNotify notify)
    {
        TipManager.AddTip(string.Format("很遗憾,{0}拒绝了你的入队申请",
            notify.refusePlayerName.WrapColor(ColorConstantV3.Color_Green_Str)));
    }

    private void RefuseTeamInvitation(RefuseTeamInvitationNotify notify)
    {
        TipManager.AddTip(string.Format("很遗憾,{0}拒绝了你的入队邀请", 
            notify.refusePlayerName.WrapColor(ColorConstantV3.Color_Green_Str)));
    }

    private void RefeshTeamFormationLv(FormationInfoDto dto)
    {
        var teamDto = _data.teamDto;
        teamDto.formation = dto;
        _data.teamDto = teamDto;
        stream.OnNext(_data);
    }
   
    private void HandleTeamStatusNotify(TeamStatusNotify notify)
    {
        if (notify ==  null)
            return;
        
        if (!HasTeam() 
            || _data.teamDto.id != notify.teamId 
            || notify.memberStatusList.Find<TeamMemberStatusDto>(s=>s.playerId == ModelManager.IPlayer.GetPlayerId()) == null)
        {
            GameDebuger.LogError("team info is not correct");
            return;
        }
        ShowTips(notify);
        var temp = new List<TeamMemberDto> (_data.GetMemberCount());

        _data.teamDto.members.ForEach(s=>{
            var member = notify.memberStatusList.Find<TeamMemberStatusDto>(m=>m.playerId == s.id);
            if (member == null){
                temp.Add(s);
            }
            else{
                _data.UpdateMemberByTeamMemberStatusDto(member);    
            }
        });

        temp.ForEach<TeamMemberDto>(s=>_data.RemoveMember(s));
        
        stream.OnNext(_data);
        backGuild.OnNext(new Unit());   //特别用于队伍状态的回公会场景
    }

    private void ShowTips(TeamStatusNotify notify)
    {
        if (notify.leaderId != _data.LeaderID)
        {
            if (notify.leaderId == ModelManager.Player.GetPlayerId())
                TipManager.AddTip("你被任命为队长");
            else
            {
                var leader = notify.memberStatusList.Find(d => d.playerId == notify.leaderId);
                TipManager.AddTip(string.Format("{0}被任命为队长", leader.memberName.WrapColor(ColorConstantV3.Color_Green_Str)));
            }
            return;
        }

        notify.memberStatusList.ForEachI((dto, idx) =>
        {
            var d = _data.TeamMembers.Find(member => member.id == dto.playerId);
            if (d.memberStatus != dto.status)
            {
                var isSelf = d.id == ModelManager.Player.GetPlayer().id;
                switch ((TeamMemberDto.TeamMemberStatus)dto.status)
                {
                    case TeamMemberDto.TeamMemberStatus.Away:
                        TipManager.AddTip(isSelf ? "你已暂离队伍" : string.Format("{0}已暂离队伍", dto.memberName.WrapColor(ColorConstantV3.Color_Green_Str)));
                        break;
                    case TeamMemberDto.TeamMemberStatus.Offline:
                        TipManager.AddTip(isSelf ? "你已离线" : string.Format("{0}已离线", dto.memberName.WrapColor(ColorConstantV3.Color_Green_Str)));
                        break;
                    case TeamMemberDto.TeamMemberStatus.Member:
                        TipManager.AddTip(isSelf ? "你已回归队伍" : string.Format("{0}已回归队伍", dto.memberName.WrapColor(ColorConstantV3.Color_Green_Str)));
                        break;
                }
            }
            else if (dto.status == (int) TeamMemberDto.TeamMemberStatus.Away)
            {
                var isSelf = d.id == ModelManager.Player.GetPlayer().id;
                switch ((TeamMemberStatusDto.AwayReason)dto.awayReason)
                {
                    case TeamMemberStatusDto.AwayReason.REASON_SCENEMAP:
                        if (isSelf)
                            TipManager.AddTip("队长所在场景不能自动归队");
                        break;
                }
            }
        });
    }

    private void HandleJoinTeamNotify(JoinTeamNotify notify){
        if (notify == null)
            return;

        if (notify.member.id == ModelManager.Player.GetPlayerId())
            TipManager.AddTip("你已经加入队伍");
        else
            TipManager.AddTip(string.Format("{0}已经加入队伍",
                notify.member.nickname.WrapColor(ColorConstantV3.Color_Green_Str)));
        _data.AddMember(notify.member);
        stream.OnNext(_data);
    }

    //申请入队
    private void HandleTeamRequestNotify(TeamRequestNotify notify)
    {
        if (notify ==  null)
            return;
        //	//相同的joinPlayerId申请入队玩家信息仅保留最近的一条
        var pid = notify.playerId;
		if(_data._joinTeamRequestNotifyList.ContainsKey(pid))
		    _data._joinTeamRequestNotifyList [pid] = global::Tuple.Create<TeamRequestNotify, bool>(notify, true);
		else if (_data._joinTeamRequestNotifyList.Count < TeamDefaultSaveApplyCnt)
        {
            _data._joinTeamRequestNotifyList.Add(pid, global::Tuple.Create<TeamRequestNotify, bool>(notify, true));
            JSTimer.Instance.SetupCoolDown(string.Format("JoinTeamRequestNotify_{0}", pid), 300f,   //5分钟有效
                         null, delegate
                         {
                             _data._joinTeamRequestNotifyList[pid] = global::Tuple.Create<TeamRequestNotify, bool>(notify, false);
                             stream.OnNext(_data);
                         });
        }
        var controller = ProxyBaseWinModule.Open();
        controller.InitView(notify);
        stream.OnNext(_data);
    }

    //附近玩家
    private void HandlerNearbyPlayerNotify(TeamNearbyDto dto)
    {
        _data.RecommendTeamList = dto;
        stream.OnNext(_data);
    }
    
    //组队平台
    private void HandlerTeamPanelNotify(TeamsDto data)
    {
        _data.CreateTeamsData = data;
        stream.OnNext(_data);
    }

    private void HandlerTeamCommandNotify(TeamCommanderNotify data)
    {
        _data.teamDto.commanderId = data.commanderMemberId;
        stream.OnNext(_data);
    }

    private void HandlerCallMemberNotify(CallMemberNotify notify)
    {
        var controller = ProxyBaseWinModule.Open();
        controller.InitView(notify);
    }

    private void HandlerMatchNotify(MatchBtnNotify notify)
    {
        _data.AutoBtnState = notify.match;
        stream.OnNext(_data);
    }

    private void CleanUpDefauleShow()
    {
        _data._joinTeamRequestNotifyList.Values.ForEachI((tuple, index) =>
        {
            if (index <= TeamDefaultShowApplyCnt) //清空前20个
            {
                JSTimer.Instance.CancelCd(string.Format("JoinTeamRequestNotify_{0}", tuple.p1.playerId));
            }
        });

        for (int i = 0; i < TeamDefaultShowApplyCnt; i++)
        {
            if (_data._joinTeamRequestNotifyList.Count != 0)
            {
                var noti = _data._joinTeamRequestNotifyList.Values[0].p1;
                _data._joinTeamRequestNotifyList.Remove(noti.playerId);
                //_data._joinTeamRequestNotifyList.RemoveAt(0);
            }
        }
        stream.OnNext(_data);
    }

    private void ClearInviteList()
    {
        for (int i = 0; i < TeamDefaultShowInviteCnt; i++)
        {
            _data.ClearTeamInviteNotifyList();
        }
    }

    #region 组队邀请
    //相同的teamUID邀请入队信息仅保留一条
    private void HandleTeamInvitationNotify(TeamInvitationNotify notify)
    {
        if (notify ==  null)
            return;

        _data.curInvitationNoti = notify;
        _data.UpdateInviteList(notify);
        stream.OnNext(_data);

        var controller = ProxyBaseWinModule.Open();
        controller.InitView(notify);
    }

    #endregion
    private void HandleLeaveTeamNotify(LeaveTeamNotify notify){
        if (notify == null)
            return;

        if (notify.playerId == ModelManager.IPlayer.GetPlayerId())
            //主角离队，清理数据
            _data.ClearTeamInfo();
        else
            _data.RemoveMember(notify.teamId, notify.playerId);

        var teamDto = GetSelfTeamDto();
        var isShowTip = teamDto == null ? false : teamDto.id == notify.teamId;
        switch ((LeaveTeamNotify.LeaveTeamReason)notify.reason)
        {
            case LeaveTeamNotify.LeaveTeamReason.REASON_LEAVE:
                if (notify.playerId != ModelManager.Player.GetPlayerId())
                {
                    if (isShowTip)
                        TipManager.AddTip(string.Format("{0}已退出队伍", notify.playerName.WrapColor(ColorConstantV3.Color_Green_Str)));
                }
                else
                    TipManager.AddTip("你已退出队伍");
                break;
            case LeaveTeamNotify.LeaveTeamReason.REASON_KICKOUT:
                if (notify.playerId != ModelManager.Player.GetPlayerId())
                {
                    SendBackGuildScene(teamDto);
                    if (isShowTip)
                        TipManager.AddTip(string.Format("{0}已退出队伍", notify.playerName.WrapColor(ColorConstantV3.Color_Green_Str)));
                }
                else
                    TipManager.AddTip("你已被队长请离队伍");
                break;
            case LeaveTeamNotify.LeaveTeamReason.REASON_DISMISS:
                if(isShowTip)
                    TipManager.AddTip("队伍已解散");
                break;
        }
        stream.OnNext(_data);
    }

    private void SendBackGuildScene(TeamDto dto)
    {
        int count = 0;
        var playerGuildInfo = GuildMainDataMgr.DataMgr.PlayerGuildInfo;
        if (playerGuildInfo != null)
        {
            dto.members.ForEach(e =>
            {
                var scenePlayer = WorldManager.Instance.GetModel().GetPlayerDto(e.id);
                if (scenePlayer != null)
                {
                    var guildInfo = scenePlayer.guildInfoDto;
                    if (guildInfo != null && guildInfo.guildId == playerGuildInfo.guildId)
                        count++;
                }
            });
        }
        if (count == 0)
            backGuild.OnNext(new Unit());
    }

    #region Handle Resp

    private void UpdateApproveJoinTeamResp(long joinPlayerId)
    {
        if (JSTimer.Instance.IsCdExist(string.Format("JoinTeamRequestNotify_{0}", joinPlayerId)))
            JSTimer.Instance.CancelCd(string.Format("JoinTeamRequestNotify_{0}",joinPlayerId));
        _data._joinTeamRequestNotifyList.Remove (joinPlayerId);
        //FireData();
        stream.OnNext(_data);
    }

    private void UpdateByLeaveTeamResp()
    {
        _data.ClearTeamInfo();
        stream.OnNext(_data);
    }

    #endregion
    public bool HasTeam(){
        return _data != null && _data.teamDto != null && _data.teamDto.id > 0;
    }

    //是否可以点击小地图切换场景
    public bool IsCanFlyScene()
    {
        if (!HasTeam())
            return true;

        var id = ModelManager.Player.GetPlayerId();
        var memberStatus = TeamState(id);

        return memberStatus != (int)TeamMemberDto.TeamMemberStatus.Member;
    }


    //返回玩家在队伍中的状态
    public int TeamState(long id)
    {
        if (!HasTeam())
            return -1;

        var player = _data.teamDto.members.Find(d => d.id == id);
        return player.memberStatus;
    }

    public bool IsLeader()
    {
        return _data.IsLeader();
    }
    
    public long GetCommanderId()
    {
        return _data != null && _data.teamDto != null ? _data.teamDto.commanderId : 0L;
    }

    public ModelStyleInfo GetMemberByFormationPos(int posKey)
    {
        throw new NotImplementedException();
    }

    public TeamDto GetTeamDtoByIdx(int idx)
    {
        return _data.GetNearByDto.teams.TryGetValue(idx);
    }

    public TeamDto GetSelfTeamDto()
    {
        return _data.GetTeamDto;
    }

    public List<TeamDto> GetTeamDto()
    {
        return _data.GetNearByDto.teams;
    }

    public bool HasAwayTeamMember()
    {
        return _data.teamDto != null && _data.teamDto.members.Find(d => d.memberStatus == (int)TeamMemberDto.TeamMemberStatus.Away
                                                                        || d.memberStatus == (int)TeamMemberDto.TeamMemberStatus.Offline) != null;
    }

    public IEnumerable<TeamMemberDto> GetAllAwayTeamMember()
    {
        if (_data.teamDto == null) return null;
        return _data.teamDto.members.Filter(d => d.memberStatus == (int)TeamMemberDto.TeamMemberStatus.Away
                                                                        || d.memberStatus == (int)TeamMemberDto.TeamMemberStatus.Offline);
    }
    public IEnumerable<TeamPlayerDto> GetTwoNearByPlayerDto(int idx)
    {
        return _data.GetTwoNearByPlayerDto(idx);
    }

    public IEnumerable<GuildMemberDto> GetGuildMemberDtos()
    {
        return _data.GetGuildMembersDto.memberList == null ? null : _data.GetGuildMembersDto.memberList;
    } 

    public List<TeamPlayerDto> GetPlayerDtoList()
    {
        return _data.RecommendFriendList;
    }

    public TeamPlayerDto GetNearByPlayerDto(int idx)
    {
        return _data.GetNearByPlayerDto(idx);
    }
    
    public IEnumerable<TeamPlayerDto> GetTwoFriendDto(int idx)
    {
        return _data.GetTwoFriendDto(idx);
    }

    public TeamPlayerDto GetFriendDto(int idx)
    {
        return _data.GetFriendDto(idx);
    }

    public List<TeamPlayerDto> GetNearbyPlayerDtoList()
    {
        return _data.GetNearByDto.players;
    }

    public List<TeamDto> GetNearbyTeamList()
    {
        return _data.GetNearByDto.teams;
    }

    public ITeamMatchTargetData GetCurTeamMatchTargetData
    {
        get{return _data.GetMatchTargetData;}
        set { _data.SetMatchTargetData(value); }
    }

    public TeamMemberDto.TeamMemberStatus GetTeamberStatueById(long id)
    {
        return _data.GetTeamberStatueById(id);
    }

    public TeamsDto GetCreateTeamPanelData()
    {
        return _data.CreateTeamsData;
    }

    private Subject<Unit> backGuild = new Subject<Unit>();
    public UniRx.IObservable<Unit> BackGuild { get { return backGuild; } }
}