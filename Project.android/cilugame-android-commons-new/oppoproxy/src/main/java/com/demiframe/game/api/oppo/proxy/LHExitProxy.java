package com.demiframe.game.api.oppo.proxy;

import android.app.Activity;

import com.demiframe.game.api.common.LHCallbackListener;
import com.demiframe.game.api.connector.IExit;
import com.demiframe.game.api.connector.IExitCallback;
import com.demiframe.game.api.connector.IExitSdk;
import com.demiframe.game.api.util.LogUtil;
import com.nearme.game.sdk.GameCenterSDK;
import com.nearme.game.sdk.callback.ApiCallback;
import com.nearme.game.sdk.callback.GameExitCallback;

public class LHExitProxy
        implements IExit, IExitSdk {
    public void onExit(Activity activity,final IExitCallback exitCallback) {
        GameCenterSDK.getInstance().onExit(activity, new GameExitCallback() {
            @Override
            public void exitGame() {
                if(exitCallback != null)
                    exitCallback.onExit(true);
            }
        });
    }

    public void onExitSdk() {

    }
}