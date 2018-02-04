
// 安卓常开，先简单处理
//#define ENABLE_XINGE

using UnityEngine;

public static class XinGeAndroidSdk
{

	private const string SDK_JAVA_CLASS = "com.cilugame.h1.XGSDKPlugin";

#if UNITY_EDITOR || (UNITY_ANDROID && ENABLE_XINGE)
	private static AndroidJavaClass cls;
#endif

	public static void Setup()
	{
#if UNITY_EDITOR || (UNITY_ANDROID && ENABLE_XINGE)
        cls = JavaSdkUtils.GetUnityJavaClass(SDK_JAVA_CLASS);
#endif
	}

	public static void Register()
	{
#if UNITY_EDITOR || (UNITY_ANDROID && ENABLE_XINGE)
        JavaSdkUtils.CallSdkApi(cls, "RegisterPush");
#endif
	}

	public static void RegisterWithAccount(string account)
	{
#if UNITY_EDITOR || (UNITY_ANDROID && ENABLE_XINGE)
        JavaSdkUtils.CallSdkApi(cls, "RegisterPushWithAccount", account);
#endif
	}

	public static void SetTag(string tagName)
	{
#if UNITY_EDITOR || (UNITY_ANDROID && ENABLE_XINGE)
        JavaSdkUtils.CallSdkApi(cls, "SetTag", tagName);
#endif
	}

	public static void DeleteTag(string tagName)
	{
#if UNITY_EDITOR || (UNITY_ANDROID && ENABLE_XINGE)
        JavaSdkUtils.CallSdkApi(cls, "DeleteTag", tagName);
#endif
	}

}
