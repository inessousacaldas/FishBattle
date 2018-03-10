// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  TeamFormationTabContentViewControllerAutoGen.cs
// this file is generate by tool
// **********************************************************************

using System;
using AppDto;
using UniRx;

public partial interface ITeamFormationController : ICloseView
{
     UniRx.IObservable<Unit> OnCloseBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnLearnBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnNoUseBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnUseBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnUpGradeBtn_UIButtonClick{get;}

    void InitFormationList(ITeamFormationData data, FormationPosController.FormationType type);
    UniRx.IObservable<int> GetUseBtnHandler { get; }
    UniRx.IObservable<Unit> GetNoUseFormationHandler { get; }
}

public partial class TeamFormationController:FRPBaseController<
    TeamFormationController
    , TeamFormationTabContentView
    , ITeamFormationController
    , ITeamFormationData>
    , ITeamFormationController
{
    //机器自动生成的事件订阅
    protected override void RegistEvent()
    {
        CloseBtn_UIButtonEvt = View.CloseBtn_UIButton.AsObservable();
        LearnBtn_UIButtonEvt = View.LearnBtn_UIButton.AsObservable();
        NoUseBtn_UIButtonEvt = View.NoUseBtn_UIButton.AsObservable();
        UseBtn_UIButtonEvt = View.UseBtn_UIButton.AsObservable();
        UpGradeBtn_UIButtonEvt = View.UpGradeBtn_UIButton.AsObservable();
        RestrainBtn_UIButtonEvt = View.RestrainBtn_UIButton.AsObservable();
        DescBtn_UIButtonEvt = View.DescBtn_UIButton.AsObservable();

    }

    protected override void RemoveEvent()
    {
        CloseBtn_UIButtonEvt = CloseBtn_UIButtonEvt.CloseOnceNull();
        LearnBtn_UIButtonEvt = LearnBtn_UIButtonEvt.CloseOnceNull();
        NoUseBtn_UIButtonEvt = NoUseBtn_UIButtonEvt.CloseOnceNull();
        UseBtn_UIButtonEvt = UseBtn_UIButtonEvt.CloseOnceNull();
        UpGradeBtn_UIButtonEvt = UpGradeBtn_UIButtonEvt.CloseOnceNull();
        RestrainBtn_UIButtonEvt = RestrainBtn_UIButtonEvt.CloseOnceNull();
        DescBtn_UIButtonEvt = DescBtn_UIButtonEvt.CloseOnceNull();

    }

    private Subject<Unit> CloseBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnCloseBtn_UIButtonClick
    {
        get { return CloseBtn_UIButtonEvt; }
    }

    private Subject<Unit> LearnBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnLearnBtn_UIButtonClick
    {
        get { return LearnBtn_UIButtonEvt; }
    }

    private Subject<Unit> NoUseBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnNoUseBtn_UIButtonClick
    {
        get { return NoUseBtn_UIButtonEvt; }
    }

    private Subject<Unit> UseBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnUseBtn_UIButtonClick
    {
        get { return UseBtn_UIButtonEvt; }
    }

    private Subject<Unit> UpGradeBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnUpGradeBtn_UIButtonClick
    {
        get { return UpGradeBtn_UIButtonEvt; }
    }

    private Subject<Unit> RestrainBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnRestrainBtn_UIButtonClick
    {
        get { return RestrainBtn_UIButtonEvt; }
    }

    private Subject<Unit> DescBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnDescBtn_UIButtonClick
    {
        get { return DescBtn_UIButtonEvt; }
    }


}
