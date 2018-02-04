// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Created  : 11/20/2017 5:02:26 PM
// **********************************************************************

public class ProxyCrewReCruit
{
    public static int crewCurrencyAddTimes;
    public static void Open()
    {
        CrewReCruitDataMgr.CrewReCruitPanelLogic.Open();
    }

    public static void Close()
    {
        UIModuleManager.Instance.CloseModule(CrewReCruitPanel.NAME);
    }

    public static void CloseCrewObtainPanel() {
        UIModuleManager.Instance.CloseModule(CrewObtainPanel.NAME);
        CrewReCruitDataMgr.CrewReCruitPanelLogic.Open();
    }
}

