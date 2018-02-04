package com.demiframe.game.api.connector;

import android.app.Activity;
import android.content.Intent;
import android.content.res.Configuration;

public abstract interface IActivity
{
  public abstract void onActivityResult(Activity activity, int paramInt1, int paramInt2, Intent intent);

  public abstract void onCreate(Activity activity, IInitCallback listener, Object obj);

  //部分sdk，如UC。闪屏会和初始化同时进行，需要等游戏闪屏结束后进行
  public abstract void afterOnCreate(Activity activity, IInitCallback listener, Object obj);

  //延后初始化
  public abstract Boolean needAfterCreate();

  public abstract void onDestroy(Activity activity);

  public abstract void onNewIntent(Activity activity, Intent intent);

  public abstract void onPause(Activity activity);

  public abstract void onRestart(Activity activity);

  public abstract void onResume(Activity activity);

  public abstract void onStop(Activity activity);

  public abstract void onStart(Activity activity);

  public abstract void onConfigurationChanged(Activity activity, Configuration newConfig);
}