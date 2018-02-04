package com.demiframe.game.api.connector;

import android.app.Activity;
import com.demiframe.game.api.common.LHRole;

public abstract interface IExtend
{
  //进入论坛接口，对应ICheckSupport的isSupportBBS, 游戏内不使用，可不接
  public abstract void enterBBS(Activity activity);

  //进入用户中心，对应ICheckSupport的isSupportUserCenter
  public abstract void enterUserCenter(Activity activity);

  //实名注册，对应ICheckSupport的isAntiAddictionQuery。游戏内不使用，可不接
  public abstract void realNameRegister(Activity activity, IHandleCallback callback);

  //上传用户数据给渠道
  public abstract void submitRoleData(Activity activity, LHRole lhRole);

  //获取渠道号
  public abstract String getSubChannelId(Activity activity);

  //获得元宝
  public abstract void GainGameCoin(Activity activity, String jsonStr);

  //消耗元宝
  public abstract void ConsumeGameCoin(Activity activity, String jsonStr);

}