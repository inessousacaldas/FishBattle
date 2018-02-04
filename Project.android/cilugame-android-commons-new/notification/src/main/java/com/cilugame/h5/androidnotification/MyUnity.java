package com.cilugame.h5.androidnotification;

import android.app.Activity;
import android.app.AlarmManager;
import android.app.NotificationManager;
import android.content.Context;

import com.unity3d.player.UnityPlayer;

public class MyUnity {

    static Activity getUnityActivity() {
        return UnityPlayer.currentActivity;
    }

    static Context getContext() {
        return getUnityActivity().getApplicationContext();
    }

    static AlarmManager getAlarmManager(Context pContent) {
        if (pContent != null) {
            return (AlarmManager) pContent.getSystemService(Context.ALARM_SERVICE);
        }
        return (AlarmManager) getUnityActivity().getSystemService(Context.ALARM_SERVICE);
    }

    static NotificationManager getNotificationManager(Context pContent) {
        if (pContent != null) {
            return (NotificationManager) pContent.getSystemService(Context.NOTIFICATION_SERVICE);
        }
        return (NotificationManager) getUnityActivity().getSystemService(Context.NOTIFICATION_SERVICE);
    }
}
