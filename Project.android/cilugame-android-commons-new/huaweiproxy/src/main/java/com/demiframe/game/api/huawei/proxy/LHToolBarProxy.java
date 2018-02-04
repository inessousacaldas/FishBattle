package com.demiframe.game.api.huawei.proxy;

import android.app.Activity;

import com.demiframe.game.api.connector.IToolBar;
import com.huawei.gameservice.sdk.GameServiceSDK;

public class LHToolBarProxy
        implements IToolBar {
    public void hideFloatToolBar(Activity activity) {
        GameServiceSDK.hideFloatWindow(activity);
    }

    public void showFloatToolBar(Activity activity) {
        GameServiceSDK.hideFloatWindow(activity);
    }
}