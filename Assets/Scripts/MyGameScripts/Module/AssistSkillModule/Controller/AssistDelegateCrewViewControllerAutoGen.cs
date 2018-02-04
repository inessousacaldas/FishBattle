// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  AssistDelegateCrewViewControllerAutoGen.cs
// this file is generate by tool
// **********************************************************************

using System;
using UniRx;

public partial interface IAssistDelegateCrewViewController
{
     UniRx.IObservable<Unit> OnCloseBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnConfirmBtn_UIButtonClick{get;}

}

public partial class AssistDelegateCrewViewController:MonoViewController<AssistDelegateCrewView>
    , IAssistDelegateCrewViewController
    {
	    //机器自动生成的事件订阅
        protected override void RegistEvent ()
        {
    CloseBtn_UIButtonEvt = View.CloseBtn_UIButton.AsObservable();
    ConfirmBtn_UIButtonEvt = View.ConfirmBtn_UIButton.AsObservable();

        }
        
        protected override void RemoveEvent()
        {
        CloseBtn_UIButtonEvt = CloseBtn_UIButtonEvt.CloseOnceNull();
        ConfirmBtn_UIButtonEvt = ConfirmBtn_UIButtonEvt.CloseOnceNull();

        }
        
            private Subject<Unit> CloseBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnCloseBtn_UIButtonClick{
        get {return CloseBtn_UIButtonEvt;}
    }

    private Subject<Unit> ConfirmBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnConfirmBtn_UIButtonClick{
        get {return ConfirmBtn_UIButtonEvt;}
    }


    }
