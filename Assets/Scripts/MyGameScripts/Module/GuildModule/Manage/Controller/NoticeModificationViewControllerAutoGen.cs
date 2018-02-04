// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  NoticeModificationViewControllerAutoGen.cs
// this file is generate by tool
// **********************************************************************

using System;
using UniRx;

public partial interface INoticeModificationViewController
{
     UniRx.IObservable<Unit> OnCloseBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnPublishBtn_UIButtonClick{get;}

}

public partial class NoticeModificationViewController : MonoViewController<NoticeModificationView>
    , INoticeModificationViewController
{
    //机器自动生成的事件订阅
    protected override void RegistEvent()
    {
        CloseBtn_UIButtonEvt = View.CloseBtn_UIButton.AsObservable();
        PublishBtn_UIButtonEvt = View.PublishBtn_UIButton.AsObservable();

    }

    protected override void RemoveEvent()
    {
        CloseBtn_UIButtonEvt = CloseBtn_UIButtonEvt.CloseOnceNull();
        PublishBtn_UIButtonEvt = PublishBtn_UIButtonEvt.CloseOnceNull();

    }

    private Subject<Unit> CloseBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnCloseBtn_UIButtonClick
    {
        get { return CloseBtn_UIButtonEvt; }
    }

    private Subject<Unit> PublishBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnPublishBtn_UIButtonClick
    {
        get { return PublishBtn_UIButtonEvt; }
    }


}
