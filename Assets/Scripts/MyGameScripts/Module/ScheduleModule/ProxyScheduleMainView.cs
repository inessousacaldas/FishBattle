// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Created  : 1/19/2018 5:35:39 PM
// **********************************************************************

public class ProxyScheduleMainView
{
    public static void Open()
    {
        ScheduleMainViewDataMgr.ScheduleMainViewLogic.Open();
    }

    public static void Close()
    {
        UIModuleManager.Instance.CloseModule(ScheduleMainView.NAME);
    }
}

