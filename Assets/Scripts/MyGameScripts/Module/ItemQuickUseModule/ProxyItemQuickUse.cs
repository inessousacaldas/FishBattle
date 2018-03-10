// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Created  : 3/3/2018 3:30:06 PM
// **********************************************************************

public class ProxyItemQuickUse
{
    public static void OpenItemQuickItemView()
    {
        ItemQuickUseDataMgr.ItemQuickUseViewLogic.Open();
    }

    public static void CloseItemQuickItemView()
    {
        UIModuleManager.Instance.CloseModule(ItemQuickUseView.NAME);
    }

    public static void HideItemQuickItemView()
    {
        UIModuleManager.Instance.HideModule(ItemQuickUseView.NAME);
    }
}

