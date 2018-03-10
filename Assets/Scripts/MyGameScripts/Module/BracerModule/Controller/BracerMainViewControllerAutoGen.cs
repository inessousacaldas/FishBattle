// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  BracerMainViewControllerAutoGen.cs
// this file is generate by tool
// **********************************************************************

using UniRx;

public partial interface IBracerMainViewController : ICloseView
{
     UniRx.IObservable<Unit> OnRewardBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnNPC_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnCloseBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnTipsBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnLvViewBtn_UIWidgetClick { get; }

}

public partial class BracerMainViewController : FRPBaseController<
    BracerMainViewController
    , BracerMainView
    , IBracerMainViewController
    , IBracerMainViewData>
    , IBracerMainViewController
{
    //机器自动生成的事件订阅
    protected override void RegistEvent()
    {
        RewardBtn_UIButtonEvt = View.RewardBtn_UIButton.AsObservable();
        NPC_UIButtonEvt = View.NPC_UIButton.AsObservable();
        Btn_UIButtonEvt = View.Btn_UIButton.AsObservable();
        CloseBtn_UIButtonEvt = View.CloseBtn_UIButton.AsObservable();
        TipsBtn_UIButtonEvt = View.TipsBtn_UIButton.AsObservable();
        LvViewBtn_UIButtonEvt = View.LvViewBtn_UUIButton.AsObservable();

    }

    protected override void RemoveEvent()
    {
        RewardBtn_UIButtonEvt = RewardBtn_UIButtonEvt.CloseOnceNull();
        NPC_UIButtonEvt = NPC_UIButtonEvt.CloseOnceNull();
        Btn_UIButtonEvt = Btn_UIButtonEvt.CloseOnceNull();
        CloseBtn_UIButtonEvt = CloseBtn_UIButtonEvt.CloseOnceNull();
        TipsBtn_UIButtonEvt = TipsBtn_UIButtonEvt.CloseOnceNull();
        LvViewBtn_UIButtonEvt = LvViewBtn_UIButtonEvt.CloseOnceNull();

    }

    private Subject<Unit> RewardBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnRewardBtn_UIButtonClick
    {
        get { return RewardBtn_UIButtonEvt; }
    }

    private Subject<Unit> NPC_UIButtonEvt;
    public UniRx.IObservable<Unit> OnNPC_UIButtonClick
    {
        get { return NPC_UIButtonEvt; }
    }

    private Subject<Unit> Btn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnBtn_UIButtonClick
    {
        get { return Btn_UIButtonEvt; }
    }

    private Subject<Unit> CloseBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnCloseBtn_UIButtonClick
    {
        get { return CloseBtn_UIButtonEvt; }
    }

    private Subject<Unit> TipsBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnTipsBtn_UIButtonClick
    {
        get { return TipsBtn_UIButtonEvt; }
    }
    private Subject<Unit> LvViewBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnLvViewBtn_UIWidgetClick{
        get { return LvViewBtn_UIButtonEvt; }
    }
}
