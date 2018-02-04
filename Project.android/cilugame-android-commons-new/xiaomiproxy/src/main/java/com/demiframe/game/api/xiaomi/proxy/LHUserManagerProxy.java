package com.demiframe.game.api.xiaomi.proxy;

import android.app.Activity;

import com.demiframe.game.api.common.LHCallbackListener;
import com.demiframe.game.api.common.LHResult;
import com.demiframe.game.api.common.LHStaticValue;
import com.demiframe.game.api.common.LHStatusCode;
import com.demiframe.game.api.common.LHUser;
import com.demiframe.game.api.connector.IUserListener;
import com.demiframe.game.api.connector.IUserManager;
import com.demiframe.game.api.util.LogUtil;
import com.demiframe.game.api.xiaomi.base.User;
import com.xiaomi.gamecenter.sdk.MiCommplatform;
import com.xiaomi.gamecenter.sdk.MiErrorCode;
import com.xiaomi.gamecenter.sdk.OnLoginProcessListener;
import com.xiaomi.gamecenter.sdk.entry.MiAccountInfo;

public class LHUserManagerProxy implements IUserManager
{
    Activity mActivity;
    IUserListener mUserListener;

    public void guestRegist(Activity activity, String paramString) {

    }

    public void login(Activity activity, Object paramObject)
    {
        LogUtil.e("LHUserManagerProxy", "XIAOMI Call Login Start");
        MiCommplatform.getInstance().miLogin( activity, new OnLoginProcessListener()
        {
            @Override
            public void finishLoginProcess(int code, MiAccountInfo arg1)
            {
                LHUser lhUser = new LHUser();
                switch (code)
                {
                    case MiErrorCode.MI_XIAOMI_GAMECENTER_SUCCESS:
                        //登录成功
                        //清空前一次的记录
                        User.getInstances().Clear();

                        lhUser.setUid(Long.toString(arg1.getUid()));//获取用户登录后的UID（即用户唯一标识）
                        lhUser.setSid(arg1.getUid() + LHStaticValue.demiSplit + arg1.getSessionId());//获取用户登录的Session，若没有登录返回null
                        lhUser.setNickName(arg1.getNikename());//获取用户角色昵称
                        lhUser.setLoginMsg("登录成功");
                        LogUtil.e("LHUserManagerProxy", "XIAOMI Login Callback, Login Success");
                        LHCallbackListener.getInstance().getUserListener().onLogin(LHStatusCode.LH_LOGIN_SUCCESS, lhUser);
                        break;

                    case MiErrorCode.MI_XIAOMI_GAMECENTER_ERROR_LOGIN_FAIL:
                        //登录失败
                        lhUser.setLoginMsg("登录失败");
                        LogUtil.e("LHUserManagerProxy", "XIAOMI Login Callback, Login Fail");
                        LHCallbackListener.getInstance().getUserListener().onLogin(LHStatusCode.LH_LOGIN_FAIL, lhUser);
                        break;

                    case MiErrorCode.MI_XIAOMI_GAMECENTER_ERROR_CANCEL:
                        //取消操作
                        lhUser.setLoginMsg("取消登录操作");
                        LogUtil.e("LHUserManagerProxy", "XIAOMI Login Callback, Login Cancel");
                        LHCallbackListener.getInstance().getUserListener().onLogin(LHStatusCode.LH_LOGIN_FAIL, lhUser);
                        break;

                    case MiErrorCode.MI_XIAOMI_GAMECENTER_ERROR_ACTION_EXECUTED:
                        //登录操作正在进行中
                        lhUser.setLoginMsg("登录操作正在进行中");
                        LogUtil.e("LHUserManagerProxy", "XIAOMI Login Callback, At Login");
                        LHCallbackListener.getInstance().getUserListener().onLogin(LHStatusCode.LH_LOGIN_FAIL, lhUser);
                        break;

                    default:
                        break;
                }
            }
        });
    }

    public void loginGuest(Activity activity, Object paramObject) {
    }

    public void logout(Activity activity, Object paramObject){
        LHCallbackListener.getInstance().getUserListener().onLogout(LHStatusCode.LH_LOGOUT_FAIL, paramObject);
    }

    public void setUserListener(Activity activity, IUserListener userListener) {
        LHCallbackListener.getInstance().setUserListener(userListener);
    }

    public void switchAccount(Activity activity, Object paramObject) {
    }

    public void updateUserInfo(Activity activity, LHUser lhUser) {

    }
}