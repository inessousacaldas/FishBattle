package com.demiframe.game.api.uc.proxy;

import android.app.Activity;

import com.demiframe.game.api.connector.IToolBar;
import com.demiframe.game.api.util.LogUtil;

public class LHToolBarProxy
  implements IToolBar
{
  public void hideFloatToolBar(Activity activity)
  {
    LogUtil.d("渠道不支持的操作<hideFloatToolBar>");
  }

  public void showFloatToolBar(Activity activity)
  {
    LogUtil.d("渠道不支持的操作<showFloatToolBar>");
  }
}