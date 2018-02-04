// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : CL-PC080
// Created  : 07/04/2017 20:48:58
// **********************************************************************

using UniRx;

public sealed partial class WorldMapDataMgr
{
    // 初始化
    private void LateInit()
    {
        
    }
    
    public void OnDispose(){
        _data.Dispose();
        stream.DoOnCompleted(null); 
    }
}
