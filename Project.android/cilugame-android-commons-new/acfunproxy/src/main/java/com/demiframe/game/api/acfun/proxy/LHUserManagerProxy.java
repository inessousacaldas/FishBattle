package com.demiframe.game.api.acfun.proxy;

import android.app.Activity;

import com.demiframe.game.api.common.LHCallbackListener;
import com.demiframe.game.api.common.LHStaticValue;
import com.demiframe.game.api.common.LHStatusCode;
import com.demiframe.game.api.common.LHUser;
import com.demiframe.game.api.connector.IUserListener;
import com.demiframe.game.api.connector.IUserManager;
import com.demiframe.game.api.util.LogUtil;
import com.joygames.hostlib.JoyGamesSDK;
import com.joygames.hostlib.listener.LoginCallback;

public class LHUserManagerProxy
        implements IUserManager {
    Activity mActivity;
    IUserListener mUserListener;

    public void guestRegist(Activity activity, String paramString) {

    }

    public void login(Activity activity, Object paramObject) {
        JoyGamesSDK.getInstance().login(activity, new LoginCallback(){
            @Override
            public void onSuccess(String userId, String token, String username) {
                LHUser lhUser = new LHUser();
                String sid = userId + LHStaticValue.demiSplit + username + LHStaticValue.demiSplit + token;
                lhUser.setUid(userId);
                lhUser.setSid(sid);
                mUserListener.onLogin(LHStatusCode.LH_LOGIN_SUCCESS, lhUser);
            }
            @Override
            public void onError(String errorMsg) {
                LHUser lhUser = new LHUser();
                lhUser.setLoginMsg(errorMsg);
                mUserListener.onLogin(LHStatusCode.LH_LOGIN_FAIL, lhUser);
            }
            @Override
            public void onCancle() {
                LHUser lhUser = new LHUser();
                mUserListener.onLogin(LHStatusCode.LH_LOGIN_CANCEL, lhUser);
            }
            @Override
            public void onLogout() {
                mUserListener.onLogout(LHStatusCode.LH_LOGOUT_SUCCESS, "onLogoutSucc");
            }
            //切换账号和登录流程一样
            @Override
            public void onSwitchAccount(String userId, String token, String username) {
                LHUser lhUser = new LHUser();
                String sid = userId + LHStaticValue.demiSplit + username + LHStaticValue.demiSplit + token;
                lhUser.setUid(userId);
                lhUser.setSid(sid);
                mUserListener.onLogin(LHStatusCode.LH_LOGIN_SUCCESS, lhUser);
            }
        });
    }

    public void loginGuest(Activity activity, Object paramObject) {
    }

    public void logout(Activity activity, Object paramObject) {
        JoyGamesSDK.getInstance().logout();
    }

    public void setUserListener(Activity activity, IUserListener userListener) {
        mUserListener = userListener;
    }

    public void switchAccount(Activity activity, Object paramObject) {
    }

    public void updateUserInfo(Activity activity, LHUser lhUser) {

    }
}