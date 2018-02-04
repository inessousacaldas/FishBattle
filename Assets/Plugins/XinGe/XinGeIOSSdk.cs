
using UnityEngine;

#if UNITY_EDITOR || (UNITY_IPHONE && ENABLE_XINGE)
using System.Runtime.InteropServices;
#endif

public static class XinGeIOSSdk
{
#if UNITY_EDITOR || (UNITY_IPHONE && ENABLE_XINGE)
    [DllImport("__Internal")]
	private static extern void __xinGeSetup(string appId, string appKey);

	[DllImport("__Internal")]
	private static extern void __xinGeRegister();

	[DllImport("__Internal")]
	private static extern void __xinGeEnableDebug(bool enable);

	[DllImport("__Internal")]
	private static extern void __xinGeRegisterWithAccount(string account);

	[DllImport("__Internal")]
	private static extern void __xinGeSetTag(string tagName);

	[DllImport("__Internal")]
	private static extern void __xinGeDeleteTag(string tagName);
#endif

	public static void Setup()
	{
#if UNITY_EDITOR || (UNITY_IPHONE && ENABLE_XINGE)
        var appId = IosBridge.GetValueFromInfoPlist("XinGeSDK.AccessId");
		var appKey = IosBridge.GetValueFromInfoPlist("XinGeSDK.AccessKey");

		__xinGeSetup(appId, appKey);
#endif
    }

    public static void Register()
	{
#if UNITY_EDITOR || (UNITY_IPHONE && ENABLE_XINGE)
        __xinGeRegister();
#endif
    }


    public static void EnableDebug(bool enable)
	{
#if UNITY_EDITOR || (UNITY_IPHONE && ENABLE_XINGE)
        __xinGeEnableDebug(enable);
#endif
    }


    public static void RegisterWithAccount(string account)
	{
#if UNITY_EDITOR || (UNITY_IPHONE && ENABLE_XINGE)
        __xinGeRegisterWithAccount(account);
#endif
	}


	public static void SetTag(string tagName)
	{
#if UNITY_EDITOR || (UNITY_IPHONE && ENABLE_XINGE)
        __xinGeSetTag(tagName);
#endif
	}


	public static void DeleteTag(string tagName)
	{
#if UNITY_EDITOR || (UNITY_IPHONE && ENABLE_XINGE)
        __xinGeDeleteTag(tagName);
#endif
	}
}
