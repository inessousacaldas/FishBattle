package com.demiframe.game.api.nubiya.proxy;

import android.app.Activity;
import android.content.Intent;
import android.content.res.Configuration;
import android.os.Bundle;

import com.demiframe.game.api.common.LHResult;
import com.demiframe.game.api.common.LHStaticValue;
import com.demiframe.game.api.common.LHStatusCode;
import com.demiframe.game.api.connector.IActivity;
import com.demiframe.game.api.connector.ICheckSupport;
import com.demiframe.game.api.connector.IInitCallback;
import com.demiframe.game.api.util.IOUtil;
import com.demiframe.game.api.util.LHCheckSupport;
import com.demiframe.game.api.util.LogUtil;
import com.demiframe.game.api.util.SDKTools;

import cn.nubia.componentsdk.constant.ErrorCode;
import cn.nubia.nbgame.sdk.GameSdk;
import cn.nubia.nbgame.sdk.entities.AppInfo;
import cn.nubia.nbgame.sdk.interfaces.CallbackListener;


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
                return false;
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
        if (initCallback != null)
        {
            AppInfo appInfo = new AppInfo();
            appInfo.setAppId(Integer.parseInt(SDKTools.GetSdkProperty(activity, "NUBIYA_APPID")));
            appInfo.setAppKey(SDKTools.GetSdkProperty(activity, "NUBIYA_APPKEY"));
            int orientation = 0;
            if(!LHStaticValue.IsLandScape){
                orientation = 1;
            }
            appInfo.setOrientation(orientation);//0：横屏；1：竖屏
            appInfo.setCanUseAdjunct(true);//设置是否显示游戏小号切换

            GameSdk.initSdk(activity.getApplication(), appInfo, new CallbackListener<Bundle>() {
                @Override
                public void callback(int responseCode, Bundle bundle)
                {
                    LHResult localLHResult = new LHResult();
                    if (responseCode == ErrorCode.SUCCESS) {
                        LogUtil.e("nubiya sdk init success");

                        localLHResult.setData("初始化成功");
                        localLHResult.setCode(LHStatusCode.LH_INIT_SUCCESS);
                    }
                    //在登录时会自动先调用初始化接口保证初始化成功。如出现初始化失败代码36，
                    // 表示联运组件未安装，用户第一次使用集成联运组件的游戏时会报此错误码，
                    // 此错误码可以不用处理，请直接调用登录接口即可，会在登录接口自动检测并且提示安装游戏组件。
                    else if(responseCode == 36){
                        LogUtil.e("nubiya sdk init code 36");

                        localLHResult.setData("初始化code 36");
                        localLHResult.setCode(LHStatusCode.LH_INIT_SUCCESS);
                    }
                    else {
                        LogUtil.e("nubiya sdk init fail, errorCode:" + responseCode);

                        localLHResult.setData("初始化失败");
                        localLHResult.setCode(LHStatusCode.LH_INIT_FAIL);
                    }
                    initCallback.onFinished(localLHResult);

                }
            });



        }
    }

    public void afterOnCreate(Activity activity, IInitCallback listener, Object obj){
        
    }

    public Boolean needAfterCreate(){
        return true;
    }

    public void onDestroy(Activity activity) {
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