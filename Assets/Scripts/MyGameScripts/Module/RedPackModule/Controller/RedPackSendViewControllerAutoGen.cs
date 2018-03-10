// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  RedPackSendViewControllerAutoGen.cs
// this file is generate by tool
// **********************************************************************

using UniRx;

public partial interface IRedPackSendViewController : ICloseView
{
     UniRx.IObservable<Unit> OnRedPack_Send_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnCloseBtn_UIButtonClick { get; }

}

public partial class RedPackSendViewController : FRPBaseController<
    RedPackSendViewController
    , RedPackSendView
    , IRedPackSendViewController
    , IRedPackData>
    , IRedPackSendViewController
{
    //机器自动生成的事件订阅
    protected override void RegistEvent()
    {
        RedPack_Send_UIButtonEvt = View.RedPack_Send_UIButton.AsObservable();
        CloseBtn_UIButtonEvt = View.CloseBtn_UIButton.AsObservable();
    }

    protected override void RemoveEvent()
    {
        RedPack_Send_UIButtonEvt = RedPack_Send_UIButtonEvt.CloseOnceNull();
        CloseBtn_UIButtonEvt = CloseBtn_UIButtonEvt.CloseOnceNull();
    }

    private Subject<Unit> RedPack_Send_UIButtonEvt;
    public UniRx.IObservable<Unit> OnRedPack_Send_UIButtonClick
    {
        get { return RedPack_Send_UIButtonEvt; }
    }
    private Subject<Unit> CloseBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnCloseBtn_UIButtonClick
    {
        get { return CloseBtn_UIButtonEvt; }
    }

}
