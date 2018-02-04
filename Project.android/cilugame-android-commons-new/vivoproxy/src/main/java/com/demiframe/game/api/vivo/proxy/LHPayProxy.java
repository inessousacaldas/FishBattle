package com.demiframe.game.api.vivo.proxy;

import android.app.Activity;
import com.demiframe.game.api.common.LHPayInfo;
import com.demiframe.game.api.common.LHResult;
import com.demiframe.game.api.common.LHStatusCode;
import com.demiframe.game.api.connector.IPay;
import com.demiframe.game.api.util.LogUtil;

import com.demiframe.game.api.util.SDKTools;
import com.demiframe.game.api.vivo.base.User;
import com.vivo.unionsdk.open.VivoPayCallback;
import com.vivo.unionsdk.open.VivoPayInfo;
import com.vivo.unionsdk.open.VivoUnionSDK;

import org.json.JSONException;
import org.json.JSONObject;



public class LHPayProxy implements IPay
{
    public void onPay(final Activity activity, final LHPayInfo lhPayInfo)
    {
        LogUtil.d("LHPayProxy", "VIVO ONPAY START");

        try {
            JSONObject extraJson = new JSONObject(lhPayInfo.getExtraJson());

            VivoPayInfo payInfo = new VivoPayInfo(
                    lhPayInfo.getProductName(),
                    lhPayInfo.getProductDes(),
                    lhPayInfo.getProductPrice(true),
                    extraJson.getString("accessKey"),
                    SDKTools.GetSdkProperty(activity, "VIVO_APPID"),
                    extraJson.getString("orderNumber"),
                    User.getInstances().getOpenId()
            );
            VivoUnionSDK.pay(activity, payInfo, new VivoPayCallback() {
                @Override
                public void onVivoPayResult(String transNo, boolean isSucc, String errorCode) {
                    LHResult lhResult = new LHResult();
                    if(isSucc){
                        lhResult.setCode(LHStatusCode.LH_PAY_SUCCESS);
                    }
                    else{
                        lhResult.setCode(LHStatusCode.LH_PAY_FAIL);
                    }
                    lhPayInfo.getPayCallback().onFinished(lhResult);
                }
            });
        }
        catch (JSONException e) {
            e.printStackTrace();
        }
        LogUtil.d("LHPayProxy", "VIVO ONPAY END");
    }
}