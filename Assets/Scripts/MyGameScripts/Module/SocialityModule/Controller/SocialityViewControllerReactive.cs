//using UniRx;

//public sealed partial class SocialityDataMgr
//{
//    public partial class ChatInfoViewController
//    {
//        #region Event
//    protected  void BeforeRegistEvent(){
//    HideBtnEvt = _view.HideBtn.AsObservable();
   
//    UnReadBtnEvt = _view.UnReadBtn.AsObservable();
//    SendBtn_UIButtonEvt = _view.SendBtn_UIButton.AsObservable();
//    SpeedOrInputBtn_UIButtonEvt = _view.SpeedOrInputBtn_UIButton.AsObservable();
//    ApplyGuideBtn_UIButtonEvt = _view.ApplyGuideBtn_UIButton.AsObservable();
//    RedPacketRemind_UIButtonEvt = _view.RedPacketRemind_UIButton.AsObservable();
//    HideBtn_UIButtonEvt = _view.HideBtn_UIButton.AsObservable();

//    }

//    protected override void RemoveEvent(){
//                Expression_UIButtonEvt = Expression_UIButtonEvt.CloseOnceNull();
//        HideBtnEvt = HideBtnEvt.CloseOnceNull();
//        lockToggleBtnEvt = lockToggleBtnEvt.CloseOnceNull();
//        UnReadBtnEvt = UnReadBtnEvt.CloseOnceNull();
//        SendBtn_UIButtonEvt = SendBtn_UIButtonEvt.CloseOnceNull();
//        SpeedOrInputBtn_UIButtonEvt = SpeedOrInputBtn_UIButtonEvt.CloseOnceNull();
//        ApplyGuideBtn_UIButtonEvt = ApplyGuideBtn_UIButtonEvt.CloseOnceNull();
//        RedPacketRemind_UIButtonEvt = RedPacketRemind_UIButtonEvt.CloseOnceNull();
//        HideBtn_UIButtonEvt = HideBtn_UIButtonEvt.CloseOnceNull();

//    }

//        private Subject<Unit> Expression_UIButtonEvt;
//    public UniRx.IObservable<Unit> OnExpression_UIButtonClick{
//        get {return Expression_UIButtonEvt;}
//    }

//    private Subject<Unit> HideBtnEvt;
//    public UniRx.IObservable<Unit> OnHideBtnClick{
//        get {return HideBtnEvt;}
//    }

//    private Subject<Unit> lockToggleBtnEvt;
//    public UniRx.IObservable<Unit> OnlockToggleBtnClick{
//        get {return lockToggleBtnEvt;}
//    }

//    private Subject<Unit> UnReadBtnEvt;
//    public UniRx.IObservable<Unit> OnUnReadBtnClick{
//        get {return UnReadBtnEvt;}
//    }

//    private Subject<Unit> SendBtn_UIButtonEvt;
//    public UniRx.IObservable<Unit> OnSendBtn_UIButtonClick{
//        get {return SendBtn_UIButtonEvt;}
//    }

//    private Subject<Unit> SpeedOrInputBtn_UIButtonEvt;
//    public UniRx.IObservable<Unit> OnSpeedOrInputBtn_UIButtonClick{
//        get {return SpeedOrInputBtn_UIButtonEvt;}
//    }

//    private Subject<Unit> ApplyGuideBtn_UIButtonEvt;
//    public UniRx.IObservable<Unit> OnApplyGuideBtn_UIButtonClick{
//        get {return ApplyGuideBtn_UIButtonEvt;}
//    }

//    private Subject<Unit> RedPacketRemind_UIButtonEvt;
//    public UniRx.IObservable<Unit> OnRedPacketRemind_UIButtonClick{
//        get {return RedPacketRemind_UIButtonEvt;}
//    }

//    private Subject<Unit> HideBtn_UIButtonEvt;
//    public UniRx.IObservable<Unit> OnHideBtn_UIButtonClick{
//        get {return HideBtn_UIButtonEvt;}
//    }

//    #endregion
//    }

//}