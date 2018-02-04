package com.demiframe.game.api.connector;

public abstract interface ICheckSupport
{
  //是否支持防沉迷，暂时无用
  public abstract boolean isAntiAddictionQuery();

  //是否支持论坛
  public abstract boolean isSupportBBS();

  //是否支持登出
  public abstract boolean isSupportLogout();

  //是否支持客服，暂时无用
  public abstract boolean isSupportOfficialPlacard();

  //是否支持显示工具栏,游戏也暂时没调用
  public abstract boolean isSupportShowOrHideToolbar();

  //是否支持用户中心
  public abstract boolean isSupportUserCenter();
}