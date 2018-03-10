// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  ChatInfoViewControllerAutoGen.cs
// this file is generate by tool
// **********************************************************************

using UniRx;

public partial interface ISocialityViewController : ICloseView
{
     UniRx.IObservable<Unit> OnHideBtnClick{get;}
     
     UniRx.IObservable<Unit> OnRedPacketRemind_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnHideBtn_UIButtonClick{get;}

}

public partial class SocialityViewController:FRPBaseController<
    SocialityViewController
    , ChatInfoView
    , ISocialityViewController
    , ISocialityData>
    , ISocialityViewController
    {
	    //机器自动生成的事件订阅
        protected override void RegistEvent ()
        {
    HideBtnEvt = View.HideBtn.AsObservable();
    RedPacketRemind_UIButtonEvt = View.RedPacketRemind_UIButton.AsObservable();
    HideBtn_UIButtonEvt = View.HideBtn_UIButton.AsObservable();

        }
        
        protected override void RemoveEvent()
        {
        HideBtnEvt = HideBtnEvt.CloseOnceNull();
        RedPacketRemind_UIButtonEvt = RedPacketRemind_UIButtonEvt.CloseOnceNull();
        HideBtn_UIButtonEvt = HideBtn_UIButtonEvt.CloseOnceNull();

        }
        
            private Subject<Unit> HideBtnEvt;
    public UniRx.IObservable<Unit> OnHideBtnClick{
        get {return HideBtnEvt;}
    }

    private Subject<Unit> SendBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnSendBtn_UIButtonClick{
        get {return SendBtn_UIButtonEvt;}
    }

    private Subject<Unit> SpeedOrInputBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnSpeedOrInputBtn_UIButtonClick{
        get {return SpeedOrInputBtn_UIButtonEvt;}
    }

    private Subject<Unit> RedPacketRemind_UIButtonEvt;
    public UniRx.IObservable<Unit> OnRedPacketRemind_UIButtonClick{
        get {return RedPacketRemind_UIButtonEvt;}
    }

    private Subject<Unit> HideBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnHideBtn_UIButtonClick{
        get {return HideBtn_UIButtonEvt;}
    }


    }
