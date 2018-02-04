// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Created  : 1/20/2018 11:55:37 AM
// **********************************************************************

public class ProxyCopyPanel
{
    public static void Open() {
        CopyPanelDataMgr.CopyMissionViewLogic.Open();
    }

    public static void Close()
    {
        CopyPanelDataMgr.CopyMissionViewLogic.OnClose();
    }
}

