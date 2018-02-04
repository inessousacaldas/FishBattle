// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  ChatChannelToggleViewControllerAutoGen.cs
// this file is generate by tool
// **********************************************************************

using System;
using UniRx;

public sealed partial class ChatDataMgr
{
    public partial class ChatChannelToggleViewController:FRPBaseController_V1<ChatChannelToggleView, IChatChannelToggleView>
    {	
	    private CompositeDisposable _disposable = null;
	    protected override void InitViewWithStream()
        {
            stream.OnNext(DataMgr._data);
        }

	    //机器自动生成的事件订阅
        protected override void RegistEvent ()
        {
	        //_disposable.Add(stream.Subscribe(data=>View.UpdateView(data)));
            _disposable.Add(View.OnCloseBtn_UIButtonClick.Subscribe(_ => CloseBtn_UIButtonClickHandler()));
            _disposable.Add(View.OnConfirmBtn_UIButtonClick.Subscribe(_ => ConfirmBtn_UIButtonClickHandler()));

        }
    }
}
