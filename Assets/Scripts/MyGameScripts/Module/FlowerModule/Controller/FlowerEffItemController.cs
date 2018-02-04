// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  FlowerEffItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using UniRx;

public partial class FlowerEffItemController
{
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
 
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        OnBg_UIButtonClick.Subscribe(_ =>
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

    private int _id = 0;
    public void UpdateView(int id, int count, string name, bool isChose)
    {
        _id = id;
        View.Crook_UISprite.enabled = isChose;
        View.Name_UILabel.text = name;
        View.Num_UILabel.text = string.Format("{0}朵以上", count);
    }

    readonly UniRx.Subject<int> clickItemStream = new UniRx.Subject<int>();
    public UniRx.IObservable<int> OnClickItemStream
    {
        get { return clickItemStream; }
    }
}
