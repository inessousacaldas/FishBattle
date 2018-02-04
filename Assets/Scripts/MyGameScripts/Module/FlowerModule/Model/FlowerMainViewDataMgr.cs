// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : xjd
// Created  : 1/13/2018 10:18:38 AM
// **********************************************************************

using UniRx;
using AppDto;
using System;
using System.Collections.Generic;

public sealed partial class FlowerMainViewDataMgr
{
    // 初始化
    private void LateInit()
    {
        
    }
    
    private void OnDispose(){
            
    }

    public void SetCurFriendId(long id)
    {
        DataMgr._data.CurFriendId = id;
        FireData();
    }

    public void SetCurFlowerId(int id)
    {
        DataMgr._data.CurFlowerId = id;
        FireData();
    }

    public void SetCurFlowerCout(int count)
    {
        DataMgr._data.CurFlowerCount = count;
        FireData();
    }

    public void SetCurFlowerContent(string str)
    {
        DataMgr._data.CurFlowerContent = str;
    }

    //搜索之后 删除搜索内容重新拉好友列表
    public void ResetFriendList()
    {
        DataMgr._data.AddAndSort();
        FireData();
    }

    public void RefreshFriendDegree(long friendId, int degree)
    {
        var friendData = DataMgr._data.SearchList.Find(x => x.friendId == friendId);
        if (friendData != null)
        {
            friendData.degree = degree;
            FireData();
        }
    }
}
