package com.demiframe.game.api.common;

public class LHRole
{
  private LHRoleDataType dataType;
  private String os;
  private String roleCTime;
  private String roleLevel;
  private String roleLevelMTime;
  private String roleName;
  private String roledId;
  private String zoneId;
  private String zoneName;

  public LHRoleDataType getDataType()
  {
    return this.dataType;
  }

  public String getOs()
  {
    return this.os;
  }

  public String getRoleCTime()
  {
    return this.roleCTime;
  }

  public String getRoleLevel()
  {
    return this.roleLevel;
  }

  public String getRoleLevelMTime()
  {
    return this.roleLevelMTime;
  }

  public String getRoleName()
  {
    return this.roleName;
  }

  public String getRoledId()
  {
    return this.roledId;
  }

  public String getZoneId()
  {
    return this.zoneId;
  }

  public String getZoneName()
  {
    return this.zoneName;
  }

  public void setDataType(LHRoleDataType paramLHRoleDataType)
  {
    this.dataType = paramLHRoleDataType;
  }

  public void setOs(String paramString)
  {
    this.os = paramString;
  }

  public void setRoleCTime(String paramLong)
  {
    this.roleCTime = paramLong;
  }

  public void setRoleLevel(String paramString)
  {
    this.roleLevel = paramString;
  }

  public void setRoleLevelMTime(String paramLong)
  {
    this.roleLevelMTime = paramLong;
  }

  public void setRoleName(String paramString)
  {
    this.roleName = paramString;
  }

  public void setRoledId(String paramString)
  {
    this.roledId = paramString;
  }

  public void setZoneId(String paramString)
  {
    this.zoneId = paramString;
  }

  public void setZoneName(String paramString)
  {
    this.zoneName = paramString;
  }

  //游戏币余额
  private String balance;
  //vip等级
  private String vipLevel;
  //帮派名
  private String partyName;

  public void setBalance(String balance){
    this.balance = balance;
  }

  public void setVipLevel(String vipLevel){
    this.vipLevel = vipLevel;
  }

  public void setPartyName(String partyName){
    this.partyName = partyName;
  }

  public String getBalance(String sDefault){
    if(this.balance == null || this.balance == ""){
      return sDefault;
    }
    return this.balance;
  }

  public String getVipLevel(String sDefault){
    if(this.vipLevel == null || this.vipLevel == ""){
      return sDefault;
    }

    return this.vipLevel;
  }

  public String getPartyName(String sDefault){
    if(this.partyName == null || this.partyName == ""){
      return sDefault;
    }

    return this.partyName;
  }
}