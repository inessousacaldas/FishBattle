﻿// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  Face_PetItemController.cs
// the file is generated by tools
// **********************************************************************


using UniRx;

public partial interface IFace_PetItemController
{
     UniRx.IObservable<Unit> OnFace_PetItem_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnIconBg_UIButtonClick{get;}

}

public partial class Face_PetItemController:MonolessViewController<Face_PetItem>, IFace_PetItemController
{
    //机器自动生成的事件绑定
    protected override void InitReactiveEvents(){
        Face_PetItem_UIButtonEvt = View.Face_PetItem_UIButton.AsObservable();
        IconBg_UIButtonEvt = View.IconBg_UIButton.AsObservable();

    }

    protected override void ClearReactiveEvents(){
        Face_PetItem_UIButtonEvt = Face_PetItem_UIButtonEvt.CloseOnceNull();
        IconBg_UIButtonEvt = IconBg_UIButtonEvt.CloseOnceNull();

    }

    private Subject<Unit> Face_PetItem_UIButtonEvt;
    public UniRx.IObservable<Unit> OnFace_PetItem_UIButtonClick{
        get {return Face_PetItem_UIButtonEvt;}
    }

    private Subject<Unit> IconBg_UIButtonEvt;
    public UniRx.IObservable<Unit> OnIconBg_UIButtonClick{
        get {return IconBg_UIButtonEvt;}
    }


}
