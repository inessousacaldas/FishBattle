package com.demiframe.game.api.a4399.proxy;

import android.app.Activity;

import com.demiframe.game.api.common.LHCallbackListener;
import com.demiframe.game.api.common.LHStaticValue;
import com.demiframe.game.api.common.LHStatusCode;
import com.demiframe.game.api.common.LHUser;
import com.demiframe.game.api.connector.IUserListener;
import com.demiframe.game.api.connector.IUserManager;
import com.demiframe.game.api.util.LogUtil;

import cn.m4399.operate.OperateCenter;
import cn.m4399.operate.User;

public class LHUserManagerProxy
        implements IUserManager {
    Activity mActivity;
    IUserListener mUserListener;

    public void guestRegist(Activity activity, String name) {

    }

    public void login(Activity activity, Object obj) {
        OperateCenter.getInstance().login(activity, new OperateCenter.OnLoginFinishedListener() {
            @Override
            public void onLoginFinished(boolean success, int resultCode, User userInfo) {
                LogUtil.d("onUserAccountLogout resultCode:" + resultCode);
                if (LHCallbackListener.getInstance().getUserListener() == null) {
                    return;
                }
                if(success){
                    LHUser lhUser = new LHUser();
                    String sid = userInfo.getUid() + LHStaticValue.demiSplit + userInfo.getState();
                    lhUser.setUid(userInfo.getUid());
                    lhUser.setSid(sid);
                    LHCallbackListener.getInstance().getUserListener().onLogin(LHStatusCode.LH_LOGIN_SUCCESS, lhUser);
                }else{
                    LogUtil.d(OperateCenter.getResultMsg(resultCode));
                    LHUser lhUser = new LHUser();
                    LHCallbackListener.getInstance().getUserListener().onLogin(LHStatusCode.LH_LOGIN_FAIL, lhUser);
                }
            }
        });
    }

    public void loginGuest(Activity activity, Object obj) {
    }


    public void logout(Activity activity, Object obj) {
        OperateCenter.getInstance().logout();
    }

    public void setUserListener(Activity activity, IUserListener userListener) {
        LHCallbackListener.getInstance().setUserListener(userListener);
    }

    //此接口框架未调用，先做支持
    public void switchAccount(Activity activity, Object obj) {
        OperateCenter.getInstance().switchAccount(activity, new OperateCenter.OnLoginFinishedListener() {
            @Override
            public void onLoginFinished(boolean success, int resultCode, User userInfo) {
                if (success) {
                    LHUser lhUser = new LHUser();
                    String sid = userInfo.getUid() + LHStaticValue.demiSplit + userInfo.getState();
                    lhUser.setUid(userInfo.getUid());
                    lhUser.setSid(sid);
                    LHCallbackListener.getInstance().getUserListener().onLogin(LHStatusCode.LH_LOGIN_SUCCESS, lhUser);
                } else {
                    LHUser lhUser = new LHUser();
                    lhUser.setLoginMsg("resultCode" + resultCode);
                    LHCallbackListener.getInstance().getUserListener().onLogin(LHStatusCode.LH_LOGIN_FAIL, lhUser);
                }
            }
        });
    }

    public void updateUserInfo(Activity activity, LHUser lhUser) {

    }
}