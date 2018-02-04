package com.demiframe.game.api.qihoo.proxy;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;

import com.demiframe.game.api.common.LHRoleDataType;
import com.demiframe.game.api.qihoo.base.User;
import com.demiframe.game.api.GameApi;
import com.demiframe.game.api.common.LHRole;
import com.demiframe.game.api.common.LHStaticValue;
import com.demiframe.game.api.connector.IExit;
import com.demiframe.game.api.connector.IExitCallback;
import com.demiframe.game.api.connector.IExitSdk;
import com.qihoo.gamecenter.sdk.activity.ContainerActivity;
import com.qihoo.gamecenter.sdk.common.IDispatcherCallback;
import com.qihoo.gamecenter.sdk.matrix.Matrix;
import com.qihoo.gamecenter.sdk.protocols.ProtocolConfigs;
import com.qihoo.gamecenter.sdk.protocols.ProtocolKeys;

import org.json.JSONObject;

public class LHExitProxy
        implements IExit, IExitSdk {
    public void onExit(final Activity activity, final IExitCallback exitCallback) {
        Bundle bundle = new Bundle();
        bundle.putBoolean(ProtocolKeys.IS_SCREEN_ORIENTATION_LANDSCAPE, LHStaticValue.IsLandScape);
        bundle.putInt(ProtocolKeys.FUNCTION_CODE, ProtocolConfigs.FUNC_CODE_QUIT);

        Intent intent = new Intent(activity, ContainerActivity.class);
        intent.putExtras(bundle);

        Matrix.invokeActivity(activity, intent, new IDispatcherCallback() {
            @Override
            public void onFinished(String data) {
                JSONObject json;
                try {
                    json = new JSONObject(data);
                    int which = json.optInt("which", -1);
                    switch (which) {
                        case 0: // 用户关闭退出界面
                            exitCallback.onExit(false);
                            break;
                            //退出游戏
                        case 1://进入论坛
                            exitCallback.onExit(false);
                            break;
                        case 2://确认退出
                            //退出前发送角色数据
                            LHRole lhRole = User.getInstances().getLastRoleData();
                            if(User.getInstances().CheckLogin() && lhRole != null){
                                //退出前发送角色数据
                                LHExtendProxy.doSubmitRoleData(activity, lhRole, LHRoleDataType.exitServer);
                            }

                            exitCallback.onExit(true);
                            break;
                        default:
                            exitCallback.onExit(false);
                            break;
                    }
                } catch (Exception e) {
                    e.printStackTrace();
                }
            }
        });
    }

    public void onExitSdk() {

    }
}