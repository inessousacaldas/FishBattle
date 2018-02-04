package com.demiframe.game.api.oppo.proxy;

import android.app.Activity;
import android.widget.Toast;

import com.demiframe.game.api.common.LHResult;
import com.demiframe.game.api.common.LHRole;
import com.demiframe.game.api.common.LHStatusCode;
import com.demiframe.game.api.connector.IExtend;
import com.demiframe.game.api.connector.IHandleCallback;
import com.demiframe.game.api.util.IOUtil;
import com.demiframe.game.api.util.LogUtil;
import com.nearme.game.sdk.GameCenterSDK;
import com.nearme.game.sdk.callback.ApiCallback;
import com.nearme.game.sdk.common.model.ApiResult;
import com.nearme.game.sdk.common.model.biz.ReportUserGameInfoParam;

public class LHExtendProxy
        implements IExtend {
    public void antiAddictionQuery(final Activity activity, IHandleCallback handleCallback) {
    }

    public void enterBBS(final Activity activity) {
    }

    public void enterUserCenter(final Activity activity) {
        GameCenterSDK.getInstance().launchGameCenter(activity);
    }

    //实名认证
    public void realNameRegister(final Activity activity, final IHandleCallback callback) {
        OnRealNameRegister(activity, callback);
    }

    public static void OnRealNameRegister(final Activity activity, final IHandleCallback callback){
        LogUtil.d("实名制校验");
        GameCenterSDK.getInstance().doGetVerifiedInfo(new ApiCallback() {
            @Override
            public void onSuccess(String resultMsg) {
                LogUtil.d("doGetVerifiedInfo onSuccess");
                LHResult lhResult = new LHResult();
                try {
                    //解析年龄age
                    int age=Integer.parseInt(resultMsg);
                    if (age < 18) {
                        lhResult.setCode(LHStatusCode.LH_ADDICTED_MINORITY);
                    } else {
                        lhResult.setCode(LHStatusCode.LH_ADDICTED_ADULTS);
                    }
                } catch (Exception e) {
                    lhResult.setCode(LHStatusCode.LH_ADDICTED_EXCEPTION);
                    e.printStackTrace();
                }
                callback.onFinished(lhResult);
            }

            //未认证先不管,可跳过
            @Override
            public void onFailure(String resultMsg, int resultCode) {
                LogUtil.d("doGetVerifiedInfo onFailure");
                LHResult lhResult = new LHResult();
                if(resultCode == ApiResult.RESULT_CODE_VERIFIED_FAILED_AND_RESUME_GAME){
                    lhResult.setCode(LHStatusCode.LH_ADDICTED_NO_USER);
                }else if(resultCode == ApiResult.RESULT_CODE_VERIFIED_FAILED_AND_STOP_GAME){
                    lhResult.setCode(LHStatusCode.LH_ADDICTED_NO_USER_QUIT);
                }
                callback.onFinished(lhResult);
            }
        });
    }

    public void submitRoleData(final Activity activity, LHRole lhRole) {
        String gameId = IOUtil.getMetaDataByName(activity, "app_id");
        ReportUserGameInfoParam param = new ReportUserGameInfoParam(gameId,lhRole.getZoneId(),lhRole.getRoleName(),lhRole.getRoledId());

        GameCenterSDK.getInstance().doReportUserGameInfoData(param, new ApiCallback() {
            @Override
            public void onSuccess(String resultMsg) {
            }
            @Override
            public void onFailure(String resultMsg, int resultCode) {
            }
        });
    }

    //提前获取渠道号
    public String getSubChannelId(Activity activity){
        return "oppo";
    }

    public void GainGameCoin(Activity activity, String jsonStr){

    }

    public void ConsumeGameCoin(Activity activity, String jsonStr){

    }
}