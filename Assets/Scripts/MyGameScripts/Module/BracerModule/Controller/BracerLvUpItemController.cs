// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  BracerLvUpItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using AppDto;
using System.Collections.Generic;

public partial class BracerLvUpItemController
{
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
 
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {

    }

    protected override void OnDispose()
    {

    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    private Dictionary<int, CharacterAbility> _characterDic = DataCache.getDicByCls<CharacterAbility>();

    public void UpdateView(int id, float value)
    {
        //View.Icon_UISprite.
        if (_characterDic.ContainsKey(id))
            View.Label_UILabel.text = string.Format("{0}+{1}", _characterDic[id].name, value);
    }

}
