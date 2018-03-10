// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  GuildMainViewControllerAutoGen.cs
// this file is generate by tool
// **********************************************************************

using UniRx;

public partial interface IGuildMainViewController : ICloseView
{
     UniRx.IObservable<Unit> OnCloseBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OndiamondBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OngoldBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnjoinBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OncreateBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnsearchBtn_UIButtonClick { get; }
        
     UniRx.IObservable<Unit> OnContactBtn_UIButtonClick { get; }
        
     UniRx.IObservable<Unit> OnoneKeyApplyBtn_UIButtonClick { get; }
     UniRx.IObservable<Unit> OnapplyBtn_UIButtonClick { get; }
    UniRx.IObservable<GuildItemController> GuildItemCtrl { get; }  //公会点击

}

public partial class GuildMainViewController:FRPBaseController<
    GuildMainViewController
    , GuildMainView
    , IGuildMainViewController
    , IGuildMainData>
    , IGuildMainViewController
{
    //机器自动生成的事件订阅
    protected override void RegistEvent()
    {
        CloseBtn_UIButtonEvt = View.CloseBtn_UIButton.AsObservable();
        diamondBtn_UIButtonEvt = View.diamondBtn_UIButton.AsObservable();
        goldBtn_UIButtonEvt = View.goldBtn_UIButton.AsObservable();
        joinBtn_UIButtonEvt = View.joinBtn_UIButton.AsObservable();
        createBtn_UIButtonEvt = View.createBtn_UIButton.AsObservable();
        searchBtn_UIButtonEvt = View.searchBtn_UIButton.AsObservable();
        contactBtn_UIButtonEvt = View.contactBtn_UIButton.AsObservable();
        oneKeyApplyBtn_UIButtonEvt = View.oneKeyApply_UIButton.AsObservable();
        applyBtn_UIButtonEvt = View.applyBtn_UIButton.AsObservable();

    }

    protected override void RemoveEvent()
    {
        CloseBtn_UIButtonEvt = CloseBtn_UIButtonEvt.CloseOnceNull();
        diamondBtn_UIButtonEvt = diamondBtn_UIButtonEvt.CloseOnceNull();
        goldBtn_UIButtonEvt = goldBtn_UIButtonEvt.CloseOnceNull();
        joinBtn_UIButtonEvt = joinBtn_UIButtonEvt.CloseOnceNull();
        createBtn_UIButtonEvt = createBtn_UIButtonEvt.CloseOnceNull();
        searchBtn_UIButtonEvt = searchBtn_UIButtonEvt.CloseOnceNull();
        contactBtn_UIButtonEvt = contactBtn_UIButtonEvt.CloseOnceNull();
        oneKeyApplyBtn_UIButtonEvt = oneKeyApplyBtn_UIButtonEvt.CloseOnceNull();
        applyBtn_UIButtonEvt = applyBtn_UIButtonEvt.CloseOnceNull();
    }

    private Subject<Unit> CloseBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnCloseBtn_UIButtonClick{
        get {return CloseBtn_UIButtonEvt;}
    }

    private Subject<Unit> diamondBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OndiamondBtn_UIButtonClick{
        get {return diamondBtn_UIButtonEvt;}
    }

    private Subject<Unit> goldBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OngoldBtn_UIButtonClick{
        get {return goldBtn_UIButtonEvt;}
    }

    private Subject<Unit> joinBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnjoinBtn_UIButtonClick{
        get {return joinBtn_UIButtonEvt;}
    }

    private Subject<Unit> createBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OncreateBtn_UIButtonClick{
        get {return createBtn_UIButtonEvt;}
    }

    private Subject<Unit> searchBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnsearchBtn_UIButtonClick
    {
        get { return searchBtn_UIButtonEvt; }
    }

    private Subject<Unit> contactBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnContactBtn_UIButtonClick
    {
        get { return contactBtn_UIButtonEvt; }
    }

    private Subject<Unit> oneKeyApplyBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnoneKeyApplyBtn_UIButtonClick
    {
        get { return oneKeyApplyBtn_UIButtonEvt; }
    }

    private Subject<Unit> applyBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnapplyBtn_UIButtonClick
    {
        get { return applyBtn_UIButtonEvt; }
    }

    private UniRx.Subject<GuildItemController> guildItemCtrl = new UniRx.Subject<GuildItemController>();
    public UniRx.IObservable<GuildItemController> GuildItemCtrl { get { return guildItemCtrl; } }
}
