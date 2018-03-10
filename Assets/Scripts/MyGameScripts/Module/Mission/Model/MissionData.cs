// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : fish
// Created  : 07/15/2017 16:35:59
// **********************************************************************
using AppDto;
using System;
using System.Collections.Generic;
using GamePlot;
using AppServices;

public partial interface IMissionData
{
    IEnumerable<Copy> GetCopyMisList { get; }
    IEnumerable<CopyMissionConfig> GetCopyMisConfig { get; }
    Dictionary<int, Mission> GetAllMissionDic { get;}
    IEnumerable<PlayerMissionDto> GetMissionListDto { get; }
    List<Mission> GetExistSubMissionMenuList();
    List<Mission> GetMissedSubMissionMenuList();
    List<MissionOption> GetMissionOptionListByNpcInternal(Npc npc);
    DialogueOption GetDialogueOptionByMissionOptionList(List<MissionOption> missionOptionList,Npc npc);
    Npc GetMissionNpcByMission(Mission mission,bool existSta);
    Npc NpcVirturlToEntity(Npc npc);
    IEnumerable<object> GetMissionCellData();
    List<int> GetPreMissionIDList();
    void FinishTargetSubmitDto(Mission mission,Npc npc,int submitIndex,int battleIndex = 0);
    void TreasureMission(IEnumerable<BagItemDto> bagtoList,BagItemDto tCurBagItem);

    //新任务系统
    SubmitDto GetSubmitDto(Mission mission,int submitIndex = -1);
    NpcInfoDto GetCompletionConditionNpc(Mission mission,int submitIndex = -1,bool getSubmitNpc = false);
    void Accpet(Mission mission);
    void ClearLastFindMission();
    void DropMission(Mission mission);
    void RemovePlayerMissionWhenDrop(int missionid);
    MissionShopItemMarkModel GetMissionShopItem();
    MissionStatDto GetMissionStatDto(int missiontype);

}

public sealed partial class MissionDataMgr
{
    public sealed partial class MissionData:IMissionData
    {
        //readonly UniRx.Subject<int> clickItemStream = new UniRx.Subject<int>();
        //public UniRx.IObservable<int> OnClickItemStream
        //{
        //    get { return clickItemStream; }
        //}
        //	所有任务信息（静态的）
        private MissionDelegateFactory _missionDelegateFactory;
        private SubmitDelegateFactory _submitDelegateFactory;
        private Dictionary<int, Mission> _allMissionDic = null;
        private Dictionary<int, MissionType> _MissionTypeDic = null;
        private PlayerMissionListDto _playerMissionListDto = null;
        private IDisposable _disposable;
        private MissionStoryPlotDelegate mMissionStoryPlotDelegate;
        private MissionApplyItemSubmitDtoDelegate mMissionApplyItemSubmitDtoDelegate;
        //	上次寻路任务ID
        private Mission _lastFindMission = null;
        private int TreasureMaxCount = 0;
        //这个是捉鬼的环数，因为现在还没有每日活动，所以暂时先放在这里
        public static int mGhostRingCount;
        #region 一个对话选项的枚举
        private DialogueOption _dialogueOption = DialogueOption.NothingOption;
        MissionShopItemMarkModel mMissionShopItemMarkModel = new MissionShopItemMarkModel();
        public DialogueOption dialogueOption
        {
            get { return _dialogueOption; }
        }
        #endregion

        public void InitData()
        {
            TreasureMaxCount = DataCache.GetStaticConfigValue(AppStaticConfigs.TREASURY_MISSION_REWARD_MAX_COUNT);
            InitRecorList();
            mMissionStoryPlotDelegate = new MissionStoryPlotDelegate(this);
            mMissionApplyItemSubmitDtoDelegate = new MissionApplyItemSubmitDtoDelegate(this);
        }

        public void Dispose()
        {
            DisposeCore();
        }
        //====================================================服务器推送信息====================================================
        #region 服务器推送数据更新

        /// <summary>
        /// 进入游戏传入missionlistdto
        /// </summary>
        /// <param name="dto"></param>
        public void UpdateMissionListDto(PlayerMissionListDto dto)
        {
            SetupCore();
            _missionDelegateFactory = new MissionDelegateFactory();
            _missionDelegateFactory.Setup(this);
            _submitDelegateFactory = new SubmitDelegateFactory();
            _submitDelegateFactory.Setup(this);
            for(int i = 0;i<dto.bodyMissions.Count;i++) {
                SubmitDto submitDto = GetSubmitDto(dto.bodyMissions[i].mission);
                IMissionDelegate missionDelegate = _missionDelegateFactory.GetHandleMissionDelegate(dto.bodyMissions[i].mission.type);
                missionDelegate.ReqEnterMission(dto.bodyMissions[i].mission,submitDto);
            }
            //
            _playerMissionListDto = dto;
            mGhostRingCount = _playerMissionListDto.ghostRingCount;
            
            //todo： 这里对数据模块创建顺序有依赖  assign to 张俊杰－－fish
            //刷新NPC头顶
            WorldManager.Instance.GetNpcViewManager().RefinishMissionFlag();
            GamePlotManager.Instance.Setup();
        }


        #region 接受任务处理
        /// <summary>
        /// 接受任务(这是自己主动请求)
        /// </summary>
        /// <param name="mission"></param>
        public void Accpet(Mission mission) {
            if(mission == null) {
                GameDebuger.LogError("Accept Mission is Null");
                return;
            }
            //MissionNetMsg.ReqAcceptMission(index);
            IMissionDelegate missionDelegate = _missionDelegateFactory.GetHandleMissionDelegate(mission.type);
            //是否满足在接受条件
            bool isAccept= missionDelegate.AcceptMission();
            if(isAccept) {
                if(mission.type != (int)MissionType.MissionTypeEnum.Guild)
                    MissionNetMsg.ReqAcceptMission(mission.id);
                else
                    MissionNetMsg.AcceptMissionGuild();
            }
        }

        /// <summary>
        /// 接受任务(这是服务器自动下发的任务)
        /// </summary>
        /// <param name="dto"></param>
        public void AcceptMission(PlayerMissionDto dto)
        {
            BattleDelayHandle(() => {
                IMissionDelegate missionDelegate = _missionDelegateFactory.GetHandleMissionDelegate(dto.mission.type);
                AcceptMissionFinish(dto);
                mMissionStoryPlotDelegate.StoryPlotInMission(dto.mission,true);
                if(GamePlotManager.Instance.TriggerPlot((int)Plot.PlotTriggerType.AcceptMission,dto.missionId))
                {
                    // 这个监听在 GamePlotManager.FinishPlot 清空了
                    GamePlotManager.Instance.OnFinishPlot += (plot) =>
                    {
                        missionDelegate.AcceptMissionFinish(dto);
                    };
                }
                else
                {
                    missionDelegate.AcceptMissionFinish(dto);
                }
            });
        }

        void AcceptMissionFinish(PlayerMissionDto dto) {
            if(_playerMissionListDto == null) {
                GameDebuger.LogError("MissionModel 数据未初始化,不能处理任务数据 : AcceptMissionFinish , 遇到该报错把复现方法一起告知程序");
                // TODO 这里看看是否有必要把数据缓存起来,等待 _playerMissionListDto 初始化后在处理
                return;
            }
            //	添加入本地任务数据列表
            int index = _playerMissionListDto.bodyMissions.FindIndex(mission => mission.missionId == dto.missionId);
            if(index != -1)
                _playerMissionListDto.bodyMissions[index] = dto;
            else
                _playerMissionListDto.bodyMissions.Add(dto);

            // 场景Npc刷新 明雷 暗雷
            AddMissionNpc(dto.mission);
            FireData();
        }

        #endregion

        #endregion
        //====================================================  任务信息  ====================================================
        #region 任务信息
        /// <summary>
        /// 获取当前绑定所有的任务
        /// </summary>
        public IEnumerable<PlayerMissionDto> GetMissionListDto
        {
            get
            {
                 return _playerMissionListDto != null ? _playerMissionListDto.bodyMissions : new List<PlayerMissionDto>();
                //return _playerMissionListDto.bodyMissions != null ? _playerMissionListDto.bodyMissions : new List<PlayerMissionDto>();
            }
        }

        /// <summary>
        /// 获取已接任务具体的任务信息
        /// </summary>
        /// <returns></returns>
        public List<Mission> GetExistSubMissionMenuList()
        {
            var playerMissionDto = GetMissionListDto;
            if (playerMissionDto != null)
            {
                List<Mission> result = new List<Mission>();
                playerMissionDto.ForEach(e =>
                {
                    if (e.mission != null)
                    {
                        result.Add(e.mission);
                    }
                });
                return result;
            }
            return new List<Mission>();
        }

        #region 获取可接任务信息
        /// <summary>
        /// 获取可接任务具体任务信息
        /// </summary>
        /// <returns></returns>
        public List<Mission> GetMissedSubMissionMenuList()
        {
            List<Mission> tMissionMenuList = new List<Mission>();
            //筛选可接任务
            //已提交任务
            var submitDic = SubmitMissionDic();
            //身上已接任务
            var exitDic = BodyMissionDic();
            //身上有同种类型的任务（不包括主/支线）
            var sampleType = IsSampleType();
            //身上已有的副本任务类型
            var exitCopyType = GetCopyMisTypeList();
            //1.不能是主线 2.不能是已提交的任务 3.不能是已接任务 4.等级判断 5.不能与身上相同任务类型 6.不能是紧急委托 7.宝图达到最大次数需屏蔽 
            //8.筛选身上没有的副本任务类型（普通，精英，else）9.不能是副本彩蛋任务
            var val = GetAllMissionDic.Filter
                (
                    e =>
                    e.Value.type != (int)MissionType.MissionTypeEnum.Master && !submitDic.ContainsKey(e.Key) && !exitDic.ContainsKey(e.Key) &&
                    JudgeAcceptionCondition(e.Value,submitDic) && !sampleType.Contains(e.Value.type) && e.Value.type != (int)MissionType.MissionTypeEnum.Urgent
                    && JudgeTreasureMis(e.Value,sampleType) && JudgeCopyMis(e.Value,exitCopyType) && e.Value.type != (int)MissionType.MissionTypeEnum.CopyExtra
                ).ToList();
            val.ForEach(e => tMissionMenuList.Add(e.Value));
            return tMissionMenuList;
        }

        //获取已接副本任务类型（普通，精英，else）
        private List<int> GetCopyMisTypeList()
        {
            List<int> list = new List<int>();
            var tExistSubMission = GetExistSubMissionMenuList();
            int count = tExistSubMission.Count;
            for(int i = 0; i < count; i++)
            {
                var mis = tExistSubMission[i];
                if (mis.type == (int)MissionType.MissionTypeEnum.Copy)
                {
                    var val = GetCopyMisConfig.Find(e => e.id == mis.id);
                    if (val != null)
                    {
                        if (!list.Contains(val.copyId))
                            list.Add(val.copyId);
                    }
                }
            }
            return list;
        }
        //筛选身上没有的副本任务类型（普通，精英，else）
        private bool JudgeCopyMis(Mission mis,List<int> hasCopyList)
        {
            if(mis.type == (int)MissionType.MissionTypeEnum.Copy)
            {
                var val = GetCopyMisConfig.Find(e => e.id == mis.id);
                if (val != null)
                {
                    return !hasCopyList.Contains(val.copyId);
                }
            }
            return true;
        }

        //判断宝图任务是否已完成最大次数
        private bool JudgeTreasureMis(Mission mis,List<int> sampleType)
        {
            if (mis.type == (int)MissionType.MissionTypeEnum.Treasury && !sampleType.Contains(mis.type))
            {
                var val = GetMissionStatDto(mis.type);
                return val.daily < TreasureMaxCount;
            }
            return true;
        }
        //先判断前置条件
        //有等级限制则判断等级，无等级限制则直接return true
        private bool JudgeAcceptionCondition(Mission mission, Dictionary<int, int> submitDic)
        {
            bool AcceptionMission = false;
            if(mission !=null && mission.acceptConditions!=null)
            {
                var list = mission.acceptConditions.acceptConditionList;
                for (int i = 0, max = list.Count; i < max; i++)
                {
                    var val = list[i] as AcceptionCondtion_1;
                    var val2 = list[i] as AcceptionCondtion_2;
                    var val3 = list[i] as AcceptionCondtion_3;
                    if(val != null)
                    {
                        if(ModelManager.Player.GetPlayerLevel() >= val.grade) AcceptionMission = true;
                    }
                    else if(val2 != null)
                    {
                        if(submitDic.ContainsKey(val2.preId)) AcceptionMission = true;
                    }
                    else if(val3 != null)
                    {
                        //分支任务不需要显示在可接界面
                        if(val3.multi == 1) AcceptionMission = false;
                        else AcceptionMission = true;
                    }

                    //只要一个条件不满足就跳出
                    if(!AcceptionMission) {
                        break;
                    }
                    //if(val2 != null)
                    //{
                    //    if(submitDic.ContainsKey(val2.preId)) return true;
                    //}
                    //else
                    //{
                    //    if(val != null)
                    //    {
                    //        if(ModelManager.Player.GetPlayerLevel() >= val.grade) return true;
                    //    }

                    //    if(val3 != null)
                    //    {
                    //        //显示1是分支不需要显示在可接界面
                    //        if(val3.multi == 1)
                    //        {
                    //            return false;
                    //        }
                    //    }
                    //    else return true;
                    //}
                }
            }
            return AcceptionMission;
        }
        private Dictionary<int,int> SubmitMissionDic()
        {
            var subList = _playerMissionListDto.submitMissionIds;
            Dictionary<int, int> tmpDic = new Dictionary<int, int>();
            for(int i =0,max = subList.Count; i < max; i++)
            {
                tmpDic[subList[i]] = 1;
            }
            return tmpDic;
        }
        private Dictionary<int,int> BodyMissionDic()
        {
            var tExistSubMission = GetExistSubMissionMenuList();
            Dictionary<int, int> tmpDic = new Dictionary<int, int>();
            for (int i = 0, len = tExistSubMission.Count; i < len; i++)
            {
                tmpDic[tExistSubMission[i].id] = 1;
            }
            return tmpDic;
        }
        private List<int> IsSampleType()
        {
            var tExistSubMission = GetExistSubMissionMenuList();
            List<int> sampleList = new List<int>();
            for (int i = 0, len = tExistSubMission.Count; i < len; i++)
            {
                if(tExistSubMission[i].type >= (int)MissionType.MissionTypeEnum.Faction && !sampleList.Contains(tExistSubMission[i].type)
                    &&tExistSubMission[i].type != (int)MissionType.MissionTypeEnum.Copy)
                    sampleList.Add(tExistSubMission[i].type);
            }
            return sampleList;
        }
        #endregion

        /// <summary>
        /// 通过id获取当前绑定的任务
        /// </summary>
        public PlayerMissionDto GetPlayerMissionDtoByMissionID(int missionID)
        {
            List<PlayerMissionDto> tPlayerMissionDtoList = GetMissionListDto.ToList();
            if (tPlayerMissionDtoList != null)
            {
                for(int i = 0,len = tPlayerMissionDtoList.Count; i < len; i++)
                {
                    if(tPlayerMissionDtoList[i].missionId == missionID)
                    {
                        return tPlayerMissionDtoList[i];
                    }
                }
            }
            return null;
        }

        #region 从本地数据中删除相关信息

        public void DeletePlayerMissionDtoByMissionCleanNotify(MissionCleanNotify notify) {
            BattleDelayHandle(() =>
            {
                RemovePlayerMissionWhenNotify(notify.missionId);
            });
        }
        private void RemovePlayerMissionWhenNotify(int missionId) {
            PlayerMissionDto playerMissionDto = GetPlayerMissionById(missionId);
            if(playerMissionDto == null)
                return;

            for(int index = 0;index < playerMissionDto.completions.Count;index++)
            {
                SubmitDto submitDto = playerMissionDto.completions[index];
                ISubmitDelegate submitDelegate = _submitDelegateFactory.GetHandleSubmitDelegate(submitDto.GetType().Name);
                submitDelegate.SubmitClear(submitDto);
            }
            Mission mission = playerMissionDto.mission;
            if(mission.submitNpc is NpcMonster) {
                WorldManager.Instance.GetNpcViewManager().RemoveNpc(mission.submitNpc.id);
            }
            WorldManager.Instance.GetNpcViewManager().HandlerMissionNpcStatus(mission.submitNpcStatus);
            _playerMissionListDto.bodyMissions.Remove(playerMissionDto);
        }

        /// <summary>
        /// 放弃任务
        /// </summary>
        /// <param name="mission"></param>
        public void DropMission(Mission mission) {
            string winTips = "";
            IMissionDelegate missionDelegate = _missionDelegateFactory.GetHandleMissionDelegate(mission.type);
            bool isDorp = missionDelegate.DropMission(mission,out winTips);
            if(isDorp) {
                if(string.IsNullOrEmpty(winTips))
                {
                    MissionNetMsg.ReqDropMission(mission.id);
                }
                else {
                    var ctrl = ProxyBaseWinModule.Open();
                    BaseTipData data = BaseTipData.Create(mission.name,winTips, 0, delegate
                    {
                        MissionNetMsg.ReqDropMission(mission.id);
                    }, null);
                    ctrl.InitView(data);
                }
            }
        }

        public void RemovePlayerMissionWhenDrop(int missionid) {
            PlayerMissionDto playerMissionDto = GetPlayerMissionById(missionid);
            if(playerMissionDto == null)
                return;

            for(int index = 0;index < playerMissionDto.completions.Count;index++) {
                SubmitDto submitDto = playerMissionDto.completions[index];
                ISubmitDelegate submitDelegate =_submitDelegateFactory.GetHandleSubmitDelegate(submitDto.GetType().Name);
                submitDelegate.SubmitClear(submitDto);
            }
            Mission mission = playerMissionDto.mission;
            WorldManager.Instance.GetNpcViewManager().HandlerMissionNpcStatus(mission.acceptNpcStatus);
            _playerMissionListDto.bodyMissions.Remove(playerMissionDto);
            WorldManager.Instance.GetNpcViewManager().RefinishMissionFlag();
        }

        public void RemovePlayerMissionWhenSubmit(int missionId,SubmitDto submitDto = null) {
            PlayerMissionDto playerMissionDto = GetPlayerMissionById(missionId);
            if(playerMissionDto == null)
                return;
            //_lastSubmitMission = playerMissionDto.mission;
            if(submitDto == null)
                submitDto = GetSubmitDto(playerMissionDto.mission);
            if(playerMissionDto.mission.submitNpc is NpcMonster)
                WorldManager.Instance.GetNpcViewManager().RemoveNpc(playerMissionDto.mission.submitNpc.id);
            WorldManager.Instance.GetNpcViewManager().HandlerMissionNpcStatus(playerMissionDto.mission.submitNpcStatus);
            //	每日累加、总量累加
            //AddMissionStat(playerMissionDto);
            //	已完成的任务加入已提交列表中  //引导任务和帮派学徒任务先忽略
            if(MissionHelper.IsMainOrExtension(playerMissionDto.mission))
            {
                _playerMissionListDto.submitMissionIds.Add(missionId);
            }

            IMissionDelegate missionDelegate = _missionDelegateFactory.GetHandleMissionDelegate(playerMissionDto.mission.type);
            missionDelegate.FinishMission(playerMissionDto,submitDto);
            //	从当前绑定任务中删除信息
            _playerMissionListDto.bodyMissions.Remove(playerMissionDto);
            WorldManager.Instance.GetNpcViewManager().RefinishMissionFlag();
        }

        public void UpdataMissionStatDtoNotify(MissionStatDto tMissionStatDto)
        {
            List<MissionStatDto> tMissionStatDtoList = _playerMissionListDto.missionStat;
            for(int i = 0, len = tMissionStatDtoList.Count;i < len;i++)
            {
                if(tMissionStatDtoList[i].missionType == tMissionStatDto.missionType)
                {
                    tMissionStatDtoList[i] = tMissionStatDto;
                    break;
                }
            }
        }
        #endregion

        #region 主界面跟踪面板具体任务信息

        public List<Mission> GetSubMissionMenuListInMainUIExpand()
        {
            List<Mission> tSubMissionMenuList = GetExistSubMissionMenuList();
            int tSceneID = MissionHelper.GetCurrentSceneID();
            mMissionApplyItemSubmitDtoDelegate.ResetApplyMission(tSubMissionMenuList,tSceneID);
            tSubMissionMenuList.Sort((x, y) =>
            {
                PlayerMissionDto xDto = GetPlayerMissionDtoByMissionID(x.id);
                PlayerMissionDto yDto = GetPlayerMissionDtoByMissionID(y.id);
                if (xDto == null || yDto == null) return 0;
                if(MissionHelper.IsMainMissionType(x) && MissionHelper.IsMainMissionType(y)||
                    !MissionHelper.IsMainMissionType(x) && !MissionHelper.IsMainMissionType(y))
                {//时间排序
                    return -xDto.acceptTime.CompareTo(yDto.acceptTime);
                }
                //类型排序
                return x.type.CompareTo(y.type);
            });
            return tSubMissionMenuList;
        }
        #endregion
        #endregion
        //====================================================  静态数据  ====================================================
        #region 静态数据

        /// <summary>
        /// 获取所有任务数据信息dic(静态表)
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, Mission> GetAllMissionDic
        {
            get
            {
                if (_allMissionDic == null)
                {
                    _allMissionDic = DataCache.getDicByCls<Mission>();
                }
                return _allMissionDic;
            }
        }
        private List<Copy> copyMisList = null;
        public IEnumerable<Copy> GetCopyMisList
        {
            get
            {
                if(copyMisList == null)
                {
                    copyMisList = DataCache.getArrayByCls<Copy>();
                }
                return copyMisList;
            }
        }

        private List<CopyMissionConfig> copyMisConfig = null;
        public IEnumerable<CopyMissionConfig> GetCopyMisConfig
        {
            get
            {
                if (copyMisConfig == null)
                {
                    copyMisConfig = DataCache.getArrayByCls<CopyMissionConfig>();
                }
                return copyMisConfig;
            }
        }
        #endregion
        //====================================================提交任务相关====================================================

        #region 通过mission获取任务Npc(当true：返回已接任务寻路NPC， false：返回接取NPC)
        /// <summary>
        /// 通过ID获取任务Npc(当true：返回已接任务寻路NPC， false：返回接取NPC) -- Gets the mission npc by mission.
        /// </summary>
        /// <returns>The mission npc by mission.</returns>
        /// <param name="mission">Mission.</param>
        /// <param name="existSta">If set to <c>true</c> exist sta.</param>
        public Npc GetMissionNpcByMission(Mission mission,bool existSta)
        {
            Npc tMissedNpc = null;
            if(existSta)
            {
                NpcInfoDto tNpcInfoDto = GetBindMissionNpcInfoByMission(mission);
                if(tNpcInfoDto == null)
                {
                    tMissedNpc = mission.submitNpc;
                }
                else
                {
                    tMissedNpc = GetNpcByNpcInfoDto(tNpcInfoDto);
                }
            }
            else
            {
                tMissedNpc = mission.missionType.acceptNpc;
                tMissedNpc = tMissedNpc == null ? mission.acceptNpc : tMissedNpc;
                //	当改NPC是虚拟NPC是转换为具体NPC
                tMissedNpc = NpcVirturlToEntity(tMissedNpc);
            }

            return tMissedNpc;
        }

        #endregion

        #region 获取已接任务寻路目标地点（使用BindByMission）
        /// <summary>
        /// 获取已接任务寻路目标地点（使用BindByMission） -- Gets the bind mission npc info by mission.
        /// </summary>
        /// <returns>The bind mission npc info by mission.</returns>
        /// <param name="mission">Mission.</param>
        /// <param name="getSubmitNpc">If set to <c>true</c> get submit npc.</param>
        private NpcInfoDto GetBindMissionNpcInfoByMission(Mission mission,int submitIndex = -1,bool getSubmitNpc = false)
        {
            SubmitDto tSubmitDto = null;
            if(submitIndex < 0)
            {
                tSubmitDto = MissionHelper.GetSubmitDtoByMission(mission);
            }
            else
            {
                tSubmitDto = MissionHelper.GetSubmitDtoByMission(mission,submitIndex);
            }

            NpcInfoDto tNpcInfoDto = null;

            if(tSubmitDto == null)
            {
                Npc tNpc = mission.submitNpc;
                if(tNpc != null)
                {
                    //	当改NPC是虚拟NPC是转换为具体NPC
                    tNpc = NpcVirturlToEntity(mission.submitNpc);
                    tNpcInfoDto = GetNpcInfoDtoByNpc(tNpc);
                }
            }
            else
            {
                tNpcInfoDto = GetBindMissionNpcInfoDtoBySubmit(tSubmitDto,getSubmitNpc);
            }

            return tNpcInfoDto;
        }

        /// <summary>
        /// 获取已接任务寻路目标地点（BySubmit） -- Gets the bind mission npc info dto by submit.
        /// </summary>
        /// <returns>The bind mission npc info dto by submit.</returns>
        /// <param name="submitDto">Submit dto.</param>
        /// <param name="getSubmitNpc">If set to <c>true</c> get submit npc.</param>
        public NpcInfoDto GetBindMissionNpcInfoDtoBySubmit(SubmitDto submitDto,bool getSubmitNpc = false)
        {
            NpcInfoDto tNpcInfoDto = null;
            tNpcInfoDto = GetCompletionConditionNpc(submitDto,getSubmitNpc);
            return tNpcInfoDto;
        }

        public NpcInfoDto GetNpcInfoDtoByNpc(Npc npc)
        {
            NpcInfoDto tNpcInfoDto = new NpcInfoDto();

            tNpcInfoDto.id = npc.id;
            tNpcInfoDto.name = npc.name;
            tNpcInfoDto.sceneId = npc.sceneId;
            tNpcInfoDto.x = npc.x;
            tNpcInfoDto.y = npc.y;
            tNpcInfoDto.z = npc.z;
            tNpcInfoDto.npcAppearanceId = 0;

            tNpcInfoDto.npc = npc;

            return tNpcInfoDto;
        }

        //#region 虚拟NPC转换为配置NPC
        /// <summary>
        /// 虚拟NPC转换为配置NPC -- Npcs the virturl to entity.
        /// </summary>
        /// <returns>The virturl to entity.</returns>
        /// <param name="npc">Npc.</param>
        public Npc NpcVirturlToEntity(Npc npc)
        {
            Npc tNpc = npc;
            if(npc is FactionNpc)
            {
                FactionNpc tFactionNpc = npc as FactionNpc;
                int tFactionId = ModelManager.Player.GetPlayer().factionId;
                tNpc = DataCache.getDtoByCls<Npc>(tFactionNpc.npcIds[tFactionId]);
            }
            return tNpc;
        }

        public List<Npc> GetMissionNpcListByMission(Mission mission,bool existSta)
        {
            List<Npc> npsList = new List<Npc>();
            Npc tMissedNpc = null;

            if(existSta)
            {
                List<SubmitDto> tSubmitDtoList = MissionHelper.GetSubmitDtoListByMission(mission);
                NpcInfoDto tNpcInfoDto = null;
                if(tSubmitDtoList.Count > 0)
                {
                    for(int index = 0;index < tSubmitDtoList.Count;index++)
                    {
                        SubmitDto tSubmitDto = tSubmitDtoList[index];
                        if(!tSubmitDto.finish)
                        {
                            tNpcInfoDto = GetBindMissionNpcInfoDtoBySubmit(tSubmitDto,false);
                            if(tNpcInfoDto == null)
                            {
                                tMissedNpc = mission.submitNpc;
                            }
                            else
                            {
                                tMissedNpc = GetNpcByNpcInfoDto(tNpcInfoDto);
                            }
                            if(tMissedNpc != null)
                                npsList.Add(tMissedNpc);
                        }
                    }
                }

                if(npsList.Count == 0)
                {
                    Npc tNpc = mission.submitNpc;
                    if(tNpc != null)
                    {
                        //	当改NPC是虚拟NPC是转换为具体NPC
                        tNpc = NpcVirturlToEntity(mission.submitNpc);
                        tNpcInfoDto = GetNpcInfoDtoByNpc(tNpc);
                        tMissedNpc = GetNpcByNpcInfoDto(tNpcInfoDto);
                        if(tMissedNpc != null)
                            npsList.Add(tMissedNpc);
                    }
                }
            }
            else
            {
                if(mission.type != (int)MissionType.MissionTypeEnum.Ghost && mission.type != (int)MissionType.MissionTypeEnum.Faction)
                {
                    tMissedNpc = mission.missionType.acceptNpc;
                    tMissedNpc = tMissedNpc == null ? mission.acceptNpc : tMissedNpc;

                    //	当改NPC是虚拟NPC是转换为具体NPC
                    tMissedNpc = NpcVirturlToEntity(tMissedNpc);
                    if(tMissedNpc != null)
                        npsList.Add(tMissedNpc);
                }
            }
            return npsList;
        }
        public Npc GetNpcByNpcInfoDto(NpcInfoDto npcInfoDto)
        {
            Npc tNpc = null;

            if(npcInfoDto == null || npcInfoDto.id == 0)
            {
                tNpc = null;
            }
            else
            {
                tNpc = new Npc();

                //	寻路
                tNpc.id = npcInfoDto.id;
                tNpc.name = npcInfoDto.name;
                tNpc.sceneId = npcInfoDto.sceneId;
                tNpc.x = npcInfoDto.x;
                tNpc.y = npcInfoDto.y;
                tNpc.z = npcInfoDto.z;

                if(npcInfoDto.npc != null)
                {
                    tNpc.type = npcInfoDto.npc.type;
                    tNpc.modelId = npcInfoDto.npc.modelId;
                    tNpc.diglogface = npcInfoDto.npc.diglogface;
                }
            }

            return tNpc;
        }
        #endregion

        #region 遍历已接\可接任务列表，判断当前NPC是否拥有任务相关对话内容，并返回一个列表（DataModel内部对 NPC（功能\门派\帮派） 进行不同处理机制）
        public List<MissionOption> GetMissionOptionListByNpcInternal(Npc npc)
        {
            List<MissionOption> tMissionOptionList = new List<MissionOption>();
            //	查找已接任务列表
            List<Mission> tCurMissionList=new List<Mission>();
            //查找可接任务列表
            List<Mission> tAceMissionList=GetMissedSubMissionMenuList();
            //把当前任务加入到tCurMissionList
            GetMissionListDto.ForEach(e =>
            {
                tCurMissionList.Add(e.mission);
            });
            for(int i = 0, imax = tCurMissionList.Count + tAceMissionList.Count;i < imax;i++)
            {
                Mission tMission=i<tCurMissionList.Count?tCurMissionList[i]:tAceMissionList[i-tCurMissionList.Count];
                if(tMission == null) continue;
                bool tIsExist= i < tCurMissionList.Count;
                Npc tAccNpc=null;
                if(!MissionHelper.IsFactionMissionType(tMission) && !MissionHelper.IsBuffyMissionType(tMission))
                {
                    tAccNpc = GetMissionNpcByMission(tMission,false);
                }
                //以上如果他不是捉鬼任务和委托任务就进入
                List<Npc> tCurNpcList = GetMissionNpcListByMission(tMission, tIsExist);

                bool tAddToList = false;
                Npc intCurListNpc = tCurNpcList.Find(
                    delegate(Npc n)
                    {
                        return n.id == npc.id;
                    }
                );


                if((tCurNpcList.Count > 0 && intCurListNpc != null && tIsExist) || (tAccNpc != null && npc.id == tAccNpc.id && !tIsExist))
                {
                    MissionOption tMissionOption = new MissionOption(tMission, tIsExist);
                    if(tIsExist)
                    {
                        tAddToList = true;
                        //tMissionOptionList.Add(tMissionOption);
                    }
                    else
                    {
                        if(tMission.type >= 3)
                            tAddToList = false;
                        else
                            tAddToList = true;
                        SubmitDto tSubmitDto = MissionHelper.GetSubmitDtoByMission(tMission);
                        MissionHelper.SubmitDtoType tSubmitDtoType = MissionHelper.GetSubmitDtoTypeBySubmitDto(tSubmitDto);
                        if(tSubmitDtoType == MissionHelper.SubmitDtoType.CollectionItemCategory || tSubmitDtoType == MissionHelper.SubmitDtoType.CollectionItem) {
                            tAddToList = false;
                        }
                    }

                    if(tAddToList)
                    {
                        tMissionOptionList.Add(tMissionOption);
                    }
                }
            }
            tMissionOptionList.Sort((missionOptionX,missionOptionY) =>
            {
                if(missionOptionX.mission.type == missionOptionY.mission.type)
                {
                    return missionOptionX.mission.id.CompareTo(missionOptionY.mission.id);
                }
                return missionOptionX.mission.type.CompareTo(missionOptionY.mission.type);
            });
            return tMissionOptionList;
        }
        #endregion

        #region 获取当前对话选项的模式 return_dialogueOption
        public DialogueOption GetDialogueOptionByMissionOptionList(List<MissionOption> missonOptionList,Npc npc)
        {
            //	任务选项数据枚举
            _dialogueOption = DialogueOption.NothingOption;

            int tMissionOptionCount = missonOptionList.Count;
            //提交任务时候，如果只有一个选项。并且是师门任务的时候，就去掉，不显示
            int tFunctionOptionCount = npc is NpcGeneral ? (npc as NpcGeneral).dialogFunctionIds.Count : 0;
            if(tFunctionOptionCount == 1 && (npc as NpcGeneral).dialogFunctionIds[0] == 4)
            {
                tFunctionOptionCount = 0;
            }
            //	判断当前 Npc 绑定功能列表数量
            if(tFunctionOptionCount > 0)
            {
                //	判断当前 Npc 绑定（挂载）任务数量（Npc挂载功能选项）
                if(tMissionOptionCount > 0)
                {
                    //	当前有多个任务 有多个 Npc 功能选项
                    _dialogueOption = DialogueOption.Function_MainMission;
                }
                else
                {
                    //	仅有 Npc 功能列表选项 不做处理
                    _dialogueOption = DialogueOption.OnlyFunction;
                }
            }
            else
            {
                //	判断当前 Npc 绑定（挂载）任务数量
                if(tMissionOptionCount > 0)
                {
                    //	判断当前 Npc 绑定（挂载）任务数量（Npc未挂载功能选项）
                    if(tMissionOptionCount == 1)
                    {
                        //	获取列表第一个任务 和 任务当前目标提交项
                        MissionOption tMissionOption = missonOptionList[0];
                        Mission tMission = tMissionOption.mission;
                        SubmitDto tSubmitDto = MissionHelper.GetSubmitDtoByMission(tMission);

                        //	判断当前任务是否已接
                        if(tMissionOption.isExis)
                        {
                            //	提交时确认选项（一定存在一级选项的）（这里策划想不起来submitConfirm是干嘛的了）
                            if(tMission.missionType.submitConfirm)
                            {
                                _dialogueOption = DialogueOption.OnlyMainMission;
                            }
                            else
                            {
                                if(tSubmitDto != null && tSubmitDto.confirm)
                                {
                                    //	当前只有一个任务且任务类型是 冥雷战斗 \ 寻物品 \ 寻宠 任务
                                    if(MissionHelper.IsShowMonster(tSubmitDto))
                                    {
                                        _dialogueOption = DialogueOption.OnlySubMission;
                                    }
                                    else
                                    {
                                        //	当前只有一个任务 没有Npc功能选项
                                        _dialogueOption = DialogueOption.OnlyMainMission;
                                    }
                                }
                                else
                                {
                                    _dialogueOption = DialogueOption.OnlySubMission;
                                }
                            }
                        }
                        else
                        {
                            //	当前只有一个任务 没有Npc功能选项
                            _dialogueOption = DialogueOption.OnlyMainMission;
                        }
                    }
                    else
                    {
                        //	当前有多个任务 没有Npc功能选项
                        _dialogueOption = DialogueOption.OnlyMainMission;
                    }
                }
                else
                {
                    //	当执行到这里表示没有菜单选项 不做处理
                    GameDebuger.YellowDebugLog(string.Format("{0}\n{1}","当执行到这里表示没有菜单选项 不做处理","1）NPC没有功能选项 2）该时刻没有对应NPC任务绑定"));
                    _dialogueOption = DialogueOption.NothingOption;
                }
            }
            return _dialogueOption;
        }
        #endregion

        #region 判断接受任务前置任务编号（传入当前可接任务前置任务编号）
        private bool IsPreMissionByPreID(int preID)
        {
            //	遍历当前已提交任务编号，当可接任务前置任务编号在列表中存在（改前置任务已提交完成），则表明该可接任务可以领取。
            if(_playerMissionListDto != null)
            {
                List<int> tPreIDList=GetPreMissionIDList();
                for(int i = 0, len = tPreIDList.Count;i < len;i++)
                {
                    if(preID == tPreIDList[i])
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public List<int> GetPreMissionIDList()
        {
            if(_playerMissionListDto != null)
            {
                return _playerMissionListDto.submitMissionIds;
            }
            return new List<int>();
        }
        #endregion

        #region 是否玩家对话任务 && 非常驻NPC
        private bool IsDynamicDialogMonster(Mission missiom,SubmitDto submitDto)
        {
            Npc npc = GetMissionNpcByMission(missiom, true);
            return submitDto is TalkSubmitDto && npc != null && (npc.type == 2 || npc.type == 4 || npc.type == 5 || npc.type == 6 || npc.type == 7);
        }
        #endregion

        #region 新任务系统 任务完成处理函数
        /// <summary>
        /// 提交任务
        /// </summary>
        /// <param name="missionId">int</param>
        public void SubmitMission(int missionId) {
            Action<int> submitAction = (mId) =>
            {
                GameUtil.GeneralReq(Services.Mission_Submit(missionId),(e) =>
                {
                    RemovePlayerMissionWhenSubmit(mId);
                    FireData();
                });
            };
            if(GamePlotManager.Instance.TriggerPlot((int)Plot.PlotTriggerType.TalkToMissionSubmitNpc,missionId)) {
                // 这个监听在 GamePlotManager.FinishPlot 清空了
                GamePlotManager.Instance.OnFinishPlot += (plot) =>
                {
                    submitAction(missionId);
                };
            }
            else {
                submitAction(missionId);
            }
        }
        #endregion

        //===========================================玩家移动到任务相关地点====================================================
        public void FindToMissionNpcByMission(Mission mission,bool isExistState,bool showTips = true)
        {
            if (!MissionHelper.DoMissionSubmitTypeAction(mission)) return;  //做等级限制任务（后续或还有其他）
            if(mission == null)
            {
                GameDebuger.LogError("当前任务为空");
                return;
            }
            if (!TeamDataMgr.DataMgr.IsLeader())
            {
                if (TeamDataMgr.DataMgr.HasTeam())
                {
                    var val = WorldManager.Instance.GetModel().GetPlayerDto(ModelManager.Player.GetPlayerId());
                    if (val != null)
                    {
                        if (val.teamStatus != (int)TeamMemberDto.TeamMemberStatus.Away)
                        {
                            if (mission.type != (int)MissionType.MissionTypeEnum.Ghost)
                            {
                                TipManager.AddTip("正在组队跟随状态中，不能进行此操作！");
                            }
                            return;
                        }
                    }
                }
            }

            _lastFindMission = mission;
            //	已接任务判断是否是暗雷
            SubmitDto tSubmitDto = MissionHelper.GetSubmitDtoByMission(mission);
            //string tips = string.Empty;
            //if(tSubmitDto != null && !tSubmitDto.finish
            //    && tSubmitDto.dialog != null && !string.IsNullOrEmpty(tSubmitDto.dialog.tips))
            //{
            //    tips = tSubmitDto.dialog.tips;
            //    TipManager.AddTip(tips);
            //}
            GameDebuger.Log("isExistState:"+ isExistState+"```````````````" + MissionHelper.IsHiddenMonster(tSubmitDto));
            if(isExistState && MissionHelper.IsHiddenMonster(tSubmitDto))
            {
                HiddenMonsterSubmitDto tHiddenMonsterSubmitDto = tSubmitDto as HiddenMonsterSubmitDto;
                int tSceneID = MissionHelper.GetCurrentSceneID();
                if(tHiddenMonsterSubmitDto.acceptScene.id == tSceneID)
                {
                    ModelManager.Player.StartAutoFram();
                }
                else
                {
                    //自动跳地图
                    WorldManager.Instance.Enter(tHiddenMonsterSubmitDto.acceptScene.id,false,true);
                }
            }else
            {
                if(GetSubmitDto(mission) != null)
                {
                    mMissionShopItemMarkModel.SetShopItemMark(GetSubmitDto(mission));
                }
                Npc tFindToTargetNpc = GetMissionNpcByMission(mission, isExistState);
                PlayerCopyMissionDto tPlayerMissionDto = GetPlayerMission(mission) as PlayerCopyMissionDto;
                //如果他是副本任务，并且是副本外面，那样要通过NPC进入副本
                if(tPlayerMissionDto != null)
                {
                    if(ModelManager.Player.SceneID != tPlayerMissionDto.sceneId)
                    {
                        tFindToTargetNpc = mission.acceptNpc;
                    }
                    else {
                        tFindToTargetNpc = GetMissionNpcByMission(mission,isExistState);
                        //	当改NPC是虚拟NPC是转换为具体NPC
                        tFindToTargetNpc = MissionHelper.NpcVirturlToEntity(tFindToTargetNpc);
                    }
                }
                else {
                    tFindToTargetNpc = GetMissionNpcByMission(mission, isExistState);
                    //	当改NPC是虚拟NPC是转换为具体NPC
                    tFindToTargetNpc = MissionHelper.NpcVirturlToEntity(tFindToTargetNpc);
                }
                //	手动提交任务
                if(tFindToTargetNpc == null)
                {
                    if(isExistState)
                    {
                        if(tSubmitDto == null)
                        {
                            MissionDataMgr.MissionNetMsg.SubmitMission(mission.id);
                        }
                        else
                        {
                            MissionDataMgr.MissionNetMsg.FinishMission(mission);
                        }
                    }
                }
                //mMissionApplyItemSubmitDtoDelegate.HeroCharacterControllerEnable(false,0);
                WorldManager.Instance.FlyToByNpc(tFindToTargetNpc, 0, () =>
                  {
                      mMissionApplyItemSubmitDtoDelegate.WalkEndToFinishSubmitDto(mission, true, tFindToTargetNpc);
                  });
            }
        }

        #region 获取明雷NPC
        /// <summary>
        /// 获取明雷NPC -- Gets the npc show monster by submit dto.
        /// </summary>
        /// <param name="submitDto">提价任务信息，里面有明雷NPC的信息</param>
        /// <param name="sceneID">实例化的场景</param>
        /// <returns></returns>
        public NpcInfoDto GetNpcInfoDtoShowMonsterBySubmitDto(SubmitDto submitDto,int sceneID)
        {
            if(!MissionHelper.IsShowMonster(submitDto))
                return null;
            NpcInfoDto tNpcInfoDto=null;
            ShowMonsterSubmitDto tShowMonsterSubmitDto=submitDto as ShowMonsterSubmitDto;
            if(sceneID == tShowMonsterSubmitDto.acceptNpc.sceneId)
            {
                if(tShowMonsterSubmitDto.acceptNpc.npc != null && submitDto.count < submitDto.needCount)
                {
                    tNpcInfoDto = tShowMonsterSubmitDto.acceptNpc;
                }
            }

            //当改NPC是虚拟NPC是转换为具体NPC
            if(tNpcInfoDto!=null&&tNpcInfoDto.npc!=null&&tNpcInfoDto.npc is FactionNpc)
            {
                tNpcInfoDto.npc = NpcVirturlToEntity(tNpcInfoDto.npc);
            }
            return tNpcInfoDto;
        }
        #endregion

        #region 获取当前场景ID中存在的明雷怪物Npc
        public List<NpcInfoDto> GetShowMonsterNpcInfoDtoList()
        {
            //这个是普通任务走的明雷怪,任务NPC列表 
            List<NpcInfoDto> tNpcInfoDtoList=new List<NpcInfoDto>();
            //这个是单独为采集物做的列表，主要是因为任务列表没有唯一的ID，服务端同事在外围特殊做了一个唯一ID，所以需要特许处理
            Dictionary<long,NpcInfoDto> tColleInfoDtoListDic = new Dictionary<long, NpcInfoDto>();
            //可接任务
            List<Mission> tGetMissedSubMissionMenuList = GetSubdivisionMenuList();
            //获取当前任务列表
            List<Mission> tGetExistSubMissionMenuList = GetExistSubMissionMenuList();
            List<MissionOption> tCurMissionMenuList = new List<MissionOption>();
            for(int i = 0;i<(tGetExistSubMissionMenuList.Count + tGetMissedSubMissionMenuList.Count);i++) {
                Mission tMission=i<tGetExistSubMissionMenuList.Count?tGetExistSubMissionMenuList[i]:tGetMissedSubMissionMenuList[i-tGetExistSubMissionMenuList.Count];
                bool tIsExist= i < tGetExistSubMissionMenuList.Count;
                MissionOption tMissionOption = new MissionOption(tMission, tIsExist);
                tCurMissionMenuList.Add(tMissionOption);
            }
            int sceneID=MissionHelper.GetCurrentSceneID();
            for(int i = 0, len = tCurMissionMenuList.Count;i < len;i++)
            {
                Mission tMission=tCurMissionMenuList[i].mission;
                List<SubmitDto> tSubmitDtoList = MissionHelper.GetSubmitDtoListByMission(tMission);
                //当前为 明雷 \ 对话 任务类型110
                NpcInfoDto tNpcInfoDto=null;
                for(int index=0;index<tSubmitDtoList.Count;index++)
                {
                    SubmitDto tSubmitDto=tSubmitDtoList[index];
                    if(tSubmitDto.finish)
                        continue;
                    if(MissionHelper.IsShowMonster(tSubmitDto))
                    {
                        tNpcInfoDto = GetNpcInfoDtoShowMonsterBySubmitDto(tSubmitDto,sceneID);
                    }
                    else if(MissionHelper.IsTalkItem(tSubmitDto))
                    {
                        tNpcInfoDto = MissionHelper.GetNpcTalkBySubmitDto(tSubmitDto,sceneID);
                    }
                    else if(MissionHelper.IsCollectionItem(tSubmitDto))
                    {
                        //tNpcInfoDto = GetCollectionItemBySubmitDto(tSubmitDto,sceneID);
                    }
                    if(tNpcInfoDto != null && MissionHelper.IsMonsterNpc(tNpcInfoDto.npc))
                    {
                        tNpcInfoDtoList.Add(tNpcInfoDto);
                    }
                    //生成采集物
                    if(MissionHelper.IsCollection(tSubmitDto)) {
                        tColleInfoDtoListDic = GetCollectionInfoShowSubmitDto(tSubmitDto,sceneID);
                        if(tColleInfoDtoListDic.Count > 0)
                        {
                            tColleInfoDtoListDic.ForEach(e =>
                            {
                                WorldManager.Instance.GetNpcViewManager().AddNpcUnit(e.Key,e.Value,tCurMissionMenuList[i].mission);
                            });
                        }
                    }
                }

                if(tNpcInfoDtoList.Count == 0 && !tMission.autoSubmit && tMission.submitNpc != null && tMission.submitNpc.sceneId == sceneID)
                {
                    tNpcInfoDto = GetNpcInfoDtoByNpc(tMission.submitNpc);
                    if(tNpcInfoDto != null && MissionHelper.IsMonsterNpc(tNpcInfoDto.npc))
                    {
                        tNpcInfoDtoList.Add(tNpcInfoDto);
                    }
                }

                //生成接受任务NPC
                if(tMission.acceptNpc != null && tMission.acceptNpc.sceneId == sceneID)
                {
                    tNpcInfoDtoList.Add(GetNpcInfoDtoByNpc(tMission.acceptNpc));
                }
                if(tCurMissionMenuList[i].isExis) {
                    WorldManager.Instance.GetNpcViewManager().HandlerMissionNpcStatus(tMission.acceptNpcStatus);
                }
            }
            return tNpcInfoDtoList;
        }
        #endregion

        #region 主界面跟踪面板具体任务信息
        public IEnumerable<object> GetMissionCellData()
        {
            List<object> tDataList = new List<object>();
            List<Mission> tCurMisisonMenuList = GetSubMissionMenuListInMainUIExpand();
            for (int i = 0, max = tCurMisisonMenuList.Count; i < max; i++)
            {
                Mission tMission = tCurMisisonMenuList[i];
                tDataList.Add(tMission);
            }
            return tDataList;
        }
        #endregion

        #region 获取生成采集物列表
        public Dictionary<long,NpcInfoDto> GetCollectionInfoShowSubmitDto(SubmitDto submitDto,int sceneID)
        {
            if(!MissionHelper.IsCollection(submitDto))
                return null;
            Dictionary<long,NpcInfoDto> tNpcInfoDto=new Dictionary<long,NpcInfoDto>();
            PickItemSubmitInfoDto tPickItemPointDto = submitDto as PickItemSubmitInfoDto;
            for(int i=0;i< tPickItemPointDto.pickNpcs.Count;i++)
            {
                if(sceneID == tPickItemPointDto.pickNpcs[i].npcInfoDto.sceneId && tPickItemPointDto.submitNpc != null && !tPickItemPointDto.pickNpcs[i].pick)
                {
                    tNpcInfoDto.Add(tPickItemPointDto.pickNpcs[i].id,tPickItemPointDto.pickNpcs[i].npcInfoDto);
                }
            }
            return tNpcInfoDto;
        }
        #endregion



        #region 接受任务状态通知更新
        /// <summary>
        ///  接受任务状态通知更新 -- Updatas the player mission notify.
        /// </summary>
        /// <param name="notify">>Notify.</param>
        public void UpdataPlayerMissionNotify(PlayerMissionNotify notify)
        {
            GameDebuger.Log(string.Format("======  服务端下发 接受任务 状态变更通知  =====\nMissionID：{0}\nMissionType:{1}\nName{2}-{3}",
                                               notify.playerMissionDto.missionId,
                                               notify.playerMissionDto.GetType().Name,
                                               notify.playerMissionDto.mission.missionType.name,notify.playerMissionDto.mission.name));
            PlayerGhostMissionDto tPlayerGhostMissionDto = notify.playerMissionDto as PlayerGhostMissionDto;
            if(tPlayerGhostMissionDto != null) {
                mGhostRingCount = tPlayerGhostMissionDto.ghostRingCount;
            }
            AcceptMission(notify.playerMissionDto);
        }
        #endregion

        #region 更新任务提交状态
        public void UpdateSubmitDto(MissionSubmitStateNotify notify) {
            PlayerMissionDto missionDto = GetPlayerMissionById(notify.missionId);
            if(missionDto == null) {
                GameDebuger.LogError("MissionSubmitStateNotify 中的任务没有在 PlayerMissionListDto.bodyMissions 内, missionId = " + notify.missionId);
                return;
            }
            SubmitDto submitDto = null;
            if(missionDto.completions != null && missionDto.completions.Count > 0) {
                submitDto = missionDto.completions.Find(submit => submit.id == notify.submitConditionId && submit.index == notify.index);
            }
            //	接受到一个玩家没有的任务通知
            if(submitDto == null) {
                GameDebuger.LogError(string.Format("MissionSubmitStateNotify 中的提交条件不在 PlayerMissionDto.completions 中,submitConditionId={0},battleIndex={1}",notify.submitConditionId,notify.index));
                return;
            }
            ISubmitDelegate submitDelegate = _submitDelegateFactory.GetHandleSubmitDelegate(submitDto.GetType().Name);
            // 注意这里不能提前修改 SubmitDto 的数据
            BattleDelayHandle(() => {
                UpdateSubmitDtoByStateNotify(notify,missionDto,submitDto);
            },submitDelegate.IsBattleDelay(submitDto,notify));

        }
        #endregion

        #region 更新任务列表数据
        private void UpdateSubmitDtoByStateNotify(MissionSubmitStateNotify notify,PlayerMissionDto missionDto,SubmitDto submitDto) {
            Mission mission = notify.mission;
            var heroView = WorldManager.Instance.GetHeroView();
            if(heroView != null)
                heroView.checkMissioinCallback = null;

            submitDto.count = notify.count;
            submitDto.finish = notify.finish;

            //	状态变更
            bool isSubmit = notify.finish && mission.autoSubmit && GetSubmitDto(mission) == null;
            missionDto.status = isSubmit ? (int)PlayerMissionDto.PlayerMissionStatus.Submitable : (int)PlayerMissionDto.PlayerMissionStatus.Progress;

            //采集任务特殊处理
            if(notify is PickItemStateNotify)
            {
                PickItemStateNotify  pisn = notify as PickItemStateNotify;
                PickItemSubmitInfoDto  piPDto = submitDto as PickItemSubmitInfoDto;
                for(int count = 0;count < piPDto.pickNpcs.Count;count++)
                {
                    piPDto.pickNpcs[count].pick = pisn.pick[count];
                }
            }
            //------------------------------------------------------------------------------
            // 1.根据 MissionSubmitStateNotify 修改 SubmitDto 并对此进行处理
            // SubmitDto 状态变化主要有:
            // I.   count++ and count < needCount
            // II.  count++ and count == needCount and finish == false
            // III. count++ and count == needCount and finish == true
            ISubmitDelegate submitDelegate = _submitDelegateFactory.GetHandleSubmitDelegate(submitDto.GetType().Name);
            if(submitDto.count < submitDto.needCount && !notify.finish)
            {
                submitDelegate.SubmitConditionUpdate(mission,submitDto);
            }
            else if(submitDto.count >= submitDto.needCount && !notify.finish)
            {
                submitDelegate.SubmitConditionReach(mission,submitDto);
            }
            else if(submitDto.count >= submitDto.needCount && notify.finish) {
                submitDelegate.SubmitConditionFinish(mission,submitDto);
            }
            if(isSubmit)
            {
                // SubmitDto 的处理/清理已经在上面处理,所以这里只需要删除Mission表生成的动态数据
                RemovePlayerMissionWhenSubmit(notify.mission.id,submitDto);
            }
            else {
                WorldManager.Instance.GetNpcViewManager().RefinishMissionFlag();
            }
            //下面关于NPC商店，暂时忽略
            //END
            IMissionDelegate missionDelegate = _missionDelegateFactory.GetHandleMissionDelegate(mission.type);
            missionDelegate.UpdateSubmitDtoByStateNotify(notify,submitDto);
            FireData();
        }
        #endregion

        #region 生成提交任务的NPC
        public void AddSubmitableNPC(SubmitDto pSubmitDto,Mission pMission,bool pFinish,int pSceneID)
        {
#if UNITY_EDITOR
            if(pMission.autoSubmit == false && pMission.submitNpc == null)
            {
                GameDebuger.Log("任务id: " + pMission.id + " 名字:" + pMission.name + "autoSubmit 字段需要设置为 true");
            }
#endif
            bool tIsMissionNpcInit = pSubmitDto == null && !pMission.autoSubmit && pMission.submitNpc != null && pMission.submitNpc.sceneId == pSceneID;
            NpcInfoDto tNpcInfoDtoInit = tIsMissionNpcInit ? DataMgr._data.GetNpcInfoDtoByNpc(pMission.submitNpc)
            : pFinish ? null
            : MissionHelper.IsShowMonster(pSubmitDto) ? DataMgr._data.GetNpcInfoDtoShowMonsterBySubmitDto(pSubmitDto, pSceneID)
            : MissionHelper.IsTalkItem(pSubmitDto) ?  MissionHelper.GetNpcTalkBySubmitDto(pSubmitDto, pSceneID) : null;
            //: MissionHelper.IsCollectionItem(pSubmitDto) ? MissionNpcModel.GetCollectionItemBySubmitDto(pSubmitDto, pSceneID)
            // : MissionHelper.IsCollectionItemCategory(pSubmitDto) ? MissionNpcModel.GetCollectionItemCategorySubmitDto(pSubmitDto, pSceneID) : null;
            if(tNpcInfoDtoInit != null && tNpcInfoDtoInit.npc != null && MissionHelper.IsMonsterNpc(tNpcInfoDtoInit.npc))
            {
                WorldManager.Instance.GetNpcViewManager().AddNpcUnit(tNpcInfoDtoInit);
            }
        }
        #endregion

        #region 对话后意图Finish任务，有可能提交该任务
        /// <summary>
        /// 对话后意图Finish任务，有可能提交该任务 (return true:关闭对话， false:不需要关闭对话) -- Finishs the target submit dto.
        /// </summary>
        /// <returns><c>true</c>, if target submit dto was finished, <c>false</c> otherwise.</returns>
        /// <param name="mission">Mission.</param>
        /// <param name="npc">Npc.</param>
        public void FinishTargetSubmitDto(Mission mission,Npc npc,int submitIndex,int battleIndex = 0)
        {
            //bool tCloseNpcDialogueSta = true;
            //	正在跳转场景的不进行请求
            if(WorldManager.Instance.isEntering)
            {
                return;
            }

            SubmitDto submitDto = GetSubmitDto(mission);
            if(submitDto == null) {
                // 所有条件完成,表示该任务可提交（非目标提交）
                SubmitMission(mission.id);
                return;
            }

            ISubmitDelegate submitDelegate = _submitDelegateFactory.GetHandleSubmitDelegate(submitDto.GetType().Name);
            submitDelegate.FinishSubmitDto(mission,submitDto,npc,battleIndex);
        }
        #endregion

        #region 宝图任务
        //private List<BagItemDto> mBagtoList;
        //private BagItemDto mCurBagItem;
        public void TreasureMission(IEnumerable<BagItemDto> bagtoList,BagItemDto CurBagItem)
        {
            TreasureItemUseLogic tTreasureItemUseLogic = new TreasureItemUseLogic(this);
            tTreasureItemUseLogic.usePropByPos(bagtoList,CurBagItem);
   
        }
        #endregion

        #region 获得某个类型的任务状态
        public MissionStatDto GetMissionStatDto(int missiontype) {
            MissionStatDto tMissionStatDto = null;
            List<MissionStatDto> tMissionStatDtoList = _playerMissionListDto.missionStat;
            for(int i = 0, len = tMissionStatDtoList.Count;i < len;i++)
            {
                if(tMissionStatDtoList[i].missionType == missiontype)
                {
                    tMissionStatDto = tMissionStatDtoList[i];
                    break;
                }
            }
            return tMissionStatDto;
        }
        #endregion

        #region 任务数据辅助函数
        public PlayerMissionDto GetPlayerMission(Mission mission) {
            if(_playerMissionListDto == null
                || _playerMissionListDto.bodyMissions == null
                || _playerMissionListDto.bodyMissions.Count == 0)
                return null;
            if(mission.id > 0)
                return _playerMissionListDto.bodyMissions.Find((m) => m.missionId == mission.id);
            else
                return _playerMissionListDto.bodyMissions.Find((m) => m.mission.type == mission.type);
        }

        /// <summary>
        /// 注意潜规则 missionId 小于 0 的时候用 mission.type 来判断
        /// 是为了兼容 CreateNewMissionByTypeId 里面客户端自己生成的虚拟任务设计的
        /// </summary>
        /// <param name="missionId"></param>
        /// <returns></returns>
        public PlayerMissionDto GetPlayerMissionById(int missionId) {
            if(_playerMissionListDto == null
                || _playerMissionListDto.bodyMissions == null
                || _playerMissionListDto.bodyMissions.Count == 0)
                return null;
            if(missionId > 0)
                return _playerMissionListDto.bodyMissions.Find((m) => m.missionId == missionId);
            else
                return _playerMissionListDto.bodyMissions.Find((m) => m.mission.type == -missionId);
        }

        public PlayerMissionDto GetPlayerMissionByType(int missionType) {
            if(_playerMissionListDto == null
                || _playerMissionListDto.bodyMissions == null
                || _playerMissionListDto.bodyMissions.Count == 0)
                return null;

            return _playerMissionListDto.bodyMissions.Find((m) => m.mission.type == missionType);
        }

        public SubmitDto GetSubmitDto(Mission mission,int submitIndex = -1) {
            return GetSubmitDto(mission.id,submitIndex);
        }

        public SubmitDto GetSubmitDto(int missionId,int submitIndex = -1) {
            PlayerMissionDto dto = GetPlayerMissionById(missionId);
            return GetSubmitDto(dto,submitIndex);
        }

        public SubmitDto GetSubmitDto(PlayerMissionDto dto,int submitIndex = -1) {
            if(dto == null || dto.completions == null || dto.completions.Count == 0)
                return null;
            //以后多个提交任务都是特殊设置一下，因为服务器不会把每个提交条件设为true
            if(MissionHelper.IsFindItem(dto.completions[0]))
            {
                //多个任务做完才可以为null，为空就可以完成任务
                return dto.completions.Find((s) => s.count < s.needCount);
            }
            else {
                if(submitIndex == -1)
                    return dto.completions.Find((s) => !s.finish);
                else
                    return dto.completions.Find((s) => s.index == submitIndex && !s.finish);
            }
        }
        #endregion

        #region 处理场景NPC相关逻辑
        public void AddMissionNpc(Mission mission) {
            PlayerMissionDto dto = GetPlayerMission(mission);
            int sceneId = WorldManager.Instance.GetModel().GetSceneId();
            if(mission.acceptNpc != null
                && mission.acceptNpc.sceneId == sceneId
                && mission.acceptNpc is NpcMonster){
                WorldManager.Instance.GetNpcViewManager().AddNpcUnit(MissionHelper.GetNpcInfoDtoByNpc(mission.acceptNpc));
            }
            WorldManager.Instance.GetNpcViewManager().HandlerMissionNpcStatus(mission.acceptNpcStatus);
            bool isAddNpc = false;
            if(dto != null) {
                isAddNpc = AddCompletionsNpc(dto);
            }

            if(!isAddNpc && !mission.autoSubmit && mission.submitNpc != null
                && mission.submitNpc.sceneId == sceneId
                && mission.submitNpc is NpcMonster
                )
            {
                WorldManager.Instance.GetNpcViewManager().AddNpcUnit(MissionHelper.GetNpcInfoDtoByNpc(mission.submitNpc));
            }

        }

        private bool AddCompletionsNpc(PlayerMissionDto dto) {
            bool isAddNpc = false;
            if(dto.completions != null && dto.completions.Count > 0) {
                for(int i = 0;i < dto.completions.Count;i++) {
                    SubmitDto submitDto = dto.completions[i];
                    NpcInfoDto npcInfoDto = _submitDelegateFactory.GetHandleSubmitDelegate(submitDto.GetType().Name).GetMissionNpcInfo(submitDto,submitDto.count >= submitDto.needCount);
                    npcInfoDto = BaseSubmitDelegate.GetNpc(npcInfoDto);
                    if(npcInfoDto != null && npcInfoDto.id > 0 && npcInfoDto.npc != null) {
                        if(npcInfoDto.npc.type == (int)Npc.NpcType.PickPoint)
                        {
                            PickItemSubmitInfoDto tPickItemPointDto = submitDto as PickItemSubmitInfoDto;
                            tPickItemPointDto.pickNpcs.ForEach(e =>
                            {
                                if(MissionHelper.GetCurrentSceneID() == tPickItemPointDto.pickNpcs[i].npcInfoDto.sceneId && tPickItemPointDto.submitNpc != null && !tPickItemPointDto.pickNpcs[i].pick) {
                                    WorldManager.Instance.GetNpcViewManager().AddNpcUnit(e.id, e.npcInfoDto,dto.mission);
                                }
                            });
                        }
                        else {
                            WorldManager.Instance.GetNpcViewManager().AddNpcUnit(npcInfoDto);
                        }
                        isAddNpc = true;
                    }
                }
            }
            return isAddNpc;
        }
        #endregion

        #region 任务寻路(新)
        /// <summary>
        /// 注意这个寻路逻辑用在 IMissionDelegate.AcceptMissionFinish 中,
        /// 但是是先执行 IMissionDelegate.AcceptMissionFinish,
        /// 然后再执行 NpcViewManager.OnMissionAccept 增加NPC到场景的,
        /// 所以是必现要延迟执行寻路
        /// </summary>
        /// <param name="mission"></param>
        public void WaitFindToMissionNpc(Mission mission) {
            if(!TeamDataMgr.DataMgr.IsLeader())
            {
                if(TeamDataMgr.DataMgr.HasTeam())
                {
                    var val = WorldManager.Instance.GetModel().GetPlayerDto(ModelManager.Player.GetPlayerId());
                    if(val != null)
                    {
                        if(val.teamStatus != (int)TeamMemberDto.TeamMemberStatus.Away)
                        {
                            if(mission.type != (int)MissionType.MissionTypeEnum.Ghost)
                            {
                                //TipManager.AddTip("正在组队跟随状态中，不能进行此操作！");
                            }
                            return;
                        }
                    }
                }
            }

            JSTimer.Instance.SetupCoolDown("__waitTimeForNextMission",1f,null,() => {
                FindToMissionNpc(mission,false);
            });
        }

        /// <summary>
        /// 当玩家在提交收集类任务时候,如果玩家身上有多个可以提交物品
        /// 提交时会有 MissionSubmitStateNotify.finish = false 下发,触发 ISubmitDelegate.SubmitConditionReach 里面的寻路逻辑
        /// 然后立刻下发 MissionSubmitStateNotify.finish = true,触发 ISubmitDelegate.SubmitConditionFinish
        /// 所以在 ISubmitDelegate.SubmitConditionFinish 增加一个终止寻路的方法调用
        /// </summary>
        public void StopFindToMissionNpc() {
            JSTimer.Instance.CancelCd("__waitTimeForNextMission");
            ModelManager.Player.StopAutoNav();
        }

        public void FindToMissionNpc(int missionTypeId) {
            PlayerMissionDto missionDto = GetPlayerMissionByType(missionTypeId);
            if(missionDto == null)
                FindToMissionNpc(MissionHelper.CreateNewMissionByTypeId(missionTypeId));
            else
                FindToMissionNpc(missionDto.mission);
        }

        public void FindToMissionNpc(Mission mission,bool showTips = true) {
            if(mission == null) {
                GameDebuger.LogError("当前任务空,无法寻路");
                return;
            }

            if(BattleDataManager.DataMgr.IsInBattle)
            {
                TipManager.AddTip("战斗中，不能进行传送");
                return;
            }

            //	当拥有队伍，且不是队长，且在队伍中
            //if(showTips)
            //{
            //    TipManager.AddTip("正在组队跟随状态中，不能进行此操作！");
            //    return;
            //}
            // 判断任务是否存在,存在就正常寻路,不存在就去领取任务npc 
            if(GetPlayerMission(mission) != null) {
                _lastFindMission = mission;
                IMissionDelegate missionDelegate = _missionDelegateFactory.GetHandleMissionDelegate(mission.type);
                // 这里有个鬼畜的处理顺序需求,现在定义处理顺序为:
                // DefaultMissionDelegate > submitDelegate > other missionDelegate
                if(missionDelegate is DefaultMissionDelegate && missionDelegate.FindToNpc(mission))
                    return;
                SubmitDto submitDto = GetSubmitDto(mission);
                if(submitDto != null) {
                    // 一个特殊的提示处理....
                    //if(submitDto.dialog != null && !string.IsNullOrEmpty(submitDto.dialog.tips))
                    //{
                    //    TipManager.AddTip(submitDto.dialog.tips);
                    //}
                    ISubmitDelegate submitDelegate = _submitDelegateFactory.GetHandleSubmitDelegate(submitDto.GetType().Name);
                    if(submitDelegate.FindToNpc(mission,submitDto))
                        return;
                }

                if(missionDelegate.FindToNpc(mission))
                    return;

                Npc targetNpc = GetMissionNpc(mission);

                //	手动提交任务
                if(targetNpc == null)
                {
                    if(submitDto == null)
                        SubmitMission(mission.id);
                    else
                        MissionNetMsg.FinishMission(mission);
                }
                else {
                    if(submitDto != null) {
                        mMissionShopItemMarkModel.SetShopItemMark(submitDto);
                    }
                    if(WorldManager.Instance.GetHeroView() != null)
                    {
                        // 这个其实挺诡异的
                        WorldManager.Instance.GetHeroView().toTargetCallback = null;
                    }
                    WorldManager.Instance.FlyToByNpc(targetNpc,0,() =>
                    {
                        // 玩家移动到任务相关地点,如果是 ApplyItemSubmitDto 执行物品弹出框
                        mMissionApplyItemSubmitDtoDelegate.WalkEndToFinishSubmitDto(mission,true,targetNpc);
                    });
                }
            }
        }
        #endregion

        #region 提取任务相关npc进行寻路
        /// <summary>
        /// 获得任务完成条件相关的 NPC
        /// </summary>
        /// <param name="mission"></param>
        /// <param name="submitIndex"></param>
        /// <param name="getSubmitNpc"></param>
        /// <returns></returns>
        public NpcInfoDto GetCompletionConditionNpc(Mission mission,int submitIndex = -1,bool getSubmitNpc = false) {
            SubmitDto submitDto =GetSubmitDto(mission,submitIndex);
            NpcInfoDto tNpcInfoDto = null;
            if(submitDto == null)
            {
                Npc tNpc = mission.submitNpc;
                if(tNpc != null)
                {
                    //	当改NPC是虚拟NPC是转换为具体NPC
                    tNpc = MissionHelper.NpcVirturlToEntity(mission.submitNpc);
                    tNpcInfoDto = MissionHelper.GetNpcInfoDtoByNpc(tNpc);
                }
            }
            else {
                tNpcInfoDto = GetCompletionConditionNpc(submitDto,getSubmitNpc);
            }
            return tNpcInfoDto;
        }

        /// <summary>
        /// 获得任务完成条件相关的 NPC
        /// </summary>
        /// <param name="submitDto"></param>
        /// <param name="getSubmitNpc"></param>
        /// <returns></returns>
        public NpcInfoDto GetCompletionConditionNpc(SubmitDto submitDto,bool getSubmitNpc = false) {
            ISubmitDelegate submitDelegate = _submitDelegateFactory.GetHandleSubmitDelegate(submitDto.GetType().Name);
            return submitDelegate.GetMissionNpcInfo(submitDto,getSubmitNpc);
        }

        public Npc GetMissionNpc(Mission mission) {
            Npc npc = null;
            PlayerMissionDto dto = GetPlayerMission(mission);
            if(dto != null)
            {
                NpcInfoDto tNpcInfoDto = GetCompletionConditionNpc(mission);
                npc = MissionHelper.GetNpcByNpcInfoDto(tNpcInfoDto);
            }
            else {
                npc = mission.missionType.acceptNpc;
                if(npc == null)
                    npc = mission.acceptNpc;
            }
            //	当改NPC是虚拟NPC是转换为具体NPC
            npc = MissionHelper.NpcVirturlToEntity(npc);
            return npc;
        }

        public Mission GetLastFindMission() {
            return _lastFindMission;
        }

        public bool IsLastFindMission(Mission mission) {
            return _lastFindMission == null || _lastFindMission.id == mission.id;
        }

        /// <summary>
        /// 清空任务
        /// </summary>
        public void ClearLastFindMission()
        {
            _lastFindMission = null;
        }



        /// <summary>
        /// 获得所有可接任务，任务类型不能相同
        /// </summary>
        /// <returns></returns>
        List<Mission> GetSubdivisionMenuList() {
            List<Mission> subMissionMenuList = GetMissedSubMissionMenuList();
            List<Mission> tMainMissionMenuList = new List<Mission>();
            for(int i = 0, max = subMissionMenuList.Count;i < max;i++)
            {
                Mission tMission = subMissionMenuList[i];
                if(tMission.missionType == null)
                {
                    GameDebuger.LogError(string.Format("数据错误 -> ID: {0} | Name: {1}",tMission.name,tMission.id));
                    return null;
                }

                //当前任务类型是否已存在列表中
                bool tExitsInList = false;
                for(int j = 0, len = tMainMissionMenuList.Count;j < len;j++)
                {
                    if(MissionHelper.IsMainOrExtension(tMission))
                    {
                        if(tMission.type == tMainMissionMenuList[j].id)
                        {
                            tExitsInList = true;
                            break;
                        }
                    }
                    else if(tMission.type >= (int)MissionType.MissionTypeEnum.Faction)
                    {
                        //日常
                        if(tMainMissionMenuList[j].id >= (int)MissionType.MissionTypeEnum.Faction)
                        {
                            tExitsInList = true;
                            break;
                        }
                    }
                    else
                    {
                        GameDebuger.OrangeDebugLog(String.Format("ERROR：未分大类任务 ID：{0} | Name：{1}-{2}",
                                                   tMission.id,tMission.missionType.name,tMission.name));
                        break;
                    }
                }

                if(!tExitsInList)
                {
                    tMainMissionMenuList.Add(tMission);
                }
            }
            return tMainMissionMenuList;
        }
        #endregion

        #region 获得当前任务需要取得商店的数据
        public MissionShopItemMarkModel GetMissionShopItem() {
            return mMissionShopItemMarkModel;
        }
        #endregion
    }
}
