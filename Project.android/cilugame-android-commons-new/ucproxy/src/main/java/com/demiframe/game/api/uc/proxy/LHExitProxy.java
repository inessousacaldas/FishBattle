package com.demiframe.game.api.uc.proxy;

import android.app.Activity;

import com.demiframe.game.api.common.LHCallbackListener;
import com.demiframe.game.api.connector.IExit;
import com.demiframe.game.api.connector.IExitCallback;
import com.demiframe.game.api.uc.ucutils.LHUCGameSdk;

public class LHExitProxy
  implements IExit
{
  public void onExit(Activity activity, IExitCallback exitCallback)
  {
    LHCallbackListener.getInstance().setExitListener(exitCallback);
    LHUCGameSdk.exitSDK(activity);
  }
}