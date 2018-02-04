package com.demiframe.game.api.shoumeng.proxy;

import android.app.Activity;

import com.demiframe.game.api.common.LHCallbackListener;
import com.demiframe.game.api.common.LHUser;
import com.demiframe.game.api.connector.IUserListener;
import com.demiframe.game.api.connector.IUserManager;

import mobi.shoumeng.integrate.game.method.GameMethod;

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
    GameMethod.getInstance().login(activity);
  }

  public void loginGuest(Activity activity, Object paramObject)
  {
  }

  public void logout(Activity activity, Object paramObject)
  {
    GameMethod.getInstance().logout(activity);
  }

  public void setUserListener(Activity activity, IUserListener userListener)
  {
    LHCallbackListener.getInstance().setUserListener(userListener);
  }

  public void switchAccount(Activity activity, Object paramObject)
  {

  }

  public void updateUserInfo(Activity activity, LHUser lhUser)
  {

  }
}