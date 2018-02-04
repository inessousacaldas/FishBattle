package com.demiframe.game.api.shoumeng.proxy;

import android.app.Activity;
import mobi.shoumeng.integrate.game.method.GameMethod;

import com.demiframe.game.api.connector.IToolBar;

public class LHToolBarProxy
  implements IToolBar
{
  public void hideFloatToolBar(Activity activity)
  {
    GameMethod.getInstance().showFloatButton(activity, false);
  }

  public void showFloatToolBar(Activity activity)
  {
    GameMethod.getInstance().showFloatButton(activity, true);
  }
}