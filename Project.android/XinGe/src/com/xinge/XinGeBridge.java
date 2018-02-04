package com.xinge;

import org.json.JSONObject;

import android.content.Context;
import com.tencent.android.tpush.XGIOperateCallback;
import com.tencent.android.tpush.XGPushConfig;
import com.tencent.android.tpush.XGPushManager;
import com.unity3d.player.UnityPlayer;

public class XinGeBridge
{
    static Context context;

    private static void Log(String paramString)
    {
        android.util.Log.d("XinGeBridge", paramString);
    }

    private static void SendToUnity(String type, int code, String data)
    {
        try {
            JSONObject jobj = new JSONObject();
            jobj.put("type", type);
            jobj.put("code", code);
            jobj.put("data", data);
            
            SendToUnity("OnSdkCallback", jobj.toString());
        } catch (Throwable e) {
            e.printStackTrace();
        }
    }

    private static void SendToUnity(String function, String params)
    {
        UnityPlayer.UnitySendMessage("_SdkMessageHandler",function, params);
    }

    public static void Setup(String appId, String appKey)
    {
        context =  UnityPlayer.currentActivity.getApplicationContext();
        XGPushConfig.setAccessId(context, Long.parseLong(appId));
        XGPushConfig.setAccessKey(context, appKey);
    }

    public static void enableDebug(boolean enable)
    {
        XGPushConfig.enableDebug(context, enable);
    }

    public static void RegisterPush()
    {       
        Log("信鸽推送 注册");

        XGPushManager.registerPush(context,
            new XGIOperateCallback() {
                @Override
                public void onSuccess(Object data, int flag) {
                    Log("注册成功，设备token为：" + data);
                    SendToUnity("OnXGRegisterResult", "0");
                }

                @Override
                public void onFail(Object data, int errCode, String msg) {
                    Log("注册失败，错误码：" + errCode + ",错误信息：" + msg);
                    SendToUnity("OnXGRegisterResult", "1");
                }
            });
    }

    /**
     * 绑定账号注册
     * 绑定账号注册指的是，在绑定设备注册的基础上，使用指定的账号（一个账号可能在多个设备登陆）注册APP，
     * 这样可以通过后台向指定的账号发送推送消息，有2个版本的API接口。
     * 注意：这里的帐号可以是邮箱、QQ号、手机号、用户名等任意类别的业务帐号。
     * @param account
     */
    public static void RegisterPushWithAccount(final String account){
         Log("信鸽推送带别名 注册,别名:" + account);

         XGPushManager.registerPush(context, account,
            new XGIOperateCallback() {
                @Override
                public void onSuccess(Object data, int flag) {
                    Log("带别名注册成功,别名是:"+ account+"设备token为：" + data);
                    SendToUnity("OnXGRegisterWithAccountResult", "0");
                }

                @Override
                public void onFail(Object data, int errCode, String msg) {
                    Log("带别名注册失败，别名是:" +account+"错误码：" + errCode + ",错误信息：" + msg);
                    SendToUnity("OnXGRegisterWithAccountResult", "1");
                }
            });
    }


    /**
     * 设置标签，拥有此标签的会受到针对这个标签的通知
     * @param tagName
     */
    public static void SetTag(String tagName)
    {
        XGPushManager.setTag(context, tagName);
        Log("设置标签:" + tagName);
    }


    /**
     * 删除标签
     * @param tagName
     */
    public static void DeleteTag(String tagName)
    {
        XGPushManager.deleteTag(context, tagName);
        Log("删除标签:" + tagName);
    }
}