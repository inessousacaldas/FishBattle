// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  StrengthContainerViewControllerAutoGen.cs
// this file is generate by tool
// **********************************************************************

using System;
using UniRx;
using UnityEngine;

public partial class StrengthContainerViewController: MonolessViewController<StrengthContainerView>
    {
    //机器自动生成的事件订阅

    protected override void InitReactiveEvents()
    {
        CheckBtn_UIButtonEvt = View.CheckBtn_UIButton.AsObservable();
        StrengthButton_UIButtonEvt = View.StrengthButton_UIButton.AsObservable();
        TipButton_UIButtonEvt = View.TipButton_UIButton.AsObservable();
        DevelopButton_UIButtonEvt = View.DevelopButton_UIButton.AsObservable();
        DevelopEffectBtn_UIButtonEvt = View.DevelopEffectBtn_UIButton.AsObservable();
        Black_UIButtonEvt = View.BlackBG_UIButton.AsObservable();
        Sure_UIButtonEvt = View.Sure_UIButton.AsObservable();
        Cancel_UIButtonEvt = View.Cancel_UIButton.AsObservable();
        StrengthTipBtnEvt = View.StrengthTipBtn_UIButton.AsObservable();

    }

    protected override void ClearReactiveEvents()
    {
        CheckBtn_UIButtonEvt = CheckBtn_UIButtonEvt.CloseOnceNull();
        StrengthButton_UIButtonEvt = StrengthButton_UIButtonEvt.CloseOnceNull();
        TipButton_UIButtonEvt = TipButton_UIButtonEvt.CloseOnceNull();
        DevelopButton_UIButtonEvt = DevelopButton_UIButtonEvt.CloseOnceNull();
        DevelopEffectBtn_UIButtonEvt = DevelopEffectBtn_UIButtonEvt.CloseOnceNull();
        Black_UIButtonEvt = Black_UIButtonEvt.CloseOnceNull();
        Sure_UIButtonEvt = Sure_UIButtonEvt.CloseOnceNull();
        Cancel_UIButtonEvt = Cancel_UIButtonEvt.CloseOnceNull();
        StrengthTipBtnEvt = StrengthTipBtnEvt.CloseOnceNull();

    }

    private Subject<Unit> DevelopEffectBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnDevelopEffectBtn_UIButtonClick {
        get { return DevelopEffectBtn_UIButtonEvt; }
    }
    private Subject<Unit> CheckBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnCheckBtn_UIButtonClick{
        get {return CheckBtn_UIButtonEvt;}
    }

    private Subject<Unit> StrengthButton_UIButtonEvt;
    public UniRx.IObservable<Unit> OnStrengthButton_UIButtonClick{
        get {return StrengthButton_UIButtonEvt;}
    }

    private Subject<Unit> TipButton_UIButtonEvt;
    public UniRx.IObservable<Unit> OnTipButton_UIButtonClick{
        get {return TipButton_UIButtonEvt;}
    }

    private Subject<Unit> DevelopButton_UIButtonEvt;
    public UniRx.IObservable<Unit> OnDevelopButton_UIButtonClick{
        get {return DevelopButton_UIButtonEvt;}
    }

    private Subject<Unit> Black_UIButtonEvt;
    public UniRx.IObservable<Unit> OnBlackButton_UIButtonClick
    {
        get { return Black_UIButtonEvt; }
    }

    private Subject<Unit> Sure_UIButtonEvt;
    public UniRx.IObservable<Unit> OnSureButton_UIButtonClick
    {
        get { return Sure_UIButtonEvt; }
    }

    private Subject<Unit> Cancel_UIButtonEvt;
    public UniRx.IObservable<Unit> OnCancelButton_UIButtonClick
    {
        get { return Cancel_UIButtonEvt; }
    }

    private Subject<Unit> StrengthTipBtnEvt;
    public UniRx.IObservable<Unit> OnStrengthTipBtnClick
    {
        get { return StrengthTipBtnEvt; }
    }


}
