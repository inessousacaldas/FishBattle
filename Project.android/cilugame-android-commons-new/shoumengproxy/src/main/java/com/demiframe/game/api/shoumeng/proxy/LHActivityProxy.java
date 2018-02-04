package com.demiframe.game.api.shoumeng.proxy;

import android.app.Activity;
import android.content.Intent;
import android.content.res.Configuration;
import android.content.res.Resources;
import android.util.Log;

import com.demiframe.game.api.common.LHCallbackListener;
import com.demiframe.game.api.common.LHResult;
import com.demiframe.game.api.common.LHStaticValue;
import com.demiframe.game.api.common.LHStatusCode;
import com.demiframe.game.api.common.LHUser;
import com.demiframe.game.api.connector.IActivity;
import com.demiframe.game.api.connector.ICheckSupport;
import com.demiframe.game.api.connector.IInitCallback;
import com.demiframe.game.api.shoumeng.base.User;
import com.demiframe.game.api.util.LHCheckSupport;

import mobi.shoumeng.integrate.game.*;
import mobi.shoumeng.integrate.game.method.GameMethod;

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

    public void onActivityResult(Activity activity, int paramInt1, int paramInt2, Intent paramIntent) {
        GameMethod.getInstance().onActivityResult(activity, paramInt1, paramInt2, paramIntent);
    }

    public void onCreate(Activity activity, IInitCallback initCallback, Object paramObject) {
        LHCallbackListener.getInstance().setInitListener(initCallback);

        Resources resources = activity.getResources();
        CharSequence charSequence = resources.getText(resources.getIdentifier("app_name", "string", activity.getPackageName()));
        GameMethod.getInstance().setGameName(charSequence.toString());

        GameMethod.getInstance().setGameSDKInitListener(gameSDKInitListener);
        GameMethod.getInstance().setGameSDKLoginListener(gameSDKLoginListener);
        GameMethod.getInstance().setGameSDKLogoutListener(gameSDKLogoutListener);
        GameMethod.getInstance().setGameSDKPaymentListener(gameSDKPaymentListener);
        GameMethod.getInstance().setGameSDKExitListener(gameSDKExitListener);

        GameMethod.getInstance().onCreate(activity);
    }

    public void afterOnCreate(Activity activity, IInitCallback listener, Object obj){

    }

    public Boolean needAfterCreate(){
        return false;
    }

    public void onDestroy(Activity activity) {
        GameMethod.getInstance().onDestroy(activity);

    /*
    try {

      if (isGameExit) {// 是用户主动退出，退出回调onExit触发的退出游戏
        System.exit(0);

      }

    } catch (Exception e) {

      e.printStackTrace();

    }
    */
    }

    public void onNewIntent(Activity activity, Intent paramIntent) {
        GameMethod.getInstance().onNewIntent(activity, paramIntent);
    }

    public void onPause(Activity activity) {
        GameMethod.getInstance().onPause(activity);
    }

    public void onRestart(Activity activity) {
        GameMethod.getInstance().onRestart(activity);
    }

    public void onResume(Activity activity) {
        GameMethod.getInstance().onResume(activity);
    }

    public void onStop(Activity activity) {
        GameMethod.getInstance().onStop(activity);
    }

    public void onStart(Activity activity) {
        GameMethod.getInstance().onStart(activity);
    }

    public void onConfigurationChanged(Activity activity, Configuration newConfig) {
        GameMethod.getInstance().onConfigurationChanged(activity, newConfig);
    }

    //初始化监听
    private GameSDKInitListener gameSDKInitListener = new GameSDKInitListener() {
        @Override
        public void onInitSuccess() {
            showToast("初始化成功");
            LHResult lhResult = new LHResult();
            lhResult.setCode(LHStatusCode.LH_INIT_SUCCESS);
            LHCallbackListener.getInstance().getInitCallback().onFinished(lhResult);
        }

        @Override
        public void onInitFailed(int i, String s) {
            LHResult lhResult = new LHResult();
            lhResult.setCode(LHStatusCode.LH_INIT_FAIL);
            lhResult.setData(s);
            LHCallbackListener.getInstance().getInitCallback().onFinished(lhResult);

            showToast("初始化失败");
        }
    };

    //登录监听
    private GameSDKLoginListener gameSDKLoginListener = new GameSDKLoginListener() {
        @Override
        public void onLoginFailed(int i, String s) {
            LHUser lhUser = new LHUser();
            lhUser.setLoginMsg(s);
            LHCallbackListener.getInstance().getUserListener().onLogin(LHStatusCode.LH_LOGIN_FAIL, lhUser);
            showToast("登录失败");
        }

        @Override
        public void onLoginSuccess(UserInfo userInfo) {
            User.getInstances().setUserInfo(userInfo);

            LHUser lhUser = new LHUser();
            String sid = userInfo.getSessionId() + LHStaticValue.demiSplit + userInfo.getLoginAccount();
            lhUser.setSid(sid);

            //手盟的唯一id
            lhUser.setUid(userInfo.getLoginAccount());
            LHCallbackListener.getInstance().getUserListener().onLogin(LHStatusCode.LH_LOGIN_SUCCESS, lhUser);
            showToast("登录成功");
        }

        @Override
        public void onLoginCancel() {
            LHUser lhUser = new LHUser();
            LHCallbackListener.getInstance().getUserListener().onLogin(LHStatusCode.LH_LOGIN_CANCEL, lhUser);
            showToast("登录取消");
        }
    };

    //登出监听
    private GameSDKLogoutListener gameSDKLogoutListener = new GameSDKLogoutListener() {
        @Override
        public void onLogoutSuccess() {
            //收到登出回调后，这里实现游戏的登出代码，回到刚开始进游戏的那个游戏登录的背景，再弹出登录框
            LHCallbackListener.getInstance().getUserListener().onLogout(LHStatusCode.LH_LOGOUT_SUCCESS, "onLogoutSucc");
            User.getInstances().setUserInfo(null);
            showToast("登出成功");

        }

        @Override
        public void onLogoutFailed(int i, String s) {
            LHCallbackListener.getInstance().getUserListener().onLogout(LHStatusCode.LH_LOGOUT_FAIL, "onLogoutFailed");
            showToast("登出失败");
        }
    };

    //支付监听
    private GameSDKPaymentListener gameSDKPaymentListener = new GameSDKPaymentListener() {
        @Override
        public void onPaySuccess() {
            LHResult lhResult = new LHResult();
            lhResult.setCode(LHStatusCode.LH_PAY_SUCCESS);
            LHCallbackListener.getInstance().getPayListener().onFinished(lhResult);
            showToast("支付成功");
        }

        @Override
        public void onPayFailed(int i, String s) {
            LHResult lhResult = new LHResult();
            lhResult.setCode(LHStatusCode.LH_PAY_FAIL);
            lhResult.setData(s);
            LHCallbackListener.getInstance().getPayListener().onFinished(lhResult);
            showToast("支付失败");
        }

        @Override
        public void onPayCancel() {
            LHResult lhResult = new LHResult();
            lhResult.setCode(LHStatusCode.LH_PAY_CANCEL);
            LHCallbackListener.getInstance().getPayListener().onFinished(lhResult);
            showToast("支付取消");
        }
    };

    //退出监听
    private GameSDKExitListener gameSDKExitListener = new GameSDKExitListener() {
        @Override
        public void onExit(int i, String s) {
            showToast("确定退出");
            LHCallbackListener.getInstance().getExitListener().onExit(true);
            //loginAccount = null;//这里是demo,仅供参考，退出后，账号置空

            /**
             * 这里是demo,不能照搬，这里demo调用finish();能确保调用到onDestroy(),也能调用到SDK 的方法GameMethod.getInstance().onDestroy(this);
             * 实际接入时，在收到退出回调onExit(),必须保证能调用到SDK 的方法GameMethod.getInstance().onDestroy(this);
             * 否则，自检提示，SDK onDestroy()未调用。
             * 杀进程等处理，建议放到onDestroy()里
             */
            //finish();
        }

        @Override
        public void onCancel(int i, String s) {
            showToast("退出取消");
            LHCallbackListener.getInstance().getExitListener().onExit(false);
        }
    };

    private void showToast(String text) {
        //demo里仅作调试提示，实际接入无需Toast提示
//        Toast.makeText(MyActivity.this, text, Toast.LENGTH_SHORT).show();
        Log.v("shoumeng_debug", "demo " + text);
    }
}