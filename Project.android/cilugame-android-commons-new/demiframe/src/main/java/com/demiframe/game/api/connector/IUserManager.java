package com.demiframe.game.api.connector;

import android.app.Activity;
import com.demiframe.game.api.common.LHUser;

public abstract interface IUserManager
{
  //游戏调用渠道的注册接口，显示注册界面。游戏内没对接，可不接
  public abstract void guestRegist(Activity activity, String paramString);

  //游戏内调出渠道的登录界面接口
  public abstract void login(Activity activity, Object object);

  //客人身份登录，游戏内不使用。可不接
  public abstract void loginGuest(Activity activity, Object object);

  //游戏通知渠道登出，必接！
  public abstract void logout(Activity activity, Object object);

  //设置用户登录登出的回调，需要保存，异步回调
  public abstract void setUserListener(Activity activity, IUserListener listener);

  //游戏通知渠道sdk切换账号，游戏内没接入，可不接
  public abstract void switchAccount(Activity activity, Object object);

  //调出渠道的用户中心界面，对应ICheckSupport的isSupportUserCenter。
  public abstract void updateUserInfo(Activity activity, LHUser lhUser);
}
