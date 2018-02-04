package com.cilugame.h1;

import com.cilugame.h1.util.Logger;
import com.unity3d.player.UnityPlayer;

import org.json.JSONObject;

public class UnityCallbackWrapper
{
	public static void OnInit(boolean success)
	{
		SendToUnity("init", success?0:1, "");
	}
	
	public static void OnLoginSuccess(String sid)
	{
		SendToUnity("login", 0, sid);
	}
 
	public static void OnLoginCancel()
	{
		SendToUnity("login", 2, "");
	}
	
	public static void OnLoginFail()
	{
		SendToUnity("login", 3, "");
	}
	
	public static void OnLoginOut(boolean  success )
	{
		SendToUnity("logout", success?0:1, "");
	}	
	
	public static void OnExit(boolean success)
	{
		SendToUnity("exit", success?0:1, "");
	}
	
	public static void OnNoExiterProvide()
	{
		SendToUnity("noExiterProvide", 0, "");
	}
	
	public static void OnPay(int flag)
	{
		SendToUnity("pay", flag, "");
	}	

	private static void SendToUnity(String type, int code, String data)
	{
        try {
            JSONObject jobj = new JSONObject();
            jobj.put("type", type);
            jobj.put("code", code);
            jobj.put("data", data);
            
            SendToUnity("OnSdkCallback", jobj.toString());
        } catch (Throwable e) {
            e.printStackTrace();
        }
	}
	
	public static void SendToUnity(String function, String params)
	{
		UnityPlayer.UnitySendMessage("_SdkMessageHandler",function, params);
	}
}