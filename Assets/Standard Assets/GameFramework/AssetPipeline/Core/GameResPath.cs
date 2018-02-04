using System;
using System.IO;
using UnityEngine;

namespace AssetPipeline
{
    /// <summary>
    /// GameResPath主要负责所有资源相关的路径的定义和处理
    /// </summary>
    public static class GameResPath
    {
        // 常用文件
        public const string VERSIONCONFIG_FILE = "versionConfig.json";
        public const string DLLVERSION_FILE = "dllVersion.json";
        public const string RESCONFIG_FILE = "resConfig.tz";
        public const string MINIRESCONFIG_FILE = "miniResConfig.jz";

        // 常用本地文件夹
        public const string MANAGER_BACKUP_ROOT = "BackupDll";
        public const string MANAGER_ROOT = "Managed";
        public const string BUNDLE_ROOT = "gameres";

        // 远程文件夹
        public const string DLL_VERSION_ROOT = "dllversion";
        public const string DLL_FILE_ROOT = "dll";
        public const string RESCONFIG_ROOT = "resconfig";
        public const string PATCH_INFO_ROOT = "patchinfo";
        public const string REMOTE_BUNDLE_ROOT = "remoteres";

        // 本地打包用文件夹
        public const string EXPORT_FOLDER = "_GameBundles";
        public const string PATCH_BUNDLE_ROOT = "patch_resources";

        // 特殊的AB
        public const string AllShaderBundleName = "common/allshader";
        public const string AllScriptBundleName = "common/allscript";

        /// <summary>
        /// 编辑器下返回工程根目录
        /// </summary>
        /// <returns></returns>
        public static string appRoot
        {
            get
            {
                if (Application.isEditor)
                {
                    return Path.GetDirectoryName(Application.dataPath);
                }

                return Application.dataPath;
            }
        }

        #region 包外资源路径
        /// <summary>
        /// PC平台特殊处理,为了删除游戏时可以连带更新资源一起删除
        /// </summary>
        public static string persistentDataPath
        {
            get
            {
                if (Application.platform == RuntimePlatform.WindowsPlayer)
                {
                    return Application.dataPath + "/persistentAssets";
                }

                return Application.persistentDataPath;
            }
        }

        private static string _androidDllRoot;

        /// <summary>
        /// 更新后的dll存放目录
        /// </summary>
        public static string dllRoot
        {
            get
            {
                if (Application.platform == RuntimePlatform.Android)
                {
                    if (_androidDllRoot == null)
                    {
                        // libmono.so 写死了，就不重新生成了
                        _androidDllRoot = BaoyugameSdk.GetAndroidInternalPersistencePath() + "/" + "dlls";
                    }

                    return _androidDllRoot;
                }

                return persistentDataPath + "/" + MANAGER_ROOT;
            }
        }


        public static string dllBackupRoot
        {
            get
            {
                return persistentDataPath + "/" + MANAGER_BACKUP_ROOT;
            }
        }

        /// <summary>
        /// 包外Bundle资源根目录
        /// </summary>
        public static string bundleRoot
        {
            get { return persistentDataPath + "/" + BUNDLE_ROOT; }
        }
        #endregion

        #region 包内资源路径
        /// <summary>
        /// 包内Bundle资源根目录
        /// </summary>
        public static string packageBundleRoot
        {
            get { return Application.streamingAssetsPath + "/" + BUNDLE_ROOT; }
        }
        /// <summary>
        /// 包内Bundle资源 gameres 目录URL路径,使用WWW加载时需要用到
        /// </summary>
        public static string packageBundleUrlRoot
        {
            get { return string.Concat(packageResUrlRoot, '/', BUNDLE_ROOT); }
        }
        /// <summary>
        /// 包内Bundle资源根目录URL路径,使用WWW加载时需要用到
        /// </summary>
        public static string packageResUrlRoot
        {
            get
            {
                if (Application.platform == RuntimePlatform.Android)
                {
                    return Application.streamingAssetsPath;
                }

                return GetLocalFileUrl(Application.streamingAssetsPath);
            }
        }
        #endregion

        /// <summary>
        /// 根据不同平台生成对应本地文件的Url,Window下需要使用file:///
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetLocalFileUrl(string filePath)
        {
            if (Application.platform == RuntimePlatform.WindowsEditor ||
                Application.platform == RuntimePlatform.WindowsPlayer)
            {
                return "file:///" + filePath;

            }

            return "file://" + filePath;
        }

    }
}
