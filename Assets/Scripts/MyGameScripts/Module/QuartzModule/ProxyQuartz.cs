// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : $Author$
// Created  : 7/1/2017 6:05:18 PM
// **********************************************************************

public class ProxyQuartz
{
    public static void OpenQuartzMainView(QuartzDataMgr.TabEnum tab = QuartzDataMgr.TabEnum.Info)
    {
        QuartzDataMgr.QuartzNetMsg.Orbment_Info(() =>
        {
            QuartzDataMgr.QuartzViewLogic.Open(tab);
        });
    }

    public static void CloseQuartzMainView()
    {
        UIModuleManager.Instance.CloseModule(QuartzView.NAME);
    }
}

