package com.demiframe.game.api.shunwang.proxy;

import android.app.Activity;

import com.demiframe.game.api.common.LHCallbackListener;
import com.demiframe.game.api.common.LHResult;
import com.demiframe.game.api.common.LHStaticValue;
import com.demiframe.game.api.common.LHStatusCode;
import com.demiframe.game.api.common.LHUser;
import com.demiframe.game.api.connector.IUserListener;
import com.demiframe.game.api.connector.IUserManager;
import com.demiframe.game.api.shunwang.base.User;
import com.demiframe.game.api.util.IOUtil;
import com.demiframe.game.api.util.LogUtil;
import com.demiframe.game.api.util.SDKTools;
import com.shunwang.sdk.game.SWGameSDK;
import com.shunwang.sdk.game.listener.OnLoginResponseListener;

public class LHUserManagerProxy implements IUserManager {
    Activity mActivity;
    IUserListener mUserListener;

    public void guestRegist(Activity activity, String paramString) {

    }

    public void login(Activity activity, Object paramObject)
    {
        LogUtil.d("LHUserManagerProxy", "shunwanglogin");

        SWGameSDK.getInstance().login(activity,
                SDKTools.GetSdkProperty(activity, "SHUNWANG_SiteID"),
                SDKTools.GetSdkProperty(activity, "SHUNWANG_GameID"),
                SDKTools.GetSdkProperty(activity, "SHUNWANG_MD5Key"),
                SDKTools.GetSdkProperty(activity, "SHUNWANG_RSAKey"),
                new OnLoginResponseListener()
        {
            @Override
            //guid : 游戏账号，唯一标识
            //accessToken : 访问令牌
            //memberId : 顺网通行证 Id，可能为空
            public void onLoginSucceed(String guid, String accessToken, String memberId) {
                LogUtil.e("LHUserManagerProxy", "Shunwang SDK Login Success");
                //保存缓存数据
                User.getInstances().setGuidId(guid);
                User.getInstances().setAccessToken(accessToken);

                String sid = guid + LHStaticValue.demiSplit + accessToken;

                LHUser lhUser = new LHUser();
                lhUser.setUid(guid);
                lhUser.setSid(sid);
                lhUser.setLoginMsg("登录操作成功");
                LHCallbackListener.getInstance().getUserListener().onLogin(LHStatusCode.LH_LOGIN_SUCCESS, lhUser);

                //设置免登陆
                SWGameSDK.getInstance().setAutologon(true);
            }

            @Override
            public void onLoginFailed() {
                LogUtil.e("LHUserManagerProxy", "Shunwang SDK Login Fail");

                LHUser lhUser = new LHUser();
                lhUser.setLoginMsg("登录操作失败");
                LHCallbackListener.getInstance().getUserListener().onLogin(LHStatusCode.LH_LOGIN_FAIL, lhUser);
            }

            @Override
            public void onLogOutSucceed() {
                LogUtil.e("LHUserManagerProxy", "Shunwang SDK Logout Success");

                User.getInstances().Clear();

                LHUser lhUser = new LHUser();
                lhUser.setLoginMsg("登出操作成功");
                LHCallbackListener.getInstance().getUserListener().onLogout(LHStatusCode.LH_LOGOUT_SUCCESS, lhUser);
            }

            @Override
            public void onLoginWindowClose() {
                LHUser lhUser = new LHUser();
                lhUser.setLoginMsg("登录操作失败");
                LHCallbackListener.getInstance().getUserListener().onLogin(LHStatusCode.LH_LOGIN_FAIL, lhUser);
            }

            @Override
            public void onSdkNoInit() {
                LogUtil.e("LHUserManagerProxy", "Shunwang Sdk No Init");

                LHUser lhUser = new LHUser();
                lhUser.setLoginMsg("登录操作失败");
                LHCallbackListener.getInstance().getUserListener().onLogin(LHStatusCode.LH_LOGIN_FAIL, lhUser);
            }
        });


    }

    public void loginGuest(Activity activity, Object paramObject) {
    }

    //如果渠道不支持登出，ICheckSupport.IsSupportLogout需要return false
    //并且这里直接执行logout回调，返回成功
    public void logout(Activity activity, Object paramObject) {
        SWGameSDK.getInstance().logout(activity);
    }

    public void setUserListener(Activity activity, IUserListener userListener)
    {
        LHCallbackListener.getInstance().setUserListener(userListener);
    }

    public void switchAccount(Activity activity, Object paramObject) {
    }

    public void updateUserInfo(Activity activity, LHUser lhUser) {

    }
}