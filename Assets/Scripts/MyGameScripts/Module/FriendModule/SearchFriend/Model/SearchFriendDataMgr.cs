// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : xjd
// Created  : 10/13/2017 6:07:28 PM
// **********************************************************************

using UniRx;

public sealed partial class SearchFriendDataMgr
{
    // 初始化
    private void LateInit()
    {
        
    }
    
    public void OnDispose(){
            
    }

    public void ClearSearchList()
    {
        _data.SearchItemList.Clear();
    }
}
