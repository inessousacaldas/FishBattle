// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  RankingOptionBtnController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using AppDto;
using System;
using UniRx;
using UnityEngine;

public interface IRankingOptionBtn
{
    UniRx.IObservable<int> GetClickHandler { get; }
}
public partial class RankingOptionBtnController
{

    private int _idx;

    private Subject<int> _clickEvt = new Subject<int>();
    public UniRx.IObservable<int> GetClickHandler { get { return _clickEvt; } }

    private Rankings _rankings;

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
 
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        EventDelegate.Add(_view.RankingOptionBtn_UIButton.onClick, () => { _clickEvt.OnNext(_idx); });
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
        SetColor(b);
    }

    public void SetArrowAngles(bool b)
    {
        _view.Arrow_UISprite.transform.localEulerAngles =
            b ? new Vector3(0f, 0f, 90f) : new Vector3(0f, 0f, 0f);
    }
    private void SetColor(bool b)
    {
        _view.BtnNameLb_UILabel.text = _rankings.name.WrapColor(b ? ColorConstantV3.Color_White_Str : ColorConstantV3.Color_Black_Str);
    }
    public void SetRankMenuBtn(Rankings data, bool isSecond = false)
    {
        _rankings = data;
        _view.BtnNameLb_UILabel.text = data.name.ToString();
        _view.Arrow_UISprite.gameObject.SetActive(!isSecond);
        transform.localScale = isSecond ?
            new Vector3(0.9f, 0.9f, 0.9f)
            : new Vector3(1f, 1f, 1f);
        SetColor(false);
    }

    public void HideArrow()
    {
        _view.Arrow_UISprite.gameObject.SetActive(false);
    }

}
