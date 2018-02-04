package com.demiframe.game.api.qihoo.proxy;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;

import com.demiframe.game.api.qihoo.base.User;
import com.demiframe.game.api.common.LHRole;
import com.demiframe.game.api.common.LHRoleDataType;
import com.demiframe.game.api.common.LHStaticValue;
import com.demiframe.game.api.connector.IExtend;
import com.demiframe.game.api.connector.IHandleCallback;
import com.qihoo.gamecenter.sdk.activity.ContainerActivity;
import com.qihoo.gamecenter.sdk.matrix.Matrix;
import com.qihoo.gamecenter.sdk.protocols.ProtocolConfigs;
import com.qihoo.gamecenter.sdk.protocols.ProtocolKeys;

import java.util.HashMap;

// TODO: 2017/3/23 客服接口？？
public class LHExtendProxy
        implements IExtend {

//    private static LHRole lastRoleData;
//    public static void setLastRoleData(LHRole lhRole){
//        lastRoleData = lhRole;
//    }

    public void antiAddictionQuery(final Activity activity, IHandleCallback handleCallback) {
    }

    public void enterBBS(final Activity activity) {
        Bundle bundle = new Bundle();
        // 界面相关参数，360SDK界面是否以横屏显示。
        bundle.putBoolean(ProtocolKeys.IS_SCREEN_ORIENTATION_LANDSCAPE, LHStaticValue.IsLandScape);

        // 必需参数，使用360SDK的论坛模块。
        bundle.putInt(ProtocolKeys.FUNCTION_CODE, ProtocolConfigs.FUNC_CODE_BBS);
        Intent intent = new Intent(activity, ContainerActivity.class);
        intent.putExtras(bundle);

        Matrix.invokeActivity(activity, intent, null);
    }

    public void enterUserCenter(final Activity activity) {
    }

    public void realNameRegister(final Activity activity, IHandleCallback handleCallback) {
    }

    public void submitRoleData(final Activity activity, LHRole lhRole) {
//        LHExtendProxy.setLastRoleData(lhRole);
        LHExtendProxy.doSubmitRoleData(activity, lhRole, lhRole.getDataType());
    }

//    //渠道要求退出的时候上传角色数据，取上次缓存的角色信息上传
//    public static void CheckExitSubmit(Activity activity){
//        if(lastRoleData != null){
//            LHExtendProxy.doSubmitRoleData(activity, lastRoleData);
//            lastRoleData = null;
//        }
//    }

    public static void doSubmitRoleData(Activity activity, LHRole lhRole, LHRoleDataType dataType){
        User.getInstances().setLastRoleData(lhRole);

        HashMap eventParams=new HashMap();
        eventParams.put("zoneid", Integer.parseInt(lhRole.getZoneId()));//当前角色所在游戏区服id
        eventParams.put("zonename", lhRole.getZoneName());//当前角色所在游戏区服名称
        eventParams.put("roleid", lhRole.getRoledId());//当前角色id
        eventParams.put("rolename", lhRole.getRoleName());//当前角色名称
        if(dataType == LHRoleDataType.createRole){
            eventParams.put("type", "createRole");
        }
        else if(dataType == LHRoleDataType.loginRole){
            eventParams.put("type", "enterServer");
        }
        else if(dataType == LHRoleDataType.upgradeRole){
            eventParams.put("type", "levelUp");
        }
        else if(dataType == LHRoleDataType.exitServer){
            eventParams.put("type", "exitServer");
        }


        eventParams.put("professionid", 0);//职业
        eventParams.put("profession", "无");//职业

        eventParams.put("gender", "无");//性别

        eventParams.put("rolelevel", Integer.parseInt(lhRole.getRoleLevel()));
        eventParams.put("power", 0);//战力
        eventParams.put("vip", 0);//vip等级
        eventParams.put("partyid", 0);//帮派
        eventParams.put("partyname", "无");//帮派名
        eventParams.put("partyroleid", 0);//帮主id
        eventParams.put("partyrolename", "无");//帮主名
        eventParams.put("friendlist", "无");//好友列表

        Matrix.statEventInfo(activity, eventParams);
    }

    public String getSubChannelId(Activity activity){
        return "qihoo";
    }

    public void GainGameCoin(Activity activity, String jsonStr){

    }

    public void ConsumeGameCoin(Activity activity, String jsonStr){

    }
}