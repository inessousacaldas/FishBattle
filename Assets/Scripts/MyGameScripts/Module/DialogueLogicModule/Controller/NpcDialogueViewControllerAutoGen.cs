// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  NpcDialogueViewControllerAutoGen.cs
// this file is generate by tool
// **********************************************************************

using UniRx;

public partial interface INpcDialogueViewController : ICloseView
{
     UniRx.IObservable<Unit> OnClickBtnBoxCollider_UIButtonClick{get;}

}

public partial class NpcDialogueViewController:FRPBaseController<
    NpcDialogueViewController
    , NpcDialogueView
    , INpcDialogueViewController
    , IMissionData>
    , INpcDialogueViewController
    {
	    //机器自动生成的事件订阅
        protected override void RegistEvent ()
        {
    ClickBtnBoxCollider_UIButtonEvt = View.ClickBtnBoxCollider_UIButton.AsObservable();

        }
        
        protected override void RemoveEvent()
        {
        ClickBtnBoxCollider_UIButtonEvt = ClickBtnBoxCollider_UIButtonEvt.CloseOnceNull();

        }
        
            private Subject<Unit> ClickBtnBoxCollider_UIButtonEvt;
    public UniRx.IObservable<Unit> OnClickBtnBoxCollider_UIButtonClick{
        get {return ClickBtnBoxCollider_UIButtonEvt;}
    }


    }
