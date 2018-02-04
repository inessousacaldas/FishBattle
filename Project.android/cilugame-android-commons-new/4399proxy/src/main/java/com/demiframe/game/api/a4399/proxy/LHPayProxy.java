package com.demiframe.game.api.a4399.proxy;

import android.app.Activity;
import android.os.Handler;
import android.widget.Toast;

import com.demiframe.game.api.common.LHPayInfo;
import com.demiframe.game.api.common.LHResult;
import com.demiframe.game.api.common.LHStatusCode;
import com.demiframe.game.api.connector.IPay;
import com.demiframe.game.api.util.LogUtil;

import cn.m4399.operate.OperateCenter;

public class LHPayProxy
        implements IPay {

    public void onPay(final Activity activity, final LHPayInfo lhPayInfo) {
        int totalCent = lhPayInfo.getTotalPriceCent();
        OperateCenter.getInstance().recharge(activity, totalCent/100, lhPayInfo.getOrderSerial(), lhPayInfo.getProductName(),
                new OperateCenter.OnRechargeFinishedListener() {
                    @Override
                    public void onRechargeFinished(boolean success, int resultCode, String msg) {
                        LHResult lhResult = new LHResult();
                        if(success){
                            //当进入到真正的支付界面再取消时，会返回成功，需要使用9000进行判断成功。
                            // 4399技术反馈：9001订单处理中；9002订单已提交这两种状态需要向服务端查询是否支付成功
                            if(resultCode == 9000){
                                lhResult.setCode(LHStatusCode.LH_PAY_SUCCESS);
                            }
                            else if(resultCode == 9001 || resultCode == 9002){
                                lhResult.setCode(LHStatusCode.LH_PAY_SUBMITED);
                            }
                            else{
                                lhResult.setCode(LHStatusCode.LH_PAY_FAIL);
                            }
                        }
                        else{
                            lhResult.setCode(LHStatusCode.LH_PAY_FAIL);
                        }

                        //直接显示sdk回传的msg
                        Toast.makeText(activity, msg, Toast.LENGTH_SHORT).show();

                        LogUtil.d("LHPay success="+success+" resultCode="+resultCode+" msg="+msg);
                        lhResult.setData(msg);
                        lhPayInfo.getPayCallback().onFinished(lhResult);
                    }
                });
    }
}