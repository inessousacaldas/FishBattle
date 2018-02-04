package com.demiframe.game.api.demi.pay;

import com.demiframe.game.api.common.LHPayInfo;

/**
 * Created by CL-PC007 on 2017/5/11.
 */

public class PayData {
    public LHPayInfo getPayInfo() {
        return payInfo;
    }

    public void setPayInfo(LHPayInfo payInfo) {
        this.payInfo = payInfo;
    }

    public String getPayWay() {
        return payWay;
    }

    public void setPayWay(String payWay) {
        this.payWay = payWay;
    }

    public byte[] getPrePayBuff() {
        return prePayBuff;
    }

    public void setPrePayBuff(byte[] prePayBuff) {
        this.prePayBuff = prePayBuff;
    }

    private LHPayInfo payInfo;
    private String payWay;
    private byte[] prePayBuff;
}
