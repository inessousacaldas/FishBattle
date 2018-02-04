// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  FriendShowViewControllerAutoGen.cs
// this file is generate by tool
// **********************************************************************

using System;
using UniRx;

public partial interface IFriendShowViewController
{
     UniRx.IObservable<Unit> OnEggBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnSupportBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnFlowerBtn_UIButtonClick{get;}

}

public partial class FriendShowViewController:MonoViewController<FriendShowView>
    , IFriendShowViewController
    {
	    //机器自动生成的事件订阅
        protected override void RegistEvent ()
        {
    EggBtn_UIButtonEvt = View.EggBtn_UIButton.AsObservable();
    SupportBtn_UIButtonEvt = View.SupportBtn_UIButton.AsObservable();
    FlowerBtn_UIButtonEvt = View.FlowerBtn_UIButton.AsObservable();

        }
        
        protected override void RemoveEvent()
        {
        EggBtn_UIButtonEvt = EggBtn_UIButtonEvt.CloseOnceNull();
        SupportBtn_UIButtonEvt = SupportBtn_UIButtonEvt.CloseOnceNull();
        FlowerBtn_UIButtonEvt = FlowerBtn_UIButtonEvt.CloseOnceNull();

        }
        
            private Subject<Unit> EggBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnEggBtn_UIButtonClick{
        get {return EggBtn_UIButtonEvt;}
    }

    private Subject<Unit> SupportBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnSupportBtn_UIButtonClick{
        get {return SupportBtn_UIButtonEvt;}
    }

    private Subject<Unit> FlowerBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnFlowerBtn_UIButtonClick{
        get {return FlowerBtn_UIButtonEvt;}
    }


    }
