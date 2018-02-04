package com.demiframe.game.api.kaopu.proxy;

import android.app.Activity;
import android.content.Intent;
import android.content.res.Configuration;

import com.demiframe.game.api.common.LHResult;
import com.demiframe.game.api.common.LHStatusCode;
import com.demiframe.game.api.connector.IActivity;
import com.demiframe.game.api.connector.ICheckSupport;
import com.demiframe.game.api.connector.IInitCallback;
import com.demiframe.game.api.util.LHCheckSupport;
import com.kaopu.supersdk.api.KPSuperSDK;
import com.kaopu.supersdk.callback.KPAuthCallBack;

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
        KPSuperSDK.onActivityResult(requestCode, resultCode, intentData);
    }

    public void onCreate(Activity activity, final IInitCallback initCallback, Object obj) {
        KPSuperSDK.onCreate(activity);
        KPSuperSDK.auth(activity, null, new KPAuthCallBack() {
            @Override
            public void onAuthSuccess() {
                KPSuperSDK.registerLogoutCallBack(LHUserManagerProxy.logoutCallback);
                LHResult lhResult = new LHResult();
                lhResult.setCode(LHStatusCode.LH_INIT_SUCCESS);
                initCallback.onFinished(lhResult);
            }

            @Override
            public void onAuthFailed() {
                LHResult lhResult = new LHResult();
                lhResult.setCode(LHStatusCode.LH_INIT_FAIL);
                initCallback.onFinished(lhResult);
            }
        });
    }

    public void afterOnCreate(Activity activity, IInitCallback listener, Object obj){
        
    }

    public Boolean needAfterCreate(){
        return false;
    }

    public void onDestroy(Activity activity) {
        KPSuperSDK.onDestroy(activity);
    }

    public void onNewIntent(Activity activity, Intent intent) {
        KPSuperSDK.onNewIntent (intent);
    }

    public void onPause(Activity activity) {
        KPSuperSDK.onPause(activity);
    }

    public void onRestart(Activity activity) {
        KPSuperSDK.onRestart(activity);
    }

    public void onResume(Activity activity) {
        KPSuperSDK.onResume(activity);
    }

    public void onStop(Activity activity) {
        KPSuperSDK.onStop(activity);
    }

    public void onStart(Activity activity){
        KPSuperSDK.onStart (activity);
    }

    public void onConfigurationChanged(Activity activity, Configuration newConfig){
        KPSuperSDK.onConfigurationChanged(newConfig);
    }
}