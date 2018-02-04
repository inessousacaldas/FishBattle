package com.cilugame.h1;

import android.app.AlertDialog;
import android.content.DialogInterface;
import android.widget.Toast;

import com.cilugame.h1.pay.alipay.AlipayAdapter;
import com.cilugame.h1.pay.wxpay.WxpayAdapter;
import com.cilugame.h1.util.Logger;
import com.cilugame.h1.util.Util;
import com.unity3d.player.UnityPlayer;

import org.json.JSONObject;

import java.util.HashMap;
import java.util.Map;

public class CLSDKPlugin {


    private static int InitFlag = -1; //-1等待，0成功，1失败
    private static boolean HasSetup = false;

    private static AlipayAdapter mAlipayAdapter;
    private static WxpayAdapter mWxpayAdapter;

    private static String SDK_URL = "https://dev.sdk.cilugame.com/v1/sdkc/pay/orderid.json";

    public static void Setup()
    {
        if (HasSetup)
        {
            return;
        }

        Logger.Log("Call Setup");

        HasSetup = true;
        InitFlag = 0;

        mAlipayAdapter = new AlipayAdapter();
        mWxpayAdapter = new WxpayAdapter();
    }

    public static void Init()
    {
        Logger.Log("Call Init");

        if (InitFlag == 0)
        {
            UnityCallbackWrapper.OnInit(true);
        }
        else
        {
            UnityCallbackWrapper.OnInit(false);
        }
    }

    /*
     * SDK 登录
     */
    public static void Login() {
        Logger.Log("Call Login");
    }

    public static boolean IsSupportLogout() {
        Logger.Log("Call IsSupportLogout");
        
        return false;
    }

    /*
     * SDK 登出
     */
    public static void Logout() {
        Logger.Log("Call logout");
    }

    /*
     * SDK 更新用户ID
     */
    public static void UpdateUserInfo(String uid)
    {
        Logger.Log("Call UpdateUserInfo");
    }

    /*
     * SDK 上传用户数据
     */
    public static void SubmitRoleData(String uid, boolean newRole, String playerId, String name, String level,
            String serverId, String serverName) {
        Logger.Log("Call SubmitRoleData");
    }

    public static String GetChannelId() {
        String currentChannelId = "";

        Logger.Log("Call GetChannelId =" + currentChannelId);
        return currentChannelId;
    }

    public static boolean IsSupportUserCenter() {
        Logger.Log("Call IsSupportUserCenter");

        return false;
    }

    public static void EnterUserCenter() {
        Logger.Log("Call EnterUserCenter");
    }

    public static boolean IsSupportBBS() {
        Logger.Log("Call IsSupportBBS");
        
        return false;
    }

    public static void EnterSdkBBS() {
        Logger.Log("Call EnterSdkBBS");
    }

    public static boolean IsSupportShowOrHideToolbar() {
        Logger.Log("Call IsSupportShowOrHideToolbar");
        
        return false;
    }

    public static void ShowFloatToolBar() {
        Logger.Log("Call ShowFloatToolBar");
    }

    public static void HideFloatToolBar() {
        Logger.Log("Call HideFloatToolBar");
    }


    /*
     * 支付
     */
    public static void DoPay(
            final String payJsonStr
            )
    {
        Logger.Log("Call DoPay");

        final CharSequence[] items = {"支付宝", "微信支付"};
        new AlertDialog.Builder(UnityPlayer.currentActivity)
                .setTitle("选择支付方式")
                .setItems(items, new DialogInterface.OnClickListener() {
                    public void onClick(DialogInterface dialog, int item) {
                        String payWay = item==0?"alipay":"wechat";
                        DoPay(payJsonStr, payWay);
                    }
                })
                .show();
    }


    public static void DoPay(
            final String payJsonStr,
            final String payWay
    )
    {

        Logger.Log("Call DoPay");

        try{
            JSONObject payJson = new JSONObject(payJsonStr);

            final String orderSerial = payJson.getString("orderSerial");
            final String productId = payJson.getString("productId");
            final String productName = payJson.getString("productName");
            final String productPrice = payJson.getString("productPrice");
            final String productCount = payJson.getString("productCount");
            final String serverId = payJson.getString("serverId");
            final String payNotifyUrl = payJson.getString("payNotifyUrl");
            final String payKey = payJson.getString("payKey");
            final String channelOrderSerial = payJson.getString("channelOrderSerial");

            Map<String, String> postmap = new HashMap<String, String>();
            postmap.put("payWay", payWay);//支付渠道(alipay/wechat)
            postmap.put("appId", payJson.getString("appId"));//服务器分配的游戏ID
            postmap.put("sid", payJson.getString("sid"));//SDK帐号登录会话ID
            postmap.put("serverId", serverId);//服务器ID
            postmap.put("playerId", payJson.getString("playerId"));//游戏角色标识
            postmap.put("deliverUrl", payNotifyUrl);//游戏回调url(通知发货)
            postmap.put("customInfo", payKey);//游戏回调信息(透传)
            postmap.put("payAmount", productPrice);//支付金额(元)

            //responses: {"code":0, "msg": "", "item": {"orderId":"订单号", "signData":"encode后的字符串"}}

            byte[] buf = Util.httpPost(SDK_URL, postmap);
            if (buf != null && buf.length > 0) {
                String content = new String(buf);
                Logger.Log("get server pay params:"+content);
                JSONObject contentJson = new JSONObject(content);
                if(null != contentJson){
                    int code = contentJson.getInt("code");
                    if (code == 0) {
                        //成功
                        Logger.Log("订单请求成功");
                        Toast.makeText(UnityPlayer.currentActivity, "订单请求成功", Toast.LENGTH_SHORT).show();

                        String itemJsonStr = contentJson.getString("item");

                        if (payWay.equals("alipay"))
                        {
                            mAlipayAdapter.DoPay(orderSerial,productId,productName,productPrice,productCount,serverId,payNotifyUrl,payKey,channelOrderSerial,itemJsonStr);
                        }
                        else
                        {
                            mWxpayAdapter.DoPay(orderSerial,productId,productName,productPrice,productCount,serverId,payNotifyUrl,payKey,channelOrderSerial,itemJsonStr);
                        }
                    }
                    else
                    {
                        //错误
                        Logger.Log("订单请求失败 " + contentJson.getString("msg"));
                        Toast.makeText(UnityPlayer.currentActivity, "订单请求失败 " + contentJson.getString(("msg")), Toast.LENGTH_SHORT).show();
                    }
                }else{
                    Logger.Log("订单请求失败 json=null");
                    Toast.makeText(UnityPlayer.currentActivity, "订单请求失败 json=null", Toast.LENGTH_SHORT).show();
                }
            }else{
                Logger.Log("订单请求失败 网络失败");
                Toast.makeText(UnityPlayer.currentActivity, "订单请求失败 网络失败", Toast.LENGTH_SHORT).show();
            }
        }catch(Exception e){
            Logger.Error("订单请求失败", e);
            Toast.makeText(UnityPlayer.currentActivity, "订单请求失败 " + e.getMessage(), Toast.LENGTH_SHORT).show();
        }
    }

    /*
     * SDK 退出
     */
    public static void DoExiter() {
        Logger.Log("Call DoExiter");

        UnityCallbackWrapper.OnExit(true);
    }


    public static void Exit()
    {
        Logger.Log("Call Exit");

        //这里收到退出时要主动关闭activy，例如360会受到这个影响导致退出自动重启
        UnityPlayerActivity.instance.finish();
    }

    //注册账号
    public static void Regster(String account, String uid)
    {
    }
}
