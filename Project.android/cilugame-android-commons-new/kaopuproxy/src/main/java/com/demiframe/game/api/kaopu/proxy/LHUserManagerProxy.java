package com.demiframe.game.api.kaopu.proxy;

import android.app.Activity;

import com.demiframe.game.api.common.LHCallbackListener;
import com.demiframe.game.api.common.LHStatusCode;
import com.demiframe.game.api.common.LHUser;
import com.demiframe.game.api.connector.IUserListener;
import com.demiframe.game.api.connector.IUserManager;
import com.demiframe.game.api.kaopu.base.User;
import com.demiframe.game.api.util.LogUtil;
import com.kaopu.supersdk.api.KPSuperSDK;
import com.kaopu.supersdk.callback.KPLoginCallBack;
import com.kaopu.supersdk.callback.KPLogoutCallBack;
import com.kaopu.supersdk.model.UserInfo;

public class LHUserManagerProxy
        implements IUserManager {
    public void guestRegist(Activity activity, String paramString) {

    }

    public void login(Activity activity, Object paramObject) {
        KPSuperSDK.login(activity, new KPLoginCallBack() {
            @Override
            public void onLoginSuccess(UserInfo userInfo) {
                LHUser lhUser = new LHUser();
                //openid当作uid使用
                lhUser.setUid(userInfo.getOpenid());
                //拼接后的验证url当作sid
                lhUser.setSid(userInfo.getVerifyurl());
                LHCallbackListener.getInstance().getUserListener().onLogin(LHStatusCode.LH_LOGIN_SUCCESS, lhUser);
            }

            @Override
            public void onLoginFailed() {
                LHUser lhUser = new LHUser();
                lhUser.setLoginMsg("登录失败");
                LHCallbackListener.getInstance().getUserListener().onLogin(LHStatusCode.LH_LOGIN_FAIL, lhUser);
            }

            @Override
            public void onLoginCanceled() {
                LHUser lhUser = new LHUser();
                lhUser.setLoginMsg("取消登录操作");
                LHCallbackListener.getInstance().getUserListener().onLogin(LHStatusCode.LH_LOGIN_CANCEL, lhUser);
            }
        }, "");
    }

    public void loginGuest(Activity activity, Object paramObject) {
    }

    //如果渠道不支持登出，ICheckSupport.IsSupportLogout需要return false
    //并且这里直接执行logout回调，返回成功
    public void logout(Activity activity, Object paramObject) {
        KPSuperSDK.logoutAccount();
    }

    public void setUserListener(Activity activity, IUserListener userListener) {
        LHCallbackListener.getInstance().setUserListener(userListener);
    }

    public void switchAccount(Activity activity, Object paramObject) {
    }

    public void updateUserInfo(Activity activity, LHUser lhUser) {

    }

    public static KPLogoutCallBack logoutCallback = new KPLogoutCallBack() {
        @Override
        public void onLogout(boolean haveCallLogin) {
            User.getInstances().Clear();

            if(LHCallbackListener.getInstance().getUserListener() == null)
                return;

            if(haveCallLogin){
                LHCallbackListener.getInstance().getUserListener().onLogout(LHStatusCode.LH_LOGOUT_SUCCESS, "haveCallLogin");
            }else{
                LHCallbackListener.getInstance().getUserListener().onLogout(LHStatusCode.LH_LOGOUT_SUCCESS, "nothaveCallLogin");
            }
        }
    };
}