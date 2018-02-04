// **********************************************************************
// Copyright (c) 2015 cilugame. All rights reserved.
// File     : SPSdkManager.cs
// Author   : senkay <senkay@126.com>
// Created  : 11/18/2015 
// Porpuse  : 
// **********************************************************************
//
using UnityEngine;
using System;
using System.Collections.Generic;
using AssetPipeline;

public class SPSdkManager
{
    private static readonly SPSdkManager instance = new SPSdkManager();

    public static SPSdkManager Instance
    {
        get
        {
            return instance;
        }
    }

#if UNITY_EDITOR
    public static Dictionary<string, SPChannel> SpChannelDic(string configSuffix = "", bool forceLoad = false)
    {
        if (forceLoad)
        {
            LoadSPChannelConfigAtEditor(configSuffix);
        }
        else
        {
            if (_spChannelDic == null)
            {
                LoadSPChannelConfigAtEditor(configSuffix);
            }
        }
        return _spChannelDic;
    }

#endif

    private static Dictionary<string, SPChannel> _spChannelDic;

    public bool WaitingLoginResult = false;

    #region SDK callback handler

    public event System.Action<bool> OnInitCallback;

    public event System.Action<bool, string> OnLoginSuccess;
    public event System.Action OnLoginFail;
    public event System.Action OnLoginCancel;

    public event System.Action<bool> OnLogoutNotify;
    public event System.Action<bool> OnLogoutCallback;
    public event System.Action<bool> OnExitCallback;
    public event System.Action OnNoExiterProvideCallback;

    public event System.Action<bool> OnPayCallback;

    public event System.Action<String> OnCYPayCallback;

    public void CallbackInit(bool success)
    {
        GameDebuger.Log("CallbackInit " + success);

        if (OnInitCallback != null)
        {
            OnInitCallback(success);
        }
    }

    public void CallbackLoginSuccess(bool isGuest, string sid)
    {
        GameDebuger.Log(string.Format("CallbackLoginSuccess isGuest={0} sid={1}", isGuest, sid));

        WaitingLoginResult = false;

        if (OnLoginSuccess != null)
        {
            OnLoginSuccess(isGuest, sid);
        }
    }

    public void CallbackLoginFail()
    {
        GameDebuger.Log("CallbackLoginFail");

        WaitingLoginResult = false;

        if (OnLoginFail != null)
        {
            OnLoginFail();
        }
    }

    public void CallbackLoginCancel()
    {
        GameDebuger.Log("CallbackLoginCancel");

        WaitingLoginResult = false;

        if (OnLoginCancel != null)
        {
            OnLoginCancel();
        }
    }

    public void CallbackLogout(bool success)
    {
        GameDebuger.Log("CallbackLogout " + success);

        if (OnLogoutCallback != null)
        {
            OnLogoutCallback(success);
            OnLogoutCallback = null;
        }
        else
        {
            if (OnLogoutNotify != null)
            {
                OnLogoutNotify(success);
            }
        }
    }

    public void CallbackNoExiterProvide()
    {
        GameDebuger.Log("CallbackNoExiterProvide");
        if (OnNoExiterProvideCallback != null)
        {
            OnNoExiterProvideCallback();
        }
    }

    public void CallbackExit(bool success)
    {
        GameDebuger.Log("CallbackExit " + success);

        if (OnExitCallback != null)
        {
            OnExitCallback(success);
        }
    }

    public void CallbackPay(bool success)
    {
        GameDebuger.Log("CallbackPay " + success);

        if (OnPayCallback != null)
        {
            OnPayCallback(success);
            OnPayCallback = null;
        }
    }

    #endregion

    #region SDK CALL

    public void Setup()
    {
        GameDebuger.Log("SPSDK Setup");

        SPSDK.Setup();
    }

    public void Init(Action<bool> callback)
    {
        GameDebuger.Log("SPSDK Init");

        OnInitCallback = callback;

        if (GameSetting.Channel == AgencyPlatform.Channel_cilugame)
        {
            OnInitCallback(true);
        }
        else if (GameSetting.Channel == AgencyPlatform.Channel_demi)
        {
            SdkLoginMessage.Instance.C2SdkInitRoot(LayerManager.Root.UIModuleRoot);
            OnInitCallback(true);
        }
        else
        {
            //畅游的Init在游戏初始化时就已经完成了
            OnInitCallback(true);
            //#if (UNITY_EDITOR || UNITY_STANDALONE)
            //            OnInitCallback(true);
            //#elif UNITY_ANDROID || UNITY_IPHONE
            //            SPSDK.Init();
            //#else
            //			OnInitCallback(false);
            //#endif
        }
    }

    public void Login()
    {
        GameDebuger.Log("SPSDK Login");

        if (GameSetting.Channel == AgencyPlatform.Channel_cilugame || GameSetting.GMMode || GameSetting.IsOriginWinPlatform)
        {
            ProxyLoginModule.OpenTestSdk();
        }
        else if (GameSetting.Channel == AgencyPlatform.Channel_demi)
        {
            SdkLoginMessage.Instance.C2SdkLogin();
        }
        else
        {
#if (UNITY_EDITOR || UNITY_STANDALONE)
            OnLoginFail();
#elif UNITY_ANDROID || UNITY_IPHONE
			WaitingLoginResult = true;
             CYLogin();
			//SPSDK.Login();
#else
			OnLoginFail();
#endif
        }
    }

    public void Bind()
    {
        GameDebuger.Log("SPSDK Bind");

#if (UNITY_EDITOR || UNITY_STANDALONE)
        OnLoginFail();
#elif UNITY_ANDROID || UNITY_IPHONE
		SPSDK.Bind();
#else
		OnLoginFail();
#endif
    }

    public bool IsSupportLogout()
    {
        GameDebuger.Log("SPSDK IsSupportLogout");

        if (GameSetting.Channel == AgencyPlatform.Channel_cilugame)
        {
            return true;
        }
        else
        {
#if (UNITY_EDITOR || UNITY_STANDALONE)
            return true;
#elif UNITY_ANDROID || UNITY_IPHONE
			return SPSDK.IsSupportLogout();
#else
			return true;
#endif
        }
    }

    public void Logout(Action<bool> callback)
    {
        OnLogoutCallback = callback;
        GameDebuger.Log("SPSDK Logout");

        if (GameSetting.Channel == AgencyPlatform.Channel_cilugame)
        {
            OnLogoutCallback(true);
        }
        else if (GameSetting.Channel == AgencyPlatform.Channel_demi)
        {
            SdkLoginMessage.Instance.C2SdkLogout();
            OnLogoutCallback(true);
        }
        else
        {
#if (UNITY_EDITOR || UNITY_STANDALONE)
            OnLogoutCallback(true);
#elif UNITY_ANDROID || UNITY_IPHONE
            //如果立即注销，则马上回调，否则等待

                     if (SPSDK.Logout())
                     {
                         OnLogoutCallback(true);
                     }
            else
            {
            	//等待SDK回调callback后处理
            }
#else
			OnLogoutCallback(true);
#endif
        }
    }

    public void DoExiter(Action<bool> exitCallback, Action noExiterCallback)
    {
        OnExitCallback = exitCallback;
        OnNoExiterProvideCallback = noExiterCallback;
        GameDebuger.Log("SPSDK DoExiter");

        if (GameSetting.Channel == AgencyPlatform.Channel_cilugame)
        {
            OnNoExiterProvideCallback();
        }
        else
        {
#if (UNITY_EDITOR || UNITY_STANDALONE)
            OnNoExiterProvideCallback();
#elif UNITY_ANDROID || UNITY_IPHONE
            //if (SPSDK.DoExiter())
            //{
            //    OnNoExiterProvideCallback();
            //}
            CYCallback(CySdk.ResultCode.EXIT_GAME, CYExitCallback);
            CYCallback(CySdk.ResultCode.EXIT_GAME_DIALOG, CYNoExiterProvideCallback);
            CYExit();


#endif




        }

    }

    public void Exit()
    {
        GameDebuger.Log("SPSDK Exit");

#if (UNITY_EDITOR || UNITY_STANDALONE)
        return;
#elif UNITY_ANDROID || UNITY_IPHONE
		SPSDK.Exit();
#endif
    }

    //注册
    public void Regster(string account, string uid)
    {
        GameDebuger.Log(string.Format("SPSDK Regster account={0} uid={1}", account, uid));

        if (GameSetting.Channel == AgencyPlatform.Channel_cilugame)
            return;

#if (UNITY_EDITOR || UNITY_STANDALONE)
        return;
#elif UNITY_ANDROID || UNITY_IPHONE
		SPSDK.Regster(account, uid);
#endif
    }

    public void UpdateUserInfo(string uid)
    {
        if (string.IsNullOrEmpty(uid))
        {
            return;
        }

        GameDebuger.Log("SPSDK UpdateUserInfo uid=" + uid);

        if (GameSetting.Channel == AgencyPlatform.Channel_cilugame)
            return;

#if (UNITY_EDITOR || UNITY_STANDALONE)
        return;
#elif UNITY_ANDROID || UNITY_IPHONE
		SPSDK.UpdateUserInfo(uid);
#endif
    }

    public void SubmitRoleData(string uid, bool newRole, string playerId, string playerName, string playerLv, string serverId, string serverName)
    {
        GameDebuger.Log("SPSDK SubmitRoleData");

        if (GameSetting.Channel == AgencyPlatform.Channel_cilugame)
            return;

#if (UNITY_EDITOR || UNITY_STANDALONE)
        return;
#elif UNITY_ANDROID || UNITY_IPHONE
		SPSDK.SubmitRoleData(uid, newRole, playerId, playerName, playerLv, serverId, serverName);
#endif
    }

    public string GetChannel()
    {
        GameDebuger.Log("SPSDK GetChannelId");

        if (GameSetting.Channel != AgencyPlatform.Channel_cilugame && GameSetting.Channel != AgencyPlatform.Channel_demi)
        {
#if (UNITY_EDITOR || UNITY_STANDALONE)
            // win下取appstore的值
            return !GameSetting.IsOriginWinPlatform ? GameSetting.Channel : WinGameSetting.Channel;
#elif UNITY_ANDROID || UNITY_IPHONE
            //畅游直接返回"cyou"
            return GameSetting.Channel;
            //var id = SPSDK.GetChannelId();
            //return !string.IsNullOrEmpty(id) ? id : GameSetting.ClientMode.ToString();
#else
            return GameSetting.ClientMode.ToString();
#endif
        }
        else
        {

            return GameSetting.Channel;
        }
    }

    public bool IsSupportUserCenter()
    {
        GameDebuger.Log("SPSDK IsSupportUserCenter");

        if (GameSetting.Channel == AgencyPlatform.Channel_cilugame)
        {
            return false;
        }
        else
        {
#if (UNITY_EDITOR || UNITY_STANDALONE)
            return false;
#elif UNITY_ANDROID || UNITY_IPHONE
            return SPSDK.IsSupportUserCenter();
#else
			return false;
#endif
        }
    }

    public void EnterUserCenter()
    {
        GameDebuger.Log("SPSDK EnterUserCenter");
#if (UNITY_EDITOR || UNITY_STANDALONE)
        return;
#elif UNITY_ANDROID || UNITY_IPHONE
		SPSDK.EnterUserCenter();
#else
		return;
#endif
    }

    public bool IsSupportBBS()
    {
        GameDebuger.Log("SPSDK IsSupportBBS");

        if (GameSetting.Channel == AgencyPlatform.Channel_cilugame)
        {
            return false;
        }
        else
        {
#if (UNITY_EDITOR || UNITY_STANDALONE)
            return false;
#elif UNITY_ANDROID || UNITY_IPHONE
            return SPSDK.IsSupportBBS();
#else
			return false;
#endif
        }
    }

    public void EnterSdkBBS()
    {
        GameDebuger.Log("SPSDK EnterSdkBBS");
#if (UNITY_EDITOR || UNITY_STANDALONE)
        return;
#elif UNITY_ANDROID || UNITY_IPHONE
		SPSDK.EnterSdkBBS();
#else
		return;
#endif
    }

    public bool IsSupportShowOrHideToolbar()
    {
        GameDebuger.Log("SPSDK IsSupportShowOrHideToolbar");
#if (UNITY_EDITOR || UNITY_STANDALONE)
        return false;
#elif UNITY_ANDROID || UNITY_IPHONE
        return SPSDK.IsSupportShowOrHideToolbar();
#else
		return false;
#endif
    }

    public void ShowFloatToolBar()
    {
        GameDebuger.Log("SPSDK ShowFloatToolBar");
#if (UNITY_EDITOR || UNITY_STANDALONE)
        return;
#elif UNITY_ANDROID || UNITY_IPHONE
		SPSDK.ShowFloatToolBar();
#else
		return;
#endif
    }

    public void HideFloatToolBar()
    {
        GameDebuger.Log("SPSDK HideFloatToolBar");
#if (UNITY_EDITOR || UNITY_STANDALONE)
        return;
#elif UNITY_ANDROID || UNITY_IPHONE
		SPSDK.HideFloatToolBar();
#else
		return;
#endif
    }

    public void DoPay(
        string orderSerial,
        string productId,
        string productName,
        string productPrice,
        string productCount,
        string serverId,
        string payNotifyUrl,
        string payKey,
        string channelOrderSerial,
        Action<bool> payCallback
    )
    {
        //TODO 集成talkingdata的充值统计
        //AdTracking.OnPlaceOrder(orderSerial, productName, int.Parse(productPrice));

        OnPayCallback = payCallback;

        GameDebuger.Log("SPSDK DoPay");

#if (UNITY_EDITOR || UNITY_STANDALONE)
        return;
   
#elif UNITY_ANDROID || UNITY_IPHONE
        SPSDK.DoPay(orderSerial, productId, productName, productPrice, productCount, serverId, payNotifyUrl, payKey, channelOrderSerial);
#else
		return;
#endif
    }

    public void DoPay(
        CySdk.Goods good,
        int balance,
        int vipLevel,
        Action<string> payCallback
    )
    {
        OnCYPayCallback = payCallback;
        GameDebuger.Log("SPSDK DoPay");
#if (UNITY_EDITOR || UNITY_STANDALONE)
        //       return;
#elif UNITY_ANDROID || UNITY_IPHONE
        SPSDK.pay(good,balance,vipLevel);
#else
		return;
#endif
    }

    public void DoPay(
      string goodsId,
      string pushInfo,
      int balance,
      int vipLevel,
       Action<string> payCallback
   )
    {
        OnCYPayCallback = payCallback;
        GameDebuger.Log("SPSDK DoPay");
#if (UNITY_EDITOR || UNITY_STANDALONE)
        //       return;
#elif UNITY_ANDROID || UNITY_IPHONE
        SPSDK.pay(goodsId, pushInfo, balance,vipLevel);
#else
		return;
#endif
    }
    public void DoPay(
    string goodsId,
    double totalPrice,
    string pushInfo,
    int balance,
    int vipLevel,
   Action<string> payCallback
    )
    {
        OnCYPayCallback = payCallback;
        GameDebuger.Log("SPSDK DoPay");
#if (UNITY_EDITOR || UNITY_STANDALONE)
        //       return;
#elif UNITY_ANDROID || UNITY_IPHONE
        SPSDK.payWilful(goodsId, totalPrice, pushInfo, balance,vipLevel);
#else
		return;
#endif
    }

    #endregion

#if UNITY_EDITOR
    private const string Config_Path = "Assets/Editor/BuildTools/Configs/SPChannelConfig{0}.json";

    public static void LoadSPChannelConfigAtEditor(string configSuffix)
    {
        if (configSuffix == null)
        {
            configSuffix = "";
        }

        if (!string.IsNullOrEmpty(configSuffix))
        {
            configSuffix = "_" + configSuffix;
        }

        _spChannelDic = new Dictionary<string, SPChannel>();

        string path = string.Format(Config_Path, configSuffix);
        GameDebuger.Log("path=" + path);
        string json = FileHelper.ReadAllText(path);
        if (!string.IsNullOrEmpty(json))
        {
            var spChannels = JsHelper.ToCollection<List<SPChannel>, SPChannel>(json);
            for (int i = 0; i < spChannels.Count; i++)
            {
                SPChannel channel = spChannels[i];
                _spChannelDic[channel.name] = channel;
            }
        }
    }
#endif

    public static void LoadSPChannelConfig(Action<bool> callback)
    {
        _spChannelDic = new Dictionary<string, SPChannel>();
        callback(true);
    }

    public static string GetChannelBundleId(string id)
    {
        SPChannel info = null;
        if (_spChannelDic.TryGetValue(id, out info))
        {
            return info.bundleId;
        }
        else
        {
            return "";
        }
    }



    #region 畅游SDK

    private Dictionary<int, Action<string>> _CYCallback = new Dictionary<int, Action<string>>();



    public void OnCYCallback(string Jsonparam)
    {
        LITJson.JsonData jsonData = LITJson.JsonMapper.ToObject(Jsonparam);
        int state_code = (int)jsonData["state_code"];
        if (_CYCallback.ContainsKey(state_code))
        {
            try
            {
                if (_CYCallback[state_code] != null)
                {
                    _CYCallback[state_code](Jsonparam);
                }
            }
            catch (Exception ex)
            {
                GameDebuger.LogError(ex.StackTrace);
            }
           
        }

    }

    public SPSdkManager CYCallback(int type, Action<string> callback)
    {
        if (_CYCallback.ContainsKey(type))
        {
            _CYCallback[type] += callback;
        }
        else
        {
            _CYCallback[type] = callback;
        }
        return this;
    }

    public void RemoveCYCallback(int type, Action<string> callback)
    {
        if (_CYCallback.ContainsKey(type))
        {
            _CYCallback[type] -= callback;
            if (_CYCallback[type] == null)
            {
                _CYCallback.Remove(type);
            }
        }
       
    }

    public SPSdkManager CYLogin()
    {
        SPSDK.Login();
        return this;
    }

    public SPSdkManager CYGetHost()
    {
        SPSDK.getHost();
        return this;
    }

    public SPSdkManager CYInit()
    {
        SPSDK.Init();
        return this;
    }

    public SPSdkManager CYGoodsData()
    {
        SPSDK.goodsData();
        return this;
    }

    public SPSdkManager CYLogout()
    {
        SPSDK.logout();
        return this;
    }

    public SPSdkManager CYExit()
    {
        SPSDK.exit();
        return this;
    }

    private void CYExitCallback(string jsonParam)
    {
       // CySdk.Result result = JsHelper.ToObject<CySdk.Result>(jsonParam);
        if (OnExitCallback != null)
        {
            OnExitCallback(true);
        }
        RemoveCYCallback(CySdk.ResultCode.EXIT_GAME, CYExitCallback);
    }

    private void CYNoExiterProvideCallback(string jsonParam)
    {
        //CySdk.Result result = JsHelper.ToObject<CySdk.Result>(jsonParam);
        if (OnNoExiterProvideCallback != null)
        {
            OnNoExiterProvideCallback();
        }
        RemoveCYCallback(CySdk.ResultCode.EXIT_GAME_DIALOG, CYNoExiterProvideCallback);
    }


    public SPSdkManager CYTokenVerify(bool success, string gameUid, string channelOid, string accessToken)
    {
        SPSDK.tokenVerify(success, gameUid, channelOid, accessToken);
        return this;
    }

    public SPSdkManager CYOtherVerify(string extension)
    {
        SPSDK.otherVerify(extension);
        return this;
    }
    /// <summary>
    /// 进入服务器
    /// </summary>
    /// <param name="serverId">服务器ID</param>
    /// <param name="serverName">服务器名字</param>
    public void CYEnterServer(int serverId, string serverName)
    {
        SPSDK.enterServer(serverId,serverName);
    }
    /// <summary>
    /// 开始游戏
    /// </summary>
    /// <param name="roleId">角色id</param>
    /// <param name="roleName">角色名称</param>
    /// <param name="roleLevel">角色等级</param>
    /// <param name="partyName">工会名称</param>
    /// <param name="balance">元宝数量</param>
    /// <param name="vipLevel">vip等级</param>
    /// <param name="roleCreatetime">角色创建时间，精确到秒</param>
    public void CYGameStarted(string roleId,string roleName,int roleLevel,string partyName,int balance,int vipLevel,long roleCreatetime)
    {
        SPSDK.gameStarted(roleId, roleName, roleLevel, partyName, balance, vipLevel, roleCreatetime);
    }
    /// <summary>
    /// 角色创建
    /// </summary>
    /// <param name="roleId">角色id</param>
    /// <param name="roleName">角色名字</param>
    /// <param name="roleLevel">角色等级</param>
    /// <param name="roleCreateTime">角色创建时间</param>
    public void CYRoleCreate(string roleId, string roleName, int roleLevel, long roleCreateTime)
    {
          SPSDK.roleCreate(roleId, roleName, roleLevel, roleCreateTime);    
    }
    /// <summary>
    /// 角色升级
    /// </summary>
    /// <param name="level"></param>
    public void CYRoleUpgrade(int level)
    {
        SPSDK.roleUpgrade(level);
    }

    public void CYServiceCenter()
    {
        if (SPSDK.apiAvailable(CySdk.ApiCode.SERVICE_CENTER))
        {
            SPSDK.serviceCenter();
        }
    }

    public SPSdkManager CYPay(CySdk.Goods good,int balance,int vipLevel)
    {
        SPSDK.pay(good,balance,vipLevel);
        return this;
    }

    public SPSdkManager CYPaywilful(string goodsId, double totalPrice, string pushInfo, int balance, int vipLevel)
    {
        SPSDK.payWilful(goodsId, totalPrice, pushInfo, balance, vipLevel);
        return this;
    }

    public SPSdkManager CYPay(string goodsId, string pushInfo, int balance, int vipLevel)
    {
        SPSDK.pay(goodsId, pushInfo, balance, vipLevel);
        return this;
    }

    public void CYPayHistoryView()
    {
        SPSDK.payHistoryView();
    }



    #endregion



}

