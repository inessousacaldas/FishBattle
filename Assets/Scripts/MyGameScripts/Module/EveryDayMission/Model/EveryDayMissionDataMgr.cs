// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : DM-PC092
// Created  : 11/10/2017 4:36:55 PM
// **********************************************************************

using AppDto;
using UniRx;

public sealed partial class EveryDayMissionDataMgr
{
    public static bool IsOpenCheckEveryMission;
    // 初始化
    private void LateInit()
    {
        IsOpenCheckEveryMission = true;
    }
    

    public void OnDispose(){

    }
}
