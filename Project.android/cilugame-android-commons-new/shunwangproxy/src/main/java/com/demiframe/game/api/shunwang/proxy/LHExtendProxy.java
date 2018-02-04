package com.demiframe.game.api.shunwang.proxy;

import android.app.Activity;
import android.widget.Toast;

import com.demiframe.game.api.common.LHRole;
import com.demiframe.game.api.connector.IExtend;
import com.demiframe.game.api.connector.IHandleCallback;

public class LHExtendProxy
        implements IExtend {
    public void antiAddictionQuery(final Activity activity, IHandleCallback handleCallback) {
    }

    public void enterBBS(final Activity activity) {
    }

    public void enterUserCenter(final Activity activity) {
    }

    public void realNameRegister(final Activity activity, IHandleCallback handleCallback) {
    }

    public void submitRoleData(final Activity activity, LHRole lhRole) {

    }

    //提前获取渠道号
    public String getSubChannelId(Activity activity){
        return "shunwang";
    }

    public void GainGameCoin(Activity activity, String jsonStr){

    }

    public void ConsumeGameCoin(Activity activity, String jsonStr){

    }
}