// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : DM-PC092
// Created  : 2/27/2018 4:27:42 PM
// **********************************************************************

using UniRx;
using AppDto;
using System;
using System.Collections.Generic;

public sealed partial class RedPackDataMgr
{
    private void HandleRedPackNotify(RedPackNotify notify)
    {
        if (notify == null) return;
        var _redPackDto = _data._redPackDic[(RedPackChannelType)notify.channelId];
    }
    // 初始化
    private void LateInit()
    {
        _disposable.Add(NotifyListenerRegister.RegistListener<RedPackNotify>(HandleRedPackNotify));
    }

    private void OnDispose()
    {

    }
    public bool IsRedPackGet()
    {
        var IsGetRedPack = _data.IsOpen.open;
        return IsGetRedPack;
    }
    public string  InputWord()
    {
         var str = _data.IsOpen.word;
        return str;
    }
    //public string 
}
