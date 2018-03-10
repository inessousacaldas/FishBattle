// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  CrewSkillItemCellControllerAutoGen.cs
// this file is generate by tool
// **********************************************************************

using System;
using UniRx;

public partial interface ICrewSkillItemCellController
{
    UniRx.IObservable<Unit> OnCellClick { get; }
}

public partial class CrewSkillItemCellController: MonolessViewController<CrewSkillItemCell>
    , ICrewSkillItemCellController
{

    protected override void InitReactiveEvents()
    {
        cellClickEvent = this.gameObject.OnClickAsObservable();
    }

    protected override void ClearReactiveEvents()
    {
        cellClickEvent = cellClickEvent.CloseOnceNull();
    }

    private Subject<Unit> cellClickEvent;
    public UniRx.IObservable<Unit> OnCellClick
    {
        get { return cellClickEvent; }
    }
}
