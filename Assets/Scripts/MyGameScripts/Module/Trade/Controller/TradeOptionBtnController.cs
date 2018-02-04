// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  TradeOptionBtnController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using AppDto;
using UniRx;
using UnityEngine;

public interface ITradeOptionBtn
{
    UniRx.IObservable<int> GetClickHandler { get; }
}

public partial class TradeOptionBtnController
{
    private int _idx;
    
    private Subject<int> _clickEvt = new Subject<int>();
    public UniRx.IObservable<int> GetClickHandler{get { return _clickEvt; }}
    
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        _view.Select.SetActive(false);
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        EventDelegate.Add(_view.TradeOptionBtn_UIButton.onClick, () => {_clickEvt.OnNext(_idx); });
    }

    protected override void OnDispose()
    {

    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    public void IsSelect(bool b)
    {
        _view.Select.SetActive(b);
        SetArrowAngles(b);
    }

    public void SetArrowAngles(bool b)
    {
        _view.Arrow_UISprite.transform.localEulerAngles =
            b ? new Vector3(0f, 0f, 90f) : new Vector3(0f, 0f, 0f);
    }

    //二级按钮不显示showArrow,并且大小缩放为90%
    public void SetTradeMenuBtn(TradeMenu data, bool isSecond = false)
    {
        _view.BtnNameLb_UILabel.text = data.name;
        _view.Arrow_UISprite.gameObject.SetActive(!isSecond);
        _view.LittleIcon_UISprite.gameObject.SetActive(false);
        transform.localScale = isSecond ? 
            new Vector3(0.9f, 0.9f, 0.9f)
            :new Vector3(1f, 1f, 1f);
    }

    public void HideArrow()
    {
        _view.Arrow_UISprite.gameObject.SetActive(false);
    }
}
