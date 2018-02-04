// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : xjd
// Created  : 12/19/2017 11:25:11 AM
// **********************************************************************

using UniRx;
using AppDto;

public sealed partial class GuideMainViewDataMgr
{
    // 初始化
    private void LateInit()
    {
        _disposable.Add(NotifyListenerRegister.RegistListener<GuideInfoNotify>(HandleGuideInfoNotify));
    }
    
    public void OnDispose(){
            
    }

    private void HandleGuideInfoNotify(GuideInfoNotify notify)
    {
        _data.UpdateGuideInfo(notify);
        FireData();
    }
}
