// **********************************************************************
// Copyright (c) 2013 Baoyugame. All rights reserved.
// File     :  NotifyListenerRegister.cs
// Author   : SK
// Created  : 2014/11/11
// Purpose  : 
// **********************************************************************
using System;
using AppDto;
using UniRx;

namespace StaticDispose
{
	public partial class StaticDispose
	{
		private StaticDelegateRunner disposeNotifyListenerRegister = new StaticDelegateRunner(
			NotifyListenerRegister.ExecuteDispose);
	}
}

public static class NotifyListenerRegister
{
	private static bool HasInited;
	
	public static void Setup ()
	{
		if (HasInited) {
			return;
		}

		HasInited = true;

        #region 协议监听

        // battlevideo
        AddListener (GenericNotifyListener.Get<CommandNotify>());
		AddListener(GenericNotifyListener.Get<FighterReadyNotifyDto>());

        //server
        AddListener (new GameServerTimeDtoListener ());

        //function open switch
        AddListener(new FunctionOpenNotifyListener());

        //Email
        AddListener(GenericNotifyListener.Get<PlayerMailDto>());
	    AddListener(GenericNotifyListener.Get<MailChangeIdsNotify>());
	    // Chat
	    AddListener(GenericNotifyListener.Get<ChatNotify>());
		AddListener(GenericNotifyListener.Get<MarqueeNoticeNotify>());
        //redpoint
        AddListener(GenericNotifyListener.Get<ShowRedPointTypeListDto>());

        #endregion
    }

	public static void ExecuteDispose()
	{
		GenericNotifyListener.RemoveAllListener(RemoveNotifyHandler);
	}

	#region
    private static bool _hasAAddLoginQueueNotifyListener = false;

	// 有风险，没有区分业务层和框架层监听，在重登录的时候可能会有问题，暂时不会出现，因为现在只有业务调用 －－fish
	public static IDisposable RegistListener<T>(Action<T> callback) where T : class
	{
		var listener = GenericNotifyListener.Get<T>(); 
		AddListener(listener);
        return listener.Stream.LastValue != null 
	        ? listener.Stream.SubscribeAndFire<T>(callback) 
	        : listener.Stream.Subscribe<T>(callback);
	}

	public static void AddLoginQueueNotifyListener ()
    {
        if (!_hasAAddLoginQueueNotifyListener) {
            _hasAAddLoginQueueNotifyListener = true;
            SocketManager.Instance.AddMessageProcessor (new QueueDtoListener ());
        }
        //        Debug.LogError("AddLoginQueueNotifyListenerAddLoginQueueNotifyListenerAddLoginQueueNotifyListener");
    }

    private static void AddListener (MessageProcessor lis)
    {
        SocketManager.Instance.AddMessageProcessor (lis);
    }

    public static NotifyHandler AddNotifyHandler (Type type, NotifyHandler.OnHandleNotify onHandleNotify)
    {
        if (type == null || onHandleNotify == null)
            return null;

        //GameDebuger.Log( string.Format("<<< Listener Register ==> clsName = {0} , excuter = {1} >>>",clsName.ToString(), excuter.GetPropertyType().ToString()) );

        var notifyHandler = new NotifyHandler (type, onHandleNotify);
        AddListener (notifyHandler);
        return notifyHandler;
    }

    public static void RemoveNotifyHandler (MessageProcessor listener)
    {
        SocketManager.Instance.RemoveMessageProcessor (listener);
    }
    #endregion
}

