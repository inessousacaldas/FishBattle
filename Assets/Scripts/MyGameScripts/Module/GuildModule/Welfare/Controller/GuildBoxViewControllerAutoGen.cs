// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  GuildBoxViewControllerAutoGen.cs
// this file is generate by tool
// **********************************************************************

using UniRx;

public partial interface IGuildBoxViewController : ICloseView
{
     UniRx.IObservable<Unit> OnboxCloseBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnboxOpenBtn_UIButtonClick{get;}

}

public partial class GuildBoxViewController:FRPBaseController<
    GuildBoxViewController
    , GuildBoxView
    , IGuildBoxViewController
    , IGuildMainData>
    , IGuildBoxViewController
    {
	    //机器自动生成的事件订阅
        protected override void RegistEvent ()
        {
    boxCloseBtn_UIButtonEvt = View.boxCloseBtn_UIButton.AsObservable();
    boxOpenBtn_UIButtonEvt = View.boxOpenBtn_UIButton.AsObservable();

        }
        
        protected override void RemoveEvent()
        {
        boxCloseBtn_UIButtonEvt = boxCloseBtn_UIButtonEvt.CloseOnceNull();
        boxOpenBtn_UIButtonEvt = boxOpenBtn_UIButtonEvt.CloseOnceNull();

        }
        
            private Subject<Unit> boxCloseBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnboxCloseBtn_UIButtonClick{
        get {return boxCloseBtn_UIButtonEvt;}
    }

    private Subject<Unit> boxOpenBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnboxOpenBtn_UIButtonClick{
        get {return boxOpenBtn_UIButtonEvt;}
    }


    }
