package com.cilugame.h5.androidnotification;

import com.tencent.android.tpush.XGPushConfig;
import com.tencent.android.tpush.XGPushManager;
import com.unity3d.player.UnityPlayerActivity;

public class RemoteNotification extends UnityPlayerActivity {

    static public void RemoteNotificationLog(boolean pLog) {
        Debug.RemoteNotificationDebugLogEnable = pLog;
    }

    static public void XGDebugEnable(boolean pLog) {
        XGPushConfig.enableDebug(MyUnity.getContext(), pLog);
    }

    static public void RegisterPush() {
        XGPushManager.registerPush(MyUnity.getContext());
    }

    static public void UnregisterPush() {
        XGPushManager.registerPush(MyUnity.getContext(), "*");
    }

    static public void RegisterPushWithAccount(String pAccountName) {
        XGPushManager.registerPush(MyUnity.getContext(), pAccountName);
    }

    static public void SetTag(String pTagName) {
        XGPushManager.setTag(MyUnity.getContext(), pTagName);
    }

    static public void DeleteTag(String pTagName) {
        XGPushManager.deleteTag(MyUnity.getContext(), pTagName);
    }
}
