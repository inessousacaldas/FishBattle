package com.demiframe.game.api.yijie.proxy;

import android.app.Activity;

import com.demiframe.game.api.common.LHCallbackListener;
import com.demiframe.game.api.common.LHUser;
import com.demiframe.game.api.connector.IUserListener;
import com.demiframe.game.api.connector.IUserManager;
import com.demiframe.game.api.util.LogUtil;
import com.snowfish.cn.ganga.helper.SFOnlineHelper;

public class LHUserManagerProxy
  implements IUserManager
{
  Activity mActivity;
  IUserListener mUserListener;

  public void guestRegist(Activity activity, String paramString)
  {
    LogUtil.d("游戏版本不支持，不处理");
  }

  public void login(Activity activity, Object object)
  {
    SFOnlineHelper.login(activity, object);
  }

  public void loginGuest(Activity activity, Object object)
  {
    LogUtil.d("游戏版本不支持，不处理");
  }

  public void logout(Activity activity, Object object)
  {
    SFOnlineHelper.logout(activity, object);
  }

  public void setUserListener(Activity activity, IUserListener userListener)
  {
    LHCallbackListener.getInstance().setUserListener(userListener);
  }

  public void switchAccount(Activity activity, Object object)
  {
    LogUtil.d("游戏版本不支持，不处理");
  }

  public void updateUserInfo(Activity activity, LHUser lhUser)
  {

  }
}