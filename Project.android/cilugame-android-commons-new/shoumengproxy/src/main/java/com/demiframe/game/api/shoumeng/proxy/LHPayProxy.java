package com.demiframe.game.api.shoumeng.proxy;

import android.app.Activity;

import com.demiframe.game.api.common.LHCallbackListener;
import com.demiframe.game.api.common.LHPayInfo;
import com.demiframe.game.api.common.LHStaticValue;
import com.demiframe.game.api.connector.IPay;
import com.demiframe.game.api.shoumeng.base.User;
import com.demiframe.game.api.util.IOUtil;
import com.demiframe.game.api.util.LogUtil;
import com.demiframe.game.api.util.SDKTools;

import org.json.JSONObject;

import mobi.shoumeng.integrate.game.method.GameMethod;
import mobi.shoumeng.integrate.game.PayInfo;

public class LHPayProxy
  implements IPay
{
  public void onPay(final Activity activity, final LHPayInfo lhPayInfo)
  {
    LHCallbackListener.getInstance().setPayListener(lhPayInfo.getPayCallback());

    PayInfo payInfo;
    try{
      LogUtil.d(lhPayInfo.getServerId());
      LogUtil.d(lhPayInfo.getOrderSerial());

      payInfo = new PayInfo();
      payInfo.setGameServerId(Integer.parseInt(lhPayInfo.getServerId()));//服务器ID
      payInfo.setCpOrderId(lhPayInfo.getOrderSerial());//cp订单
      int total = Integer.parseInt(lhPayInfo.getProductCount()) * Integer.parseInt(lhPayInfo.getProductPrice(false));
      payInfo.setTotalFee(total);//充值金额

      String ratio = SDKTools.GetSdkProperty(activity, "GoldRatio");
      int iRatio;
      try{
        iRatio = Integer.parseInt(ratio);
      }
      catch (Exception e){
        e.printStackTrace();
        iRatio = LHStaticValue.balanceRatio;
      }
      payInfo.setRatio(iRatio);//游戏币和人民币兑换比例, 打包工具里面填了10
      payInfo.setIsChange(false);//支付类型：false 定额支付；true 不定额支付
      payInfo.setCoinName(lhPayInfo.getProductName());//游戏币名称
      payInfo.setUserId(User.getInstances().getUserId());//用户唯一标识
    }catch (Exception e){
      e.printStackTrace();
      return;
    }

    GameMethod.getInstance().pay(activity, payInfo);
  }
}