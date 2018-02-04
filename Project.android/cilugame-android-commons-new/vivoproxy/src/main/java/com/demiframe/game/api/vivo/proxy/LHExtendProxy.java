package com.demiframe.game.api.vivo.proxy;

import android.app.Activity;
import android.widget.Toast;

import com.demiframe.game.api.common.LHRole;
import com.demiframe.game.api.connector.IExtend;
import com.demiframe.game.api.connector.IHandleCallback;
import com.demiframe.game.api.vivo.base.User;
import com.vivo.unionsdk.open.VivoRoleInfo;
import com.vivo.unionsdk.open.VivoUnionSDK;

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

    public void submitRoleData(final Activity activity, LHRole lhRole)
    {
        User.getInstances().setLastRoleData(lhRole);
        VivoUnionSDK.reportRoleInfo(new VivoRoleInfo(lhRole.getRoledId(), lhRole.getRoleLevel(),
                lhRole.getRoleName(), lhRole.getZoneId(), lhRole.getZoneName()));
    }

    //提前获取渠道号
    public String getSubChannelId(Activity activity){
        return "vivo";
    }

    public void GainGameCoin(Activity activity, String jsonStr){

    }

    public void ConsumeGameCoin(Activity activity, String jsonStr){

    }
}