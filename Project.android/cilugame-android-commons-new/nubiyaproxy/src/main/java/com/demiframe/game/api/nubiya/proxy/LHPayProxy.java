package com.demiframe.game.api.nubiya.proxy;

import android.app.Activity;
import android.widget.Toast;

import com.demiframe.game.api.common.LHPayInfo;
import com.demiframe.game.api.common.LHResult;
import com.demiframe.game.api.common.LHStatusCode;
import com.demiframe.game.api.connector.IPay;
import com.demiframe.game.api.nubiya.base.User;
import com.demiframe.game.api.util.IOUtil;
import com.demiframe.game.api.util.LogUtil;
import com.demiframe.game.api.util.SDKTools;

import org.json.JSONObject;

import java.util.HashMap;

import cn.nubia.componentsdk.constant.ConstantProgram;
import cn.nubia.nbgame.sdk.GameSdk;
import cn.nubia.nbgame.sdk.interfaces.CallbackListener;

public class LHPayProxy implements IPay {

    public void onPay(final Activity activity, final LHPayInfo lhPayInfo)
    {
        LogUtil.d("LHPayProxy", "nubiya onPay start");
        HashMap<String, String> map = new HashMap<String, String>();
        try {
            JSONObject extraJson = new JSONObject(lhPayInfo.getExtraJson());
            map.put("app_id", extraJson.getString("app_id"));
            map.put("uid", extraJson.getString("uid"));
            map.put("cp_order_id", extraJson.getString("cp_order_id"));
            map.put("amount", extraJson.getString("amount"));
            map.put("product_name", extraJson.getString("product_name"));
            map.put("product_des", extraJson.getString("product_des"));
            map.put("number", extraJson.getString("number"));
            map.put("data_timestamp", extraJson.getString("data_timestamp"));
            map.put("cp_order_sign", extraJson.getString("cp_order_sign"));
        }catch (Exception e){
            e.printStackTrace();
            return;
        }

        map.put("token_id", User.getInstances().getSessionId());
        //map.put("uid", User.getInstances().getUId());
        //map.put("app_id", SDKTools.GetSdkProperty(activity, "NUBIYA_APPID"));
        map.put("app_key", SDKTools.GetSdkProperty(activity, "NUBIYA_APPKEY"));
        //map.put("amount", String.valueOf (Integer.parseInt(lhPayInfo.getProductCount()) * Long.parseLong(lhPayInfo.getProductPrice(false))));//商品支付的金额
        //map.put("number", lhPayInfo.getProductCount());//商品数量
        map.put("price", lhPayInfo.getProductPrice(false));//商品单价
        //map.put("product_name", lhPayInfo.getProductName());//商品名称
        //map.put("product_des", lhPayInfo.getProductDes());//商品详情
        map.put("product_id", lhPayInfo.getProductId());//商品ID
        map.put("product_unit", "个");//商品单位
        //map.put("cp_order_id", lhPayInfo.getOrderSerial());//CP第三方业务服务器返回的支付订单号
        //map.put("sign", "");//CP第三方业务服务器返回的订单签名信息
        //map.put("channel_dis", "1");//分发渠道编号
//        map.put("data_timestamp", "");//CP服务端创建订单时间戳
        map.put("game_id", User.getInstances().getGameID());//玩家用于游戏的账号
       // map.put("order_type", "0");//订单来源
        //map.put("platform", "2");//系统平台，IOS:1，Android：2，默认为2
		
        GameSdk.doPay(activity, map, new CallbackListener<String>() {
            @Override
            public void callback(int responseCode, String s)
            {
                LHResult lhResult = new LHResult();
                switch (responseCode)
                {
                    case 0:
                        // 支付完成
                        LogUtil.d("LHPayProxy", "Nubiya onPay Success");
                        lhResult.setCode(LHStatusCode.LH_PAY_SUCCESS);
                        break;

                    case -1:
                        //支付失败
                        LogUtil.d("LHPayProxy", "Nubiya onPay Fail");
                        lhResult.setCode(LHStatusCode.LH_PAY_FAIL);
                        break;

                    case 10001:
                        //取消支付
                        Toast.makeText(activity.getApplicationContext(), "您已经取消本次支付", Toast.LENGTH_SHORT).show();
                        LogUtil.d("LHPayProxy", "Nubiya onPay Cancel");
                        lhResult.setCode(LHStatusCode.LH_PAY_CANCEL);
                        break;

                    case 10002:
                        //网络异常
                        LogUtil.e("LHPayProxy", "Nubiya onPay Net Exception");
                        lhResult.setCode(LHStatusCode.LH_PAY_FAIL);
                        break;

                    case 10003:
                        //订单结果确认中，建议客户端向自己业务服务器校验支付结果
                        LogUtil.e("LHPayProxy", "Nubiya onPay Net Exception");
                        lhResult.setCode(LHStatusCode.LH_PAY_SUBMITED);
                        break;

                    case 10004:
                        //支付服务正在升级
                        LogUtil.d("LHPayProxy", "Nubiya onPay upgradding");
                        lhResult.setCode(LHStatusCode.LH_PAY_FAIL);
                        break;

                    case 10005:
                        //支付组件安装失败或是未安装
                        LogUtil.e("LHPayProxy", "Nubiya onPay error");
                        lhResult.setCode(LHStatusCode.LH_PAY_FAIL);
                        break;

                    case 10006:
                        //订单信息有误
                        LogUtil.e("LHPayProxy", "Nubiya onPay msg error");
                        lhResult.setCode(LHStatusCode.LH_PAY_FAIL);
                        break;

                    case 10007:
                        //获取支付渠道失败
                        LogUtil.e("LHPayProxy", "Nubiya onPay get channel fail");
                        lhResult.setCode(LHStatusCode.LH_PAY_FAIL);
                        break;

                    case 10008:
                        //Android6.0没允许安装和更新所需权限，需要运行时请求，主要是存储权限
                        LogUtil.e("LHPayProxy", "Nubiya onPay no permission");
                        lhResult.setCode(LHStatusCode.LH_PAY_FAIL);
                        break;

                    default:
                        LogUtil.e("LHPayProxy", "Nubiya onPay fail");
                        lhResult.setCode(LHStatusCode.LH_PAY_FAIL);
                        break;
                }
                lhPayInfo.getPayCallback().onFinished(lhResult);
            }
        });

    }
}