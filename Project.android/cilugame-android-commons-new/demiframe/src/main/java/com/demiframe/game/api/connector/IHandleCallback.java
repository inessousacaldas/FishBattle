package com.demiframe.game.api.connector;

import com.demiframe.game.api.common.LHResult;

public abstract interface IHandleCallback
{
  public abstract void onFinished(LHResult lhResult);
}