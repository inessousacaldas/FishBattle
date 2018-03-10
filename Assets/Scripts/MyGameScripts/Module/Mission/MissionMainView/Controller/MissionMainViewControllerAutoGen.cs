// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  MissionMainViewControllerAutoGen.cs
// this file is generate by tool
// **********************************************************************

using UniRx;

public partial interface IMissionMainViewController : ICloseView
{
    UniRx.IObservable<Unit> Onclose_UIButtonClick { get; }
    UniRx.IObservable<Unit> OnDropBtn_UIButtonClick { get; }
    UniRx.IObservable<Unit> OnGoonBtn_UIButtonClick { get; }
    UniRx.IObservable<Unit> OnCanAccessBtn_UIButtonClick { get; }
    UniRx.IObservable<Unit> OnRecordBtn_UIButtonClick { get; }
    UniRx.IObservable<Unit> OnBlackbgBtn_UIButtonClick { get; }
}

public partial class MissionMainViewController:FRPBaseController<
    MissionMainViewController
    , MissionMainView
    , IMissionMainViewController
    , IMissionData>
    , IMissionMainViewController
    {
    //机器自动生成的事件订阅
    protected override void RegistEvent()
    {
        close_UIButtonEvt = View.close_UIButton.AsObservable();
        DropBtn_UIButtonEvt = View.DropBtn_UIButton.AsObservable();
        GoonBtn_UIButtonEvt = View.GoonBtn_UIButton.AsObservable();
        CanAccessBtn_UIButtonEvt = View.CanAccessBtn_UIButton.AsObservable();
        RecordBtn_UIButtonEvt = View.RecordBtn_UIButton.AsObservable();
        BlackbgBtn_UIButtonEvt = View.blackBG_UIButton.AsObservable();
    }

    protected override void RemoveEvent()
    {
        close_UIButtonEvt = close_UIButtonEvt.CloseOnceNull();
        DropBtn_UIButtonEvt = DropBtn_UIButtonEvt.CloseOnceNull();
        GoonBtn_UIButtonEvt = GoonBtn_UIButtonEvt.CloseOnceNull();
        CanAccessBtn_UIButtonEvt = CanAccessBtn_UIButtonEvt.CloseOnceNull();
        RecordBtn_UIButtonEvt = RecordBtn_UIButtonEvt.CloseOnceNull();
        BlackbgBtn_UIButtonEvt = BlackbgBtn_UIButtonEvt.CloseOnceNull();
    }

    private Subject<Unit> close_UIButtonEvt;
    public UniRx.IObservable<Unit> Onclose_UIButtonClick{
        get {return close_UIButtonEvt;}
    }

    private Subject<Unit> DropBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnDropBtn_UIButtonClick { get{ return DropBtn_UIButtonEvt; } }

    private Subject<Unit> GoonBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnGoonBtn_UIButtonClick { get { return GoonBtn_UIButtonEvt; } }

    private Subject<Unit> CanAccessBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnCanAccessBtn_UIButtonClick { get { return CanAccessBtn_UIButtonEvt; } }

    private Subject<Unit> RecordBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnRecordBtn_UIButtonClick { get { return RecordBtn_UIButtonEvt; } }

    private Subject<Unit> BlackbgBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnBlackbgBtn_UIButtonClick { get { return BlackbgBtn_UIButtonEvt; } }

}
