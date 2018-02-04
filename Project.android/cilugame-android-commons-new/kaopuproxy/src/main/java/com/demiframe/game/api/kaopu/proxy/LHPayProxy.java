package com.demiframe.game.api.kaopu.proxy;

import android.app.Activity;
import android.widget.Toast;

import com.demiframe.game.api.common.LHPayInfo;
import com.demiframe.game.api.common.LHResult;
import com.demiframe.game.api.common.LHRole;
import com.demiframe.game.api.common.LHStaticValue;
import com.demiframe.game.api.common.LHStatusCode;
import com.demiframe.game.api.connector.IPay;
import com.demiframe.game.api.kaopu.base.User;
import com.demiframe.game.api.util.SDKTools;
import com.kaopu.supersdk.api.KPSuperSDK;
import com.kaopu.supersdk.callback.KPPayCallBack;
import com.kaopu.supersdk.model.params.PayParams;

public class LHPayProxy
        implements IPay {

    public void onPay(final Activity activity, final LHPayInfo lhPayInfo) {
        if(!KPSuperSDK.isLogin()){
            Toast.makeText(activity, "请先登录", Toast.LENGTH_SHORT).show();
            return;
        }

        LHRole lastRole = User.getInstances().getLastRoleData();
        if(lastRole == null){
            Toast.makeText(activity, "角色数据为空", Toast.LENGTH_SHORT).show();
            return;
        }

        String ratio = SDKTools.GetSdkProperty(activity, "GoldRatio");
        int iRatio;
        try{
            iRatio = Integer.parseInt(ratio);
        }
        catch (Exception e){
            e.printStackTrace();
            iRatio = LHStaticValue.balanceRatio;
        }

        PayParams payParams = new PayParams();
        double price = Integer.parseInt(lhPayInfo.getProductCount()) * Integer.parseInt(lhPayInfo.getProductPrice(true)) / 100.0;
        payParams.setAmount(price);
        payParams.setGamename(LHStaticValue.appName);
        payParams.setGameserver(lastRole.getZoneName());
        payParams.setRolename(lastRole.getRoleName());
        payParams.setOrderid(lhPayInfo.getOrderSerial());
        payParams.setCurrencyname(lhPayInfo.getProductName());
        payParams.setProportion(iRatio);
        payParams.setGoodsCount(Integer.parseInt(lhPayInfo.getProductCount()));
        payParams.setGoodsId(lhPayInfo.getProductId());
        payParams.setCustomPrice(false);
        payParams.setCustomText("");
        /**
         * 支付接口,需要同时提供支付和登录的回调接口,若用户没用登录,将直接跳转至登录界面 如果不使用回调,传null即可
         */
        KPSuperSDK.pay(activity, payParams, "", new KPPayCallBack() {
            @Override
            public void onPaySuccess() {
                LHResult lhResult = new LHResult();
                lhResult.setCode(LHStatusCode.LH_PAY_SUCCESS);
                lhPayInfo.getPayCallback().onFinished(lhResult);
            }

            @Override
            public void onPayFailed() {
                LHResult lhResult = new LHResult();
                lhResult.setCode(LHStatusCode.LH_PAY_FAIL);
                lhPayInfo.getPayCallback().onFinished(lhResult);
            }

            @Override
            public void onPayCancel() {
                //退出支付页面，或者支付状态由服务端回调时会触发.
                // 游戏收到该回调时, 若有支付等待逻辑, 取消等待即可, 无需其他操作
                LHResult lhResult = new LHResult();
                lhResult.setCode(LHStatusCode.LH_PAY_CANCEL);
                lhPayInfo.getPayCallback().onFinished(lhResult);
            }
        });
    }
}