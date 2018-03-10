// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  BattleDemoViewControllerAutoGen.cs
// this file is generate by tool
// **********************************************************************

using UniRx;

public partial interface IBattleDemoViewController : ICloseView
{
     UniRx.IObservable<Unit> OnCancelButton_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnRetreatButton_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnAutoButton_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnManualButton_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnBtnCommand_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnItemButton_UIButtonClick{get;}
    
    UniRx.IObservable<Unit> OnShowPanelBtn_UIButtonClick { get; }
    UniRx.IObservable<Unit> OnCancelSkillBtn_UIButtonClick { get; }
    UniRx.IObservable<Unit> OnCallButton_UIButtonClick { get; }

}

public sealed partial class BattleDataManager
{
    public partial class BattleDemoViewController : FRPBaseController<
            BattleDemoViewController
            , BattleDemoView
            , IBattleDemoViewController
            , IBattleDemoModel>
        , IBattleDemoViewController
    {
	    //机器自动生成的事件订阅
        protected override void RegistEvent ()
        {
    CancelButton_UIButtonEvt = View.CancelButton_UIButton.AsObservable();
    AutoButton_UIButtonEvt = View.AutoButton_UIButton.AsObservable();
    ManualButton_UIButtonEvt = View.ManualButton_UIButton.AsObservable();
    BtnCommand_UIButtonEvt = View.BtnCommand_UIButton.AsObservable();
    ItemButton_UIButtonEvt = View.ItemButton_UIButton.AsObservable();
            
            ShowPanelBtn_UIButtonEvt = View.ShowPanelBtn_UIButton.AsObservable();
            CancelSkillBtn_UIButtonEvt = View.CancelSkillButton_UIButton.AsObservable();
            CallButton_UIButtonEvt = View.CallButton_UIButton.AsObservable();
        }
        
        protected override void RemoveEvent()
        {
        CancelButton_UIButtonEvt = CancelButton_UIButtonEvt.CloseOnceNull();
        RetreatButton_UIButtonEvt = RetreatButton_UIButtonEvt.CloseOnceNull();
        AutoButton_UIButtonEvt = AutoButton_UIButtonEvt.CloseOnceNull();
        ManualButton_UIButtonEvt = ManualButton_UIButtonEvt.CloseOnceNull();
        BtnCommand_UIButtonEvt = BtnCommand_UIButtonEvt.CloseOnceNull();
        ItemButton_UIButtonEvt = ItemButton_UIButtonEvt.CloseOnceNull();
            
            ShowPanelBtn_UIButtonEvt = ShowPanelBtn_UIButtonEvt.CloseOnceNull();
            CancelSkillBtn_UIButtonEvt = CancelSkillBtn_UIButtonEvt.CloseOnceNull();
            CallButton_UIButtonEvt = CallButton_UIButtonEvt.CloseOnceNull();
        }
        
            private Subject<Unit> CancelButton_UIButtonEvt;
    public UniRx.IObservable<Unit> OnCancelButton_UIButtonClick{
        get {return CancelButton_UIButtonEvt;}
    }

    private Subject<Unit> RetreatButton_UIButtonEvt;
    public UniRx.IObservable<Unit> OnRetreatButton_UIButtonClick{
        get {return RetreatButton_UIButtonEvt;}
    }

    private Subject<Unit> AutoButton_UIButtonEvt;
    public UniRx.IObservable<Unit> OnAutoButton_UIButtonClick{
        get {return AutoButton_UIButtonEvt;}
    }

    private Subject<Unit> ManualButton_UIButtonEvt;
    public UniRx.IObservable<Unit> OnManualButton_UIButtonClick{
        get {return ManualButton_UIButtonEvt;}
    }

    private Subject<Unit> BtnCommand_UIButtonEvt;
    public UniRx.IObservable<Unit> OnBtnCommand_UIButtonClick{
        get {return BtnCommand_UIButtonEvt;}
    }

    private Subject<Unit> ItemButton_UIButtonEvt;
    public UniRx.IObservable<Unit> OnItemButton_UIButtonClick{
        get {return ItemButton_UIButtonEvt;}
    }

    private Subject<Unit> ShowPanelBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnShowPanelBtn_UIButtonClick { get { return ShowPanelBtn_UIButtonEvt; } }

    private Subject<Unit> CancelSkillBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnCancelSkillBtn_UIButtonClick { get { return CancelSkillBtn_UIButtonEvt; } }
    private Subject<Unit> CallButton_UIButtonEvt;
    public UniRx.IObservable<Unit> OnCallButton_UIButtonClick { get { return CallButton_UIButtonEvt; } }
    }
}
