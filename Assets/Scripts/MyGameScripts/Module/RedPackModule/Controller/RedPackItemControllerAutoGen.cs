﻿// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  RedPackItemController.cs
// the file is generated by tools
// **********************************************************************


using UniRx;

public partial interface IRedPackItemController
{
     UniRx.IObservable<Unit> OnRedPackItem_UIButtonClick{get;}

}

public partial class RedPackItemController:MonolessViewController<RedPackItem>, IRedPackItemController
{
    //机器自动生成的事件绑定
    protected override void InitReactiveEvents(){
        RedPackItem_UIButtonEvt = View.RedPackItem_UIButton.AsObservable();

    }

    protected override void ClearReactiveEvents(){
        RedPackItem_UIButtonEvt = RedPackItem_UIButtonEvt.CloseOnceNull();

    }

    private Subject<Unit> RedPackItem_UIButtonEvt;
    public UniRx.IObservable<Unit> OnRedPackItem_UIButtonClick{
        get {return RedPackItem_UIButtonEvt;}
    }


}
