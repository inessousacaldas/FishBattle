﻿// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  guildModifyJobBtnController.cs
// the file is generated by tools
// **********************************************************************


using UniRx;

public partial interface IguildModifyJobBtnController
{
     UniRx.IObservable<Unit> OnguildModifyJobBtn_UIButtonClick{get;}

}

public partial class guildModifyJobBtnController:MonolessViewController<guildModifyJobBtn>, IguildModifyJobBtnController
{
    //机器自动生成的事件绑定
    protected override void InitReactiveEvents(){
        guildModifyJobBtn_UIButtonEvt = View.guildModifyJobBtn_UIButton.AsObservable();

    }

    protected override void ClearReactiveEvents(){
        guildModifyJobBtn_UIButtonEvt = guildModifyJobBtn_UIButtonEvt.CloseOnceNull();

    }

    private Subject<Unit> guildModifyJobBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnguildModifyJobBtn_UIButtonClick{
        get {return guildModifyJobBtn_UIButtonEvt;}
    }


}
