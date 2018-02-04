using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Debug = UnityEngine.Debug;

public static class ProcessHelper
{
    public static ProcessStartInfo CreateStartInfo(string fileName = null, string arguments = null, bool createNoWindow = true, bool useShell = false)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsEditor:
                    {
                        // win默认
                        fileName = "cmd.exe";
                        break;
                    }
                case RuntimePlatform.OSXEditor:
                    {
                        // mac默认
                        fileName = "/bin/bash";
                        break;
                    }
            }
        }

        var startInfo = new ProcessStartInfo()
        {
            FileName = fileName,
            Arguments = arguments ?? "",
            CreateNoWindow = createNoWindow,
            UseShellExecute = useShell,
            ErrorDialog = true,
            RedirectStandardInput = true,
            RedirectStandardOutput = !useShell,
            RedirectStandardError = !useShell,
//            StandardOutputEncoding = Encoding.UTF8,
//            StandardOutputEncoding = Encoding.GetEncoding(65001),
//            StandardErrorEncoding = Encoding.UTF8,
//            StandardErrorEncoding = Encoding.GetEncoding(65001),
        };

        return startInfo;
    }

    /// <summary>
    /// 为了方便记忆使用这个容器
    /// </summary>
    /// <returns></returns>
    public static List<string> CreateArgumentsContainer()
    {
        return new List<string>();
    }


    /// <summary>
    /// 方便记忆同时打印参数
    /// </summary>
    /// <param name="argList"></param>
    /// <returns></returns>
    public static string CreateArguments(List<string> argList)
    {
        var arguments = string.Join(" ", argList.ToArray());
        Debug.Log("Process Arguments: " + arguments);

        return arguments;
    }


    /// <summary>
    /// 封装方便以后使用
    /// </summary>
    /// <param name="startInfo"></param>
    /// <returns></returns>
    public static Process Start(ProcessStartInfo startInfo)
    {
        return Process.Start(startInfo);
    }

    public static void WriteLine(Process process, params string[] args)
    {
        // win下加个换行否则乱码
        process.StandardInput.WriteLine();
        process.StandardInput.WriteLine("chcp 65001");
        if (args != null)
        {
            foreach (var arg in args)
            {
                if (arg == null)
                {
                    process.StandardInput.WriteLine();
                }
                else
                {
                    process.StandardInput.WriteLine(arg);
                }
            }
        }
        // 运行为退出程序
        process.StandardInput.WriteLine("exit");
    }

    public static void WaitForExit(Process process, int exitCode = 0)
    {
        var result = process.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();
        process.WaitForExit();
        var exit = process.ExitCode;
        process.Close();

        Debug.Log(result);
//        Debug.Log(error);
//        Debug.Log("Process ExitCode: " + exit);
        if (exit != exitCode)
        {
            throw new Exception(string.Format("Process ExitCode: {0}\n{1}", exit, error));
        }
    }
}
