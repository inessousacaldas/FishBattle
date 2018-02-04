// **********************************************************************
// Copyright (c) 2016 cilugame. All rights reserved.
// File     : PlayerGameState.cs
// Author   : senkay <senkay@126.com>
// Created  : 3/29/2016 
// Porpuse  : 
// **********************************************************************
//

//玩家游戏状态，用来做游戏断线后的维持处理，例如挂机，自动寻路等
using AppDto;


public class PlayerGameState
{
	//自动巡逻
	public static bool IsAutoFram = false;

	//自动寻路
	public static Npc FollowTargetNpc = null;

	public static void Save()
	{
		IsAutoFram = ModelManager.Player.IsAutoFram;
		FollowTargetNpc = WorldManager.Instance.GetTargetNpc();
	}

	//重置
	public static void Reset()
	{
		IsAutoFram = false;
		FollowTargetNpc = null;
	}
}
