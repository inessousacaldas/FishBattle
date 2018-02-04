// **********************************************************************
// Copyright (c) 2013 Baoyugame. All rights reserved.
// File     :  BaseDtoListener.cs
// Author   : SK
// Created  : 2013/1/30
// Purpose  : 基本的的DTO监听处理, 需要被子类继承实现
// **********************************************************************

public abstract class BaseDtoListener<T> : MessageProcessor where T : class
{
    /**
	 * 获取激发消息处理器的事件类型
	 * 一般用 getQualifiedClassName(XXXDto);
	 */

    public string getEventType()
    {
		return typeof(T).FullName;
    }

    /// <summary>
    /// SocketManger收到信息后调用此接口来处理消息
    /// </summary>
    /// <param name="message"></param>
    public void ProcessMsg(object message)
    {
        HandleNotify(message as T);
    }

    protected abstract void HandleNotify(T notify);

    public void Dispose(){
        OnDispose();
    }

    protected virtual void OnDispose(){
    }
}