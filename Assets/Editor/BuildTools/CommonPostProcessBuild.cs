using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using AssetPipeline;
using UnityEditor;
using UnityEditor.Callbacks;
using Debug = UnityEngine.Debug;


/// <summary>
/// 对一般的导出工程进行处理
/// </summary>
public static class CommonPostProcessBuild
{
//    [MenuItem("Test/Test _%#R")]
    public static void Test()
    {
        var path =
            @"D:\Workspace\XClient\XClient_Android\APK\Xlsj\LocalDev\nucleus\Assembly-UnityScript-firstpass_9C35BD34E1827CEF2396755469EB6828.dll";
        File.WriteAllBytes(path + "t", DllHelper.DecryptDll(File.ReadAllBytes(path)));
    }
    #region 获取dll重新编译不变的部分

    private const string IldasmPath = @"C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.6 Tools\ildasm.exe";
    private static readonly Regex TimeDateStampRgex = new Regex(@"^// Time-date stamp:.*$", RegexOptions.Multiline);
    private static readonly Regex ImageBaseRgex = new Regex(@"^// Image base:.*$", RegexOptions.Multiline);
    private static readonly Regex MVIDRgex = new Regex(@"^// MVID:.*$", RegexOptions.Multiline);

    private static readonly Regex AllRgex = new Regex(@"^// ((Image base)|(MVID)):.*$", RegexOptions.Multiline);
    //    private static readonly Regex AllRgex = new Regex(@"(^// Time-date stamp:.*$)|(^// Image base:.*$)|(^// MVID:.*$)", RegexOptions.Multiline);


    /// <summary>
    /// 这个方法由于生成的文件比较大，不适用，会out of memory
    /// </summary>
    /// <param name="dllPath"></param>
    /// <returns></returns>
    public static string GetDllUniquePart(string dllPath)
    {
        if (!File.Exists(IldasmPath) || !File.Exists(dllPath))
        {
            throw new Exception(string.Format("{0} or {1} not exists!", IldasmPath, dllPath));
        }

        var argList = new List<string>();
        //        argList.Add("/ALL");
        argList.Add("/TEXT");
        //        argList.Add("/UTF8");
        argList.Add(string.Format("\"{0}\"", dllPath));
        var cmd = string.Join(" ", argList.ToArray());
        Debug.Log(cmd);

        var p = new Process();
        //        p.StartInfo.FileName = "cmd.exe";
        p.StartInfo.FileName = IldasmPath;
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.RedirectStandardInput = true;
        p.StartInfo.RedirectStandardOutput = true;
        p.StartInfo.CreateNoWindow = true;
        p.StartInfo.Arguments = cmd;

        p.Start();
        //        p.StandardInput.WriteLine();
        //        p.StandardInput.WriteLine(cmd);
        p.StandardInput.WriteLine("exit");
        var result = p.StandardOutput.ReadToEnd();
        //        var result = new StringBuilder(p.StandardOutput.ReadToEnd());
        p.WaitForExit();
        p.Close();

        //        result = TimeDateStampRgex.Replace(result, String.Empty);
        //        result = ImageBaseRgex.Replace(result.ToString(), String.Empty);
        //        result = MVIDRgex.Replace(result, String.Empty);
        result = AllRgex.Replace(result, String.Empty, 2);
        //        Debug.Log(result);

        return result;
        //        return result.ToString();
    }


    public static byte[] GetDllUniquePartByBytes(string dllPath)
    {
        var bytes = File.ReadAllBytes(dllPath);

        // Time-date stamp
        bytes[136] = 0;
        bytes[137] = 0;

        var needResetGuid = true;
        var guidCount = 0;
        var guidCheckStartLast = 2000;
        var guidCheckLength = 24;
        var guidLength = 16;
        for (int i = 0; i < bytes.Length; i++)
        {
            // MVID or Image Base
            if (needResetGuid && i > bytes.Length - guidCheckStartLast && i > bytes.Length * 0.8)
            {
                if (bytes[i] == 0)
                {
                    guidCount++;
                    // 取巧而已
                    if (guidCount == guidCheckLength)
                    {
                        var startIndex = i - guidCheckLength - guidLength + 1;
                        for (int j = startIndex; j < startIndex + guidLength; j++)
                        {
                            bytes[j] = 0;
                        }
                        needResetGuid = false;
                    }
                }
                else
                {
                    guidCount = 0;
                }
            }
        }

        return bytes;
    }

    #endregion
    #region 生成Android平台的dll

    private const string ApkToolPath = "Project.android/build-tools/apktool_2.0.1.jar";
    private const string armv7a_libmono = "Project.cpp/mono/libs/armv7a/libmono.so";
    private const string x86_libmono = "Project.cpp/mono/libs/x86/libmono.so";

    public static void GenerateAndroidDll(string pathToBuildProject)
    {
        DecodeApk(pathToBuildProject);
        ReplaceAndroidMonoDll(pathToBuildProject);
        BackupAndroidDll(pathToBuildProject);
        EncryptAndroidBackupDll(pathToBuildProject);
        EncryptAndroidManagedDll(pathToBuildProject);
        GenerateAndroidDllConfig(pathToBuildProject);
        EncodeApk(pathToBuildProject);
        SignApk(pathToBuildProject);
    }

    private static string GetDecodeApkFolder(string pathToBuildProject)
    {
        return Path.GetDirectoryName(pathToBuildProject) + "/" + Path.GetFileNameWithoutExtension(pathToBuildProject) + "_out";
    }


    private static void DecodeApk(string pathToBuildProject)
    {
        FileHelper.DeleteDirectory(GetDecodeApkFolder(pathToBuildProject), true);

        var argList = new List<string>();
        argList.Add("java -jar");
        argList.Add(ApkToolPath);
        argList.Add("d");
        argList.Add(pathToBuildProject);
        argList.Add("-o");
        argList.Add(GetDecodeApkFolder(pathToBuildProject));
        var cmd = string.Join(" ", argList.ToArray());

        var p = new Process();
        p.StartInfo.FileName = "cmd.exe";
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.RedirectStandardInput = true;
        p.StartInfo.RedirectStandardOutput = true;
        p.StartInfo.RedirectStandardError = true;
        p.StartInfo.CreateNoWindow = true;
        //        p.StartInfo.Arguments = cmd;

        p.Start();
        p.StandardInput.WriteLine();
        p.StandardInput.WriteLine(cmd);
        p.StandardInput.WriteLine("exit");
        var result = p.StandardOutput.ReadToEnd();
        var error = p.StandardError.ReadToEnd();
        p.WaitForExit();
        p.Close();

        Debug.Log(result);
        Debug.Log(error);
    }


    private static void ReplaceAndroidMonoDll(string pathToBuildProject)
    {
        var decodePath = GetDecodeApkFolder(pathToBuildProject);
        var armv7aPath = decodePath + "/lib/armeabi-v7a/libmono.so";
        var x86Path = decodePath + "/lib/x86/libmono.so";
        File.Copy(armv7a_libmono, armv7aPath, true);
        File.Copy(x86_libmono, x86Path, true);
    }


    private static string GetAndroidBackupDllFolder(string pathToBuildProject)
    {
        return Path.GetDirectoryName(pathToBuildProject) + "/" + Path.GetFileNameWithoutExtension(pathToBuildProject) + "_Dll";
    }


    private static string GetAndroidManagedDllFolder(string pathToBuildProject)
    {
        return GetDecodeApkFolder(pathToBuildProject) + "/assets/bin/Data/Managed";
    }


    private static void BackupAndroidDll(string pathToBuildProject)
    {
        FileHelper.DeleteDirectory(GetAndroidBackupDllFolder(pathToBuildProject), true);
        FileUtil.CopyFileOrDirectory(GetAndroidManagedDllFolder(pathToBuildProject), GetAndroidBackupDllFolder(pathToBuildProject));

        foreach (var file in Directory.GetFiles(GetAndroidBackupDllFolder(pathToBuildProject), "*.*"))
        {
            if (!DllHelper.IsProjectDll(Path.GetFileNameWithoutExtension(file)))
            {
                FileHelper.DeleteFile(file);
            }
        }
    }


    private static void EncryptAndroidBackupDll(string pathToBuildProject)
    {
        foreach (var file in Directory.GetFiles(GetAndroidBackupDllFolder(pathToBuildProject), "*.dll"))
        {
            var name = Path.GetFileNameWithoutExtension(file);
            var zipBytes = ZipLibUtils.Compress(FileHelper.ReadAllBytes(file));
            // 非项目dll不做加密处理
            FileHelper.WriteAllBytes(file, DllHelper.IsProjectDll(name) ? DllHelper.EncryptDll(zipBytes) : zipBytes);
        }
    }


    private static void EncryptAndroidManagedDll(string pathToBuildProject)
    {
        foreach (var file in Directory.GetFiles(GetAndroidManagedDllFolder(pathToBuildProject), "*.dll"))
        {
            var name = Path.GetFileNameWithoutExtension(file);
            if (DllHelper.IsProjectDll(name))
            {
                FileHelper.WriteAllBytes(file, DllHelper.EncryptDll(FileHelper.ReadAllBytes(file)));
            }
        }
    }


    private static void GenerateAndroidDllConfig(string pathToBuildProject)
    {
        var backupFolder = GetAndroidBackupDllFolder(pathToBuildProject);
        var dllVersion = new DllVersion();

        foreach (var file in Directory.GetFiles(backupFolder, "*.dll"))
        {
            var name = Path.GetFileNameWithoutExtension(file);
            var md5 = MD5Hashing.HashFile(file);
            var size = (new FileInfo(file)).Length;
            var dllInfo = new DllInfo()
            {
                dllName = name,
                MD5 = md5,
                size = size,
            };
            dllVersion.Manifest.Add(name, dllInfo);
            FileUtil.MoveFileOrDirectory(file, backupFolder + "/" + dllInfo.ToFileName());
        }

        dllVersion.Version = DateTime.UtcNow.ToFileTimeUtc();

        FileHelper.SaveJsonObj(dllVersion, backupFolder + "/" + dllVersion.ToFileName());
        FileHelper.SaveJsonObj(dllVersion, GetDecodeApkFolder(pathToBuildProject) + "/assets/dllVersion.json");
    }


    private static string GetUnSignedApkPath(string pathToBuildProject)
    {
        return Path.GetDirectoryName(pathToBuildProject) + "/" + Path.GetFileNameWithoutExtension(pathToBuildProject) + "_unsigned.apk";
    }


    private static string GetFinalApkPath(string pathToBuildProject)
    {
        return Path.GetDirectoryName(pathToBuildProject) + "/" + Path.GetFileNameWithoutExtension(pathToBuildProject) + "_final.apk";
    }

    private static void EncodeApk(string pathToBuildProject)
    {
        var unsignedApk = GetUnSignedApkPath(pathToBuildProject);
        FileHelper.DeleteFile(unsignedApk);

        var argList = new List<string>();
        argList.Add("java -jar");
        argList.Add(ApkToolPath);
        argList.Add("b");
        argList.Add(GetDecodeApkFolder(pathToBuildProject));
        argList.Add("-o");
        argList.Add(unsignedApk);
        var cmd = string.Join(" ", argList.ToArray());

        var p = new Process();
        p.StartInfo.FileName = "cmd.exe";
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.RedirectStandardInput = true;
        p.StartInfo.RedirectStandardOutput = true;
        p.StartInfo.RedirectStandardError = true;
        p.StartInfo.CreateNoWindow = true;
        //        p.StartInfo.Arguments = cmd;

        p.Start();
        p.StandardInput.WriteLine();
        p.StandardInput.WriteLine(cmd);
        p.StandardInput.WriteLine("exit");
        var result = p.StandardOutput.ReadToEnd();
        var error = p.StandardError.ReadToEnd();
        p.WaitForExit();
        p.Close();

        Debug.Log(result);
        Debug.LogError(error);
    }


    private static void SignApk(string pathToBuildProject)
    {
        var unsignedApk = GetUnSignedApkPath(pathToBuildProject);
        var finalApk = GetFinalApkPath(pathToBuildProject);
        FileHelper.DeleteFile(finalApk);

        var argList = new List<string>();
        argList.Add("jarsigner");
        argList.Add("-keystore PublishKey/nucleus.keystore");
        argList.Add("-storepass nucleus123");
        argList.Add("-signedjar");
        argList.Add(finalApk);
        argList.Add(unsignedApk);
        argList.Add("nucleus");
        var cmd = string.Join(" ", argList.ToArray());

        var p = new Process();
        p.StartInfo.FileName = "cmd.exe";
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.RedirectStandardInput = true;
        p.StartInfo.RedirectStandardOutput = true;
        p.StartInfo.RedirectStandardError = true;
        p.StartInfo.CreateNoWindow = true;
        //        p.StartInfo.Arguments = cmd;

        p.Start();
        p.StandardInput.WriteLine();
        p.StandardInput.WriteLine(cmd);
        p.StandardInput.WriteLine("exit");
        var result = p.StandardOutput.ReadToEnd();
        var error = p.StandardError.ReadToEnd();
        p.WaitForExit();
        p.Close();

        Debug.Log(result);
        Debug.LogError(error);

        // 生成成功，这个也没什么用了
        FileHelper.DeleteFile(unsignedApk);
    }

    #endregion

    #region 生成Win平台的dll

    public static void GenerateWinDll(string pathToBuiltProject)
    {
        if (GameSetting.LoadGameSettingData().platformType != GameSetting.PlatformType.Win)
        {
            return;
        }

        Debug.Log(pathToBuiltProject);
        var rootFolder = Path.GetDirectoryName(pathToBuiltProject);
        var dataPath = string.Format("{0}/{1}_Data", rootFolder, Path.GetFileNameWithoutExtension(pathToBuiltProject));

        ReplaceWinMonoDll(dataPath);
        BackupWinDll(rootFolder, dataPath);
        EncryptWinBackupDll(rootFolder);
        EncryptWinManagedDll(dataPath);
        GenerateWinDllConfig(rootFolder, dataPath);
    }

    private static void ReplaceWinMonoDll(string dataPath)
    {
        var monoName = "mono.dll";
        var gameMonoFolder = string.Format("{0}/Mono", dataPath);
        var projectMonoFolder = EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneWindows64
            ? "Project.cpp/mono_win/libs/win64"
            : "Project.cpp/mono_win/libs/win32";
        File.Copy(string.Format("{0}/{1}", projectMonoFolder, monoName), string.Format("{0}/{1}", gameMonoFolder, monoName), true);
    }

    private static string GetWinBackupDllFolder(string rootFolder)
    {
        return rootFolder + "_Dll";
    }

    private static string GetWinManagedDllFolder(string dataPath)
    {
        return dataPath + "/" + GameResPath.MANAGER_ROOT;
    }

    private static void BackupWinDll(string rootFolder, string dataPath)
    {
        var dllFolder = GetWinManagedDllFolder(dataPath);
        var backupFolder = GetWinBackupDllFolder(rootFolder);
        FileHelper.DeleteDirectory(backupFolder, true);
        FileUtil.CopyFileOrDirectory(dllFolder, backupFolder);

        foreach (var file in Directory.GetFiles(backupFolder, "*.*"))
        {
            if (!DllHelper.IsProjectDll(Path.GetFileNameWithoutExtension(file)))
            {
                FileHelper.DeleteFile(file);
            }
        }
    }

    private static void EncryptWinBackupDll(string rootFolder)
    {
        var backupFolder = GetWinBackupDllFolder(rootFolder);
        foreach (var file in Directory.GetFiles(backupFolder, "*.dll"))
        {
            var name = Path.GetFileNameWithoutExtension(file);
            var zipBytes = ZipLibUtils.Compress(FileHelper.ReadAllBytes(file));
            // 非项目dll不做加密处理
            FileHelper.WriteAllBytes(file, DllHelper.IsProjectDll(name) ? DllHelper.EncryptDll(zipBytes) : zipBytes);
        }
    }

    private static void EncryptWinManagedDll(string dataPath)
    {
        var dllFolder = GetWinManagedDllFolder(dataPath);
        foreach (var file in Directory.GetFiles(dllFolder, "*.dll"))
        {
            var name = Path.GetFileNameWithoutExtension(file);
            if (DllHelper.IsProjectDll(name))
            {
                FileHelper.WriteAllBytes(file, DllHelper.EncryptDll(FileHelper.ReadAllBytes(file)));
            }
        }
    }

    private static void GenerateWinDllConfig(string rootFolder, string dataPath)
    {
        var backupFolder = GetWinBackupDllFolder(rootFolder);
        var dllVersion = new DllVersion();

        foreach (var file in Directory.GetFiles(backupFolder, "*.dll"))
        {
            var name = Path.GetFileNameWithoutExtension(file);
            var md5 = MD5Hashing.HashFile(file);
            var size = (new FileInfo(file)).Length;
            var dllInfo = new DllInfo()
            {
                dllName = name,
                MD5 = md5,
                size = size,
            };
            dllVersion.Manifest.Add(name, dllInfo);
            FileUtil.MoveFileOrDirectory(file, backupFolder + "/" + dllInfo.ToFileName());
        }

        dllVersion.Version = DateTime.UtcNow.ToFileTimeUtc();

        FileHelper.SaveJsonObj(dllVersion, backupFolder + "/" + dllVersion.ToFileName());
        FileHelper.SaveJsonObj(dllVersion, dataPath + "/StreamingAssets/" + GameResPath.DLLVERSION_FILE);
    }

    #endregion
    #region 压缩PC生成自解压exe
    public const string SFXConfigFolder = "BuildPC";

    public static void GenerateWinExe(BuildTarget target, string pathToBuiltProject)
    {
        if (target != BuildTarget.StandaloneWindows)
        {
            return;
        }

        var rootFolder = Path.GetDirectoryName(pathToBuiltProject);
        var fileName = Path.GetFileNameWithoutExtension(pathToBuiltProject);
        var configFilePath = string.Format("{0}/{1}_sfx.txt", SFXConfigFolder, fileName);
        var buildAsExe = File.Exists(configFilePath);
        var exePath = Path.GetDirectoryName(rootFolder) + "/" + Path.GetFileName(rootFolder) + (buildAsExe ? ".exe" : ".rar");
        FileUtil.DeleteFileOrDirectory(exePath);

        var argList = new List<string>();
        argList.Add("winrar");
        argList.Add("a");
        argList.Add("-r");
        argList.Add("-ep1");
        if (buildAsExe)
        {
            argList.Add("-sfx");
            argList.Add("-iicon" + string.Format("{0}/{1}.ico ", SFXConfigFolder, fileName));
            argList.Add("-scuc");
            argList.Add("-z" + configFilePath);
        }
        argList.Add(exePath);
        argList.Add(rootFolder + "/*.*");
        var cmd = string.Join(" ", argList.ToArray());

        var p = new Process();
        p.StartInfo.FileName = "cmd.exe";
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.RedirectStandardInput = true;
        p.StartInfo.RedirectStandardOutput = true;
        p.Start();
        p.StandardInput.WriteLine();
        p.StandardInput.WriteLine(cmd);
        p.StandardInput.WriteLine("exit");
        p.WaitForExit();
        Debug.Log(p.StandardOutput.ReadToEnd());
    }
    #endregion
}
