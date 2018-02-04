// **********************************************************************
// Copyright (c) 2016 cilugame. All rights reserved.
// File     : JavaSdkUtils.cs
// Author   : senkay <senkay@126.com>
// Created  : 2/20/2016 
// Porpuse  : 
// **********************************************************************
//


using System;
using UnityEngine;

public class JavaSdkUtils
{
#if UNITY_EDITOR || UNITY_ANDROID
	public static AndroidJavaClass GetUnityJavaClass(string path)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			try
			{
				return new AndroidJavaClass(path);
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
		}

		return null;
	}

	public static void CallSdkApi(AndroidJavaClass javaClass, string apiName, params object[] args)
	{
        Debug.Log("CallSdkApi " + apiName);

		if (Application.platform == RuntimePlatform.Android)
		{
			if (javaClass != null)
			{
				try
				{
					javaClass.CallStatic(apiName, args);
				}
				catch (Exception e)
				{
					Debug.LogException(e);
				}
			}
			else
			{
				Debug.LogError("Cannot find javaClass with " + apiName);
			}
		}
	}


	public static T CallSdkApi<T>(AndroidJavaClass javaClass, string apiName, params object[] args)
	{
        Debug.Log("CallSdkApi " + apiName);

		if (Application.platform == RuntimePlatform.Android)
		{
			if (javaClass != null)
			{
				try
				{
					return javaClass.CallStatic<T>(apiName, args);
				}
				catch (Exception e)
				{
					Debug.LogException(e);
				}
			}
			else
			{
				Debug.LogError("Cannot find javaClass with " + apiName);
			}
		}

		return default(T);
	}
#endif
}

