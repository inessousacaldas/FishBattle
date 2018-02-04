package com.demiframe.game.api.uc.proxy;

import android.app.Activity;
import android.text.TextUtils;

import com.demiframe.game.api.common.LHPayInfo;
import com.demiframe.game.api.common.LHResult;
import com.demiframe.game.api.common.LHRole;
import com.demiframe.game.api.common.LHStatusCode;
import com.demiframe.game.api.connector.IPay;
import com.demiframe.game.api.uc.base.UcHelper;
import com.demiframe.game.api.uc.ucutils.LHUCGameSdk;
import com.demiframe.game.api.util.LogUtil;

import org.json.JSONObject;

public class LHPayProxy
  implements IPay
{
  private boolean checkPayInfo(LHPayInfo lhPayInfo)
  {
    if (lhPayInfo == null)
    {
      LogUtil.e("pay params error");
      return false;
    }
    if (lhPayInfo.getPayCallback() == null)
    {
      LogUtil.e("IPayCallback is null");
      return false;
    }
    if (TextUtils.isEmpty(lhPayInfo.getOrderSerial()))
    {
      LogUtil.e("OrderSerial is null");
      return false;
    }
    return true;
  }

  public void onPay(Activity activity, LHPayInfo lhPayInfo)
  {
    if(!checkPayInfo(lhPayInfo)){
      return;
    }

    LHRole lhRole = UcHelper.getInstances().getRole();
    if(!UcHelper.getInstances().checkLogin() || lhRole == null){
      LHResult lhResult = new LHResult();
      lhResult.setCode(LHStatusCode.LH_PAY_FAIL);
      lhResult.setData("未登录");
      lhPayInfo.getPayCallback().onFinished(lhResult);
      return;
    }

    try {
      JSONObject extraJson = new JSONObject(lhPayInfo.getExtraJson());
      String callbackInfo = extraJson.getString("callbackInfo");
      String amount = extraJson.getString("amount");
      String notifyUrl = extraJson.getString("notifyUrl");
      String cpOrderId = extraJson.getString("cpOrderId");
      String accountId = extraJson.getString("accountId");
      String signType = extraJson.getString("signType");
      String sign = extraJson.getString("sign");

      LogUtil.d(cpOrderId + "  "+notifyUrl);

      LHUCGameSdk.pay(activity, accountId, cpOrderId, amount, callbackInfo, notifyUrl, signType, sign);
    }catch (Exception e){
      e.printStackTrace();
      return;
    }

  }
}