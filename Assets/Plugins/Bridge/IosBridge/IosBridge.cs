using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;


public static class IosBridge
{
#if UNITY_EDITOR || UNITY_IPHONE
    [DllImport("__Internal")]
	private static extern string __getValueFromInfoPlist(string keyPath);

	[DllImport("__Internal")]
	private static extern void __copyToClipboard(string value);
#endif

    public static string GetValueFromInfoPlist(string keyPath)
	{
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
#if UNITY_EDITOR || UNITY_IPHONE
            return __getValueFromInfoPlist(keyPath);
#endif
        }

        return null;
	}

	public static void CopyToClipboard(string value)
	{
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
#if UNITY_EDITOR || UNITY_IPHONE
            __copyToClipboard(value);
#endif
        }
	}
}
