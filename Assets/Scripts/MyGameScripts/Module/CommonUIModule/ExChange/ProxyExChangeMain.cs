// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : $Author$
// Created  : 8/24/2017 2:50:25 PM
// **********************************************************************

using AppDto;

public class ProxyExChangeMain
{
    /// <summary>
    /// 打开兑换通道 非快捷兑换
    /// <param name="expectId">要兑换的</param>
    /// </summary>
    public static void OpenExChangeMain(AppVirtualItem.VirtualItemEnum expectId)
    {
        ExChangeMainDataMgr.ExChangeMainViewLogic.Open((int)expectId);
    }

    public static void CloseExChangeMain()
    {
        UIModuleManager.Instance.CloseModule(ExChangeMainView.NAME);
    }

    public static void CloseExChangeFast()
    {
        UIModuleManager.Instance.CloseModule(ExChangeFastView.NAME);
    }
}

