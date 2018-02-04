package com.demiframe.game.api.qihoo.proxy;

import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.content.res.Configuration;

import com.demiframe.game.api.qihoo.base.User;
import com.demiframe.game.api.common.LHCallbackListener;
import com.demiframe.game.api.common.LHResult;
import com.demiframe.game.api.common.LHStaticValue;
import com.demiframe.game.api.common.LHStatusCode;
import com.demiframe.game.api.common.LHUser;
import com.demiframe.game.api.connector.IActivity;
import com.demiframe.game.api.connector.ICheckSupport;
import com.demiframe.game.api.connector.IInitCallback;
import com.demiframe.game.api.util.LHCheckSupport;
import com.demiframe.game.api.util.LogUtil;
import com.qihoo.gamecenter.sdk.activity.ContainerActivity;
import com.qihoo.gamecenter.sdk.common.IDispatcherCallback;
import com.qihoo.gamecenter.sdk.matrix.Matrix;
import com.qihoo.gamecenter.sdk.protocols.CPCallBackMgr;
import com.qihoo.gamecenter.sdk.protocols.ProtocolConfigs;
import com.qihoo.gamecenter.sdk.protocols.ProtocolKeys;

import org.json.JSONObject;

public class LHActivityProxy
        implements IActivity {
    public LHActivityProxy() {
        LHCheckSupport.setCheckSupport(new ICheckSupport() {
            public boolean isAntiAddictionQuery() {
                return false;
            }

            public boolean isSupportBBS() {
                return true;
            }

            public boolean isSupportLogout() {
                return true;
            }

            public boolean isSupportOfficialPlacard() {
                return false;
            }

            public boolean isSupportShowOrHideToolbar() {
                return false;
            }

            public boolean isSupportUserCenter() {
                return false;
            }
        });
    }

    public void onActivityResult(Activity activity, int paramInt1, int paramInt2, Intent paramIntent) {
        Matrix.onActivityResult(activity, paramInt1, paramInt2, paramIntent);
    }

    public void onCreate(final Activity activity, final IInitCallback initCallback, Object obj) {
        CPCallBackMgr.MatrixCallBack callback = new CPCallBackMgr.MatrixCallBack() {
            @Override
            public void execute(Context context, int code, String params) {
                if(code == ProtocolConfigs.FUNC_CODE_SWITCH_ACCOUNT){
                    DoSdkSwitchAccount(activity);
                }else if(code == ProtocolConfigs.FUNC_CODE_INITSUCCESS){
                    LHResult lhResult = new LHResult();
                    lhResult.setCode(LHStatusCode.LH_INIT_SUCCESS);
                    initCallback.onFinished(lhResult);
                }else{
                    //没有定义初始化失败的情况，不成功就定义为失败
                    LHResult lhResult = new LHResult();
                    lhResult.setCode(LHStatusCode.LH_INIT_FAIL);
                    lhResult.setData(params);
                    initCallback.onFinished(lhResult);
                    LogUtil.d("初始化失败");
                }
            }
        };

        Matrix.setActivity(activity, callback);
    }

    public void DoSdkSwitchAccount(Activity activity){
        Intent intent = new Intent(activity, ContainerActivity.class);
        intent.putExtra(ProtocolKeys.IS_SCREEN_ORIENTATION_LANDSCAPE, LHStaticValue.IsLandScape);
        intent.putExtra(ProtocolKeys.FUNCTION_CODE, ProtocolConfigs.FUNC_CODE_SWITCH_ACCOUNT);

        Matrix.invokeActivity(activity, intent, new IDispatcherCallback() {
            @Override
            public void onFinished(String s) {
                try{
                    JSONObject jsonObj = new JSONObject(s);
                    int errno = jsonObj.optInt("errno", -1);
                    if(errno == -1){
                        //TODO:切换失败的时候，是否需要回调
                        LHUser lhUser = new LHUser();
                        lhUser.setLoginMsg("切换失败");
                        LHCallbackListener.getInstance().getUserListener().onLogin(LHStatusCode.LH_LOGIN_FAIL, lhUser);
                    }
                    else{
                        //清空角色缓存
                        User.getInstances().Clear();

                        String jsonData = jsonObj.getString("data");
                        JSONObject dataJsonObj = new JSONObject(jsonData);
                        String sid = dataJsonObj.getString("access_token");

                        LHUser lhUser = new LHUser();
                        lhUser.setSid(sid);
                        LHCallbackListener.getInstance().getUserListener().onLogin(LHStatusCode.LH_LOGIN_SUCCESS, lhUser);
                    }
                }catch (Exception e){
                    e.printStackTrace();

                    LHUser lhUser = new LHUser();
                    lhUser.setLoginMsg("切换登录数据解析异常");
                    LHCallbackListener.getInstance().getUserListener().onLogin(LHStatusCode.LH_LOGIN_FAIL, lhUser);
                }
            }
        });
    }

    public void afterOnCreate(Activity activity, IInitCallback listener, Object obj){
        
    }

    public Boolean needAfterCreate(){
        return false;
    }

    public void onDestroy(Activity activity) {
        Matrix.destroy(activity);
    }

    public void onNewIntent(Activity activity, Intent intent) {
        Matrix.onNewIntent(activity, intent);

    }

    public void onPause(Activity activity) {
        Matrix.onPause(activity);
    }

    public void onRestart(Activity activity) {
        Matrix.onRestart(activity);
    }

    public void onResume(Activity activity) {
        Matrix.onStop(activity);
    }

    public void onStop(Activity activity) {
        Matrix.onStop(activity);
    }

    public void onStart(Activity activity){
        Matrix.onStart(activity);
    }

    public void onConfigurationChanged(Activity activity, Configuration newConfig){

    }
}