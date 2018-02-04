// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : DM-PC092
// Created  : 7/29/2017 10:26:04 AM
// **********************************************************************

using UniRx;

public sealed partial class GMDataMgr
{
    // 初始化
    private void LateInit()
    {
        
    }
    
    public void OnDispose(){
            
    }

    public GMDtoConnData DtoConnData
    {
        get { return _data.DtoConnData; }
    }
}
