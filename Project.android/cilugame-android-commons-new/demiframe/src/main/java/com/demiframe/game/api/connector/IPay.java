package com.demiframe.game.api.connector;

import android.app.Activity;
import com.demiframe.game.api.common.LHPayInfo;

public abstract interface IPay
{
  //支付接口，如果渠道要求支付的同时传递角色的信息，可以在LHExtendProxy上传角色数据的时候
  //缓存起来使用。支付完成需要调用支付回调paramLHPayInfo.getPayCallback
  public abstract void onPay(Activity activity, LHPayInfo paramLHPayInfo);
}