package com.demiframe.game.api.uc.proxy;

import android.app.Activity;
import android.content.Intent;
import android.content.res.Configuration;
import android.os.Handler;
import android.text.TextUtils;

import cn.uc.gamesdk.UCGameSdk;

import com.demiframe.game.api.common.LHCallbackListener;
import com.demiframe.game.api.connector.IActivity;
import com.demiframe.game.api.connector.ICheckSupport;
import com.demiframe.game.api.connector.IInitCallback;
import com.demiframe.game.api.util.IOUtil;
import com.demiframe.game.api.util.LHCheckSupport;

import com.demiframe.game.api.uc.ucutils.LHUCGameSdk;
import com.demiframe.game.api.uc.ucutils.SdkEventReceiverImpl;
import com.demiframe.game.api.util.LogUtil;
import com.demiframe.game.api.util.SDKTools;

import org.json.JSONObject;

public class LHActivityProxy
        implements IActivity {
    private SdkEventReceiverImpl eventReceiver;
    private static final boolean debugMode = false;

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

    private void ucSdkDestoryFloatButton(Activity activity) {

    }

    public void onActivityResult(Activity activity, int paramInt1, int paramInt2, Intent paramIntent) {
    }

    public void onCreate(final Activity activity, final IInitCallback initCallback, Object object) {
        LHCallbackListener.getInstance().setInitListener(initCallback);

        if (eventReceiver == null) {
            eventReceiver = new SdkEventReceiverImpl();
            UCGameSdk.defaultSdk().registerSDKEventReceiver(eventReceiver);
        }

        String str = SDKTools.GetSdkProperty(activity, "UCGameId");
        if(TextUtils.isEmpty(str)){
            LogUtil.d("UCGameId 错误");
            return;
        }
        final int gameId;
        try{
            gameId = Integer.parseInt(str);
        }catch (Exception e){
            e.printStackTrace();
            return;
        }

        Intent intent = activity.getIntent();
        String pullData = null;
        if(intent != null){
            pullData = intent.getDataString();
        }

        final String pullInfo = pullData;

//        new Handler().postDelayed(new Runnable() {
//            @Override
//            public void run() {
//                LHUCGameSdk.initSDK(activity, debugMode, gameId, true, true, 1, pullInfo);
//            }
//        }, 2000);
        LHUCGameSdk.initSDK(activity, debugMode, gameId, true, true, 1, pullInfo);
    }

    public void afterOnCreate(Activity activity, IInitCallback listener, Object obj) {
    }

    public Boolean needAfterCreate(){
        return true;
    }

    public void onDestroy(Activity activity) {
        UCGameSdk.defaultSdk().unregisterSDKEventReceiver(eventReceiver);
        eventReceiver = null;
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

    public void onStart(Activity activity) {
    }

    public void onConfigurationChanged(Activity activity, Configuration newConfig){

    }
}