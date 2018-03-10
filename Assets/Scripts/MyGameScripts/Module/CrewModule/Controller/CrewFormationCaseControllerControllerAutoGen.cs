// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  CrewFormationCaseControllerAutoGen.cs
// this file is generate by tool
// **********************************************************************

using System;
using UniRx;

public partial interface ICrewFormationCaseController
{
     UniRx.IObservable<Unit> OnFormationBtn_UIButtonClick{get;}
    UniRx.IObservable<Unit> CaseItemBtn_UIButtonClick{get;}
}

public partial class CrewFormationCaseController:MonolessViewController<CrewFormationCaseItem>
    , ICrewFormationCaseController
    {
	    //机器自动生成的事件订阅
        protected override void InitReactiveEvents ()
        {
            FormationBtn_UIButtonEvt = View.FormationBtn_UIButton.AsObservable();
            CaseItemBtn_UIButtonEvt = View.CrewFormationCaseItem_UIButton.AsObservable();
        }
        
        protected override void ClearReactiveEvents()
        {
            FormationBtn_UIButtonEvt = FormationBtn_UIButtonEvt.CloseOnceNull();
            CaseItemBtn_UIButtonEvt = CaseItemBtn_UIButtonEvt.CloseOnceNull();
        }
        
        private Subject<Unit> FormationBtn_UIButtonEvt;
        public UniRx.IObservable<Unit> OnFormationBtn_UIButtonClick{
            get {return FormationBtn_UIButtonEvt;}
    }

        private Subject<Unit> CaseItemBtn_UIButtonEvt;
        public UniRx.IObservable<Unit> CaseItemBtn_UIButtonClick
        {
            get { return CaseItemBtn_UIButtonEvt; }
        }

    }
