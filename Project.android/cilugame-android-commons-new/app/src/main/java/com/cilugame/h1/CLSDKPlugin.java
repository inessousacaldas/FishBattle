package com.cilugame.h1;

import com.cilugame.h1.util.Logger;
import com.demiframe.game.api.util.LogUtil;
import com.unity3d.player.UnityPlayer;

import com.demiframe.game.api.GameApi;
import com.demiframe.game.api.common.LHPayInfo;
import com.demiframe.game.api.common.LHResult;
import com.demiframe.game.api.common.LHRole;
import com.demiframe.game.api.common.LHRoleDataType;
import com.demiframe.game.api.common.LHStatusCode;
import com.demiframe.game.api.common.LHUser;
import com.demiframe.game.api.connector.IExitCallback;
import com.demiframe.game.api.connector.IExitSdk;
import com.demiframe.game.api.connector.IHandleCallback;
import com.demiframe.game.api.connector.IInitCallback;
import com.demiframe.game.api.connector.IUserListener;
import com.demiframe.game.api.util.LHCheckSupport;

import android.content.res.Resources;

import org.json.JSONObject;

public class CLSDKPlugin {
    //版本号
    private static String version = "0.1.0";

    public static LHUser loginUser;

    private static int InitFlag = -1; //-1未执行，0成功，1失败, 2等待中
    private static boolean HasSetup = false;

    private static IInitCallback initCallback = new IInitCallback() {
        public void onFinished(LHResult arg0) {
            switch (arg0.getCode()) {
                case LHStatusCode.LH_INIT_SUCCESS:
                    // SDK初始化成功
                    InitFlag = 0;
                    Logger.Log("LH_SETUP_SUCCESS");
                    UnityCallbackWrapper.OnInit(true);
                    break;
                case LHStatusCode.LH_INIT_FAIL:
                default:
                    // SDK初始化失败
                    InitFlag = 1;
                    Logger.Log("LH_SETUP_FAIL");
                    UnityCallbackWrapper.OnInit(false);
                    break;
            }
        }
    };

    public static void Setup()
    {
        if (HasSetup)
        {
            return;
        }

        Logger.Log("Call Setup");

        HasSetup = true;

        //设置游戏名字
        Resources localResources = UnityPlayerActivity.instance.getResources();
        CharSequence charSequence = localResources.getText(localResources.getIdentifier("app_name", "string", UnityPlayerActivity.instance.getPackageName()));
        Logger.Log("setAppName=" + charSequence.toString());
        GameApi.getInstance().setAppName(charSequence.toString());

        //有些SDK需要延后初始化
        if(!GameApi.getInstance().needAfterCreate(UnityPlayerActivity.instance)){
            InitFlag = 2;
            GameApi.getInstance().onCreate(UnityPlayerActivity.instance, initCallback, "init");
        }
        else{
            GameApi.getInstance().InitChannelInfo(UnityPlayerActivity.instance);
        }


        GameApi.getInstance().setUserListener(UnityPlayerActivity.instance,
                new IUserListener() {
                    public void onLogout(int arg0, Object arg1) {
                        // TODO Auto-generated method stub
                        switch (arg0) {
                            case LHStatusCode.LH_LOGOUT_SUCCESS:
                                Logger.Log("LH_LOGOUT_SUCCESS");
                                loginUser = null;
                                UnityCallbackWrapper.OnLoginOut(true);
                                break;
                            default:
                                UnityCallbackWrapper.OnLoginOut(false);
                                break;
                        }
                    }

                    @Override
                    public void onLogin(int arg0, LHUser lhUser) {
                        switch (arg0) {
                            case LHStatusCode.LH_LOGIN_SUCCESS:
                                Logger.Log("LH_LOGIN_SUCCESS");
                                loginUser = lhUser;
                                JSONObject jsonObject = new JSONObject();
                                try{
                                    jsonObject.put("sessionId", loginUser.getSid());
                                    if(loginUser.getUid() != null){
                                        jsonObject.put("uid", loginUser.getUid());//可能是渠道唯一id，也可能是代理商的唯一id,可能为空
                                    }
                                    //支付额外字段
                                    if(loginUser.getPayExt() != null){
                                        jsonObject.put("payExt", loginUser.getPayExt());
                                    }

                                }catch(Exception e){
                                    e.printStackTrace();
                                    UnityCallbackWrapper.OnLoginFail();
                                    return;
                                }

                                UnityCallbackWrapper.OnLoginSuccess(jsonObject.toString());
                                break;

                            case LHStatusCode.LH_LOGIN_CANCEL:
                                Logger.Log("LH_LOGIN_CANCEL");
                                UnityCallbackWrapper.OnLoginCancel();
                                break;
                            case LHStatusCode.LH_LOGIN_FAIL:
                                Logger.Log("LH_LOGIN_FAIL");
                                UnityCallbackWrapper.OnLoginFail();
                                break;
                        }
                    }
                });
    }

    private static boolean needInitCallback = false;
    public static void Init()
    {
        Logger.Log("Call Init");

        if (InitFlag == 0)
        {
            Logger.Log("LHStatusCode.LH_INIT_SUCCESS");
            UnityCallbackWrapper.OnInit(true);
        }
        //初始化失败情况，再次执行初始化
        else if(InitFlag == 1 || InitFlag == -1)
        {
            InitFlag = 2;
            GameApi.getInstance().onCreate(UnityPlayerActivity.instance, initCallback, "init");
        }
    }

    //部分渠道需要延后初始化
    public static void AfterInit(){
        if(InitFlag == -1 && GameApi.getInstance().needAfterCreate(UnityPlayerActivity.instance)){
            InitFlag = 2;
            GameApi.getInstance().onCreate(UnityPlayerActivity.instance, initCallback, "init");
        }
    }

    /*
     * SDK 登录
     */
    public static void Login() {
        Logger.Log("Call Login");
        GameApi.gameApi.login(UnityPlayerActivity.instance, "login");
    }

    public static boolean IsSupportLogout() {
        Logger.Log("Call IsSupportLogout");
        return LHCheckSupport.getCheckSupport().isSupportLogout();
    }

    /*
     * SDK 登出
     */
    public static void Logout() {
        Logger.Log("Call logout");
        GameApi.gameApi.logout(UnityPlayerActivity.instance, "logout");
    }

    /*
     * SDK 更新用户ID
     */
    public static void UpdateUserInfo(String uid)
    {
        Logger.Log("Call UpdateUserInfo");
        if (loginUser != null)
        {
            loginUser.setUid(uid);
            GameApi.gameApi.updateUserInfo(UnityPlayerActivity.instance, loginUser);
        }
    }

    //上传角色数据
    //"submittype": loginRole createrole, upgrade, selectserver
    public static void SubmitRoleData(String roleJsonStr) {
        Logger.Log("Call SubmitRoleData " + roleJsonStr);
        LHRole lhRole = new LHRole();
        try {
            JSONObject roleJson = new JSONObject(roleJsonStr);
            int submitType = roleJson.getInt("submittype") ;
            LHRoleDataType type = LHRoleDataType.values()[submitType];
            lhRole.setDataType(type);
            lhRole.setRoledId(roleJson.getString("roleId"));
            lhRole.setRoleLevel(roleJson.getString("roleLevel"));
            lhRole.setRoleName(roleJson.getString("roleName"));
            lhRole.setZoneId(roleJson.getString("zoneId"));
            lhRole.setZoneName(roleJson.getString("zoneName"));
            lhRole.setRoleCTime(roleJson.getString("roleCTime"));
            if(type != LHRoleDataType.loginRole){
                lhRole.setBalance(roleJson.getString("balance"));
                lhRole.setVipLevel(roleJson.getString("vipLevel"));
                lhRole.setPartyName(roleJson.getString("partyName"));
                lhRole.setRoleLevelMTime(roleJson.getString("roleLevelMTime"));
            }
        }catch (Exception e){
            e.printStackTrace();
        }

        GameApi.gameApi.submitRoleData(UnityPlayerActivity.instance, lhRole);
    }

    //获取游戏币
    public static void GainGameCoin(String jsonStr){
        GameApi.gameApi.GainGameCoin(UnityPlayerActivity.instance, jsonStr);
    }

    //消耗游戏币
    public static void ConsumeGameCoin(String jsonStr){
        GameApi.gameApi.ConsumeGameCoin(UnityPlayerActivity.instance, jsonStr);
    }

    public static String GetChannelId() {
        String currentChannelId =  GameApi.getInstance().getChannelInfo()
                .getChannelId();

        Logger.Log("Call GetChannelId =" + currentChannelId);
        return currentChannelId;
    }

    public static String GetSubChannelId(){
        String subChannelId =  GameApi.getInstance().getChannelInfo()
                .getSubChannelId();

        Logger.Log("Call GetSubChannelId =" + subChannelId);
        return subChannelId;
    }

    public static String GetMutilPackageId(){
        String mutiPackageId = GameApi.getInstance().GetMutilPackageId(UnityPlayerActivity.instance);
        Logger.Log("Call GetMutilPackageId =" + mutiPackageId);
        return mutiPackageId;
    }

    public static String GetGameId(){
        String gameId = GameApi.getInstance().GetGameId(UnityPlayerActivity.instance);
        Logger.Log("Call GetGameId=" + gameId);
        return gameId;
    }

    public static boolean IsSupportUserCenter() {
        Logger.Log("Call IsSupportUserCenter");
        return LHCheckSupport.getCheckSupport().isSupportUserCenter();
    }

    public static void EnterUserCenter() {
        Logger.Log("Call EnterUserCenter");
        GameApi.getInstance().enterUserCenter(UnityPlayerActivity.instance);
    }

    public static boolean IsSupportBBS() {
        Logger.Log("Call IsSupportBBS");
        return LHCheckSupport.getCheckSupport().isSupportBBS();
    }

    public static void EnterSdkBBS() {
        Logger.Log("Call EnterSdkBBS");
        GameApi.getInstance().enterSdkBBS(UnityPlayerActivity.instance);
    }

    public static boolean IsSupportShowOrHideToolbar() {
        Logger.Log("Call IsSupportShowOrHideToolbar");
        return LHCheckSupport.getCheckSupport().isSupportShowOrHideToolbar();
    }

    public static void ShowFloatToolBar() {
        Logger.Log("Call ShowFloatToolBar");
        GameApi.getInstance().showFloatToolBar(UnityPlayerActivity.instance);
    }

    public static void HideFloatToolBar() {
        Logger.Log("Call HideFloatToolBar");
        GameApi.getInstance().hideFloatToolBar(UnityPlayerActivity.instance);
    }

    public static Boolean IsAntiAddictionQuery(){
        return LHCheckSupport.getCheckSupport().isAntiAddictionQuery();
    }

    //实名制
//    public static void RealNameRegister(){
//        Logger.Log("Call RealNameRegister");
//        GameApi.getInstance().realNameRegister(UnityPlayerActivity.instance, new IHandleCallback() {
//            @Override
//            public void onFinished(LHResult lhResult) {
//
//            }
//        });
//    }

    /*
     * 支付
     */
    public static void DoPay(
            final String payJsonStr
            )
    {
        Logger.Log("Call DoPay " + payJsonStr);
        IHandleCallback callback = new IHandleCallback()
        {
            public void onFinished(LHResult lhResult)
            {
                if (lhResult != null)
                {
                    Logger.Log("paycallback code=" + lhResult.getCode());
                    switch (lhResult.getCode())
                    {
                        default:
                            UnityCallbackWrapper.OnPay(1);
                            return;
                        case LHStatusCode.LH_PAY_SUCCESS:
                            UnityCallbackWrapper.OnPay(0);
                            GameApi.getInstance().paySuccessDone(UnityPlayer.currentActivity);
                            return;
                        case LHStatusCode.LH_PAY_CANCEL:
                            UnityCallbackWrapper.OnPay(1);
                            return;
                        case LHStatusCode.LH_PAY_SUBMITED:
                            UnityCallbackWrapper.OnPay(1);
                            return;
                        case LHStatusCode.LH_PAY_FAIL:
                            UnityCallbackWrapper.OnPay(2);
                    }
                }
                else{
                    UnityCallbackWrapper.OnPay(1);
                }
            }
        };
//        Call DoPay {"productCount":"1","productName":"60元宝","payNotifyUrl":"http://dev.h5.cilugame.com/h5/gpayc/pay/sm.json",
//            "channelOrderSerial":"","payKey":"","serverId":"1019","productId":"com.demigame.yhxj.item001",
//            "appOrderId":"522_1019_750974386734895104",
//            "sid":"19meng_ndxo9uhgsaur7rei","appId":"1","productPrice":"6","playerId":103539}
        LHPayInfo lhPayInfo = new LHPayInfo();
        try {
            JSONObject payJson = new JSONObject(payJsonStr);
            lhPayInfo.setOrderSerial(payJson.getString("appOrderId"));
            lhPayInfo.setProductId(payJson.getString("productId"));
            lhPayInfo.setProductName(payJson.getString("productName"));
            lhPayInfo.setProductDes(payJson.getString("productDes"));
            lhPayInfo.setGainGold(payJson.getString("gainGold"));
            lhPayInfo.setProductPrice(payJson.getString("productPrice"));
            lhPayInfo.setProductCount(payJson.getString("productCount"));
            lhPayInfo.setServerId(payJson.getString("serverId"));
            if(payJson.has("payCustomInfo")){
                lhPayInfo.setPayCustomInfo(payJson.getString("payCustomInfo"));
            }
            else{
                lhPayInfo.setPayCustomInfo("");
            }

            //设置回调链接
            try{
                JSONObject extraJson = new JSONObject(payJson.getString("extraJson"));
                String notifyUrl = extraJson.getString("demiPayCallbackURL");
                lhPayInfo.setPayNotifyUrl(notifyUrl);
            }catch (Exception e){
                e.printStackTrace();
                return;
            }

            lhPayInfo.setExtraJson(payJson.getString("extraJson"));

            lhPayInfo.setAppId(payJson.getString("appId"));
            lhPayInfo.setSid(payJson.getString("sid"));
            lhPayInfo.setPlayerId(payJson.getString("playerId"));
            lhPayInfo.setBalance(payJson.getString("balance"));

        }
        catch (Exception e){
            Logger.Error("支付参数错误", e);
            return;
        }


        //Date localDate = new Date();
        //lhPayInfo.setPaySubmitTime(new SimpleDateFormat("yyyyMMddHHmmss").format(localDate));
        Resources localResources = UnityPlayerActivity.instance.getResources();
        CharSequence localCharSequence = localResources.getText(localResources.getIdentifier("app_name", "string", UnityPlayerActivity.instance.getPackageName()));
        Logger.Log("setAppName=" + localCharSequence.toString());
        lhPayInfo.setAppName(localCharSequence.toString());
        lhPayInfo.setPayCallback(callback);
        //lhPayInfo.setCharge(false);
        GameApi.getInstance().onPay(UnityPlayerActivity.instance, lhPayInfo);
    }

    //扩展接口
    public static void Extend(final String jsonStr){
        Logger.Log("Call Extend ");
    }

    /*
	 * SDK 退出
	 */
    public static void DoExiter() {
        Logger.Log("Call DoExiter");

        GameApi.getInstance().onExit(UnityPlayerActivity.instance,
                new IExitCallback() {
                    @Override
                    public void onExit(boolean isExit) {
                        Logger.Log("onExit " + isExit);
                        if (isExit) {
                            //这里收到退出时要主动关闭activy，例如360会受到这个影响导致退出自动重启
                            UnityPlayerActivity.instance.finish();

                            UnityCallbackWrapper.OnExit(true);
                        } else {
                            UnityCallbackWrapper.OnExit(false);
                        }
                    }

                    @Override
                    public void onNoExiterProvide(final IExitSdk exiter) {
                        Logger.Log("onNoExiterProvide");
                        gameExiter = exiter;
                        UnityCallbackWrapper.OnNoExiterProvide();
                    }
                });
    }

    private static IExitSdk gameExiter;

    public static void Exit()
    {
        Logger.Log("Call Exit");

        //这里收到退出时要主动关闭activy，例如360会受到这个影响导致退出自动重启
        UnityPlayerActivity.instance.finish();

        if (gameExiter != null)
        {
            Logger.Log("gameExiter.onExitSdk");
            gameExiter.onExitSdk();
        }
    }

    //暂不提供游戏内的注册接口
    //注册账号
    public static void Regster(String account, String uid)
    {
        Logger.Log("Register not support");
//        //TSIDataSDK.getInstance().regster(account, uid);
    }

    public static String GetVersion(){
        return version;
    }

    public static String GetAppName(){
        String appName = GameApi.getInstance().getAppName();
        Logger.Log("Call GetAppName = "+appName);
        return appName;
    }

    //unity 第一个脚本执行的时候调用（即初始化完毕）
    public static void UnityInitFinish(){
        //移除自绘闪屏
        if(UnityPlayerActivity.instance != null){
            UnityPlayerActivity.instance.UnityInitFinish();
        }
    }
}
