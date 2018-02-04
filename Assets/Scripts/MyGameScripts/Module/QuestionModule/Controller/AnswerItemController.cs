// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  TxtAnswerItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using UniRx;

public partial interface IAnswerItemController
{
    UniRx.IObservable<int> GetOnClickHandler { get; }
}

public partial class AnswerItemController
{
    private IDisposable _disposable; 

    private Subject<int> _onClick = new Subject<int>();
    public UniRx.IObservable<int> GetOnClickHandler { get { return _onClick; } }

    private int _idx;
    private const int _height = 144;
    private const int _width = 135;
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        _disposable = TxtAnswerItem_UIButtonEvt.Subscribe(_ => { _onClick.OnNext(_idx); });
    }

    protected override void OnDispose()
    {
        _disposable.Dispose();
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    public void SetItemInfo(int num, string txt)
    {
        _idx = num;
        var list = txt.Split('|');
        _view.TxtLb_UILabel.text = list[0];
        _view.NumLb_UILabel.text = String.Format("{0}.", num);
        _view.MarkSpr_UISprite.gameObject.SetActive(false);
        _view.Texture_UITexture.gameObject.SetActive(false);
        if (list.Length == 2 && !string.IsNullOrEmpty(list[1]))
            SetTexture(list[1]);
    }

    public void IsRight(bool b)
    {
        _view.MarkSpr_UISprite.gameObject.SetActive(true);
        _view.MarkSpr_UISprite.spriteName = b ? "chose_dui_fg" : "wrong";
    }

    public void SetTexture(string texture)
    {
        _view.MarkSpr_UISprite.gameObject.SetActive(false);
        _view.Texture_UITexture.gameObject.SetActive(true);
        UIHelper.SetUITexture(_view.Texture_UITexture, texture, callback: () =>
        {
            var height = (int) (_view.Texture_UITexture.height*0.6);
            var width = (int) (_view.Texture_UITexture.width * 0.6);
            _view.Texture_UITexture.height = height;
            _view.Texture_UITexture.width = width;
        });
    }

    public void Select()
    {
        _view.BackGround_UISprite.spriteName = "Answer_bg_mysely";
        _view.AnswerItem_UIButton.normalSprite = "Answer_bg_mysely";
    }
}
