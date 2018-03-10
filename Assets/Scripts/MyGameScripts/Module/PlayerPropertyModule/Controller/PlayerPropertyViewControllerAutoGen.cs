// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  PlayerPropertyViewControllerAutoGen.cs
// this file is generate by tool
// **********************************************************************

using UniRx;

public partial interface IPlayerPropertyViewController : ICloseView
{
     UniRx.IObservable<Unit> OnChangeNameBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnAppellationBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnProfessionBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnNoteBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnAdvancedPropertyBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnCloseButton_UIButtonClick{get;}
    UniRx.IObservable<Unit> BasePropertyEvtClick { get; }
    UniRx.IObservable<Unit> FightPropertyEvtClick { get; }
}

public partial class PlayerPropertyViewController:FRPBaseController<
    PlayerPropertyViewController
    , PlayerPropertyView
    , IPlayerPropertyViewController
    , IPlayerPropertyData>
    , IPlayerPropertyViewController
    {
	    //机器自动生成的事件订阅
        protected override void RegistEvent ()
        {
    ChangeNameBtn_UIButtonEvt = View.ChangeNameBtn_UIButton.AsObservable();
    AppellationBtn_UIButtonEvt = View.AppellationBtn_UIButton.AsObservable();
    ProfessionBtn_UIButtonEvt = View.ProfessionBtn_UIButton.AsObservable();
    NoteBtn_UIButtonEvt = View.NoteBtn_UIButton.AsObservable();
    AdvancedPropertyBtn_UIButtonEvt = View.AdvancedPropertyBtn_UIButton.AsObservable();
    CloseButton_UIButtonEvt = View.CloseButton_UIButton.AsObservable();
        BasePropertyEvt = View.BasePropertyContainer.OnClickAsObservable();
        FightPropertyEvt = View.FightPropertyContainer.OnClickAsObservable();
    }
        
        protected override void RemoveEvent()
        {
        ChangeNameBtn_UIButtonEvt = ChangeNameBtn_UIButtonEvt.CloseOnceNull();
        AppellationBtn_UIButtonEvt = AppellationBtn_UIButtonEvt.CloseOnceNull();
        ProfessionBtn_UIButtonEvt = ProfessionBtn_UIButtonEvt.CloseOnceNull();
        NoteBtn_UIButtonEvt = NoteBtn_UIButtonEvt.CloseOnceNull();
        AdvancedPropertyBtn_UIButtonEvt = AdvancedPropertyBtn_UIButtonEvt.CloseOnceNull();
        CloseButton_UIButtonEvt = CloseButton_UIButtonEvt.CloseOnceNull();
        BasePropertyEvt = BasePropertyEvt.CloseOnceNull();
        FightPropertyEvt = FightPropertyEvt.CloseOnceNull();
        }
        
            private Subject<Unit> ChangeNameBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnChangeNameBtn_UIButtonClick{
        get {return ChangeNameBtn_UIButtonEvt;}
    }

    private Subject<Unit> AppellationBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnAppellationBtn_UIButtonClick{
        get {return AppellationBtn_UIButtonEvt;}
    }

    private Subject<Unit> ProfessionBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnProfessionBtn_UIButtonClick{
        get {return ProfessionBtn_UIButtonEvt;}
    }

    private Subject<Unit> NoteBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnNoteBtn_UIButtonClick{
        get {return NoteBtn_UIButtonEvt;}
    }

    private Subject<Unit> AdvancedPropertyBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnAdvancedPropertyBtn_UIButtonClick{
        get {return AdvancedPropertyBtn_UIButtonEvt;}
    }

    private Subject<Unit> CloseButton_UIButtonEvt;
    public UniRx.IObservable<Unit> OnCloseButton_UIButtonClick{
        get {return CloseButton_UIButtonEvt;}
    }

    private Subject<Unit> BasePropertyEvt;
    public UniRx.IObservable<Unit> BasePropertyEvtClick
    {
        get { return BasePropertyEvt; }
    }

    private Subject<Unit> FightPropertyEvt;
    public UniRx.IObservable<Unit> FightPropertyEvtClick
    {
        get { return FightPropertyEvt; }
    }


}
