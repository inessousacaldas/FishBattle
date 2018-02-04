package com.demiframe.game.api.haima.proxy;

import android.app.Activity;

import com.demiframe.game.api.common.LHPayInfo;
import com.demiframe.game.api.common.LHResult;
import com.demiframe.game.api.common.LHStaticValue;
import com.demiframe.game.api.common.LHStatusCode;
import com.demiframe.game.api.connector.IPay;
import com.demiframe.game.api.util.LogUtil;
import com.haimawan.paysdk.cpapi.CPOrderInfo;
import com.haimawan.paysdk.cpapi.ErrorInfoBean;
import com.haimawan.paysdk.cpapi.OnPayListener;
import com.haimawan.paysdk.enter.HMPay;

public class LHPayProxy
        implements IPay {

    public void onPay(final Activity activity, final LHPayInfo lhPayInfo) {
        int totalCent = lhPayInfo.getTotalPriceCent();
        CPOrderInfo orderInfo = new CPOrderInfo();
        orderInfo.setGameName(LHStaticValue.appName);
        orderInfo.setGoodsName(lhPayInfo.getProductName());
        orderInfo.setGoodsPrice(totalCent/100f);

        orderInfo.setOrderNo(lhPayInfo.getOrderSerial());
        orderInfo.setUserParam("");
        orderInfo.setShowUrl(lhPayInfo.getPayNotifyUrl());

        try {
            HMPay.pay(activity, orderInfo, new OnPayListener() {
                @Override
                public void onPaySuccess(CPOrderInfo cpOrderInfo) {
                    LHResult lhResult = new LHResult();
                    lhResult.setCode(LHStatusCode.LH_PAY_SUCCESS);
                    lhPayInfo.getPayCallback().onFinished(lhResult);
                }

                @Override
                public void onPayFailed(CPOrderInfo cpOrderInfo, ErrorInfoBean errorInfoBean) {
                    LogUtil.d("pay error code="+errorInfoBean.getStateCode() +" msg="+errorInfoBean.getErrorMessage());

                    LHResult lhResult = new LHResult();
                    lhResult.setCode(LHStatusCode.LH_PAY_FAIL);
                    lhPayInfo.getPayCallback().onFinished(lhResult);
                }
            });
        } catch (Exception e) {
            e.printStackTrace();
        }
    }
}