// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Created  : 1/13/2018 10:18:38 AM
// **********************************************************************

using AppDto;

public class ProxyFlowerMainView
{
    public static void Open(FriendInfoDto dto = null, int flowerId=0)
    {
        FlowerMainViewDataMgr.FlowerMainViewLogic.Open(dto, flowerId);
    }

    public static void Close()
    {
        UIModuleManager.Instance.CloseModule(FlowerMainView.NAME);
    }
}

