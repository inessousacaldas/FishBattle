// **********************************************************************
// Copyright (c) 2016 cilugame. All rights reserved.
// File     : FunctionOpenNotifyListener.cs
// Author   : senkay <senkay@126.com>
// Created  : 5/3/2016 
// Porpuse  : 
// **********************************************************************
//
using System;
using AppDto;

/// <summary>
/// 后台功能开关控制监听
/// </summary>
public class FunctionOpenNotifyListener: BaseDtoListener<FunctionOpenNotify>
{
	protected override void HandleNotify(FunctionOpenNotify notify)
	{
		FunctionOpenHelper.UpdateFunctionSwitch(notify);
	}
}

