// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  DelegateMissionEffectPanelControllerAutoGen.cs
// this file is generate by tool
// **********************************************************************

using UniRx;

public partial interface IDelegateMissionEffectPanelController : ICloseView
{

}

public partial class DelegateMissionEffectPanelController:FRPBaseController<
    DelegateMissionEffectPanelController
    , DelegateMissionEffectPanel
    , IDelegateMissionEffectPanelController
    ,IEveryDayMissionData>
    , IDelegateMissionEffectPanelController
    {
	    //机器自动生成的事件订阅
        protected override void RegistEvent ()
        {

        }
        
        protected override void RemoveEvent()
        {

        }
        
        
    }
