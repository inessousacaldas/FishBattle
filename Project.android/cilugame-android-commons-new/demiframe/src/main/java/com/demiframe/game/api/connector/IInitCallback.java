package com.demiframe.game.api.connector;

import com.demiframe.game.api.common.LHResult;

//初始化回调
public abstract interface IInitCallback
{
    public abstract void onFinished(LHResult lhResult);
}
