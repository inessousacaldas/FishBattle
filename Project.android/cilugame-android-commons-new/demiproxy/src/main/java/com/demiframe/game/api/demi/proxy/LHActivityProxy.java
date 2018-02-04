package com.demiframe.game.api.demi.proxy;

import android.app.Activity;
import android.content.Intent;
import android.content.res.Configuration;

import com.demiframe.game.api.common.LHResult;
import com.demiframe.game.api.connector.IActivity;
import com.demiframe.game.api.connector.ICheckSupport;
import com.demiframe.game.api.connector.IInitCallback;
import com.demiframe.game.api.util.LHCheckSupport;

public class LHActivityProxy
  implements IActivity
{
  public LHActivityProxy()
  {
    LHCheckSupport.setCheckSupport(new ICheckSupport()
    {
      public boolean isAntiAddictionQuery()
      {
        return false;
      }

      public boolean isSupportBBS()
      {
        return false;
      }

      public boolean isSupportLogout()
      {
        return true;
      }

      public boolean isSupportOfficialPlacard()
      {
        return true;
      }

      public boolean isSupportShowOrHideToolbar()
      {
        return false;
      }

      public boolean isSupportUserCenter()
      {
        return false;
      }
    });
  }

  public void onActivityResult(Activity activity, int paramInt1, int paramInt2, Intent paramIntent)
  {
  }

  public void onCreate(Activity activity, IInitCallback paramIInitCallback, Object paramObject)
  {
    if (paramIInitCallback != null)
    {
      LHResult localLHResult = new LHResult();
      localLHResult.setCode(0);
      localLHResult.setData("初始化成功");
      paramIInitCallback.onFinished(localLHResult);
    }
  }

  public void afterOnCreate(Activity activity, IInitCallback listener, Object obj){

  }

  public Boolean needAfterCreate(){
    return false;
  }

  public void onDestroy(Activity activity)
  {
  }

  public void onNewIntent(Activity activity, Intent paramIntent)
  {
  }

  public void onPause(Activity activity)
  {
  }

  public void onRestart(Activity activity)
  {
  }

  public void onResume(Activity activity)
  {
  }

  public void onStop(Activity activity)
  {
  }

  public void onStart(Activity activity)
  {
  }

  public void onConfigurationChanged(Activity activity, Configuration newConfig){

  }
}