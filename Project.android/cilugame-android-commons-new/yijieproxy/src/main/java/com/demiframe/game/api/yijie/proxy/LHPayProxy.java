package com.demiframe.game.api.yijie.proxy;

import android.app.Activity;

import com.demiframe.game.api.common.LHPayInfo;
import com.demiframe.game.api.common.LHResult;
import com.demiframe.game.api.common.LHStatusCode;
import com.demiframe.game.api.connector.IPay;
import com.snowfish.cn.ganga.helper.SFOnlineHelper;
import com.snowfish.cn.ganga.helper.SFOnlinePayResultListener;

public class LHPayProxy
  implements IPay
{

  public void onPay(final Activity activity, final LHPayInfo lhPayInfo)
  {
    SFOnlineHelper.pay(activity, Integer.parseInt(lhPayInfo.getProductPrice(false)),
            lhPayInfo.getProductName(),
            Integer.parseInt(lhPayInfo.getProductCount()),
            lhPayInfo.getPayCustomInfo(),
            lhPayInfo.getPayNotifyUrl(),

            new SFOnlinePayResultListener() {
              @Override
              public void onFailed(String s) {
                LHResult lhResult = new LHResult();
                lhResult.setCode(LHStatusCode.LH_PAY_FAIL);
                lhResult.setData(s);
                lhPayInfo.getPayCallback().onFinished(lhResult);
              }

              @Override
              public void onSuccess(String s) {
                LHResult lhResult = new LHResult();
                lhResult.setCode(LHStatusCode.LH_PAY_SUCCESS);
                lhResult.setData(s);
                lhPayInfo.getPayCallback().onFinished(lhResult);
              }

              @Override
              public void onOderNo(String s) {
                LHResult lhResult = new LHResult();
                lhResult.setCode(LHStatusCode.LH_PAY_CANCEL);
                lhResult.setData(s);
                lhPayInfo.getPayCallback().onFinished(lhResult);
              }
            });
  }
}