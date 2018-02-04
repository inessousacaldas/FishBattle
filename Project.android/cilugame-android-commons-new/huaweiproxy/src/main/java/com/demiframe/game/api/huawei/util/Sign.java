package com.demiframe.game.api.huawei.util;

import android.app.Activity;
import android.content.Context;

import com.demiframe.game.api.util.LogUtil;
import com.demiframe.game.api.util.SDKTools;

/**
 * Created by xianjian on 2017/3/27.
 */

public class Sign {
    private static String buoSecret = "";
    public static void Init(Activity activity){
        buoSecret = SDKTools.GetSdkProperty(activity, "HUAWEI_BUOSECRET");
    }

    /**
     * 生成游戏签名 generate the game sign
     */
    public static String createGameSign(String data) {
        try {
            String result = RSAUtil.sha256WithRsa(data.getBytes("UTF-8"), buoSecret);
            return result;
        } catch (Exception e) {
            e.printStackTrace();
            return null;
        }
    }
}
