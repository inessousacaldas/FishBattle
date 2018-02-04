package com.demiframe.game.api.shoumeng.proxy;

import android.app.Activity;

import com.demiframe.game.api.connector.IExit;
import com.demiframe.game.api.connector.IExitCallback;
import com.demiframe.game.api.connector.IExitSdk;
import com.demiframe.game.api.common.LHCallbackListener;

import mobi.shoumeng.integrate.game.method.GameMethod;

public class LHExitProxy
  implements IExit, IExitSdk
{
  public void onExit(Activity activity, IExitCallback exitCallback)
  {
    LHCallbackListener.getInstance().setExitListener(exitCallback);
    GameMethod.getInstance().exit(activity);
  }

  public void onExitSdk()
  {

  }
}