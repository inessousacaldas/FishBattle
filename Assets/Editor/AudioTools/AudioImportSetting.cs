// **********************************************************************
// Copyright (c) 2015 cilugame. All rights reserved.
// File     : AudioImportSetting.cs
// Author   : senkay <senkay@126.com>
// Created  : 09/16/2015 
// Porpuse  : 
// **********************************************************************
//
using UnityEngine;
using UnityEditor;

public class AudioImportSetting : EditorWindow
{
	/// <summary>
	/// 创建、显示窗体
	/// </summary>
	[@MenuItem("OptimizationTool/Audio Import Settings")]
	private static void Init()
	{    
		AudioImportSetting window = (AudioImportSetting)EditorWindow.GetWindow(typeof(AudioImportSetting), true, "AudioImportSetting");
		window.Show();
	}

	/// <summary>
	/// 显示窗体里面的内容
	/// </summary>
	private void OnGUI()
	{
		if (GUILayout.Button("Set Audio ImportSettings"))
			LoopSetAudio();
	}

	/// <summary>
	/// 循环设置选择的贴图
	/// </summary>
	private void LoopSetAudio()
	{
		Object[] audioClips = GetSelectedAudios();
		Selection.objects = new Object[0];
		foreach (AudioClip audioClip in audioClips)
		{
			string path = AssetDatabase.GetAssetPath(audioClip);
			AudioImporter audImporter = AssetImporter.GetAtPath(path) as AudioImporter;
            audImporter.forceToMono = true;
			AssetDatabase.ImportAsset(path);
		}
	}
	
	/// <summary>
	/// 获取选择的贴图
	/// </summary>
	/// <returns></returns>
	private Object[] GetSelectedAudios()
	{
		return Selection.GetFiltered(typeof(AudioClip), SelectionMode.DeepAssets);
	}
}

