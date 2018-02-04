// **********************************************************************
// Copyright (c) 2013 Baoyugame. All rights reserved.
// File     :  BaseDtoExcuteListener.cs
// Author   : SK
// Created  : 2013/3/1
// Purpose  : 
// **********************************************************************

using System;

public class NotifyHandler : MessageProcessor
{
    private Type _type;
    public delegate void OnHandleNotify(object notify);
    private OnHandleNotify _handler;

    public NotifyHandler(Type type, OnHandleNotify handler)
    {
        _type = type;
        _handler = handler;
    }

    public string getEventType()
    {
		return _type.FullName;
    }

    public void ProcessMsg(object message)
    {
        if (_handler != null)
            _handler(message);
    }

    public void Dispose(){
    }
}

