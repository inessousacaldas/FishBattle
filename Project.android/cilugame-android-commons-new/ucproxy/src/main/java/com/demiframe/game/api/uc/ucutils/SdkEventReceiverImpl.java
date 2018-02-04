package com.demiframe.game.api.uc.ucutils;

import com.demiframe.game.api.common.LHCallbackListener;
import com.demiframe.game.api.common.LHResult;
import com.demiframe.game.api.common.LHStatusCode;
import com.demiframe.game.api.common.LHUser;
import com.demiframe.game.api.uc.base.UcHelper;
import com.demiframe.game.api.uc.base.UcUser;

import org.json.JSONObject;

import cn.uc.gamesdk.even.SDKEventKey;
import cn.uc.gamesdk.even.SDKEventReceiver;
import cn.uc.gamesdk.even.Subscribe;
import cn.uc.gamesdk.open.OrderInfo;

public class SdkEventReceiverImpl extends SDKEventReceiver {
    @Subscribe(event = SDKEventKey.ON_INIT_SUCC)
    private void onInitSucc() {
        LHResult lhResult = new LHResult();
        lhResult.setCode(LHStatusCode.LH_INIT_SUCCESS);
        LHCallbackListener.getInstance().getInitCallback().onFinished(lhResult);
    }

    @Subscribe(event = SDKEventKey.ON_INIT_FAILED)
    private void onInitFailed(String data) {
        LHResult lhResult = new LHResult();
        lhResult.setCode(LHStatusCode.LH_INIT_FAIL);
        lhResult.setData(data);
        LHCallbackListener.getInstance().getInitCallback().onFinished(lhResult);
    }

    @Subscribe(event = SDKEventKey.ON_LOGIN_SUCC)
    private void onLoginSucc(String sid) {
        UcUser ucUser = new UcUser();
        ucUser.setSid(sid);
        UcHelper.getInstances().setUcUser(ucUser);

        LHUser lhUser = new LHUser();
        lhUser.setSid(sid);
        LHCallbackListener.getInstance().getUserListener().onLogin(LHStatusCode.LH_LOGIN_SUCCESS, lhUser);
    }

    @Subscribe(event = SDKEventKey.ON_LOGIN_FAILED)
    private void onLoginFailed(String desc) {
        LHUser lhUser = new LHUser();
        lhUser.setLoginMsg(desc);
        //返回键会调用这个接口，UC要求不能出现"登录失败"，这里返回为登录取消
        LHCallbackListener.getInstance().getUserListener().onLogin(LHStatusCode.LH_LOGIN_CANCEL, lhUser);
    }

    @Subscribe(event = SDKEventKey.ON_LOGOUT_SUCC)
    private void onLogoutSucc() {
        LHCallbackListener.getInstance().getUserListener().onLogout(LHStatusCode.LH_LOGOUT_SUCCESS, "onLogoutSucc");
    }

    @Subscribe(event = SDKEventKey.ON_LOGOUT_FAILED)
    private void onLogoutFailed() {
        LHCallbackListener.getInstance().getUserListener().onLogout(LHStatusCode.LH_LOGOUT_FAIL, "onLogoutFailed");
    }

    @Subscribe(event = SDKEventKey.ON_EXIT_SUCC)
    private void onExitSucc(String desc) {
        LHCallbackListener.getInstance().getExitListener().onExit(true);
    }

    @Subscribe(event = SDKEventKey.ON_EXIT_CANCELED)
    private void onExitCanceled(String desc) {
        LHCallbackListener.getInstance().getExitListener().onExit(false);
    }

    @Subscribe(event = SDKEventKey.ON_CREATE_ORDER_SUCC)
    private void onCreateOrderSucc(OrderInfo orderInfo) {
    }

    @Subscribe(event = SDKEventKey.ON_PAY_USER_EXIT)
    private void onPayUserExit(OrderInfo orderInfo) {
    }

    private String createOrderString(OrderInfo orderInfo) {
        try {
            JSONObject json = new JSONObject();
            json.put("orderId", orderInfo.getOrderId());
            json.put("orderAmount", orderInfo.getOrderAmount());
            json.put("payWay", orderInfo.getPayWay());
            json.put("payWayName", orderInfo.getPayWayName());
            return json.toString();
        } catch (Throwable e) {
            e.printStackTrace();
        }
        return "";
    }

}

