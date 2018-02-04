package com.demiframe.game.api.qihoo.proxy;

import android.app.Activity;
import android.content.Intent;

import com.demiframe.game.api.qihoo.base.User;
import com.demiframe.game.api.common.LHCallbackListener;
import com.demiframe.game.api.common.LHStaticValue;
import com.demiframe.game.api.common.LHStatusCode;
import com.demiframe.game.api.common.LHUser;
import com.demiframe.game.api.connector.IUserListener;
import com.demiframe.game.api.connector.IUserManager;
import com.demiframe.game.api.util.LogUtil;
import com.qihoo.gamecenter.sdk.activity.ContainerActivity;
import com.qihoo.gamecenter.sdk.common.IDispatcherCallback;
import com.qihoo.gamecenter.sdk.matrix.Matrix;
import com.qihoo.gamecenter.sdk.protocols.ProtocolConfigs;
import com.qihoo.gamecenter.sdk.protocols.ProtocolKeys;

import org.json.JSONObject;

public class LHUserManagerProxy
        implements IUserManager {
    Activity mActivity;
    IUserListener mUserListener;

    public void guestRegist(Activity activity, String paramString) {

    }

    public void login(Activity activity, Object paramObject) {
        IDispatcherCallback callback = new IDispatcherCallback() {
            @Override
            public void onFinished(String s) {
                try{
                    JSONObject jsonObj = new JSONObject(s);
                    int errno = jsonObj.optInt("errno", -1);
                    if(errno == -1){
                        //登录失败或取消
                        LHUser lhUser = new LHUser();
                        lhUser.setLoginMsg("登录失败");
                        LHCallbackListener.getInstance().getUserListener().onLogin(LHStatusCode.LH_LOGIN_FAIL, lhUser);
                    }
                    else{
                        String jsonData = jsonObj.getString("data");
                        JSONObject dataJsonObj = new JSONObject(jsonData);
                        String sid = dataJsonObj.getString("access_token");

                        //清空前一次的记录
                        User.getInstances().Clear();

                        LHUser lhUser = new LHUser();
                        lhUser.setSid(sid);
                        LHCallbackListener.getInstance().getUserListener().onLogin(LHStatusCode.LH_LOGIN_SUCCESS, lhUser);
                    }
                }catch (Exception e){
                    e.printStackTrace();

                    LHUser lhUser = new LHUser();
                    lhUser.setLoginMsg("登录数据解析异常");
                    LHCallbackListener.getInstance().getUserListener().onLogin(LHStatusCode.LH_LOGIN_FAIL, lhUser);
                }
            }
        };

        Intent intent = new Intent(activity, ContainerActivity.class);
        intent.putExtra(ProtocolKeys.IS_SCREEN_ORIENTATION_LANDSCAPE, LHStaticValue.IsLandScape);
        intent.putExtra(ProtocolKeys.FUNCTION_CODE, ProtocolConfigs.FUNC_CODE_LOGIN);

        //TODO：测试参数，上线去掉
        intent.putExtra(ProtocolKeys.IS_SOCIAL_SHARE_DEBUG, true);

        Matrix.execute(activity, intent, callback);

    }

    public void loginGuest(Activity activity, Object paramObject) {
    }

    public void logout(final Activity activity, Object paramObject) {
        if(!User.getInstances().CheckLogin()){
            LHCallbackListener.getInstance().getUserListener().onLogout(LHStatusCode.LH_LOGOUT_FAIL, "onLogoutSucc");
            return;
        }

        Intent intent = getLogoutIntent();
        Matrix.execute(activity, intent, new IDispatcherCallback() {
            @Override
            public void onFinished(String data) {
                LogUtil.d("登出"+data);
                User.getInstances().Clear();
                LHCallbackListener.getInstance().getUserListener().onLogout(LHStatusCode.LH_LOGOUT_SUCCESS, "onLogoutSucc");

            }
        });
    }

    private Intent getLogoutIntent(){

        /*
         * 必须参数：
         *  function_code : 必须参数，表示调用SDK接口执行的功能
        */
        Intent intent = new Intent();
        intent.putExtra(ProtocolKeys.FUNCTION_CODE, ProtocolConfigs.FUNC_CODE_LOGOUT);
        return intent;
    }


    public void setUserListener(Activity activity, IUserListener userListener) {
        LHCallbackListener.getInstance().setUserListener(userListener);
    }

    public void switchAccount(Activity activity, Object paramObject) {
    }

    public void updateUserInfo(Activity activity, LHUser lhUser) {
        User.getInstances().setUserInfo(lhUser);
    }
}