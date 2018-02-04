package com.demiframe.game.api.vivo.proxy;

import android.app.Activity;

import com.demiframe.game.api.common.LHCallbackListener;
import com.demiframe.game.api.common.LHStatusCode;
import com.demiframe.game.api.common.LHUser;
import com.demiframe.game.api.connector.IUserListener;
import com.demiframe.game.api.connector.IUserManager;
import com.demiframe.game.api.util.IOUtil;
import com.demiframe.game.api.util.LogUtil;

import com.demiframe.game.api.util.SDKTools;
import com.demiframe.game.api.vivo.base.User;
import com.vivo.unionsdk.open.VivoAccountCallback;
import com.vivo.unionsdk.open.VivoUnionSDK;
import com.vivo.unionsdk.open.VivoConstants;


public class LHUserManagerProxy implements IUserManager {
    public void guestRegist(Activity activity, String paramString) {

    }

    public void login(Activity activity, Object paramObject)
    {
        VivoUnionSDK.login(activity);
    }

    public void loginGuest(Activity activity, Object paramObject) {
    }

    public void logout(Activity activity, Object paramObject){
        LogUtil.e("LHUserManagerProxy", "vivo inner logout");
        LHCallbackListener.getInstance().getUserListener().onLogout(LHStatusCode.LH_LOGOUT_FAIL,paramObject);
    }

    public void setUserListener(Activity activity, IUserListener userListener)
    {
        LHCallbackListener.getInstance().setUserListener(userListener);
    }

    public void switchAccount(Activity activity, Object paramObject) {
    }

    public void updateUserInfo(Activity activity, LHUser lhUser) {

    }

    public static VivoAccountCallback accountListener = new VivoAccountCallback(){
        @Override
        public void onVivoAccountLogin(String name, String openid, String authtoken){
            LogUtil.d("---------- OnVivoAccountChangedListener onAccountLogin ----------");
            LogUtil.d("authtoken = " + authtoken);
            LogUtil.d("openid = " + openid);
            //缓存玩家信息
            User.getInstances().setOpenId(openid);
            User.getInstances().setAuthtoken(authtoken);
            User.getInstances().setName(name);

            LHUser lhUser = new LHUser();
            lhUser.setUid(openid);
            lhUser.setSid(authtoken);
            LHCallbackListener.getInstance().getUserListener().onLogin(LHStatusCode.LH_LOGIN_SUCCESS, lhUser);
        }

        @Override
        public void onVivoAccountLogout(int requestCode){
            LHUser lhUser = new LHUser();
            LogUtil.d("onVivoAccountLogout code="+requestCode);
            if(requestCode == VivoConstants.LOGOUT_SWICH_ACCOUNT||
                    requestCode == VivoConstants.LOGOUT_USER_LOGOUT)
            {
                User.getInstances().Clear();
                lhUser.setLoginMsg("登出操作成功");
                LHCallbackListener.getInstance().getUserListener().onLogin(LHStatusCode.LH_LOGOUT_SUCCESS, lhUser);
                User.getInstances().Clear();
            }
            else{
                lhUser.setLoginMsg("登出操作失败");
                LHCallbackListener.getInstance().getUserListener().onLogin(LHStatusCode.LH_LOGOUT_FAIL, lhUser);
            }
        }

        @Override
        public void onVivoAccountLoginCancel(){
            LogUtil.d("---------- OnVivoAccountChangedListener onAccountLoginCancled ----------");
            LHUser lhUser = new LHUser();
            lhUser.setLoginMsg("取消登录操作");
            LHCallbackListener.getInstance().getUserListener().onLogin(LHStatusCode.LH_LOGIN_CANCEL, lhUser);
        }
    };
}