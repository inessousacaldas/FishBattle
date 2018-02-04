package com.demiframe.game.api.acfun.proxy;

import android.app.Activity;
import android.content.Intent;
import android.content.pm.ActivityInfo;
import android.content.res.Configuration;

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
import com.joygames.hostlib.JoyGamesSDK;
import com.joygames.hostlib.listener.InitCallback;


import java.util.HashMap;

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
                return true;
            }

            public boolean isSupportUserCenter() {
                return false;
            }
        });
    }

    public void onActivityResult(Activity activity, int requestCode, int resultCode, Intent intentData) {
    }

    public void onCreate(Activity activity, final IInitCallback initCallback, Object obj) {
        HashMap<String, String> data = new HashMap<String, String>();
        data.put("app_id", SDKTools.GetSdkProperty(activity, "ACFUN_APPID"));
        data.put("app_key", SDKTools.GetSdkProperty(activity, "ACFUN_APPKEY"));
        data.put("channel", SDKTools.GetSdkProperty(activity, "ACFUN_CHANNEL"));
        data.put("game_name", LHStaticValue.appName);
        data.put("subChannel", SDKTools.GetSdkProperty(activity, "ACFUN_SUBCHANNEL")); //子渠道编号,写死acfun
        String debug = SDKTools.GetSdkProperty(activity, "ACFUN_DEBUG");
        data.put("is_debug", debug);//设置是否为debug模式，若为debug模式将连接测试服务器；否则连接正式服务器。
        data.put("orientation", ActivityInfo.SCREEN_ORIENTATION_LANDSCAPE + "");  //设置activity显示的屏幕方向。此处只能取 SCREEN_ORIENTATION_LANDSCAPE 或  SCREEN_ORIENTATION_PORTRAIT
        data.put("backPressedInValid", "false"); //设置在登录界面是否屏蔽返回按键。
        JoyGamesSDK.init(activity, data, new InitCallback() {
            @Override
            public void onSuccess() {
                LHResult lhResult = new LHResult();
                lhResult.setCode(LHStatusCode.LH_INIT_SUCCESS);
                initCallback.onFinished(lhResult);
            }

            @Override
            public void onError(final String errorMsg) {
                LHResult lhResult = new LHResult();
                lhResult.setCode(LHStatusCode.LH_INIT_FAIL);
                lhResult.setData(errorMsg);
                initCallback.onFinished(lhResult);
                LogUtil.d("初始化失败");
            }
        });
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
        if (JoyGamesSDK.getInstance() != null){
            JoyGamesSDK.getInstance().hideFloat();
        }
    }

    public void onRestart(Activity activity) {
    }

    public void onResume(Activity activity) {
        if (JoyGamesSDK.getInstance() != null){
            JoyGamesSDK.getInstance().showFloat();
        }
    }

    public void onStop(Activity activity) {
    }

    public void onStart(Activity activity){

    }

    public void onConfigurationChanged(Activity activity, Configuration newConfig){

    }
}