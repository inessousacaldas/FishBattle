// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  EveryDayMissionPanelControllerAutoGen.cs
// this file is generate by tool
// **********************************************************************

using UniRx;

public partial interface IEveryDayMissionPanelController : ICloseView
{

}

public partial class EveryDayMissionPanelController:FRPBaseController<
    EveryDayMissionPanelController
    , EveryDayMissionPanel
    , IEveryDayMissionPanelController
    , IEveryDayMissionData>
    , IEveryDayMissionPanelController
    {
	    //机器自动生成的事件订阅
        protected override void RegistEvent ()
        {

        }
        
        protected override void RemoveEvent()
        {

        }
        
        
    }
