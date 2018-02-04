package com.demiframe.game.api.vivo.proxy;

import android.app.Activity;
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
import com.vivo.unionsdk.open.VivoAccountCallback;
import com.vivo.unionsdk.open.VivoUnionSDK;


public class LHActivityProxy implements IActivity
{
    public LHActivityProxy()
    {
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

    public void onCreate(Activity activity, IInitCallback initCallback, Object obj)
    {
        VivoUnionSDK.registerAccountCallback(activity, LHUserManagerProxy.accountListener);

        LHResult lhResult = new LHResult();
        lhResult.setCode(LHStatusCode.LH_INIT_SUCCESS);
        initCallback.onFinished(lhResult);
    }

    public void afterOnCreate(Activity activity, IInitCallback listener, Object obj)
    {
    }

    public Boolean needAfterCreate(){
        return false;
    }

    public void onDestroy(Activity activity)
    {
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