// **********************************************************************
// Copyright (c) 2013 Baoyugame. All rights reserved.
// File     :  MultipleNotifyListener.cs
// Author   : SK
// Created  : 2013/3/1
// Purpose  : 
// **********************************************************************

using System;
using System.Collections.Generic;

public class MultipleNotifyListener
{
    private List<NotifyHandler> _listener;
    private List<Type> _notifyClass;

    public MultipleNotifyListener()
    {
        _listener = new List<NotifyHandler>();
        _notifyClass = new List<Type>();
    }

    public void AddNotify(Type notifyClass)
    {
        _notifyClass.Add(notifyClass);
    }

    public void Start(NotifyHandler.OnHandleNotify onHandleNotify)
    {
        for (int i=0;i< _notifyClass.Count;i++)
        {
            Type notifyClass = _notifyClass[i];
            _listener.Add(NotifyListenerRegister.AddNotifyHandler(notifyClass, onHandleNotify));
        }
        _notifyClass.Clear();
    }

    public void Stop()
    {
        if (_listener != null)
        {
            for (int i = 0; i < _listener.Count; i++)
            {
                NotifyHandler handler = _listener[i];
                NotifyListenerRegister.RemoveNotifyHandler(handler);
            }
        }

        _listener = null;
        _notifyClass = null;
    }
}

