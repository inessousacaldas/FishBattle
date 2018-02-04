package com.demiframe.game.api.oppo.proxy;

import com.demiframe.game.api.util.LogUtil;
import com.demiframe.game.api.util.SDKTools;
import com.nearme.game.sdk.GameCenterSDK;
import com.nearme.game.sdk.common.util.AppUtil;
import android.app.Application;

public class LHApplication extends Application {

	@Override
	public void onCreate() {
		super.onCreate();
		LogUtil.d("============LHApplication=========");
		String secret = SDKTools.GetSdkProperty(this, "OPPO_APPSECRET");
		GameCenterSDK.init(secret, this);
	}
}
