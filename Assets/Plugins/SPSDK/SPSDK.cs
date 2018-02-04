using System;
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using CySdk;
public static class SPSDK
{
    private const string SDK_JAVA_CLASS = "com.cilugame.h1.CLSDKPlugin";

#if UNITY_ANDROID
	private static AndroidJavaClass cls{
        get
        {
            if (_cls == null)
            {
                _cls = JavaSdkUtils.GetUnityJavaClass(SDK_JAVA_CLASS);
            }
            return _cls;
        }
    }
	private static AndroidJavaClass _cls;
    

#elif UNITY_IPHONE
    [DllImport("__Internal")]
    private static extern void __spsdk_init();

    [DllImport("__Internal")]
    private static extern void __spsdk_getHost();
    [DllImport("__Internal")]
    private static extern void __spsdk_login();

    [DllImport("__Internal")]
    private static extern void __spsdk_goodsData();
    [DllImport("__Internal")]
    private static extern void __spsdk_getGoodsListView();
    [DllImport("__Internal")]
    private static extern void __spsdk_payHistory(int page,int pagesize);
    [DllImport("__Internal")]
    private static extern void __spsdk_payHistoryView();
    [DllImport("__Internal")]
    private static extern void __spsdk_payWilful(string goodsId,double totalPrice,string pushInfo,int balance,int vipLevel);
    [DllImport("__Internal")]
    private static extern void __spsdk_pay(string goodsId, string pushInfo, int balance, int vipLevel);
    [DllImport("__Internal")]
    private static extern void __spsdk_payGood(string goodsname, int goodsNum, string goodsId, string goodsregisterId, double goodsPrice, string icon, string desp, string pushInfo, int type, int balance, int vipLevel);
    [DllImport("__Internal")]
    private static extern void __spsdk_exit();
    [DllImport("__Internal")]
    private static extern void __spsdk_logout();
    [DllImport("__Internal")]
    private static extern void __spsdk_switchUser();
    [DllImport("__Internal")]
    private static extern void __spsdk_userCenter();
    [DllImport("__Internal")]
    private static extern void __spsdk_serverCenter();
    [DllImport("__Internal")]
    private static extern void __spsdk_tokenVerify(bool success,string gameUid,string channelOid,string accessToken);
    [DllImport("__Internal")]
    private static extern void __spsdk_gameStarted(string roleId,string roleName,int roleLevel,string partyName,int balance,int vipLevel,long roleCreateTime);
    [DllImport("__Internal")]
    private static extern void __spsdk_enterServer(int serverId,string serverName);
    [DllImport("__Internal")]
    private static extern void __spsdk_roleCreate(string roleId,string roleName,int roleLevel,long roleCreateTime);
    [DllImport("__Internal")]
    private static extern void __spsdk_roleUpgrade(int newRoleLevel);

    [DllImport("__Internal")]
    private static extern void __spsdk_replaceOrder();

    [DllImport("__Internal")]
    private static extern bool __spsdk_apiAvailable(int code);
    [DllImport("__Internal")]
    private static extern void __spsdk_gameEvent(string eventId);
    [DllImport("__Internal")]
    private static extern void __spsdk_fps(int fps);
    [DllImport("__Internal")]
    private static extern void __spsdk_killgame();
    [DllImport("__Internal")]
    private static extern void __spsdk_showToast(string toast);

    [DllImport("__Internal")]
    private static extern void __spsdk_showBindingView();

    [DllImport("__Internal")]
    private static extern void __spsdk_isAuthention();

    [DllImport("__Internal")]
    private static extern void __spsdk_otherVerify(string param);

    [DllImport("__Internal")]
    private static extern string __spsdk_deviceId();

    [DllImport("__Internal")]
    private static extern bool __spsdk_isLog();

    [DllImport("__Internal")]
    private static extern string __spsdk_ip();

    [DllImport("__Internal")]
    private static extern int __spsdk_mode();

    [DllImport("__Internal")]
    private static extern bool __spsdk_isLandscape();

    [DllImport("__Internal")]
    private static extern string __spsdk_appVersionName();
    [DllImport("__Internal")]
    private static extern int __spsdk_appVersionCode();
    [DllImport("__Internal")]
    private static extern int __spsdk_sdkVersionCode();
    [DllImport("__Internal")]
    private static extern string __spsdk_sdkVersionName();
    [DllImport("__Internal")]
    private static extern string __spsdk_channelVersion();
    [DllImport("__Internal")]
    private static extern string __spsdk_appKey();
    [DllImport("__Internal")]
    private static extern string __spsdk_appSecret();
    [DllImport("__Internal")]
    private static extern string __spsdk_channelId();
    [DllImport("__Internal")]
    private static extern string __spsdk_cmbiChannelId();

    [DllImport("__Internal")]
    private static extern string __spsdk_getChannelId();
    [DllImport ("__Internal")]
	private static extern string __spsdk_appName();
	[DllImport ("__Internal")]
	private static extern string __spsdk_networkType();

    [DllImport("__Internal")]
	private static extern void __spsdk_submitRoleData(string uid, bool newRole, string playerId, string playerName, string playerLv, string serverId, string serverName);

    [DllImport("__Internal")]
    private static extern void __spsdk_bind();

    [DllImport("__Internal")]
    private static extern bool __spsdk_isSupportUserCenter();

    [DllImport("__Internal")]
    private static extern void __spsdk_enterUserCenter();

    [DllImport("__Internal")]
    private static extern bool __spsdk_isSupportBBS();

    [DllImport("__Internal")]
    private static extern void __spsdk_enterSdkBBS();

#endif

    public static void Setup()
    {
#if UNITY_ANDROID
		
#endif
    }

    private static void callSdkApi(string apiName, params object[] args)
    {
#if UNITY_ANDROID
		JavaSdkUtils.CallSdkApi(cls, apiName, args);
#endif
    }
    private static T callSdkApi<T>(string apiName, params object[] args)
    {
#if UNITY_ANDROID
       return JavaSdkUtils.CallSdkApi<T>(cls, apiName, args);
#else
        return default(T);
#endif
    }

    public static void Bind()
    {
    }


    public static void Init()
    {
#if UNITY_ANDROID
		//callSdkApi("Init");
        //初始化在android已经完成
#elif UNITY_IPHONE
        __spsdk_init();
#endif
    }

    public static void Login()
    {

#if UNITY_ANDROID
		callSdkApi("Login");
#elif UNITY_IPHONE
        __spsdk_login();
#endif
    }
    public static bool Logout()
    {
#if UNITY_ANDROID
		if(cls != null)
		{
			callSdkApi("Logout");
		}
		else
		{
		    return true;
		}
#elif UNITY_IPHONE
        __spsdk_logout();
#endif
        return false;
    }

    public static void Exit()
    {
#if UNITY_ANDROID
		if (cls != null)
		{
		callSdkApi("Exit");
		}
#endif
    }
    public static void DoPay(
string orderSerial,
string productId,
string productName,
string productPrice,
string productCount,
string serverId,
string payNotifyUrl,
string payKey,
string channelOrderSerial
)
    {
        Debug.Log("SPSDK DoPay");
#if UNITY_ANDROID
		// 天成支付单位是分
		callSdkApi("DoPay", orderSerial, productId, productName, (Convert.ToInt32(productPrice) * 100).ToString(), productCount, serverId, payNotifyUrl, payKey, channelOrderSerial);
#elif UNITY_IPHONE
		__spsdk_pay(orderSerial, productId, productName, productPrice, productCount, serverId);
#else
        return;
#endif
    }



    /// <summary>
    /// token验证
    /// </summary>
    /// <param name="success"></param>
    /// <param name="gameUid"></param>
    /// <param name="channelOid"></param>
    /// <param name="accessToken"></param>
    public static void tokenVerify(bool success, string gameUid, string channelOid, string accessToken)
    {
#if UNITY_ANDROID
		callSdkApi("tokenVerify",success,gameUid,channelOid,accessToken);
#elif UNITY_IPHONE
        __spsdk_tokenVerify(success, gameUid, channelOid, accessToken);
#endif
    }
    /// <summary>
    /// 游戏开始
    /// </summary>
    /// <param name="roleId"></param>
    /// <param name="roleName"></param>
    /// <param name="roleLevel"></param>
    /// <param name="partyName"></param>
    /// <param name="balance"></param>
    /// <param name="vipLevel"></param>
    /// <param name="roleCreateTime"></param>
    public static void gameStarted(string roleId, string roleName, int roleLevel, string partyName, int balance, int vipLevel, long roleCreateTime)
    {
#if UNITY_ANDROID
		callSdkApi("gameStarted",roleId,roleName,roleLevel,partyName,balance,vipLevel,roleCreateTime);
#elif UNITY_IPHONE
        __spsdk_gameStarted(roleId, roleName, roleLevel, partyName, balance, vipLevel, roleCreateTime);
#endif
    }

    /// <summary>
    /// 获取网关
    /// </summary>
    public static void getHost()
    {
#if UNITY_ANDROID
		callSdkApi("getHost");
#elif UNITY_IPHONE
        __spsdk_getHost();
#endif
    }

    /// <summary>
    /// 进入服务器
    /// </summary>
    /// <param name="serverId"></param>
    /// <param name="serverName"></param>
    public static void enterServer(int serverId, string serverName)
    {
#if UNITY_ANDROID
		callSdkApi("enterServer",serverId,serverName);
#elif UNITY_IPHONE
        __spsdk_enterServer(serverId, serverName);
#endif
    }

    /// <summary>
    /// 获取商品数据
    /// </summary>
    public static void goodsData()
    {
#if UNITY_ANDROID
		callSdkApi("goodsData");
#elif UNITY_IPHONE
        __spsdk_goodsData();
#endif
    }
    /// <summary>
    /// 角色创建
    /// </summary>
    /// <param name="roleId">角色id</param>
    /// <param name="roleName">角色名字</param>
    /// <param name="roleLevel">角色等级</param>
    /// <param name="roleCreateTime">创建事件</param>
    public static void roleCreate(string roleId, string roleName, int roleLevel, long roleCreateTime)
    {
#if UNITY_ANDROID
        callSdkApi("roleCreate", roleId, roleName, roleLevel, roleCreateTime);
#elif UNITY_IPHONE
        __spsdk_roleCreate(roleId, roleName, roleLevel, roleCreateTime);
#endif
    }
    /// <summary>
    /// 角色升级
    /// </summary>
    /// <param name="newRoleLevel"></param>
    public static void roleUpgrade(int newRoleLevel)
    {
#if UNITY_ANDROID
        callSdkApi("roleUpgrade", newRoleLevel);
#elif UNITY_IPHONE
        __spsdk_roleUpgrade(newRoleLevel);
#endif
    }
    /// <summary>
    /// 手动补单,目前只有应用宝用到
    /// </summary>
    public static void replaceOrder()
    {
#if UNITY_ANDROID
        callSdkApi("replaceOrder");
#elif UNITY_IPHONE
        return __spsdk_replaceOrder();
#endif
    }
    /// <summary>
    /// 判断渠道是否可用
    /// </summary>
    /// <param name="code">CySdk.ApiCode</param>
    /// <returns></returns>
    public static bool apiAvailable(int code)
    {
#if UNITY_IPHONE
       return __spsdk_apiAvailable(code);
#elif UNITY_ANDROID
       return callSdkApi<bool>("apiAvailable",code);
#else
        return true;
#endif
    }

    /// <summary>
    /// 获取充值记录数据
    /// </summary>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    public static void payHistory(int page, int pageSize)
    {
#if UNITY_ANDROID
        callSdkApi("payHistory", page, pageSize);
#elif UNITY_IPHONE
        __spsdk_payHistory(page, pageSize);
#endif
    }
    /// <summary>
    /// 充值记录页面
    /// </summary>
    public static void payHistoryView()
    {
#if UNITY_ANDROID
        callSdkApi("payHistoryView");
#elif UNITY_IPHONE
        __spsdk_payHistoryView();
#endif
    }
    /// <summary>
    /// 随意购
    /// </summary>
    /// <param name="goodsId">物品ID，通过goods获取</param>
    /// <param name="totalPrice">总价格，通过goods获取</param>
    /// <param name="pushInfo">透传参数</param>
    /// <param name="balance">用户元宝</param>
    /// <param name="vipLevel">vip等级</param>
    public static void payWilful(string goodsId, double totalPrice, string pushInfo, int balance, int vipLevel)
    {
#if UNITY_ANDROID
        callSdkApi("payWilful", goodsId, totalPrice, pushInfo, balance, vipLevel);
#elif UNITY_IPHONE
        __spsdk_payWilful(goodsId, totalPrice, pushInfo, balance, vipLevel);
#endif
    }
    /// <summary>
    /// 购买
    /// </summary>
    /// <param name="goodsId"></param>
    /// <param name="pushInfo"></param>
    /// <param name="balance"></param>
    /// <param name="vipLevel"></param>
    public static void pay(string goodsId, string pushInfo, int balance, int vipLevel)
    {
#if UNITY_ANDROID
        callSdkApi("pay", goodsId, pushInfo, balance, vipLevel);
#elif UNITY_IPHONE
        __spsdk_pay(goodsId, pushInfo, balance, vipLevel);
#endif
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="goods"></param>
    /// <param name="balance"></param>
    /// <param name="vipLevel"></param>
    public static void pay(Goods goods, int balance, int vipLevel)
    {
#if UNITY_ANDROID
        callSdkApi("pay", goods.getGoodsName(), goods.getGoodsNumber(), goods.getGoodsId(), goods.getGoodsRegisterId(), goods.getGoodsPrice(), goods.getGoodsIcon(), goods.getGoodsDescribe(), goods.getPushInfo(), goods.getType(), balance, vipLevel);
#elif UNITY_IPHONE
        __spsdk_payGood(goods.getGoodsName(), goods.getGoodsNumber(), goods.getGoodsId(), goods.getGoodsRegisterId(), goods.getGoodsPrice(), goods.getGoodsIcon(), goods.getGoodsDescribe(), goods.getPushInfo(), goods.getType(), balance, vipLevel);
#endif
    }


    #region 以下5个渠道接口需要先使用 apiAvailable 方法判断 是否可用
    public static void exit()
    {
#if UNITY_ANDROID
        callSdkApi("exit");
#elif UNITY_IPHONE
        __spsdk_exit();
#endif

    }

    public static void logout()
    {
#if UNITY_ANDROID
        callSdkApi("logout");
#elif UNITY_IPHONE
        __spsdk_logout();
#endif
    }

    public static void switchUser()
    {
#if UNITY_ANDROID
        callSdkApi("switchUser");
#elif UNITY_IPHONE
        __spsdk_switchUser();
#endif
    }

    public static void userCenter()
    {
#if UNITY_ANDROID
        callSdkApi("userCenter");
#elif UNITY_IPHONE
        __spsdk_userCenter();
#endif
    }

    public static void serviceCenter()
    {
#if UNITY_ANDROID
        callSdkApi("serviceCenter");
#elif UNITY_IPHONE
        __spsdk_serverCenter();
#endif
    }
    #endregion
    /// <summary>
    /// 自定义事件埋点
    /// </summary>
    /// <param name="eventId"></param>
    public static void gameEvent(string eventId)
    {
#if UNITY_ANDROID
        callSdkApi("gameEvent",eventId);
#elif UNITY_IPHONE
        __spsdk_gameEvent(eventId);
#endif
    }

    public static void fps(int fps)
    {
#if UNITY_ANDROID
        callSdkApi("fps", fps);
#elif UNITY_IPHONE
        __spsdk_fps(fps);
#endif
    }
    /// <summary>
    /// 退出游戏
    /// </summary>
    public static void killGame()
    {
#if UNITY_ANDROID
        callSdkApi("killGame");
#elif UNITY_IPHONE
        __spsdk_killgame();
#endif
    }

    public static void showToast(string toast)
    {
#if UNITY_ANDROID
        callSdkApi("showToast",toast);
#elif UNITY_IPHONE
        __spsdk_showToast(toast);
#endif
    }
    /// <summary>
    /// 是否实名函数
    /// </summary>
    public static void isAuthention()
    {
#if UNITY_ANDROID
        callSdkApi("isAuthention");
#elif UNITY_IPHONE
        __spsdk_isAuthention();
#endif
    }

    public static void showBindingView()
    {

#if UNITY_ANDROID
        callSdkApi("showBindingView");
#elif UNITY_IPHONE
         __spsdk_showBindingView();
#endif
    }
    /// <summary>
    /// 第三方渠道需要橙柚绑定实名时 调用
    /// </summary>
    /// <param name="param"></param>
    public static void otherVerify(string param)
    {
#if UNITY_ANDROID
        callSdkApi("otherVerify", param);
#endif
    }

    public static void QRCancel()
    {
        //仅windows版本使用
    }

    public static void scan()
    {
#if UNITY_ANDROID
        callSdkApi("currencyReqMetJson", "ScanCodePlugin", "StartScanCode", "StartScanCode");
#endif
    }

    public static void currencyReqMet(string pluginId, string methodName, string param)
    {
#if UNITY_ANDROID
        callSdkApi("currencyReqMetJson", pluginId, methodName, param);
#endif
    }

    public static void channelExtend(string param1, string param2)
    {
#if UNITY_ANDROID
        callSdkApi("channelExtend", param1, param2);
#endif
    }

    public static bool IsSupportLogout()
	{
		#if UNITY_ANDROID
		if(cls != null)
		{
		return cls.CallStatic<bool>("IsSupportLogout");
		}
		else
		{
		return false;
		}
		#elif UNITY_IPHONE
		return true;
		#else
		return true;
		#endif
	}

    public static bool DoExiter()
    {
#if UNITY_ANDROID
        if (cls != null)
        {
			callSdkApi("DoExiter");
        }
        else
        {
            return true;
        }
#else
#endif
        return false;
    }

	public static void Regster(string account, string uid)
	{
		#if UNITY_ANDROID
		callSdkApi("Regster", account, uid);
		#elif UNITY_IPHONE

		#endif
	}

	public static void UpdateUserInfo(string uid)
	{
#if UNITY_ANDROID
		callSdkApi("UpdateUserInfo", uid);
#elif UNITY_IPHONE

#endif
	}

	public static void SubmitRoleData(string uid, bool newRole, string playerId, string playerName, string playerLv, string serverId, string serverName)
    {
#if UNITY_ANDROID
		callSdkApi("SubmitRoleData", uid, newRole, playerId, playerName, playerLv, serverId, serverName);
#elif UNITY_IPHONE
		__spsdk_submitRoleData(uid, newRole, playerId, playerName, playerLv, serverId, serverName);
#endif
    }


    public static string GetChannelId()
    {
#if UNITY_ANDROID
			if(cls != null)
			{
			return cls.CallStatic<string>("GetChannelId");
			}
#elif UNITY_IPHONE
        return __spsdk_getChannelId();
#endif
        return null;
    }


    public static bool IsSupportUserCenter()
    {
#if UNITY_ANDROID
			if(cls != null)
			{
			return cls.CallStatic<bool>("IsSupportUserCenter");
			}
			else
			{
			return false;
			}
#elif UNITY_IPHONE
        return __spsdk_isSupportUserCenter();
#else
        return false;
#endif
    }


    public static void EnterUserCenter()
    {
#if UNITY_ANDROID
		callSdkApi("EnterUserCenter");
#elif UNITY_IPHONE
		__spsdk_enterUserCenter();
#else
        return;
#endif
    }


    public static bool IsSupportBBS()
    {
#if UNITY_ANDROID
			if(cls != null)
			{
			return cls.CallStatic<bool>("IsSupportBBS");
			}
#elif UNITY_IPHONE
			return __spsdk_isSupportBBS();
#endif
        return false;
    }


    public static void EnterSdkBBS()
    {
#if UNITY_ANDROID
		callSdkApi("EnterSdkBBS");
#elif UNITY_IPHONE
		__spsdk_enterSdkBBS();
#else
        return;
#endif
    }

    public static bool IsSupportShowOrHideToolbar()
    {
#if UNITY_ANDROID
		if(cls != null)
		{
			return cls.CallStatic<bool>("IsSupportShowOrHideToolbar");
		}
		else
		{
			return false;
		}
#else
        return false;
#endif
    }


    public static void ShowFloatToolBar()
    {
#if UNITY_ANDROID
		callSdkApi("ShowFloatToolBar");
#else
        return;
#endif
    }

    public static void HideFloatToolBar()
    {
#if UNITY_ANDROID
		callSdkApi("HideFloatToolBar");
#else
        return;
#endif
    }

    //Unity初始化完成
    internal static void UnityInitFinish()
    {

#if UNITY_ANDROID
        try
        {
            //unity初始化完成，通知android层，android层去掉自绘闪屏
            callSdkApi("UnityInitFinish");
        }
        catch (Exception e)
        {
            //旧版本(0.0.1版本及之前无此接口)
        }
#endif
    }

    #region configInteface
    public static bool isLog()
    {
#if UNITY_EDITOR
        return true;
#elif UNITY_IPHONE
        return __spsdk_isLog();
#elif UNITY_ANDROID
        return callSdkApi<bool>("isLog");
#else
        return true;
#endif
    }

    public static string ip()
    {
#if UNITY_EDITOR
        return "";
#elif UNITY_IPHONE
          return __spsdk_ip();
#elif UNITY_ANDROID
        return callSdkApi<string>("ip");
#else
        return "";
#endif
    }

    public static int mode()
    {
#if UNITY_EDITOR
        return 0;
#elif UNITY_IPHONE
        return __spsdk_mode();
#elif UNITY_ANDROID
        return callSdkApi<int>("mode");
#else
        return 0;
#endif
    }

    public static bool isLandscape()
    {
#if UNITY_EDITOR
        return true;
#elif UNITY_IPHONE
         return __spsdk_isLandscape();
#elif UNITY_ANDROID
        return callSdkApi<bool>("isLandscape");
#else
        return true;
#endif
    }

    public static string appVersionName()
    {
#if UNITY_EDITOR
        return "";
#elif UNITY_IPHONE
        return __spsdk_appVersionName();
#elif UNITY_ANDROID
        return callSdkApi<string>("appVersionName");
#else
        return "";
#endif
    }

    public static int appVersionCode()
    {
#if UNITY_EDITOR
        return 1;
#elif UNITY_IPHONE
        return __spsdk_appVersionCode();
#elif UNITY_ANDROID
        return callSdkApi<int>("appVersionCode");
#else
        return 1;
#endif
    }

    public static string deviceId()
    {
#if UNITY_EDITOR
        return "";
#elif UNITY_IPHONE
           return __spsdk_deviceId();
#elif UNITY_ANDROID
        return callSdkApi<string>("deviceId");
#else
        return "";
#endif
    }

    public static string sdkVersionName()
    {
#if UNITY_EDITOR
        return "";
#elif UNITY_IPHONE
        return __spsdk_sdkVersionName();
#elif UNITY_ANDROID
        return callSdkApi<string>("sdkVersionName");
#else
        return "";
#endif
    }

    public static int sdkVersionCode()
    {

#if UNITY_EDITOR
        return 1;
#elif UNITY_IPHONE
        return __spsdk_sdkVersionCode();
#elif UNITY_ANDROID
        return callSdkApi<int>("sdkVersionCode");
#else
        return 1;
#endif
    }

    public static string channelVersion()
    {

#if UNITY_EDITOR
        return "";
#elif UNITY_IPHONE
        return __spsdk_channelVersion();
#elif UNITY_ANDROID
        return callSdkApi<string>("channelVersion");
#else
        return "";
#endif
    }

    public static string appKey()
    {

#if UNITY_EDITOR
        return "1417605600013";
#elif UNITY_IPHONE
        return __spsdk_appKey();
#elif UNITY_ANDROID
        return callSdkApi<string>("appKey");
#else
        return "1417605600013";
#endif
    }

    public static string appSecret()
    {

#if UNITY_EDITOR
        return "95c505cf187f44dc8c450e0b88fc1672";
#elif UNITY_IPHONE
        return __spsdk_appSecret();
#elif UNITY_ANDROID
        return callSdkApi<string>("appSecret");
#else
        return "95c505cf187f44dc8c450e0b88fc1672";
#endif
    }

    public static string channelId()
    {
#if UNITY_EDITOR
        return "4001";
#elif UNITY_IPHONE
         return __spsdk_channelId();
#elif UNITY_ANDROID
        return callSdkApi<string>("channelId");
#else
        return "4001";
#endif
    }

    public static string cmbiChannelId()
    {
#if UNITY_EDITOR
        return "";
#elif UNITY_IPHONE
        return __spsdk_cmbiChannelId();
#elif UNITY_ANDROID
        return callSdkApi<string>("cmbiChannelId");
#else
        return "";
#endif
    }

    public static string appName()
    {
#if UNITY_EDITOR
        return "";
#elif UNITY_IPHONE
          return __spsdk_appName();
#elif UNITY_ANDROID
        return callSdkApi<string>("appName");
#else
        return "";
#endif
    }

    public static string networkType()
    {
#if UNITY_EDITOR
        return "";
#elif UNITY_IPHONE
         return __spsdk_networkType();
#elif UNITY_ANDROID
        return callSdkApi<string>("networkType");
#else
        return "";
#endif
    }
#endregion
}
