package com.demiframe.game.api.huawei.proxy;

import android.app.Activity;
import android.content.Intent;
import android.content.res.Configuration;
import android.os.Handler;

import com.demiframe.game.api.common.LHResult;
import com.demiframe.game.api.common.LHStatusCode;
import com.demiframe.game.api.connector.IActivity;
import com.demiframe.game.api.connector.ICheckSupport;
import com.demiframe.game.api.connector.IInitCallback;
import com.demiframe.game.api.huawei.util.Sign;
import com.demiframe.game.api.util.IOUtil;
import com.demiframe.game.api.util.LHCheckSupport;
import com.demiframe.game.api.util.LogUtil;
import com.demiframe.game.api.util.SDKTools;
import com.huawei.gameservice.sdk.GameServiceSDK;
import com.huawei.gameservice.sdk.control.GameEventHandler;
import com.huawei.gameservice.sdk.model.Result;

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
                return true;
            }

            public boolean isSupportUserCenter() {
                return false;
            }
        });
    }

    public void onActivityResult(Activity activity, int requestCode, int resultCode, Intent intentData) {
    }

    public void onCreate(final Activity activity, final IInitCallback initCallback, Object obj) {
        //初始化浮标秘钥
        Sign.Init(activity);

        final String appID = SDKTools.GetSdkProperty(activity, "HUAWEI_APPID");
        final String cpID = SDKTools.GetSdkProperty(activity, "HUAWEI_CPID");
        GameServiceSDK.init(activity, appID, cpID, "", new GameEventHandler() {
            @Override
            public void onResult(Result result) {
                if (result.rtnCode != Result.RESULT_OK) {
                    LHResult lhResult = new LHResult();
                    //返回成功给unity
                    lhResult.setCode(LHStatusCode.LH_INIT_SUCCESS);
                    lhResult.setData("初始化，但是返回成功给unity，失败code："+result.rtnCode);
                    initCallback.onFinished(lhResult);
                }else{
                    LHResult lhResult = new LHResult();
                    lhResult.setCode(LHStatusCode.LH_INIT_SUCCESS);
                    initCallback.onFinished(lhResult);

                    checkUpdate(activity);
                }
            }

            @Override
            public String getGameSign(String appId, String cpId, String ts) {
                return Sign.createGameSign(appId + cpId + ts);
            }

        });

    }

    /**
     * 检测游戏更新 check the update for game
     */
    private void checkUpdate(Activity activity) {
        GameServiceSDK.checkUpdate(activity, new GameEventHandler() {

            @Override
            public void onResult(Result result) {
                if (result.rtnCode != Result.RESULT_OK) {
                    LogUtil.d("huawei check update failed:" + result.rtnCode);
                }
            }

            @Override
            public String getGameSign(String appId, String cpId, String ts) {
                LogUtil.d("init getGameSign "+appId+cpId);
                return Sign.createGameSign(appId + cpId + ts);
            }

        });
    }


    public void afterOnCreate(Activity activity, IInitCallback listener, Object obj){
        
    }

    public Boolean needAfterCreate(){
        return false;
    }

    public void onDestroy(Activity activity) {
        GameServiceSDK.destroy(activity);
    }

    public void onNewIntent(Activity activity, Intent intent) {
    }

    public void onPause(Activity activity) {
        GameServiceSDK.hideFloatWindow(activity);
    }

    public void onRestart(Activity activity) {
    }

    public void onResume(Activity activity) {
        GameServiceSDK.showFloatWindow(activity);
    }

    public void onStop(Activity activity) {
    }

    public void onStart(Activity activity){

    }

    public void onConfigurationChanged(Activity activity, Configuration newConfig){

    }
}