package com.cilugame.h1.pay.alipay;

import android.annotation.SuppressLint;
import android.app.AlertDialog;
import android.content.DialogInterface;
import android.os.Handler;
import android.os.Message;
import android.text.TextUtils;
import android.util.Log;
import android.view.View;
import android.widget.Toast;

import com.alipay.sdk.app.PayTask;
import com.cilugame.h1.UnityCallbackWrapper;
import com.cilugame.h1.util.Logger;
import com.unity3d.player.UnityPlayer;

import com.cilugame.h1.pay.alipay.util.OrderInfoUtil2_0;

import org.json.JSONObject;

import java.util.Map;

/**
 * Created by senkay on 12/22/16.
 */
public class AlipayAdapter
{
    /** 支付宝支付业务：入参app_id */
    //public static final String APPID = "2016121704366510";
    public static final String APPID = "2016073000122638";

    private static final int SDK_PAY_FLAG = 1;


    @SuppressLint("HandlerLeak")
    private Handler mHandler = new Handler() {
        @SuppressWarnings("unused")
        public void handleMessage(Message msg) {
            switch (msg.what) {
                case SDK_PAY_FLAG: {
                    @SuppressWarnings("unchecked")
                    PayResult payResult = new PayResult((Map<String, String>) msg.obj);
                    /**
                     对于支付结果，请商户依赖服务端的异步通知结果。同步通知结果，仅作为支付结束的通知。
                     */
                    String resultInfo = payResult.getResult();// 同步返回需要验证的信息
                    String resultStatus = payResult.getResultStatus();
                    // 判断resultStatus 为9000则代表支付成功
                    if (TextUtils.equals(resultStatus, "9000")) {
                        // 该笔订单是否真实支付成功，需要依赖服务端的异步通知。
                        Toast.makeText(UnityPlayer.currentActivity, "支付成功", Toast.LENGTH_SHORT).show();
                        UnityCallbackWrapper.OnPay(0);
                    } else {
                        // 该笔订单真实的支付结果，需要依赖服务端的异步通知。
                        Toast.makeText(UnityPlayer.currentActivity, "支付失败", Toast.LENGTH_SHORT).show();
                        UnityCallbackWrapper.OnPay(1);
                    }
                    break;
                }
                default:
                    break;
            }
        };
    };

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
            final String orderJsonStr
    )
    {
        Logger.Log("Call AlipayAdapter DoPay");

        String orderId = "";
        String signData = "";
        try
        {
            JSONObject orderJson = new JSONObject(orderJsonStr);
            orderId = orderJson.getString("orderId");
            signData = orderJson.getString("signData");
        }catch(Exception e){
            Logger.Error("订单请求失败", e);
            Toast.makeText(UnityPlayer.currentActivity, "订单请求失败 " + e.getMessage(), Toast.LENGTH_SHORT).show();
        }

        Map<String, String> params = OrderInfoUtil2_0.buildOrderParamMap(APPID, productPrice, productName, productName, orderId);
        String orderParam = OrderInfoUtil2_0.buildOrderParam(params);

        Logger.Log("orderParam = "+orderParam);

        final String orderInfo = signData;

        Logger.Log("signData = "+signData);


        Runnable payRunnable = new Runnable() {

            @Override
            public void run() {
                PayTask alipay = new PayTask(UnityPlayer.currentActivity);
                Map<String, String> result = alipay.payV2(orderInfo, true);
                Log.i("msp", result.toString());

                Message msg = new Message();
                msg.what = SDK_PAY_FLAG;
                msg.obj = result;
                mHandler.sendMessage(msg);
            }
        };

        Thread payThread = new Thread(payRunnable);
        payThread.start();
    }
}
