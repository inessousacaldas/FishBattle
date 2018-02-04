package com.demiframe.game.api.demi.pay.wxpay;

import android.app.Activity;
import android.widget.Toast;

import com.demiframe.game.api.demi.proxy.LHPayProxy;
import com.demiframe.game.api.demi.demiutil.Logger;
import com.demiframe.game.api.util.SDKTools;
import com.tencent.mm.sdk.modelpay.PayReq;
import com.tencent.mm.sdk.openapi.IWXAPI;
import com.tencent.mm.sdk.openapi.WXAPIFactory;

import org.json.JSONObject;

/**
 * Created by senkay on 12/27/16.
 */
public class WxpayAdapter
{
    private IWXAPI api;

    public void DoPay(
            final Activity activity,
            final String orderJsonStr
    )
    {
        Logger.Log("Call WxpayAdapter DoPay");

        if (api == null)
        {
            String appID = SDKTools.GetSdkProperty(activity, "WXPAY_APPID");
            Logger.Log("createWXAPI APP_ID=" + appID);
            api = WXAPIFactory.createWXAPI(activity, appID);
            api.registerApp(appID);
        }

        try
        {
            JSONObject orderJson = new JSONObject(orderJsonStr);

            PayReq req = new PayReq();
            req.appId			= orderJson.getString("appid");
            req.partnerId		= orderJson.getString("partnerid");
            req.prepayId		= orderJson.getString("prepayid");
            req.nonceStr		= orderJson.getString("noncestr");
            req.timeStamp		= orderJson.getString("timestamp");
            req.packageValue	= orderJson.getString("package");
            req.sign			= orderJson.getString("sign");
            req.extData			= "app data"; // optional

            Logger.Log("Call PayReq sendReq");

            api.sendReq(req);

        }catch(Exception e){
            Logger.Error("订单请求失败", e);
            Toast.makeText(activity, "订单请求失败 " + e.getMessage(), Toast.LENGTH_SHORT).show();
        }
    }
}