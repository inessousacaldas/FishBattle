


using System;
using System.Collections.Generic;
using UnityEngine;

public static class SystemProcess
{
	private const string MainJavaClassPath = "com.util.Process";


#if UNITY_EDITOR || UNITY_ANDROID
	private static AndroidJavaClass _mainJavaClass;

	private static AndroidJavaClass MainJavaClass
	{
		get
		{
			if (_mainJavaClass == null)
			{
				_mainJavaClass = JavaSdkUtils.GetUnityJavaClass(MainJavaClassPath);
			}
			return _mainJavaClass;
		}
	}
#endif

	public static string[] GetRunningProcess()
	{
		switch (Application.platform)
		{
			case RuntimePlatform.Android:
				{
#if UNITY_EDITOR || UNITY_ANDROID
					var processStr = JavaSdkUtils.CallSdkApi<string>(MainJavaClass, "GetRunningProcess");

					if (!string.IsNullOrEmpty(processStr))
					{
						return processStr.Split('#');
					}
#endif

					break;
				}
			default:
				{
					var runProcess = System.Diagnostics.Process.GetProcesses();
					var runProcessList = new List<string>();
					for (int i = 0; i < runProcess.Length; i++)
					{
						var process = runProcess[i];
						try
						{
							runProcessList.Add(process.ProcessName);
						}
						catch (Exception)
						{
							// ignored
						}
					}
					return runProcessList.ToArray();
				}
		}

		return null;
	}
}
