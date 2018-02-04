package com.demiframe.game.api.oppo.proxy;

import android.app.Activity;
import android.content.Intent;
import android.content.res.Configuration;

import com.demiframe.game.api.common.LHCallbackListener;
import com.demiframe.game.api.common.LHResult;
import com.demiframe.game.api.common.LHStatusCode;
import com.demiframe.game.api.connector.IActivity;
import com.demiframe.game.api.connector.ICheckSupport;
import com.demiframe.game.api.connector.IInitCallback;
import com.demiframe.game.api.util.LHCheckSupport;
import com.nearme.game.sdk.GameCenterSDK;
import com.nearme.game.sdk.callback.ApiCallback;

public class LHActivityProxy
        implements IActivity {
    public LHActivityProxy() {
        //是否需要防沉迷
        LHCheckSupport.setCheckSupport(new ICheckSupport() {
            public boolean isAntiAddictionQuery() {
                return true;
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
                return true;
            }
        });
    }

    public void onActivityResult(Activity activity, int paramInt1, int paramInt2, Intent paramIntent) {
    }

    public void onCreate(Activity activity, IInitCallback initCallback, Object obj) {
        if (initCallback != null) {
            LHResult localLHResult = new LHResult();
            localLHResult.setCode(LHStatusCode.LH_INIT_SUCCESS);
            localLHResult.setData("初始化成功");
            initCallback.onFinished(localLHResult);
        }
    }

    public void afterOnCreate(Activity activity, IInitCallback listener, Object obj){
    }

    public Boolean needAfterCreate(){
        return false;
    }

    public void onDestroy(Activity activity) {
    }

    public void onNewIntent(Activity activity, Intent paramIntent) {
    }

    public void onPause(Activity activity) {
        GameCenterSDK.getInstance().onPause();
    }

    public void onRestart(Activity activity) {
    }

    public void onResume(Activity activity) {
        GameCenterSDK.getInstance().onResume(activity);
    }

    public void onStop(Activity activity) {
    }

    public void onStart(Activity activity){

    }

    public void onConfigurationChanged(Activity activity, Configuration newConfig){

    }
}