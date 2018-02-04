using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class NGUIDepthConfig
{
	private List<NGUIPrefabInfo> _infoList = new List<NGUIPrefabInfo>();

	public List<NGUIPrefabInfo> InfoList
	{
		get { return _infoList; }
	}

	public static NGUIDepthConfig CreateConfig(string[] searchPathList)
	{
		var config = new NGUIDepthConfig();

		var guids = AssetDatabase.FindAssets("t:Prefab", searchPathList);
		foreach (var guid in guids)
		{
            config.AddPrefab(AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(guid)) as GameObject);
		}

		return config;
	}


	private void AddPrefab(GameObject go)
	{
		_infoList.Add(new NGUIPrefabInfo(go));
	}
}
