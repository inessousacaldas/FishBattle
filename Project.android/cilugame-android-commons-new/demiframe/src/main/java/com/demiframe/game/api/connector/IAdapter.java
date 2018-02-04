package com.demiframe.game.api.connector;

public abstract interface IAdapter
{
  public abstract IActivity activityProxy();

  public abstract IExit exitProxy();

  public abstract IExtend extendProxy();

  public abstract IPay payProxy();

  public abstract IToolBar toolbarProxy();

  public abstract IUserManager userManagerProxy();

}