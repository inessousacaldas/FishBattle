package com.demiframe.game.api.uc.ucutils;

import android.app.Activity;
import android.content.pm.ActivityInfo;
import cn.uc.gamesdk.open.GameParamInfo;
import cn.uc.gamesdk.open.UCOrientation;
import cn.uc.gamesdk.param.SDKParamKey;
import cn.uc.gamesdk.param.SDKParams;

public class LHUCGameSdk {
    private final static String TAG = "UCGameSdk";
    public final static int ORIENTATION_PORTRAIT = 0;//竖屏
    public final static int ORIENTATION_LANDSCAPE = 1;//横屏

    /**
     * 初始化 初始化SDK
     *
     * @param debugMode        是否联调模式， false=连接SDK的正式生产环境，true=连接SDK的测试联调环境
     * @param gameId           游戏ID，该ID由UC游戏中心分配，唯一标识一款游戏
     * @param enablePayHistory 是否启用支付查询功能
     * @param enableUserChange 是否启用账号切换功能
     * @param orientation      游戏横竖屏设置 0：竖屏 1：横屏
     */
    public static void initSDK(final Activity activity, final boolean debugMode, int gameId, boolean enablePayHistory,
                               boolean enableUserChange, int orientation, String pullupInfo) {
        final GameParamInfo gameParamInfo = new GameParamInfo();
        gameParamInfo.setGameId(gameId);

        gameParamInfo.setEnablePayHistory(enablePayHistory);
        gameParamInfo.setEnableUserChange(enableUserChange);

        if (ORIENTATION_PORTRAIT == orientation) {
            gameParamInfo.setOrientation(UCOrientation.PORTRAIT);
        } else if (ORIENTATION_LANDSCAPE == orientation) {
            gameParamInfo.setOrientation(UCOrientation.LANDSCAPE);
        } else {
            if (activity.getRequestedOrientation() == ActivityInfo.SCREEN_ORIENTATION_PORTRAIT) {
                gameParamInfo.setOrientation(UCOrientation.PORTRAIT);
            } else {
                gameParamInfo.setOrientation(UCOrientation.LANDSCAPE);
            }
        }

        final SDKParams sdkParams = new SDKParams();
        sdkParams.put(SDKParamKey.GAME_PARAMS, gameParamInfo);
        sdkParams.put(SDKParamKey.DEBUG_MODE, debugMode);

        try {
            cn.uc.gamesdk.UCGameSdk.defaultSdk().initSdk(activity, sdkParams);
        } catch (Throwable e) {
            e.printStackTrace();
        }
    }

    /**
     * 调用SDK的用户登录
     */
    public static void login(final Activity activity) {
        try {
            cn.uc.gamesdk.UCGameSdk.defaultSdk().login(activity, null);
        } catch (Throwable e) {
            e.printStackTrace();
        }

    }

    /**
     * 退出当前登录的账号
     */
    public static void logout(final Activity activity) {
        try {
            cn.uc.gamesdk.UCGameSdk.defaultSdk().logout(activity, null);
        } catch (Throwable e) {
            e.printStackTrace();
        }
    }


    /**
     * 提交玩家选择的游戏分区及角色信息
     *
     * @param zoneId    区服ID
     * @param zoneName  区服名称
     * @param roleId    角色编号
     * @param roleName  角色名称
     * @param roleCTime 角色创建时间(单位：秒)，长度10，获取服务器存储的时间，不可用手机本地时间
     */
    public static void submitRoleData(final Activity activity, String zoneId, final String zoneName,
                                      final String roleId, final String roleName, long roleLevel, long roleCTime) {
        final SDKParams sdkParams = new SDKParams();
        sdkParams.put(SDKParamKey.STRING_ROLE_ID, roleId);
        sdkParams.put(SDKParamKey.STRING_ROLE_NAME, roleName);
        sdkParams.put(SDKParamKey.LONG_ROLE_LEVEL, roleLevel);
        sdkParams.put(SDKParamKey.STRING_ZONE_ID, zoneId);
        sdkParams.put(SDKParamKey.STRING_ZONE_NAME, zoneName);
        sdkParams.put(SDKParamKey.LONG_ROLE_CTIME, roleCTime);

        try {
            cn.uc.gamesdk.UCGameSdk.defaultSdk().submitRoleData(activity, sdkParams);
        } catch (Throwable e) {
            e.printStackTrace();
        }
    }

    /**
     * 执行充值下单操作，此操作会调出充值界面。
     *
     * @param amount       充值金额。默认为0，如果不设或设为0，充值时用户从充值界面中选择或输入金额；如果设为大于0的值，表示固定充值金额，
     *                     不允许用户选择或输入其它金额。
     * @param serverId     当前充值的游戏服务器（分区）标识，此标识即UC分配的游戏服务器ID
     * @param roleId       当前充值用户在游戏中的角色标识
     * @param roleName     当前充值用户在游戏中的角色名称
     * @param grade        当前充值用户在游戏中的角色等级
     * @param callbackInfo cp自定义信息，在支付结果通知时回传,CP可以自己定义格式,长度不超过250
     * @param notifyUrl    支付回调通知URL
     * @param signType     签名类型
     * @param sign         签名结果
     */
    public static void pay(final Activity activity, String accountId, String cpOrderID, String amount, String callbackInfo, String notifyUrl, String signType, String sign) {
        final SDKParams sdkParams = new SDKParams();

//        if (!callbackInfo.isEmpty()) {
//            sdkParams.put(SDKParamKey.CALLBACK_INFO, callbackInfo);
//        }
        sdkParams.put(SDKParamKey.CALLBACK_INFO, callbackInfo);
        sdkParams.put(SDKParamKey.AMOUNT, amount);
        sdkParams.put(SDKParamKey.NOTIFY_URL, notifyUrl);
        sdkParams.put(SDKParamKey.CP_ORDER_ID, cpOrderID);
        sdkParams.put(SDKParamKey.ACCOUNT_ID, accountId);
        sdkParams.put(SDKParamKey.SIGN_TYPE, signType);
        sdkParams.put(SDKParamKey.SIGN, sign);

        try {
            cn.uc.gamesdk.UCGameSdk.defaultSdk().pay(activity, sdkParams);
        } catch (Throwable e) {
            e.printStackTrace();
        }
    }

    /**
     * 退出SDK，游戏退出前必须调用此方法，以清理SDK占用的系统资源。如果游戏退出时不调用该方法，可能会引起程序错误。
     */
    public static void exitSDK(final Activity activity) {
        try {
            cn.uc.gamesdk.UCGameSdk.defaultSdk().exit(activity, null);
        } catch (Throwable e) {
            e.printStackTrace();
        }
    }

}
