// **********************************************************************
// Copyright (c) 2013 Baoyugame. All rights reserved.
// File     :  ProxyTestModule.cs
// Author   : willson
// Created  : 2014/12/11 
// Porpuse  : 
// **********************************************************************

using UnityEngine;

public static class ProxyAnimatorTestModule
{
	private const string NAME = "AnimatorPanel";
	
	public static void Open()
	{
		GameObject module = UIModuleManager.Instance.OpenFunModule(NAME,UILayerType.DefaultModule,false);
		module.GetMissingComponent<AnimatorTestController>();
	}

	public static void Hide()
	{
		UIModuleManager.Instance.HideModule(NAME);	
	}
	
	public static void Close()
	{
		UIModuleManager.Instance.CloseModule(NAME);
	}
}