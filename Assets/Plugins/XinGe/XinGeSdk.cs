
using UnityEngine;

public static class XinGeSdk
{
	public static void Setup()
	{
		switch (Application.platform)
		{
			case RuntimePlatform.Android:
				{
					XinGeAndroidSdk.Setup();
					break;
				}
			case RuntimePlatform.IPhonePlayer:
				{
					XinGeIOSSdk.Setup();
					break;
				}
		}
	}


	public static void Register()
	{
		switch (Application.platform)
		{
			case RuntimePlatform.Android:
				{
					XinGeAndroidSdk.Register();
					break;
				}
			case RuntimePlatform.IPhonePlayer:
				{
					XinGeIOSSdk.Register();
					break;
				}
		}
	}


	public static void EnableDebug(bool enable)
	{
		switch (Application.platform)
		{
			case RuntimePlatform.Android:
				{
					break;
				}
			case RuntimePlatform.IPhonePlayer:
				{
					XinGeIOSSdk.EnableDebug(enable);
					break;
				}
		}
	}


	public static void RegisterWithAccount(string account)
	{
		switch (Application.platform)
		{
			case RuntimePlatform.Android:
				{
					XinGeAndroidSdk.RegisterWithAccount(account);
					break;
				}
			case RuntimePlatform.IPhonePlayer:
				{
					XinGeIOSSdk.RegisterWithAccount(account);
					break;
				}
		}
	}


	public static void SetTag(string tagName)
	{
		switch (Application.platform)
		{
			case RuntimePlatform.Android:
				{
					XinGeAndroidSdk.SetTag(tagName);
					break;
				}
			case RuntimePlatform.IPhonePlayer:
				{
					XinGeIOSSdk.SetTag(tagName);
					break;
				}
		}
	}


	public static void DeleteTag(string tagName)
	{
		switch (Application.platform)
		{
			case RuntimePlatform.Android:
				{
					XinGeAndroidSdk.DeleteTag(tagName);
					break;
				}
			case RuntimePlatform.IPhonePlayer:
				{
					XinGeIOSSdk.DeleteTag(tagName);
					break;
				}
		}
	}
}
