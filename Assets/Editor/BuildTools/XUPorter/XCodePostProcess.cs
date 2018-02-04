using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.XCodeEditor;
#endif
using System.IO;

public static class XCodePostProcess
{
	public static string SelectProjmods = "";

//	[MenuItem("Custom/XCodePostProcess")]
	public static void PostProcessBuild()
	{
		OnPostProcessBuild (BuildTarget.iOS, "/Users/cilu2/Workspaces/H1/xcode/appstore");
	}

	#if UNITY_EDITOR
	[PostProcessBuild(999)]
	public static void OnPostProcessBuild( BuildTarget target, string pathToBuiltProject )
	{
		if (target != BuildTarget.iOS) {
			Debug.LogWarning("Target is not iPhone. XCodePostProcess will not run");
			return;
		}

        XCFileChecker.InitModeDict();

		//得到xcode工程的路径
		string path = Path.GetFullPath (pathToBuiltProject);
		// Create a new project object from build target
		XCProject project = new XCProject( pathToBuiltProject );

		// Find and run through all projmods files to patch the project.
		// Please pay attention that ALL projmods files in your project folder will be excuted!
		string applicationPath = Application.dataPath.Replace ("/Assets", "");
		string[] files = Directory.GetFiles( applicationPath + "/Mods", "*.projmods", SearchOption.TopDirectoryOnly );

        XCFileChecker.SortMods(ref files);

		string selectProjmods = EditorPrefs.GetString("selectProjmods");
		Debug.Log("selectProjmods = " + selectProjmods);

		foreach( string file in files ) {
			if (string.IsNullOrEmpty(selectProjmods))
			{
				UnityEngine.Debug.Log("ProjMod File: "+file);
				project.ApplyMod( file );
			}
			else
			{
				if (XCFileChecker.CheckApplyMod(file))
				{
					UnityEngine.Debug.Log("ProjMod File: "+file);
					project.ApplyMod( file );
				}
			}
		}

		//TODO implement generic settings as a module option
		project.overwriteBuildSetting("CODE_SIGN_IDENTITY[sdk=iphoneos*]", "iPhone Developer", "Release");

		//modify for xcode7
		project.overwriteBuildSetting("ENABLE_BITCODE", "NO", "all");


        XCFileChecker.EditCode(path);

//		if (selectProjmods == "kuaiyongSDK.projmods")
//		{
//			//编辑代码文件
//			EditorCodeForKuaiyong(path);
//		}

		// Finally save the xcode project
		project.Save();

	}
	#endif

	/// <summary>
	/// 修改快用渠道的打包需求， 修改UnityAppController.mm文件，屏蔽部分代码来控制SDK的横竖控制
	/// </summary>
	/// <param name="filePath">File path.</param>
	private static void EditorCodeForKuaiyong(string filePath)
	{
		//读取UnityAppController.mm文件
		XClass UnityAppController = new XClass(filePath + "/Classes/UnityAppController.mm");

		//在指定代码后面增加一行代码
		UnityAppController.WriteBelow("\t[_unityView didRotate];\n}","/*");

		//在指定代码中替换一行
		//UnityAppController.Replace("return YES;","return [ShareSDK handleOpenURL:url sourceApplication:sourceApplication annotation:annotation wxDelegate:nil];");

		//在指定代码后面增加一行
		UnityAppController.WriteBelow("\t\t   | (1 << UIInterfaceOrientationLandscapeRight) | (1 << UIInterfaceOrientationLandscapeLeft);\n}","*/");

	}

	public static void Log(string message)
	{
		UnityEngine.Debug.Log("PostProcess: "+message);
	}
}
