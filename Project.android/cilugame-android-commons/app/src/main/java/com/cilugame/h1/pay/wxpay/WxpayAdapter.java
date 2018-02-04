package com.cilugame.h1.pay.wxpay;

import android.os.Message;
import android.util.Log;

import com.alipay.sdk.app.PayTask;
import com.cilugame.h1.pay.alipay.util.OrderInfoUtil2_0;
import com.cilugame.h1.util.Logger;
import com.tencent.mm.sdk.modelpay.PayReq;
import com.tencent.mm.sdk.openapi.IWXAPI;
import com.tencent.mm.sdk.openapi.WXAPIFactory;
import com.unity3d.player.UnityPlayer;

import java.sql.Timestamp;
import java.util.Date;
import java.util.Map;

/**
 * Created by senkay on 12/27/16.
 */
public class WxpayAdapter
{
    private IWXAPI api;

    public void DoPay(
            final String orderSerial,
            final String productId,
            final String productName,
            final String productPrice,
            final String productCount,
            final String serverId,
            final String payNotifyUrl,
            final String payKey,
            final String channelOrderSerial,
            final String orderJson
    )
    {
        Logger.Log("Call AlipayAdapter DoPay");

//        request.appId = "wxd930ea5d5a258f4f";
//
//        request.partnerId = "1900000109";
//
//        request.prepayId= "1101000000140415649af9fc314aa427",;
//
//        request.packageValue = "Sign=WXPay";
//
//        request.nonceStr= "1101000000140429eb40476f8896f4c9";
//
//        request.timeStamp= "1398746574";
//
//        request.sign= "7FFECB600D7157C5AA49810D2D8F28BC2811827B";


        api = WXAPIFactory.createWXAPI(UnityPlayer.currentActivity, "wxb4ba3c02aa476ea1");

        new Thread(new Runnable() {
            @Override
            public void run() {

                PayReq req = new PayReq();
                req.appId = Constants.APP_ID;
                //req.partnerId = json.getString("partnerid");
                req.prepayId = orderSerial;
                //req.nonceStr = json.getString("noncestr");
                req.timeStamp = System.currentTimeMillis()+"";
                //req.packageValue = json.getString("package");
                req.sign = payKey;
                req.extData = "app data"; // optional

                api.sendReq(req);
            }
        }).start();
    }
}