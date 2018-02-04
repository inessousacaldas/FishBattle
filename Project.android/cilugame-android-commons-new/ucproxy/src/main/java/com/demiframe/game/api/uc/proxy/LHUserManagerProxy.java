package com.demiframe.game.api.uc.proxy;

import android.app.Activity;

import com.demiframe.game.api.common.LHCallbackListener;
import com.demiframe.game.api.common.LHUser;
import com.demiframe.game.api.connector.IUserListener;
import com.demiframe.game.api.connector.IUserManager;
import com.demiframe.game.api.uc.base.UcHelper;
import com.demiframe.game.api.uc.base.UcUser;
import com.demiframe.game.api.uc.ucutils.LHUCGameSdk;
import com.demiframe.game.api.util.LogUtil;

public class LHUserManagerProxy
  implements IUserManager
{
  public void guestRegist(Activity activity, String paramString)
  {
    LogUtil.d("游戏版本不支持，不处理");
  }

  public void login(Activity activity, Object paramObject)
  {
    LHUCGameSdk.login(activity);
  }

  public void loginGuest(Activity activity, Object paramObject)
  {
    LogUtil.d("游戏版本不支持，不处理");
  }

  public void logout(Activity activity, Object paramObject)
  {
    LHUCGameSdk.logout(activity);
  }

  public void setUserListener(Activity activity, IUserListener userListener)
  {
    LHCallbackListener.getInstance().setUserListener(userListener);
  }

  public void switchAccount(Activity activity, Object paramObject)
  {
    LogUtil.d("不支持切换账号");
  }

  public void updateUserInfo(Activity activity, LHUser lhUser)
  {
    UcUser localUcUser = UcHelper.getInstances().getUcUser();
    localUcUser.setUid(lhUser.getUid());
  }
}