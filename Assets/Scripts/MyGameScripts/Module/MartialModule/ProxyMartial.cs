// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Created  : 1/17/2018 5:10:21 PM
// **********************************************************************

public class ProxyMartial
{
    public static void OpenMartialView()
    {
        MartialDataMgr.MartialNetMsg.OpenKungFu(() =>
        {
            MartialDataMgr.MartialViewLogic.Open();
        });
    }

    public static void CloseMartialView()
    {
        UIModuleManager.Instance.CloseModule(MartialView.NAME);
    }
}

