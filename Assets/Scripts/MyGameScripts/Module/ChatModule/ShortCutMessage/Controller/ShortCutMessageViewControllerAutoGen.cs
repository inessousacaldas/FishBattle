// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  ShortCutMessageViewControllerAutoGen.cs
// this file is generate by tool
// **********************************************************************

using UniRx;

public partial interface IShortCutMessageViewController : ICloseView
{
     UniRx.IObservable<Unit> OnPlayerAnimBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnShotCutBtn_UIButtonClick{get;}

}

public partial class ShortCutMessageViewController:FRPBaseController<
    ShortCutMessageViewController
    , ShortCutMessageView
    , IShortCutMessageViewController
    , IShortCutMessageViewData>
    , IShortCutMessageViewController
    {
	    //机器自动生成的事件订阅
        protected override void RegistEvent ()
        {
    PlayerAnimBtn_UIButtonEvt = View.PlayerAnimBtn_UIButton.AsObservable();
    ShotCutBtn_UIButtonEvt = View.ShotCutBtn_UIButton.AsObservable();

        }
        
        protected override void RemoveEvent()
        {
        PlayerAnimBtn_UIButtonEvt = PlayerAnimBtn_UIButtonEvt.CloseOnceNull();
        ShotCutBtn_UIButtonEvt = ShotCutBtn_UIButtonEvt.CloseOnceNull();

        }
        
            private Subject<Unit> PlayerAnimBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnPlayerAnimBtn_UIButtonClick{
        get {return PlayerAnimBtn_UIButtonEvt;}
    }

    private Subject<Unit> ShotCutBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnShotCutBtn_UIButtonClick{
        get {return ShotCutBtn_UIButtonEvt;}
    }


    }
