// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  SpeakViewControllerAutoGen.cs
// this file is generate by tool
// **********************************************************************

using System;
using UniRx;

public partial interface ISpeakViewController:ICloseView
{
     UniRx.IObservable<Unit> OnCloseBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnSendBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnHistoryBtn_UIButtonClick{get;}

}

public partial interface ISpeakViewData
{
    
}

public partial class SpeakViewController:FRPBaseController<
    SpeakViewController
    , SpeakView
    , ISpeakViewController
    , ISpeakViewData>
    , ISpeakViewController
    {
	    //机器自动生成的事件订阅
        protected override void RegistEvent ()
        {
    CloseBtn_UIButtonEvt = View.CloseBtn_UIButton.AsObservable();
    SendBtn_UIButtonEvt = View.SendBtn_UIButton.AsObservable();
    HistoryBtn_UIButtonEvt = View.HistoryBtn_UIButton.AsObservable();

        }
        
        protected override void RemoveEvent()
        {
        CloseBtn_UIButtonEvt = CloseBtn_UIButtonEvt.CloseOnceNull();
        SendBtn_UIButtonEvt = SendBtn_UIButtonEvt.CloseOnceNull();
        HistoryBtn_UIButtonEvt = HistoryBtn_UIButtonEvt.CloseOnceNull();

        }
        
            private Subject<Unit> CloseBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnCloseBtn_UIButtonClick{
        get {return CloseBtn_UIButtonEvt;}
    }

    private Subject<Unit> SendBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnSendBtn_UIButtonClick{
        get {return SendBtn_UIButtonEvt;}
    }

    private Subject<Unit> HistoryBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnHistoryBtn_UIButtonClick{
        get {return HistoryBtn_UIButtonEvt;}
    }


    }
