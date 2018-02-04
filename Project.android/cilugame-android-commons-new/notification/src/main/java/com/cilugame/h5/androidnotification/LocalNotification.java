package com.cilugame.h5.androidnotification;

import android.app.AlarmManager;
import android.app.Notification;
import android.app.PendingIntent;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.pm.ApplicationInfo;
import android.graphics.BitmapFactory;
import android.graphics.Color;
import android.media.RingtoneManager;
import android.os.Bundle;

public class LocalNotification extends BroadcastReceiver {
    static Debug.NotificationType mType = Debug.NotificationType.LocalNotification;
    static int mRequestCode = 0;

    static public void LocalNotificationLog(boolean pLog) {
        Debug.LocalNotificationDebugLogEnable = pLog;
    }

    public static void SetNotification(String pTitle, String pContent, long pDelaySec, String pCLassName) {
        if (pDelaySec < 0) {
            Debug.Log(mType, "SetNotification", "该通知延迟时间不能小于0秒");
            return;
        }
        Notify(pTitle, pContent, pDelaySec, -1, pCLassName);
    }

    static public void SetRepeatNotification(String pTitle, String pContent, long pDelaySec, long pRepeatSec, String pCLassName) {
        if (pDelaySec < 0) {
            Debug.Log(mType, "SetRepeatNotification", "该重复通知DelayMillis不能小于0秒");
            return;
        }
        if (pRepeatSec <= 0) {
            Debug.Log(mType, "SetRepeatNotification", "该重复通知RepeatMillis不能小于0秒");
            return;
        }
        Notify(pTitle, pContent, pDelaySec, pRepeatSec, pCLassName);
    }

    static void Notify(String pTitle, String pContent, long pDelaySec, long pRepeatSec, String pCLassName) {
        Intent tIntent = new Intent(MyUnity.getUnityActivity(), LocalNotification.class);
        tIntent.putExtra("title", pTitle);
        tIntent.putExtra("content", pContent);
        tIntent.putExtra("classname", pCLassName);
        tIntent.putExtra("request_code", mRequestCode);
        PendingIntent tPIntent = PendingIntent.getBroadcast(MyUnity.getUnityActivity(), mRequestCode, tIntent, 0);
        mRequestCode++;

        if (pRepeatSec <= 0) {
            MyUnity.getAlarmManager(null).set(AlarmManager.RTC_WAKEUP, System.currentTimeMillis() + (pDelaySec * 1000), tPIntent);
            Debug.Log(mType, "SetNotification", "设置通知成功--title:" + pTitle + " content:" + pContent + " mRequestCode=" + mRequestCode);
        } else {
            MyUnity.getAlarmManager(null).setRepeating(AlarmManager.RTC_WAKEUP, System.currentTimeMillis() + (pDelaySec * 1000), pRepeatSec * 1000, tPIntent);
            Debug.Log(mType, "SetRepeatNotification", "设置重复通知成功--title:" + pTitle + " content:" + pContent + " mRequestCode=" + mRequestCode);
        }
    }

    static public void CancelAll() {
        MyUnity.getNotificationManager(null).cancelAll();
        mRequestCode = 0;
        Debug.Log(mType, "CancelAll", "已清除所有通知" + " mRequestCode=" + mRequestCode);
    }

    @Override
    public void onReceive(Context pContext, Intent pIntent) {
        Class<?> tUnityActivity = null;
        try {
            tUnityActivity = Class.forName(pIntent.getStringExtra("classname"));
        } catch (Exception ex) {
            ex.printStackTrace();
            return;
        }

        Bundle tBundle = pIntent.getExtras();
        ApplicationInfo tAppInfo = pContext.getApplicationInfo();
        Intent notificationIntent = new Intent(pContext, tUnityActivity);
        notificationIntent.addCategory(Intent.CATEGORY_LAUNCHER);
        PendingIntent tContentIntent = PendingIntent.getActivity(pContext, 0, notificationIntent, 0);
        Notification.Builder tBuilder = new Notification.Builder(pContext);

        String tTitle = (String) tBundle.get("title");
        String tContent = (String) tBundle.get("content");
        int tRequestCode = tBundle.getInt("request_code");

        tBuilder.setContentIntent(tContentIntent)
                .setWhen(System.currentTimeMillis())
                .setAutoCancel(true)
                .setContentTitle(tTitle)
                .setTicker(tContent)
                .setContentText(tContent)
                .setSmallIcon(tAppInfo.icon)
                .setLargeIcon(BitmapFactory.decodeResource(pContext.getResources(), tAppInfo.icon))
                .setSound(RingtoneManager.getDefaultUri(2))
                .setVibrate(new long[]{1000L, 1000L})
                .setLights(Color.GREEN, 3000, 3000);

        if (android.os.Build.VERSION.SDK_INT >= android.os.Build.VERSION_CODES.JELLY_BEAN) {
            Notification notification = tBuilder.build();
            MyUnity.getNotificationManager(pContext).notify(tRequestCode, notification);
            Debug.Log(mType, "onReceive", "title:" + tTitle + " content:" + tContent + " mRequestCode=" + mRequestCode);
        }
    }
}
