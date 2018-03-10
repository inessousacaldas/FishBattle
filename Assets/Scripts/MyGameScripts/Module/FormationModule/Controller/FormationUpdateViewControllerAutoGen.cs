// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  FormationUpdateViewControllerAutoGen.cs
// this file is generate by tool
// **********************************************************************

using UniRx;

public partial interface IFormationUpdateViewController : ICloseView
{
     IObservable<Unit> OnCloseBtn_UIButtonClick{get;}
     IObservable<Unit> OnUpgradeBtn_UIButtonClick{get;}
}

public partial class FormationUpdateViewController:FRPBaseController<
        FormationUpdateViewController
    , FormationUpdateView
    , IFormationUpdateViewController
    , ITeamFormationData>
    , IFormationUpdateViewController
    {
	    //机器自动生成的事件订阅
        protected override void RegistEvent ()
        {
    CloseBtn_UIButtonEvt = View.CloseBtn_UIButton.AsObservable();
    UpgradeBtn_UIButtonEvt = View.UpgradeBtn_UIButton.AsObservable();

        }
        
        protected override void RemoveEvent()
        {
        CloseBtn_UIButtonEvt = CloseBtn_UIButtonEvt.CloseOnceNull();
        UpgradeBtn_UIButtonEvt = UpgradeBtn_UIButtonEvt.CloseOnceNull();

        }
        
        private Subject<Unit> CloseBtn_UIButtonEvt;
        public IObservable<Unit> OnCloseBtn_UIButtonClick{
            get {return CloseBtn_UIButtonEvt;}
        }

        private Subject<Unit> UpgradeBtn_UIButtonEvt;
        public IObservable<Unit> OnUpgradeBtn_UIButtonClick{
            get {return UpgradeBtn_UIButtonEvt;}
        }    
    }
