// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Created  : 12/19/2017 11:25:11 AM
// **********************************************************************

public class ProxyGuideMainView
{
    public static void Open()
    {
        GuideMainViewDataMgr.GuideMainViewLogic.Open();
    }

    public static void Close()
    {
        UIModuleManager.Instance.CloseModule(GuideMainView.NAME);
    }
}

