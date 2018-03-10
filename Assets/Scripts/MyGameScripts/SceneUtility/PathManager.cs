using UnityEngine;
using System.Collections;

public class PathManager {

	public static string GetNavmeshPath (string sceneName)
	{
		string rootDir = GetPersistentAssetsDir ();
		string navmeshDir = System.IO.Path.Combine (rootDir, "config/navmesh");
		string path = System.IO.Path.Combine (navmeshDir, "navmesh_" + sceneName + ".json");
		return path;
	}
		
	public static string GetPersistentAssetsDir ()
	{
		#if TOOL
		return GetInitAssetsDir ();
		#endif
		return Application.persistentDataPath + "/res";
	}
		
	public static string GetInitAssetsDir ()
	{
		//return Application.streamingAssetsPath;
		return Application.dataPath + "/../res";
	}

	public static string GetABOutputDir ()
	{
		return Application.dataPath + "/../res";
	}
	public static string GetWWWPre ()
	{
		#if UNITY_STANDALONE || UNITY_EDITOR
			return "file:///";
		#elif UNITY_ANDROID
			return "jar:file:///";
		#elif UNITY_IOS
			return "file:///"; 
		#endif
	}

}
