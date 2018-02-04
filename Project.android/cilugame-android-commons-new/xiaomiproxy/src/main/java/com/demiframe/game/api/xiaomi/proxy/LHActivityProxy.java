package com.demiframe.game.api.xiaomi.proxy;

import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.content.res.Configuration;

import com.demiframe.game.api.common.LHResult;
import com.demiframe.game.api.common.LHStatusCode;
import com.demiframe.game.api.connector.IActivity;
import com.demiframe.game.api.connector.ICheckSupport;
import com.demiframe.game.api.connector.IInitCallback;
import com.demiframe.game.api.util.IOUtil;
import com.demiframe.game.api.util.LHCheckSupport;
import com.demiframe.game.api.util.LogUtil;
import com.demiframe.game.api.util.SDKTools;
import com.demiframe.game.api.xiaomi.base.User;
import com.xiaomi.gamecenter.sdk.MiCommplatform;
import com.xiaomi.gamecenter.sdk.entry.LoginResult;
import com.xiaomi.gamecenter.sdk.entry.MiAppInfo;
import com.xiaomi.gamecenter.sdk.entry.MiAppType;

public class LHActivityProxy implements IActivity
{
    public LHActivityProxy()
    {
        LHCheckSupport.setCheckSupport( new ICheckSupport()
        {
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
                return true;
            }

            public boolean isSupportShowOrHideToolbar() {
                return false;
            }

            public boolean isSupportUserCenter() {
                return false;
            }
        });
    }

    public void onActivityResult(Activity activity, int paramInt1, int paramInt2, Intent paramIntent) {
    }

    public void onCreate(Activity activity, IInitCallback initCallback, Object obj)
    {
        LogUtil.d("LHActivityProxy", "XIAO MI Init");
        MiAppInfo appInfo = new MiAppInfo();
        appInfo.setAppId(SDKTools.GetSdkProperty(activity, "XIAOMI_APPID"));
        appInfo.setAppKey(SDKTools.GetSdkProperty(activity, "XIAOMI_APPKEY"));
        appInfo.setAppType(MiAppType.online);
        MiCommplatform.Init(activity, appInfo);

        LHResult lhResult = new LHResult();
        lhResult.setCode(LHStatusCode.LH_INIT_SUCCESS);
        initCallback.onFinished(lhResult);
        LogUtil.d("LHActivityProxy", "XIAO MI Init Finish");
    }

    public void afterOnCreate(Activity activity, IInitCallback listener, Object obj){
        
    }

    public Boolean needAfterCreate(){
        return false;
    }

    public void onDestroy(Activity activity) {
        User.getInstances().Clear();
    }

    public void onNewIntent(Activity activity, Intent paramIntent) {
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