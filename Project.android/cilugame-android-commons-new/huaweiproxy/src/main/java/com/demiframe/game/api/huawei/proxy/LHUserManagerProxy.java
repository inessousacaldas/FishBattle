package com.demiframe.game.api.huawei.proxy;

import android.app.Activity;

import com.demiframe.game.api.common.LHStaticValue;
import com.demiframe.game.api.common.LHStatusCode;
import com.demiframe.game.api.common.LHUser;
import com.demiframe.game.api.connector.IUserListener;
import com.demiframe.game.api.connector.IUserManager;
import com.demiframe.game.api.huawei.util.Sign;
import com.demiframe.game.api.util.IOUtil;
import com.demiframe.game.api.util.LogUtil;
import com.demiframe.game.api.util.SDKTools;
import com.huawei.gameservice.sdk.GameServiceSDK;
import com.huawei.gameservice.sdk.control.GameEventHandler;
import com.huawei.gameservice.sdk.model.Result;
import com.huawei.gameservice.sdk.model.UserResult;

import java.util.ArrayList;
import java.util.List;

public class LHUserManagerProxy
        implements IUserManager {
    Activity mActivity;
    IUserListener mUserListener;

    public void guestRegist(Activity activity, String paramString) {

    }

    public void login(Activity activity, Object paramObject) {
        final String appID = SDKTools.GetSdkProperty(activity, "HUAWEI_APPID");
        GameServiceSDK.login(activity, new GameEventHandler() {
            @Override
            public void onResult(Result result) {
                if (result.rtnCode != Result.RESULT_OK) {
                    if(result.rtnCode == Result.RESULT_ERR_CANCEL){
                        LHUser lhUser = new LHUser();
                        lhUser.setLoginMsg("登录取消");
                        mUserListener.onLogin(LHStatusCode.LH_LOGIN_CANCEL, lhUser);
                        LogUtil.d("login cancel:" + result.toString());
                    }
                    else{
                        LHUser lhUser = new LHUser();
                        lhUser.setLoginMsg("登录失败");
                        mUserListener.onLogin(LHStatusCode.LH_LOGIN_FAIL, lhUser);
                        LogUtil.d("login failed:" + result.toString());
                    }
                } else {
                    UserResult userResult = (UserResult) result;
                    if (userResult.isAuth != null && userResult.isAuth == 1) {
                        LHUser lhUser = new LHUser();
                        String sid = appID + LHStaticValue.demiSplit +
                                userResult.ts + LHStaticValue.demiSplit +
                                userResult.playerId + LHStaticValue.demiSplit +
                                userResult.gameAuthSign;
                        lhUser.setSid(sid);
                        mUserListener.onLogin(LHStatusCode.LH_LOGIN_SUCCESS, lhUser);
                        LogUtil.d("login success:" + userResult.toString());
                    }
                    //切换账号
                    else if (userResult.isChange != null && userResult.isChange == 1) {
                        mUserListener.onLogout(LHStatusCode.LH_LOGOUT_SUCCESS, "SwitchLogout");
                    } else {
                        //华为登录会返回两次，一次只返回playerId（可用其免验证登录），后面一次带其他参数的
                        //需要服务端去校验登录是否合法。
                        //这里去掉这种异步的处理
//                        LHUser lhUser = new LHUser();
//                        lhUser.setSid(userResult.playerId);
//                        mUserListener.onLogin(LHStatusCode.LH_LOGIN_SUCCESS, lhUser);
                        LogUtil.d("login return success no check:" + userResult.toString());
                    }
                }
            }

            @Override
            public String getGameSign(String appId, String cpId, String ts) {
                LogUtil.d("login getGameSign "+appId+cpId);
                return Sign.createGameSign(appId + cpId + ts);
            }
        }, 1);
    }

    public void loginGuest(Activity activity, Object paramObject) {
    }

    public void logout(Activity activity, Object paramObject) {
        mUserListener.onLogout(LHStatusCode.LH_LOGOUT_FAIL, "登出失败");
    }

    public void setUserListener(Activity activity, IUserListener userListener) {
        mUserListener = userListener;
    }

    public void switchAccount(Activity activity, Object paramObject) {
    }

    public void updateUserInfo(Activity activity, LHUser lhUser) {

    }
}