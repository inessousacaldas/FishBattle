package com.demigame.h5.android.demi.wxapi;


import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.util.Log;

import com.demiframe.game.api.common.LHResult;
import com.demiframe.game.api.common.LHStatusCode;
import com.demiframe.game.api.demi.pay.wxpay.Constants;
import com.demiframe.game.api.demi.proxy.LHPayProxy;
import com.demiframe.game.api.util.SDKTools;
import com.tencent.mm.sdk.constants.ConstantsAPI;
import com.tencent.mm.sdk.modelbase.BaseReq;
import com.tencent.mm.sdk.modelbase.BaseResp;
import com.tencent.mm.sdk.openapi.IWXAPI;
import com.tencent.mm.sdk.openapi.IWXAPIEventHandler;
import com.tencent.mm.sdk.openapi.WXAPIFactory;

public class WXPayEntryActivity extends Activity implements IWXAPIEventHandler{
	
	private static final String TAG = "WXPayEntryActivity";
	
    private IWXAPI api;
	
    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        //setContentView(R.layout.pay_result);

		String appID = SDKTools.GetSdkProperty(this, "WXPAY_APPID");
    	api = WXAPIFactory.createWXAPI(this, appID);
        api.handleIntent(getIntent(), this);
    }

	@Override
	protected void onNewIntent(Intent intent) {
		super.onNewIntent(intent);
		setIntent(intent);
        api.handleIntent(intent, this);
	}

	@Override
	public void onReq(BaseReq req) {
	}

	@Override
	public void onResp(BaseResp resp) {
		Log.d(TAG, "onPayFinish, errCode = " + resp.errCode);

		LHResult lhResult = new LHResult();
		if (resp.getType() == ConstantsAPI.COMMAND_PAY_BY_WX) {
			switch (resp.errCode)
			{
				case 0:
					lhResult.setCode(LHStatusCode.LH_PAY_SUCCESS);
					LHPayProxy.mPayCallback.onFinished(lhResult);
					break;
				case -1:
					lhResult.setCode(LHStatusCode.LH_PAY_FAIL);
					LHPayProxy.mPayCallback.onFinished(lhResult);
					break;
				case -2:
					lhResult.setCode(LHStatusCode.LH_PAY_CANCEL);
					LHPayProxy.mPayCallback.onFinished(lhResult);
					break;
			}
			this.finish();
		}
	}
}