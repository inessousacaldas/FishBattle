// **********************************************************************
// Copyright (c) 2016 cilugame. All rights reserved.
// File     : AppGameVersion.cs
// Author   : senkay <senkay@126.com>
// Created  : 3/22/2016 
// Porpuse  : 
// **********************************************************************
//

//游戏版本
public class AppGameVersion
{
	/// <summary>
	///     片头剧情开关
	/// </summary>
    public static bool startPlotMode = false;

	/// <summary>
	///     片头CG开关
	/// </summary>
	public static bool startMovieMode = false; //取消片头，取消片头资源 @2016.4.22

	public static bool EnableStartPlotMode
	{
		get { return GameSetting.Release && startPlotMode; }
	}

	public static bool EnableStartMovieMode
	{
		get { return GameSetting.Release && startMovieMode; }
	}

	/// <summary>
	///     游戏版本号
	/// </summary>
	private static readonly string sp_version = "0.1.0";

	/// <summary>
	///     svn版本号
	/// </summary>
	public static int ver = 1;

	#region 版本号相关属性

	public static int SpVersionCode
	{
		get { return GameSetting.ParseVersionCode(sp_version); }
	}

	public static string BundleVersion
	{
		get { return sp_version; }
	}

	public static string ShortBundleVersion
	{
		get { return sp_version; }
	}

	public static int BundleVersionCode
	{
		get { return ver; }
	}

	public static string ShowVersion
	{
		get
		{
			var channelVersionStr = (GameSetting.IsOriginWinPlatform) ? "" : "_" + GameSetting.Channel;

			if (GameSetting.Release)
			{
				return "v" + BundleVersion + channelVersionStr;
			}
			return "v" + sp_version + "." + ver + channelVersionStr + "_debug";
		}
	}

	public static string GetBanhao()
	{
        return "";//新广出审[2016]336号\nISBN 978-7-89988-587-1\n文网游备字[2016]M-RPG 033号
	}

	#endregion


	#region 检查新增内容是否支持热更
	/// <summary>
	/// 检查是否支持热更
	/// 有可能相同功能不同平台不同的版本号
	/// </summary>
	/// <param name="supportVersion">功能支持的框架版本号</param>
	/// <param name="msg">自定制的错误内容</param>
	/// <returns>不支持的话返回错误内容</returns>
	public static string CheckFunctionSupport(int supportVersion, string msg = null)
	{
#if UNITY_EDITOR
		return null;
#endif

		if (supportVersion > FrameworkVersion.ver)
		{
			return string.IsNullOrEmpty(msg) ? "请使用最新版本使用完整功能" : msg;
		}
		else
		{
			return null;
		}
	}
	#endregion
}

