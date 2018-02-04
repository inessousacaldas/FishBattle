package com.demiframe.game.api.a4399.proxy;

import android.app.Activity;

import com.demiframe.game.api.connector.IExit;
import com.demiframe.game.api.connector.IExitCallback;
import com.demiframe.game.api.connector.IExitSdk;
import com.demiframe.game.api.util.LogUtil;

import cn.m4399.operate.OperateCenter;

public class LHExitProxy
        implements IExit, IExitSdk {
    public void onExit(Activity activity, final IExitCallback exitCallback) {
        OperateCenter.getInstance().shouldQuitGame(activity, new OperateCenter.OnQuitGameListener() {
            @Override
            public void onQuitGame(boolean shouldQuit) {
                if (shouldQuit) {
                    LHExitProxy.this.onExitSdk();
                    exitCallback.onExit(true);
                }
                else{
                    exitCallback.onExit(false);
                }
            }
        });
    }

    public void onExitSdk() {
        OperateCenter.getInstance().destroy();
    }
}