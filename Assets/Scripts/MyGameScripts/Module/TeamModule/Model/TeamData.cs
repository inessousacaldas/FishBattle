using UnityEngine;
using AppDto;
using System.Collections.Generic;

public interface IRecommendViewData
{
    IEnumerable<TeamPlayerDto> GetFriendList { get; }
    TeamNearbyDto GetNearByDto { get; }
    IEnumerable<TeamPlayerDto> GetTwoFriendDto(int idx);
    TeamPlayerDto GetFriendDto(int idx);
    IEnumerable<TeamPlayerDto> GetTwoNearByPlayerDto(int idx);
    TeamPlayerDto GetNearByPlayerDto(int idx);
}

public interface ITeamBeInviteData
{
    TeamInvitationNotify GetCurrentInvitation();
    bool HasInvitationInfo();
}

public interface IExpandTeamData
{
    bool HasTeam();
    IEnumerable<TeamMemberDto> TeamMembers{ get;}
    int GetMemberCount();
    long LeaderID{ get;}
    TeamMemberDto GetMainRoleTeamMemberDto();
    bool IsLeader();
}

public interface ITeamApplyViewData
{
    IEnumerable<TeamRequestNotify> GetJoinTeamRequestList();
    TeamRequestNotify GetJoinTeamRequestByIndex(int idx);

    IEnumerable<TeamRequestNotify> GetJoinTeamTwoRequestByIndex(int idx);
    int GetMemberCount();
    int GetApplicationCnt();
}

public interface ITeamInviteViewData
{
    IEnumerable<TeamInvitationNotify> GetInvitationList();
    TeamInvitationNotify GetTeamInvitationNotifyByIndex(int idx);
    int GetInvitationCount();
}

public interface ITeamMainViewData{
//    string GetTipsStr();
//    string GetTargetStr();
    TeamMainViewTab CurTab{get;}

    TeamsDto GetCreatTeamsDto { get;}
    Dictionary<TeamMainViewTab, string> GetTabInfo();
    bool HasTeam();
    bool IsLeader();
    bool IsAutoMatch { get; }
    bool IsCommander(long playerID);
    
//    int GetArrayID();  //阵容id 类型暂不确定
//    IEnumerable<TeamMemberDto> GetTeamMembers();
//    IEnumerable<TeamMemberDto> GetPets();

    bool HasApplicationInfo();
    bool HasInvitationInfo();
    string GetUITips();

    ActiveCaseInfoDto CurFormationInfo { get; }

    void SetMatchTargetData(ITeamMatchTargetData data);

    ITeamMatchTargetData GetMatchTargetData { get; }
}

public enum TeamMainViewTab{
    Team,           //我的队伍
    CreateTeam,     //便捷组队
    RecommendTeam   //推荐队伍
}

public enum SpeakChannel
{
    unKnown,
    country,
    guide,
    team,
    system,
    hearsay,
    friend,
    current,
    horn
}

public enum RecommendViewTab
{
    MyFriend,       //我的好友
    GuildPlayer,    //帮派成员
    NearByPlayer,   //附近的玩家
    NearByTeam,     //附近队伍
}

public interface ITeamData
{
    TeamMemberDto GetTeamMemberDtoByPlayerID(long id);

    TeamDto GetTeamDto { get; }
    long TeamUniqueId{ get;}
    long LeaderID{ get;}

    int GetApplicationCnt();

    ITeamMainViewData TeamMainViewData{get;}
    ITeamInviteViewData TeamInviteViewData{get;}
    ITeamApplyViewData TeamApplyViewData{get;}
    IExpandTeamData ExpandTeamViewData{get;}
    ITeamBeInviteData TeamBeInviteData{get;}
    IRecommendViewData RecommendData { get; }
    bool AutoBtnState { get; }
    IEnumerable<TeamMemberDto> GetTeamMember { get; }
    GuildMembersDto GetGuildMembersDto { get; }
}

public sealed partial class TeamDataMgr{
    public sealed partial class TeamData
        :ITeamData
    , ITeamMainViewData
    , ITeamInviteViewData
    , ITeamApplyViewData
    , IExpandTeamData
    , ITeamBeInviteData
    , IRecommendViewData
    {
        #region Server Data
        public TeamDto teamDto;
        public TeamDto GetTeamDto { get { return teamDto;} }
        // 用一个bool标记是否已经超时  －－fish
        public SortedList<long, Tuple<TeamRequestNotify, bool>> _joinTeamRequestNotifyList = new SortedList<long, Tuple<TeamRequestNotify, bool>>();
        private List<TeamInvitationNotify> _teamInviteNotifyList = new List<TeamInvitationNotify>();


        public int _teamPlayerNum = 1;  //队伍人数    

        public List<TeamPlayerDto> RecommendFriendList = new List<TeamPlayerDto>();        //好友
        public TeamNearbyDto RecommendTeamList;                //附近玩家以及队伍
        public TeamsDto CreateTeamsData;                           //组队平台
        private GuildMembersDto _guildMembersDto;       //公会在线成员

        public GuildMembersDto GetGuildMembersDto
        {
            get {return _guildMembersDto;}
            set { _guildMembersDto = value; }
        }

        private ActiveCaseInfoDto _formationInfoDto;             //队伍阵法信息
        #endregion

        #region ClientCache Data
        public TeamMainViewTab curTab = TeamMainViewTab.Team;
        public TeamInvitationNotify curInvitationNoti;
        private ITeamMatchTargetData curTeamMatchData;

        public int CdTimeThirty = 30;
        public int CdTimeTen = 10;
        public int RefreshCdTime = 10;        //组队平台剩余CD时间

        private bool _isAutoMatch = false;
        private bool _autoBtnState = false; //自动匹配按钮显隐状态
        #endregion

        #region Static Data
        public static List<UITips> tipsList = null;

        public Dictionary<TeamMainViewTab, string> tabInfoSet = null;

        public int _goalId = 0;             //组队目标
        public int _goalLevelMin = 0;       //任务下限等级
        public int _goalLevelMax = 0;       //任务上限等级
        public bool _beginMatch = false;    //是否开始匹配

        private void InitClientData(){            
            tabInfoSet = new Dictionary<TeamMainViewTab, string> (4){
                {TeamMainViewTab.Team, "我的队伍"}, 
                {TeamMainViewTab.CreateTeam, "组队平台"},
                {TeamMainViewTab.RecommendTeam, "推荐队友"}
            };

            curTab = TeamMainViewTab.Team;
        }

        #endregion


        public TeamData()
        {
            
        }

        public void InitData()
        {
            InitClientData();
            InitUITipsData();
            curTeamMatchData = TeamMatchTargetData.Create(0, 100, 1, true);
        }

        
        private void InitUITipsData()
        {
            if (tipsList == null)
            {
                tipsList = DataCache.getArrayByCls<UITips>().Filter(s=>s.clientModule == "team").ToList();
            }
        }

        public void Dispose()
        {
            _teamInviteNotifyList.Clear();
            _joinTeamRequestNotifyList.Clear();
        }

        public long LeaderID{get{ return teamDto == null ? -1 : teamDto.leaderPlayerId;}}
        public TeamMemberDto GetMainRoleTeamMemberDto()
        {
            return teamDto == null ? null : teamDto.members.Find(t=>t.id == ModelManager.IPlayer.GetPlayerId());
        }

        public long TeamUniqueId{ get{ 
                return teamDto != null ? teamDto.id : -1;
            }
        }

        public bool UpdateMemberByTeamMemberStatusDto(TeamMemberStatusDto sDto){
            if (teamDto == null)
                return false;
            var item = teamDto.members.Find<TeamMemberDto>(s=>s.id == sDto.playerId);
            if (item == null)
            {
                return false;
            }

            item.memberStatus = sDto.status;
            item.index = sDto.index;
            if (sDto.status == (int)TeamMemberDto.TeamMemberStatus.Leader)
            {
                teamDto.leaderPlayerId = sDto.playerId;
            }
            return false;
        }

        public void AddMember(TeamMemberDto member){
            if (teamDto == null || teamDto.members == null)
                return;

            teamDto.members.ReplaceOrAdd<TeamMemberDto>(s=>s.id == member.id, member);
        }

        public void RemoveMember(long teamID, long memberID){

            if (teamDto == null || teamDto.members == null || teamDto.id != teamID)
                return;

            teamDto.members.Remove<TeamMemberDto>(s=>s.id == memberID);
        }

        public void RemoveMember(TeamMemberDto member){
            if (teamDto == null || teamDto.members == null)
                return;

            teamDto.members.Remove<TeamMemberDto>(s=>s.id == member.id);
        }

        public TeamMemberDto GetTeamMemberDtoByPlayerID(long id){
            if (teamDto == null)
                return null;
            return teamDto.members.Find<TeamMemberDto>(s=>s.id == id);
        }

        public void ClearTeamInfo(){
            CleanUpApplicationInfo();
            ClearTeamInviteNotifyList();
            teamDto = null;
        }

        private void CleanUpApplicationInfo()
        {
            if (_joinTeamRequestNotifyList != null) _joinTeamRequestNotifyList.Clear();
        }

        public void ClearTeamInviteNotifyList()
        {
            if (_teamInviteNotifyList != null) _teamInviteNotifyList.Clear();
        }

        public List<TeamMemberDto> FindTembersByStatus(TeamMemberDto.TeamMemberStatus status)
        {
            if (teamDto == null)
                return null;

            List<TeamMemberDto> list = new List<TeamMemberDto>();
            teamDto.members.ForEach(data =>
            {
                if(data.memberStatus == (int)status)
                    list.Add(data);
            });
            return list;
        }
        public IEnumerable<TeamMemberDto> GetTembersByStatus(TeamMemberDto.TeamMemberStatus status)
        {
            if (teamDto == null)
                return null;
            return teamDto.members.Filter(m => m.memberStatus == (int) status);
        }

        public TeamMemberDto.TeamMemberStatus GetTeamberStatueById(long id)
        {
            var player = teamDto.members.Find(d => d.id == id);
            return player == null
                ? TeamMemberDto.TeamMemberStatus.NoTeam
                : (TeamMemberDto.TeamMemberStatus) player.memberStatus;
        }
        public int GetMemberCount()
        {
            return teamDto == null ? 0 : teamDto.members.Count;
        }

        public IEnumerable<TeamMemberDto> TeamMembers{ 
            get{
                return teamDto == null ? null : teamDto.members;
            }
        }
        public bool CheckApplicationOverTime(long playerID)
        {
            Tuple<TeamRequestNotify, bool> data = null;
            _joinTeamRequestNotifyList.TryGetValue(playerID, out data);
            return data == null ? false : data.p2;
        }

        public ActiveCaseInfoDto CurFormationInfo
        {
            get { return _formationInfoDto; }
            set { _formationInfoDto = value; }
        }

        public bool AutoBtnState
        {
            get { return _autoBtnState; }
            set { _autoBtnState = value; }
        }

        #region inplememt ITeamData
        public ITeamMainViewData TeamMainViewData{get{ return this;}}
        public ITeamInviteViewData TeamInviteViewData{get{ return this;}}
        public ITeamApplyViewData TeamApplyViewData{get{ return this;}}
        public IExpandTeamData ExpandTeamViewData { get{return this;} }
        public ITeamBeInviteData TeamBeInviteData { get{return this;} }
//        public ITeamFormationData TeamFormationData {get { return this;} }
        public IRecommendViewData RecommendData{get { return this; }}

        #endregion

        #region 申请入队 Application

        public TeamRequestNotify GetJoinTeamRequestByIndex(int idx)
        {
            return idx >= _joinTeamRequestNotifyList.Count
                ? null
                : _joinTeamRequestNotifyList.Values[idx].p1;
        }

        //获取两个连续的玩家数据
        public IEnumerable<TeamRequestNotify> GetJoinTeamTwoRequestByIndex(int idx)
        {
            List<TeamRequestNotify> list = new List<TeamRequestNotify>();
            if(idx < _joinTeamRequestNotifyList.Count)
                list.Add(_joinTeamRequestNotifyList.Values[idx].p1);                
            if(idx + 1 < _joinTeamRequestNotifyList.Count)
                list.Add(_joinTeamRequestNotifyList.Values[idx + 1].p1);
            return list;
        }

        public IEnumerable<TeamRequestNotify> GetJoinTeamRequestList(){
            int maxCount = Mathf.Min(_joinTeamRequestNotifyList.Count,TeamDataMgr.TeamDefaultShowApplyCnt);
            var result = new List<TeamRequestNotify>(maxCount);
            var i = 0;
            foreach(var kv in _joinTeamRequestNotifyList){
                if(i < maxCount){
                    result.Add(kv.Value.p1);
                    i++;
                }else
                    break;
            }
            return result;
        }

        public int GetApplicationCnt()
        {
            return _joinTeamRequestNotifyList == null ? 0 : _joinTeamRequestNotifyList.Count;
        }

        #endregion

        #region inplememt ITeamMainViewData
        public bool IsCommander(long playerID){
            return teamDto != null && teamDto.commanderId == playerID 
                && teamDto.leaderPlayerId != playerID;
        }

        public bool HasApplicationInfo()
        {
            return _joinTeamRequestNotifyList != null && _joinTeamRequestNotifyList.Count > 0;
        }

        public bool HasInvitationInfo()
        {
            return _teamInviteNotifyList != null && _teamInviteNotifyList.Count > 0;
        }

        public Dictionary<TeamMainViewTab, string> GetTabInfo(){
            return tabInfoSet;
        }

        public TeamMainViewTab CurTab{get{ return curTab;}}

        public bool HasTeam(){
            return teamDto != null;
        }

        public bool IsLeader(){
            return teamDto != null && teamDto.leaderPlayerId == ModelManager.IPlayer.GetPlayerId();
        }

        public bool IsAutoMatch
        {
            get { return _isAutoMatch; }
            set { _isAutoMatch = value; }
        }

        public string GetUITips()
        {
            UITips tip = null;
            if (tipsList.IsNullOrEmpty())
                return string.Empty;
            var random = new System.Random();
            var idx = random.Next(tipsList.Count);
            tipsList.TryGetValue(idx, out tip);
            return tip == null ? string.Empty : tip.tips;
        }

        #endregion

        #region 邀请入队 Invitation

        public void UpdateInviteList(TeamInvitationNotify notify)
        {
            _teamInviteNotifyList.ReplaceOrAdd(d => d.inviterPlayerId == notify.inviterPlayerId, notify);
        }

        public IEnumerable<TeamInvitationNotify> GetInvitationList()
        {
            return _teamInviteNotifyList;
        }

        public TeamInvitationNotify GetTeamInvitationNotifyByIndex(int idx)
        {
            TeamInvitationNotify notify = null;
            _teamInviteNotifyList.TryGetValue(idx, out notify);
            return notify;
        }

        public int GetInvitationCount()
        {
            return _teamInviteNotifyList.IsNullOrEmpty() ? 0 : _teamInviteNotifyList.Count;
        }

        public void RemoveInvitation (long leaderId)
        {
            _teamInviteNotifyList.Remove(s=>s.inviterPlayerId == leaderId);
        }


        #endregion

        #region 组队平台
        public TeamsDto GetCreatTeamsDto{get { return CreateTeamsData; }}
        #endregion
        
        #region 推荐队友 Recommend

        public IEnumerable<TeamPlayerDto> GetFriendList{ get { return RecommendFriendList; }}

        public TeamNearbyDto GetNearByDto { get { return RecommendTeamList; } }
        public IEnumerable<TeamPlayerDto> GetTwoFriendDto(int idx)
        {
            List<TeamPlayerDto> list = new List<TeamPlayerDto>();
            list.Add(RecommendFriendList.TryGetValue(idx));
            list.Add(RecommendFriendList.TryGetValue(idx + 1));
            return list;
        }

        public TeamPlayerDto GetFriendDto(int idx)
        {
            return RecommendFriendList.TryGetValue(idx);
        }

        /// <summary>
        /// 返回当前队员列表
        /// </summary>
        public IEnumerable<TeamMemberDto> GetTeamMember
        {
            get { return teamDto.members; }
        }


        public IEnumerable<TeamPlayerDto> GetTwoNearByPlayerDto(int idx)
        {
            List<TeamPlayerDto> list = new List<TeamPlayerDto>();
            list.Add(RecommendTeamList.players.TryGetValue(idx));
            list.Add(RecommendTeamList.players.TryGetValue(idx + 1));
            return list;
        }

        public TeamPlayerDto GetNearByPlayerDto(int idx)
        {
            return RecommendTeamList.players.TryGetValue(idx);
        }
        #endregion

        #region 组对目标
        public void SetMatchTargetData(ITeamMatchTargetData data)
        {
            curTeamMatchData = data;
        }

        public ITeamMatchTargetData GetMatchTargetData { get { return curTeamMatchData; } }
        #endregion
        public TeamInvitationNotify GetCurrentInvitation()
        {
            return curInvitationNoti;
        }
    }
}

