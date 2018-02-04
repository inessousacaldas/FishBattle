using UnityEngine;
using System;

public class ProxyServerListModule
{
	private const string NAME = "ServerListView";
	
	public static void Open(Action<GameServerInfo, AccountPlayerDto> selectCallback)
	{
		var controller = UIModuleManager.Instance.OpenFunModule<ServerListController>(NAME, UILayerType.ThreeModule, true);
		controller.Open (selectCallback);
	}
	
	public static void Close()
	{
		UIModuleManager.Instance.CloseModule (NAME);
	}
}

