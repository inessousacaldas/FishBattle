package com.demiframe.game.api.shoumeng.proxy;

import android.app.Activity;

import com.demiframe.game.api.common.LHRole;
import com.demiframe.game.api.common.LHRoleDataType;
import com.demiframe.game.api.connector.IExtend;
import com.demiframe.game.api.connector.IHandleCallback;
import com.demiframe.game.api.util.LogUtil;

import org.json.JSONObject;

import mobi.shoumeng.integrate.game.RoleInfo;
import mobi.shoumeng.integrate.game.method.GameMethod;
import mobi.shoumeng.integrate.game.Constants;
import mobi.shoumeng.integrate.game.GameCoinInfo;

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

  public void submitRoleData(final Activity activity, LHRole lhRole)
  {
    RoleInfo roleInfo = new RoleInfo();
    roleInfo.setRoleId(lhRole.getRoledId());
    roleInfo.setRoleName(lhRole.getRoleName());
    roleInfo.setAreaId(lhRole.getZoneId());
    roleInfo.setArea(lhRole.getZoneName());

    roleInfo.setSociaty(lhRole.getPartyName("无公会"));
    roleInfo.setVip(lhRole.getVipLevel("1"));

    roleInfo.setLevel(lhRole.getRoleLevel());

    if(lhRole.getRoleCTime() != null && !lhRole.getRoleCTime().isEmpty()){
      roleInfo.setRoleCreateTime(Long.parseLong(lhRole.getRoleCTime()));
    }
    if(lhRole.getRoleLevelMTime() != null && !lhRole.getRoleLevelMTime().isEmpty()){
      roleInfo.setLevelChangeTime(Long.parseLong(lhRole.getRoleLevelMTime()));
    }


    String roleType = "";
    if(lhRole.getDataType() == LHRoleDataType.createRole){
      roleType = Constants.ROLE_TYPE_CREATE_ROLE;
    }
    else if(lhRole.getDataType() == LHRoleDataType.loginRole){
      roleType = Constants.ROLE_TYPE_ENTER_SERVER;
    }
    else if(lhRole.getDataType() == LHRoleDataType.upgradeRole){
      roleType = Constants.ROLE_TYPE_LEVEL_UP;
    }
    else{
      //退出的情况手盟sdk已经帮处理
      return;
    }
    roleInfo.setRoleType(roleType);

    GameMethod.getInstance().saveRoleInfo(activity, roleInfo);
  }

  //提前获取渠道号
  public String getSubChannelId(Activity activity){
    return GameMethod.getInstance().getChannelLabel();
  }

  public void GainGameCoin(Activity activity, String jsonStr){
    LogUtil.d("====GainGameCoin=====");
    LogUtil.d(jsonStr);
    GameCoinInfo gameCoinInfo = new GameCoinInfo();
    try{
      JSONObject jsonObject = new JSONObject(jsonStr);
      gameCoinInfo.setCoinNumber(Integer.parseInt(jsonObject.getString("amount")));//每次使用消耗的游戏币数量
      gameCoinInfo.setRoleId(jsonObject.getString("playerId"));//角色ID
      gameCoinInfo.setRoleName(jsonObject.getString("playerName"));//角色名称
      gameCoinInfo.setRoleLevel(jsonObject.getString("playerLevel"));//角色等级，纯数字
      gameCoinInfo.setServerId(jsonObject.getString("serverId"));//角色所在区服ID，传数字
      gameCoinInfo.setCoinTime(Long.parseLong(jsonObject.getString("changeTime")));// 使用消耗游戏币的时间，单位：秒，数据类型：long
    }catch (Exception e){
      e.printStackTrace();
      return;
    }

    GameMethod.getInstance().gainGameCoin(activity, gameCoinInfo);
  }

  public void ConsumeGameCoin(Activity activity, String jsonStr){
    LogUtil.d("====ConsumeGameCoin=====");
    LogUtil.d(jsonStr);

    GameCoinInfo gameCoinInfo = new GameCoinInfo();
    try{
      JSONObject jsonObject = new JSONObject(jsonStr);
      gameCoinInfo.setCoinNumber(Integer.parseInt(jsonObject.getString("amount")));//每次使用消耗的游戏币数量
      gameCoinInfo.setRoleId(jsonObject.getString("playerId"));//角色ID
      gameCoinInfo.setRoleName(jsonObject.getString("playerName"));//角色名称
      gameCoinInfo.setRoleLevel(jsonObject.getString("playerLevel"));//角色等级，纯数字
      gameCoinInfo.setServerId(jsonObject.getString("serverId"));//角色所在区服ID，传数字
      gameCoinInfo.setCoinTime(Long.parseLong(jsonObject.getString("changeTime")));// 使用消耗游戏币的时间，单位：秒，数据类型：long
    }catch (Exception e){
      e.printStackTrace();
      return;
    }

    GameMethod.getInstance().consumeGameCoin(activity, gameCoinInfo);
  }
}