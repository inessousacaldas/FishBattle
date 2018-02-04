// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : xush
// Created  : 1/17/2018 5:10:21 PM
// **********************************************************************

using AppDto;
using UniRx;

public sealed partial class MartialDataMgr
{
    // 初始化
    private void LateInit()
    {
        _disposable.Add(NotifyListenerRegister.RegistListener<KungfuActivityInfo>(UpdateKungFuInfo));
        _disposable.Add(NotifyListenerRegister.RegistListener<BattleReadyNotify>(OpenBattleReadyView));
    }
    
    private void OnDispose(){
            
    }

    private void UpdateKungFuInfo(KungfuActivityInfo info)
    {
        DataMgr._data.ActivityInfo = info;
        var t = info.endAt - SystemTimeManager.Instance.GetUTCTimeStamp();
        DataMgr._data.EndAtTime = (int)t /1000;
        DataMgr._data.UpdateEndTime();
        DataMgr._data.UpdateActiveState(info.state);
        //GameDebuger.Log("======KungfuActivityInfo========" + DataMgr._data.EndAtTime);
        stream.OnNext(_data);   //通知主界面刷新活动按钮状态
    }

    public IMartialData GetMartialData()
    {
        return _data;
    }

    private static void OpenBattleReadyView(BattleReadyNotify notify)
    {
        ProxyMartial.CloseMartialView();
        var controller = UIModuleManager.Instance.OpenFunModule<BattleReadyViewController>(BattleReadyView.NAME,
            UILayerType.SubModule, true, false);
        controller.OpenBattleReadyView(notify);
    }
}
