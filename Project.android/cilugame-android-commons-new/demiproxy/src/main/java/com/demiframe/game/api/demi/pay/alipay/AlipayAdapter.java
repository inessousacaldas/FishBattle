package com.demiframe.game.api.demi.pay.alipay;

import android.annotation.SuppressLint;
import android.app.Activity;
import android.os.Handler;
import android.os.Message;
import android.text.TextUtils;
import android.util.Log;
import android.widget.Toast;

import com.alipay.sdk.app.PayTask;
import com.demiframe.game.api.common.LHResult;
import com.demiframe.game.api.common.LHStatusCode;
import com.demiframe.game.api.demi.pay.alipay.util.OrderInfoUtil2_0;
import com.demiframe.game.api.demi.proxy.LHPayProxy;
import com.demiframe.game.api.demi.demiutil.Logger;
import com.ta.utdid2.android.utils.StringUtils;

import org.json.JSONObject;

import java.net.URLDecoder;
import java.net.URLEncoder;
import java.util.ArrayList;
import java.util.List;
import java.util.Map;

/**
 * Created by senkay on 12/22/16.
 */
public class AlipayAdapter
{
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

                        LHResult lhResult = new LHResult();
                        lhResult.setCode(LHStatusCode.LH_PAY_SUCCESS);
                        LHPayProxy.mPayCallback.onFinished(lhResult);

                    } else {
                        // 该笔订单真实的支付结果，需要依赖服务端的异步通知。
                        LHResult lhResult = new LHResult();
                        lhResult.setCode(LHStatusCode.LH_PAY_FAIL);
                        LHPayProxy.mPayCallback.onFinished(lhResult);
                    }
                    break;
                }
                default:
                    break;
            }
        };
    };

    public void DoPay(
            final Activity activity,
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
            Toast.makeText(activity, "订单请求失败 " + e.getMessage(), Toast.LENGTH_SHORT).show();
            return;
        }

        String finalSignData = "";
        try {
            // 仅需对sign 做URL编码
            //signData = URLDecoder.decode(signData, "utf-8");
            String[] paramList = signData.split("&");
            for(int i=0; i<paramList.length; ++i){
                String param = paramList[i];
                if(!param.startsWith("sign=")){
                    finalSignData += URLDecoder.decode(param, "utf-8") + "&";
                }
                else{
                    finalSignData += param + "&";
                }
            }
            finalSignData = finalSignData.substring(0, finalSignData.length()-1);

        } catch (Exception e) {
            e.printStackTrace();
            return;
        }

        final String orderInfo = finalSignData;

        Logger.Log("signData = "+finalSignData);


        Runnable payRunnable = new Runnable() {

            @Override
            public void run() {
                PayTask alipay = new PayTask(activity);
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
