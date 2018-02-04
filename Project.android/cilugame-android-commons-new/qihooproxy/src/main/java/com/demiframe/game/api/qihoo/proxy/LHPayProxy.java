package com.demiframe.game.api.qihoo.proxy;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.text.TextUtils;
import android.widget.Toast;

import com.demiframe.game.api.qihoo.base.User;
import com.demiframe.game.api.common.LHPayInfo;
import com.demiframe.game.api.common.LHResult;
import com.demiframe.game.api.common.LHRole;
import com.demiframe.game.api.common.LHStaticValue;
import com.demiframe.game.api.common.LHStatusCode;
import com.demiframe.game.api.connector.IPay;
import com.demiframe.game.api.util.IOUtil;
import com.demiframe.game.api.util.SDKTools;
import com.qihoo.gamecenter.sdk.activity.ContainerActivity;
import com.qihoo.gamecenter.sdk.common.IDispatcherCallback;
import com.qihoo.gamecenter.sdk.matrix.Matrix;
import com.qihoo.gamecenter.sdk.protocols.ProtocolConfigs;
import com.qihoo.gamecenter.sdk.protocols.ProtocolKeys;

import org.json.JSONObject;

public class LHPayProxy
        implements IPay {

//    private boolean isAccessTokenValid;
//    private boolean isQTValid;

    public Intent getPayIntent(Activity activity, LHPayInfo lhPayInfo){
        LHRole lhRole = User.getInstances().getLastRoleData();

        Bundle bundle = new Bundle();

        // 界面相关参数，360SDK界面是否以横屏显示。
        bundle.putBoolean(ProtocolKeys.IS_SCREEN_ORIENTATION_LANDSCAPE, LHStaticValue.IsLandScape);

        // *** 以下非界面相关参数 ***

        // 设置QihooPay中的参数。

        // 必需参数，360账号id，整数。
        bundle.putString(ProtocolKeys.QIHOO_USER_ID, User.getInstances().getUserId());

        // 必需参数，所购买商品金额, 以分为单位。金额大于等于100分，360SDK运行定额支付流程； 金额数为0，360SDK运行不定额支付流程。
        int total = Integer.parseInt(lhPayInfo.getProductCount()) * Integer.parseInt(lhPayInfo.getProductPrice(true));
        bundle.putString(ProtocolKeys.AMOUNT, Integer.toString(total));

        // 必需参数，所购买商品名称，应用指定，建议中文，最大10个中文字。
        bundle.putString(ProtocolKeys.PRODUCT_NAME, lhPayInfo.getProductName());

        // 必需参数，购买商品的商品id，应用指定，最大16字符。
        bundle.putString(ProtocolKeys.PRODUCT_ID, lhPayInfo.getProductId());

        // 必需参数，应用方提供的支付结果通知uri，最大255字符。360服务器将把支付接口回调给该uri，具体协议请查看文档中，支付结果通知接口–应用服务器提供接口。
        bundle.putString(ProtocolKeys.NOTIFY_URI, lhPayInfo.getPayNotifyUrl());

        // 必需参数，游戏或应用名称，最大16中文字。
        bundle.putString(ProtocolKeys.APP_NAME, lhPayInfo.getAppName());

        // 必需参数，应用内的用户名，如游戏角色名。 若应用内绑定360账号和应用账号，则可用360用户名，最大16中文字。（充值不分区服，
        // 充到统一的用户账户，各区服角色均可使用）。
        bundle.putString(ProtocolKeys.APP_USER_NAME, lhRole.getRoleName());

        // 必需参数，应用内的用户id。
        // 若应用内绑定360账号和应用账号，充值不分区服，充到统一的用户账户，各区服角色均可使用，则可用360用户ID最大32字符。
        bundle.putString(ProtocolKeys.APP_USER_ID, lhPayInfo.getPlayerId());

        // 可选参数，应用扩展信息1，原样返回，最大255字符。
        bundle.putString(ProtocolKeys.APP_EXT_1, lhPayInfo.getPayCustomInfo());

        // 可选参数，应用扩展信息2，原样返回，最大255字符。
        //bundle.putString(ProtocolKeys.APP_EXT_2, lhPayInfo.getPayCustomInfo());

        // 可选参数，应用订单号，应用内必须唯一，最大32字符。
        bundle.putString(ProtocolKeys.APP_ORDER_ID, lhPayInfo.getOrderSerial());

        // 可选参数，默认支付类型
        //bundle.putString(ProtocolKeys.DEFAULT_PAY_TYPE, "");

        // 必需参数，使用360SDK的支付模块:CP可以根据需求选择使用 带有收银台的支付模块 或者 直接调用微信支付模块或者直接调用支付宝支付模块。
        //functionCode 对应三种支付模块：
        //ProtocolConfigs.FUNC_CODE_PAY;//表示 带有360收银台的支付模块。
        //ProtocolConfigs.FUNC_CODE_WEIXIN_PAY;//表示 微信支付模块。
        //ProtocolConfigs.FUNC_CODE_ALI_PAY;//表示支付宝支付模块。
        bundle.putInt(ProtocolKeys.FUNCTION_CODE, ProtocolConfigs.FUNC_CODE_PAY);

        // 必需参数，商品数量,游戏内逻辑请传递游戏内真实数据
        bundle.putInt(ProtocolKeys.PRODUCT_COUNT, Integer.parseInt(lhPayInfo.getProductCount()));

        // 必需参数，服务器id ,游戏内逻辑请传递游戏内真实数据
        bundle.putString(ProtocolKeys.SERVER_ID, lhPayInfo.getServerId());

        // 必需参数，服务器名称 ,游戏内逻辑请传递游戏内真实数据
        bundle.putString(ProtocolKeys.SERVER_NAME, lhRole.getZoneName());

        String ratio = SDKTools.GetSdkProperty(activity, "GoldRatio");
        int iRatio;
        try{
            iRatio = Integer.parseInt(ratio);
        }
        catch (Exception e){
            e.printStackTrace();
            iRatio = LHStaticValue.balanceRatio;
        }
        // 必需参数，兑换比例 （游戏内虚拟币兑换人民币） ,游戏内逻辑请传递游戏内真实数据
        bundle.putInt(ProtocolKeys.EXCHANGE_RATE, iRatio);

        // 必需参数，货币名称 （比如：钻石）,游戏内逻辑请传递游戏内真实数据
        bundle.putString(ProtocolKeys.GAMEMONEY_NAME, lhPayInfo.getProductName());

        // 必需参数，角色id ,游戏内逻辑请传递游戏内真实数据
        bundle.putString(ProtocolKeys.ROLE_ID, lhPayInfo.getPlayerId());


        // 必需参数，角色名称 ,游戏内逻辑请传递游戏内真实数据
        bundle.putString(ProtocolKeys.ROLE_NAME, lhRole.getRoleName());

        // 必需参数，角色等级 ,游戏内逻辑请传递游戏内真实数据
        bundle.putInt(ProtocolKeys.ROLE_GRADE, Integer.parseInt(lhRole.getRoleLevel()));

        // 必需参数，虚拟币余额 ,游戏内逻辑请传递游戏内真实数据
        bundle.putInt(ProtocolKeys.ROLE_BALANCE, Integer.parseInt(lhPayInfo.getBalance()));

        // 必需参数，vip等级 ,游戏内逻辑请传递游戏内真实数据
        bundle.putString(ProtocolKeys.ROLE_VIP, lhRole.getVipLevel("0"));

        // 必需参数，工会名称 ,游戏内逻辑请传递游戏内真实数据
        bundle.putString(ProtocolKeys.ROLE_USERPARTY, lhRole.getPartyName("无"));

        Intent intent = new Intent(activity, ContainerActivity.class);
        intent.putExtras(bundle);
        return intent;
    }


    public void onPay(final Activity activity, final LHPayInfo lhPayInfo) {
        if(!User.getInstances().CheckLogin()){
            Toast.makeText(activity, "支付请求失败，未登陆", Toast.LENGTH_SHORT).show();
            return;
        }

        if(User.getInstances().getLastRoleData() == null){
            Toast.makeText(activity, "支付请求失败，无角色数据", Toast.LENGTH_SHORT).show();
            return;
        }

        Intent intent = getPayIntent(activity, lhPayInfo);

        Matrix.invokeActivity(activity, intent, new IDispatcherCallback() {
            @Override
            public void onFinished(String data) {
                if (TextUtils.isEmpty(data)) {
                    return;
                }

                JSONObject jsonRes;
                try {
                    jsonRes = new JSONObject(data);
                    // error_code 状态码： 0 支付成功， -1 支付取消， 1 支付失败， -2 支付进行中, 4010201和4009911 登录状态已失效，引导用户重新登录
                    // error_msg 状态描述
                    int errorCode = jsonRes.optInt("error_code");
                    LHResult lhResult = new LHResult();
                    switch (errorCode) {
                        case 0:
                            lhResult.setCode(LHStatusCode.LH_PAY_SUCCESS);
                            lhPayInfo.getPayCallback().onFinished(lhResult);
                            break;
                        case 1:
                            lhResult.setCode(LHStatusCode.LH_PAY_FAIL);
                            lhPayInfo.getPayCallback().onFinished(lhResult);
                            break;
                        case -1:
                            lhResult.setCode(LHStatusCode.LH_PAY_CANCEL);
                            lhPayInfo.getPayCallback().onFinished(lhResult);
                            break;
                        case -2:
                            lhResult.setCode(LHStatusCode.LH_PAY_SUBMITED);
                            lhPayInfo.getPayCallback().onFinished(lhResult);
                            break;
                        //统一提示失败
                        case 4010201:
                            lhResult.setCode(LHStatusCode.LH_PAY_FAIL);
                            lhPayInfo.getPayCallback().onFinished(lhResult);
                            //acess_token失效
                            Toast.makeText(activity, "会话已失效，请重新登录", Toast.LENGTH_SHORT).show();
                            break;
                        case 4009911:
                            //QT失效
                            lhResult.setCode(LHStatusCode.LH_PAY_FAIL);
                            lhPayInfo.getPayCallback().onFinished(lhResult);
                            Toast.makeText(activity, "账号不是登录状态，请重新登录", Toast.LENGTH_SHORT).show();
                            break;
                        default:
                            break;
                    }
                } catch (Exception e) {
                    e.printStackTrace();
                }
            }
        });
    }
}