// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : $Author$
// Created  : 8/28/2017 11:18:56 AM
// **********************************************************************

public class ProxyPlayerProperty
{
    public static void OpenPlayerPropertyModule()
    {
        PlayerPropertyDataMgr.PlayerPropertyViewLogic.Open();
    }
    public static void ClosePlayerPropertyModule()
    {
        UIModuleManager.Instance.CloseModule(PlayerPropertyView.NAME);
    }
}

