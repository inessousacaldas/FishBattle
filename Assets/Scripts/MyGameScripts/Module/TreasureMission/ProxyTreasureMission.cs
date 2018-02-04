// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Created  : 12/13/2017 4:15:20 PM
// **********************************************************************

public class ProxyTreasureMission
{
    public static void Close()
    {
        TreasureMissionDataMgr.TreasurePanelLogic.Close();
    }

    public static void Open() {
        TreasureMissionDataMgr.TreasurePanelLogic.Open();
    }
}

