package com.demiframe.game.api;

import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.content.res.Configuration;
import android.text.TextUtils;

import com.demiframe.game.api.common.LHChannelInfo;
import com.demiframe.game.api.common.LHPayInfo;
import com.demiframe.game.api.common.LHRole;
import com.demiframe.game.api.common.LHRoleDataType;
import com.demiframe.game.api.common.LHStaticValue;
import com.demiframe.game.api.common.LHUser;
import com.demiframe.game.api.connector.IActivity;
import com.demiframe.game.api.connector.IAdapter;
import com.demiframe.game.api.connector.IExitCallback;
import com.demiframe.game.api.connector.IHandleCallback;
import com.demiframe.game.api.connector.IInitCallback;
import com.demiframe.game.api.connector.IUserListener;
import com.demiframe.game.api.util.IOUtil;
import com.demiframe.game.api.util.LogUtil;

abstract class LHGameApi {
    static final String LOG_TAG = "LHGameApi";
    private static LHChannelInfo channelInfo;
    private IAdapter adapterProxy;
    private LHUser loginUser;

    private IAdapter createAdapter(Context context) {
        if (this.adapterProxy != null)
            return this.adapterProxy;
        try {
            String str = (String) context.getClassLoader().loadClass("com.demiframe.game.api.proxy.AdapterProxy").getDeclaredMethod("createProxy", new Class[0]).invoke(null, new Object[0]);
            this.adapterProxy = ((IAdapter) context.getClassLoader().loadClass(str).newInstance());
            IAdapter localIAdapter = this.adapterProxy;
            return localIAdapter;
        } catch (Exception localException) {
            localException.printStackTrace();
        }
        return null;
    }

    public IAdapter GetAdapter() {
        return this.adapterProxy;
    }

    //德米框架大渠道ID
    private static String GetDemiFrameChannel(Context context) {
        String str = IOUtil.getMetaDataByName(context, "DemiFrameChannelId");
        if (TextUtils.isEmpty(str)) {
            LogUtil.e("大渠道为空");
            str = "demi";
        }
        return str;
    }

    //德米框架子渠道id
    private static String GetDemiFrameSubChannel(Context context) {
        String str = IOUtil.getMetaDataByName(context, "DemiFrameSubChannelId");
        if (TextUtils.isEmpty(str)) {
            LogUtil.e("子渠道为空：");
            str = "0";
        }
        return str;
    }


  /*微信
  public static String fetchTSIWxAppId(Context paramContext)
  {
    String str = IOUtil.getMetaDataByName(paramContext, "LHWXAppId");
    if (!TextUtils.isEmpty(str))
      LogUtil.e("微信id：" + str);
    return str;
  }

  public static String fetchTSIWxAppSecret(Context paramContext)
  {
    String str = IOUtil.getMetaDataByName(paramContext, "LHWXAppSecret");
    if (!TextUtils.isEmpty(str))
      LogUtil.e("微信secret：" + str);
    return str;
  }
  */

    public void InitChannelInfo(Activity activity) {
        if (channelInfo == null) {
            String channelId = GetDemiFrameChannel(activity);
            channelInfo = new LHChannelInfo();
            channelInfo.setChannelId(channelId);
            channelInfo.setSubChannelId(getSubChannelId(activity));
            LogUtil.d("InitChannelInfo channelId, subchannelId:" + channelInfo.getChannelId() + " " + channelInfo.getSubChannelId());
            //channelInfo.setWxAppId(fetchTSIWxAppId(context));
            //channelInfo.setWxAppSecret(fetchTSIWxAppSecret(context));
        }
    }

    private String getSubChannelId(Activity activity){
        return createAdapter(activity).extendProxy().getSubChannelId(activity);
    }

    public LHChannelInfo getChannelInfo() {
        if (channelInfo == null) {
            channelInfo = new LHChannelInfo();
            channelInfo.setChannelId("");
            LogUtil.e("未初始化渠道");
        }
        return channelInfo;
    }


    private void resetLoginUser(LHUser lhUser) {
        if (this.loginUser != null)
            this.loginUser = null;
        this.loginUser = lhUser;
    }

    public static void setChannelInfo(LHChannelInfo lhChannelInfo) {
        channelInfo = lhChannelInfo;
    }


    public static void updateChannelInfo(Context context, String channelName, String appIdentifier) {
        if (channelInfo == null) {
            LogUtil.e("未初始化渠道");
            return;
        }

        channelInfo.setAppIdentifier(appIdentifier);
        channelInfo.setChannelName(channelName);
    }

    public void enterSdkBBS(final Activity activity) {
        activity.runOnUiThread(new Runnable() {
            public void run() {
                LHGameApi.this.createAdapter(activity).extendProxy().enterBBS(activity);
            }
        });
    }

    public void enterUserCenter(final Activity activity) {
        activity.runOnUiThread(new Runnable() {
            public void run() {
                LHGameApi.this.createAdapter(activity).extendProxy().enterUserCenter(activity);
            }
        });
    }

    public void hideFloatToolBar(final Activity activity) {
        activity.runOnUiThread(new Runnable() {
            public void run() {
                LHGameApi.this.createAdapter(activity).toolbarProxy().hideFloatToolBar(activity);
            }
        });
    }

    public void login(final Activity activity, final Object extObject) {
        activity.runOnUiThread(new Runnable() {
            public void run() {
                LHGameApi.this.createAdapter(activity).userManagerProxy().login(activity, extObject);
            }
        });
    }

    public void logout(final Activity activity, final Object extObject) {
        activity.runOnUiThread(new Runnable() {
            public void run() {
                LHGameApi.this.createAdapter(activity).userManagerProxy().logout(activity, extObject);
            }
        });
    }

    public void onActivityResult(final Activity activity, final int paramInt1, final int paramInt2, final Intent intent) {
        activity.runOnUiThread(new Runnable() {
            public void run() {
                LHGameApi.this.createAdapter(activity).activityProxy().onActivityResult(activity, paramInt1, paramInt2, intent);
            }
        });
    }

    public void onCreate(final Activity activity, final IInitCallback initCallback, final Object object) {
        InitChannelInfo(activity);

        activity.runOnUiThread(new Runnable() {
            public void run() {
                LHGameApi.this.createAdapter(activity).activityProxy().onCreate(activity, initCallback, object);
            }
        });
    }

    public void afterOnCreate(final Activity activity, final IInitCallback initCallback, final Object object) {
        activity.runOnUiThread(new Runnable() {
            public void run() {
                LHGameApi.this.createAdapter(activity).activityProxy().afterOnCreate(activity, initCallback, object);
            }
        });
    }

    public Boolean needAfterCreate(final Activity activity){
        return LHGameApi.this.createAdapter(activity).activityProxy().needAfterCreate();
    }

    public void onDestroy(final Activity activity) {
        activity.runOnUiThread(new Runnable() {
            public void run() {
                LHGameApi.this.createAdapter(activity).activityProxy().onDestroy(activity);
            }
        });
    }

    public void onExit(final Activity activity, final IExitCallback exitCallback) {
        activity.runOnUiThread(new Runnable() {
            public void run() {
                LHGameApi.this.createAdapter(activity).exitProxy().onExit(activity, exitCallback);
            }
        });
    }

    public void onNewIntent(final Activity activity, final Intent intent) {
        activity.runOnUiThread(new Runnable() {
            public void run() {
                LHGameApi.this.createAdapter(activity).activityProxy().onNewIntent(activity, intent);
            }
        });
    }

    public void onPause(final Activity activity) {
        activity.runOnUiThread(new Runnable() {
            public void run() {
                LHGameApi.this.createAdapter(activity).activityProxy().onPause(activity);
            }
        });
    }

    public void onConfigurationChanged(final Activity activity, final Configuration newConfig){
        activity.runOnUiThread(new Runnable() {
            public void run() {
                LHGameApi.this.createAdapter(activity).activityProxy().onConfigurationChanged(activity, newConfig);
            }
        });
    }

    public void onPay(final Activity activity, final LHPayInfo lhPayInfo) {
        activity.runOnUiThread(new Runnable() {
            public void run() {
                LHGameApi.this.createAdapter(activity).payProxy().onPay(activity, lhPayInfo);
            }
        });
    }

    public void onRestart(final Activity activity) {
        activity.runOnUiThread(new Runnable() {
            public void run() {
                LHGameApi.this.createAdapter(activity).activityProxy().onRestart(activity);
            }
        });
    }

    public void onResume(final Activity activity) {
        activity.runOnUiThread(new Runnable() {
            public void run() {
                LHGameApi.this.createAdapter(activity).activityProxy().onResume(activity);
            }
        });
    }

    public void onStop(final Activity activity) {
        activity.runOnUiThread(new Runnable() {
            public void run() {
                LHGameApi.this.createAdapter(activity).activityProxy().onStop(activity);
            }
        });
    }

    public void onStart(final Activity activity) {
        activity.runOnUiThread(new Runnable() {
            public void run() {
                LHGameApi.this.createAdapter(activity).activityProxy().onStart(activity);
            }
        });
    }


    public void paySuccessDone(Activity activity) {
        ;
    }

    @Deprecated
    public void realNameRegister(final Activity activity, final IHandleCallback handleCallback) {
        activity.runOnUiThread(new Runnable() {
            public void run() {
                LHGameApi.this.createAdapter(activity).extendProxy().realNameRegister(activity, handleCallback);
            }
        });
    }

    public void setUserListener(final Activity activity, final IUserListener userListener) {
        activity.runOnUiThread(new Runnable() {
            public void run() {
                LHGameApi.this.createAdapter(activity).userManagerProxy().setUserListener(activity, userListener);
            }
        });
    }

    public void showFloatToolBar(final Activity activity) {
        activity.runOnUiThread(new Runnable() {
            public void run() {
                LHGameApi.this.createAdapter(activity).toolbarProxy().showFloatToolBar(activity);
            }
        });
    }

    public void submitRoleData(final Activity activity, final LHRole lhRole) {
        activity.runOnUiThread(new Runnable() {
            public void run() {
                LHGameApi.this.createAdapter(activity).extendProxy().submitRoleData(activity, lhRole);
            }
        });
    }

    public void ConsumeGameCoin(final Activity activity, final String jsonStr){
        activity.runOnUiThread(new Runnable() {
            public void run() {
                LHGameApi.this.createAdapter(activity).extendProxy().ConsumeGameCoin(activity, jsonStr);
            }
        });
    }

    public void GainGameCoin(final Activity activity, final String jsonStr){
        activity.runOnUiThread(new Runnable() {
            public void run() {
                LHGameApi.this.createAdapter(activity).extendProxy().GainGameCoin(activity, jsonStr);
            }
        });
    }

    public void updateUserInfo(Activity activity, LHUser lhUser) {
        createAdapter(activity).userManagerProxy().updateUserInfo(activity, lhUser);
        if (lhUser != null) {
            resetLoginUser(lhUser);
        }
    }

    public void setAppName(String name){
        LHStaticValue.appName = name;
    }

    public String getAppName(){
        return LHStaticValue.appName;
    }

    public String GetMutilPackageId(Activity activity){
        return IOUtil.getMetaDataByName(activity, "DemiMutiPackageId");
    }

    public String GetGameId(Activity activity){
        return IOUtil.getMetaDataByName(activity, "DemiGameId");
    }
}