// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  CopyMissionViewControllerAutoGen.cs
// this file is generate by tool
// **********************************************************************

using UniRx;

public partial interface ICopyMissionViewController : ICloseView
{
    UniRx.IObservable<Unit> OnEnter_UIButtonClick{get;}
    UniRx.IObservable<Unit> OnClose_UIButtonClick { get; }

    UniRx.IObservable<Unit> OnNormalButton_ButtonClick { get; }
    UniRx.IObservable<Unit> OnEliteButton_ButtonClick { get; }
}

public partial class CopyMissionViewController:FRPBaseController<
    CopyMissionViewController
    , CopyMissionView
    , ICopyMissionViewController
    , ICopyPanelData>
    , ICopyMissionViewController
{
	//机器自动生成的事件订阅
    protected override void RegistEvent ()
    {
        Enter_UIButtonEvt = View.Enter_UIButton.AsObservable();
        OnClose_UIButtonEvt = View.OnClose_UIButton.AsObservable();
        OnNormalButton_UIButtonEvt = View.NormalButton_UIButton.AsObservable();
        OnEliteButtonEvt_UIButtonEvt = View.EliteButton_UIButton.AsObservable();
    }
        
    protected override void RemoveEvent()
    {
        Enter_UIButtonEvt = Enter_UIButtonEvt.CloseOnceNull();
        OnClose_UIButtonEvt = OnClose_UIButtonEvt.CloseOnceNull();
        OnNormalButton_UIButtonEvt = OnNormalButton_UIButtonEvt.CloseOnceNull();
        OnEliteButtonEvt_UIButtonEvt = OnEliteButtonEvt_UIButtonEvt.CloseOnceNull();
    }
        
    private Subject<Unit> Enter_UIButtonEvt;
    public UniRx.IObservable<Unit> OnEnter_UIButtonClick{
        get {return Enter_UIButtonEvt;}
    }

    private Subject<Unit> OnClose_UIButtonEvt;
    public UniRx.IObservable<Unit> OnClose_UIButtonClick {
        get { return OnClose_UIButtonEvt; }
    }

    private Subject<Unit> OnNormalButton_UIButtonEvt;
    public UniRx.IObservable<Unit> OnNormalButton_ButtonClick {
        get { return OnNormalButton_UIButtonEvt; }
    }

    private Subject<Unit> OnEliteButtonEvt_UIButtonEvt;
    public UniRx.IObservable<Unit> OnEliteButton_ButtonClick
    {
        get { return OnEliteButtonEvt_UIButtonEvt; }
    }



}
