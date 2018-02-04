using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public class SVNTool
{
    [MenuItem("Tools/SVN/Update", false, 1)]
    static void SVNUpdate()
    {
        string applicationPath = Application.dataPath;
        ProcessCommand("TortoiseProc.exe", "/command:update /path:" + Application.dataPath + " /closeonend:0");
    }

    [MenuItem("Tools/SVN/Commit", false, 2)]
    static void SVNCommit()
    {
        string applicationPath = Application.dataPath.Replace("/Assets", "");
        List<string> pathList = new List<string>();
        pathList.Add(Application.dataPath);
        pathList.Add(applicationPath + "/ProjectSettings");

        string commitPath = string.Join("*",pathList.ToArray());
        ProcessCommand("TortoiseProc.exe", "/command:commit /path:" + commitPath);
    }

    [MenuItem("Tools/SVN/", false, 3)]
    static void Breaker() { }

    [MenuItem("Tools/SVN/CleanUp", false, 4)]
    static void SVNCleanUp()
    {
        string applicationPath = Application.dataPath;
        ProcessCommand("TortoiseProc.exe", "/command:cleanup /path:" + Application.dataPath.Replace("/Assets", ""));
    }

    [MenuItem("Tools/SVN/Log", false, 5)]
    static void SVNLog()
    {
        string applicationPath = Application.dataPath;
        ProcessCommand("TortoiseProc.exe", "/command:log /path:" + Application.dataPath.Replace("/Assets", ""));
    }

    public static void ProcessCommand(string command,string argument)
    {
        System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo(command);
        info.Arguments = argument;
        info.CreateNoWindow = false;
        info.ErrorDialog = true;
        info.UseShellExecute = true;

        if (info.UseShellExecute)
        {
            info.RedirectStandardOutput = false;
            info.RedirectStandardError = false;
            info.RedirectStandardInput = false;
        }
        else
        {
            info.RedirectStandardOutput = true;
            info.RedirectStandardError = true;
            info.RedirectStandardInput = true;
            info.StandardOutputEncoding = System.Text.UTF8Encoding.UTF8;
            info.StandardErrorEncoding = System.Text.UTF8Encoding.UTF8;
        }

        System.Diagnostics.Process process = System.Diagnostics.Process.Start(info);
        if (!info.UseShellExecute)
        {
            Debug.Log(process.StandardOutput);
            Debug.Log(process.StandardError);
        }

        process.WaitForExit();
        process.Close();
    }
}
