package com.demiframe.game.api.demi.proxy;

import android.app.Activity;

import com.demiframe.game.api.common.LHUser;
import com.demiframe.game.api.connector.IUserListener;
import com.demiframe.game.api.connector.IUserManager;
import com.demiframe.game.api.util.LogUtil;

public class LHUserManagerProxy
  implements IUserManager
{
  Activity mActivity;
  IUserListener mUserListener;

  public void guestRegist(Activity activity, String paramString)
  {

  }

  public void login(Activity activity, Object paramObject)
  {
    LogUtil.e("LHUserManagerProxy", "inner login");
  }

  public void loginGuest(Activity activity, Object paramObject)
  {
  }

  public void logout(Activity activity, Object paramObject)
  {
    LogUtil.e("LHUserManagerProxy", "inner logout");
  }

  public void setUserListener(Activity activity, IUserListener userListener)
  {

  }

  public void switchAccount(Activity activity, Object paramObject)
  {
  }

  public void updateUserInfo(Activity activity, LHUser lhUser)
  {

  }
}