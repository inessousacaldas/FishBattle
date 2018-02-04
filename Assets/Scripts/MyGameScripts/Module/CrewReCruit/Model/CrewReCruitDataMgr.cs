// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : DM-PC092
// Created  : 11/20/2017 5:02:26 PM
// **********************************************************************

using UniRx;

public sealed partial class CrewReCruitDataMgr
{
    // 初始化
    private void LateInit()
    {
        SystemTimeManager.Instance.OnChangeNextDay += OnChangeNextDay;
    }
    
    public void OnDispose(){
        SystemTimeManager.Instance.OnChangeNextDay -= OnChangeNextDay;
    }

    void OnChangeNextDay() {
        ProxyCrewReCruit.crewCurrencyAddTimes = 50;
        FireData();
    }
}
