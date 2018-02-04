package com.demiframe.game.api.haima.proxy;

import android.app.Activity;

import com.demiframe.game.api.common.LHCallbackListener;
import com.demiframe.game.api.common.LHStaticValue;
import com.demiframe.game.api.common.LHStatusCode;
import com.demiframe.game.api.common.LHUser;
import com.demiframe.game.api.connector.IUserListener;
import com.demiframe.game.api.connector.IUserManager;
import com.demiframe.game.api.util.LogUtil;
import com.haimawan.paysdk.cpapi.ErrorInfoBean;
import com.haimawan.paysdk.cpapi.OnLoginListener;
import com.haimawan.paysdk.cpapi.OnLogoutListener;
import com.haimawan.paysdk.enter.CPUserInfo;
import com.haimawan.paysdk.enter.HMPay;

public class LHUserManagerProxy
        implements IUserManager {
    Activity mActivity;
    IUserListener mUserListener;

    public void guestRegist(Activity activity, String name) {

    }

    public void login(Activity activity, Object obj) {
        HMPay.login(activity, new OnLoginListener() {
            @Override
            public void onLoginSuccessed(CPUserInfo cpUserInfo) {
                LHUser lhUser = new LHUser();
                String sid = cpUserInfo.getUid() + LHStaticValue.demiSplit + cpUserInfo.getvToken();
                lhUser.setUid(cpUserInfo.getUid());
                lhUser.setSid(sid);
                mUserListener.onLogin(LHStatusCode.LH_LOGIN_SUCCESS, lhUser);
            }

            @Override
            public void onLoginFailed(ErrorInfoBean errorInfoBean) {
                LogUtil.d("code="+errorInfoBean.getStateCode() +" msg="+errorInfoBean.getErrorMessage());
                LHUser lhUser = new LHUser();
                mUserListener.onLogin(LHStatusCode.LH_LOGIN_FAIL, lhUser);
            }

            @Override
            public void onLoginCancel() {
                LHUser lhUser = new LHUser();
                mUserListener.onLogin(LHStatusCode.LH_LOGIN_CANCEL, lhUser);
            }

            @Override
            public void onLoginedNotify() {
                LogUtil.d("onLoginedNotify");
            }
        });
    }

    public void loginGuest(Activity activity, Object obj) {
    }

    //如果渠道不支持登出，ICheckSupport.IsSupportLogout需要return false
    //并且这里直接执行logout回调，返回成功
    public void logout(Activity activity, Object obj) {
        HMPay.logout(activity, onLogoutListener);
    }

    public void setUserListener(Activity activity, IUserListener userListener) {
        //保持回调
        //也可保存在LHCallbackListener.getInstance().setUserListener，方便跨文件访问
        mUserListener = userListener;
        HMPay.registerLogoutListener(onLogoutListener);
    }

    public void switchAccount(Activity activity, Object obj) {
    }

    public void updateUserInfo(Activity activity, LHUser lhUser) {

    }

    public OnLogoutListener onLogoutListener = new OnLogoutListener() {
        @Override
        public void onLogoutSuccessed() {
            if(mUserListener == null)
                return;

            mUserListener.onLogout(LHStatusCode.LH_LOGOUT_SUCCESS, "LH_LOGOUT_FAIL");
        }

        @Override
        public void onLogoutFailed(ErrorInfoBean var1) {
            if(mUserListener == null)
                return;

            mUserListener.onLogout(LHStatusCode.LH_LOGOUT_FAIL, var1.getErrorMessage());
        }
    };
}