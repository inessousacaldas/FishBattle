// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  PlayerChangeNameControllerAutoGen.cs
// this file is generate by tool
// **********************************************************************

using System;
using UniRx;

public partial interface IPlayerChangeNameController
{
     UniRx.IObservable<Unit> OnLeftBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnRightBtn_UIButtonClick{get;}

}

public partial class PlayerChangeNameController:MonoViewController<PlayerChangeName>
    , IPlayerChangeNameController
    {
	    //机器自动生成的事件订阅
        protected override void RegistEvent ()
        {
    LeftBtn_UIButtonEvt = View.LeftBtn_UIButton.AsObservable();
    RightBtn_UIButtonEvt = View.RightBtn_UIButton.AsObservable();

        }
        
        protected override void RemoveEvent()
        {
        LeftBtn_UIButtonEvt = LeftBtn_UIButtonEvt.CloseOnceNull();
        RightBtn_UIButtonEvt = RightBtn_UIButtonEvt.CloseOnceNull();

        }
        
            private Subject<Unit> LeftBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnLeftBtn_UIButtonClick{
        get {return LeftBtn_UIButtonEvt;}
    }

    private Subject<Unit> RightBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnRightBtn_UIButtonClick{
        get {return RightBtn_UIButtonEvt;}
    }


    }
