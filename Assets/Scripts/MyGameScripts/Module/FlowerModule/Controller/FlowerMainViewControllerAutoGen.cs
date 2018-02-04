// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  FlowerMainViewControllerAutoGen.cs
// this file is generate by tool
// **********************************************************************

using UniRx;

public partial interface IFlowerMainViewController : ICloseView
{
     UniRx.IObservable<Unit> OnCloseBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnSearchBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnGiveBtn_UIButtonClick{get;}
    UniRx.IObservable<Unit> OnMaxBtn_UIButtonClick {get;}
}

public partial class FlowerMainViewController:FRPBaseController<
    FlowerMainViewController
    , FlowerMainView
    , IFlowerMainViewController
    , IFlowerMainViewData>
    , IFlowerMainViewController
    {
	    //机器自动生成的事件订阅
        protected override void RegistEvent ()
        {
    CloseBtn_UIButtonEvt = View.CloseBtn_UIButton.AsObservable();
    SearchBtn_UIButtonEvt = View.SearchBtn_UIButton.AsObservable();
    GiveBtn_UIButtonEvt = View.GiveBtn_UIButton.AsObservable();
    MaxBtn_UIButtonEvt = View.MaxBtn_UIButton.AsObservable();

    }
        
        protected override void RemoveEvent()
        {
        CloseBtn_UIButtonEvt = CloseBtn_UIButtonEvt.CloseOnceNull();
        SearchBtn_UIButtonEvt = SearchBtn_UIButtonEvt.CloseOnceNull();
        GiveBtn_UIButtonEvt = GiveBtn_UIButtonEvt.CloseOnceNull();
        MaxBtn_UIButtonEvt = MaxBtn_UIButtonEvt.CloseOnceNull();

    }
        
            private Subject<Unit> CloseBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnCloseBtn_UIButtonClick{
        get {return CloseBtn_UIButtonEvt;}
    }

    private Subject<Unit> SearchBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnSearchBtn_UIButtonClick{
        get {return SearchBtn_UIButtonEvt;}
    }

    private Subject<Unit> GiveBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnGiveBtn_UIButtonClick{
        get {return GiveBtn_UIButtonEvt;}
    }

    private Subject<Unit> MaxBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnMaxBtn_UIButtonClick
    {
        get { return MaxBtn_UIButtonEvt; }
    }

}
