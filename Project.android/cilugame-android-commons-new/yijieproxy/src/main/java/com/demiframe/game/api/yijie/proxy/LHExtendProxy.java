package com.demiframe.game.api.yijie.proxy;

import android.app.Activity;
import android.text.TextUtils;

import com.demiframe.game.api.common.LHRole;
import com.demiframe.game.api.common.LHRoleDataType;
import com.demiframe.game.api.connector.IExtend;
import com.demiframe.game.api.connector.IHandleCallback;
import com.demiframe.game.api.util.IOUtil;
import com.demiframe.game.api.util.LogUtil;
import com.snowfish.cn.ganga.helper.SFOnlineHelper;

import org.json.JSONObject;

public class LHExtendProxy
  implements IExtend
{
  public void antiAddictionQuery(final Activity activity, IHandleCallback handleCallback)
  {
  }

  public void enterBBS(final Activity activity)
  {
  }

  public void enterUserCenter(final Activity activity)
  {
  }

  public void realNameRegister(final Activity activity, IHandleCallback handleCallback)
  {
  }

  public void submitRoleData(final Activity activity, final LHRole lhRole)
  {
    //登录成功
    if(lhRole.getDataType() == LHRoleDataType.loginRole){
      SFOnlineHelper.setRoleData(activity, lhRole.getRoledId(), lhRole.getRoleName(), lhRole.getRoleLevel(),
              lhRole.getZoneId(), lhRole.getZoneName());
    }
    else{
      JSONObject roleJson = new JSONObject();
      String key = "";
      if(lhRole.getDataType() == LHRoleDataType.createRole){
        key = "createrole";
      }
      else if(lhRole.getDataType() == LHRoleDataType.loginRole){
        key = "enterServer";
      }
      else if(lhRole.getDataType() == LHRoleDataType.upgradeRole){
        key = "levelup";
      }
      else{
        //退出不用处理
        return;
      }

      try{
        roleJson.put("roleId", lhRole.getRoledId());
        roleJson.put("roleName", lhRole.getRoleName());
        roleJson.put("roleLevel", lhRole.getRoleLevel());
        roleJson.put("zoneId", lhRole.getZoneId());
        roleJson.put("zoneName", lhRole.getZoneName());

        roleJson.put("balance", lhRole.getBalance("0"));
        roleJson.put("vip", lhRole.getVipLevel("1"));
        roleJson.put("partyName", lhRole.getPartyName("无帮派"));
        roleJson.put("roleCTime", lhRole.getRoleCTime());
        roleJson.put("roleLevelMTime", lhRole.getRoleLevelMTime());
      }catch (Exception e){
        e.printStackTrace();
        return;
      }
      SFOnlineHelper.setData(activity, key, roleJson);
    }
  }

  //提前获取渠道号
  public String getSubChannelId(Activity activity){
    String subChannelId = IOUtil.getMetaDataByName(activity, "com.snowfish.channel");
    if(TextUtils.isEmpty(subChannelId)){
      LogUtil.e("subChannalId is null");
    }
    return subChannelId;
  }

  public void GainGameCoin(Activity activity, String jsonStr){
    LogUtil.d("====GainGameCoin=====");
    LogUtil.d(jsonStr);
  }

  public void ConsumeGameCoin(Activity activity, String jsonStr){
    LogUtil.d("====ConsumeGameCoin=====");
    LogUtil.d(jsonStr);
  }
}