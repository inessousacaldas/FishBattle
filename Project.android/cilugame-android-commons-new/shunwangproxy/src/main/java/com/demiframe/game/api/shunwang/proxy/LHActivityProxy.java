package com.demiframe.game.api.shunwang.proxy;

import android.app.Activity;
import android.content.Intent;
import android.content.res.Configuration;
import android.os.Handler;

import com.demiframe.game.api.common.LHResult;
import com.demiframe.game.api.common.LHStatusCode;
import com.demiframe.game.api.connector.IActivity;
import com.demiframe.game.api.connector.ICheckSupport;
import com.demiframe.game.api.connector.IInitCallback;
import com.demiframe.game.api.shunwang.base.User;
import com.demiframe.game.api.util.IOUtil;
import com.demiframe.game.api.util.LHCheckSupport;
import com.demiframe.game.api.util.LogUtil;
import com.shunwang.sdk.game.SWGameSDK;
import com.shunwang.sdk.game.SWLogLevel;
import com.shunwang.sdk.game.SWOrientation;
import com.shunwang.sdk.game.listener.ILoaderListener;


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

    public void onCreate(final Activity activity, final IInitCallback initCallback, Object obj)
    {
//        new Handler().postDelayed(new Runnable() {
//            @Override
//            public void run() {
//                SWGameSDK swGameSdk = SWGameSDK.getInstance();
//                swGameSdk.setAutologon(true);
//                swGameSdk.init(activity, SWOrientation.LANDSCAPE, new ILoaderListener() {
//                    @Override
//                    public void onStart() {
//                        LogUtil.e("Shunwang SDK Init Start");
//                    }
//
//                    @Override
//                    public void onSuccess() {
//                        LogUtil.e("Shunwang SDK Init Success");
//                        LHResult localLHResult = new LHResult();
//                        localLHResult.setCode(LHStatusCode.LH_INIT_SUCCESS);
//                        initCallback.onFinished(localLHResult);
//                        //localLHResult.setData("顺网SDK初始化成功");
//                    }
//
//                    @Override
//                    public void onFailed() {
//                        LogUtil.e("Shunwang SDK Init Fail");
//                        LHResult localLHResult = new LHResult();
//                        localLHResult.setCode(LHStatusCode.LH_INIT_FAIL);
//                        initCallback.onFinished(localLHResult);
//                        //localLHResult.setData("顺网SDK初始化失败");
//                    }
//                });
//            }
//        }, 2000);

        if (initCallback != null)
        {
            SWGameSDK swGameSdk = SWGameSDK.getInstance();
            swGameSdk.setAutologon(true);
            swGameSdk.init(activity, SWOrientation.LANDSCAPE, new ILoaderListener() {
                @Override
                public void onStart() {
                    LogUtil.e("Shunwang SDK Init Start");
                }

                @Override
                public void onSuccess() {
                    LogUtil.e("Shunwang SDK Init Success");
                    LHResult localLHResult = new LHResult();
                    localLHResult.setCode(LHStatusCode.LH_INIT_SUCCESS);
                    initCallback.onFinished(localLHResult);
                    //localLHResult.setData("顺网SDK初始化成功");
                }

                @Override
                public void onFailed() {
                    LogUtil.e("Shunwang SDK Init Fail");
                    LHResult localLHResult = new LHResult();
                    localLHResult.setCode(LHStatusCode.LH_INIT_FAIL);
                    initCallback.onFinished(localLHResult);
                    //localLHResult.setData("顺网SDK初始化失败");
                }
            });
        }
    }

    public void afterOnCreate(Activity activity, IInitCallback listener, Object obj){
        
    }

    public Boolean needAfterCreate(){
        return true;
    }

    public void onDestroy(Activity activity)
    {
        User.getInstances().Clear();
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
        SWGameSDK.getInstance().hideFloatingView(activity);
    }

    public void onStart(Activity activity){
        SWGameSDK.getInstance().showFloatingView(activity);
    }

    public void onConfigurationChanged(Activity activity, Configuration newConfig){

    }
}