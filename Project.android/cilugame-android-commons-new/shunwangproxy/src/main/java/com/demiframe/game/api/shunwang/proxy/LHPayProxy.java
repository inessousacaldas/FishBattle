package com.demiframe.game.api.shunwang.proxy;

import android.app.Activity;

import com.demiframe.game.api.common.LHPayInfo;
import com.demiframe.game.api.common.LHResult;
import com.demiframe.game.api.common.LHStatusCode;
import com.demiframe.game.api.connector.IPay;
import com.demiframe.game.api.shunwang.base.User;
import com.demiframe.game.api.util.IOUtil;
import com.demiframe.game.api.util.LogUtil;

import com.demiframe.game.api.util.SDKTools;
import com.shunwang.sdk.game.SWGameSDK;
import com.shunwang.sdk.game.entity.PayData;
import com.shunwang.sdk.game.listener.OnPayResponseListener;


public class LHPayProxy implements IPay
{

    public void onPay(final Activity activity, final LHPayInfo lhPayInfo)
    {
        LogUtil.d("Shunwang onPay Start");

        PayData data = new PayData();
        data.setGameId(SDKTools.GetSdkProperty(activity, "SHUNWANG_GameID"));//游戏编码
        data.setRegion("");//区服编号,暂时统一传空字符串
        data.setGuid(User.getInstances().getGuidId());//接入方guid，唯一标识。由登录验证成功后返回
        data.setSiteId(SDKTools.GetSdkProperty(activity, "SHUNWANG_SiteID"));//接入方siteId，由商务统一分配
        String url = lhPayInfo.getPayNotifyUrl() + "?cpOrderID=" + lhPayInfo.getOrderSerial();
        data.setGameCallback(url);//充值成功后回调地址(可在 URL 后面追加自定义参数，我方会透传该参数)
        data.setRsaKey(SDKTools.GetSdkProperty(activity, "SHUNWANG_RSAKey"));
        int cent = Integer.parseInt(lhPayInfo.getProductPrice(true));
        double price = cent*Integer.parseInt(lhPayInfo.getProductCount())/100d;
        LogUtil.d("price="+String.valueOf(price));
        data.setPrice(price);//充值总金额，单位：元
        //充值总金额”price”字段对应的总游戏币数，可传 0，传 0 时则 gameCoinMes 参数必传
        data.setGameCoin(0);
        //充值游说明信息（如“月卡”、“新年大礼包”等）。若传递price参数，且gameCoin为 0，则此字段必传，否则可不传
        //传元宝
        data.setGameCoinMes(lhPayInfo.getProductName());

        SWGameSDK.getInstance().pay(activity, data, new OnPayResponseListener() {
            @Override
            public void onPaySucceed() {
                LHResult lhResult = new LHResult();
                lhResult.setCode(LHStatusCode.LH_PAY_SUCCESS);
                lhPayInfo.getPayCallback().onFinished(lhResult);
            }

            @Override
            public void onPayFailed(String s) {
                LHResult lhResult = new LHResult();
                lhResult.setCode(LHStatusCode.LH_PAY_FAIL);
                lhPayInfo.getPayCallback().onFinished(lhResult);
            }

            @Override
            public void onPayWindowClose() {
                LHResult lhResult = new LHResult();
                lhResult.setCode(LHStatusCode.LH_PAY_CANCEL);
                lhPayInfo.getPayCallback().onFinished(lhResult);
            }

            @Override
            public void onSdkNoInit() {
                LHResult lhResult = new LHResult();
                lhResult.setCode(LHStatusCode.LH_PAY_FAIL);
                lhPayInfo.getPayCallback().onFinished(lhResult);
            }
        });

    }
}