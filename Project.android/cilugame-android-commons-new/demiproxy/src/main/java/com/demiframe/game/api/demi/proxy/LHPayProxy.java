package com.demiframe.game.api.demi.proxy;

import android.annotation.SuppressLint;
import android.app.Activity;
import android.app.AlertDialog.Builder;
import android.content.DialogInterface;
import android.content.DialogInterface.OnClickListener;
import android.os.AsyncTask;
import android.os.Handler;
import android.os.Looper;
import android.os.Message;
import android.text.TextUtils;
import android.util.Log;
import android.widget.Toast;

import com.alipay.sdk.app.PayTask;
import com.demiframe.game.api.common.LHPayInfo;
import com.demiframe.game.api.common.LHResult;
import com.demiframe.game.api.common.LHStatusCode;
import com.demiframe.game.api.connector.IHandleCallback;
import com.demiframe.game.api.connector.IPay;
import com.demiframe.game.api.demi.pay.PayData;
import com.demiframe.game.api.demi.pay.alipay.AlipayAdapter;
import com.demiframe.game.api.demi.pay.alipay.PayResult;
import com.demiframe.game.api.demi.pay.wxpay.WxpayAdapter;
import com.demiframe.game.api.demi.demiutil.Logger;
import com.demiframe.game.api.demi.demiutil.Util;
import com.demiframe.game.api.util.SDKTools;

import org.json.JSONObject;

import java.util.HashMap;
import java.util.Map;

public class LHPayProxy
  implements IPay
{
  private static AlipayAdapter mAlipayAdapter;
  private static WxpayAdapter mWxpayAdapter;
  public static Activity mActivity;

  public static boolean isPaying;

  public static IHandleCallback mPayCallback;
  //private static String SDK_URL = "https://dev.sdk.cilugame.com/v1/sdkc/pay/orderid.json";

  class MyHandler extends Handler {
    public MyHandler(Looper L){
      super(L);
    }

    @Override
    public void handleMessage(Message msg) {
      LHPayProxy.isPaying = false;
      switch (msg.what) {
        case 1:{
          final PayData payData = (PayData)msg.obj;
          OnCommitPay(mActivity, payData.getPrePayBuff(), payData.getPayInfo(), payData.getPayWay());
          break;
        }
        default:
          break;
      }
    };
  }

  public void onPay(final Activity activity, final LHPayInfo lhPayInfo)
  {
    Logger.Log("Call DoPay");

    final CharSequence[] items = {"支付宝", "微信支付"};
    new Builder(activity)
            .setTitle("选择支付方式")
            .setItems(items, new OnClickListener() {
              public void onClick(DialogInterface dialog, int item) {
                String payWay = item==0?"alipay":"wechat";
                DoPay(activity, lhPayInfo, payWay);
              }
            })
            .show();

  }

  public void DoPay(
          final Activity activity,
          final LHPayInfo lhPayInfo,
          final String payWay
  )
  {
    Logger.Log("Call DoPay");
    if(mAlipayAdapter == null){
      mAlipayAdapter = new AlipayAdapter();
    }
    if(mWxpayAdapter == null){
      mWxpayAdapter = new WxpayAdapter();
    }

    //支付中
    if(isPaying){
      Toast.makeText(activity, "支付请求过于频繁,请稍后", Toast.LENGTH_SHORT).show();
      return;
    }

    LHPayProxy.isPaying = true;
    mActivity = activity;
    mPayCallback = lhPayInfo.getPayCallback();

    try{
      final Map<String, String> postmap = new HashMap<String, String>();
      postmap.put("payWay", payWay);//支付渠道(alipay/wechat)
      postmap.put("appId", lhPayInfo.getAppId());//服务器分配的游戏ID
      postmap.put("sid", lhPayInfo.getSid());//SDK帐号登录会话ID
      postmap.put("serverId", lhPayInfo.getServerId());//服务器ID
      postmap.put("playerId", lhPayInfo.getPlayerId());//游戏角色标识
      postmap.put("deliverUrl", lhPayInfo.getPayNotifyUrl());//游戏回调url(通知发货)
      postmap.put("customInfo", lhPayInfo.getPayCustomInfo());//游戏回调信息(透传)
      postmap.put("payAmount", lhPayInfo.getProductPrice(true));//支付金额(分)
      postmap.put("appOrderId", lhPayInfo.getOrderSerial());

      final String sdkUrl = SDKTools.GetSdkProperty(activity, "DEMI_ORDERURL");

      final MyHandler handler = new MyHandler(activity.getMainLooper());
      //responses: {"code":0, "msg": "", "item": {"orderId":"订单号", "signData":"encode后的字符串"}}
      Runnable prePayRunnable = new Runnable() {
        @Override
        public void run() {
          try {
            byte[] buf = Util.httpPost(sdkUrl, postmap);
            Message msg = new Message();
            msg.what = 1;

            PayData payData = new PayData();
            payData.setPayInfo(lhPayInfo);
            payData.setPayWay(payWay);
            payData.setPrePayBuff(buf);
            msg.obj = payData;
            handler.sendMessage(msg);
          }catch (Exception e){
            LHPayProxy.isPaying = false;
            e.printStackTrace();
          }
        }
      };

      Thread payThread = new Thread(prePayRunnable);
      payThread.start();

    }catch(Exception e){
      LHPayProxy.isPaying = false;
      Logger.Error("订单请求失败", e);
      Toast.makeText(activity, "订单请求失败 " + e.getMessage(), Toast.LENGTH_SHORT).show();
    }
  }

  private void OnCommitPay(
                          final Activity activity,
                          byte[] buf,
                          final LHPayInfo lhPayInfo,
                          final String payWay){

    if (buf != null && buf.length > 0) {
      String content = new String(buf);
      Logger.Log("get server pay params:"+content);
      try{
        JSONObject contentJson = new JSONObject(content);
        if(null != contentJson){
          int code = contentJson.getInt("code");
          if (code == 0) {
            //成功
            Logger.Log("订单请求成功");
            Toast.makeText(activity, "订单请求成功", Toast.LENGTH_SHORT).show();

            String itemJsonStr = contentJson.getString("item");
            String productName = lhPayInfo.getGainGold() + lhPayInfo.getProductName();

            if (payWay.equals("alipay"))
            {
              mAlipayAdapter.DoPay(activity, itemJsonStr);
            }
            else
            {
              mWxpayAdapter.DoPay(activity, itemJsonStr);
            }
          }
          else
          {
            LHPayProxy.isPaying = false;
            //错误
            Logger.Log("订单请求失败 " + contentJson.getString("msg"));
            Toast.makeText(activity, "订单请求失败 " + contentJson.getString(("msg")), Toast.LENGTH_SHORT).show();
          }
        }else{
          LHPayProxy.isPaying = false;
          Logger.Log("订单请求失败 json=null");
          Toast.makeText(activity, "订单请求失败 json=null", Toast.LENGTH_SHORT).show();
        }
      }catch (Exception e){
        e.printStackTrace();
        LHPayProxy.isPaying = false;
        return;
      }
    }else{
      LHPayProxy.isPaying = false;
      Logger.Log("订单请求失败 网络失败");
      Toast.makeText(activity, "订单请求失败 网络失败", Toast.LENGTH_SHORT).show();
    }
  }
}