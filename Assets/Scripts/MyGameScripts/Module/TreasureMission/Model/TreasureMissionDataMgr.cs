// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : DM-PC092
// Created  : 12/13/2017 4:15:20 PM
// **********************************************************************

using AppDto;
using UniRx;

public sealed partial class TreasureMissionDataMgr
{
    // 初始化
    private void LateInit()
    {
        _disposable.Add(NotifyListenerRegister.RegistListener<HighTreasuryNotify>(_data.HighTreasuryNotify));
        _disposable.Add(NotifyListenerRegister.RegistListener<HighTreasueyChangeNotify>(_data.UpdataDiamondsNotify));
        SystemTimeManager.Instance.OnChangeNextDay += _data.OnChangeNextDay;
    }
    
    public void OnDispose(){
        SystemTimeManager.Instance.OnChangeNextDay -= _data.OnChangeNextDay;
    }
}
