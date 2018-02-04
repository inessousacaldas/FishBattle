// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Created  : 12/14/2017 2:45:59 PM
// **********************************************************************

public class ProxyGarandArenaMainView
{
    public static void Open()
    {
        GarandArenaMainViewDataMgr.GarandArenaMainViewLogic.Open();
    }

    public static void Close()
    {
        UIModuleManager.Instance.CloseModule(GarandArenaMainView.NAME);
    }
}

