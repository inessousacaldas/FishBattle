// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  CrewPassiveTypeBtnController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using UniRx;

public partial interface ICrewPassiveTypeBtnController
{
    UniRx.IObservable<int> OnClickItemStream { get; }
}

public partial class CrewPassiveTypeBtnController
{


    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {

    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        OnCrewPassiveTypeBtn_UIButtonClick.Subscribe(_ => { clickItemStream.OnNext(_idx); });
    }

    protected override void OnDispose()
    {

    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }
    private int _idx;
    public int Index { get { return _idx; } }
    public void UpdateView(int type, string name)
    {
        _idx = type;
        _view.TypeNameLbl_UILabel.text = name;
        _view.SelectSprite_UISprite.gameObject.SetActive(false);

    }
    public void SetSelected(bool isChose)
    {
        _view.SelectSprite_UISprite.gameObject.SetActive(isChose);
    }

    private UniRx.Subject<int> clickItemStream = new UniRx.Subject<int>();
    public UniRx.IObservable<int> OnClickItemStream
    {
        get { return clickItemStream; }
    }

}
