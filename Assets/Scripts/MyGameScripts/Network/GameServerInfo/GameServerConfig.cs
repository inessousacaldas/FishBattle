// **********************************************************************
// Copyright  2013 Baoyugame. All rights reserved.
// File     :  ServerInfo.cs
// Author   : senkay
// Created  : 4/8/2013 5:47:24 PM
// Purpose  : 
// **********************************************************************

using System.Collections.Generic;

/// <summary>
///     服务器列表
/// </summary>
public class GameServerConfig
{
    public List<GameServerInfo> serverInfoList = new List<GameServerInfo>();
    public List<AreaInfo> areaInfoList = new List<AreaInfo>();
}

public class AreaInfo
{
    public int id;
    public string name;
}

public class GameServerInfo
{
    //区
    public int areaId;

    //服务器维护状态 0：关闭   1:开放
    public int dboState = 1;

    //目标服务器编号 (合服后可能会变)
    public int destServerId;

    //HA版本 0 old  1 new
    public int haVer = 0;
    //host
    public string host;

    //服务器名称
    public string name;

    //是否需要充值url
    public bool needPayUrl;
    //开服时间
    public long openTime;
    //服务器端口
    public int port;

    //是否推荐  >0 表示推荐服
    public int recommendType;

	//是否新服 开服时间小于2周，表示新服
	public bool newServer;

    //服务器状态 0：流畅  1：火爆 2: 繁忙 3: 维护
    public int runState;
    //游戏服编号 (合服后不变)
    public int serverId;

    //服务器接入HA匹配id
    public int serviceId;

    //------------------------------

    //获取服务器唯一id
    public string GetServerUID()
    {
        return host + "|" + serviceId + "|" + serverId;
    }

    //是否审核服务器
    public bool isTestServer()
    {
        return serverId == 1000;
    }
}