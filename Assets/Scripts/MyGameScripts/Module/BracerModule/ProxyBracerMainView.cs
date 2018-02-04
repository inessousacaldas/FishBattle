// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Created  : 11/9/2017 3:18:17 PM
// **********************************************************************

public class ProxyBracerMainView
{
    public static void Open()
    {
        BracerMainViewDataMgr.BracerMainViewLogic.Open();
    }

    public static void Close()
    {
        UIModuleManager.Instance.CloseModule(BracerMainView.NAME);
    }
}

