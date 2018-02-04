// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : DM-PC092
// Created  : 12/12/2017 11:42:28 AM
// **********************************************************************

using UniRx;
using AppDto;


namespace StaticInit
{
    public partial class StaticInit
    {
        private StaticDispose.StaticDelegateRunner misRewardTipDataMgr = new StaticDispose.StaticDelegateRunner(
            () => { var mgr = MisRewardTipDataMgr.DataMgr; });

    }
}

public sealed partial class MisRewardTipDataMgr
{
    // 初始化
    private void LateInit()
    {
        _disposable.Add(NotifyListenerRegister.RegistListener<ItemTipsNotify>(_data.UpdateItemTipNotify));
        _disposable.Add(BattleDataManager.Stream.Select((data, state) => data.battleState).Subscribe(stateTuple =>
        {
            _data.UpdateBattleTip();
        }));
    }
    
    public void OnDispose(){
            
    }
}
