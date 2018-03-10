// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  CrewFavorableViewControllerAutoGen.cs
// this file is generate by tool
// **********************************************************************

using UniRx;

public partial interface ICrewFavorableViewController : ICloseView
{
     UniRx.IObservable<Unit> OnCloseBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnLastBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnCloseRecordBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnDressBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnPrefixBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnBiographyBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnNextBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnTextureBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnRecordBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnHistoryBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnRewardBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnTipBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnModelBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnCloseHistoryBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnCloseRewardBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnFavorableSlider_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnTexture_UIButtonClick{get;}

}

public partial class CrewFavorableViewController:FRPBaseController<
    CrewFavorableViewController
    , CrewFavorableView
    , ICrewFavorableViewController
    , ICrewViewData>
    , ICrewFavorableViewController
    {
	    //机器自动生成的事件订阅
        protected override void RegistEvent ()
        {
    CloseBtn_UIButtonEvt = View.CloseBtn_UIButton.AsObservable();
    LastBtn_UIButtonEvt = View.LastBtn_UIButton.AsObservable();
    CloseRecordBtn_UIButtonEvt = View.CloseRecordBtn_UIButton.AsObservable();
    DressBtn_UIButtonEvt = View.DressBtn_UIButton.AsObservable();
    PrefixBtn_UIButtonEvt = View.PrefixBtn_UIButton.AsObservable();
    BiographyBtn_UIButtonEvt = View.BiographyBtn_UIButton.AsObservable();
    NextBtn_UIButtonEvt = View.NextBtn_UIButton.AsObservable();
    TextureBtn_UIButtonEvt = View.TextureBtn_UIButton.AsObservable();
    RecordBtn_UIButtonEvt = View.RecordBtn_UIButton.AsObservable();
    HistoryBtn_UIButtonEvt = View.HistoryBtn_UIButton.AsObservable();
    RewardBtn_UIButtonEvt = View.RewardBtn_UIButton.AsObservable();
    TipBtn_UIButtonEvt = View.TipBtn_UIButton.AsObservable();
    ModelBtn_UIButtonEvt = View.ModelBtn_UIButton.AsObservable();
    CloseHistoryBtn_UIButtonEvt = View.CloseHistoryBtn_UIButton.AsObservable();
    CloseRewardBtn_UIButtonEvt = View.CloseRewardBtn_UIButton.AsObservable();
    FavorableSlider_UIButtonEvt = View.FavorableSlider_UIButton.AsObservable();
    Texture_UIButtonEvt = View.Texture_UIButton.AsObservable();

        }
        
        protected override void RemoveEvent()
        {
        CloseBtn_UIButtonEvt = CloseBtn_UIButtonEvt.CloseOnceNull();
        LastBtn_UIButtonEvt = LastBtn_UIButtonEvt.CloseOnceNull();
        CloseRecordBtn_UIButtonEvt = CloseRecordBtn_UIButtonEvt.CloseOnceNull();
        DressBtn_UIButtonEvt = DressBtn_UIButtonEvt.CloseOnceNull();
        PrefixBtn_UIButtonEvt = PrefixBtn_UIButtonEvt.CloseOnceNull();
        BiographyBtn_UIButtonEvt = BiographyBtn_UIButtonEvt.CloseOnceNull();
        NextBtn_UIButtonEvt = NextBtn_UIButtonEvt.CloseOnceNull();
        TextureBtn_UIButtonEvt = TextureBtn_UIButtonEvt.CloseOnceNull();
        RecordBtn_UIButtonEvt = RecordBtn_UIButtonEvt.CloseOnceNull();
        HistoryBtn_UIButtonEvt = HistoryBtn_UIButtonEvt.CloseOnceNull();
        RewardBtn_UIButtonEvt = RewardBtn_UIButtonEvt.CloseOnceNull();
        TipBtn_UIButtonEvt = TipBtn_UIButtonEvt.CloseOnceNull();
        ModelBtn_UIButtonEvt = ModelBtn_UIButtonEvt.CloseOnceNull();
        CloseHistoryBtn_UIButtonEvt = CloseHistoryBtn_UIButtonEvt.CloseOnceNull();
        CloseRewardBtn_UIButtonEvt = CloseRewardBtn_UIButtonEvt.CloseOnceNull();
        FavorableSlider_UIButtonEvt = FavorableSlider_UIButtonEvt.CloseOnceNull();
        Texture_UIButtonEvt = Texture_UIButtonEvt.CloseOnceNull();

        }
        
            private Subject<Unit> CloseBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnCloseBtn_UIButtonClick{
        get {return CloseBtn_UIButtonEvt;}
    }

    private Subject<Unit> LastBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnLastBtn_UIButtonClick{
        get {return LastBtn_UIButtonEvt;}
    }

    private Subject<Unit> CloseRecordBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnCloseRecordBtn_UIButtonClick{
        get {return CloseRecordBtn_UIButtonEvt;}
    }

    private Subject<Unit> DressBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnDressBtn_UIButtonClick{
        get {return DressBtn_UIButtonEvt;}
    }

    private Subject<Unit> PrefixBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnPrefixBtn_UIButtonClick{
        get {return PrefixBtn_UIButtonEvt;}
    }

    private Subject<Unit> BiographyBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnBiographyBtn_UIButtonClick{
        get {return BiographyBtn_UIButtonEvt;}
    }

    private Subject<Unit> NextBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnNextBtn_UIButtonClick{
        get {return NextBtn_UIButtonEvt;}
    }

    private Subject<Unit> TextureBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnTextureBtn_UIButtonClick{
        get {return TextureBtn_UIButtonEvt;}
    }

    private Subject<Unit> RecordBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnRecordBtn_UIButtonClick{
        get {return RecordBtn_UIButtonEvt;}
    }

    private Subject<Unit> HistoryBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnHistoryBtn_UIButtonClick{
        get {return HistoryBtn_UIButtonEvt;}
    }

    private Subject<Unit> RewardBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnRewardBtn_UIButtonClick{
        get {return RewardBtn_UIButtonEvt;}
    }

    private Subject<Unit> TipBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnTipBtn_UIButtonClick{
        get {return TipBtn_UIButtonEvt;}
    }

    private Subject<Unit> ModelBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnModelBtn_UIButtonClick{
        get {return ModelBtn_UIButtonEvt;}
    }

    private Subject<Unit> CloseHistoryBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnCloseHistoryBtn_UIButtonClick{
        get {return CloseHistoryBtn_UIButtonEvt;}
    }

    private Subject<Unit> CloseRewardBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnCloseRewardBtn_UIButtonClick{
        get {return CloseRewardBtn_UIButtonEvt;}
    }

    private Subject<Unit> FavorableSlider_UIButtonEvt;
    public UniRx.IObservable<Unit> OnFavorableSlider_UIButtonClick{
        get {return FavorableSlider_UIButtonEvt;}
    }

    private Subject<Unit> Texture_UIButtonEvt;
    public UniRx.IObservable<Unit> OnTexture_UIButtonClick{
        get {return Texture_UIButtonEvt;}
    }


    }
