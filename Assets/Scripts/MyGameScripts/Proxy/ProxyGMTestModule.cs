using System.Collections.Generic;
using System.IO;
using AppDto;
using AppServices;
using GamePlot;
using UnityEngine;
using AssetPipeline;
using UnityEngine.SceneManagement;

public class ProxyGMTestModule
{
	private const string NAME = "GMTestView";

	public static void Open ()
	{
//		UIModuleManager.Instance.OpenFunModule<GMTestViewController> (NAME, UILayerType.DefaultModule, true);
		UIModuleManager.Instance.OpenFunModule<GMController>(NewGMView.NAME,UILayerType.Guide,true,true);
	}

	public static void Close ()
	{
        UIModuleManager.Instance.CloseModule(GMView.NAME);
        UIModuleManager.Instance.CloseModule (NAME);
		UIModuleManager.Instance.CloseModule(NewGMView.NAME);
	}
}