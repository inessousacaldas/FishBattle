package com.demiframe.game.api.demi.proxy;

import android.app.Activity;

import com.demiframe.game.api.common.IInnerExitCallback;
import com.demiframe.game.api.common.LHInnerExitDialog;
import com.demiframe.game.api.connector.IExit;
import com.demiframe.game.api.connector.IExitCallback;
import com.demiframe.game.api.connector.IExitSdk;
import com.demiframe.game.api.util.LogUtil;

public class LHExitProxy
  implements IExit, IExitSdk
{
  public void onExit(Activity activity, final IExitCallback exitCallback)
  {
    LHInnerExitDialog.exit(activity, new IInnerExitCallback() {
      @Override
      public void onSuccess() {
        LHExitProxy.this.onExitSdk();
        exitCallback.onExit(true);

      }

      @Override
      public void onCancel() {
        exitCallback.onExit(false);
      }
    });
  }

  public void onExitSdk()
  {

  }
}