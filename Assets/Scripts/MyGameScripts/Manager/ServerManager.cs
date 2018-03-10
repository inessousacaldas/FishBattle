// **********************************************************************
// Copyright (c) 2013 Baoyugame. All rights reserved.
// File     :  ServerManager.cs
// Author   : SK
// Created  : 2013/9/6
// Purpose  : 
// **********************************************************************

using System.Collections.Generic;

//服务器管理器
using AppDto;


public class ServerManager
{
	private static readonly ServerManager instance = new ServerManager();
    public static ServerManager Instance
    {
        get
		{
			return instance;
		}
    }	
	
	public delegate void OnRequestTokenDelegate(string token, string errorMsg);
	public OnRequestTokenDelegate OnRequestToken;		

	public delegate void OnRequestOrderIdDelegate(string orderId, string errorMsg);
	public OnRequestOrderIdDelegate OnRequestOrderId;		
	
	public static string gservice = "";

	public bool isGuest = false;
	public string sid = "";
	public string uid = "";
	
	private GameServerInfo _serverInfo;

	public LoginAccountDto loginAccountDto;

	public void SetServerInfo(GameServerInfo info){
		_serverInfo = info;
	}
	
	public GameServerInfo GetServerInfo(){
		return _serverInfo;
	}
	
	public AccountPlayerDto HasPlayerAtServer(int gameServerId)
	{
		long roleId = GameSetting.GetLastRolePlayerId();
        int i;
        if (loginAccountDto != null)
		{
			if (roleId == 0)
			{
                for ( i = 0; i < loginAccountDto.players.Count; i++)
                {
                    AccountPlayerDto dto = loginAccountDto.players[i];
                    if (dto.gameServerId == gameServerId)
                    {
                        return dto;
                    }
                }
			}
			else
			{
                for ( i = 0; i < loginAccountDto.players.Count; i++)
                {
                    AccountPlayerDto dto = loginAccountDto.players[i];
                    if (dto.gameServerId == gameServerId && dto.id == roleId)
                    {
                        return dto;
                    }
                }

                //if not find the last role then use the first accountPlayer at this server
                for ( i = 0; i < loginAccountDto.players.Count; i++)
                {
                    AccountPlayerDto dto = loginAccountDto.players[i];
                    if (dto.gameServerId == gameServerId)
                    {
                        return dto;
                    }
                }
			}
		}
		return null;
	}

	public List<AccountPlayerDto> GetPlayersAtServer(int gameServerId)
	{
		List<AccountPlayerDto> list = new List<AccountPlayerDto>();

		if (loginAccountDto != null)
		{
			List<AccountPlayerDto> players = loginAccountDto.players;
			for (int i=0; i<players.Count; i++)
			{
				AccountPlayerDto dto = players[i];
				if (dto.gameServerId == gameServerId)
				{
					list.Add(dto);
				}
			}
		}
		return list;
	}

	//获取该服务器上角色的最后登陆时间
	public long GetPlayerRecentLoginTime(int gameServerId)
	{
		long recentLoginTime = 0;
		List<AccountPlayerDto> list = GetPlayersAtServer(gameServerId);
		for(int i=0; i<list.Count; i++)
		{
			AccountPlayerDto dto = list[i];
			if (dto.recentLoginTime > recentLoginTime)
			{
				recentLoginTime = dto.recentLoginTime;
			}
		}

		return recentLoginTime;
	}

    public void DelectPlayer(AccountPlayerDto playerDtp)
    {
        if (loginAccountDto != null)
        {
            for (int i = 0; i < loginAccountDto.players.Count; i++)
            {
                AccountPlayerDto dto = loginAccountDto.players[i];
                if (dto.id == playerDtp.id)
                {
                    loginAccountDto.players.Remove(dto);
                    break;
                }
            }
        }
    }

	public AccountPlayerDto AddAccountPlayer(CreatePlayerDto dto)
	{
		TalkingDataHelper.SetupAccount(dto.gameServerId, dto.id, dto.name, dto.grade, dto.factionId, 1);
        TestinAgentHelper.SetUserInfo(dto.gameServerId + "_" + dto.id + "_" + dto.name);

		AccountPlayerDto accountPlayerDto = GetAccountPlayer(dto.id);
		if (accountPlayerDto == null)
		{
			accountPlayerDto = new AccountPlayerDto();
			accountPlayerDto.id = dto.id;
            accountPlayerDto.nickname = dto.name;
			accountPlayerDto.grade = dto.grade;
			accountPlayerDto.gameServerId = _serverInfo.serverId;
			accountPlayerDto.charactorId = dto.charactorId;
			accountPlayerDto.factionId = dto.factionId;
			accountPlayerDto.recentLoginTime = SystemTimeManager.Instance.GetUTCTimeStamp();

			loginAccountDto.players.Add(accountPlayerDto);
		}

		return accountPlayerDto;
	}

	public AccountPlayerDto AddAccountPlayer(PlayerDto dto)
	{
        GameDebuger.TODO(@"TalkingDataHelper.SetupAccount(dto.serviceId, dto.id, dto.name, dto.grade, dto.factionId, dto.gender);
            TestinAgentHelper.SetUserInfo(dto.serviceId + '_' + dto.id + '_' + dto.name);");

		AccountPlayerDto accountPlayerDto = GetAccountPlayer(dto.id);
		if (accountPlayerDto == null)
		{
			accountPlayerDto = new AccountPlayerDto();
			accountPlayerDto.id = dto.id;
            accountPlayerDto.nickname = dto.name;
			accountPlayerDto.grade = dto.grade;
			accountPlayerDto.gameServerId = _serverInfo.serverId;
			accountPlayerDto.charactorId = dto.charactorId;
			accountPlayerDto.factionId = dto.factionId;

			loginAccountDto.players.Add(accountPlayerDto);
		}

		return accountPlayerDto;
	}

	public void UpdateAccountPlayer(PlayerDto dto)
	{
		AccountPlayerDto accountPlayerDto = GetAccountPlayer(dto.id);
		if (accountPlayerDto != null)
		{
            accountPlayerDto.nickname = dto.name;
			accountPlayerDto.grade = dto.grade;
			accountPlayerDto.factionId = dto.factionId;
			accountPlayerDto.charactorId = dto.charactorId;
		}
	}

	public AccountPlayerDto GetAccountPlayer(long id)
	{
        for (int i = 0; i < loginAccountDto.players.Count; i++)
        {
            AccountPlayerDto dto = loginAccountDto.players[i];
            if (dto.id == id)
            {
                return dto;
            }
        }
		return null;
	}
	
	public int GetPlayerCount(int gameServerId)
	{
		int count = 0;
		if (loginAccountDto != null)
		{
            for (int i = 0; i < loginAccountDto.players.Count; i++)
            {
                AccountPlayerDto dto = loginAccountDto.players[i];
                if (dto.gameServerId == gameServerId)
                {
                    count++;
                }
            }
		}
		return count;
	}
}

