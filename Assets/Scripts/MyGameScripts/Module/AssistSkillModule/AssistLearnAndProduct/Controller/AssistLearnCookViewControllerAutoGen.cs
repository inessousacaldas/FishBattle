﻿// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  AssistLearnCookViewController.cs
// the file is generated by tools
// **********************************************************************


using UniRx;

public partial interface IAssistLearnCookViewController
{
     UniRx.IObservable<Unit> OnNoBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnYesBtn_UIButtonClick{get;}

}

public partial class AssistLearnCookViewController:MonolessViewController<AssistLearnCookView>, IAssistLearnCookViewController
{
    //机器自动生成的事件绑定
    protected override void InitReactiveEvents(){
        NoBtn_UIButtonEvt = View.NoBtn_UIButton.AsObservable();
        YesBtn_UIButtonEvt = View.YesBtn_UIButton.AsObservable();

    }

    protected override void ClearReactiveEvents(){
        NoBtn_UIButtonEvt = NoBtn_UIButtonEvt.CloseOnceNull();
        YesBtn_UIButtonEvt = YesBtn_UIButtonEvt.CloseOnceNull();

    }

    private Subject<Unit> NoBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnNoBtn_UIButtonClick{
        get {return NoBtn_UIButtonEvt;}
    }

    private Subject<Unit> YesBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnYesBtn_UIButtonClick{
        get {return YesBtn_UIButtonEvt;}
    }


}
