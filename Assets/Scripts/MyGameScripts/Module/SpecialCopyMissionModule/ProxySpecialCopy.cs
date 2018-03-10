// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Created  : 2/8/2018 3:10:11 PM
// **********************************************************************

public class ProxySpecialCopy
{
    public static void Open(int missionid)
    {
        SpecialCopyDataMgr.SpecialCopyMissionPanelLogic.Open(missionid);
    }
    public static void Close()
    {
        SpecialCopyDataMgr.SpecialCopyMissionPanelLogic.Close();
    }
}

