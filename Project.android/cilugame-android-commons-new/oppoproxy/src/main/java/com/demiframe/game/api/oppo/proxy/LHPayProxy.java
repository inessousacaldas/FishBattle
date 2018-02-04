package com.demiframe.game.api.oppo.proxy;

import android.app.Activity;

import com.demiframe.game.api.common.LHCallbackListener;
import com.demiframe.game.api.common.LHPayInfo;
import com.demiframe.game.api.common.LHResult;
import com.demiframe.game.api.common.LHStaticValue;
import com.demiframe.game.api.common.LHStatusCode;
import com.demiframe.game.api.connector.IHandleCallback;
import com.demiframe.game.api.connector.IPay;
import com.demiframe.game.api.util.LogUtil;
import com.nearme.game.sdk.GameCenterSDK;
import com.nearme.game.sdk.callback.ApiCallback;
import com.nearme.game.sdk.common.model.biz.PayInfo;

public class LHPayProxy
        implements IPay {

    public void onPay(final Activity activity, final LHPayInfo lhPayInfo) {
        LHExtendProxy.OnRealNameRegister(activity, new IHandleCallback()
        {
            public void onFinished(LHResult lhResult)
            {
                //实名认证通过才能够支付
                if(lhResult.getCode() != LHStatusCode.LH_ADDICTED_NO_USER &&
                        lhResult.getCode() != LHStatusCode.LH_ADDICTED_NO_USER_QUIT &&
                        lhResult.getCode() != LHStatusCode.LH_ADDICTED_EXCEPTION){
                    OnCommitPay(activity, lhPayInfo);
                }
            }
        });
    }

    public void OnCommitPay(final Activity activity, final LHPayInfo lhPayInfo){
        LHCallbackListener.getInstance().setPayListener(lhPayInfo.getPayCallback());
        PayInfo payInfo;
        try{
            LogUtil.d(lhPayInfo.getServerId());
            LogUtil.d(lhPayInfo.getOrderSerial());

            //充值金额
            int total = Integer.parseInt(lhPayInfo.getProductCount()) * Integer.parseInt(lhPayInfo.getProductPrice(true));
            payInfo = new PayInfo(lhPayInfo.getOrderSerial(),"",total);

            payInfo.setProductDesc(lhPayInfo.getProductDes());
            payInfo.setProductName(lhPayInfo.getProductName());
            payInfo.setCallbackUrl(lhPayInfo.getPayNotifyUrl());
        }catch (Exception e){
            e.printStackTrace();
            return;
        }
        GameCenterSDK.getInstance().doPay(activity,payInfo,doPayCallback);
    }

    private ApiCallback doPayCallback = new ApiCallback() {
        @Override
        public void onSuccess(String s) {
            LHResult lhResult = new LHResult();
            lhResult.setCode(LHStatusCode.LH_INIT_SUCCESS);
            LHCallbackListener.getInstance().getPayListener().onFinished(lhResult);
        }

        @Override
        public void onFailure(String s, int i) {
            LHResult lhResult = new LHResult();
            lhResult.setCode(LHStatusCode.LH_INIT_FAIL);
            lhResult.setData(s);
            LHCallbackListener.getInstance().getPayListener().onFinished(lhResult);
        }
    };
}