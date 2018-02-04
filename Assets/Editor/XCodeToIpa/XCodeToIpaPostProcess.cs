using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

public class XCodeToIpaPostProcess
{
    /// <summary>
    /// todo 根据类名动态获取路径
    /// </summary>
    public static readonly string FolderPath = Application.dataPath + "/Editor/XCodeToIpa";
    public static readonly string DefaultOptionsPlistPath = FolderPath + "/DefaultOptionsPlist.plist";

    public const string ShellFileName = "XCodeToIpaBuild.sh";
    public const string LogFileName = "XCodeToIpaLog.log";
    public const string ProvisioningProfile = "\"com tiancity xlsj for ALL ADHOC\"";

    [MenuItem("Tools/AutoIpa/Excute")]
    public static void Excute()
    {
		var folderPath = EditorUtility.OpenFolderPanel ("", "", "");
		if (string.IsNullOrEmpty (folderPath)) {
			return;
		}

		OnPostProcessBuild (BuildTarget.iOS, folderPath);
    }


    // [PostProcessBuild(1001)]
    public static void OnPostProcessBuild(BuildTarget target, string projectPath)
    {
        if (target != BuildTarget.iOS )
        {
            return;
        }

        Debug.Log("开始生成ipa！");
        // Debug.Log(projectPath);

        // // 覆盖旧文件
        var logFilePath = string.Format("{0}/{1}", FolderPath, LogFileName);
        // File.WriteAllText(logFilePath, null);

        var argList = new List<string>();
        argList.Add(string.Format("{0}/{1}", FolderPath, ShellFileName));
        argList.Add(logFilePath);
        argList.Add(projectPath);
        argList.Add (Path.GetFileName(projectPath));
        argList.Add (ProvisioningProfile);
        argList.Add(DefaultOptionsPlistPath);
        argList.Add(string.Format("> {0}", logFilePath));

        var cmd = string.Join(" ", argList.ToArray());
        Debug.Log(cmd);

        var process = System.Diagnostics.Process.Start("/bin/bash", cmd);
        process.WaitForExit();

//        Debug.Log(File.ReadAllText(logFilePath));
    }
}
