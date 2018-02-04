// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  FlowerReceiveViewControllerAutoGen.cs
// this file is generate by tool
// **********************************************************************

using System;
using UniRx;

public partial interface IFlowerReceiveViewController
{
     UniRx.IObservable<Unit> OnGiveBackBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnThxBtn_UIButtonClick{get;}

}

public partial class FlowerReceiveViewController:MonoViewController<FlowerReceiveView>
    , IFlowerReceiveViewController
    {
	    //机器自动生成的事件订阅
        protected override void RegistEvent ()
        {
    GiveBackBtn_UIButtonEvt = View.GiveBackBtn_UIButton.AsObservable();
    ThxBtn_UIButtonEvt = View.ThxBtn_UIButton.AsObservable();

        }
        
        protected override void RemoveEvent()
        {
        GiveBackBtn_UIButtonEvt = GiveBackBtn_UIButtonEvt.CloseOnceNull();
        ThxBtn_UIButtonEvt = ThxBtn_UIButtonEvt.CloseOnceNull();

        }
        
            private Subject<Unit> GiveBackBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnGiveBackBtn_UIButtonClick{
        get {return GiveBackBtn_UIButtonEvt;}
    }

    private Subject<Unit> ThxBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnThxBtn_UIButtonClick{
        get {return ThxBtn_UIButtonEvt;}
    }


    }
