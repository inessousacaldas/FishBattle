package com.demiframe.game.api.nubiya.proxy;

import android.app.Activity;
import android.os.Bundle;

import com.demiframe.game.api.common.LHCallbackListener;
import com.demiframe.game.api.common.LHStaticValue;
import com.demiframe.game.api.common.LHStatusCode;
import com.demiframe.game.api.common.LHUser;
import com.demiframe.game.api.connector.IUserListener;
import com.demiframe.game.api.connector.IUserManager;
import com.demiframe.game.api.nubiya.base.User;
import com.demiframe.game.api.util.LogUtil;

import cn.nubia.nbgame.sdk.GameSdk;
import cn.nubia.nbgame.sdk.entities.ErrorCode;
import cn.nubia.nbgame.sdk.interfaces.CallbackListener;

public class LHUserManagerProxy
        implements IUserManager {
    public void guestRegist(Activity activity, String paramString) {

    }

    public void login(Activity activity, Object paramObject)
    {
        LogUtil.e("LHUserManagerProxy", "nubiya inner login start");

        //已登录直接返回登录成功
        if(GameSdk.isLogined())
        {
            LHUser lhUser = new LHUser();
            LogUtil.e("LHUserManagerProxy", "账号已登录，GameID:" + GameSdk.getLoginGameId());
            User.getInstances().setUId(GameSdk.getLoginUid());
            //初始sessionId与lhUser.setSid不用
            User.getInstances().setSessionId(GameSdk.getSessionId());
            User.getInstances().setGameID(GameSdk.getLoginGameId());

            //sid当做发给后端的参数
            String sid = GameSdk.getLoginUid() + LHStaticValue.demiSplit + GameSdk.getLoginGameId() +
                    LHStaticValue.demiSplit + GameSdk.getSessionId();

            //gameID作为游戏的唯一id
            lhUser.setUid(GameSdk.getLoginGameId());
            lhUser.setSid(sid);
            //设置支付额外参数
            lhUser.setPayExt(GameSdk.getLoginUid());
            LHCallbackListener.getInstance().getUserListener().onLogin(LHStatusCode.LH_LOGIN_SUCCESS, lhUser);
            return;
        }

        GameSdk.openLoginActivity(activity, new CallbackListener<Bundle>() {
            @Override
            public void callback(int responseCode, Bundle bundle)
            {
                LHUser lhUser = new LHUser();
                switch (responseCode)
                {
                    case ErrorCode.SUCCESS:
                        LogUtil.e("nubiya inner login success");

                        User.getInstances().setUId(GameSdk.getLoginUid());
                        //初始sessionId与lhUser.setSid不用
                        User.getInstances().setSessionId(GameSdk.getSessionId());
                        User.getInstances().setGameID(GameSdk.getLoginGameId());

                        //sid当做发给后端的参数
                        String sid = GameSdk.getLoginUid() + LHStaticValue.demiSplit + GameSdk.getLoginGameId() +
                                LHStaticValue.demiSplit + GameSdk.getSessionId();

                        //gameID作为游戏的唯一id
                        lhUser.setUid(GameSdk.getLoginGameId());
                        lhUser.setSid(sid);
                        //设置支付额外参数
                        lhUser.setPayExt(GameSdk.getLoginUid());
                        LHCallbackListener.getInstance().getUserListener().onLogin(LHStatusCode.LH_LOGIN_SUCCESS, lhUser);
                        break;

                    case ErrorCode.NO_PERMISSION:
                        // Android6.0没允许安装和更新所需权限，需要运行时请求，主要是存储权限
                        LogUtil.e("nubiya inner login NO_PERMISSION");

                        LHCallbackListener.getInstance().getUserListener().onLogin(LHStatusCode.LH_LOGIN_FAIL, lhUser);
                        break;

                    default:
                        LogUtil.e("nubiya inner login fail,errorCode:" + responseCode);

                        LHCallbackListener.getInstance().getUserListener().onLogin(LHStatusCode.LH_LOGIN_FAIL, lhUser);
                        break;
                }
            }
        });

    }

    public void loginGuest(Activity activity, Object paramObject) {
    }

    //如果渠道不支持登出，ICheckSupport.IsSupportLogout需要return false
    //并且这里直接执行logout回调，返回成功
    public void logout(Activity activity, Object paramObject) {
        LHCallbackListener.getInstance().getUserListener().onLogout(LHStatusCode.LH_LOGOUT_FAIL, "No Support Logout");
    }

    public void setUserListener(Activity activity, IUserListener userListener) {
        LHCallbackListener.getInstance().setUserListener(userListener);
    }

    public void switchAccount(Activity activity, Object paramObject) {
        //changeAdjunctAccount(Context context, CallbackListener<Bundle> loginListener)
    }

    public void updateUserInfo(Activity activity, LHUser lhUser) {

    }
}