// **********************************************************************
// Copyright  2013 Baoyugame. All rights reserved.
// File     :  BaoyugameSdk.cs
// Author   : senkay
// Created  : 6/26/2013 5:54:13 PM
// Purpose  : 
// **********************************************************************

using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using System.IO;

public class BaoyugameSdk
{
	private const string SDK_JAVA_DeviceUtils = "com.cilugame.android.commons.DeviceUtils";

	private const string SDK_JAVA_AssetsUtils = "com.cilugame.android.commons.AssetsUtils";

	public const string SDK_JAVA_ZPHTemp = "com.cilugame.android.commons.ZPHTemp";

	public const string SDK_JAVA_PatchUtils = "com.cilugame.h1.patch.PatchUtils";

	public const int BATTERY_CHARGING = 9999;
	public static int batteryLevelOfAndroid = 100;
	public static bool batteryChargingOfAndroid = false;

	//没有网络
	public const string NET_STATE_NONE = "NONE";
	//2G网络
	public const string NET_STATE_2G = "2G";
	//3G网络
	public const string NET_STATE_3G = "3G";
	//WIFI网络
	public const string NET_STATE_WIFI = "WIFI";

	#if UNITY_ANDROID
	private static AndroidJavaClass SDK_DeviceUtils;
	private static AndroidJavaClass SDK_AssetsUtils;
	private static AndroidJavaClass SDK_ZPHTemp;
	private static AndroidJavaClass SDK_PatchUtils;
	#endif

	public static void Setup ()
	{
		#if UNITY_ANDROID
		SDK_DeviceUtils = JavaSdkUtils.GetUnityJavaClass(SDK_JAVA_DeviceUtils);
		SDK_AssetsUtils = JavaSdkUtils.GetUnityJavaClass(SDK_JAVA_AssetsUtils);
		SDK_ZPHTemp = JavaSdkUtils.GetUnityJavaClass(SDK_JAVA_ZPHTemp);
		SDK_PatchUtils = JavaSdkUtils.GetUnityJavaClass(SDK_JAVA_PatchUtils);
		#endif
	}

	#region SDK_DeviceUtils

	private static void callSdkApi (string apiName, params object[] args)
	{
		#if UNITY_ANDROID
		JavaSdkUtils.CallSdkApi(SDK_DeviceUtils, apiName, args);
		#endif
	}

	/// <summary>
	/// Installs the apk.
	/// </summary>
	/// <param name='apkUrl'>
	/// Apk URL.
	/// </param>
	public static void InstallApk (string apkUrl)
	{
		#if (UNITY_EDITOR || UNITY_STANDALONE)
		#elif UNITY_ANDROID
		callSdkApi("installApk", apkUrl);
		#else
		#endif
	}

    public static string GetPackageName()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
#elif UNITY_ANDROID
#endif

        return string.Empty;
    }

	public static void RestartGame ()
	{
#if (UNITY_EDITOR || UNITY_STANDALONE)
#elif UNITY_ANDROID
		callSdkApi("RestartGame", 0);
#else
#endif
	}

	public static void GetPermisson ()
	{
#if (UNITY_EDITOR || UNITY_STANDALONE)
#elif UNITY_ANDROID
		callSdkApi("GetPermisson");
#else
#endif
	}

	public static void CheckWriteExternalPermission ()
	{
#if (UNITY_EDITOR || UNITY_STANDALONE)
#elif UNITY_ANDROID
		callSdkApi("CheckWriteExternalPermission");
#else
#endif
	}

	//return  0-100
	public static int GetBatteryLevel ()
	{
#if (UNITY_EDITOR || UNITY_STANDALONE)
		return UnityEngine.Random.Range (-1, 101);
#elif UNITY_ANDROID
		return batteryLevelOfAndroid;
#elif UNITY_IPHONE
		return iOSUtility.GetBatteryLevel ( );
#else
		return 100;
#endif
	}

	public static bool IsBattleCharging ()
	{
#if (UNITY_EDITOR || UNITY_STANDALONE)
		return false;
#elif UNITY_ANDROID
		return batteryChargingOfAndroid;
#elif UNITY_IPHONE
		return iOSUtility.IsBattleCharging ( );
#else
		return false;
#endif
	}

	private static bool _powerRegistered = false;

	public static void RegisterPower ()
	{
		if (_powerRegistered) {
			return;
		}
		_powerRegistered = true;

#if (UNITY_EDITOR || UNITY_STANDALONE)
		return;
#elif UNITY_ANDROID
		if (SDK_DeviceUtils != null)
		{
			SDK_DeviceUtils.CallStatic("RegisterPower");
		}
#else
        return;
#endif
	}

	public static void UnregisterPower ()
	{
		if (_powerRegistered == false) {
			return;
		}

		_powerRegistered = false;

#if (UNITY_EDITOR || UNITY_STANDALONE)
		return;
#elif UNITY_ANDROID
		if (SDK_DeviceUtils != null)
		{
			SDK_DeviceUtils.CallStatic("UnregisterPower");
		}
#else
        return;
#endif
	}

	public static long getFreeMemory ()
	{
#if (UNITY_EDITOR || UNITY_STANDALONE)
		return 0;
#elif UNITY_ANDROID
		if (SDK_DeviceUtils != null)
		{
			return SDK_DeviceUtils.CallStatic<long>("getFreeMemory");
		}
		else
		{
			return 0;
		}
#elif UNITY_IPHONE
		return (long)iOSUtility.GetFreeMemory ( );
#else
        return 0;
#endif
	}

	public static long getTotalMemory ()
	{
#if (UNITY_EDITOR || UNITY_STANDALONE)
		return 0;
#elif UNITY_ANDROID
		if (SDK_DeviceUtils != null)
		{
			return SDK_DeviceUtils.CallStatic<long>("getTotalMemory");
		}
		else
		{
			return 0;
		}
#elif UNITY_IPHONE
		return (long)iOSUtility.GetTotalMemory ( );
#else
		return 0;
#endif
	}

	public static long getExternalStorageAvailable ()
	{
		Debug.Log ("Unity3D getExternalStorageAvailable calling...");
#if (UNITY_EDITOR || UNITY_STANDALONE)
		return 0;
#elif UNITY_ANDROID
		if (SDK_DeviceUtils != null)
		{
			return SDK_DeviceUtils.CallStatic<long>("getExternalStorageAvailable");
		}
		else
		{
			return 0;
		}
#elif UNITY_IPHONE
		long freeDisk = (long)iOSUtility.GetFreeDiskSpaceInBytes ( );
		return freeDisk >> 10;
#else
		return 0;
#endif
	}

    /**
     * <pre>
     * 获取网络类型
     * 无网络: NONE
     * 未知类型: UNKNOWN
     * WIFI: WIFI
     * 2G: 2G
     * 3G: 3G
     * 4G: 4G
     * </pre>
     * @return 网络类型标识
     */
    public static string getNetworkType()
    {
#if (UNITY_EDITOR || UNITY_STANDALONE || UNITY_IPHONE)
        NetworkReachability ability = Application.internetReachability;

        if (ability == NetworkReachability.NotReachable)
        {
            return NET_STATE_NONE;
        }
        else if (ability == NetworkReachability.ReachableViaLocalAreaNetwork)
        {
            return NET_STATE_WIFI;
        }
        else
        {
            return NET_STATE_3G;
        }
#elif UNITY_ANDROID
		if (SDK_DeviceUtils != null)
		{
			return SDK_DeviceUtils.CallStatic<string>("getNetworkType");
		}
		else
		{
			return NET_STATE_WIFI;
		}
#else
        return NET_STATE_WIFI;
#endif
    }

    public static void RegisterGsmSignalStrength ()
	{
#if (UNITY_EDITOR || UNITY_STANDALONE)
		return;
#elif UNITY_ANDROID
		if (SDK_DeviceUtils != null)
		{
			SDK_DeviceUtils.CallStatic("RegisterGsmSignalStrength");
		}
#elif UNITY_IPHONE
		return;
#else
		return;
#endif
	}

	public static void UnregisterGsmSignalStrength ()
	{
#if (UNITY_EDITOR || UNITY_STANDALONE)
		return;
#elif UNITY_ANDROID
		if (SDK_DeviceUtils != null)
		{
			SDK_DeviceUtils.CallStatic("UnregisterGsmSignalStrength");
		}
#elif UNITY_IPHONE
		return;
#else
		return;
#endif
	}

	public static int getWifiSignal ()
	{
#if (UNITY_EDITOR || UNITY_STANDALONE)
		return UnityEngine.Random.Range (0, 101);
#elif UNITY_ANDROID
		if (SDK_DeviceUtils != null)
		{
			return SDK_DeviceUtils.CallStatic<int>("getWifiSignal");
		}
		else
		{
			return 100;
		}
#elif UNITY_IPHONE
		return 100;
#else
		return 100;
#endif
	}

	/**
     * 获取本机网卡Mac地址
     * @return
     */
	public static string getLocalMacAddress ()
	{
		Debug.Log ("Unity3D getLocalMacAddress calling.....");

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_IPHONE
		try
		{
			var interfaces = NetworkInterface.GetAllNetworkInterfaces();
			for (int i = 0; i < interfaces.Length; i++)
			{
				var inter = interfaces[i];
				if (!string.IsNullOrEmpty(inter.GetPhysicalAddress().ToString()))
				{
					var mac = inter.GetPhysicalAddress().ToString();
					for (int j = mac.Length - 1 - 1; j >= 1; j = j - 2)
					{
						mac = mac.Insert(j, "-");
					}
					return mac;
				}
			}
		}
		catch (Exception e)
		{
			Debug.LogWarning(e);
		}
#elif UNITY_ANDROID
		if (SDK_DeviceUtils != null)
		{
			return SDK_DeviceUtils.CallStatic<string>("getLocalMacAddress");
		}
		else
		{
			return "111-11-11-111";
		}
#endif
		return "111-11-11-111";
	}

	/**
     * 取sdcard容量与本机容量, 返回字符串(sdcard容量|手机容量)
     * @return
     */
	public static string getStorageInfos ()
	{
		Debug.Log ("Unity3D getStorageInfos calling.....");
#if (UNITY_EDITOR || UNITY_STANDALONE)
		return "1|1";
		
#elif UNITY_ANDROID
		if (SDK_DeviceUtils != null)
		{
			return SDK_DeviceUtils.CallStatic<string>("getStorageInfos");
		}
		else
		{
			return "1|1";
		}
#elif UNITY_IPHONE
		long freeDisk = (long)iOSUtility.GetFreeDiskSpaceInBytes ( );
		freeDisk >>= 10;
		return "0|" + freeDisk.ToString( );
#else
		return "1|1";
#endif
	}


    	
	/**
	 * 判断是否带sdcard
	 * @return
	 */
	public static bool hasExternalStorage ()
	{
		Debug.Log ("Unity3D hasExternalStorage..");
#if (UNITY_EDITOR || UNITY_STANDALONE)
		return false;

#elif UNITY_ANDROID
		if (SDK_DeviceUtils != null)
		{
			return SDK_DeviceUtils.CallStatic<bool>("externalStorageAvailable");
		}
		else
		{
			return false;
		}
#else
        return false;
#endif
	}


    public static string GetAndroidInternalPersistencePath()
    {
#if UNITY_EDITOR
#elif UNITY_ANDROID
        try
        {
            return SDK_DeviceUtils.CallStatic<string>("GetInternalPersistencePath");
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
#endif

        return string.Empty;
    }

    public static string GetAndroidPersistencePath()
	{	
		Debug.Log ("Unity3D GetAndroidPersistencePath..");
		string p = Application.persistentDataPath;
		
#if (UNITY_EDITOR || UNITY_STANDALONE)
		return p;
		
#elif UNITY_ANDROID
		if (string.IsNullOrEmpty(p))
		{
			
			try
			{
				p = SDK_DeviceUtils.CallStatic<string>("GetExternalPersistencePath");
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
			
			if (!string.IsNullOrEmpty(p) &&
			    !Directory.Exists(p))
			{
				try { Directory.CreateDirectory(p); } 
				catch {}
			}
			
			bool ok = (!string.IsNullOrEmpty(p) && Directory.Exists(p));
			
			if (!ok)
			{
				// 强制使用 Internal 了
				try
				{
					p = SDK_DeviceUtils.CallStatic<string>("GetInternalPersistencePath");
				}
				catch (Exception e)
				{
					Debug.LogException(e);
				}
			}
		}
#else
		return p;
#endif

	    return p;
	}

	/**
         * 显示安装未签名app的设置窗口
         */
	public static void showSettingsInstallNonMarketApps ()
	{
		Debug.Log ("Unity3D showSettingsInstallNonMarketApps calling.....");

#if (UNITY_EDITOR || UNITY_STANDALONE)
#elif UNITY_ANDROID
		if (SDK_DeviceUtils != null)
		{
			SDK_DeviceUtils.CallStatic("showSettingsInstallNonMarketApps");
		}
#endif
	}


	/**
	 * 是否可安装未签名APP
	 * @return
	 * 			0 不可安装
	 * 			1可安装
	 */
	public static int getOptionInstallNonMarketApps ()
	{
		Debug.Log ("Unity3D getOptionInstallNonMarketApps calling.....");
#if (UNITY_EDITOR || UNITY_STANDALONE)
		return 0;

#elif UNITY_ANDROID
		if (SDK_DeviceUtils != null)
		{
			return SDK_DeviceUtils.CallStatic<int>("getOptionInstallNonMarketApps");
		}
		else
		{
			return 0;
		}
#else
        return 0;
#endif
	}

	/**
     * 取得生成应用的唯一串,返回字符串(新生成标记|UUID)
     * 新生成标记: 1首次生成, 0读取缓存的
     * @return
     */
	public static string getUUID ()
	{
		string uuid = SystemInfo.deviceUniqueIdentifier;

		Debug.Log ("Unity3D getUUID = "+uuid);

		return uuid;

//#if (UNITY_EDITOR || UNITY_STANDALONE)
//		return SystemInfo.deviceUniqueIdentifier;
//#elif UNITY_ANDROID
//		if (SDK_DeviceUtils != null)
//		{
//			return SDK_DeviceUtils.CallStatic<string>("getUUID");
//		}
//		else
//		{
//		return SystemInfo.deviceUniqueIdentifier;
//		}
//#elif UNITY_IPHONE
//		return SystemInfo.deviceUniqueIdentifier;
//#else
//		return SystemInfo.deviceUniqueIdentifier;
//#endif
	}

	/** 
		* 判断是否开启了自动亮度调节 
		*/
	public static bool isAutoBrightness ()
	{
#if (UNITY_EDITOR || UNITY_STANDALONE)
		return false;
#elif UNITY_ANDROID
		if (SDK_DeviceUtils != null)
		{
		return SDK_DeviceUtils.CallStatic<bool>("isAutoBrightness");
		}
		else
		{
		return false;
		}
#else
		return false;
#endif
	}

	/** 
		* 关闭亮度自动调节 
		*/
	public static void stopAutoBrightness ()
	{
#if (UNITY_EDITOR || UNITY_STANDALONE)
		return;
#elif UNITY_ANDROID
		if (SDK_DeviceUtils != null)
		{
		SDK_DeviceUtils.CallStatic("stopAutoBrightness");
		}
#else
		return;
#endif
	}

	/** 
		* 开启亮度自动调节 
		*/
	public static void startAutoBrightness ()
	{
#if (UNITY_EDITOR || UNITY_STANDALONE)
		return;
#elif UNITY_ANDROID
		if (SDK_DeviceUtils != null)
		{
			SDK_DeviceUtils.CallStatic("startAutoBrightness");
		}
#else
		return;
#endif
	}

	/**
		* 获取当前屏幕亮度
		*/
	public static int getScreenBrightness ()
	{
#if (UNITY_EDITOR || UNITY_STANDALONE)
		return 255;
#elif UNITY_ANDROID
		if (SDK_DeviceUtils != null)
		{
		return SDK_DeviceUtils.CallStatic<int>("getScreenBrightness");
		}
		else
		{
		return 255;
		}
#elif UNITY_IPHONE
		return iOSUtility.GetBrightness ( ); 
#else
		return 255;
#endif
	}


	/**
		* 省电模式,设置亮度
		*/
	public static void setBrightness (int brightness)
	{
#if (UNITY_EDITOR || UNITY_STANDALONE)
		return;
#elif UNITY_ANDROID
		if (SDK_DeviceUtils != null)
		{
		SDK_DeviceUtils.CallStatic("setBrightness",brightness);
		}
#elif UNITY_IPHONE
		iOSUtility.SetBrightness ( brightness );
#else
		return;
#endif
	}


#endregion


#region SDK_AssetsUtils

	/**
	 * 把asset复制成另一个目录文件
	 * @param assetName 资源名字
	 * @param toFilePath 文件全路径
	 * @param decompresses 是否解压(true解压,false正常读取)
	 * @return
	 */
	public static bool copyAssetAs (String assetName, String toFilePath, bool decompresses)
	{
#if UNITY_ANDROID
		if (SDK_AssetsUtils != null)
		{
			return SDK_AssetsUtils.CallStatic<bool>("copyAssetAs", assetName, toFilePath, decompresses);
		}
		else
		{
			return false;
		}
#endif
		return false;
	}

	
	
	/**
	 * 复制资源的批量接口
	 * assetName|toFilePath|decompresses,...
	 * @param assets
	 * @return
	 */
	public static void copyAssetAs (string assets)
	{
#if UNITY_ANDROID
		if (SDK_AssetsUtils != null)
		{
		 SDK_AssetsUtils.CallStatic("copyAssetAs", assets);
		}
#endif
	}


	/**
         * 获取Assets 目录下的资源信息
         * @return
         */
	public static byte[] readBytes (string assetPath)
	{
#if UNITY_ANDROID
		if (SDK_AssetsUtils != null)
		{
			return SDK_AssetsUtils.CallStatic<byte[]>("readBytes", assetPath);
		}
		else
		{
		return null;
		}
#endif
		return null;
	}

	/**
		* 读取metadata
		* @return
		*/
	public static string getMetaData (string name)
	{
		Debug.Log ("Unity3D getMetaData calling.....");
#if (UNITY_EDITOR || UNITY_STANDALONE)
		return "";

#elif UNITY_ANDROID
		if (SDK_AssetsUtils != null)
		{
		return SDK_AssetsUtils.CallStatic<string>("getMetaData", name);
		}
		else
		{
		return "";
		}
#else
		return "";
#endif
	}

#endregion



#region SDK_PatchUtils

	/// <summary>
	///	* 操作-修改版本检查下载URL 1
	///	* 操作-修改整个版本内容 2
	///	* 操作-恢复 3
	///	* 操作-弹出版本文件内容 4
	/// </summary>
	/// <param name="opt">Opt.</param>
	/// <param name="content">Content.</param>
	public static void PatchUtilsOpt (int opt, string content)
	{
#if UNITY_EDITOR || UNITY_STANDALONE
		return;

#elif UNITY_ANDROID
		if (SDK_PatchUtils != null)
		{
		Debug.Log(string.Format("PatchUtilsOpt opt={0} content={1}", opt, content));
		SDK_PatchUtils.CallStatic<int>("opt", opt, content);
		}
		else
		{
		Debug.Log("PatchUtilsOpt SDK_PatchUtils == null");
		}
#endif
	}

#endregion
}
