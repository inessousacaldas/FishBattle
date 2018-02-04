// **********************************************************************
// Copyright (c) 2013 Baoyugame. All rights reserved.
// File     :  PlayerModel.cs
// Author   : SK
// Created  : 2013/1/31
// Purpose  : 玩家数据模型
// **********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using AppDto;
using UniRx;
using VirtualItemEnum = AppDto.AppVirtualItem.VirtualItemEnum;

public class PlayerPropertyInfo
{

    /** 人物基础属性 */
    public CharactorDto playerDto;

    /** 装备资质属性 */
    public int[] EqAps;

    /** 坐骑资质属性 */
    public int[] RideAps;

    /** 怒气 */
    public int sp;

    public PlayerPropertyInfo(CharactorDto dto)
    {
        playerDto = dto;
        EqAps = new int[5];
        RideAps = new int[5];
    }

    public PlayerPropertyInfo(PlayerPropertyInfo playerInfo)
    {
        this.playerDto = new CharactorDto();
        this.EqAps = new int[5];
        this.RideAps = new int[5];
        GameDebuger.TODO(@"ResetPlayerInfo(playerInfo);");
    }

    /**public void ResetPlayerInfo(PlayerPropertyInfo playerInfo){
        this.playerDto.level = playerInfo.playerDto.level;
        this.playerDto.aptitudeProperties = playerInfo.playerDto.aptitudeProperties;
        this.playerDto.potential = playerInfo.playerDto.potential;
        this.playerDto.extraPotential = playerInfo.playerDto.extraPotential;

        playerInfo.EqAps.CopyTo(this.EqAps,0);
        playerInfo.RideAps.CopyTo(this.RideAps,0);
        this.playerDto.properties = playerInfo.playerDto.properties;
    }*/

    /**public int[] ToApInfoArray(){
        int[] apInfoArray = new int[5]{
            this.playerDto.aptitudeProperties.constitution,
            this.playerDto.aptitudeProperties.intelligent,
            this.playerDto.aptitudeProperties.strength,
            this.playerDto.aptitudeProperties.stamina,
            this.playerDto.aptitudeProperties.dexterity
        };

        return apInfoArray;
    }*/
}

public interface IPlayerModel
{
    long GetPlayerId();
    int FactionID { get; }
    int SceneID { get; }
    PlayerDto GetPlayer();
    long GetPlayerWealth(VirtualItemEnum id);
    string GetPropertyDesc(int propertyId, bool isPlayer);
}

public class PlayerModel : IModuleModel, IPlayerModel
{
    private PlayerDto _playerDto;
    private PlayerPropertyInfo _playerPropertyInfo;
    private MainCrewInfoNotify _crewInfoDto;
    private long _totalSpellExp = 0;
    private bool _dailyQuestion;
    private List<int> _scheduleCancelNotify = new List<int>();

    private static UniRx.Subject<IPlayerModel> stream = null;
    private static UniRx.Subject<int> lvUpStream = null;

    public static UniRx.IObservableExpand<IPlayerModel> Stream {
        get { 
            if (stream == null)
                stream = new Subject<IPlayerModel>();
            return stream; 
        }
    }

    public static UniRx.IObservableExpand<int> LvUpStream
    {
        get
        {
            if (lvUpStream == null)
                lvUpStream = new Subject<int>();
            return lvUpStream;
        }
    }

    private CompositeDisposable _disposible = null;
    //总修炼经验
    public int tempRideId = 0;

    public long TotalSpellExp
    {
        get { return _totalSpellExp; }
    }       

    public PlayerModel(){

    }

    public static PlayerModel Create()
    {
        var model = new PlayerModel();
        model.Init();
        return model;
    }

    public void Init()
    {
        if (stream == null)
            stream = new Subject<IPlayerModel>();
        stream.Hold(this);
        if (lvUpStream  == null)
        lvUpStream = new Subject<int>();
        lvUpStream.Hold(0);
        _disposible = new CompositeDisposable();
        _disposible.Add(TeamDataMgr.Stream.Subscribe(s => {
            if (s == null)
                return;
            if (s.LeaderID != ModelManager.IPlayer.GetPlayerId())
                StopAutoNav();
        }));

        _disposible.Add(NotifyListenerRegister.RegistListener<WealthNotify>(HandleWealthNotify));
        //角色经验变更
        _disposible.Add(NotifyListenerRegister.RegistListener<CharactorExpInfoNotify>(HandleCharactorExpInfoNotify));
    }

    private void HandleWealthNotify(WealthNotify noti)
    {
        if (noti != null && _playerDto != null)
        {
            noti.wealthItems.ForEach(s=>_playerDto.wealth.wealthItems.ReplaceOrAdd(item=>item.virtualItemId == s.virtualItemId, s));
            //noti.wealthItems.ForEach(s => _playerPropertyInfo.wealth.wealthItems.ReplaceOrAdd(item => item.virtualItemId == s.virtualItemId, s));
            stream.OnNext(this);
        }
    }


    #region ReserveExp

    public int ReserveExp
    {
        get
        {
            GameDebuger.TODO(@"return _playerDto.subWealth.reserveExp;");
            return 0;
        }
    }

    /**private ReserveExpDto _reserveExpDto = null;
    public ReserveExpDto ReserveExpDto{
        get{
            return _reserveExpDto;
        }
        set{
            _reserveExpDto = value;
        }
    }*/
    #endregion ReserveExp

	#region Vigour
	public bool isEnoughVigour(int needVigour, bool tip = false)
    {
        GameDebuger.TODO(@"if(_playerDto.subWealth.vigour >= needVigour){
            return true;
        }
        else{
            if (tip)
            {
                //TipManager.AddTip('活力不足');
                      ProxyManager.Tips.Open('增加活力', 10007, 1, (ingot) =>
                      {
                          //ModelManager.BackpackModel.UsePropByItemId(item.itemId);
                          ServiceRequestAction.requestServer(BackpackService.fastGainVigour());
                      });
            }
            return false;
        }");
        return true;
    }

    public int Vigour
    {
        get
        {
            GameDebuger.TODO(@"return _playerDto.subWealth.vigour;");
            return 0;
        }
        set
        {
            GameDebuger.TODO(@"_playerDto.subWealth.vigour = value;");
        }
    }

    public int VigourMax
    {
        get
        {
            return this.GetPlayerLevel() * 20 + 50;
        }
    }

    #endregion Vigour

    #region ServerGrade

    private GameServerGradeDto _serverGradeDto;
    public GameServerGradeDto GetServerGradeDto { get { return _serverGradeDto; } }
    private long _nextServerGradeOpenTime;

    public void UpdateServerGradeDto(GameServerGradeDto serverGradeDto)
    {
        if (serverGradeDto == null)
        {
            Debug.LogError("GameServerGradeDto is null");
            return;
        }

        _serverGradeDto = serverGradeDto;
        _nextServerGradeOpenTime = GetNextServerOpenTime(serverGradeDto);
    }

    public int ServerGrade
    {
        get
        {
            return _serverGradeDto.serverGrade;
        }
    }

    public long NextServerGradeOpenTime
    {
        get
        {
            return _nextServerGradeOpenTime;
        }
    }

    //返回-1代表已到达服务器最高等级上限，否则返回下次服务器等级开放时间
    private long GetNextServerOpenTime(GameServerGradeDto serverGradeDto)
    {
        List<GameServerGrade> serverGradeList = DataCache.getArrayByCls<GameServerGrade>();
        if (serverGradeList == null)
            return -1L;
		
        int nextGradeIndex = -1;
        for (int i = 0; i < serverGradeList.Count; ++i)
        {
            if (serverGradeList[i].grade == serverGradeDto.serverGrade)
            {
                nextGradeIndex = i + 1;
                break;
            }
        }

        if (nextGradeIndex != -1 && nextGradeIndex < serverGradeList.Count)
        {
            long daySpan = serverGradeList[nextGradeIndex].id;
            //注意这里的openTime 是服务器的开服时间
            return serverGradeDto.openTime + daySpan * 86400000L;
        }
        else
            return -1L;
    }


    //服务器等级上限
    public int GetServerGradeUpLimit()
    {
        List<GameServerGrade> serverGradeList = DataCache.getArrayByCls<GameServerGrade>();
        if (serverGradeList == null)
            return 0;
        for (int i = 0; i < serverGradeList.Count; ++i)
        {
            if (serverGradeList[i].max)
                return serverGradeList[i].grade;
        }

        return 0;
    }

    #endregion

    #region VIP

    public int GetSlotsElementLimit { get { return _playerPropertyInfo.playerDto.slotsElementLimit; } }

    public bool IsVip()
    {
        GameDebuger.TODO(@"return _playerDto.vipExpiredTime > SystemTimeManager.Instance.GetUTCTimeStamp();");
        return false;
    }

    public void BuyVip(int vipId)
    {
        //TODO 【技术性测试】【VIP】暂停VIP功能（客户端）
        TipManager.AddTip("测试期间未开放VIP");
        return;

//		Vip vip = DataCache.getDtoByCls<Vip>(vipId);
//		ServiceRequestAction.requestServer(PlayerService.buyVip(vipId),"BuyVip",(e)=>{
//			int day = (int)(vip.time / 86400000);
////			TipManager.AddTip(string.Format("购买成功，VIP天数增加{0}天", day));
//			TipManager.AddTip("购买VIP月卡成功");
//			_playerDto.vipExpiredTime = SystemTimeManager.Instance.GetUTCTimeStamp() + vip.time;

//			if(OnPlayerVipUpdate != null)
//				OnPlayerVipUpdate();
//		});
    }

    #endregion

    //为模拟战斗设置的临时playerDto
    public void SetupTempPlayerDto(PlayerDto playerDto)
    {
        _playerDto = playerDto;
        PlayerPrefsExt.PLAYER_PREFIX = _playerDto.id.ToString();
    }

    public void Setup(object data)
    {
        _playerDto = data as PlayerDto;
        PlayerPrefsExt.PLAYER_PREFIX = _playerDto.id.ToString();
        XinGeSdk.RegisterWithAccount(string.Format("{0}_{1}", GameSetting.BundleId, _playerDto.id));

//		ModelManager.Team.SetupTeamInfo(_playerDto.curTeamDto);
//		_playerDto.curTeamDto = null;

        GameDebuger.TODO(@"_festivalDto = null;
        _festivalReward = null;");

        SystemTimeManager.Instance.OnChangeNextDay -= OnChangeNextDay;
        SystemTimeManager.Instance.OnChangeNextDay += OnChangeNextDay;

        GameDebuger.TODO(@"ModelManager.DailyPush.InitDefaultPush();");
    }

    private void HandleCharactorExpInfoNotify(CharactorExpInfoNotify noti)
    {
        //GameDebuger.Log("玩家升级:?" + noti.upgarded);
        if (noti.level > _playerPropertyInfo.playerDto.level)
        {
            _playerPropertyInfo.playerDto.level = noti.level;
            lvUpStream.OnNext(noti.level);
        }

        _playerPropertyInfo.playerDto.exp = noti.exp;

        FireData();
    }

    public void OnChangeNextDay()
    {
        GameDebuger.TODO(@"ModelManager.Arena.ResetRemainTimes();");
        ResetAddPointPlanChangeTimes();
        GameDebuger.TODO(@"_festivalDto = null;
        _festivalReward = null;");
    }

    public void SetupFromAfterLogin(AfterLoginDto afterLoginDto)
    {
        if (afterLoginDto.sceneId != 0)
        {
            _playerDto.sceneId = afterLoginDto.sceneId;
        }

        if (afterLoginDto.mainCrewInfo != null)
            _crewInfoDto = afterLoginDto.mainCrewInfo;

        /** 角色属性信息 */
        _playerPropertyInfo = new PlayerPropertyInfo(afterLoginDto.charactor);

        //初始化加点提升的对应属性类型
       // InitConvertRateDic(_playerPropertyInfo.playerDto.charactorType);// todo haowei

        InitConvertRateDic(_playerPropertyInfo.playerDto.factionId);

        /** 服务器等级信息 */
        UpdateServerGradeDto(afterLoginDto.gameServerGradeDto);

        /** 是否可以答题 */
        _dailyQuestion = afterLoginDto.dailyQuestion;

        /** 日程 取消的活动通知id */
        _scheduleCancelNotify.Clear();
        afterLoginDto.cancelNotify.ForEach(itemId =>
        {
            _scheduleCancelNotify.Add(itemId);
        });
        ScheduleMainViewDataMgr.DataMgr.UpdateCancelList();


        GameDebuger.TODO(@"/** 本次储备经验，可为null表示没有 */
        _reserveExpDto = afterLoginDto.reserveExpDto;

        /** 总储备经验 */
        _playerDto.subWealth.reserveExp = afterLoginDto.reserverExp;


        /** 玩家阵法信息 */
        SetupFormationInfo(afterLoginDto);

        /** 师傅名字 */
        MasterName = afterLoginDto.masterName;

        /** 玩家称谓信息 */
        SetupTitleInfo(afterLoginDto.titleDtoList);
        // SetupMaterTitleInfo(afterLoginDto.titleDtoList);

        _guildInfo = afterLoginDto.guildInfo;

        _showItemsCount = afterLoginDto.showItemsCount;

        //提升 因为需要获取活力VigourMax需要用到_playerPropertyInfo。
        PromoteManager.Instance.Setup();

        CheckPlayerHasPotentialPoint();

        _playerDefaultSkillId = afterLoginDto.playerDefaultSkillId;
        _petDefaultSkillId = afterLoginDto.petDefaultSkillId;

        _stopUpgrade = afterLoginDto.stopUpgrade;
        _merits = afterLoginDto.merits;

        _totalSpellExp = afterLoginDto.spellsInfo.totalExp;

        mNextChangeFactionTime = afterLoginDto.nextChangeFactionTime;

        mChangeEmbedEndTime = afterLoginDto.changeEmbedEndTime;");
    }

    /**public void SetupPlayerTitleDtoFromChangeFaction(List<PlayerTitleDto> pTitleDtoList)
    {
        SetupTitleInfo(pTitleDtoList);
    }*/

    public IEnumerable<int> ScheduleCancelNotify { get { return _scheduleCancelNotify; } }
    public void AddScheduleCancelId(int id)
    {
        if (!_scheduleCancelNotify.Contains(id))
            _scheduleCancelNotify.Add(id);
    }
    public void RemoveScheduleCancelId(int id)
    {
        if (_scheduleCancelNotify.Contains(id))
            _scheduleCancelNotify.Remove(id);
    }

    public void Dispose()
    {
        _playerDto = null;
        TalkingDataHelper.DisposeAccount();
        _playerPropertyInfo = null;

        //	清除节日数据信息
        GameDebuger.TODO(@"_festivalDto = null;");

        if (SystemTimeManager.Instance != null)
        {
            SystemTimeManager.Instance.OnChangeNextDay -= OnChangeNextDay;
        }
        if(_disposible != null)
        {
            _disposible.Dispose();
            _disposible = null;
        }
        lvUpStream = lvUpStream.CloseOnceNull();
        stream = stream.CloseOnceNull();
        GameDebuger.TODO(@"_delayDuelLoseNotifySta = false;");
    }

    #region 玩家基础信息

    public int SceneID {
        get {
            return _playerDto != null ? _playerDto.sceneId : -1;
        }
    }

    public PlayerDto GetPlayer()
    {
        return _playerDto;
    }

    public long GetPlayerWealthGold()
    {
        return GetPlayerWealth(VirtualItemEnum.GOLD);
    }

    public long GetPlayerWealthSilver()
    {
        return GetPlayerWealth(VirtualItemEnum.SILVER);
    }

    public long GetPlayerWealth(VirtualItemEnum id)
    {
        if (_playerDto == null || _playerDto.wealth == null) return 0L;
        var item = _playerDto.wealth.wealthItems.Find(s => s.virtualItemId == (int)id);
        return item == null ? 0L : item.value;
    }

    public long GetPlayerWealthById(int id)
    {
        if (_playerDto == null || _playerDto.wealth == null) return 0L;
        var item = _playerDto.wealth.wealthItems.Find(s => s.virtualItemId == id);
        return item == null ? 0L : item.value;
    }

    public int GetBracerGrade{get { return _playerPropertyInfo.playerDto.bracerGrade; }}
    public void SetBracerGrade(int grade)
    {
        _playerPropertyInfo.playerDto.bracerGrade = grade;
    }

    public int FactionID
    {
        get {
            return _playerDto != null ? _playerDto.factionId : -1;
        }
    }

    public string FactionName
    {
        get
        {
            return _playerDto != null ? _playerDto.faction.name : "无职业名字";
        }
    }

    public Faction PlayerFaction
    {
        get
        {
            return _playerDto.faction;
        }
    }

    public string GetPlayerName()
    {
        return _playerDto.name;
    }

    public void SetPlayerName(string str)
    {
        _playerDto.name = str;
        FireData();
    }

    public void UpdatePlayerName(string nickName)
    {
        GameDebuger.TODO(@"_playerDto.nickname = nickName;
              GameEventCenter.SendEvent(GameEvent.Player_OnPlayerNicknameUpdate, nickName);");
        _playerPropertyInfo.playerDto.name = nickName;
        ServerManager.Instance.UpdateAccountPlayer(_playerDto);
        TalkingDataHelper.SetNickname(nickName);
    }

    public int GetPlayerGender()
    {
        if (_playerPropertyInfo.playerDto.charactor != null && (_playerPropertyInfo.playerDto.charactor as MainCharactor) != null)
            return (_playerPropertyInfo.playerDto.charactor as MainCharactor).gender;

        return 1;
    }

    public int GetPlayerLevel()
    {
        if (_playerPropertyInfo == null || null == _playerPropertyInfo.playerDto)
            return 0;
        else
            return _playerPropertyInfo.playerDto.level;
    }

    /**public float GetPlayerExpPercent()
    {
        int playerLv = GetPlayerLevel();
        ExpGrade expGrade = DataCache.getDtoByCls<ExpGrade>(playerLv + 1);
        if (expGrade != null && expGrade.mainCharactorExp != 0)
        {
            return (float)ModelManager.Player.GetPlayerExp() / (float)expGrade.mainCharactorExp;
        }
        else
            return 1f;
    }*/

    public long GetPlayerId()
    {
        if (_playerDto == null)
        {
            return 0;
        }
        else
        {
            return _playerDto.id;
        }
    }

    public long GetPlayerExp()
    {
        if (_playerPropertyInfo == null)
            return 0;
        else
            return _playerPropertyInfo.playerDto.exp;
    }

    #endregion


    #region 玩家财富

    //是否有足够元宝
    public bool isEnoughIngot(long needIngot, bool openPayModule = false, string tip = "")
    {
        GameDebuger.TODO(@"if (_playerDto.wealth.ingot >= needIngot || !ServiceRequestActionMgr.ServerRequestCheck) {
            return true;
        }
        else{
            if (openPayModule)
            {
                ProxyManager.Pay.OpenPay();
            }
            if (!string.IsNullOrEmpty(tip))
            {
                TipManager.AddTip(tip);
            }
            return false;
        }");
        return false;
    }

    
    /// <summary>
    /// 是否有足够的绑定元宝
    /// </summary>
    /// <param name="needVoucher"></param>
    /// <param name="openPayModule"></param>
    /// <param name="tip"></param>
    /// <returns></returns>
    public bool isEnoughVoucher(long needVoucher, bool openPayModule = false, string tip = "")
    {
        GameDebuger.TODO(@"if (_playerDto.wealth.voucher >= needVoucher || !ServiceRequestActionMgr.ServerRequestCheck)
        {
            return true;
        }
        else
        {
            if (openPayModule)
            {
                ProxyManager.Pay.OpenPay();
            }
            if (!string.IsNullOrEmpty(tip))
            {
                TipManager.AddTip(tip);
            }
            return false;
        }");
        return false;
    }

    /// <summary>
    /// 是否足够拍卖行可用元宝 Ises the enough forbid ingot.
    /// </summary>
    /// <returns><c>true</c>, if enough forbid ingot was ised, <c>false</c> otherwise.</returns>
    /// <param name="needForbidIngot">Need forbid ingot.</param>
    /// <param name="openPayModule">If set to <c>true</c> open pay module.</param>
    /// <param name="tip">Tip.</param>
    public bool isEnoughForbidIngot(long needForbidIngot, bool openPayModule = false, string tip = "")
    {
        GameDebuger.TODO(@"int forbidIngot = _playerDto.wealth.ingot - _playerDto.wealth.forbidIngot;
        if (forbidIngot >= needForbidIngot || !ServiceRequestActionMgr.ServerRequestCheck)
        {
            return true;
        }
        else
        {
            if (openPayModule)
            {
                ProxyManager.Pay.OpenPay();
            }
            if (!string.IsNullOrEmpty(tip))
            {
                TipManager.AddTip(tip);
            }
            return false;
        }");
        return false;
    }

    /// <summary>
    /// 是否有足够的经验货币
    /// </summary>
    /// <param name="expCurrency"></param>
    /// <param name="openPayModule"></param>
    /// <param name="tip"></param>
    /// <returns></returns>
    public bool isEnoughExpCurrency(long expCurrency, bool openPayModule = false, string tip = "")
    {
        GameDebuger.TODO(@"if (_playerDto.wealth.expCurrency >= expCurrency || !ServiceRequestActionMgr.ServerRequestCheck)
        {
            return true;
        }
        else
        {
            if (!string.IsNullOrEmpty(tip))
            {
                TipManager.AddTip(tip);
            }
            return false;
        }");
        return false;
    }

    #region 玩家是否有足够改名花费的钱

    public bool hasEnoughMoneyChangeName()
    {
        GameDebuger.TODO(@"if (_playerDto.wealth.ingot >= GetMoneyCost() || !ServiceRequestActionMgr.ServerRequestCheck)
        {
            return true;
        }
        else
        {
            return false;
        }");
        return false;
    }

    #endregion

    public int GetMoneyCost()
    {
        int lv = ModelManager.Player.GetPlayerLevel();
        int RENAME_FACTOR1 = DataCache.GetStaticConfigValue(AppStaticConfigs.RENAME_FACTOR1);
        int RENAME_FACTOR2 = DataCache.GetStaticConfigValue(AppStaticConfigs.RENAME_FACTOR2);
        int RENAME_FACTOR3 = DataCache.GetStaticConfigValue(AppStaticConfigs.RENAME_FACTOR3);

        int money = RENAME_FACTOR1 + (lv - RENAME_FACTOR2) * RENAME_FACTOR3;

        return money;
    }

    public bool isEnoughCopper(long needCopper, bool openPayModule = false, string tip = "")
    {
        GameDebuger.TODO(@"if(_playerDto.wealth.copper >= needCopper || !ServiceRequestActionMgr.ServerRequestCheck){
            return true;
        }
        else{
            if (openPayModule)
            {
                ProxyManager.Pay.OpenCopper();
            }
            if (!string.IsNullOrEmpty(tip))
            {
                TipManager.AddTip(tip);
            }
            return false;
        }");
        return false;
    }

    public bool isEnoughSilver(int needSilver, bool openPayModule = false, string tip = "")
    {
        GameDebuger.TODO(@"if(_playerDto.wealth.silver >= needSilver || !ServiceRequestActionMgr.ServerRequestCheck){
            return true;
        }
        else{
            if (openPayModule)
            {
                ProxyManager.Pay.OpenSilver();
            }
            if (!string.IsNullOrEmpty(tip))
            {
                TipManager.AddTip(tip);
            }
            return false;
        }");
        return false;
    }

    public bool isEnoughScore(long needScore, bool tip = false)
    {
        GameDebuger.TODO(@"if(_playerDto.wealth.score >= needScore || !ServiceRequestActionMgr.ServerRequestCheck){
            return true;
        }
        else{
            if (tip) TipManager.AddTip(string.Format('你的{0}不足', ItemIconConst.GetIconConstByItemId(AppVirtualItem.VirtualItemEnum_SCORE)));
            return false;
        }");
        return false;
    }

    public bool isEnoughContribute(long needContribute, bool openPayModule = false, string tip = "")
    {
        GameDebuger.TODO(@"if (_playerDto.wealth.contribute >= needContribute || !ServiceRequestActionMgr.ServerRequestCheck)
        {
                  return true;
              }
              else
              {
                  if (openPayModule)
                  {
                ProxyManager. Guide.OpenGuildContributeExchange();
                  }

                  if (!string.IsNullOrEmpty(tip))
                  {
                      //TipManager.AddTip(string.Format('你的{0}不足', ItemIconConst.GetIconConstByItemId(AppVirtualItem.VirtualItemEnum_CONTRIBUTE)));
                      TipManager.AddTip(tip);
                  }
                  return false;
              }");
        return false;
    }

    //	public SubWealthNotify GetSubWealth()
    //	{
    //        return _playerDto.subWealth;
    //	}

    //	public void UpdateSubWealth(SubWealthNotify newSubWealth, bool delayShow){
    //		//	角色数据由于延迟还未回来，不处理
    //		if (_playerDto == null) return;
    //		if (newSubWealth == null) return;
    //
    //		SubWealthNotify oldNotify = _playerDto.subWealth;
    //
    //        int changeVigour = newSubWealth.vigour - oldNotify.vigour;//活力
    //        int changeReserveExp = newSubWealth.reserveExp - oldNotify.reserveExp;//储备经验
    //        long changeNimbus = newSubWealth.nimbus - oldNotify.nimbus;//灵气
    //        int changeSatiation = newSubWealth.satiation - oldNotify.satiation; //饱食度
    //
    //        if(newSubWealth.traceType != null && newSubWealth.traceType.tip)
    //        {
    //            if (changeVigour < 0) {
    //                if (newSubWealth.traceTypeId == AppTraceTypes.CHAT_TALK_LOST)
    //                {
    //                    TipManager.AddLostCurrencyTip(changeVigour,'活力', delayShow, '发言成功，消耗{0}{1}');
    //                }
    //                else
    //                {
    //                    TipManager.AddLostCurrencyTip(changeVigour,'活力', delayShow);
    //                }
    //            } else if(changeVigour > 0) {
    //                TipManager.AddGainCurrencyTip(changeVigour,'活力', delayShow);
    //            }
    //
    //            if (changeReserveExp < 0) {
    //                TipManager.AddLostCurrencyTip(changeReserveExp, '储备经验', delayShow);
    //            } else if(changeReserveExp > 0) {
    //                TipManager.AddGainCurrencyTip(changeReserveExp, '储备经验', delayShow);
    //            }
    //
    //            if (changeNimbus < 0) {
    //                TipManager.AddLostCurrencyTip(changeNimbus, '灵气', delayShow);
    //            } else if(changeNimbus > 0) {
    //                TipManager.AddGainCurrencyTip(changeNimbus, '灵气', delayShow);
    //            }
    //
    //            if (changeSatiation < 0) {
    //                TipManager.AddLostCurrencyTip(changeSatiation, '饱食度', delayShow);
    //            } else if(changeSatiation > 0) {
    //                TipManager.AddGainCurrencyTip(changeSatiation, '饱食度', delayShow);
    //            }
    //        }
    //
    //        _playerDto.subWealth = newSubWealth;
    //        GameEventCenter.SendEvent(GameEvent.Player_OnSubWealthChanged, newSubWealth);
    //
    //        if (changeSatiation != 0)
    //            CheckOutSatiationState ();
    //        //通知提升处是否还需要显示活力按钮
    //        if( ModelManager.Player.Vigour < ModelManager.Player.VigourMax * 0.8 )
    //            PromoteManager.Instance.SetState(PromoteType.VIGOUR,false);
    //	}

    public void CheckOutSatiationState()
    {
        GameDebuger.TODO(@"if(_playerDto.subWealth.satiation > 0){
            ModelManager.PlayerBuff.ToggleSatiationBuffTip(true);
        }else
            ModelManager.PlayerBuff.ToggleSatiationBuffTip(false);

        if(_playerDto.subWealth.satiation <= 10)
            PromoteManager.Instance.SetState(PromoteType.SATIATION,true);
        else if(_playerDto.subWealth.satiation >= 20)
            PromoteManager.Instance.SetState(PromoteType.SATIATION,false);

        if (_playerDto.subWealth.satiation < 30) {
            if(ModelManager.Backpack.GetDto() != null)
                ProxyManager.MainUI.OpenSatiationPropsUseView ();
        }");
        
    }

    //获取补充饱食度消耗铜币数量
    public long GetReplenishSatiationFee()
    {
        GameDebuger.TODO(@"int count = MaxSatiationVal - _playerDto.subWealth.satiation;
              return (long)Math.Floor((ModelManager.Player.ServerGrade * DataCache.GetStaticConfigValue(AppStaticConfigs.INGOT_CONVERT_COPPER_FACTOR1) + DataCache.GetStaticConfigValue(AppStaticConfigs.INGOT_CONVERT_COPPER_FACTOR2)) / 10.0 * count);");
        return 0;
    }

    public int MaxSatiationVal
    {
        get
        {
            GameDebuger.TODO(@"AppVirtualItem info = DataCache.getDtoByCls<GeneralItem>(AppVirtualItem.VirtualItemEnum_SATIATION) as AppVirtualItem;
                     return info == null?0:(int)info.carryLimit;");
            return 0;
        }
    }

    public bool isFullSatiation()
    {
        GameDebuger.TODO(@"if(_playerDto.subWealth.satiation >= MaxSatiationVal){
            return true;
        }");
        return false;
    }

    /// <summary>
    /// 使用绑定元宝
    /// </summary>
    /// <param name="cost"></param>
    public void UseVoucher(int cost)
    {
        if (cost > 0)
        {
            GameDebuger.TODO(@"_playerDto.wealth.voucher -= cost;
            if (_playerDto.wealth.voucher < 0)
                _playerDto.wealth.voucher = 0;

            TipManager.AddLostCurrencyTip(cost, ItemIconConst.Voucher);

            GameEventCenter.SendEvent(GameEvent.Player_OnWealthChanged, _playerDto.wealth);");
        }
    }

    public void UseIngot(int cost)
    {
        if (cost > 0)
        {
            GameDebuger.TODO(@"_playerDto.wealth.ingot -= cost;
            if(_playerDto.wealth.ingot < 0)
                _playerDto.wealth.ingot = 0;

            TipManager.AddLostCurrencyTip(cost, ItemIconConst.Ingot);

            GameEventCenter.SendEvent(GameEvent.Player_OnWealthChanged, _playerDto.wealth);");
        }
    }

    public void UseSilver(int cost)
    {
        if (cost > 0)
        {
            GameDebuger.TODO(@"_playerDto.wealth.silver -= cost;
            if(_playerDto.wealth.silver < 0)
                _playerDto.wealth.silver = 0;

            TipManager.AddLostCurrencyTip(cost, ItemIconConst.Silver);

            GameEventCenter.SendEvent(GameEvent.Player_OnWealthChanged, _playerDto.wealth);");
        }
    }

    public void UseCopper(long cost, bool showTip = true)
    {
        if (cost > 0L)
        {
            GameDebuger.TODO(@"_playerDto.wealth.copper -= cost;
            if(_playerDto.wealth.copper < 0)
                _playerDto.wealth.copper = 0;

                     if (showTip)
                     {
                         TipManager.AddLostCurrencyTip(cost, ItemIconConst.Copper);
                     }

            GameEventCenter.SendEvent(GameEvent.Player_OnWealthChanged, _playerDto.wealth);");
        }
    }

    #endregion

    #region WealthNotify特殊处理

    //	public void DoOnWealthChanged(WealthNotify newWealth) {
    //		_playerDto.wealth = newWealth;
    //
    //        GameEventCenter.SendEvent(GameEvent.Player_OnWealthChanged, newWealth);
    //	}

    #endregion

    #region 玩家属性信息

    //----------------------------------------------------------------------------
    #region 属性获取相关

    public float GetPropertyByID(int id)
    {
        List<CharacterPropertyDto> proList = _playerPropertyInfo.playerDto.properties;

        var item = proList.Find<CharacterPropertyDto>(s => s.propId == id);
        return item == null ? 0 : item.propValue;
    }

    public CharacterPropertyDto GetPropertyDtoById(int id)
    {
        List<CharacterPropertyDto> proList = _playerPropertyInfo.playerDto.properties;

        return proList.Find<CharacterPropertyDto>(s => s.propId == id);
    }

    public void SetPropertyDtoById(int id, float value)
    {
        List<CharacterPropertyDto> proList = _playerPropertyInfo.playerDto.properties;
        if (proList.Find(x => x.propId == id) != null)
            proList.Find(x => x.propId == id).propValue = value;
    }

    //获得ID属性对应的<影响属性,影响字符串>
    private Dictionary<int, string> GetConvertDic(int ID)
    {
        var retStr = new Dictionary<int, string>();

        var bpcList = DataCache.getArrayByCls<BasePropertyTransform>();

        for (int i = 0; i < bpcList.Count; i++)
        {
            if(bpcList[i].factionId == ID)
                retStr.Add(bpcList[i].propertyId, bpcList[i].battleProperty);
        }

        return retStr;
    }

    //一级属性对应的二级属性增量列表
    private Dictionary<int, Dictionary<int, float>> convertRateDic;

    private void InitConvertRateDic(int id)
    {
        convertRateDic = new Dictionary<int, Dictionary<int, float>>();

        Dictionary<int, string> temp = GetConvertDic(id);   //<计算属性id，对应的字符串>
        foreach (var kvp in temp)
        {
            convertRateDic.Add(kvp.Key, new Dictionary<int, float>());
            Dictionary<int, float> dic = Rate(kvp.Value);
            foreach (var item in dic)
            {
                convertRateDic[kvp.Key].Add(item.Key, item.Value);
            }
        }
    }

    public string GetPropertyDesc(int propertyId, bool isPlayer = false)
    {
        var temp = convertRateDic[propertyId];
        var sb = new StringBuilder("每级增加");
        if (isPlayer)
        {
            foreach (var item in temp)
            {
                var ca = DataCache.getDtoByCls<CharacterAbility>(item.Key);
                if (item.Value == 0)
                    continue;
                sb.Append(" ");
                sb.Append(item.Value.ToString().WrapColor(ColorConstantV3.Color_Blue2));
                sb.Append(" ");
                sb.Append(ca.name);
                sb.Append("，");
            }
        }
        else
        {
            var dic = DataCache.getDicByCls<CharacterAbility>();
            foreach (var item in temp)
            {
                if(dic.ContainsKey(item.Key))
                {
                    sb.Append(dic[item.Key].name);
                    sb.Append("，");
                }
            }
        }
        sb.Remove(sb.ToString().LastIndexOf("，"), 1);        //删除最后一个 、

        return sb.ToString();
    }

    //参数为影响字符串，返回<影响的属性值,影响的比例>
    private Dictionary<int, float> Rate(string str)
    {
        Dictionary<int, float> retDic = new Dictionary<int, float>();

        string[] strList = str.Split(',');
        for (int i = 0; i < strList.Length; i++)
        {
            string[] item = strList[i].Split(':');
            retDic.Add(StringHelper.ToInt(item[0]), float.Parse(item[1]));
        }
        return retDic;
    }

    #endregion
    //-----------------------------------------------------------------------------

    //private CharactorPropertyUpdateNotify _lastBpNotify;
    //private CharactorExpInfoNotify _lastExpNotify;
    public void CheckDelayShow()
    {
        GameDebuger.TODO(@"if(_lastBpNotify != null)
        {
            UpdatePlayerBp(_lastBpNotify,false);
            _lastBpNotify = null;
        }");

        GameDebuger.TODO(@"if(_lastExpNotify != null)
        {
            if (_lastExpNotify.upgarded)
            {
                GameEventCenter.SendEvent(GameEvent.Player_OnPlayerGradeUpdate);
            }

            FunctionOpenViewController.CheckNeedOpenView(_lastExpNotify.oldLevel,_lastExpNotify.level);
            _lastExpNotify = null;

            GameEventCenter.SendEvent(GameEvent.Player_OnPlayerExpUpdate);
        }

        //  国宝信息输出信息Delay
        if (_delayDuelLoseNotifySta) {
            _delayDuelLoseNotifySta = false;

            GameEventCenter.SendEvent(GameEvent.Player_OnPlayerExpUpdate);
        }        
");
    }

    /**public void UpdatePlayerBp(CharactorPropertyUpdateNotify notify, bool delayShow){
        if(notify == null) return;

        if(delayShow){
               _lastBpNotify = notify;
        }else{
            ShortBattlePropertyDto bpInfo = notify.properties;
            if(notify.showTips){
                ShortBattlePropertyDto oldBpInfo = _playerPropertyInfo.playerDto.properties;
                PrintBpChangeTips(bpInfo.maxHp - oldBpInfo.maxHp,"气血");
                PrintBpChangeTips(bpInfo.maxMp - oldBpInfo.maxMp,"魔法");
                PrintBpChangeTips(bpInfo.attack - oldBpInfo.attack,"攻击");
                PrintBpChangeTips(bpInfo.defense - oldBpInfo.defense,"防御");
                PrintBpChangeTips(bpInfo.speed - oldBpInfo.speed,"速度");
                PrintBpChangeTips(bpInfo.magic - oldBpInfo.magic,"灵力");
            }
            _playerPropertyInfo.playerDto.properties = bpInfo;
            _playerPropertyInfo.sp = 0;
               GameEventCenter.SendEvent(GameEvent.Player_OnPlayerPropertyUpdate);
        }
    }*/

    public void CleanPlayerSp()
    {
        if (_playerPropertyInfo == null) return;
        _playerPropertyInfo.sp = 0;
        GameDebuger.TODO(@"GameEventCenter.SendEvent(GameEvent.Player_OnPlayerPropertyUpdate);");
    }

    private void PrintBpChangeTips(int changeVal, string title)
    {
        if (changeVal > 0)
            TipManager.AddTip(string.Format("{0} +{1}", title, changeVal).WrapColor(ColorConstant.Color_Tip_GainCurrency_Str));
        else if (changeVal < 0)
            TipManager.AddTip(string.Format("{0} {1}", title, changeVal).WrapColor(ColorConstant.Color_Tip_LostCurrency_Str));
    }

    public void UpdateHpMpSp(int hp, int mp, int sp)
    {
        GameDebuger.TODO(@"_playerPropertyInfo.playerDto.properties.hp = hp;
        _playerPropertyInfo.playerDto.properties.mp = mp;");
        _playerPropertyInfo.sp = sp;

        GameDebuger.TODO(@"GameEventCenter.SendEvent(GameEvent.Player_OnPlayerPropertyUpdate);");
    }

    public void ResetPlayerPerAp(int itemIndex, int apType, int resetPoint, bool useIngot, System.Action onSuccess = null)
    {
        GameDebuger.TODO(@"
ServiceRequestAction.requestServer(PlayerService.resetPerAptitude(itemIndex,apType,useIngot),""ResetPlayerAp"",(e)=>{
            if(apType == AptitudeProperties.AptitudeType_Constitution)
                _playerPropertyInfo.playerDto.aptitudeProperties.constitution -= resetPoint;
            else if(apType == AptitudeProperties.AptitudeType_Intelligent)
                _playerPropertyInfo.playerDto.aptitudeProperties.intelligent -= resetPoint;
            else if(apType == AptitudeProperties.AptitudeType_Strength)
                _playerPropertyInfo.playerDto.aptitudeProperties.strength -= resetPoint;
            else if(apType == AptitudeProperties.AptitudeType_Stamina)
                _playerPropertyInfo.playerDto.aptitudeProperties.stamina -= resetPoint;
            else if(apType == AptitudeProperties.AptitudeType_Dexterity)
                _playerPropertyInfo.playerDto.aptitudeProperties.dexterity -= resetPoint;
            
            _playerPropertyInfo.playerDto.potential +=resetPoint;
            TipManager.AddTip(string.Format('你的{0}属性减掉了{1}点，潜力点增加{2}点',
                                            ItemHelper.AptitudeTypeName(apType).WrapColor(ColorConstant.Color_Tip_LostCurrency_Str),
                                            resetPoint.ToString().WrapColor(ColorConstant.Color_Tip_LostCurrency_Str),
                                            resetPoint.ToString().WrapColor(ColorConstant.Color_Tip_GainCurrency_Str)));
//          CalculatePlayerBp();
            CheckPlayerHasPotentialPoint();
            GameEventCenter.SendEvent(GameEvent.Player_OnPlayerPropertyUpdate);

            if(onSuccess != null)
                onSuccess();
        });        
");
    }

    /**public void UpdatePlayerExpInfo (CharactorExpInfoNotify expNotify, bool needTip, bool delayShow)
    {
           if (expNotify == null)
               return;

           if (_playerDto == null)
           {
               return;
           }

           if(!expNotify.maxLevelReached){
               if (needTip)
               {
                   string tip = string.Format("你获得{0}{1}",expNotify.expGain.ToString().WrapColor(ColorConstant.Color_Tip_GainCurrency),ItemIconConst.GetIconConstByItemId(AppVirtualItem.VirtualItemEnum_PLAYER_EXP));

                   if(expNotify.reserveExp > 0 || expNotify.leaderExp > 0)
                   {
                       tip += "（";
                       string tempTip = "";
                       if (expNotify.leaderExp > 0)
                       {

                           tempTip += string.Format("队长经验加成{0}{1}", expNotify.leaderExp.ToString().WrapColor(ColorConstant.Color_Tip_GainCurrency), ItemIconConst.GetIconConstByItemId(AppVirtualItem.VirtualItemEnum_PLAYER_EXP));
                       }

                       if (expNotify.reserveExp > 0)
                       {
                           if (!string.IsNullOrEmpty(tempTip))
                               tempTip += "、";
                           tempTip += string.Format("储备经验加成{0}{1}", expNotify.reserveExp.ToString().WrapColor(ColorConstant.Color_Tip_GainCurrency), ItemIconConst.GetIconConstByItemId(AppVirtualItem.VirtualItemEnum_PLAYER_EXP));
                       }
                       tip += tempTip + "）";
                   }


                   if (expNotify.copperGain > 0)
                   {
                       tip += "、" + string.Format("{0}{1}", expNotify.copperGain.ToString().WrapColor(ColorConstant.Color_Tip_GainCurrency), ItemIconConst.GetIconConstByItemId(AppVirtualItem.VirtualItemEnum_COPPER));
                   }

                   if (expNotify.invertCopper > 0)
                   {
                       tip += string.Format("（经验转化{0}{1}）",expNotify.invertCopper.ToString().WrapColor(ColorConstant.Color_Tip_GainCurrency),ItemIconConst.GetIconConstByItemId(AppVirtualItem.VirtualItemEnum_COPPER));
                   }

                   TipManager.AddTip(tip, true, delayShow);
               }

               //同步更新PlayerDto的grade
               _playerDto.grade = expNotify.level;

               if(_playerPropertyInfo != null){
                   _playerPropertyInfo.playerDto.exp = expNotify.exp;
                   _playerPropertyInfo.playerDto.level = expNotify.level;
               }

               if (expNotify.upgarded) {

                   TalkingDataHelper.SetLevel(expNotify.level);
                   ServerManager.Instance.UpdateAccountPlayer(_playerDto);

                   if(_playerPropertyInfo != null){
                       int lvCount = expNotify.level - expNotify.oldLevel;

                       for (int i=0; i<lvCount; ++i) {
                           //每级默认各资质属性增加1点
                           AddApPoint (_playerPropertyInfo.playerDto.aptitudeProperties, 1, 1, 1, 1, 1);

                           //小于40级且未重置过资质属性，潜力点按默认配点分配
                           if (expNotify.oldLevel+(i+1) < DataCache.GetStaticConfigValue (AppStaticConfigs.MAIN_CHARACTOR_DISPOSABLE_POTENTIAL_MIN_LEVEL, 40) 
                               && !ModelManager.Player.HasCustomAptitude ()){
                               AptitudeProperties defaultApDistrubute = _playerPropertyInfo.playerDto.faction.defaultAptitudeDistrubute;
                               AddApPoint (_playerPropertyInfo.playerDto.aptitudeProperties,
                                                   defaultApDistrubute.constitution,
                                                   defaultApDistrubute.intelligent,
                                                   defaultApDistrubute.strength,
                                                   defaultApDistrubute.stamina,
                                                   defaultApDistrubute.dexterity);
                               
                           } else {
                               //40级后(包括40级)不自动分配潜力点
                               _playerPropertyInfo.playerDto.potential += DataCache.GetStaticConfigValue (AppStaticConfigs.DISPOSABLE_POTENTIAL_POINT_GAIN_PER_UPGRADE, 5);
                           }
                       }
                       //重新计算伙伴属性值
                       ModelManager.Crew.SetupCrewUpdateFlag();
                       ModelManager.Crew.CheckNewCrewCanRecruit();
    //                  ModelManager.Pet.CheckHasNewPet();
                       ModelManager.Pet.CheckPetHasPotentialPoint();
                       ModelManager.DailyPush.NewOpenActivity(expNotify);
                       CheckPlayerHasPotentialPoint();
                       RedPointManager.Instance.JudgeFunctionOpen();
                       if( FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum_FactionSkill,false) )
                           ModelManager.FactionSkill.SetPromoteShowTips(); //提升-可学习技能
    //                  CalculatePlayerBp ();

                       ModelManager.MallShopping.CheckMallFunctionOpen(expNotify.oldLevel, expNotify.level);
                   }

                   if(delayShow == false)
                   {
                       GameEventCenter.SendEvent(GameEvent.Player_OnPlayerGradeUpdate);

                       FunctionOpenViewController.CheckNeedOpenView(expNotify.oldLevel,expNotify.level);
                   }
               }

               if (delayShow == false)
               {
                   GameEventCenter.SendEvent(GameEvent.Player_OnPlayerExpUpdate);
               }
               else
               {
                   _lastExpNotify = expNotify;
               }
           }
           else
           {
               if (expNotify.copperGain > 0L && needTip)
               {
                   string tip = string.Format("获得{0}{1}", expNotify.copperGain.ToString().WrapColor(ColorConstant.Color_Tip_GainCurrency), ItemIconConst.GetIconConstByItemId(AppVirtualItem.VirtualItemEnum_COPPER));

                   if (expNotify.invertCopper > 0)
                   {
                       tip += string.Format("（经验转化{0}{1}）",expNotify.invertCopper.ToString().WrapColor(ColorConstant.Color_Tip_GainCurrency),ItemIconConst.GetIconConstByItemId(AppVirtualItem.VirtualItemEnum_COPPER));
                   }

                   TipManager.AddTip(tip, true, delayShow);
               }
           }

           _playerDto.subWealth.reserveExp -= expNotify.reserveExp;
           GameEventCenter.SendEvent(GameEvent.Player_OnSubWealthChanged, _playerDto.subWealth);

           ModelManager.GrowUpGuide.CheckNewOpen();
    }*/

    //	经验扣除
    //	private bool _delayDuelLoseNotifySta = false;
    //	public long UpdatePlayerExpInfo (DuelLoseNotify expNotify, bool delayShow, bool needTip = false) {
    //		long tInventExp = 0;
    //
    //		if (expNotify == null)
    //			return tInventExp;
    //		
    //		if (_playerDto == null)
    //			return tInventExp;
    //
    //		if (needTip) {
    //			//	该方法目前只有决斗中调用，其他业务逻辑需要tip另行处理
    //			string tip = "";
    //			TipManager.AddTip(tip, true, delayShow);
    //		}
    //		
    //		if (_playerPropertyInfo != null) {
    //			long tLastExp = _playerPropertyInfo.playerDto.exp;
    //			long tFinalExp = Math.Max(tLastExp - expNotify.loseExp, 0);
    //			_playerPropertyInfo.playerDto.exp = tFinalExp;
    //
    //			tInventExp = tFinalExp > 0? expNotify.loseExp : tLastExp;
    //		}
    //		
    //        if (delayShow)
    //            _delayDuelLoseNotifySta = true;
    //        else
    //            GameDebuger.TODO(@"GameEventCenter.SendEvent(GameEvent.Player_OnPlayerExpUpdate);");
    //
    //		//	扣除经验，不用升级检查了吧
    //		//ModelManager.GrowUpGuide.CheckNewOpen();
    //
    //		return tInventExp;
    //	}

    public PlayerPropertyInfo GetPlayerPropertyInfo()
    {
        return _playerPropertyInfo;
    }

    public bool HasCustomAptitude()
    {
        return _playerPropertyInfo.playerDto.hasCustomAptitude;
    }

    //重置主角所有资质属性
    /**public void RestPlayerApPoint ()
    {
        int oldTotalPoint = _playerPropertyInfo.playerDto.aptitudeProperties.constitution
                        +_playerPropertyInfo.playerDto.aptitudeProperties.intelligent
                        +_playerPropertyInfo.playerDto.aptitudeProperties.strength
                        +_playerPropertyInfo.playerDto.aptitudeProperties.stamina
                        +_playerPropertyInfo.playerDto.aptitudeProperties.dexterity
                        +_playerPropertyInfo.playerDto.potential;

           int originPoint = _playerPropertyInfo.playerDto.level + DataCache.GetStaticConfigValue (AppStaticConfigs.MIN_APTITUDE_RESET_POINT, 10);
           int originPoint = 0;
        _playerPropertyInfo.playerDto.aptitudeProperties.constitution = originPoint;
        _playerPropertyInfo.playerDto.aptitudeProperties.intelligent = originPoint;
        _playerPropertyInfo.playerDto.aptitudeProperties.strength = originPoint;
        _playerPropertyInfo.playerDto.aptitudeProperties.stamina = originPoint;
        _playerPropertyInfo.playerDto.aptitudeProperties.dexterity = originPoint;

        _playerPropertyInfo.playerDto.potential = oldTotalPoint-5*originPoint;
        _playerPropertyInfo.playerDto.hasCustomAptitude = true;
        CheckPlayerHasPotentialPoint();
    //      CalculatePlayerBp ();
           GameDebuger.TODO(@"GameEventCenter.SendEvent(GameEvent.Player_OnPlayerPropertyUpdate);");
           TipManager.AddTip("你的属性点已全部重置");
       }*/

    /**public void UpdatePlayerAp (int potential, int constitution, int intelligent, int strength, int stamina, int dexterity)
    {
        _playerPropertyInfo.playerDto.potential = potential;
        _playerPropertyInfo.playerDto.aptitudeProperties.constitution = constitution;
        _playerPropertyInfo.playerDto.aptitudeProperties.intelligent = intelligent;
        _playerPropertyInfo.playerDto.aptitudeProperties.strength = strength;
        _playerPropertyInfo.playerDto.aptitudeProperties.stamina = stamina;
        _playerPropertyInfo.playerDto.aptitudeProperties.dexterity = dexterity;

        CheckPlayerHasPotentialPoint();
    //      CalculatePlayerBp();
           GameEventCenter.SendEvent(GameEvent.Player_OnPlayerPropertyUpdate);
    }*/

    //更新玩家额外潜力点，此潜力点可通过GM指令添加
    public void UpdatePlayerPotential(int potential, int extraPotential)
    {
        if (_playerPropertyInfo == null)
            return;

        _playerPropertyInfo.playerDto.potential = potential;
        _playerPropertyInfo.playerDto.extraPotential = extraPotential;
        GameDebuger.TODO(@"GameEventCenter.SendEvent(GameEvent.Player_OnPlayerPropertyUpdate);");
    }
		
    //重新计算装备资质属性
    public void UpdatePlayerEqAps()
    {
        GameDebuger.TODO(@"List<BagItemDto> equips = ModelManager.Backpack.GetBodyEquip();
        if(equips != null && equips.Count > 0){
        int[] eqAps = new int[5];
        for (int i=0,len=equips.Count;i<len; i++)
        {
        BagItemDto itemDto = equips[i];
        //Equipment equipmentInfo = itemDto.item as Equipment;
        EquipmentExtraDto eqExtraDto = itemDto.extra as EquipmentExtraDto;

                // 耐久为0不计算属性
                if(eqExtraDto.duration <= 0)
                    continue;

                //装备资质属性
                for(int index = 0;index < eqExtraDto.aptitudeProperties.Count;index++)
                {
                    if(eqExtraDto.aptitudeProperties[index].aptitudeType != AptitudeProperties.MP_TYPE)
                    {
                        int n = eqExtraDto.aptitudeProperties[index].aptitudeType - 1;
                        if(n < eqAps.Length)
                        {
                            eqAps[n] = eqAps[n] + eqExtraDto.aptitudeProperties[index].value;
                        }
                    }
                }

                //装备认证属性
                for(int index = 0;index < eqExtraDto.certificateProperties.Count;index++)
                {
                    if(eqExtraDto.certificateProperties[index].aptitudeType != AptitudeProperties.MP_TYPE)
                    {
                        int n = eqExtraDto.certificateProperties[index].aptitudeType - 1;
                        if(n < eqAps.Length)
                        {
                            eqAps[n] = eqAps[n] + eqExtraDto.certificateProperties[index].value;
                        }
                    }
                }
        }

        //更新人物装备资质属性数值
        eqAps.CopyTo(_playerPropertyInfo.EqAps,0);
        }else{
        //没有装备时清空人物装备资质属性
        for(int i=0;i<_playerPropertyInfo.EqAps.Length;++i){
        _playerPropertyInfo.EqAps[i] = 0;
        }
        }");
    }

    /**public static void AddApPoint (AptitudeProperties apDto, int constitution, int intelligent, int strength, int stamina, int dexterity)
    {
        apDto.constitution += constitution;
        apDto.intelligent += intelligent;
        apDto.strength += strength;
        apDto.stamina += stamina;
        apDto.dexterity += dexterity;
    }*/
    #endregion

    #region 玩家加点配置      S1
    private bool autoAddPoint;
    public bool AutoAddPoint
    {
        set {
            autoAddPoint = value;
            PlayerPrefsExt.SetBool(string.Format("{0}AutoAddPoint", GetPlayerName()), autoAddPoint);
        }
        get
        {
            autoAddPoint = PlayerPrefsExt.GetBool(string.Format("{0}AutoAddPoint", GetPlayerName()));
            return autoAddPoint;
        }
    }

    public Dictionary<int, int> APPlandic
    {
        get
        {
            Dictionary<int, int> retDic = new Dictionary<int, int>();
            for (int i = 0; i < 5; i++)
            {
                int temp = PlayerPrefsExt.GetPlayerInt(string.Format("{0}{1}", GetPlayerName(), i + 101), 0);
                retDic.Add(i + 101, temp);
            }
            return retDic;
        }
    }
   public void SetAPPlan(int propertyID, int point)
    {
        string name = GetPlayerName();

        PlayerPrefsExt.SetPlayerInt(string.Format("{0}{1}", name, propertyID), point);
    }

    public int CalulateTotalPoint()
    {
        int totalPoint = 0;
        for (int i = 0; i < 5; i++)
        {
            totalPoint += (int)GetPropertyByID(i + 101);
        }
        return totalPoint;
    }

    public List<AptitudeTips> GetAptitudeTips()
    {
        List<AptitudeTips> tipsList = DataCache.getArrayByCls<AptitudeTips>();

        return tipsList.FindAll(s => s.factionId == _playerPropertyInfo.playerDto.charactorType);
    }

    public int CanResetPoint
    {
        get { return 10 + GetPlayerLevel(); }
    }

    private List<AptitudeTips> tipsList;
    public List<AptitudeTips> TipsList      //推荐加点方案
    {
        get
        {
            if (tipsList == null)
                tipsList = DataCache.getArrayByCls<AptitudeTips>().FindAll(s => s.factionId == _playerPropertyInfo.playerDto.charactorType);
            return tipsList;
        }
    }

    public int RecommendIndex
    {
        set { PlayerPrefsExt.SetPlayerInt(string.Format("{0}RecommendAddPointIndex", GetPlayerName()), value); }
        get { return PlayerPrefsExt.GetPlayerInt(string.Format("{0}RecommendAddPointIndex", GetPlayerName())); }
    }

    #endregion


    #region 玩家加点方案
    public int GetAddPointPlanOpenCount()
    {
        int openCount = 1;
        int playerLv = GetPlayerLevel();
        if (playerLv >= DataCache.GetStaticConfigValue(AppStaticConfigs.MAIN_CHARACTOR_DISPOSABLE_POINT_PLAN_TWO_LEVEL, 40)
            && playerLv < DataCache.GetStaticConfigValue(AppStaticConfigs.MAIN_CHARACTOR_DISPOSABLE_POINT_PLAN_THREE_LEVEL, 90))
            openCount = 2;
        else if (playerLv >= DataCache.GetStaticConfigValue(AppStaticConfigs.MAIN_CHARACTOR_DISPOSABLE_POINT_PLAN_THREE_LEVEL, 90))
            openCount = 3;

        return openCount;
    }

    public int GetAddPointPlanOpenLevel(int openCount)
    {
        if (openCount == 2)
            return DataCache.GetStaticConfigValue(AppStaticConfigs.MAIN_CHARACTOR_DISPOSABLE_POINT_PLAN_TWO_LEVEL, 40);
        else if (openCount == 3)
            return DataCache.GetStaticConfigValue(AppStaticConfigs.MAIN_CHARACTOR_DISPOSABLE_POINT_PLAN_THREE_LEVEL, 90);
        return 0;
    }

    public int GetActivedAddPointPlanId()
    {
        return _playerPropertyInfo.playerDto.pointPlan;
    }

    /**public void UpdateActivedAddPointPlanId(int newPlanId,PlayerPropertyInfo newPropertyInfo){
        _playerPropertyInfo.playerDto.pointPlan = newPlanId;
        _playerPropertyInfo.playerDto.changeTimes += 1;

        _playerPropertyInfo.ResetPlayerInfo(newPropertyInfo);
        CheckPlayerHasPotentialPoint();
           GameEventCenter.SendEvent(GameEvent.Player_OnPlayerPropertyUpdate);
    }*/

    public void ResetAddPointPlanChangeTimes()
    {
        if (_playerPropertyInfo != null && null != _playerPropertyInfo.playerDto)
            _playerPropertyInfo.playerDto.changeTimes = 0;
    }

    public void CheckPlayerHasPotentialPoint()
    {
        GameDebuger.TODO(@"if(_playerPropertyInfo.playerDto.potential >= 1)
            PromoteManager.Instance.SetState(PromoteType.PLAYERADDPOINT,true);
        else
            PromoteManager.Instance.SetState(PromoteType.PLAYERADDPOINT,false);        
");
    }

    #endregion

    #region 挂机

    //	private DoubleExpStateBarDto _doubleExpStateDto;
    /**public void SetupDoubleExpDto(DoubleExpStateBarDto dto){
    //      _doubleExpStateDto = dto;
        CheckOutDoubleExp();
    }*/

    /**public DoubleExpStateBarDto GetDoubleExpDto(){
        return _doubleExpStateDto;
    }*/

    /**public void UpdateDoubleExpDto(DoubleExpDto expDto){
        //  有可能DoubleExpStateBarDto数据由于延迟慢于expDto通知，导致异常
        if (_doubleExpStateDto == null) return;

        _doubleExpStateDto.openPoint = expDto.openPoint;
        _doubleExpStateDto.point = expDto.point;

           GameDebuger.TODO(@"GameEventCenter.SendEvent(GameEvent.Player_OnOpenDoublePointChanged); ");

        CheckOutDoubleExp();
    }*/

    public bool CheckOutDoubleExp()
    {
        GameDebuger.TODO(@"
if(_doubleExpStateDto != null && _doubleExpStateDto.openPoint > 0){
            ModelManager.PlayerBuff.ToggleDoubleExpBuffTip(true);
            return true;
        }
        ModelManager.PlayerBuff.ToggleDoubleExpBuffTip(false);
        return false;        
");
        return false;
    }

    public void FreezeDoubleExp()
    {
        GameDebuger.TODO(@"
if (!FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum_OnhookDoublePoint))
        {
            return;
        }

        if(_doubleExpStateDto != null && _doubleExpStateDto.openPoint == 0)
        {
            TipManager.AddTip(""当前双倍点数为0"");
            return;
        }
        
        ServiceRequestAction.requestServer(PatrolService.freezeDoubleExp(),""FreezeDoubleExp"", 
        (e) => {
            DoubleExpDto expDto = e as DoubleExpDto;
            TipManager.AddTip(""当前双倍点数已结算到本周剩余双倍点数"");
            UpdateDoubleExpDto(expDto);
        });        
");
    }

    public void ReceiveDoubleExp()
    {
        GameDebuger.TODO(@"if (!FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum_OnhookDoublePoint))
        {
            return;
        }

        if(_doubleExpStateDto != null)
        {
            if(_doubleExpStateDto.openPoint >= DataCache.GetStaticConfigValue(AppStaticConfigs.OPEN_MAX_DOUBLE_EXP_POINT,120)){
                TipManager.AddTip(string.Format(""最多领取{0}点双倍点数"",_doubleExpStateDto.openPoint));
                return;
            }
            else if(_doubleExpStateDto.point == 0){
                TipManager.AddTip(""本周的双倍点数已领完"");
                return;
            }
        }
        
        ServiceRequestAction.requestServer(PatrolService.receiveDoubleExp(),""ReceiveDoubleExp"", 
                                           (e) => {
            DoubleExpDto expDto = e as DoubleExpDto;
            int point = expDto.point;
            if(_doubleExpStateDto != null)
                point = _doubleExpStateDto.point - expDto.point;
            
            TipManager.AddTip(string.Format(""你领取了{0}点双倍点数，快去奋勇杀敌吧！"",point));
            UpdateDoubleExpDto(expDto);
        });        
");
    }

    public bool IsAutoFram
    {
        get;
        set;
    }

    public void StartAutoFram()
    {
        if (JoystickModule.DisableMove)
        {
            GameDebuger.TODO(@"if (ModelManager.BridalSedan.IsMe())
            {
                TipManager.AddTip('你正在乘坐花轿，不能到处乱跑哦！');
            }
            else");
            {
                TipManager.AddTip("你正在组队无法进行挂机操作");
            }
            return;
        }

        IsAutoFram = true;
        HeroView heroView = WorldManager.Instance.GetHeroView();
        if (heroView)
        {
            heroView.SetAutoFram(true);
            heroView.SetPatrolFlag(true);
        }
        GameDebuger.TODO(@"ServiceRequestAction.requestServer(SceneService.patrolBegin());");
    }

    public void StopAutoFram(bool needStop = false)
    {
        if (IsAutoFram)
        {
            IsAutoFram = false;
            HeroView heroView = WorldManager.Instance.GetHeroView();
            if (heroView)
            {
                heroView.SetAutoFram(false);
                heroView.SetPatrolFlag(false);
                if (needStop)
                {
                    heroView.StopAndIdle();
                }
            }
            GameDebuger.TODO("ServiceRequestAction.requestServer(SceneService.patrolEnd());");
        }
    }

    #endregion

    #region 自动寻路

    public void StartAutoNav()
    {
        GameDebuger.TODO(@"
if (ModelManager.Team.IsFollowLeader()) {
            return;
        }        
");

        HeroView heroView = WorldManager.Instance.GetHeroView();
        if (heroView)
        {
            heroView.SetNavFlag(true);
        }
    }

    public void StopAutoNav()
    {
        if (WorldManager.Instance == null)      //队员身份下登录的时候会报错,WorldManager.Instance为null
            return;

        WorldManager.Instance.CleanTargetNpc();

        if (IsAutoFram == false)
        {
            HeroView heroView = WorldManager.Instance.GetHeroView();
            if (heroView)
            {
                heroView.SetNavFlag(false);
            }
        }
    }

    #endregion

    public void StopAutoRun()
    {
        if (IsAutoFram)
        {
            StopAutoFram(true);
        }
        else
        {
            StopAutoNav();
			
            HeroView heroView = WorldManager.Instance.GetHeroView();
            if (heroView != null)
            {
                heroView.StopAndIdle();
            }
        }
    }

    #region 玩家称号

    //	private Dictionary<int,PlayerTitleDto> _titleDtoDic;
    //	public string MasterName;//老师的名字

    /**public List<PlayerTitleDto> GetTitleList ()
    {
        return new List<PlayerTitleDto>(_titleDtoDic.Values);
    }*/

    /**private void SetupTitleInfo (List<PlayerTitleDto> titleDtoList)
    {
        _titleDtoDic = new Dictionary<int, PlayerTitleDto> (titleDtoList.Count);

        for (int i = 0; i < titleDtoList.Count; ++i) {
            PlayerTitleDto titleDto = titleDtoList [i];
               string fereName = "";
           if (ModelManager.Marry.MyMarryInfoDto != null && ModelManager.Marry.MyMarryInfoDto.fereInfo!=null)
                    fereName =  ModelManager.Marry.MyMarryInfoDto.fereInfo.name;
               titleDto.titleName = titleDto.titleName.Replace ("{factionName}", _playerDto.faction.shortName).Replace("{fereName}", fereName).Replace("{masterName}", MasterName);            

               _titleDtoDic.Add(titleDtoList[i].titleId, titleDtoList[i]);
           }
    }*/


    /**public void UpdateTitleInfo(int titileId,string titleName)
    {
        if (_titleDtoDic == null) return;
        if (_titleDtoDic.ContainsKey(titileId))
        {
            _titleDtoDic[titileId].titleName = titleName;
        }
    }*/

    //    public void UpdateTitleInfoByFereRename(PlayerFereRenameNotify notify)
    //    {
    //        foreach (var title in _titleDtoDic.Values)
    //        {
    //            title.titleName = title.titleName.Replace(notify.fereBeforeName, notify.fereNowName);
    //        }
    //        WorldManager.Instance.GetModel().HandlePlayerTitleChangeRefresh(notify); 
    //    }

    //	public void UpdateTitleInfoByMasterRename(MasterChangeNickameNotify notify)
    //	{
    //		Dictionary<int,PlayerTitleDto>.Enumerator tEnum = _titleDtoDic.GetEnumerator ();
    //		PlayerTitleDto tPlayerTitleDto;
    //		string tTitleName;
    //		string tOldTitle = notify.beforeName + "的徒弟";
    //		string tNewTitle = notify.nickname + "的徒弟";
    //		while(tEnum.MoveNext())
    //		{
    //			tPlayerTitleDto = tEnum.Current.Value;
    //			if(null != tPlayerTitleDto )
    //			{
    //				tTitleName = tPlayerTitleDto.titleName;
    //				if (!string.IsNullOrEmpty (tTitleName)) {
    //					if (tTitleName == tOldTitle)
    //						tPlayerTitleDto.titleName = tNewTitle;
    //				}
    //			}
    //		}
    //		WorldManager.Instance.GetModel().HandlePlayerTitleChangeRefresh(notify.playerId,notify.beforeName, notify.nickname); 
    //	}

    /**public void GainNewTitle(PlayerTitleDto titleDto){
           if (titleDto == null || _titleDtoDic == null)
               return;
                   
                   titleDto.titleName = titleDto.titleName.Replace (""{factionName}"", _playerDto.faction.shortName).Replace (""{masterName}"",MasterName);
                   string fereName = """";
                   if (ModelManager.Marry.MyMarryInfoDto != null && ModelManager.Marry.MyMarryInfoDto.fereInfo != null)
                       fereName = ModelManager.Marry.MyMarryInfoDto.fereInfo.name;
                   if (!string.IsNullOrEmpty(fereName))
                   {
                       titleDto.titleName = titleDto.titleName.Replace(""{fereName}"", fereName);
                   }
                   if (_titleDtoDic.ContainsKey (titleDto.titleId)) {
                       _titleDtoDic [titleDto.titleId] = titleDto;
                   } else {
                       _titleDtoDic.Add (titleDto.titleId,titleDto);
                   }

                   GameEventCenter.SendEvent(GameEvent.Player_OnTitleListUpdate);
    }*/

    /**public void RemoveTile(int titleId){

        if (_titleDtoDic.Remove (titleId)) {
               GameEventCenter.SendEvent(GameEvent.Player_OnTitleListUpdate);
        }
    }*/

    //	public void UpdateTitleInfo(PlayerTitleNotify notify){
    //if(_playerDto != null){
    //            _playerDto.titleId = notify.titleId;
    //
    //            if (string.IsNullOrEmpty (notify.titleName))
    //                TipManager.AddTip ("你取消了称谓的显示");
    //            else {
    //                PlayerTitleDto titleDto = null;
    //                if (_titleDtoDic.TryGetValue (notify.titleId, out titleDto))
    //                {
    //                    if (ModelManager.Marry.MyMarryInfoDto != null && ModelManager.Marry.MyMarryInfoDto.fereInfo!=null && !string.IsNullOrEmpty(ModelManager.Marry.MyMarryInfoDto.fereInfo.name))
    //                    {
    //                        string fereName = null;
    //                        if (ModelManager.Marry.MyMarryInfoDto != null && ModelManager.Marry.MyMarryInfoDto.fereInfo != null)
    //                        {
    //                            fereName = ModelManager.Marry.MyMarryInfoDto.fereInfo.name;
    //                            UpdateTitleInfo(notify.titleId, titleDto.titleName.Replace("{fereName}", fereName));
    //                        }
    //
    //                        if (!string.IsNullOrEmpty(fereName))
    //                        {
    //                           ScenePlayerDto player = WorldManager.Instance.GetModel().GetPlayerDto(GetPlayerId());
    //                            if (player != null)
    //                            {
    //                                player.fereName = fereName;
    //                            }
    //                            if (ModelManager.Marry.IsIMarry(GetPlayerId()))
    //                            {
    //                                TipManager.AddTip(
    //                                    string.Format("当前称谓设置为{0}",
    //                                        titleDto.titleName.Replace("{fereName}", fereName).WrapColor(ColorConstant.Color_Title_Str)),
    //                                    false, true);
    //                            }
    //                            else
    //                            {
    //                                TipManager.AddTip(
    //                                    string.Format("当前称谓设置为{0}",
    //                                        titleDto.titleName.Replace("{fereName}", fereName).WrapColor(ColorConstant.Color_Title_Str)));
    //                            }
    //                            
    //                        }  
    //                    }
    //                    else
    //                    {
    //                        UpdateMasterName(MasterName);
    //                        TipManager.AddTip(string.Format("当前称谓设置为{0}", titleDto.titleName.WrapColor(ColorConstant.Color_Title_Str)));
    //                    }    
    //                } else {
    //                    Debug.LogError (string.Format("该玩家没有titleId:{0} 的数据，请检查是否服务器下发异常",notify.titleId));
    //                }
    //            }
    //
    //            GameEventCenter.SendEvent(GameEvent.Player_OnPlayerTitleUpdate);
    //        }       
    //	}

    /**public string GetTitleName(){
    if(_titleDtoDic.ContainsKey(_playerDto.titleId))
            return _titleDtoDic[_playerDto.titleId].titleName;
        else
            return "无"; 
        return string.Empty;
    }*/

    /**public void UpdateMasterName(string pMasterName)
    {
        if (null != _playerDto) {
            MasterName = pMasterName;
            UpdateTitleInfo ("{masterName}",pMasterName);
        }
    }*/

    /// <summary>
    /// 更新头衔值
    /// </summary>
    /// <param name="pTitleMask">P title mask. 如：{masterName}</param>
    /// <param name="pTitleValue">P title value.如：玩家名</param>
    /**public void UpdateTitleInfo(string pTitleMask,string pTitleValue)
    {
        if (null != _titleDtoDic && _titleDtoDic.Count > 0) {
            Dictionary<int,PlayerTitleDto>.Enumerator tEnum = _titleDtoDic.GetEnumerator ();
            PlayerTitleDto tPlayerTitleDto;
            while(tEnum.MoveNext())
            {
                tPlayerTitleDto = tEnum.Current.Value;
                if (null != tPlayerTitleDto && tPlayerTitleDto.titleName.IndexOf (pTitleMask) != -1) {
                    tPlayerTitleDto.titleName = tPlayerTitleDto.titleName.Replace (pTitleMask,pTitleValue);
                }
            }
        }
    }*/
    #endregion

	#region 功德值
//	private int _merits;
//	public int Merits {
//		get {
//			return _merits;
//		}
//	}
//
//	public void UpdateMerits(int newMertis){
//		int changeMerits = newMertis -_merits;
//		_merits = newMertis;
//		if (changeMerits < 0) {
//			TipManager.AddGainCurrencyTip(changeMerits, "功德值");
//		} else if(changeMerits > 0) {
//			TipManager.AddGainCurrencyTip(changeMerits, "功德值");
//		}
//	}
	#endregion

	#region 玩家染色
	private int _transformModelId = 0;

    public int TransformModelId
    {
        get
        {
            return _transformModelId;
        }
    }

    public void UpdateTransformModelId(int modelId)
    {
        _transformModelId = modelId;
        GameDebuger.TODO(@"
        GameEventCenter.SendEvent(GameEvent.Player_OnModelChange);
");
    }

    //从静态表中筛选出当前角色各部位配色方案
    /**public Dictionary<int,List<Dye>> GetColorScheme(){
        Dictionary<int,Dye> dyeInfoDic = DataCache.getDicByCls<Dye>();
        if(dyeInfoDic == null){
            Debug.LogError("dyeInfoDic is null");
            return null;
        }

        Dictionary<int,List<Dye>> result = new Dictionary<int, List<Dye>>(3);
        result.Add(Dye.DyePartTypeEnum_Hair,new List<Dye>(10));
        result.Add(Dye.DyePartTypeEnum_Clothes,new List<Dye>(10));
        result.Add(Dye.DyePartTypeEnum_Ornaments,new List<Dye>(10));

        foreach(Dye dye in dyeInfoDic.Values){
            if(dye.charactorId == _playerDto.charactorId){
                result[dye.dyePartType].Add(dye);
            }
           }

        return result;
    }*/

    public static string GetDyeColorParams(PlayerDressInfoDto dressInfo){
        if(dressInfo == null)
            return "";

        return GetDyeColorParams(dressInfo.hairDyeId,dressInfo.dressDyeId,dressInfo.accoutermentDyeId);
    }

    public static string GetDyeColorParams(PlayerDressInfo dressInfo)
    {
        if (dressInfo == null)
            return "";
		
        return GetDyeColorParams(dressInfo.hairDyeId, dressInfo.dressDyeId, dressInfo.accoutermentDyeId);
    }

    public static string GetDyeColorParams(int hairId, int clothId, int decorationId)
    {
        if (hairId == 0 && clothId == 0 && decorationId == 0)
            return "";

        string[] colorParams = new string[3];
        int[] colorSchemeIds = new int[3]{ hairId, clothId, decorationId };

        for (int i = 0; i < colorParams.Length; ++i)
        {
            GameDebuger.TODO(@"if(colorSchemeIds[i] != 0)
                colorParams[i] = DataCache.getDtoByCls<Dye>(colorSchemeIds[i]).colorStr;
            else");
            colorParams[i] = "0,1,1,0";
        }

        return string.Join(";", colorParams);
    }

    public int GetPartDyeId(int dyePartType)
    {
        GameDebuger.TODO(@"
if(dyePartType == Dye.DyePartTypeEnum_Hair)
            return _playerDto.dressInfoDto.hairDyeId;
        else if(dyePartType == Dye.DyePartTypeEnum_Clothes)
            return _playerDto.dressInfoDto.dressDyeId;
        else if(dyePartType == Dye.DyePartTypeEnum_Ornaments)
            return _playerDto.dressInfoDto.accoutermentDyeId;
       
");
        return 0; 
    }

    public void UpdateDyeIds(int hairId, int clothId, int decorationId)
    {
        GameDebuger.TODO(@"
ServiceRequestAction.requestServer(PlayerService.dye(clothId,hairId,decorationId));
        _playerDto.dressInfoDto.hairDyeId = hairId;
        _playerDto.dressInfoDto.dressDyeId = clothId;
        _playerDto.dressInfoDto.accoutermentDyeId = decorationId;        
");
    }

    #endregion

    #region 帮派信息

    //    /** 帮派信息 */
    //    private PersonalGuildSimpleInfoDto _guildInfo;
    //
    //    public bool HasGuild()
    //    {
    //        return _guildInfo != null && _guildInfo.guildId != 0 && !string.IsNullOrEmpty(_guildInfo.guildName);
    //    }
    //
    //    public string GetGuildName()
    //    {
    //        if (_guildInfo == null)
    //            return "";
    //        return _guildInfo.guildName;
    //    }
    //
    //    public long GetGuildId()
    //    {
    //        if (_guildInfo != null)
    //            return _guildInfo.guildId;
    //        else
    //            return 0;
    //    }
    //
    //    public string GetMyPosition()
    //    {
    //        return _guildInfo.position.name;
    //    }
    //
    //    public int GetMyPositionId()
    //    {
    //        return _guildInfo.positionId;
    //    }
    //
    //    public int GetLastPositionId()
    //    {
    //        return _guildInfo.lastPositionId;
    //    }
    //
    //    public void SetMyPosiotion(int posiontion)
    //    {
    //        if (_guildInfo != null)
    //        {
    //            _guildInfo.positionId = posiontion;
    //        }
    //    }
    //
    //    public int GetGuildGrade()
    //    {
    //        return _guildInfo.guildGrade;
    //    }
    //
    //    public bool GetGuildSiegeBattle()
    //    {
    //        return _guildInfo.inSiegeBattle;
    //    }
    //
    //    public void UpDateGuild(GuildDto dto)
    //    {
    //        if(_guildInfo == null)
    //        {
    //            _guildInfo = new PersonalGuildSimpleInfoDto();
    //        }
    //
    //        if(dto != null)
    //        {
    //            _guildInfo.guildName = dto.name;
    //            _guildInfo.guildId = dto.id;
    //            _guildInfo.position = null;
    //            GameDebuger.TODO(@"
    // _guildInfo.positionId = ModelManager.Guild.GetSelf().position;
    //            _guildInfo.lastPositionId = ModelManager.Guild.GetSelf().lastPositionId;
    //");
    //            _guildInfo.guildGrade = dto.grade;
    //        }
    //        else
    //        {
    //            _guildInfo.guildName = "";
    //            _guildInfo.guildId = 0;
    //            _guildInfo.position = null;
    //            _guildInfo.positionId = 0;
    //            _guildInfo.lastPositionId = 0;
    //            _guildInfo.guildGrade = 0;
    //        }
    //
    //        GameDebuger.TODO("GameEventCenter.SendEvent(GameEvent.Player_OnGuildStateChange);");
    //    }
    //
    //    public void UpdateGuildSiegeBattleState()
    //    {
    //        if (_guildInfo != null)
    //        {
    //            _guildInfo.inSiegeBattle = true;
    //        }
    //    }

    #endregion

    #region 节日仙子数据

    //    private FestivalDto _festivalDto;
    //	public FestivalDto festivalDto{
    //		get{ return _festivalDto ;}
    //	}
    //	private FestivalReward _festivalReward;
    //	public FestivalReward festivalReward{
    //		get{ return _festivalReward ;}
    //	}
    //
    //	public void GetFestivalInfoFromServer(System.Action onSuccess = null){
    //		if(_festivalDto != null){
    //			if(onSuccess != null)
    //				onSuccess();
    //		}else{
    //ServiceRequestAction.requestServer (FestivalService.festivalInfo(),""getFestivalInfo"",(e)=>{
    //                _festivalDto = e as FestivalDto;
    //                int tIndex = _festivalDto.todayFestival? _festivalDto.currentFestivalId : _festivalDto.nextFestivalId;
    //                _festivalReward = DataCache.getDtoByCls<FestivalReward>(tIndex);
    //
    //                if(onSuccess != null)
    //                    onSuccess();
    //            });
    //		}
    //	}

    //	public FestivalReward GetNextFestivalInfo() {
    //		return DataCache.getDtoByCls<FestivalReward>(_festivalDto.nextFestivalId);
    //	}
    //
    //	public FestivalReward GetNextTwoFestivalInfo() {
    //		return DataCache.getDtoByCls<FestivalReward>(GetNextFestivalInfo().nextFestivalId);
    //	}
    //
    //	public FestivalReward GetNextThreeFestivalInfo() {
    //		return DataCache.getDtoByCls<FestivalReward>(GetNextTwoFestivalInfo().nextFestivalId);
    //	}

    #endregion

    #region 人物评分

    //    public void UpDateScore(int score)
    //    {
    //_playerDto.score = score;
    //        GameEventCenter.SendEvent(GameEvent.Player_OnScoreUpDate);
    //    }

    #endregion

    #region 挂机默认技能

    /** 主角默认技能 */
    private int _playerDefaultSkillId;

    /** 宠物默认技能 */
    private int _petDefaultSkillId;

    public int GetPlayerDefaultSkillId()
    {
        return _playerDefaultSkillId;
    }

    public void SetPlayerDefaultSkillId(int skillId)
    {
        _playerDefaultSkillId = skillId;
    }

    public int GetPetDefaultSkillId()
    {
        return _petDefaultSkillId;
    }

    public void SetPetDefaultSkillId(int skillId)
    {
        _petDefaultSkillId = skillId;
    }

    #endregion

    #region 使用物品数量显示


    //    private List<ItemDto> _showItemsCount;
    //    public int GetShowItemCount(int itemId)
    //    {
    //        if(_showItemsCount != null && _showItemsCount.Count > 0)
    //        {
    //            for (int index = 0; index < _showItemsCount.Count; index++)
    //            {
    //                if (_showItemsCount[index].itemId == itemId)
    //                    return _showItemsCount[index].itemCount;
    //            }
    //        }
    //        return 0;
    //    }

    //    public void SetShowItemCount(int itemId, int count)
    //    {
    //        if (_showItemsCount != null && _showItemsCount.Count > 0)
    //        {
    //            for (int index = 0; index < _showItemsCount.Count; index++)
    //            {
    //                if (_showItemsCount[index].itemId == itemId)
    //                {
    //                    _showItemsCount[index].itemCount = count;
    //                GameEventCenter.SendEvent(GameEvent.Player_OnShowItemsCountUpDate);
    //                    return;
    //                }
    //            }
    //        }
    //
    //        if (_showItemsCount == null)
    //            _showItemsCount = new List<ItemDto>();
    //
    //        ItemDto dto = new ItemDto();
    //        dto.itemId = itemId;
    //        dto.itemCount = count;
    //        _showItemsCount.Add(dto);
    //
    //        GameEventCenter.SendEvent(GameEvent.Player_OnShowItemsCountUpDate);
    //    }

    #endregion

    #region 是否卡等级

    private bool _stopUpgrade;

    public bool StopUpgrade
    {
        get
        {
            return _stopUpgrade;
        }
        set
        {
            _stopUpgrade = value;
        }
    }

    #endregion

    #region 飞升相关

    public bool NeedShowFlyTip
    {
        get
        {
            int tFlyGrade = DataCache.GetStaticConfigValue(AppStaticConfigs.FLY_MISSION_MIN_GRADE);
            if (GetPlayerLevel() >= tFlyGrade)
            {
                int tFlyMissionId = DataCache.GetStaticConfigValue(AppStaticConfigs.FLY_MISSION);
                GameDebuger.TODO(@"MissionDataModel.MissionSta tMissionSta = ModelManager.MissionData.GetMissionStaByID(tFlyMissionId);
                if (tMissionSta != MissionDataModel.MissionSta.FINISHED && tMissionSta != MissionDataModel.MissionSta.ACCEPTED_F)
                {
                    return true;
                }");
            }
            return false;
        }
    }

    #endregion

    #region 是否封停模式

    public void UpdatePlayerBanTime(long banExpieAt)
    {
        if (_playerDto != null)
        {
            GameDebuger.TODO(@"
_playerDto.banExpireAt = banExpieAt;            
");
        }
    }

    //玩家是否处于封停模式
    public bool IsPlayerBandMode(bool tip = true)
    {
        GameDebuger.TODO(@"if (_playerDto == null || _playerDto.banExpireAt == 0)
        {
            return false;
        }
        else
        {
            if (tip)
            {
                long leftTime = (_playerDto.banExpireAt - SystemTimeManager.Instance.GetUTCTimeStamp())/1000;
                TipManager.AddTip(string.Format(""由于你违反用户协议需要在此地闭关修炼，剩余时间{0}"", DateUtil.GetMinutes(leftTime)));
            }
            return true;
        }        
");
        return false;
    }

    #endregion


    #region 是否有经验玩家

    public int experienceType
    {
        get
        {
            GameDebuger.TODO(@"
return _playerDto.experienceType;            
");
            return 0;
        }

        set
        {
            GameDebuger.TODO(@"_playerDto.experienceType = value; ");
        }
    }

    #endregion

    #region 转换门派/宝石

    //下次转换门派的时间点
    private long mNextChangeFactionTime;

    public long NextChangeFactionTime
    {
        get { return mNextChangeFactionTime; }
        set { mNextChangeFactionTime = value; }
    }

    //转换宝石结束时间
    private long mChangeEmbedEndTime;

    public long ChangeEmbedEndTime
    {
        get { return mChangeEmbedEndTime; }
        set { mChangeEmbedEndTime = value; }
    }

    #endregion

    #region 钱币是否足够

    public bool EnoughVirtualCurrency(AppVirtualItem.VirtualItemEnum type, int number)
    {
        return GetPlayerWealth(type) > number;
    }
    #endregion

    #region 答题
    public bool GetDailyQuestion { get { return _dailyQuestion; } }
    #endregion
    public MainCrewInfoNotify GetCrewInfoDto()
    {
        return _crewInfoDto;
    }
    public void ApResetSucess()
    {
        GameDebuger.TODO(@"GameEventCenter.SendEvent(GameEvent.Player_OnApReset);");
    }

    // 地理位置开关
    public bool closedLocation
    {
        get
        {
            if (_playerDto != null)
            {
                GameDebuger.TODO(@"
return _playerDto.closedLocation;                
");
                return false;
            }
            else
                return true;
        }

        set
        {
            GameDebuger.TODO(@"
ServiceRequestAction.requestServer(PlayerService.closeLocation(value), """", (e) =>
            {
                if(_playerDto!=null)
                _playerDto.closedLocation = value;
            });            
");
        }
    }

    //    public void HandleSpellTotalNotify(SpellTotalNotify expNotify)
    //    {
    //        _totalSpellExp = expNotify.exp;
    //        GameEventCenter.SendEvent(GameEvent.Player_SpellTotalChange);
    //    }
    public ModelStyleInfo GetMemberByFormationPos(int posKey)
    {
        throw new NotImplementedException();
    }

    public bool HasGuild()
    {
        return false;
    }

    public void FireData()
    {
        stream.OnNext(this);
    }
}