package com.demiframe.game.api.connector;

public abstract interface IExitCallback
{
  //渠道执行退出的回调接口。（有提供退出界面）
  public abstract void onExit(boolean paramBoolean);

  //渠道执行退出的回调接口。（没有提供退出界面）
  public abstract void onNoExiterProvide(IExitSdk paramIExitSdk);
}