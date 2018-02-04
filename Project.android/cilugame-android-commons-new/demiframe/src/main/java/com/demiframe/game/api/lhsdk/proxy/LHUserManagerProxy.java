package com.demiframe.game.api.lhsdk.proxy;

import android.app.Activity;

import com.demiframe.game.api.common.LHStatusCode;
import com.demiframe.game.api.common.LHUser;
import com.demiframe.game.api.connector.IUserListener;
import com.demiframe.game.api.connector.IUserManager;
import com.demiframe.game.api.util.LogUtil;

public class LHUserManagerProxy
        implements IUserManager {
    Activity mActivity;
    IUserListener mUserListener;

    public void guestRegist(Activity activity, String name) {

    }

    public void login(Activity activity, Object obj) {
        LogUtil.e("LHUserManagerProxy", "inner login");
    }

    public void loginGuest(Activity activity, Object obj) {
    }

    //如果渠道不支持登出，ICheckSupport.IsSupportLogout需要return false
    //并且这里直接执行logout回调，返回成功
    public void logout(Activity activity, Object obj) {
        mUserListener.onLogout(LHStatusCode.LH_LOGOUT_FAIL, "No Support Logout");
    }

    public void setUserListener(Activity activity, IUserListener userListener) {
        //保持回调
        //也可保存在LHCallbackListener.getInstance().setUserListener，方便跨文件访问
        mUserListener = userListener;
    }

    public void switchAccount(Activity activity, Object obj) {
    }

    public void updateUserInfo(Activity activity, LHUser lhUser) {

    }
}