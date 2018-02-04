// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : DM-PC092
// Created  : 1/22/2018 3:17:47 PM
// **********************************************************************

using AppDto;
using UniRx;

namespace StaticInit
{
    public partial class StaticInit
    {
        private StaticDispose.StaticDelegateRunner misTremChallengeConfirmDataMgr = new StaticDispose.StaticDelegateRunner(
            () => { var mgr = TremChallengeConfirmDataMgr.DataMgr; });

    }
}

public sealed partial class TremChallengeConfirmDataMgr
{
    // 初始化
    private void LateInit()
    {
        _disposable.Add(NotifyListenerRegister.RegistListener<TeamChallengeConfirmNotify>(_data.TeamChallengeConfirmNotify));
        _disposable.Add(NotifyListenerRegister.RegistListener<TeamMemberConfirmNotify>(_data.TeamMemberConfirmNotify));
        _disposable.Add(NotifyListenerRegister.RegistListener<CopySyncConfirmNotify>(_data.CopySyncConfirmPanel));
    }
    
    private void OnDispose(){
            
    }
}
