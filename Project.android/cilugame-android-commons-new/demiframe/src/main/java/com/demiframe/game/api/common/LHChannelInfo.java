package com.demiframe.game.api.common;

public class LHChannelInfo
{
  private String appIdentifier;
  private String channelId;
  private String channelName;
  private String subChannelId;

  private String wxAppSecret;

  public String getAppIdentifier()
  {
    return this.appIdentifier;
  }

  public String getChannelId()
  {
    return this.channelId;
  }

  public String getChannelName()
  {
    return this.channelName;
  }

  public String getWxAppSecret()
  {
    return this.wxAppSecret;
  }

  public void setAppIdentifier(String identifier)
  {
    this.appIdentifier = identifier;
  }

  public void setChannelId(String channelId)
  {
    this.channelId = channelId;
  }

  public void setChannelName(String channelName)
  {
    this.channelName = channelName;
  }

  public void setWxAppSecret(String paramString)
  {
    this.wxAppSecret = paramString;
  }

  public void setSubChannelId(String subChannelId){
    this.subChannelId = subChannelId;
  }

  public String getSubChannelId(){
    return this.subChannelId;
  }
}