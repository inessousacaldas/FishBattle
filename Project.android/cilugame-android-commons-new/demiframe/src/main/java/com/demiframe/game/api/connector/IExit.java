package com.demiframe.game.api.connector;

import android.app.Activity;

//Unity退出登录时，会调用此接口
public abstract interface IExit
{
  public abstract void onExit(Activity activity, IExitCallback iExitCallback);
}