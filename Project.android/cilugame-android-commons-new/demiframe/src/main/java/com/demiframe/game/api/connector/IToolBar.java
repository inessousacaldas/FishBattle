package com.demiframe.game.api.connector;

import android.app.Activity;

public abstract interface IToolBar
{
  //隐藏渠道sdk的悬浮bar
  public abstract void hideFloatToolBar(Activity activity);

  //显示渠道sdk的悬浮bar
  public abstract void showFloatToolBar(Activity activity);
}