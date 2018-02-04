// **********************************************************************
// Copyright (c) 2013 Baoyugame. All rights reserved.
// File     :  ProxyBattleDemo.cs
// Author   : SK
// Created  : 2014/11/7
// Purpose  : 
// **********************************************************************

public class ProxyBattleDemoConfigModule
{

	public static void Open()
	{
        UIModuleManager.Instance.OpenFunModule<BattleDemoConfigController>( BattleDemoConfigView.NAME ,UILayerType.BaseModule, true);
	}

    public static void Close()
	{
        UIModuleManager.Instance.CloseModule( BattleDemoConfigView.NAME );
	}

    public static void Hide()
	{
        UIModuleManager.Instance.HideModule( BattleDemoConfigView.NAME );
	}

    public static void OpenEasyConfig()
    {
	    UIModuleManager.Instance.OpenFunModule<BattleDemoEasyConfigController>( BattleDemoEasyConfigView.NAME ,UILayerType.BaseModule, true);
    }

    public static void CloseEasyConfig()
    {
        UIModuleManager.Instance.CloseModule( BattleDemoEasyConfigView.NAME );
    }

    public static void HideEasyConfig()
    {
        UIModuleManager.Instance.HideModule( BattleDemoEasyConfigView.NAME );
    }
}