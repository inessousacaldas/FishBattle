package com.cilugame.h5.androidnotification;

import android.util.Log;

import org.json.JSONObject;

import com.unity3d.player.UnityPlayer;

public class Debug {

    static public enum NotificationType {
        LocalNotification,
        RemoteNotification,
    }

    static public boolean LocalNotificationDebugLogEnable = false;
    static public boolean RemoteNotificationDebugLogEnable = false;

    static public void Log(NotificationType pType, String pMethodName, String pMessage) {
        if (RemoteNotificationDebugLogEnable && pType == NotificationType.RemoteNotification) {
            Log.d(pType.toString(), "method:" + pMethodName + " message:" + pMessage);
        } else if (LocalNotificationDebugLogEnable && pType == NotificationType.LocalNotification) {
            Log.d(pType.toString(), "method:" + pMethodName + " message:" + pMessage);
        }
        SendMessageToUnity(pType, pMethodName, pMessage);
    }

    static void SendMessageToUnity(NotificationType pType, String pMethodName, String pMessage) {
        JSONObject tJson = new JSONObject();
        try {
            tJson.put("type", pType);
            tJson.put("method", pMethodName == null ? "" : pMethodName);
            tJson.put("message", pMessage == null ? "" : pMessage);

        } catch (Throwable e) {
            e.printStackTrace();
        }
        if (MyUnity.getUnityActivity() != null)
            UnityPlayer.UnitySendMessage("_SdkMessageHandler", "OnNotificationCallBack", tJson.toString());
    }

}
