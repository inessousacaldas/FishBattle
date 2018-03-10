﻿// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  TipsEquipmentEffectItemController.cs
// the file is generated by tools
// **********************************************************************


using UniRx;

public partial interface ITipsEquipmentEffectItemController
{
     UniRx.IObservable<Unit> OnBtn_UIButtonClick{get;}

}

public partial class TipsEquipmentEffectItemController:MonolessViewController<TipsEquipmentEffectItem>, ITipsEquipmentEffectItemController
{
    //机器自动生成的事件绑定
    protected override void InitReactiveEvents(){
        Btn_UIButtonEvt = View.Btn_UIButton.AsObservable();

    }

    protected override void ClearReactiveEvents(){
        Btn_UIButtonEvt = Btn_UIButtonEvt.CloseOnceNull();

    }

    private Subject<Unit> Btn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnBtn_UIButtonClick{
        get {return Btn_UIButtonEvt;}
    }


}
