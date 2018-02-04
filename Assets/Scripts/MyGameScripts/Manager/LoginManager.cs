using System;
using AppDto;
using AppServices;
using UnityEngine;
using GamePlot;

/* ................................................... 
*                       _oo0oo_ 
*                      o8888888o 
*                      88" . "88 
*                      (| -_- |) 
*                      0\  =  /0 
*                    ___/`---'\___ 
*                  .' \\|     |// '. 
*                 / \\|||  :  |||// \ 
*                / _||||| -Öd-|||||- \ 
*               |   | \\\  -  /// |   | 
*               | \_|  ''\---/''  |_/ | 
*               \  .-\__  '-'  ___/-. / 
*             ___'. .'  /--.--\  `. .'___ 
*          ."" '<  `.___\_<|>_/___.' >' "". 
*         | | :  `- \`.;`\ _ /`;.`/ - ` : | | 
*         \  \ `_.   \_ __\ /__ _/   .-` /  / 
*     =====`-.____`.___ \_____/___.-`___.-'===== 
*                       `=---=' 
*                        
*..................佛祖保佑‚ ,永无BUG................... 
*  
*/



public class LoginManager
{
    public static LoginManager _instance;

    public static LoginManager Instance
    {
        get {
            if (_instance == null)
            {
                _instance = new LoginManager();
                Asyn.AsynInitManager.OnAllComplete += _instance.OnLoginDataLoadingFinish;
            }
            return _instance; 
        }
    }

    public void Dispose(){
        Asyn.AsynInitManager.OnAllComplete -= _instance.OnLoginDataLoadingFinish;
    }
    /**
     *登录管理器
     * @author senkay
     * @date Nov 6, 2010
     */
    public static readonly string ERROR_time_out = "链接超时";
    public static readonly string ERROR_socket_error = "网络错误";
    public static readonly string ERROR_socket_close = "网络已断开";
    public static readonly string ERROR_sid_error = "用户账号错误";
    public static readonly string ERROR_user_invalid = "用户无效";

    //private static readonly int MAX_TRY_COUNT = 5;

    //leave state
    public static uint LeaveState = 0;
    //close state
    public static uint CloseState = 0;

    private AccountPlayerDto _accountPlayerDto;

    public PayExtInfo PayExtInfo;

    private bool _createRole;

    private bool _afterLogin;

    private bool _keepSocket;

    private PlayerDto _playerDto;

    private bool _reLogin;

    //	public delegate void CallOnTokenNotExist();
    //	public CallOnTokenNotExist callOnTokenNotExist;

    private GameServerInfo _serverInfo;

    //public List<ServiceInfo> serviceInfoList = new List<ServiceInfo>();

    public string Token { get; private set; }

    public string LoginId { get; set; }

    public uint HaState { get; private set; }

    public bool KeepSocket
    {
        get { return _keepSocket; }
    }

    private LoginManager()
    {
        _keepSocket = true;
    }

    public event Action<string> OnLoginMessage;
    public event Action<float> OnLoginProcess;
    //public event Action<LoginQueuePlayerDto> OnWaitForLoginQueue;

    //断线重连成功后的回调//
    public event Action OnReloginSuccess;

    public void start(string token, GameServerInfo serverInfo, AccountPlayerDto accountPlayerDto)
    {
        _reLogin = false;
        HaState = HaStage.CONNECTED;

        _serverInfo = serverInfo;
        _accountPlayerDto = accountPlayerDto;
        _playerDto = null;
        _createRole = false;
        _afterLogin = false;

        SPSdkManager.Instance.OnLoginSuccess += OnLoginSuccess;
        SPSdkManager.Instance.OnLogoutNotify += OnLogout;

        //        if (ServiceProviderManager.HasSP())
        //        {
        //            _token = null;
        //        }
        //        else
        //        {
        Token = token;
        //        }

        if (GameSetting.GMMode)
        {
            if (GameDebuger.Debug_PlayerId != 0)
            {
                _accountPlayerDto = new AccountPlayerDto();
                _accountPlayerDto.nickname = GameDebuger.Debug_PlayerId.ToString();
                _accountPlayerDto.id = GameDebuger.Debug_PlayerId;
                _accountPlayerDto.gameServerId = 0;
            }
        }

        if (!DataManager.AllDataLoadFinish)
        {
            UpdateStaticData();
        }
        else
        {
            DataLoadingMsgProcess(1f);
            ConnectSocket();
        }
    }

    private void ConnectSocket()
    {
        ProxyLoginModule.Show();

        ServiceRequestActionMgr.Setup();

        SocketManager.Instance.Setup();
        SocketManager.Instance.OnHAConnected += HandleOnHAConnected;
        SocketManager.Instance.OnHaError += HandleOnHaError;
        SocketManager.Instance.OnHaCloseed += HandleOnHaCloseed;
        SocketManager.Instance.OnStateEvent += HandleOnStateEvent;

        GameDebuger.Log("Login With " + Token + " At " + _serverInfo.host + ":" + _serverInfo.port + " accessId=" +
            _serverInfo.serviceId + " gameServerId=" + _serverInfo.serverId);
        PrintLog("连接服务器...");

        TalkingDataHelper.OnEventSetp("GameLogin/ConnectSocket"); //连接服务器

        SPSdkManager.Instance.CYEnterServer(_serverInfo.serverId, _serverInfo.name);

        Connect();
    }

    private void HandleOnStateEvent(uint state)
    {
        HaState = state;
        if (HaState == HaStage.LOGINED && _playerDto != null && DataManager.AllDataLoadFinish)
        {
            DoLogin(_playerDto);
        }
    }

    private void HandleOnHAConnected()
    {
        GameDebuger.Log("OnHAConnected");
        TalkingDataHelper.OnEventSetp("GameLogin/HandleOnHAConnected"); //连接服务器成功

        ShowMessageBox("账号验证中，请稍候...");

        if (_reLogin)
        {
            _playerDto = null;
        }

        OnRequestTokenCallback(Token, "");

        ExitGameScript.CheckConnected = true;
        ExitGameScript.NeedReturnToLogin = false;
        ExitGameScript.WaitForReConnect = false;
    }

    private void HandleOnHaError(string msg)
    {
        Destroy();

        LayerManager.Instance.LockUICamera(false);

        ProxyWindowModule.OpenSimpleMessageWindow(msg, delegate
            {
                GotoLoginScene();
            }, UIWidget.Pivot.Left, null,
            UILayerType.TopDialogue);

        ShowMessageBox(msg);
    }

    private void HandleOnHaCloseed(uint status)
    {
        LayerManager.Instance.LockUICamera(false);
        GameCheatManager.Instance.Dispose();

        if (LayerManager.Instance.CurUIMode == UIMode.NULL)
        {
            ExitGameScript.OpenReloginTipWindow(string.Format("网络中断, 请重新进入游戏[{0}]", status));
        }
        else
        {
            ExitGameScript.CheckConnected = true;
        }
    }

    private void Connect()
    {
        SocketManager.Instance.Connect(_serverInfo);
    }

    public void OnRequestTokenCallback(string token, string errorMsg)
    {
        Token = token;
        if (Token == null)
        {
            ShowMessageBox("账号验证失败:" + errorMsg);

            LayerManager.Instance.LockUICamera(false);

            ProxyWindowModule.OpenSimpleMessageWindow("账号验证失败:" + errorMsg, delegate
                {
                    GotoLoginScene();
                },
                UIWidget.Pivot.Left, null, UILayerType.TopDialogue);
        }
        else
        {
            if (ProxyRoleCreateModule.IsOpen())
            {
                LayerManager.Instance.LockUICamera(false);
                Login();
            }
            else
            {
                if (_accountPlayerDto == null)
                {
                    RequestLoadingTip.Reset();
                    ProxyLoginModule.Hide();
                    ProxyRoleCreateModule.Open(_serverInfo, CreatePlayerSuccess);
                }
                else
                {
                    Login();
                }
            }
        }
    }

    //private void __OnHATimeOut()
    //{
    //    if (reTryConnect() == false)
    //    {
    //        showMessageBox(ERROR_time_out);
    //    }
    //}

    //private void __OnSocketErr()
    //{
    //    if (reTryConnect() == false)
    //    {
    //        showMessageBox(ERROR_socket_error);
    //        ExitGameScript.CheckReloginWhenConnectClose(ExitGameScript.CheckConnected == false);
    //    }
    //}

    //private void __OnSocketClose()
    //{
    //    if (reTryConnect() == false)
    //    {
    //        showMessageBox(ERROR_socket_close);
    //        ExitGameScript.CheckReloginWhenConnectClose(ExitGameScript.CheckConnected == false);
    //    }
    //}

    //private void __OnSidError()
    //{
    //    showMessageBox(ERROR_sid_error);
    //}

    //private bool reTryConnect()
    //{
    //    _tryCount--;
    //    if (_tryCount < 0)
    //    {
    //        return false;
    //    }
    //    else
    //    {
    //        return true;
    //    }
    //}

    public bool SupportRelogin()
    {
        return _serverInfo != null && SocketManager.Instance.IsSetup();
    }

    public void ReConnect()
    {
        if (_serverInfo != null)
        {
            _reLogin = true;
            _afterLogin = false;

            LayerManager.Instance.LockUICamera(true);

            Connect();
        }
    }

    //private void FetchServerInfo(ArrayList _serviceInfoList)
    //{
    //	if (serviceInfoList.Count > 0)
    //		serviceInfoList.Clear();

    //	foreach (ServiceInfo serviceInfo in _serviceInfoList)
    //	{
    //		serviceInfoList.Add(serviceInfo);
    //		GameDebuger.Log(serviceInfo.id + " " + serviceInfo.name + " " + serviceInfo.doc);
    //	}
    //}

    private void Login()
    {
        if (Token != null)
        {
            if (HaState == HaStage.LOGINED && _playerDto != null)
            {
                DoLogin(_playerDto);
            }
            else
            {
                PrintLog("账号登录...");
                string ip = HaApplicationContext.getConfiguration().getLocalIp().Trim();

                GameDebuger.Log("LoginFromIp = " + ip);

                if (_accountPlayerDto != null)
                {
                    TalkingDataHelper.OnEventSetp("GameLogin/ReqeustLogin"); //请求登陆
                    ServiceRequestAction.requestServer(
                        Services.Login_Player(Token, ip,
                            _accountPlayerDto.id, BaoyugameSdk.getUUID()), "账号登录",
                        OnLogin, OnNotLogin);
                }
                else
                {
                    if (_reLogin)
                    {
                        RequestLoadingTip.Reset();
                        CallBackReLogin();
                    }
                }
            }
        }
    }

    private void CreatePlayerSuccess(GeneralResponse e)
    {
        ProxyLoginModule.Show();
        _createRole = true;
        OnLogin(e);
    }

    private void OnNotLogin(ErrorResponse e)
    {
        GameDebuger.Log("OnNotLogin: ErrorResponse Message:" + e.message);
        PrintLog("登录失败！");

        if (e.id == 19)
        {
            //会话ID失效
            ServiceProviderManager.RequestSsoAccountLogin(ServerManager.Instance.sid, ServerManager.Instance.uid, GameSetting.Channel,
                GameSetting.SubChannel, GameSetting.LoginWay, GameSetting.AppId, GameSetting.PlatformTypeId, SPSDK.deviceId(), SPSDK.channelId(), SPSDK.appName(), SPSDK.appVersionName().ToString(),
                delegate (LoginAccountDto response)
                {
                    if (response != null && response.code == 0)
                    {
                        Token = response.token;
                        Login();
                    }
                    else
                    {
                        string msg = "服务器请求失败，请检查网络";
                        if (response != null)
                        {
                            msg = response.msg;
                        }

                        ExitGameScript.OpenReloginTipWindow(msg, true);
                    }
                });
        }
        else if (e.id == 28)
        {
            //访问受限，白名单
            ExitGameScript.OpenReloginTipWindow(e.message, false);
        }
        else
        {
            //其它错误
            ExitGameScript.OpenReloginTipWindow(e.message, false);
        }
    }

    private void OnLogin(GeneralResponse e)
    {
        if (e is QueueDto)
        {
            var queueDto = (QueueDto)e;

            if (queueDto.index < 0)//不需要排队了
            {
                if (queueDto.playerDto != null)//可以进入
                {
                    SystemTimeManager.Instance.Setup(queueDto.playerDto.gameServerTime);
                    _accountPlayerDto = ServerManager.Instance.AddAccountPlayer(queueDto.playerDto);

                    if (HaState == HaStage.LOGINED)
                    {
                        GameDebuger.Log("登录成功");
                        if (DataManager.AllDataLoadFinish)
                        {
                            DoLogin(queueDto.playerDto);
                        }
                        else
                        {
                            GameDebuger.Log("等待allDataLoadFinish");
                            _playerDto = queueDto.playerDto;
                        }
                    }
                    else
                    {
                        GameDebuger.Log("等待HaStage.LOGINED");
                        _playerDto = queueDto.playerDto;
                    }
                    return;
                }
            }
            else//需要排队
            {
                SystemTimeManager.Instance.Setup(queueDto.playerDto.gameServerTime);
                _accountPlayerDto = ServerManager.Instance.AddAccountPlayer(queueDto.playerDto);
            }
            GameDebuger.Log("登陆排队");
            LoginQueue1((QueueDto)e);
        }
        else if (e is PlayerDto)
        {
            var playerDto = e as PlayerDto;

            SystemTimeManager.Instance.Setup(playerDto.gameServerTime);
            _accountPlayerDto = ServerManager.Instance.AddAccountPlayer(playerDto);

            if (HaState == HaStage.LOGINED)
            {
                GameDebuger.Log("登录成功");
                if (DataManager.AllDataLoadFinish)
                {
                    DoLogin(playerDto);
                }
                else
                {
                    GameDebuger.Log("等待allDataLoadFinish");
                    _playerDto = playerDto;
                }
            }
            else
            {
                GameDebuger.Log("等待HaStage.LOGINED");
                _playerDto = playerDto;
            }
        }
        //        else
        //        {
        //            Debug.LogError("返回的不是QueueDto类型");
        //        }
        //以下注释掉的内容是以前的实现方式--------------------------------

        //        //是否需要排队
        //        if (e is PlayerDto)
        //        {
        //
        //			PlayerDto playerDto = e as PlayerDto;
        //
        //			SystemTimeManager.Instance.Setup(playerDto.gameServerTime);
        //			_accountPlayerDto = ServerManager.Instance.AddAccountPlayer(playerDto);
        //
        //			if (haState == HaStage.LOGINED)
        //			{
        //				GameDebuger.Log("登录成功");
        //				DoLogin(playerDto);
        //			}
        //			else
        //			{	
        //				GameDebuger.Log("等待HaStage.LOGINED");
        //				_playerDto = playerDto;
        //			}
        //        }
        //        else if (e is QueueDto)
        //        {
        //            GameDebuger.LogTime("登陆排队");
        //
        //            
        //            //ExitGameScript.CheckConnected = false;
        //            //destroy(true);
        //
        //            //进行排队操作
        //            if (loginQueue != null)
        //            {
        //                //loginQueue(loginQueueDto);
        //            }
        //            LoginQueue1((QueueDto)e);
        //        }
    }

    private void OnQueueLogin(PlayerDto playerDto)
    {
        SystemTimeManager.Instance.Setup(playerDto.gameServerTime);
        _accountPlayerDto = ServerManager.Instance.AddAccountPlayer(playerDto);
        //        Debug.LogError("haState++++++++++++++" + haState + "----------" + HaStage.LOGINED);
        if (HaState == HaStage.LOGINED)
        {
            GameDebuger.Log("登录成功");
            DoLogin(playerDto);
        }
        else
        {
            GameDebuger.Log("等待HaStage.LOGINED");
            _playerDto = playerDto;
        }
    }


    private void DoLogin(PlayerDto playerDto)
    {
        PlayerPrefs.SetString(GameSetting.LastRolePrefsName, playerDto.id.ToString());

        if (playerDto.sceneId == 0)
        {
            Debug.LogError(string.Format("Error:PlayerDto.sceneId == 0,PlayerDto.sceneId will be set with default value {0}", BattleDemoConfigModel.DEFAULT_SCENE_ID));
            playerDto.sceneId = BattleDemoConfigModel.DEFAULT_SCENE_ID;
        }

        var serverInfo = ServerManager.Instance.GetServerInfo();
        bool newRole = _createRole;
        //判断是否新建角色
        GameDebuger.TODO("if (playerDto.grade == 0 && playerDto.experienceType == PlayerDto.ExperienceTypeEnum_NoSelect)");
        //if (playerDto.grade == 0 /**&& playerDto.experienceType == PlayerDto.ExperienceTypeEnum_NoSelect*/)
        //{
        //    newRole = true;
        //}
        if (newRole)
        {
            SPSdkManager.Instance.CYRoleCreate(playerDto.id.ToString(), playerDto.name, playerDto.grade, playerDto.roleCreateTime);
         //SPSdkManager.Instance.SubmitRoleData(ServerManager.Instance.uid, newRole, playerDto.id.ToString(), playerDto.name, playerDto.grade.ToString(), serverInfo.serverId.ToString(), serverInfo.name);
        }

        ModelManager.Player.Setup(playerDto);

        if (_afterLogin == false)
        {
            AfterLogin();
        }

        // 由于登陆那里还没有PlayerId，所以无法恢复购买
        // 因此在这里做恢复购买的操作
        PayManager.Instance.RestoreCompletedTransactions();

        //		if (_reLogin)
        //		{
        //			CallBackReLogin();
        //
        //			int sceneId = ModelManager.Player.GetPlayer ().sceneId;
        //			ServiceRequestAction.requestServer (SceneService.enter (sceneId));
        //		}
    }

    //private void OnReLogin(GeneralResponse e)
    //{
    //    var playerDto = e as PlayerDto;

    //    if (playerDto != null)
    //    {
    //        GameDebuger.Log("重新登录成功");
    //        TipManager.AddTip("重新登录成功");
    //        ModelManager.Player.Setup(playerDto);

    //        //统一在重新登陆后调用logon， 来获取新的信息, 例如战斗
    //        //ServiceRequestAction.requestServer(PlayerService.logon());
    //        CallBackReLogin();

    //        //WorldManager.Instance.ReEnterScene();
    //    }
    //    else
    //    {
    //        GameDebuger.Log("重新登录失败");

    //        ExitGameScript.OpenReloginTipWindow("重新登陆失败， 请重新进入游戏");
    //    }
    //}

    public void UpdateStaticData()
    {
        TalkingDataHelper.OnEventSetp("GameLogin/UpdateStaticData"); //更新静态数据并加载
        DataManager.Instance.UpdateStaticData(OnPreLoadDataFinish, OnAllStaticDataFinish, DataLoadingMessage, DataLoadingMsgProcess);
    }

    private void DataLoadingMessage(string msg)
    {
        if (OnLoginMessage != null)
        {
            OnLoginMessage(msg);
        }
    }

    private void DataLoadingMsgProcess(float msgProcess)
    {
        if (OnLoginProcess != null)
        {
            OnLoginProcess(msgProcess);
        }
    }

    private void OnPreLoadDataFinish()
    {
        GameDebuger.Log("OnPreLoadDataFinish");

        //创建新角色
        if (_accountPlayerDto == null)
        {
            ConnectSocket();
        }
    }

    private void OnAllStaticDataFinish()
    {
        TalkingDataHelper.OnEventSetp("GameLogin/GetStaticDataSuccess"); //游戏数据加载完成
        DataLoadingMsgProcess(1f);

        if (_playerDto == null && _accountPlayerDto != null)
        {
            ConnectSocket();
        }
        else
        {
            Login();
        }
    }


    private void AfterLogin()
    {
        LayerManager.Instance.LockUICamera(true);
        NotifyListenerRegister.Setup();
        GameDebuger.TODO(" FunctionOpenHelper.Setup ();");

        PrintLog("获取角色数据...");

        TalkingDataHelper.OnEventSetp("GameLogin/RequestAfterLogin"); //请求AfterLogin                      

        WorldManager.IsWaitingEnter = true;

        ServiceRequestAction.requestServer(Services.Player_AfterLogin(BaoyugameSdk.getUUID()), "Player_AfterLogin", e =>
            {

                WorldManager.Create();
                //设置保存的自动巡逻
                ModelManager.Player.IsAutoFram = PlayerGameState.IsAutoFram;

                _afterLogin = true;
                var afterLoginDto = e as AfterLoginDto;
                /** 玩家相关信息 */
                GamePlotManager.Instance.SetLastPlotId(afterLoginDto.plotId);
                /** 记录当天伙伴招募次数  **/
                ProxyCrewReCruit.crewCurrencyAddTimes = afterLoginDto.remainCrewCurrencyAddTimes;
                TowerDataMgr.DataMgr.SetUp(afterLoginDto);

                ModelManager.Player.SetupFromAfterLogin(afterLoginDto);
                StaticInit.StaticInit.doOnce();

                ModelManager.BattleConfig.Setup();

                GameDebuger.TODO(@"NpcModelModule.Instance.SetUpNpcModelModule ();
            //  场景怪物战斗模块数据处理
            SceneMonsterModel.Instance.SetUp (afterLoginDto.starRewardCount, afterLoginDto.worldBossRewardCount);
 TeamModel.Instance.Setup();

          ");
                Asyn.AsynInitManager.StartInit();
            });

        //登录时判断刷新身上的游击士任务
        if (FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum.FUN_27))
            ServiceRequestAction.requestServer(Services.Bracer_LoginRefresh());
    }

    private void OnLoginDataLoadingFinish()
    {
        LayerManager.Instance.LockUICamera(false);
        GameDebuger.Log("登录数据加载完成");
        ReadyMapRes();
    }
    private void ReadyMapRes()
    {
        PrintLog("载入场景资源...");
        if (_reLogin && !WorldManager.Instance.IsDestroy())
        {
            OnLoadMapFinish();
        }
        else
        {
            int sceneId = ModelManager.Player.GetPlayer().sceneId;
            WorldManager.Instance.OldSceneId = sceneId;

            var sceneMap = DataCache.getDtoByCls<SceneMap>(sceneId);
            if (sceneMap != null)
            {
                WorldManager.Instance.FirstEnterSceneRequest((SceneDto sceneDto) =>
                {
                    var playerPos = Vector3.zero;
                    var dto = sceneDto.objects.Find(objDto => objDto.id == ModelManager.IPlayer.GetPlayerId());

                    if (dto != null)
                    {
                        playerPos = new Vector3(dto.x, 0, dto.z);
                    }
                    WorldMapLoader.Instance.LoadWorldMap(sceneMap.resId, playerPos, OnLoadMapFinish,
                        OnLoadLevelProgress);
                });
            }
        }
    }

    private void OnLoadMapFinish()
    {
        if (_reLogin)
        {
            //            ProxyMainUI.Close();
            //			if (MainUIViewController.Instance != null) {
            //				MainUIViewController.Instance.Dispose ();
            //			}
            //ProxyMainUI.Hide();

            CallBackReLogin();

            //			int sceneId = ModelManager.Player.GetPlayer ().sceneId;
            //			ServiceRequestAction.requestServer (SceneService.enter (sceneId));

            RequestLoadingTip.Reset();
        }

        if (WorldManager.FirstEnter)
        {
            if (_reLogin)
            {
                if (ProxyLoginModule.IsOpen())
                {
                    ScreenMaskManager.FadeInOut(() =>
                        {
                            DelayExitLoginScene();
                        });
                }
                else
                {
                    DelayExitLoginScene();
                }
            }
            else
            {
                ScreenMaskManager.FadeInOut(() =>
                    {
                        DelayExitLoginScene();
                    });
            }
        }
        else
        {
            DelayExitLoginScene();
        }
    }

    private void DelayExitLoginScene()
    {
        JSTimer.Instance.SetupCoolDown("DelayExitLoginScene", 0.1f, null, delegate ()
            {
                ExitLoginScene();
            });
    }

    private void ExitLoginScene()
    {
        TalkingDataHelper.OnEventSetp("GameLogin/EnterGame"); //进入游戏
        SPSDK.gameEvent("10023");       //进入游戏
        GameDebuger.Log("ExitLoginScene");

        try
        {
            //调用后会唤起畅游SDK的悬浮球，应该在场景资源加载完成后调用
            int balance = _playerDto.balance > Int32.MaxValue ? Int32.MaxValue : (int)_playerDto.balance;
            SPSdkManager.Instance.CYGameStarted(_playerDto.id.ToString(), _playerDto.name, _playerDto.grade, _playerDto.partyName, balance, _playerDto.vipLevel, _playerDto.roleCreateTime);
        }
        catch (Exception ex)
        {
            GameDebuger.LogException(ex);
        }


        ProxyLoginModule.Close();
        JoystickModule.Instance.Setup();
        GameDebuger.TODO(@"NewBieGuideManager.Instance.Setup ();");
        ProxyMainUI.Open();
        WorldManager.IsWaitingEnter = false;

        if (!GamePlotManager.Instance.HasLastPlot())
        {
            //无需播放剧情，直接切换到游戏场景
            LayerManager.Instance.SwitchLayerMode(UIMode.GAME);
            WorldManager.Instance.FirstEnterScene();
        }
        else
        {
            GamePlotManager.Instance.PlayLastPlot();
            GamePlotManager.Instance.OnFinishPlot += OnFinishPlot;
        }

        GameDebuger.TODO(@"var reserveExpDto = ModelManager.Player.ReserveExpDto;
		if (reserveExpDto != null) {
			string reserveExpTip = string.Format ('距上次离线时间{0}分钟，共获得{1}储备经验。详情打开人物属性界面点击经验条查询', reserveExpDto.minutes,
				                                reserveExpDto.value);
			TipManager.AddTip (reserveExpTip);
			ModelManager.Player.ReserveExpDto = null;
    }");
    }

    private void OnFinishPlot(Plot plot)
    {
        WorldManager.Instance.FirstEnterScene();
        GamePlotManager.Instance.OnFinishPlot -= OnFinishPlot;
    }

    private void OnLoadLevelProgress(float precent)
    {
        //Debug.Log("OnLoadLevelProgress " + precent);
    }

    private void ShowMessageBox(string msg)
    {
        PrintLog(msg);
        _keepSocket = false;
    }

    private void PrintLog(string msg)
    {
        GameDebuger.Log(msg);
        if (OnLoginMessage != null)
        {
            OnLoginMessage(msg);
        }
    }

    private void OnLoginSuccess(bool isGuest, string sid)
    {
        if (ServerManager.Instance.sid != sid)
        {
            bool bindTip = false;
            if (ServerManager.Instance.isGuest && !isGuest)
            {
                bindTip = true;
            }
            ServerManager.Instance.isGuest = isGuest;

            if (!string.IsNullOrEmpty(sid) && sid != "(null)")
            {
                ServerManager.Instance.sid = sid;
                ExitGameScript.Instance.DoReloginAccount(false);
            }
            else
            {
                if (bindTip)
                {
                    TipManager.AddTip("账号绑定成功");
                }
                ProxyWindowModule.closeSimpleWinForTop();
            }
        }
        else
        {
            ProxyWindowModule.closeSimpleWinForTop();
        }
    }

    private void OnLogout(bool success)
    {
        if (success)
        {
            ExitGameScript.OpenReloginTipWindow("您已经注销了账号， 请重新游戏", true);
        }
    }

    public void RemoveListener()
    {
        SocketManager.Instance.OnHAConnected -= HandleOnHAConnected;
        SocketManager.Instance.OnHaError -= HandleOnHaError;
        SocketManager.Instance.OnHaCloseed -= HandleOnHaCloseed;
        SocketManager.Instance.OnStateEvent -= HandleOnStateEvent;

        SPSdkManager.Instance.OnLoginSuccess -= OnLoginSuccess;
        SPSdkManager.Instance.OnLogoutNotify -= OnLogout;

        GamePlotManager.Instance.OnFinishPlot -= OnFinishPlot;
    }

    private void Destroy()
    {
        RemoveListener();
        _serverInfo = null;
    }

    private void CloseSocket()
    {
        SocketManager.Instance.Close(true);
        SocketManager.Instance.Destroy();
    }

    public void CallBackReLogin()
    {
        //重新登录后的回调//
        if (OnReloginSuccess != null)
        {
            OnReloginSuccess();
        }
    }

    //open loginScene
    public void GotoLoginScene()
    {
        GameDebuger.Log("GotoLoginScene!!!");

        ExitGameScript.CheckConnected = false;
        Destroy();
        CloseSocket();
        RequestLoadingTip.Reset();

        ProxyLoginModule.Open();
    }

    #region 排队

    private QueueWindowPrefabController _queueWindowCon;

    private void LoginQueue1(QueueDto dto)
    {
        var serverInfo = ServerManager.Instance.GetServerInfo();
        if (_queueWindowCon == null)
        {
            _queueWindowCon = ProxyWindowModule.OpenQueueWindow(serverInfo.name + " 已满", dto.index, dto.remain);
        }
        else
        {
            _queueWindowCon.UpdateData(serverInfo.name + " 已满", dto.index, dto.remain);
        }
    }

    private const float CanLoginWaitTime = 600f;

    public void UpdateLoginQueueData(QueueDto dto)
    {
        var serverInfo = ServerManager.Instance.GetServerInfo();
        if (_queueWindowCon == null)
        {
            _queueWindowCon = ProxyWindowModule.OpenQueueWindow(serverInfo.name + " 已满", dto.index, dto.remain);
        }
        else
        {
            _queueWindowCon.UpdateData(serverInfo.name + " 已满", dto.index, dto.remain);
        }

        //        Debug.LogError("排队数据变更");
        if (dto.playerDto != null)
        {
            _queueWindowCon.SetCloseTime(CanLoginWaitTime, () =>
                {
                    OnQueueLogin(dto.playerDto);
                });
        }
    }

    #endregion
}