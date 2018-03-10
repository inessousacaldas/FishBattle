// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  ContestPanelControllerAutoGen.cs
// this file is generate by tool
// **********************************************************************

using UniRx;

public partial interface IContestPanelController : ICloseView
{

}

public partial class ContestPanelController:FRPBaseController<
    ContestPanelController
    , ContestPanel
    , IContestPanelController
    , IContestData>
    , IContestPanelController
    {
	    //机器自动生成的事件订阅
        protected override void RegistEvent ()
        {

        }
        
        protected override void RemoveEvent()
        {

        }
        
        
    }
