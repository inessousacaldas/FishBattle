using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using AssetPipeline;
using LITJson;
using UnityEditor;

public static class GameResVersionManager
{
    public static string GameResVersionConfigPath = Application.dataPath + "/Editor/GameRes/GameResVersionConfig.txt";

    private static GameResVersionConfig _config;

    private static void ReloadConfig()
    {
        var config = JsHelper.ToObject<GameResVersionConfig>(FileHelper.ReadAllText(GameResVersionConfigPath));
        _config = config;
    }


//    [MenuItem("Test/Test %#R")]
    public static void Test()
    {
        FileHelper.WriteAllText(GameResVersionConfigPath, JsonMapper.ToJson(new GameResVersionConfig(), true));
    }

    public static string GetRemoteFullPath(string path, string winPrefix = "\\\\oa.cilugame.com")
    {
#if UNITY_EDITOR_WIN
        path = winPrefix + "\\" + path;
#elif UNITY_EDITOR_OSX
		path = "/Volumes/" + path;
#endif

        return path;
    }


    public static void UploadVersionResourcesPatch(ResConfig config)
    {
        ReloadConfig();

        CopyPatchResources(config);
        CopyResConfig(config);

        Debug.Log("UploadVersionResourcesPatch Finished");
    }


    public static void UploadMiniVersionResourcesPatch()
    {
        ReloadConfig();

        var miniVer = CopyMiniResources();
        CopyMiniPatchInfo(miniVer);

        Debug.Log("UploadMiniVersionResourcesPatch Finished");
    }

    public static void UploadDllsPatch()
    {
        ReloadConfig();

        string dllsRoot = null;
        if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneWindows)
        {
            dllsRoot = EditorUtility.SaveFolderPanel("选择dll目录",
                "Pcbao", "");
        }
        else if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
        {
            dllsRoot = EditorUtility.SaveFolderPanel("选择dll目录", "BuildAPK", "");
        }

        if (string.IsNullOrEmpty(dllsRoot))
        {
            Debug.LogError(string.Format("该路径：{0} 不存在要拷贝的dll！", dllsRoot));

            return;
        }

        CopyDlls(dllsRoot);
        CopyDllConfig(dllsRoot);

        Debug.Log("UploadDllsPatch Finished");
    }


    #region 远程目录
    //	public static string RemoteUpdatePath;
    public static string RemoteUpdateWinPrefix = "\\\\oa.cilugame.com";

    public static string RemoteUpdateFullPath
    {
        get { return GetRemoteFullPath(_config.RemoteUpdatePath, RemoteUpdateWinPrefix); }
    }


    private static Dictionary<string, string[]> RemotePlatformFolderDict
    {
        get { return _config.RemotePlatformFolderDict; }
    }


    public static string[] GetRemotePlatformFolderList()
    {
        return RemotePlatformFolderDict[EditorUserBuildSettings.activeBuildTarget.ToString()];
    }
    #endregion


    #region 本地目录
    public static string GetPlatformExportRoot()
    {
        if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
        {
            return GameResPath.EXPORT_FOLDER + "/Android";
        }
        if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS)
        {
            return GameResPath.EXPORT_FOLDER + "/IOS";
        }
        return GameResPath.EXPORT_FOLDER + "/PC";
    }
    public static string GetGameResourcesRoot()
    {
        return GetPlatformExportRoot() + "/" + GameResPath.BUNDLE_ROOT;
    }

    public static string GetPatchResourcesRoot()
    {
        return Path.GetDirectoryName(AssetBundleBuilder.GetPatchResRoot(new ResPatchInfo()));
    }

    public static string GetPatchInfoRoot()
    {
        return GetPlatformExportRoot() + "/patchinfo";
    }


    public static string GetMiniPatchInfo(int miniVer)
    {
        return string.Format("{0}/patch_minres_{1}.jz", GetPatchInfoRoot(), miniVer);
    }


    public static string GetResConfigRoot(ResConfig config)
    {
        return AssetBundleBuilder.GetResConfigRoot() + "/" + config.ToRemoteName();
    }



    public static string GetDllConfigPath(string dllsRoot)
    {
        return dllsRoot + "/version.json";
    }
    #endregion

    #region 资源
    public static string RemoteUpdateResPath
    {
        get { return RemoteUpdateFullPath + "/v" + AppGameVersion.BundleVersion + "-update"; }
    }


    public static string RemoteUpdateMiniResPath
    {
        get { return RemoteUpdateResPath + "-mini"; }
    }


    public static string GetDllsFullPath(string path)
    {
        return path + "/staticRes/dll";
    }


    public static string GetGameMiniResourcesFullPath(string path, int miniVer)
    {
        return path + "/staticRes/mini_resources_" + miniVer;
    }


    public static string GetGameResourcesFullPath(string path)
    {
        return path + "/staticRes/" + GameResPath.REMOTE_BUNDLE_ROOT;
    }


    public static string GetPatchInfoFullPath(string path)
    {
        return path + "/staticRes/patchinfo";
    }


    public static string GetMiniPatchInfoFullPath(string path, int miniVer)
    {
        return string.Format("{0}/patch_minres_{1}.jz", GetPatchInfoFullPath(path), miniVer);
    }


    public static int CopyMiniResources()
    {
        var path = EditorUtility.SaveFolderPanel("选择整包资源路径：", GetGameResourcesRoot(), "");
        if (string.IsNullOrEmpty(path) || !path.Contains("version_"))
        {
            // 版本号不对
            return -1;
        }

        var pathName = Path.GetFileNameWithoutExtension(path);
        var ver = Convert.ToInt32(pathName.Replace("version_", ""));

        foreach (var folder in GetRemotePlatformFolderList())
        {
            var remotePath = GetGameMiniResourcesFullPath(RemoteUpdateMiniResPath + "/" + folder, ver);
            FileHelper.CreateDirectory(Path.GetDirectoryName(remotePath));
            FileHelper.DeleteDirectory(remotePath, true);
            FileUtil.CopyFileOrDirectory(path, remotePath);
        }

        return ver;
    }


    public static void CopyPatchResources(ResConfig config)
    {
        var path = EditorUtility.SaveFolderPanel("选择补丁路径：", GetPatchResourcesRoot(), "");
        if (string.IsNullOrEmpty(path) || !path.Contains("patch_"))
        {
            return;
        }

        var pathName = Path.GetFileNameWithoutExtension(path);
        var isMatch = Regex.IsMatch(pathName, @"^patch_\d+_" + config.Version);
        if (!isMatch)
        {
            var goOn = EditorUtility.DisplayDialog("版本号不符合", "当前AssetBundleBuilder的版本号和上传资源版本号不符，确认无误可以继续！", "确认", "取消");
            if (!goOn)
            {
                throw new Exception("主动取消上传补丁资源！");
            }
        }

        foreach (var folder in GetRemotePlatformFolderList())
        {
            var remotePath = GetGameResourcesFullPath(RemoteUpdateResPath + "/" + folder);
            FileHelper.CreateDirectory(Path.GetDirectoryName(remotePath));
            FileHelper.DeleteDirectory(remotePath, true);
            FileUtil.CopyFileOrDirectory(path, remotePath);
        }
    }


    public static void CopyMiniPatchInfo(int miniVer)
    {
        foreach (var folder in GetRemotePlatformFolderList())
        {
            var remotePath = GetMiniPatchInfoFullPath(RemoteUpdateMiniResPath + "/" + folder, miniVer);
            FileHelper.DeleteDirectory(Path.GetDirectoryName(remotePath), true);
            FileHelper.CreateDirectory(Path.GetDirectoryName(remotePath));
            File.Copy(GetMiniPatchInfo(miniVer), remotePath);
        }
    }


    public static void CopyDlls(string dllsRoot)
    {
        foreach (var folder in GetRemotePlatformFolderList())
        {
            var remotePath = GetDllsFullPath(RemoteUpdateResPath + "/" + folder);
            FileHelper.CreateDirectory(Path.GetDirectoryName(remotePath));
            FileHelper.DeleteDirectory(remotePath, true);
            FileUtil.CopyFileOrDirectory(dllsRoot, remotePath);

            foreach (var file in Directory.GetFiles(remotePath, "*.*"))
            {
                // 删除多余的文件
                if (Path.GetExtension(file) != ".dll")
                {
                    FileHelper.DeleteFile(file);
                }
            }
        }
    }

    #endregion

    #region 版本
    public static string RemoteUpdateVerPath
    {
        get { return RemoteUpdateFullPath + "/v" + AppGameVersion.BundleVersion + "-version"; }
    }


    public static string GetDllsConfigFullPath(string path)
    {
        return path + "/staticRes/dllversion";
    }


    public static string GetStaticResConfigFullPath(string path, ResConfig reconfig)
    {
        return path + "/staticRes/" + GameResPath.RESCONFIG_ROOT + "/" + reconfig.ToRemoteName();
    }


    public static void CopyResConfig(ResConfig config)
    {
        foreach (var folder in GetRemotePlatformFolderList())
        {
            var remotePath = GetStaticResConfigFullPath(RemoteUpdateVerPath + "/" + folder, config);
            FileHelper.CreateDirectory(Path.GetDirectoryName(remotePath));
            FileUtil.DeleteFileOrDirectory(remotePath);

            var jzPath = GetResConfigRoot(config);
            FileUtil.CopyFileOrDirectory(jzPath, remotePath);
        }
    }


    public static void CopyDllConfig(string dllsRoot)
    {
        foreach (var folder in GetRemotePlatformFolderList())
        {
            var remotePath = GetDllsConfigFullPath(RemoteUpdateVerPath + "/" + folder);
            FileHelper.DeleteDirectory(remotePath, true);
            FileHelper.CreateDirectory(remotePath);

            foreach (var file in Directory.GetFiles(dllsRoot, "*.json"))
            {
                FileUtil.CopyFileOrDirectory(file, remotePath + "/" + Path.GetFileName(file));
            }
        }
    }
    #endregion
}


public class GameResVersionConfig
{
    public string RemoteUpdatePath = "赐麓/H1/版本更新";


    public Dictionary<string, string[]> RemotePlatformFolderDict = new Dictionary<string, string[]>()
    {
//        {BuildTarget.Android.ToString(),
//            new []
//            {
//                GameSetting.DomainType.Release.ToString().ToLower() + "/" + GameSetting.PlatformType.Android.ToString().ToLower(),
//            }
//        },
//        {BuildTarget.iOS.ToString(),
//            new []
//            {
//                GameSetting.DomainType.Release.ToString().ToLower() + "/" + GameSetting.PlatformType.IOS.ToString().ToLower(),
//            }
//        },
//        {BuildTarget.StandaloneWindows.ToString(),
//            new []
//            {
//                GameSetting.DomainType.Release.ToString().ToLower() + "/" + GameSetting.PlatformType.Win.ToString().ToLower(),
//            }
//        },
    };
}