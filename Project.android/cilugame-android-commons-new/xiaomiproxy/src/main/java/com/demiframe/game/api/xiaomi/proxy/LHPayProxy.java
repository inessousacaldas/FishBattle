package com.demiframe.game.api.xiaomi.proxy;

import android.app.Activity;
import android.os.Bundle;

import com.demiframe.game.api.common.LHPayInfo;
import com.demiframe.game.api.common.LHResult;
import com.demiframe.game.api.common.LHStatusCode;
import com.demiframe.game.api.connector.IPay;
import com.demiframe.game.api.util.LogUtil;
import com.demiframe.game.api.xiaomi.base.User;
import com.xiaomi.gamecenter.sdk.GameInfoField;
import com.xiaomi.gamecenter.sdk.MiCommplatform;
import com.xiaomi.gamecenter.sdk.MiErrorCode;
import com.xiaomi.gamecenter.sdk.OnPayProcessListener;
import com.xiaomi.gamecenter.sdk.entry.MiBuyInfoOnline;

import org.json.JSONObject;

import java.util.UUID;

public class LHPayProxy implements IPay
{

    public void onPay(final Activity activity, final LHPayInfo lhPayInfo) {
        LogUtil.e("LHUserManagerProxy", "XIAO MI Pay");

        MiBuyInfoOnline online = new MiBuyInfoOnline();
        //订单号唯一（不为空）
        online.setCpOrderId(lhPayInfo.getOrderSerial());
        //此参数在用户支付成功后会透传给CP的服务器
        online.setCpUserInfo(lhPayInfo.getPayCustomInfo());
        //购买数额
        online.setMiBi(Integer.parseInt(lhPayInfo.getProductPrice(false)) * Integer.parseInt(lhPayInfo.getProductCount()));

        //用户信息（以下皆为必填信息）
        Bundle mBundle = new Bundle();
        mBundle.putString(GameInfoField.GAME_USER_BALANCE, lhPayInfo.getBalance());//用户余额
        mBundle.putString(GameInfoField.GAME_USER_GAMER_VIP, User.getInstances().getLastRoleData().getVipLevel("0"));//vip等级
        mBundle.putString(GameInfoField.GAME_USER_LV, User.getInstances().getLastRoleData().getRoleLevel());//角色等级
        mBundle.putString(GameInfoField.GAME_USER_PARTY_NAME, User.getInstances().getLastRoleData().getPartyName("无"));//公会or帮派
        mBundle.putString(GameInfoField.GAME_USER_ROLE_NAME, User.getInstances().getLastRoleData().getRoleName());//角色名称
        mBundle.putString(GameInfoField.GAME_USER_ROLEID, User.getInstances().getLastRoleData().getRoledId());//角色id
        mBundle.putString(GameInfoField.GAME_USER_SERVER_NAME, User.getInstances().getLastRoleData().getZoneName());//所在服务器

        MiCommplatform.getInstance().miUniPayOnline(activity, online, mBundle, new OnPayProcessListener() {
            @Override
            public void finishPayProcess(int code) {
                LHResult lhResult = new LHResult();
                try
                {
                    switch (code)
                    {
                        case MiErrorCode.MI_XIAOMI_GAMECENTER_SUCCESS:
                            //购买成功
                            lhResult.setCode(LHStatusCode.LH_PAY_SUCCESS);
                            lhPayInfo.getPayCallback().onFinished(lhResult);
                            LogUtil.e("LHUserManagerProxy", "XIAO MI Pay Callback Success");
                            break;

                        case MiErrorCode.MI_XIAOMI_GAMECENTER_ERROR_PAY_CANCEL:
                            //取消购买
                            lhResult.setCode(LHStatusCode.LH_PAY_CANCEL);
                            lhPayInfo.getPayCallback().onFinished(lhResult);
                            LogUtil.e("LHUserManagerProxy", "XIAO MI Pay Callback Cancel");
                            break;

                        case MiErrorCode.MI_XIAOMI_PAYMENT_ERROR_PAY_FAILURE:
                            //购买失败
                            lhResult.setCode(LHStatusCode.LH_PAY_FAIL);
                            lhPayInfo.getPayCallback().onFinished(lhResult);
                            LogUtil.e("LHUserManagerProxy", "XIAO MI Pay Callback Fail");
                            break;

                        case MiErrorCode.MI_XIAOMI_GAMECENTER_ERROR_ACTION_EXECUTED:
                            //支付进行中
                            lhResult.setCode(LHStatusCode.LH_PAY_SUBMITED);
                            lhPayInfo.getPayCallback().onFinished(lhResult);
                            LogUtil.e("LHUserManagerProxy", "XIAO MI Pay Callback Executed");
                            break;

                        default:
                            break;
                    }

                }catch (Exception e)
                {
                    e.printStackTrace();
                }

            }
        });

    }
}