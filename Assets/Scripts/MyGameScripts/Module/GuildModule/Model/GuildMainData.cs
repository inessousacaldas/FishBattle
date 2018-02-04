// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : DM-PC092
// Created  : 1/10/2018 2:11:21 PM
// **********************************************************************
using AppDto;
using System.Collections.Generic;
public enum GuildState
{
    None,   //未知
    NotJoin,//未加入
    HasJoin //已加入
}

public enum GuildTab
{
    InfoView,       //信息
    ManageView,     //管理
    ActivityView,   //活动
    WelfareView,    //福利
    BuildView,      //建设
    None            //未知
}
public enum MemOrGuildEnum
{
    Guild,
    Mem,
    None
}

//是否搜索状态
public enum IsSearchGuildState
{
    No,
    Yes
}

public interface IGuildMainData
{
    GuildState GuildState { get; }  //是否加入公会
    int CreateNeed(bool isDiamond); //创建公会所需（钻石或金币）
    int CurServerGuildCount { get; }//当前服务器公会数量
    NotJoinViewState JoinOrCreateState { get; }//加入或是创建
    IEnumerable<GuildBaseInfoDto> GuildList { get; }//公会列表
    IsSearchGuildState SearchGuildState { get; }    //是否搜索公会状态
    bool NeedReposition { get; }    //需要重置panel位置
    #region 信息
    IEnumerable<GuildMemberDto> GuildMemberList { get; }    //公会成员列表
    IEnumerable<GuildPosition> GuildPosition { get; }   //公会职位列表
    GuildBaseInfoDto GuildBaseInfo { get; } //当前公会信息
    IEnumerable<GuildApprovalDto> GuildApprovalList { get; }    //公会申请列表
    PlayerGuildInfoDto PlayerGuildInfo { get; }
    SceneMap GuildScene { get; }        //公会场景
    #endregion
    #region 管理
    GuildDetailInfoDto GuildDetailInfo { get; }     //公会具体信息
    #endregion
    #region 建筑
    IEnumerable<string> BuildList { get; }      //公会建筑（种类）
    IEnumerable<GuildBuilding> GuildBuildList { get; }  //公会建筑配表数据
    IEnumerable<long> BuildUpTimeList { get; }  //公会建筑时间
    #endregion
}

public sealed partial class GuildMainDataMgr
{
    public const int ReqGuildListCount = 20;                               //每次请求公会列表数量
    public const int ReqRequestListCount = 20;                             //每次请求申请列表数量
    public const int ReqMemListCount = 20;                                 //每次请求成员列表数量
    public const int BatchId = -9999999;                                   //一键申请ID设置
    public sealed partial class GuildMainData:IGuildMainData
    {
        private GuildState guildState = GuildState.None;                        //加入或是未加入
        private NotJoinViewState joinOrCreateState = NotJoinViewState.None;     //加入或是创建
        private List<FunctionOpen> functionOpen;                                //功能开放表
        private List<GuildBaseInfoDto> guildList;                               //公会列表
        private List<GuildBaseInfoDto> tmpGuildList;                            //本地缓存公会列表
        public int reqGuildPageIndex = 1;                                       //当前请求工会列表第几页
        private int curServerGuildCount = 0;                                    //当前服务器公会数量
        private IsSearchGuildState searchGuildState = IsSearchGuildState.No;    //是否搜索公会状态，默认不是
        private bool needReposition = false;                                    //需要重置panel位置
        private Dictionary<int, int> repeatRequestDic = new Dictionary<int, int>();            //已申请加入公会
        private PlayerGuildInfoDto playerGuildInfo = null;                      //自身公会信息

        private SceneMap guildScene = null;             //公会场景
        public SceneMap GuildScene
        {
            get
            {
                if(guildScene == null)
                {
                    guildScene = DataCache.getArrayByCls<SceneMap>().Find(e => e.type == (int)SceneMap.SceneType.Guild);
                }
                return guildScene;
            }
        }
        public PlayerGuildInfoDto PlayerGuildInfo
        {
            get { return playerGuildInfo; }
            private set { }
        }

        public void InitData()
        {
        }

        public void Dispose()
        {
            playerGuildInfo = null;
        }

        //覆盖公会数据
        public void UpdateGuildState(ScenePlayerDto dto)
        {
            if(dto != null)
            {
                if (dto.guildInfoDto == null && playerGuildInfo != null)
                {
                    GuildMainViewLogic.CloseHasJoinView();  //被开除后关闭界面
                }
            }
            playerGuildInfo = dto == null ? null : dto.guildInfoDto;
        }
        public void UpdateGuildState(PlayerGuildInfoDto data)
        {
            playerGuildInfo = data;
        }
        #region 加入或创建
        //是否加入公会
        public GuildState GuildState
        {
            get
            {
                guildState = playerGuildInfo == null ? GuildState.NotJoin : GuildState.HasJoin;
                return guildState;
            }
        }

        public NotJoinViewState JoinOrCreateState
        {
            get { return joinOrCreateState; }
            set { joinOrCreateState = value; }
        }
        //创建公会等级限制
        public int CreateLevelLimit
        {
            get
            {
                int level = 0;
                if(functionOpen==null)
                     functionOpen = DataCache.getArrayByCls<FunctionOpen>();
                level = functionOpen.Find(e => e.id == (int)FunctionOpen.FunctionOpenEnum.FUN_34).grade;
                return level;
            }
        }

        //创建所需钻石或金币
        public int CreateNeed(bool isDiamond)
        {
            var diamond = DataCache.GetStaticConfigValue(isDiamond ? AppStaticConfigs.GUILD_CREATE_DIAMOND : AppStaticConfigs.GUILD_CREATE_GOLD);
            return diamond;
        }

        //TODO 当前服务器公会数量(现在是模拟数量)
        public int CurServerGuildCount
        {
            get { return curServerGuildCount; }
        }

        //公会列表数据刷新
        public void UpdateGuildList(GuildBaseInfoListDto dto)
        {
            if (guildList == null)
                guildList = new List<GuildBaseInfoDto>();
            if (tmpGuildList == null)
                tmpGuildList = new List<GuildBaseInfoDto>();

            needReposition = false;
            //此处接口是所有公会列表数据，所以需要与缓存数据进行比对，防止界面逻辑(guildList)和业务逻辑(tmpGuildList)在汇总时不一样。
            if(guildList.Count != tmpGuildList.Count)
            {
                tmpGuildList.ForEach(e =>
                {
                    var val = guildList.Find(f => f.showId == e.showId);
                    if (val == null) guildList.Add(e);
                });
            }
            dto.guildList.ForEach(e =>
            {
                var val = guildList.Find(f => f.showId == e.showId);
                if (val == null) guildList.Add(e);

                var val2 = tmpGuildList.Find(f => f.showId == e.showId);
                if (val2 == null) tmpGuildList.Add(e);
            });
            int page = guildList.Count / ReqGuildListCount;
            reqGuildPageIndex = page + 1;
        }

        //当前服务器数量刷新
        public void UpdateGuildCount(GuildCountDto dto)
        {
            curServerGuildCount = dto.count;
        }
        
        //搜索时的公会列表数据刷新
        public void UpdateGuildList(GuildBaseInfoDto dto)
        {
            searchGuildState = IsSearchGuildState.Yes;
            needReposition = true;
            if (guildList == null)
                guildList = new List<GuildBaseInfoDto>();
            guildList.Clear();
            guildList.Add(dto);
            FireData();
        }
        //搜索时的公会列表数据刷新
        public void UpdateSearchGuildList(GuildBaseInfoListDto dto)
        {
            searchGuildState = IsSearchGuildState.Yes;
            needReposition = true;
            if (guildList == null)
                guildList = new List<GuildBaseInfoDto>();
            guildList.Clear();
            dto.guildList.ForEach(e =>
            {
                guildList.Add(e);
            });
            FireData();
        }

        //返回所有公会列表数据刷新
        public void UpdateGuildList()
        {
            if (searchGuildState == IsSearchGuildState.No) return;
            searchGuildState = IsSearchGuildState.No;
            needReposition = false;
            if (guildList != null)
                guildList.Clear();
            if (tmpGuildList != null)
            {
                tmpGuildList.ForEach(e =>
                {
                    guildList.Add(e);
                });
            }
            FireData();
        }

        private const string REPEATREQUST = "GuildRepeatRequst";
        //重复申请处理
        public void RepeatRequstHandler(int showId)
        {
            repeatRequestDic.AddOrReplace(showId, showId);
            JSTimer.Instance.SetupCoolDown(REPEATREQUST + showId, 60, null, () =>
            {
                if (repeatRequestDic.ContainsKey(showId))
                    repeatRequestDic.Remove(showId);
            });
        }
        public bool IsRepeatRequst(int showId)
        {
            return repeatRequestDic.ContainsKey(showId);
        }

        //是否搜索状态
        public IsSearchGuildState SearchGuildState
        {
            get { return searchGuildState; }
        }

        //是否需要重置位置
        public bool NeedReposition
        {
            get { return needReposition; }
        }

        //公会列表
        public IEnumerable<GuildBaseInfoDto> GuildList
        {
            get
            {
                return guildList;
            }
        }

        //缓存公会列表
        public IEnumerable<GuildBaseInfoDto> TmpGuildList
        {
            get { return tmpGuildList; }
        }

        //每次退出界面都需要清空数据，因为不实时刷新数据，只针对打开界面刷新
        public void ClearGuildList()
        {
            reqGuildPageIndex = 1;
            guildList = null;
            tmpGuildList = null;
        }
        #endregion

        #region 信息

        private List<GuildMemberDto> guildMemberList;   //公会成员列表    （界面）
        private List<GuildMemberDto> tmpGuildMemberList;//缓存公会成员列表（数据）
        public int reqMemPageIndex = 1;                 //请求公会成员页数

        private List<GuildPosition> guildPositionList;  //公会职位表
        private GuildBaseInfoDto guildBaseInfo;         //当前公会信息

        private List<GuildApprovalDto> guildApprovalList;//申请列表
        public int reqRequesterPageIndex = 1;            //请求申请列表页数
        public string applyIdStr = "";                 //拼接已接收入会id       
        private ErrorCode ErrorCodes(int key)
        {
            var data = DataCache.getDtoByCls<ErrorCode>(key);
            return data;
        }

        //刷新公会成员数据
        public void UpdateMemList(GuildMemberListDto dto)
        {
            guildBaseInfo = dto.guildBaseInfoDto;
            if (guildMemberList == null) guildMemberList = new List<GuildMemberDto>();
            dto.memberList.ForEach(e =>
            {
                var val = guildMemberList.Find(f => f.id == e.id);
                if (val == null)
                    guildMemberList.Add(e);
            });
            int page = guildMemberList.Count / ReqMemListCount;
            reqMemPageIndex = page + 1;
        }
        //开除成员后再次请求本页数据
        public void UpdateMemList(long memId)
        {
            guildMemberList.Remove(e => e.id == memId);
        }

        //修改职位，更新列表,客户端修改数据
        public void UpdateMemList(long memId,int pos)
        {
            if (guildMemberList == null) guildMemberList = new List<GuildMemberDto>();
            var val = guildMemberList.Find(e => e.id == memId);
            if(val != null)
            {
                GuildMemberDto dto = val;
                dto.position = pos;
                guildMemberList.Replace<GuildMemberDto>(e => e.id == memId, dto);
            }
            FireData();
        }
        //移交会长，客户端自己修改数据
        public void TransferBoss(long memId)
        {
            if (guildMemberList == null) guildMemberList = new List<GuildMemberDto>();
            var member = guildMemberList.Find(e => e.id == memId);
            var boss = guildMemberList.Find(e => e.position == (int)AppDto.GuildPosition.GuildPositionEnum.Boss);
            if (boss != null)
            {
                int pos = (int)AppDto.GuildPosition.GuildPositionEnum.Masses;
                boss.position = pos;    //修改列表数据
                guildMemberList.Replace<GuildMemberDto>(e => e.position == (int)AppDto.GuildPosition.GuildPositionEnum.Boss, boss);
                //修改自己公会数据
                playerGuildInfo.positionId = pos;
                playerGuildInfo.position.id = pos;
            }
            if(member != null)
            {
                member.position = (int)AppDto.GuildPosition.GuildPositionEnum.Boss;
                guildMemberList.Replace<GuildMemberDto>(e => e.id == memId, member);
            }
            FireData();
        }

        //公会成员列表
        public IEnumerable<GuildMemberDto> GuildMemberList
        {
            get { return guildMemberList; }
        }

        //公会职位表
        public IEnumerable<GuildPosition> GuildPosition
        {
            get
            {
                if (guildPositionList == null)
                    guildPositionList = DataCache.getArrayByCls<GuildPosition>();
                return guildPositionList;
            }
        }

        //当前公会信息
        public GuildBaseInfoDto GuildBaseInfo { get { return guildBaseInfo; } }

        //刷新公会申请列表数据(showRequestView区别打开申请界面和一键接收)
        public void UpdateApprovalList(GuildApprovalListDto dto, bool showRequestView)
        {
            if (guildApprovalList == null) guildApprovalList = new List<GuildApprovalDto>();

            if (!showRequestView)
            {
                guildApprovalList.Clear();
                if (dto.memberFull)
                    TipManager.AddTip(ErrorCodes(AppErrorCodes.GUILD_MEMBER_FULL).message);
            }

            dto.approvals.ForEach(e =>
            {
                var val = guildApprovalList.Find(f => f.applyerId == e.applyerId);
                if (val == null)
                    guildApprovalList.Add(e);
            });
            guildApprovalList.RemoveItems(e => e.applyStat == (int)GuildApprovalDto.GuildApplyStat.Comrade);
            int page = guildApprovalList.Count / ReqRequestListCount;
            reqRequesterPageIndex = page + 1;
        }
        //刷新公会申请列表数据(批准入会成功)
        public void UpdateApprovalList(GuildApprovalDto dto)
        {
            long id = dto.applyerId;
            applyIdStr += id + ",";
            if (guildApprovalList == null || dto == null)
            {
                GameDebuger.Log(dto.GetType() + "数据有错");
                return;
            }

            guildApprovalList.Replace<GuildApprovalDto>(f => f.applyerId == dto.applyerId, dto);
            guildApprovalList.Remove(e => dto.applyStat == (int)GuildApprovalDto.GuildApplyStat.Comrade);
        }
        //清除申请列表
        public void ClearApprovalDataList()
        {
            if (guildApprovalList == null) guildApprovalList = new List<GuildApprovalDto>();
            guildApprovalList.Clear();
            FireData();
        }

        //申请列表
        public IEnumerable<GuildApprovalDto> GuildApprovalList { get { return guildApprovalList; } }


        //每次退出界面都需要清空数据，因为不实时刷新数据，只针对打开界面刷新
        public void ClearMemList()
        {
            reqMemPageIndex = 1;
            guildMemberList = null;
        }
        //清空申请列表数据
        public void ClearApprovalList()
        {
            reqRequesterPageIndex = 1;
            guildApprovalList = null;
            applyIdStr = "";
        }
        #endregion

        #region 管理

        private GuildDetailInfoDto guildDetailInfo = null;      //公会详细信息
        public GuildDetailInfoDto GuildDetailInfo { get { return guildDetailInfo; } }
        private List<GuildBuilding> guildBuildList;
        public IEnumerable<GuildBuilding> GuildBuildList
        {
            get
            {
                if (guildBuildList == null)
                    guildBuildList = DataCache.getArrayByCls<GuildBuilding>();
                return guildBuildList;
            }
        }
        
        public void UpdateGuildDetailInfo(GuildDetailInfoDto dto)
        {
            guildDetailInfo = dto;
            guildBaseInfo = dto.baseInfo;
            SetBuildUpTimeList(dto.buildingInfo);
            FireData();
        }
        public void ChangeNoticeMessage(NoticeModificationViewController.NoticeType noticeType,string message)
        {
            if (noticeType == NoticeModificationViewController.NoticeType.guildNotic)
                guildBaseInfo.notice = message;
            else
                guildBaseInfo.memo = message;
            FireData();
        }

        #endregion

        #region 建筑

        private List<string> buildList = null;      //建筑
        public IEnumerable<string> BuildList
        {
            get
            {
                if (buildList == null)
                {
                    var build = DataCache.GetStaticConfigValues(AppStaticConfigs.GUILD_BUILDING_NAME);
                    var split = build.Split(new char[] { '/' });
                    buildList = new List<string>();
                    split.ForEach(e =>
                    {
                        var val = e.Split(new char[] { '-' });
                        buildList.Add(val[1]);
                    });
                }
                return buildList;
            }
        }
        //1-规模 2-酒馆 3-等级 4-哨塔 5-工坊
        private List<long> buildUpTimeList = new List<long>() { 0, 0, 0, 0, 0, 0 };  
        public IEnumerable<long> BuildUpTimeList { get { return buildUpTimeList; } }

        private void SetBuildUpTimeList(GuildBuildingDto dto)
        {
            buildUpTimeList[1] = dto.gradeFinishTime;
            buildUpTimeList[2] = dto.barpubFinishTime;
            buildUpTimeList[3] = dto.treasuryFinishTime;
            buildUpTimeList[4] = dto.guardTowerFinishTime;
            buildUpTimeList[5] = dto.workshopFinishTime;
        }

        public void UpdateBuildUpTime(int idx,OnUpgradeBuildingDto dto)
        {
            buildUpTimeList[idx] = dto.finishTime;
            guildDetailInfo.wealthInfo.assets = dto.assets;
        }
        #endregion
    }
}
