// **********************************************************************
// Copyright (c) 2016 cilugame. All rights reserved.
// File     : FrameworkVersion.cs
// Author   : senkay <senkay@126.com>
// Created  : 3/22/2016 
// Porpuse  : 
// **********************************************************************
//
using System;

//引擎版本
public class FrameworkVersion
{
	//此版本号用来判断是否需要整包更新，因为框架的版本不能动态更新
	public static int ver = 52007;

	#region 版本号相关属性

	public static string ShowVersion
	{
		get
		{
			if (GameSetting.Release)
			{
				return "v" + ver;
			}
			return "v" + ver + "_debug";
		}
	}

	#endregion
}
