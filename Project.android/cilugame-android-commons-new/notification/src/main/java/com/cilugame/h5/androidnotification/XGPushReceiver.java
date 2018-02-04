package com.cilugame.h5.androidnotification;

import android.content.Context;

import com.tencent.android.tpush.*;

import org.json.JSONObject;

public class XGPushReceiver extends XGPushBaseReceiver {
    Debug.NotificationType mType = Debug.NotificationType.RemoteNotification;

    @Override
    public void onRegisterResult(Context context, int errCode, XGPushRegisterResult xgPushRegisterResult) {

        JSONObject pJson = new JSONObject();
        try {
            pJson.put("errCode", errCode);
            pJson.put("token", xgPushRegisterResult.getToken());
        } catch (Throwable e) {
            e.printStackTrace();
        }
        Debug.Log(mType, " ", pJson.toString());
    }

    @Override
    public void onUnregisterResult(Context context, int errCode) {
        Debug.Log(mType, "onUnregisterResult", String.valueOf(errCode));
    }

    @Override
    public void onSetTagResult(Context context, int errCode, String result) {
        JSONObject pJson = new JSONObject();
        try {
            pJson.put("errCode", errCode);
            pJson.put("result", result);
        } catch (Throwable e) {
            e.printStackTrace();
        }
        Debug.Log(mType, "onSetTagResult", pJson.toString());
    }

    @Override
    public void onDeleteTagResult(Context context, int errCode, String result) {
        JSONObject pJson = new JSONObject();
        try {
            pJson.put("errCode", errCode);
            pJson.put("result", result);
        } catch (Throwable e) {
            e.printStackTrace();
        }
        Debug.Log(mType, "onDeleteTagResult", pJson.toString());
    }

    @Override
    public void onTextMessage(Context context, XGPushTextMessage xgPushTextMessage) {
        JSONObject pJson = new JSONObject();
        try {
            pJson.put("content", xgPushTextMessage.getContent());
        } catch (Throwable e) {
            e.printStackTrace();
        }
        Debug.Log(mType, "onTextMessage", pJson.toString());
    }

    @Override
    public void onNotifactionClickedResult(Context context, XGPushClickedResult xgPushClickedResult) {
        JSONObject pJson = new JSONObject();
        try {
            pJson.put("title", xgPushClickedResult.getTitle());
            pJson.put("content", xgPushClickedResult.getContent());
        } catch (Throwable e) {
            e.printStackTrace();
        }
        Debug.Log(mType, "onNotifactionClickedResult", pJson.toString());
    }

    @Override
    public void onNotifactionShowedResult(Context context, XGPushShowedResult xgPushShowedResult) {
        JSONObject pJson = new JSONObject();
        try {
            pJson.put("title", xgPushShowedResult.getTitle());
            pJson.put("content", xgPushShowedResult.getContent());
        } catch (Throwable e) {
            e.printStackTrace();
        }
        Debug.Log(mType, "onNotifactionShowedResult", pJson.toString());
    }
}
