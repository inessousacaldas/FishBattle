package com.demiframe.game.api.yijie.proxy;

import android.app.Activity;

import com.demiframe.game.api.common.IInnerExitCallback;
import com.demiframe.game.api.common.LHInnerExitDialog;
import com.demiframe.game.api.connector.IExit;
import com.demiframe.game.api.connector.IExitCallback;
import com.demiframe.game.api.connector.IExitSdk;
import com.snowfish.cn.ganga.helper.SFOnlineExitListener;
import com.snowfish.cn.ganga.helper.SFOnlineHelper;

public class LHExitProxy
  implements IExit, IExitSdk
{
  public void onExit(final Activity activity, final IExitCallback iExitCallback)
  {
    SFOnlineHelper.exit(activity, new SFOnlineExitListener() {
      @Override
      public void onNoExiterProvide() {
        LHExitProxy.this.onInnerExitSdk(activity, iExitCallback);
      }

      @Override
      public void onSDKExit(boolean b) {
        iExitCallback.onExit(b);
      }
    });
  }

  public void onInnerExitSdk(Activity activity, final IExitCallback iExitCallback){
    LHInnerExitDialog.exit(activity, new IInnerExitCallback() {
      @Override
      public void onSuccess() {
        LHExitProxy.this.onExitSdk();
        iExitCallback.onExit(true);

      }

      @Override
      public void onCancel() {
        iExitCallback.onExit(false);
      }
    });
  }

  public void onExitSdk()
  {

  }
}