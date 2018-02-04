package com.demiframe.game.api.shunwang.proxy;

import android.app.Activity;

import com.demiframe.game.api.connector.IToolBar;
import com.shunwang.sdk.game.SWGameSDK;

public class LHToolBarProxy
        implements IToolBar {
    public void hideFloatToolBar(Activity activity) {
        SWGameSDK.getInstance().hideFloatingView(activity);
    }

    public void showFloatToolBar(Activity activity) {
        SWGameSDK.getInstance().showFloatingView(activity);
    }
}