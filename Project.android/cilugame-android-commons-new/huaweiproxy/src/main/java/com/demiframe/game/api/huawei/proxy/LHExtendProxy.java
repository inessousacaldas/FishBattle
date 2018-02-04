package com.demiframe.game.api.huawei.proxy;

import android.app.Activity;
import android.widget.Toast;

import com.demiframe.game.api.common.LHRole;
import com.demiframe.game.api.connector.IExtend;
import com.demiframe.game.api.connector.IHandleCallback;
import com.demiframe.game.api.huawei.util.Sign;
import com.demiframe.game.api.util.LogUtil;
import com.huawei.gameservice.sdk.GameServiceSDK;
import com.huawei.gameservice.sdk.control.GameEventHandler;
import com.huawei.gameservice.sdk.model.Result;
import com.huawei.gameservice.sdk.model.RoleInfo;

import java.util.HashMap;

public class LHExtendProxy
        implements IExtend {
    public void antiAddictionQuery(final Activity activity, IHandleCallback handleCallback) {
    }

    public void enterBBS(final Activity activity) {
    }

    public void enterUserCenter(final Activity activity) {
    }

    public void realNameRegister(final Activity activity, IHandleCallback handleCallback) {
    }

    public void submitRoleData(final Activity activity, LHRole lhRole) {
        HashMap<String, String> playerInfo = new HashMap<String, String>();

        /**
         * 将用户的等级等信息保存起来，必须的参数为RoleInfo.GAME_RANK(等级)/RoleInfo.GAME_ROLE(角色名称)
         * /RoleInfo.GAME_AREA(角色所属区)/RoleInfo.GAME_SOCIATY(角色所属公会名称)
         * 全部使用String类型存放
         */
        /**
         * the CP save the level, role, area and sociaty of the game player into
         * the SDK
         */
        playerInfo.put(RoleInfo.GAME_RANK, lhRole.getRoleLevel());
        playerInfo.put(RoleInfo.GAME_ROLE, lhRole.getRoleName());
        playerInfo.put(RoleInfo.GAME_AREA, lhRole.getZoneId());
        String partyName = lhRole.getPartyName(null);
        if(partyName != null)
        {
            playerInfo.put(RoleInfo.GAME_SOCIATY, lhRole.getPartyName(""));
        }
        GameServiceSDK.addPlayerInfo(activity, playerInfo, new GameEventHandler() {
            @Override
            public void onResult(Result result) {
                if (result.rtnCode != Result.RESULT_OK) {
                    LogUtil.d("add player info failed:" + result.rtnCode);
                }

            }

            @Override
            public String getGameSign(String appId, String cpId, String ts) {
                return Sign.createGameSign(appId+cpId+ts);
            }

        });
    }

    //提前获取渠道号
    public String getSubChannelId(Activity activity){
        return "huawei";
    }

    public void GainGameCoin(Activity activity, String jsonStr){

    }

    public void ConsumeGameCoin(Activity activity, String jsonStr){

    }
}