// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  EquipmentSpecialEffectControllerAutoGen.cs
// this file is generate by tool
// **********************************************************************

using UniRx;

public partial interface IEquipmentEffectsController : ICloseView
{
     UniRx.IObservable<Unit> OnHoleBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnCloseBtn_UIButtonClick{get;}

}

public partial class EquipmentEffectsController:FRPBaseController<
    EquipmentEffectsController
    , EquipmentEffects
    , IEquipmentEffectsController
    , IEquipmentEffectsData>
    , IEquipmentEffectsController
    {
	    //机器自动生成的事件订阅
        protected override void RegistEvent ()
        {
    HoleBtn_UIButtonEvt = View.HoleBtn_UIButton.AsObservable();
    CloseBtn_UIButtonEvt = View.CloseBtn_UIButton.AsObservable();

        }
        
        protected override void RemoveEvent()
        {
        HoleBtn_UIButtonEvt = HoleBtn_UIButtonEvt.CloseOnceNull();
        CloseBtn_UIButtonEvt = CloseBtn_UIButtonEvt.CloseOnceNull();

        }
        
            private Subject<Unit> HoleBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnHoleBtn_UIButtonClick{
        get {return HoleBtn_UIButtonEvt;}
    }

    private Subject<Unit> CloseBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnCloseBtn_UIButtonClick{
        get {return CloseBtn_UIButtonEvt;}
    }


    }
