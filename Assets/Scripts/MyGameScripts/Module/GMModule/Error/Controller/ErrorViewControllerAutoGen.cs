// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  ErrorViewControllerAutoGen.cs
// this file is generate by tool
// **********************************************************************

using System;
using UniRx;

public partial interface IErrorViewController
{
     UniRx.IObservable<Unit> OnbtnCopy_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnCloseBtn_UIButtonClick{get;}

}

public partial class ErrorViewController:MonoViewController<ErrorView>
    , IErrorViewController
    {
	    //机器自动生成的事件订阅
        protected override void RegistEvent ()
        {
    btnCopy_UIButtonEvt = View.btnCopy_UIButton.AsObservable();
    CloseBtn_UIButtonEvt = View.CloseBtn_UIButton.AsObservable();

        }
        
        protected override void RemoveEvent()
        {
        btnCopy_UIButtonEvt = btnCopy_UIButtonEvt.CloseOnceNull();
        CloseBtn_UIButtonEvt = CloseBtn_UIButtonEvt.CloseOnceNull();

        }
        
            private Subject<Unit> btnCopy_UIButtonEvt;
    public UniRx.IObservable<Unit> OnbtnCopy_UIButtonClick{
        get {return btnCopy_UIButtonEvt;}
    }

    private Subject<Unit> CloseBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnCloseBtn_UIButtonClick{
        get {return CloseBtn_UIButtonEvt;}
    }


    }
