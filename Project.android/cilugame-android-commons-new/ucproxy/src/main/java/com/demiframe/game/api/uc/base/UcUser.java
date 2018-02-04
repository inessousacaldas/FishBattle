package com.demiframe.game.api.uc.base;

public class UcUser
{
  private String sid;
  private String ucid;

  private String uid;

  public String getSid()
  {
    return this.sid;
  }

  public String getUcid()
  {
    return this.ucid;
  }

  public void setSid(String paramString)
  {
    this.sid = paramString;
  }

  public void setUcid(String paramString)
  {
    this.ucid = paramString;
  }

  public void setUid(String uid){
    this.uid = uid;
  }

  public String getUid(){
    return this.uid;
  }
}