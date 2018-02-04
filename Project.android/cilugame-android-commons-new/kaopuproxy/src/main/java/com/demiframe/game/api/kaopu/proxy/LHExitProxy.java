package com.demiframe.game.api.kaopu.proxy;

import android.app.Activity;

import com.demiframe.game.api.connector.IExit;
import com.demiframe.game.api.connector.IExitCallback;
import com.demiframe.game.api.connector.IExitSdk;
import com.demiframe.game.api.util.LogUtil;
import com.kaopu.supersdk.api.KPSuperSDK;
import com.kaopu.supersdk.callback.KPExitCallBack;

public class LHExitProxy
        implements IExit, IExitSdk {
    public void onExit(Activity activity, final IExitCallback exitCallback) {
        KPSuperSDK.exitGame(new KPExitCallBack() {
            @Override
            public void exitSuccess() {
                exitCallback.onExit(true);
            }
        });
    }

    public void onExitSdk() {

    }
}