// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  ChatSettingPanelControllerAutoGen.cs
// this file is generate by tool
// **********************************************************************

using System;
using UniRx;

public partial interface IChatSettingPanelController
{
     UniRx.IObservable<Unit> OnShowChannelBtn_UIButtonClick{get;}
     UniRx.IObservable<Unit> OnAutoPlayerVoiceBtn_UIButtonClick{get;}

}

public partial class ChatSettingPanelController:MonoViewController<ChatSettingPanel>
    , IChatSettingPanelController
    {
	    //机器自动生成的事件订阅
        protected override void RegistEvent ()
        {
    ShowChannelBtn_UIButtonEvt = View.ShowChannelBtn_UIButton.AsObservable();
    AutoPlayerVoiceBtn_UIButtonEvt = View.AutoPlayerVoiceBtn_UIButton.AsObservable();

        }
        
        protected override void RemoveEvent()
        {
        ShowChannelBtn_UIButtonEvt = ShowChannelBtn_UIButtonEvt.CloseOnceNull();
        AutoPlayerVoiceBtn_UIButtonEvt = AutoPlayerVoiceBtn_UIButtonEvt.CloseOnceNull();

        }
        
            private Subject<Unit> ShowChannelBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnShowChannelBtn_UIButtonClick{
        get {return ShowChannelBtn_UIButtonEvt;}
    }

    private Subject<Unit> AutoPlayerVoiceBtn_UIButtonEvt;
    public UniRx.IObservable<Unit> OnAutoPlayerVoiceBtn_UIButtonClick{
        get {return AutoPlayerVoiceBtn_UIButtonEvt;}
    }


    }
