// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Created  : 12/17/2017 2:57:47 PM
// **********************************************************************

public class ProxyContest
{
    public static void Close()
    {
        ContestDataMgr.ContestPanelLogic.OnClose();
    }

    public static void Open(object itemdto)
    {
        ContestDataMgr.ContestPanelLogic.Open(itemdto);
    }
}

