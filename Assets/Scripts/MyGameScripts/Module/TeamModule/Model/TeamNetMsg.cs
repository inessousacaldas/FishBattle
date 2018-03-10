using System;
using System.Collections.Generic;
using AppServices;
using AppDto;

/** 队伍操作状态 */
// todo fish:不确定新的数据结构
public class TeamActionStatusDto : GeneralResponse
{

    /** 错误代码(0:正常) */
    public int errorCode;
}
public sealed partial class TeamDataMgr
{

    private enum TeamAction{
        Away,
        Back,
        Leave,
        KickOutMember,
        AssignLeader
    }

    private class TeamOpAction{
        public	TeamAction action;
        public TeamMemberDto target;
        public TeamOpAction(TeamAction action,TeamMemberDto target){
            this.action = action;
            this.target = target;
        }
    }
//    创建队伍 finish
//        加入队伍 finish
//    退出队伍 finish
//        移交队长
//    暂时离队
//        回归队伍
//    召回队员
//        邀请入队
//    申请入队
//        请离玩家
//    调整位置
//        委任指挥
//    取消指挥

//    pivate Dictionary<long,TeamInviteNotify> _teamInviteNotifyDic = new Dictionary<long,TeamInviteNotify> ();
//	private Dictionary<long,JoinTeamRequestNotify> _joinTeamRequestNotifyDic = new Dictionary<long,JoinTeamRequestNotify> ();
    private static TeamOpAction _cachedAction;

    public static class TeamNetMsg
    {
        // todo fish :退出游戏／重新登陆的时候清数据
        private static Dictionary<long, TeamMemberDto> _kickOutMemberDic = new Dictionary<long, TeamMemberDto> ();

        public static void CreateTeam(ITeamMatchTargetData target, Action success = null)
        {
            //四轮之塔不能组队
            if (TowerDataMgr.DataMgr.IsInTower())
                return;
            if (DataMgr.HasTeam())
            {
                TipManager.AddTip("已经创建队伍了，无法重复创建");
                return;
            }

            ServiceRequestAction.requestServer(
                //Services.Team_CreateTeam(target.GetActiveId, 20, 50, "")
                                Services.Team_CreateTeam(target.GetActiveId, target.GetMinLv, target.GetMaxLv,"")
                , "CreateTeam"
                , (e) =>
                {
                    TipManager.AddTip("创建队伍成功");
                    GameUtil.SafeRun(success);
                });
        }

        //改变组队目标
        public static void ChangeTarget(int targetIDs, int minGrades, int maxGrades, bool auto = false)
        {
            ServiceRequestAction.requestServer(Services.Team_ChangeTarget(targetIDs,minGrades,maxGrades),"ChangeTarget",
                (e) =>
                {
                    ChangeTargetData(targetIDs, minGrades, maxGrades, auto);
                });
        }

        public static void ChangeSelfTarget(int targetIDs, int minGrades, int maxGrades, bool auto = false)
        {
            ChangeTargetData(targetIDs, minGrades, maxGrades, auto);
        }

        private static void ChangeTargetData(int targetIDs, int minGrades, int maxGrades, bool auto = false)
        {
            TeamMatchTargetData target = TeamMatchTargetData.Create(targetIDs, maxGrades, minGrades, auto);
            DataMgr._data.SetMatchTargetData(target);
            FireData();
            TipManager.AddTip("修改目标成功");

            //勾选了自动匹配的时候,请求一次自动匹配的接口
            if (auto)
                AutoMatchTeam(DataMgr._data.GetMatchTargetData, true);
        }

        public static void JoinTeam(long teamPlayerId)
        {
            if (DataMgr.HasTeam())
            {
                TipManager.AddTip("已有队伍，无法申请加入");
                return;
            }
            bool checkCopy = true;
            bool notCooldown = false;
            string taskName = string.Format("JoinTeamTimer_{0}", teamPlayerId);

            ServiceRequestAction.requestServer(Services.Team_JoinTeam(teamPlayerId, checkCopy.ToString()), "JoinTeam",
                (e) =>
                {
                    if (checkCopy)
                    {
                        TipManager.AddTip("已发起入队申请，请耐心等待回复");
                    }
                    JSTimer.Instance.SetupCoolDown(taskName, 10f, null, null);
                });
        }

        public static void ApproveJoinTeam(long pendingJoinPlayerId)
        {
            bool isOverTime = DataMgr._data.CheckApplicationOverTime(pendingJoinPlayerId);
            if (isOverTime)
            {
                ServiceRequestAction.requestServer(
                    Services.Team_ApproveJoin(pendingJoinPlayerId, "")
                , onSuccess:delegate(GeneralResponse response) {
                    DataMgr.UpdateApproveJoinTeamResp(pendingJoinPlayerId);
                }
                , onRequestError: delegate(ErrorResponse errorResponse)
                    {
                        GameDebuger.LogError(errorResponse);
                    });
            }
            else
            {
                TipManager.AddTip("申请已超时");
            }
        }

        public static void SpeakOut(string channelId, string message, Action callback)
        {
            ServiceRequestAction.requestServer(Services.Team_Shout(channelId, message), "TeamShout", (e) =>
            {
                TipManager.AddTip("发送成功");
                callback();
            });
        }

        //邀请
        public static void InviteMember(long playerId, string nickName)
        {
            //四轮之塔不能组队
            if (TowerDataMgr.DataMgr.IsInTower())
                return;
            DoInviteMember(playerId, nickName);
        }

        private static void DoInviteMember(long playerId, string nickName)
        {
            var teamDto = DataMgr.GetSelfTeamDto();
            string taskName = string.Format("InviteMemberTimer_{0}", playerId);
            if (DataMgr._data.GetMemberCount() >= 5)
            {
                TipManager.AddTip("队伍人数已满，无法发送邀请");
                return;
            }

            Action InvitePlayerAction = () =>
            {
                if (!JSTimer.Instance.IsCdExist(taskName))
                {
                    //目前无组队,需要构建一个组队目标去邀请对方
                    if (teamDto == null)
                    {
                        teamDto = new TeamDto();
                        teamDto.actionTargetId = 0;
                        teamDto.minGrade = 1;
                        teamDto.maxGrade = 100;
                    }
                    ServiceRequestAction.requestServer(Services.Team_InvitePlayer(playerId, teamDto.actionTargetId,
                    teamDto.minGrade, teamDto.maxGrade, ""), "InviteMember", (e) =>
                    {
                        //                    DataMgr._data._teamInviteNotifyList = e 
                        TipManager.AddTip(string.Format("已邀请[2DC6F8]{0}[-]加入队伍，请耐心等待回复", nickName));
                        JSTimer.Instance.SetupCoolDown(taskName, 10f, null, null);
                        FireData();
                    });
                }
                else
                {
                    TipManager.AddTip("请耐心等待回复");
                }
            };
            bool isCopyExtr = false;
            List<Mission> tMission = MissionDataMgr.DataMgr.GetExistSubMissionMenuList();
            for(int i = 0;i < tMission.Count;i++) {
                if(tMission[i].type == (int)MissionType.MissionTypeEnum.CopyExtra) {
                    isCopyExtr = true;
                    break;
                }
            }
            if(isCopyExtr)
            {
                var ctrl = ProxyBaseWinModule.Open();
                BaseTipData data = BaseTipData.Create("副本任务","加入新队员将放弃当前的彩蛋任务，是否继续邀请队员？", 0, delegate
                {
                    InvitePlayerAction();
                }, null);
                ctrl.InitView(data);
            }
            else {
                InvitePlayerAction();
            }
        }

        public static void ApproveInviteMember(
            TeamInvitationNotify notify
            , Action onSuccess = null)
        {
            ServiceRequestAction.requestServer(
                Services.Team_ApproveInvitation(notify.teamId, notify.inviterPlayerId, notify.teamTargetId, notify.minGrade, notify.maxGrade,"")
            , ""
            , delegate{
                    DataMgr._data.RemoveInvitation(notify.inviterPlayerId);
                    FireData();
                    GameUtil.SafeRun(onSuccess);
              });
        }

        private static bool ValidateBattleState(TeamAction status, TeamMemberDto target)
        {
            if (BattleDataManager.DataMgr.IsInBattle)
            {
//			_cachedAction = new TeamOpAction(action,target); //todo fish
                TipManager.AddTip("此操作将在战斗结束后自动执行");
                return true;
            }
            return false;
        }

        public static void LeaveTeam()
        {
            if (!DataMgr.HasTeam())
            {
                TipManager.AddTip("当前没有队伍信息，无法离队");
                return;
            }

            if (ValidateBattleState(TeamAction.Leave, null))
                return;

            LeaveTeamRequest();
        }

        private static void LeaveTeamRequest()
        {
            ServiceRequestAction.requestServer(Services.Team_LeaveTeam(), "LeaveTeam", (e) =>
            {
                _cachedAction = null;
                DataMgr.UpdateByLeaveTeamResp();
            });
        }

        private static bool ValidateTeamActionStatusDto(TeamActionStatusDto statusDto)
        {
            if (statusDto != null)
            {
                if (statusDto.errorCode == 0)
                {
                    //				TipManager.AddTip ("正常");
                    return true;
                }
                else if (statusDto.errorCode == 4003)
                    TipManager.AddTip("已离队");
                else if (statusDto.errorCode == 4000)
                    TipManager.AddTip("队伍不存在");
                else if (statusDto.errorCode == 4004)
                    TipManager.AddTip("已归队");
                else if (statusDto.errorCode == 4005)
                    TipManager.AddTip("不在同一队伍里");
            }
            else
                GameDebuger.LogError("TeamActionStatusDto is null");

            return false;
        }


    public static void AwayTeam()
    {
        if (ValidateBattleState(TeamAction.Away, null))
            return;

        ServiceRequestAction.requestServer (Services.Team_AwayTeam(), "AwayTeam", (e) => 
        {
            ValidateTeamActionStatusDto (e as TeamActionStatusDto);
		});
    }

    public static void BackTeam ()
	{
		ServiceRequestAction.requestServer (Services.Team_BackTeam(), "BackTeam", (e) => 
        {
            if (BattleDataManager.DataMgr.IsInBattle)
                TipManager.AddTip("此操作将在战斗结束后自动执行");
            else
            {
                ValidateTeamActionStatusDto(e as TeamActionStatusDto);
                SpecialActivityDispose();
            }
        });
	}

	//活动时归队的特殊处理
	private static void SpecialActivityDispose()
	{
		if (!DataMgr.HasTeam()) return;
	}

        public static void AssignLeader(TeamMemberDto memberDto)
        {
            if (ValidateBattleState(TeamAction.AssignLeader, memberDto)) return;
            if (memberDto == null) return;

            if (memberDto.memberStatus == (int)TeamMemberDto.TeamMemberStatus.Away)
                TipManager.AddTip("暂离状态无法提升为队长");
            else if (memberDto.memberStatus == (int)TeamMemberDto.TeamMemberStatus.Offline)
                TipManager.AddTip("离线状态无法提升为队长");
            else
                ServiceRequestAction.requestServer(Services.Team_AssignLeader(memberDto.id));
        }

        public static void KickOutMember(TeamMemberDto memberDto)
        {
		    if (ValidateBattleState (TeamAction.KickOutMember, memberDto))
            {
			    if (!_kickOutMemberDic.ContainsKey(memberDto.id))
                {
				    _kickOutMemberDic.Add(memberDto.id, memberDto);
			    }
			    return;
		    }
            ServiceRequestAction.requestServer(Services.Team_Kickout(memberDto.id));
        }

        public static void KickOutMember()
        {
            foreach (TeamMemberDto tTeamMemberDto in _kickOutMemberDic.Values)
            {
                ServiceRequestAction.requestServer(Services.Team_Kickout(tTeamMemberDto.id));
            }

            _kickOutMemberDic.Clear();
        }

        //一键召回,参数必须为0才能一键召回所有队员
        public static void SummonAwayTeamMembers()
        {
            TipManager.AddTip("正在召回队员");
            ServiceRequestAction.requestServer(Services.Team_Recall(0));
        }

        public static void SummonAwayTeamMember(long playerId)
        {
            TipManager.AddTip("正在召回队员");
            ServiceRequestAction.requestServer(Services.Team_CallMember(playerId));
        }

        public static void SetCommander(long playerId){
            if (!DataMgr.HasTeam()) return;
            var teamDto = DataMgr._data.teamDto;

            if (playerId != teamDto.commanderId)
                ServiceRequestAction.requestServer(Services.Team_SetCommander(playerId));
            else
                ServiceRequestAction.requestServer(Services.Team_SetCommander(teamDto.leaderPlayerId));
        }
        
        //组队平台
        public static void TeamPanel()
        {
            var formationId = DataMgr.GetCurTeamMatchTargetData == null
                ? 0 : DataMgr.GetCurTeamMatchTargetData.GetActiveId;

            ServiceRequestAction.requestServer(Services.Team_Panel(formationId), "TeamPanel");
        }
        
        //自动匹配
        public static void AutoMatchTeam(ITeamMatchTargetData _data, bool isAuto = false)
        {
            if(TowerDataMgr.DataMgr.IsInTower())
                return;
            if (isAuto && _data.GetActiveId == 0)
            {
                TipManager.AddTip("请选择组队目标");
                return;
            }
            Action InvitePlayerAction = () =>
            {
                GameUtil.GeneralReq(
                Services.Team_AutoMatch(_data.GetActiveId, isAuto, _data.GetMinLv, _data.GetMaxLv), response =>
                {
                    if (isAuto)
                        TipManager.AddTip("开始匹配");
                    else
                        TipManager.AddTip("取消匹配");
                });
            };
            bool isCopyExtr = false;
            List<Mission> tMission = MissionDataMgr.DataMgr.GetExistSubMissionMenuList();
            for(int i = 0;i < tMission.Count;i++)
            {
                if(tMission[i].type == (int)MissionType.MissionTypeEnum.CopyExtra)
                {
                    isCopyExtr = true;
                    break;
                }
            }
            if(isCopyExtr)
            {
                var ctrl = ProxyBaseWinModule.Open();
                BaseTipData data = BaseTipData.Create("副本任务","加入新队员将放弃当前的彩蛋任务，是否继续邀请队员？", 0, delegate
                {
                    InvitePlayerAction();
                }, null);
                ctrl.InitView(data);
            }
            else
            {
                InvitePlayerAction();
            }
        }
        
        // 我的好友
        public static void RecommendPlayer()
        {
            ServiceRequestAction.requestServer(Services.Team_Friends(),"RecommendPlayer", response =>
            {
                var playerList = response as DataList;
                List<TeamPlayerDto> list = new List<TeamPlayerDto>();
                playerList.items.ForEach(dto =>
                {
                    if(dto is TeamPlayerDto)
                        list.Add(dto as TeamPlayerDto);
                });
                DataMgr._data.RecommendFriendList = list;
                FireData();
            });
        }

        //公会成员
        //GuildMembersDto
        public static void GetGuildMember()
        {
            ServiceRequestAction.requestServer(Services.Team_GuildMember(), "GetGuildMember", response =>
            {
                var dto = response as GuildMembersDto;
                if (dto == null)
                {
                    GameDebuger.LogError("Team_GuildMember返回的数据有问题");
                    return;
                }

                DataMgr._data.GetGuildMembersDto = dto;
                FireData();
            });
        }
        
        //推荐队伍
        public static void RecommendTeam()
        {
            ServiceRequestAction.requestServer(Services.Team_Nearby(), "RecommendTeam");
        }
        
        //一键邀请
        public static void OnkeyInvitation(string nameList, string strMeta)
        {
            var target = DataMgr.GetCurTeamMatchTargetData;
            ServiceRequestAction.requestServer(Services.Team_BatchInvite(nameList, target.GetActiveId, 
                target.GetMinLv, target.GetMaxLv, strMeta), "OnkeyInvitation", (e) =>
            {
                TipManager.AddTip("一键邀请成功");
//                DataMgr._data.RecommendTeamList = e as List<TeamPlayerDto>;
                FireData();
            });
        }
        
        //一键申请
        public static void OnKeyApplication(string nameList, string strMeta)
        {
            ServiceRequestAction.requestServer(Services.Team_BatchJoin(nameList, strMeta), "OnKeyApplication", (e) =>
            {
                TipManager.AddTip("一键申请成功");
//                DataMgr._data.RecommendTeamList = e as List<TeamPlayerDto>;
                FireData();
            });
        }

        //刷新申请
        public static void RefreshApply()
        {
            GameUtil.GeneralReq(Services.Team_joinTeamList(), RespApplyInfo);
        }

        public static void RefuseTeamRequest(long id)
        {
            ServiceRequestAction.requestServer(Services.Team_RefuseTeamRequest(id), "RefuseTeamRequest");
        }

        public static void RefuseTeamInvitation(long id)
        {
            ServiceRequestAction.requestServer(Services.Team_RefuseTeamInvitation(id), "RefuseTeamInvitation");
        }

        public static void RespApplyInfo(GeneralResponse resp)
        {
            var list = resp as DataList;

            list.items.ForEach(data =>
            {
                bool had = false;              
                var dto = data as JoinTeamDto;
                var key = dto.id;
                if (dto == null) return;

                DataMgr._data._joinTeamRequestNotifyList.ForEachI((tuple,n) =>
                {
                    if (DataMgr._data._joinTeamRequestNotifyList.ContainsKey(key))
                        had = true;
                    if (had)  //是否时间过期
                        DataMgr._data._joinTeamRequestNotifyList.RemoveAt(n);
                });
                if (!had)
                {
                    TeamRequestNotify notify = new TeamRequestNotify();
                    notify.playerId = dto.id;
                    //notify.timeout = dto.timeout;
                    notify.playerNickname = dto.nickname;
                    notify.playerFactionId = dto.factionId;
                    notify.playerGrade = dto.grade;
                    notify.playerCharactorId = dto.charactorId;
                    notify.playerFaction = dto.faction;
                    //notify.playerCharactor = dto.charactor;
                    if (DataMgr._data._joinTeamRequestNotifyList.Count < TeamDataMgr.TeamDefaultSaveApplyCnt)
                    {
                        DataMgr._data._joinTeamRequestNotifyList.Add(key,
                        global::Tuple.Create<TeamRequestNotify, bool>(notify, true));
                        //设置入队时间
                        JSTimer.Instance.SetupCoolDown(string.Format("JoinTeamRequestNotify_{0}", key), 7200f, null,
                            delegate
                            {
                                DataMgr._data._joinTeamRequestNotifyList[key] =
                                    global::Tuple.Create<TeamRequestNotify, bool>(notify, false);
                                stream.OnNext(DataMgr._data);
                            });
                    }
                }
            });
        }

        //刷新邀请
        public static void RefreshInvite()
        {
            GameUtil.GeneralReq(Services.Team_InviteList(), RespInviteInfo);
        }

        private static void RespInviteInfo(GeneralResponse resp)
        {
            var list = resp as DataList;
            
            list.items.ForEach(data =>
            {
                bool had = false;
                var dto = data as TeamInvitationDto;
                if (dto == null) return;

                DataMgr._data.GetInvitationList().ForEach(n =>
                {
                    if (dto.inviterPlayerId == n.inviterPlayerId)
                        had = true;

                    if (had)  //是否过期
                        DataMgr._data.RemoveInvitation(n.inviterPlayerId);
                });
                if (!had)
                {
                    TeamInvitationNotify notify = new TeamInvitationNotify();
                    notify.targetPlayerId = dto.targetPlayerId;
                    notify.inviterPlayerId = dto.inviterPlayerId;
                    notify.inviteTeamMembers = dto.inviteTeamMembers;
                    notify.timeout = dto.timeout;
                    notify.minGrade = dto.minGrade;
                    notify.maxGrade = dto.maxGrade;
                    notify.teamTargetId = dto.teamTargetId;
                    DataMgr._data.UpdateInviteList(notify);
                }
            });
            FireData();
        }

        //获取阵法信息
        //FormationInfoDto
        public static void GetFormationInfo(Action callback)
        {
            GameUtil.GeneralReq(Services.Formation_FormationInfo(), response =>
            {
                ActiveCaseInfoDto dto = response as ActiveCaseInfoDto;
                if (dto == null)
                {
                    GameDebuger.LogError("=======数据返回错误=======");
                    return;
                }
                DataMgr._data.CurFormationInfo = dto;
                TeamFormationDataMgr.DataMgr.SetUseCaseId(dto.activeId);
                TeamFormationDataMgr.DataMgr.SetFormationId(dto.formationId);
                if (callback != null)
                    callback();
            });
        } 

        //交换伙伴位置
        //ActiveCaseInfoDto
        public static void TeamMenuPosition(string order)
        {
            GameUtil.GeneralReq(Services.Formation_TeamMenuPosition(order), response =>
            {
                ActiveCaseInfoDto dto = response as ActiveCaseInfoDto;
                if (dto == null)
                    return;

                DataMgr._data.CurFormationInfo = dto;
                FireData();
            });
        }

        //用于刷新按钮
        public static void TeamMatchBtn()
        {
            GameUtil.GeneralReq(Services.Team_MatchBtn());
        }
    }
}

