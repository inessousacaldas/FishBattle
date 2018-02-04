package com.demiframe.game.api.util;

import android.app.Activity;
import android.content.Context;

import org.json.JSONObject;

import java.io.InputStream;

/**
 * Created by Xianjian on 2017/2/21.
 */

public class SDKTools {
    private static String PROP_FILE = "demichannel_config";
    private static JSONObject propJson;
    public static JSONObject ReadSdkProperty(Context context){
        if(propJson != null){
            return propJson;
        }

        JSONObject json = null;
        try {
            InputStream inputStream = context.getResources().getAssets().open(PROP_FILE);
            int size = inputStream.available();
            byte[] buffer = new byte[size];
            inputStream.read(buffer);
            inputStream.close();

            String txt = new String(buffer);
            json = new JSONObject(EncryptUtil.decrypt(txt));
        }catch (Exception e){
            e.printStackTrace();
        }
        propJson = json;
        LogUtil.d(json.toString());
        return json;
    }

    public static String GetSdkProperty(Context context, String propName){
        JSONObject json = ReadSdkProperty(context);
        if(json == null){
            LogUtil.e("GetSdkProperty json is null");
            return "";
        }
        try{
            String value = json.getString(propName);
            LogUtil.d("GetSdkProperty "+ propName + "=" + value);
            return value;
        }catch (Exception e){
            LogUtil.e("GetSdkProperty error key="+propName);
            e.printStackTrace();
            return "";
        }
    }

    public static boolean GetSdkBoolProperty(Context context, String propName, boolean bDefault){
            String value = GetSdkProperty(context, propName);
            if(value.equals("")){
                return bDefault;
            }

            if(value.equals("true")) {
                return true;
            }else if(value.equals("false")){
                return false;
            }
            LogUtil.d("propName key value is not true or false");
            return bDefault;
    }
}
