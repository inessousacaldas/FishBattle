package com.demiframe.game.api.acfun.proxy;

import android.app.Activity;
import android.os.Handler;
import android.os.Looper;
import android.os.Message;
import android.text.TextUtils;
import android.widget.Toast;

import com.demiframe.game.api.common.LHCallbackListener;
import com.demiframe.game.api.common.LHPayInfo;
import com.demiframe.game.api.common.LHResult;
import com.demiframe.game.api.common.LHRole;
import com.demiframe.game.api.common.LHStatusCode;
import com.demiframe.game.api.connector.IPay;
import com.demiframe.game.api.util.LogUtil;
import com.joygames.hostlib.JoyGamesSDK;
import com.joygames.hostlib.listener.PayCallback;

import java.text.DecimalFormat;
import java.util.HashMap;
import java.util.Map;

public class LHPayProxy
        implements IPay {
    public void onPay(final Activity activity, final LHPayInfo lhPayInfo) {
        LHRole lastRole = LHExtendProxy.lastRole;
        if(lastRole == null){
            LogUtil.d("LHExtendProxy.lastRole is null");
            return;
        }

        final Looper mainLooper = activity.getMainLooper();

        int level = Integer.parseInt(lastRole.getRoleLevel());
        //acfun等级0会判断未非法，这里0级时设置为-1
        if(level == 0){
            level = -1;
        }

        HashMap<String, String> payData = new HashMap<String, String>();
// 角色信息
        payData.put("role_id", lastRole.getRoledId());//角色id
        payData.put("role_level", level + "");//角色等级
        payData.put("role_name", lastRole.getRoleName());//角色名
        payData.put("server_id", lastRole.getZoneId());//服务器id
        payData.put("server_name", lastRole.getZoneName());//服务器名称
//商品信息
        int cent = Integer.parseInt(lhPayInfo.getProductPrice(true));
        payData.put("price", cent * Integer.parseInt(lhPayInfo.getProductCount()) / 100f + "");//支付总金额，单位：元
        payData.put("product_desc", lhPayInfo.getProductDes());//商品描述
        payData.put("product_name", lhPayInfo.getProductName());//商品名称
        payData.put("product_id", lhPayInfo.getProductId());//商品id
        payData.put("coin", lhPayInfo.getBalance());//玩家货币数量
        payData.put("ext_info", "null");//透传参数
        payData.put("cp_order_id", lhPayInfo.getOrderSerial());//CP 方订单号，请确保唯一
        JoyGamesSDK.getInstance().pay(activity, payData, new PayCallback() {
            @Override
            public void onSuccess(final String afOrderId) {
                LHResult lhResult = new LHResult();
                lhResult.setCode(LHStatusCode.LH_PAY_SUCCESS);
                lhPayInfo.getPayCallback().onFinished(lhResult);
            }
            @Override
            public void onCancle() {
                LHResult lhResult = new LHResult();
                lhResult.setCode(LHStatusCode.LH_PAY_CANCEL);
                lhPayInfo.getPayCallback().onFinished(lhResult);

                new Handler(mainLooper).post(new Runnable() {
                    @Override
                    public void run() {
                        Toast.makeText(activity, "支付取消", Toast.LENGTH_SHORT).show();
                    }
                });
            }
            @Override
            public void onError(final String errorMsg) {
                LHResult lhResult = new LHResult();
                lhResult.setCode(LHStatusCode.LH_PAY_FAIL);
                lhResult.setData(errorMsg);
                lhPayInfo.getPayCallback().onFinished(lhResult);

                new Handler(mainLooper).post(new Runnable() {
                    @Override
                    public void run() {
                        Toast.makeText(activity, "支付发生错误", Toast.LENGTH_SHORT).show();
                    }
                });
            }
        });
    }
}