// **********************************************************************
// Copyright (c) 2013 Baoyugame. All rights reserved.
// File     :  Test.cs
// Author   : willson
// Created  : 2014/12/10 
// Porpuse  : 
// **********************************************************************


using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.IO;

public class AutoCreateAnimator : Editor {
	[MenuItem("Tools/AutoCreateAnimator", false, 2)]
	static void DoAutoCreateAnimator()
	{
		if (EditorUtility.DisplayDialog ("导出确认", "是否确认CreateAnimator?", "确认", "取消")) 
		{
			// Hero
			//HandleAnimationAssets("Hero");
			// Npc
			//HandleAnimationAssets("Npc");
			// Pet
			HandleAnimationAssets("Pet");
			//Fashion/body
			HandleAnimationAssets("Fashion/body");
			//Fashion/head
			HandleAnimationAssets("Fashion/head");

			Debug.Log(">>>>>>>>>>>  AutoCreateAnimator 完成,请按 \"Ctrl + S\" 保存");
		}
	}

	static string BaseACPath = "Assets/GameResources/ArtResources/Characters/AnimatorController/BaseAC.controller";
	static string AnimationPath = "Assets/GameResources/ArtResources/Characters/Animation/";

	static void HandleAnimationAssets(string type)
	{
		string path = "Assets/GameResources/ArtResources/Characters/{0}/";
		path = string.Format(path,type);
		DirectoryInfo directoryInfo = new DirectoryInfo(path);

		foreach (DirectoryInfo d in directoryInfo.GetDirectories())
		{
			if(d.Name.IndexOf("svn") == -1 && d.Name.IndexOf("Template") == -1)
			{
				DoCreateAnimationAssets(path,d.Name);
			}
		}
	}

	static void DoCreateAnimationAssets(string path,string actorName)
	{
		// 1.处理动作
		var controller = new AnimatorOverrideController();
		controller.runtimeAnimatorController = AssetDatabase.LoadAssetAtPath<UnityEditor.Animations.AnimatorController>(BaseACPath);

		SetAnimation(controller,path,actorName,ModelHelper.AnimType.idle);
		SetAnimation(controller,path,actorName,ModelHelper.AnimType.run);
		SetAnimation(controller,path,actorName,ModelHelper.AnimType.hit);

		SetAnimation(controller,path,actorName,ModelHelper.AnimType.death);
		SetAnimation(controller,path,actorName,ModelHelper.AnimType.show,ModelHelper.AnimType.idle);

		SetAnimation(controller,path,actorName,ModelHelper.AnimType.attack,ModelHelper.AnimType.attack);
		SetAnimation(controller,path,actorName,ModelHelper.AnimType.battle,ModelHelper.AnimType.idle);

		SetAnimation(controller,path,actorName,ModelHelper.AnimType.magic,ModelHelper.AnimType.attack);

		var animatorPath = SaveAnimatorOverrideController(controller,path,actorName);

		// 2.Prefab处理

		// A.判断 Prefab 是否存在
		string prefabDirectory = path + actorName + "/Prefabs/";
		GameObject prefab = AssetDatabase.LoadAssetAtPath(prefabDirectory + actorName + ".prefab",typeof(GameObject)) as GameObject;
		if(prefab == null)
		{
			string fbxPath = path + actorName + "/Meshes/" + actorName + ".FBX";
			GameObject obj = AssetDatabase.LoadAssetAtPath(fbxPath,typeof(GameObject)) as GameObject;
			if (obj != null)
			{
				// prefab 不存在,创建 Prefab
				if(!Directory.Exists(prefabDirectory))
				{
					Directory.CreateDirectory(prefabDirectory);
				}

				prefab = PrefabUtility.CreatePrefab(prefabDirectory + actorName + ".prefab",obj);

				// B.Prefab 关联动作
				Animator animator = prefab.GetComponent<Animator>();
				if (animator != null)
				{
					AnimatorOverrideController overrideController = AssetDatabase.LoadAssetAtPath<AnimatorOverrideController> (animatorPath);
					animator.runtimeAnimatorController = overrideController; 
				}
			}
		}
		else
		{
			// B.Prefab 关联动作
//			CamAnimator animator = prefab.GetComponent<CamAnimator>();
//			if (animator != null)
//			{
//				AnimatorOverrideController overrideController = Resources.LoadAssetAtPath<AnimatorOverrideController> (animatorPath);
//				animator.runtimeAnimatorController = overrideController; 
//			}
		}
	}

	/*
	 * 动作模型只有2种,1.完整模型动作 2.简化模型动作,详细请看
	 * http://oa.baoyugame.net/redmine/documents/27
	*/
	static void SetAnimation(
		AnimatorOverrideController controller
		,string path
		,string actorName
		,ModelHelper.AnimType animationName
		,ModelHelper.AnimType replaceAnimationName = ModelHelper.AnimType.idle)
	{
		var animationNameStr = ModelHelper.AnimToString(animationName );
		var replaceAnimationNameStr = ModelHelper.AnimToString(replaceAnimationName );
		var animationPath = GetAnimationClipPath(path,actorName,animationNameStr);
		var clip = AssetDatabase.LoadAssetAtPath(animationPath, typeof(AnimationClip)) as AnimationClip;
		if(clip != null)
		{
			controller[animationNameStr] = clip;
		}
		else if(!string.IsNullOrEmpty(replaceAnimationNameStr))
		{
			animationPath = GetAnimationClipPath(path,actorName,replaceAnimationNameStr);
			clip = AssetDatabase.LoadAssetAtPath(animationPath, typeof(AnimationClip)) as AnimationClip;
			if(clip != null)
			{
				controller[animationNameStr] = clip;
			}
			else
			{
				Debug.LogError(actorName + " 动作模型不符合规范,缺少 [替换] 动作: " + replaceAnimationName);
			}
		}
		else
		{
			Debug.LogError(actorName + " 动作模型不符合规范,缺少动作: " + animationName);
		}
	}

	static string GetAnimationClipPath(string path,string actorName,string animationName)
	{
		bool isFashion = false;
		if (actorName.EndsWith("_body") || actorName.EndsWith("_head"))
		{
			isFashion = true;
		}

		string animName = actorName;

		if (actorName.Contains("_"))
		{
			string petId = actorName.Split ('_') [1];
			animName = "anim_" + petId;
		}

		if (isFashion)
		{
			animName = animName + "_fashion";
		}

		return AnimationPath + animName + "/" + animName + "@" + animationName + ".FBX";
	}

	static string SaveAnimatorOverrideController(AnimatorOverrideController controller,string path,string actorName)
	{
		bool isFashion = false;
		if (actorName.EndsWith("_body") || actorName.EndsWith("_head"))
		{
			isFashion = true;
		}

		string animName = actorName;
		
		if (actorName.Contains("_"))
		{
			string petId = actorName.Split ('_') [1];
			animName = "anim_" + petId;
		}

		if (isFashion)
		{
			animName = animName + "_fashion";
		}

		string animatorDirectory = AnimationPath + animName + "/";
		if(!Directory.Exists(animatorDirectory))
		{
			Directory.CreateDirectory(animatorDirectory);
		}

		string animatorPath = animatorDirectory + animName + ".overrideController";
		AssetDatabase.CreateAsset(controller,animatorPath);

		return animatorPath;
	}
}

