package com.demiframe.game.api.huawei.proxy;

import android.app.Activity;
import android.content.res.Configuration;

import com.demiframe.game.api.common.LHPayInfo;
import com.demiframe.game.api.common.LHResult;
import com.demiframe.game.api.common.LHStaticValue;
import com.demiframe.game.api.common.LHStatusCode;
import com.demiframe.game.api.connector.IPay;
import com.demiframe.game.api.huawei.util.Sign;
import com.demiframe.game.api.util.IOUtil;
import com.demiframe.game.api.util.LogUtil;
import com.demiframe.game.api.util.SDKTools;
import com.huawei.gameservice.sdk.GameServiceSDK;
import com.huawei.gameservice.sdk.control.GameEventHandler;
import com.huawei.gameservice.sdk.model.PayResult;
import com.huawei.gameservice.sdk.model.Result;

import org.json.JSONObject;

import java.util.HashMap;
import java.util.Map;

public class LHPayProxy
        implements IPay {
    public static final boolean Debug = false;

    public static final String USER_ID = "userID";

    public static final String APPLICATION_ID = "applicationID";

    public static final String AMOUNT = "amount";

    public static final String PRODUCT_NAME = "productName";

    public static final String PRODUCT_DESC = "productDesc";

    public static final String REQUEST_ID = "requestId";

    public static final String USER_NAME = "userName";

    public static final String SIGN = "sign";

    public static final String NOTIFY_URL = "notifyUrl";

    public static final String SERVICE_CATALOG = "serviceCatalog";

    public static final String SHOW_LOG = "showLog";

    public static final String SCREENT_ORIENT = "screentOrient";

    public static final String SDK_CHANNEL = "sdkChannel";

    public static final String URL_VER = "urlver";

    public void onPay(final Activity activity, final LHPayInfo lhPayInfo) {
        String appID = SDKTools.GetSdkProperty(activity, "HUAWEI_APPID");

        /*
        Map<String, String> params = new HashMap<String, String>();
        // 必填字段，不能为null或者""，请填写从联盟获取的支付ID
        // the pay ID is required and can not be null or ""
        params.put(USER_ID, GlobalParam.PAY_ID);
        // 必填字段，不能为null或者""，请填写从联盟获取的应用ID
        // the APP ID is required and can not be null or ""
        params.put(APPLICATION_ID, GlobalParam.APP_ID);
        // 必填字段，不能为null或者""，单位是元，精确到小数点后两位，如1.00
        // the amount (accurate to two decimal places) is required
        params.put(AMOUNT, price);
        // 必填字段，不能为null或者""，道具名称
        // the product name is required and can not be null or ""
        params.put(PRODUCT_NAME, productName);
        // 必填字段，不能为null或者""，道具描述
        // the product description is required and can not be null or ""
        params.put(PRODUCT_DESC, productDesc);
        // 必填字段，不能为null或者""，最长30字节，不能重复，否则订单会失败
        // the request ID is required and can not be null or "". Also it must be unique.
        params.put(REQUEST_ID, requestId);

        String noSign = getSignData(params);
        LogUtil.d("startPay", "noSign：" + noSign);

        // CP必须把参数传递到服务端，在服务端进行签名，然后把sign传递下来使用；服务端签名的代码和客户端一致
        // the CP need to send the params to the server and sign the params on the server ,
        // then the server passes down the sign to client;
        String sign = RSAUtil.sign(noSign, GlobalParam.PAY_RSA_PRIVATE);
        LogUtil.d("startPay", "sign： " + sign);
        */


        Map<String, Object> payInfo = new HashMap<String, Object>();
        // 必填字段，不能为null或者""
        // the amount is required and can not be null or ""
        //payInfo.put(AMOUNT, lhPayInfo.getProductCount());
        // 必填字段，不能为null或者""
        // the product name is required and can not be null or ""
        //payInfo.put(PRODUCT_NAME, lhPayInfo.getProductName());
        // 必填字段，不能为null或者""
        // the request ID is required and can not be null or ""
        //payInfo.put(REQUEST_ID, lhPayInfo.getOrderSerial());
        // 必填字段，不能为null或者""
        // the product description is required and can not be null or ""
        //payInfo.put(PRODUCT_DESC, lhPayInfo.getProductDes());
        // 必填字段，不能为null或者""，请填写自己的公司名称
        // the user name is required and can not be null or "". Input the company name of CP.
        payInfo.put(USER_NAME, LHStaticValue.cpName);

       payInfo.put(NOTIFY_URL, lhPayInfo.getPayNotifyUrl());

        // 必填字段，不能为null或者""
        // the APP ID is required and can not be null or "".
        //payInfo.put(APPLICATION_ID, appID);

        try {
            JSONObject extraJson = new JSONObject(lhPayInfo.getExtraJson());
            // 必填字段，不能为null或者""
            // the user ID is required and can not be null or "".
            payInfo.put(USER_ID, extraJson.getString("userID"));
            payInfo.put(APPLICATION_ID, extraJson.getString("applicationID"));
            payInfo.put(AMOUNT, extraJson.getString("amount"));
            payInfo.put(REQUEST_ID, extraJson.getString("requestId"));


            payInfo.put(PRODUCT_NAME, extraJson.getString("productName"));
            payInfo.put(PRODUCT_DESC, extraJson.getString("productDesc"));
            // 必填字段，不能为null或者""
            // the sign is required and can not be null or "".
            payInfo.put(SIGN, extraJson.getString("sign"));
        }catch (Exception e){
            e.printStackTrace();
            return;
        }


        // 必填字段，不能为null或者""，此处写死X6
        // the service catalog is required and can not be null or "".
        payInfo.put(SERVICE_CATALOG, "X6");


        // 调试期可打开日志，发布时注释掉
        // print the log for demo
        payInfo.put(SHOW_LOG, Debug);

        if (activity.getResources().getConfiguration().orientation == Configuration.ORIENTATION_LANDSCAPE)
        {
            payInfo.put(SCREENT_ORIENT, 2);
        }
        else
        {
            payInfo.put(SCREENT_ORIENT, 1);
        }

        GameServiceSDK.startPay(activity, payInfo, new GameEventHandler()
        {
            @Override
            public String getGameSign(String appId, String cpId, String ts) {
                return Sign.createGameSign(appId+cpId+ts);
            }

            @Override
            public void onResult(Result result)
            {
                LHResult lhResult = new LHResult();
                //不进行验签，因为客户端仅仅做一个提示
                if(result.rtnCode == Result.RESULT_OK){
                    lhResult.setCode(LHStatusCode.LH_PAY_SUCCESS);
                    lhPayInfo.getPayCallback().onFinished(lhResult);
                }
                else{
                    lhResult.setCode(LHStatusCode.LH_PAY_FAIL);
                    lhPayInfo.getPayCallback().onFinished(lhResult);
                }
            }
        });
    }
}