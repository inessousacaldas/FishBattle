package com.demiframe.game.api.connector;

public abstract interface IExitSdk
{
  //渠道未提供退出界面，游戏内点击确认退出时的回调
  public abstract void onExitSdk();

}