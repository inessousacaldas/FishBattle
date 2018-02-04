package com.demiframe.game.api.oppo.proxy;

import android.app.Activity;
import android.util.Log;

import com.demiframe.game.api.common.LHCallbackListener;
import com.demiframe.game.api.common.LHStaticValue;
import com.demiframe.game.api.common.LHStatusCode;
import com.demiframe.game.api.common.LHUser;
import com.demiframe.game.api.connector.IUserListener;
import com.demiframe.game.api.connector.IUserManager;
import com.demiframe.game.api.util.LogUtil;
import com.nearme.game.sdk.GameCenterSDK;
import com.nearme.game.sdk.callback.ApiCallback;
import com.nearme.game.sdk.common.model.ApiResult;

import org.json.JSONObject;

public class LHUserManagerProxy
        implements IUserManager {
    Activity mActivity;
    IUserListener mUserListener;

    public void guestRegist(Activity activity, String paramString) {

    }

    public void login(Activity activity, Object paramObject) {
        GameCenterSDK.getInstance().doLogin(activity, new ApiCallback() {
            @Override
            public void onSuccess(String s) {
                GameCenterSDK.getInstance().doGetTokenAndSsoid(doGetTokenAndSsoidCallback);
            }

            @Override
            public void onFailure(String s, int i) {
                LHUser lhUser = new LHUser();
                lhUser.setLoginMsg(i + " " + s);
                LogUtil.d(i + " " + s);
                LHCallbackListener.getInstance().getUserListener().onLogin(LHStatusCode.LH_LOGIN_FAIL, lhUser);
            }
        });
    }

    public void loginGuest(Activity activity, Object paramObject) {
    }

    public void logout(Activity activity, Object paramObject) {
        LHCallbackListener.getInstance().getUserListener().onLogout(LHStatusCode.LH_LOGIN_FAIL,paramObject);
    }

    public void setUserListener(Activity activity, IUserListener userListener) {
        LHCallbackListener.getInstance().setUserListener(userListener);
    }

    public void switchAccount(Activity activity, Object paramObject) {
    }

    public void updateUserInfo(Activity activity, LHUser lhUser) {

    }

    private ApiCallback doGetTokenAndSsoidCallback = new ApiCallback() {
        @Override
        public void onSuccess(String resultMsg) {
            try {
                JSONObject json = new JSONObject(resultMsg);

                String token = json.getString("token");
                String ssoid = json.getString("ssoid");
                String sid = ssoid+ LHStaticValue.demiSplit + token;
                LHUser lhUser = new LHUser();
                lhUser.setSid(sid);
                LHCallbackListener.getInstance().getUserListener().onLogin(LHStatusCode.LH_LOGIN_SUCCESS, lhUser);
                LogUtil.d("登录成功");

            } catch (Exception e) {
                e.printStackTrace();
            }
        }

        @Override
        public void onFailure(String s, int i) {
            LHUser lhUser = new LHUser();
            lhUser.setLoginMsg(i + " " + s);
            LogUtil.d(i + " " + s);
            LHCallbackListener.getInstance().getUserListener().onLogin(LHStatusCode.LH_LOGIN_FAIL, lhUser);
        }
    };
}