using UnityEngine;
using System;
using AppDto;

public class ProxyRoleCreateModule
{
	public const string NAME = "RoleCreateView";

	public static void Open(GameServerInfo info, Action<GeneralResponse> onCreatePlayerSuccess)
	{
        SPSDK.gameEvent("10017");       //打开创角界面
		GameObject ui = UIModuleManager.Instance.OpenFunModule(NAME, UILayerType.SubModule, false);
		
		var controller = ui.GetMissingComponent<RoleCreateController>();
		controller.Open (info, onCreatePlayerSuccess);
	}

	public static void Close()
	{
		UIModuleManager.Instance.CloseModule (NAME);
	}

	public static bool IsOpen()
	{
		return UIModuleManager.Instance.IsModuleCacheContainsModule(NAME);
	}
}