// **********************************************************************
// Copyright (c) 2016 cilugame. All rights reserved.
// File     : SdkMessageManager.cs
// Author   : senkay <senkay@126.com>
// Created  : 7/1/2016 
// Porpuse  : 
// **********************************************************************
//
using System;
using AppServices;
using UnityEngine;
using LITJson;
using CySdk;

public class SdkMessageManager
{
	//sdk返回信息事件
	public event Action<SdkCallbackInfo, string> OnSdkCallbackInfo;

	private static readonly SdkMessageManager instance = new SdkMessageManager();
	public static SdkMessageManager Instance
	{
		get
		{
			return instance;
		}
	}

	public void Setup()
	{
		SdkMessageScript sdkMessageScript = SdkMessageScript.Setup();
		sdkMessageScript.OnSdkCallbackInfo += HandleSdkCallbackInfo;
        sdkMessageScript.OnCYSdkCallbackInfo += onResult;
    }

	private void HandleSdkCallbackInfo(string json)
	{
		Debug.Log("SdkMessageManager.HandleSdkCallbackInfo " + json);

		if (!string.IsNullOrEmpty(json))
		{
			SdkCallbackInfo info = null;
			try{
				info = JsHelper.ToObject<SdkCallbackInfo>(json);
			}
			catch (Exception e)
			{
				GameDebuger.LogException(e);
			}

			if (OnSdkCallbackInfo != null)
			{
				OnSdkCallbackInfo(info, json);
			}

			if (info != null)
			{
				Debug.Log("info.type=" + info.type + " code=" + info.code + " data=" + info.data);
				switch(info.type)
				{
				//电量
				case "power":
					int intPower = 0;
					int.TryParse(info.data, out intPower);

					if (intPower == BaoyugameSdk.BATTERY_CHARGING)
					{
						BaoyugameSdk.batteryChargingOfAndroid = true;
					}
					else
					{
						BaoyugameSdk.batteryChargingOfAndroid = false;
						BaoyugameSdk.batteryLevelOfAndroid = intPower;
					}			
					break;
                    //畅游统一回调
                    case "onResult":
                        onResult(info.data);
                        break;
                //	//信鸽注册
                //case "XGRegisterResult":
                //	//0 success  1 fail
                //                    GameDebuger.TODO(@"ModelManager.DailyPush.SetXgState(info.data);");
                //	break;
                //	//信鸽账号注册
                //case "XGRegisterWithAccountResult":
                //	//0 success  1 fail
                //                    GameDebuger.TODO(" ServiceRequestAction.requestServer(PlayerService.pigeon(GameSetting.BundleId,info.data == '0',GameSetting.PlatformTypeId));			");
                //	break;
                //	//SDK 初始化
                //case "init":
                //	SPSdkManager.Instance.CallbackInit(info.code == 0);
                //	break;
                //	//SDK 登陆
                //case "login":
                //	//0 accountLogin 1 guestLogin  2 logincancel 3 loginFail
                //	if (info.code == 0)
                //	{
                //		string[] strs = info.data.Split(';');
                //		if (strs.Length > 1) {
                //			//这里处理MHXY的登陆返回
                //			GameSetting.LoginWay = strs[1];

                        //			PayExtInfo payExtInfo = LoginManager.Instance.PayExtInfo;
                        //			if (payExtInfo == null)
                        //			{
                        //				payExtInfo = new PayExtInfo();
                        //				LoginManager.Instance.PayExtInfo = payExtInfo;
                        //			}

                        //			payExtInfo.openid = strs[0].Split('|')[0];
                        //			payExtInfo.openkey = strs[2];
                        //			payExtInfo.pf = strs[3];
                        //			payExtInfo.pfkey = strs[4];

                        //			SPSdkManager.Instance.CallbackLoginSuccess(false, strs[0]);
                        //		} else {
                        //			SPSdkManager.Instance.CallbackLoginSuccess(false, info.data);
                        //		}
                        //	}
                        //	else if(info.code == 1)
                        //	{
                        //		SPSdkManager.Instance.CallbackLoginSuccess(true, info.data);
                        //	}
                        //	else if(info.code == 2)
                        //	{
                        //		SPSdkManager.Instance.CallbackLoginCancel();
                        //	}
                        //	else
                        //	{
                        //		SPSdkManager.Instance.CallbackLoginFail();
                        //	}			
                        //	break;
                        //	//SDK 登出
                        //case "logout":
                        //	SPSdkManager.Instance.CallbackLogout(info.code == 0);
                        //	break;
                        //	//SDK 没有退出提供
                        //case "noExiterProvide":
                        //	SPSdkManager.Instance.CallbackNoExiterProvide();
                        //	break;
                        //	//退出
                        //case "exit":
                        //	SPSdkManager.Instance.CallbackExit(info.code == 0);
                        //	break;
                        //	//错误
                        //case "error":
                        //	GameDebuger.LogError(string.Format("错误 {0}，原因 {1}", info.code, info.data));
                        //	break;
                        //	//支付
                        //case "pay":
                        //	//0 paySuccess 1 payCancle  2 payFail
                        //	if (info.code == 0)
                        //	{
                        //		SPSdkManager.Instance.CallbackPay(true);
                        //	}
                        //	else
                        //	{
                        //		SPSdkManager.Instance.CallbackPay(false);
                        //	}			
                        //	break;
                }
			}
		}
	}

    /// <summary>
    /// 畅游回调
    /// </summary>
    /// <param name="jsonParam"></param>
    public void onResult(string jsonParam)
    {
        Debug.Log("onResult:" + jsonParam);

        SPSdkManager.Instance.OnCYCallback(jsonParam);

        //JsonData jsonData = JsonMapper.ToObject(jsonParam);
        //int state_code = (int)jsonData["state_code"];
        //string message = (string)jsonData["message"];
        //string data= (string)jsonData["data"];
        //switch (state_code)
        //{
        //    case ResultCode.INIT_SUCCESS:
        //        break;
        //    case ResultCode.LOGIN_SUCCESS:
        //    case ResultCode.LOGIN_CANCEL:
        //    case ResultCode.LOGIN_FAILED:
        //    case ResultCode.SWITCH_USER_SUCCESS:
        //        // GameHandler.Instance.showLoginView();
        //        // GameHandler.Instance.verifyToken(jsonParam);
        //        break;
        //    case ResultCode.SWITCH_USER_FAILED:
        //        // PlatformGame.game().showToast("切换账号失败");
                
        //        Debug.Log(jsonParam);
        //        break;
        //    case ResultCode.LOGOUT:
        //        //  GameHandler.Instance.doLogout(jsonParam);
        //        break;
        //    case ResultCode.HOST_SUCCESS:
        //    case ResultCode.HOST_FAILED:
        //        //  GameHandler.Instance.doGotHost(jsonParam);
        //        break;
        //    case ResultCode.GOODS_SUCCESS:
        //    case ResultCode.GOODS_FAILED:
        //        // GameHandler.Instance.showGoods(jsonParam);
        //        break;
        //    case ResultCode.PAY_SUCCESS:
        //        SPSdkManager.Instance.CallbackPay(true);
        //        break;
        //    case ResultCode.PAY_FAILED:
        //        SPSdkManager.Instance.CallbackPay(false);
        //        break;
        //    case ResultCode.PAY_CANCEL:
        //        //  GameHandler.Instance.doPayment(jsonParam);
        //        break;
        //    case ResultCode.EXIT_GAME:
        //    case ResultCode.EXIT_GAME_DIALOG:
        //        //  GameHandler.Instance.doExit(jsonParam);
        //        break;
        //    case ResultCode.AUTH_SUCCESS:
        //        //  GameHandler.Instance.doAuth(jsonParam);
        //        break;
        //    case ResultCode.PLUGIN_RESULT:
        //        //  GameHandler.Instance.doPlugin(jsonParam);
        //        break;
        //    default:
        //        Debug.LogError(message);
        //        break;
        //}
    }

}

