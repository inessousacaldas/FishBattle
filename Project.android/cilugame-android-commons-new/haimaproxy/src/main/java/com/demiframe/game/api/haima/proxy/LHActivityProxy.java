package com.demiframe.game.api.haima.proxy;

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
import com.demiframe.game.api.util.LHCheckSupport;
import com.demiframe.game.api.util.SDKTools;
import com.haimawan.paysdk.cpapi.ErrorInfoBean;
import com.haimawan.paysdk.cpapi.OnCheckUpdateListener;
import com.haimawan.paysdk.enter.HMPay;

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
                return true;
            }
        });
    }

    public void onActivityResult(Activity activity, int requestCode, int resultCode, Intent intentData) {
    }

//    * @param screenOrientation   屏幕方向，必须传入以下参数中其中的一个，否则报错：<br/>
//            *                            参数为1时，强制竖屏<br/>
//            *                            参数为0时，强制横屏
//    * @param isTestMode          是否是测试模式。true,是测试模式；false，非测试模式
//    * @param hintType            如果检查更新失败,需要的提示,必须传入以下参数中其中的一个，否则报错:<br/>
//            *                            参数为2时，同时有重试和取消按钮;<br/>
//            *                            参数为1时，仅仅有重试按钮;<br/>
//            *                            参数为0时，不显示对话框.
    public void onCreate(Activity activity, final IInitCallback initCallback, Object obj) {
        int orientation = ActivityInfo.SCREEN_ORIENTATION_LANDSCAPE;
        if(!LHStaticValue.IsLandScape){
            orientation = ActivityInfo.SCREEN_ORIENTATION_PORTRAIT;
        }
        final boolean isLogEnable = SDKTools.GetSdkBoolProperty(activity, "HAIMA_LOGENABLE",false);
        boolean isTestMode = SDKTools.GetSdkBoolProperty(activity, "HAIMA_DEBUG", false);

        HMPay.init(activity, orientation, new OnCheckUpdateListener(){
            @Override
            public void onCheckUpdateSuccess(boolean isNeedUpdate, boolean isForceUpdate){
                LHResult lhResult = new LHResult();
                lhResult.setCode(LHStatusCode.LH_INIT_SUCCESS);
                lhResult.setData("onCheckUpdateSuccess");
                initCallback.onFinished(lhResult);

                HMPay.setLogEnable(isLogEnable);
            }
            @Override
            public void onCheckUpdateFail(ErrorInfoBean var1){
                LHResult lhResult = new LHResult();
                //lhResult.setCode(LHStatusCode.LH_INIT_FAIL);
                lhResult.setCode(LHStatusCode.LH_INIT_SUCCESS);
                lhResult.setData("onCheckUpdateFail");
                initCallback.onFinished(lhResult);
            }
        }, isTestMode, HMPay.POSITIVE_AND_NEGATIVE_DIALOG_TYPE);
    }

    public void afterOnCreate(Activity activity, IInitCallback listener, Object obj){
        
    }

    public Boolean needAfterCreate(){
        return false;
    }

    public void onDestroy(Activity activity) {
    }

    public void onNewIntent(Activity activity, Intent intent) {
    }

    public void onPause(Activity activity) {
        HMPay.onPause(activity);
    }

    public void onRestart(Activity activity) {
    }

    public void onResume(Activity activity) {
        HMPay.onResume(activity);
    }

    public void onStop(Activity activity) {
    }

    public void onStart(Activity activity){

    }

    public void onConfigurationChanged(Activity activity, Configuration newConfig){
        HMPay.onConfigurationChanged(activity);
    }
}