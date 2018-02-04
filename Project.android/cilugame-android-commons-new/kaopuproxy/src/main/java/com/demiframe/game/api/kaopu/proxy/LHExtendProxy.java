package com.demiframe.game.api.kaopu.proxy;

import android.app.Activity;
import android.speech.tts.UtteranceProgressListener;
import android.widget.Toast;

import com.demiframe.game.api.common.LHRole;
import com.demiframe.game.api.common.LHRoleDataType;
import com.demiframe.game.api.connector.IExtend;
import com.demiframe.game.api.connector.IHandleCallback;
import com.demiframe.game.api.kaopu.base.User;
import com.kaopu.supersdk.api.KPSuperSDK;
import com.kaopu.supersdk.model.UpLoadData;

public class LHExtendProxy
        implements IExtend {
    public void antiAddictionQuery(final Activity activity, IHandleCallback handleCallback) {
    }

    public void enterBBS(final Activity activity) {
        //不接入
        //KPSuperSDK.openService()
    }

    public void enterUserCenter(final Activity activity) {
    }

    public void realNameRegister(final Activity activity, IHandleCallback handleCallback) {
    }

    public void submitRoleData(final Activity activity, LHRole lhRole) {
        User.getInstances().setLastRoleData(lhRole);

        UpLoadData upLoadData = new UpLoadData();
        upLoadData.setServerID(lhRole.getZoneId());
        upLoadData.setServerName(lhRole.getZoneName());
        upLoadData.setGameRoleBalance(lhRole.getBalance("0"));
        upLoadData.setVipLevel(lhRole.getVipLevel("0"));
        upLoadData.setPartyName(lhRole.getPartyName("无"));
        upLoadData.setRoleId(lhRole.getRoledId());
        upLoadData.setRoleName(lhRole.getRoleName());
        long roleCreateTime = Long.parseLong(lhRole.getRoleCTime()) / 1000L;
        upLoadData.setRoleCTime(roleCreateTime + "");
        //修改时间设置和创建时间一样
        if(lhRole.getRoleLevelMTime() != null && !lhRole.getRoleLevelMTime().isEmpty()){
            long levelMTime = Long.parseLong(lhRole.getRoleLevelMTime()) / 1000L;
            upLoadData.setRoleLevelMTime(levelMTime + "");
        }else {
            //不能为空，设置为和创建人物时间一样
            upLoadData.setRoleLevelMTime(roleCreateTime + "");
        }

        //传空
        upLoadData.setGuildId("");
        upLoadData.setGuildName("");
        upLoadData.setGuildLevel("");
        upLoadData.setGuildLeader("");
        upLoadData.setPower(0);

        //部分类型不上传
        int type = 6;
        if(lhRole.getDataType() == LHRoleDataType.createRole){
            type = 1;
        }
        else if(lhRole.getDataType() == LHRoleDataType.loginRole){
            type = 6;
        }
        else if(lhRole.getDataType() == LHRoleDataType.upgradeRole){
            type = 2;
        }

        KPSuperSDK.upLoadUserGameData(activity, upLoadData, type);
    }

    //提前获取渠道号
    public String getSubChannelId(Activity activity){
        return "kaopu";
    }

    public void GainGameCoin(Activity activity, String jsonStr){

    }

    public void ConsumeGameCoin(Activity activity, String jsonStr){

    }
}