// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  RedPackItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using AppDto;

public partial class RedPackItemController
{
    private IDisposable _disposable;
    
    //private List<RedPackDetailDto> _redPacktemData = new List<RedPackDetailDto>();
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
        _disposable = _disposable.DisposeNotNull();
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }
    //红包信息
    public void SetItemInfo(RedPackDetailDto data)//RedPack.RedPackType type
    {
        _view.PlayerName_UILabel.text = data.playerName;
        if (data.open == false)
        {
            _view.word_UILabel.text = string.Empty;
            _view.Open_Sprite.spriteName = "rp_1";
        }
        else
        {
            _view.word_UILabel.text = data.word;
            _view.Open_Sprite.spriteName = "rp_1_open";
        }
    }
}
