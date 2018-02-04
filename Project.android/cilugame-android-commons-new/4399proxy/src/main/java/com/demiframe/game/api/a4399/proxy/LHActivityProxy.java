package com.demiframe.game.api.a4399.proxy;

import android.app.Activity;
import android.content.Intent;
import android.content.pm.ActivityInfo;
import android.content.res.Configuration;

import com.demiframe.game.api.common.LHCallbackListener;
import com.demiframe.game.api.common.LHResult;
import com.demiframe.game.api.common.LHStaticValue;
import com.demiframe.game.api.common.LHStatusCode;
import com.demiframe.game.api.common.LHUser;
import com.demiframe.game.api.connector.IActivity;
import com.demiframe.game.api.connector.ICheckSupport;
import com.demiframe.game.api.connector.IInitCallback;
import com.demiframe.game.api.util.LHCheckSupport;
import com.demiframe.game.api.util.LogUtil;
import com.demiframe.game.api.util.SDKTools;

import cn.m4399.operate.OperateCenter;
import cn.m4399.operate.OperateCenterConfig;
import cn.m4399.operate.User;

public class LHActivityProxy
        implements IActivity {
    public LHActivityProxy() {
        LHCheckSupport.setCheckSupport(new ICheckSupport() {
            public boolean isAntiAddictionQuery() {
                return false;
            }

            public boolean isSupportBBS() {
                return false;
            }

            public boolean isSupportLogout() {
                return true;
            }

            public boolean isSupportOfficialPlacard() {
                return false;
            }

            public boolean isSupportShowOrHideToolbar() {
                return false;
            }

            public boolean isSupportUserCenter() {
                return false;
            }
        });
    }

    public void onActivityResult(Activity activity, int requestCode, int resultCode, Intent intentData) {
    }

    public void onCreate(Activity activity, final IInitCallback initCallback, Object obj) {
        OperateCenter opeCenter = OperateCenter.getInstance();
        int orientation = ActivityInfo.SCREEN_ORIENTATION_LANDSCAPE;
        if(!LHStaticValue.IsLandScape){
            orientation = ActivityInfo.SCREEN_ORIENTATION_PORTRAIT;
        }
        String appKey = SDKTools.GetSdkProperty(activity, "4399_APPKEY");
        boolean isDebug = SDKTools.GetSdkBoolProperty(activity, "4399_DEBUG", false);
        // 配置sdk属性,比如可扩展横竖屏配置
        OperateCenterConfig.Builder builder = new OperateCenterConfig.Builder(activity)
                .setOrientation(orientation)
                .setPopLogoStyle(OperateCenterConfig.PopLogoStyle.POPLOGOSTYLE_ONE)
                .setPopWinPosition(OperateCenterConfig.PopWinPosition.POS_LEFT)
                .setSupportExcess(false)
                .setGameKey(appKey)
                .setDebugEnabled(isDebug);
        opeCenter.setConfig(builder.build());

        opeCenter.init(activity, new OperateCenter.OnInitGloabListener() {
            // 初始化结束执行后回调
            @Override
            public void onInitFinished(boolean isLogin, User userInfo) {
                LHResult lhResult = new LHResult();
                lhResult.setCode(LHStatusCode.LH_INIT_SUCCESS);
                initCallback.onFinished(lhResult);
            }

            // 注销帐号的回调， 包括个人中心里的注销和logout()注销方式
            @Override
            public void onUserAccountLogout(boolean fromUserCenter, int resultCode) {
                LogUtil.d("onUserAccountLogout resultCode:"+resultCode);
                if(LHCallbackListener.getInstance().getUserListener() != null){
                    LHCallbackListener.getInstance().getUserListener().onLogout(LHStatusCode.LH_LOGOUT_SUCCESS,
                            "onLogoutSucc");
                }
            }

            // 个人中心里切换帐号的回调
            @Override
            public void onSwitchUserAccountFinished(User userInfo) {
                if(LHCallbackListener.getInstance().getUserListener() == null){
                    return;
                }

                LHUser lhUser = new LHUser();
                String sid = userInfo.getUid() + LHStaticValue.demiSplit + userInfo.getState();
                lhUser.setUid(userInfo.getUid());
                lhUser.setSid(sid);
                LHCallbackListener.getInstance().getUserListener().onLogin(LHStatusCode.LH_LOGIN_SUCCESS, lhUser);
            }
        });
    }

    public void afterOnCreate(Activity activity, IInitCallback listener, Object obj){
        
    }

    public Boolean needAfterCreate(){
        return false;
    }

    public void onDestroy(Activity activity) {
        if(OperateCenter.getInstance() != null){
            OperateCenter.getInstance().destroy();
        }
    }

    public void onNewIntent(Activity activity, Intent intent) {
    }

    public void onPause(Activity activity) {
    }

    public void onRestart(Activity activity) {
    }

    public void onResume(Activity activity) {
    }

    public void onStop(Activity activity) {
    }

    public void onStart(Activity activity){

    }

    public void onConfigurationChanged(Activity activity, Configuration newConfig){

    }
}