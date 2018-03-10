﻿// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  TeamPlayerInfoItemController.cs
// the file is generated by tools
// **********************************************************************

using System;
using AppDto;
using UniRx;

public partial class TeamPlayerInfoItemController:MonolessViewController<TeamPlayerInfoItem>
{
    //机器自动生成的事件绑定
    protected override void InitReactiveEvents(){
        agreeBtn_UIButtonEvt = View.agreeBtn_UIButton.AsObservable();
    }

    protected override void ClearReactiveEvents(){
        agreeBtn_UIButtonEvt = agreeBtn_UIButtonEvt.CloseOnceNull();

    }

    private Subject<Unit> agreeBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnagreeBtn_UIButtonClick{
        get {return agreeBtn_UIButtonEvt;}
    }
}
