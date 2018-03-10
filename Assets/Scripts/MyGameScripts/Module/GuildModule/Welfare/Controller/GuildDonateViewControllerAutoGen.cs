// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  GuildDonateViewControllerAutoGen.cs
// this file is generate by tool
// **********************************************************************

using UniRx;

public partial interface IGuildDonateViewController : ICloseView
{
     UniRx.IObservable<Unit> OnminusBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnaddBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OndonateBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OncloseBtn_UIButtonClick{get;}

}

public partial class GuildDonateViewController:FRPBaseController<
    GuildDonateViewController
    , GuildDonateView
    , IGuildDonateViewController
    , IGuildMainData>
    , IGuildDonateViewController
    {
	    //机器自动生成的事件订阅
        protected override void RegistEvent ()
        {
    minusBtn_UIButtonEvt = View.minusBtn_UIButton.AsObservable();
    addBtn_UIButtonEvt = View.addBtn_UIButton.AsObservable();
    donateBtn_UIButtonEvt = View.donateBtn_UIButton.AsObservable();
    closeBtn_UIButtonEvt = View.closeBtn_UIButton.AsObservable();

        }
        
        protected override void RemoveEvent()
        {
        minusBtn_UIButtonEvt = minusBtn_UIButtonEvt.CloseOnceNull();
        addBtn_UIButtonEvt = addBtn_UIButtonEvt.CloseOnceNull();
        donateBtn_UIButtonEvt = donateBtn_UIButtonEvt.CloseOnceNull();
        closeBtn_UIButtonEvt = closeBtn_UIButtonEvt.CloseOnceNull();

        }
        
            private Subject<Unit> minusBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnminusBtn_UIButtonClick{
        get {return minusBtn_UIButtonEvt;}
    }

    private Subject<Unit> addBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnaddBtn_UIButtonClick{
        get {return addBtn_UIButtonEvt;}
    }

    private Subject<Unit> donateBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OndonateBtn_UIButtonClick{
        get {return donateBtn_UIButtonEvt;}
    }

    private Subject<Unit> closeBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OncloseBtn_UIButtonClick{
        get {return closeBtn_UIButtonEvt;}
    }


    }
