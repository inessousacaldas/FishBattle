// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  SearchFriendViewControllerAutoGen.cs
// this file is generate by tool
// **********************************************************************

using UniRx;

public partial interface ISearchFriendViewController : ICloseView
{
     UniRx.IObservable<Unit> OnCloseButton_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnCancleInputBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnSearchBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnChangeBatchBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnMapBtn_UIButtonClick{get;}

}

public partial class SearchFriendViewController:FRPBaseController<
    SearchFriendViewController
    , SearchFriendView
    , ISearchFriendViewController
    , ISearchFriendData>
    , ISearchFriendViewController
    {
	    //机器自动生成的事件订阅
        protected override void RegistEvent ()
        {
    CloseButton_UIButtonEvt = View.CloseButton_UIButton.AsObservable();
    CancleInputBtn_UIButtonEvt = View.CancleInputBtn_UIButton.AsObservable();
    SearchBtn_UIButtonEvt = View.SearchBtn_UIButton.AsObservable();
    ChangeBatchBtn_UIButtonEvt = View.ChangeBatchBtn_UIButton.AsObservable();
    MapBtn_UIButtonEvt = View.MapBtn_UIButton.AsObservable();

        }
        
        protected override void RemoveEvent()
        {
        CloseButton_UIButtonEvt = CloseButton_UIButtonEvt.CloseOnceNull();
        CancleInputBtn_UIButtonEvt = CancleInputBtn_UIButtonEvt.CloseOnceNull();
        SearchBtn_UIButtonEvt = SearchBtn_UIButtonEvt.CloseOnceNull();
        ChangeBatchBtn_UIButtonEvt = ChangeBatchBtn_UIButtonEvt.CloseOnceNull();
        MapBtn_UIButtonEvt = MapBtn_UIButtonEvt.CloseOnceNull();

        }
        
            private Subject<Unit> CloseButton_UIButtonEvt;
    public UniRx.IObservable<Unit> OnCloseButton_UIButtonClick{
        get {return CloseButton_UIButtonEvt;}
    }

    private Subject<Unit> CancleInputBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnCancleInputBtn_UIButtonClick{
        get {return CancleInputBtn_UIButtonEvt;}
    }

    private Subject<Unit> SearchBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnSearchBtn_UIButtonClick{
        get {return SearchBtn_UIButtonEvt;}
    }

    private Subject<Unit> ChangeBatchBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnChangeBatchBtn_UIButtonClick{
        get {return ChangeBatchBtn_UIButtonEvt;}
    }

    private Subject<Unit> MapBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnMapBtn_UIButtonClick{
        get {return MapBtn_UIButtonEvt;}
    }


    }
