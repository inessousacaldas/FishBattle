// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  MisRewardTipViewControllerAutoGen.cs
// this file is generate by tool
// **********************************************************************

using UniRx;

public partial interface IMisRewardTipViewController : ICloseView
{
     UniRx.IObservable<Unit> OnwidgtBox_UIButtonClick{get;}

}

public partial class MisRewardTipViewController:FRPBaseController<
    MisRewardTipViewController
    , MisRewardTipView
    , IMisRewardTipViewController
    , IMisRewardTipData>
    , IMisRewardTipViewController
    {
	    //机器自动生成的事件订阅
        protected override void RegistEvent ()
        {
    widgtBox_UIButtonEvt = View.widgtBox_UIButton.AsObservable();

        }
        
        protected override void RemoveEvent()
        {
        widgtBox_UIButtonEvt = widgtBox_UIButtonEvt.CloseOnceNull();

        }
        
            private Subject<Unit> widgtBox_UIButtonEvt;
    public UniRx.IObservable<Unit> OnwidgtBox_UIButtonClick{
        get {return widgtBox_UIButtonEvt;}
    }


    }
