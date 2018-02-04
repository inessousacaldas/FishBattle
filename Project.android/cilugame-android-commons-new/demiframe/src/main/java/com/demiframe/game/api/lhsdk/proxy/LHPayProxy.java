package com.demiframe.game.api.lhsdk.proxy;

import android.app.Activity;

import com.demiframe.game.api.common.LHPayInfo;
import com.demiframe.game.api.connector.IPay;
import com.demiframe.game.api.util.LogUtil;

public class LHPayProxy
        implements IPay {

    public void onPay(final Activity activity, final LHPayInfo lhPayInfo) {
        LogUtil.e("LHPayProxy", "onPay");
    }
}