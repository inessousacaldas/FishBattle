// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : xush
// Created  : 8/25/2017 6:05:18 PM
// **********************************************************************

using AppDto;
using UniRx;

public sealed partial class QuartzDataMgr
{
    // 初始化
    private void LateInit()
    {
        
    }
    
    public void OnDispose(){
            
    }

    public OrbmentInfoDto GetCurOrbmentInfoDto { get { return _data.GetCurOrbmentInfoDto; } }
}
