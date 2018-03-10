// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  BracerLvItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using AppDto;
using UniRx;
using System.Collections.Generic;

public partial class BracerLvItemController
{
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
 
    }

    private int _id;

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        OnBracerLvItem_UIButtonClick.Subscribe(_ =>
        {
            clickItemStream.OnNext(_id);
        });
    }

    protected override void OnDispose()
    {

    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    public void UpdateView(BracerGrade dto)
    {
        View.Icon_UISprite.spriteName = dto.icon;
        View.Lable_UILabel.text = dto.name;
        _id = dto.id;
    }

    public void SetIsChose(bool b)
    {
        View.ChoseBg_UISprite.gameObject.SetActive(b);
    }

    public void SetIsGrey(bool b)
    {
        View.Icon_UISprite.isGrey = b;
    }

    readonly UniRx.Subject<int> clickItemStream = new UniRx.Subject<int>();
    public UniRx.IObservable<int> OnClickItemStream
    {
        get { return clickItemStream; }
    }

}
