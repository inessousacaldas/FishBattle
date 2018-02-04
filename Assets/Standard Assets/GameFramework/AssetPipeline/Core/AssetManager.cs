//#define GAMERES_LOG
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using LITJson;
using Priority_Queue;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;
using System.Text.RegularExpressions;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AssetPipeline
{
    public class AssetManager : MonoBehaviour
    {
        private static AssetManager _instance;
        private static bool _isQuit = false;

        public static AssetManager Instance
        {
            get
            {
                if (_instance == null && !_isQuit)
                {
                    var type = typeof(AssetManager);
                    var objects = FindObjectsOfType<AssetManager>();

                    if (objects.Length > 0)
                    {
                        _instance = objects[0];
                        if (objects.Length > 1)
                        {
                            Debug.LogWarning("There is more than one instance of Singleton of type \"" + type + "\". Keeping the first. Destroying the others.");
                            for (var i = 1; i < objects.Length; i++)
                                DestroyImmediate(objects[i].gameObject);
                        }
                        return _instance;
                    }

                    GameObject go = new GameObject(type.Name);
                    _instance = go.AddComponent<AssetManager>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        private static Dictionary<int, string> resGrounpMap;

        static AssetManager()
        {
            resGrounpMap = new Dictionary<int, string>();
            ResGroup[] array = (ResGroup[])Enum.GetValues(typeof(ResGroup));
            for (int i = 0; i < array.Length; i++)
            {
                ResGroup resGroup = array[i];
                resGrounpMap.Add((int)resGroup, resGroup.ToString().ToLower() + "/");
            }
        }

        private const int MAX_RETRYCOUNT = 3;
        private const int MAX_DOWNLOADCOUNT = 5;

        public enum LoadMode
        {
            EditorLocal,
            Mobile
        }

#if UNITY_EDITOR
        public static float EditorLoadDelay = 0f;
        //编辑器下模拟资源加载延迟
        public static LoadMode ResLoadMode = LoadMode.EditorLocal;
#if !USE_JSZ
        public static bool _assetErrorPause = false;
        public static string[] assetErrorPauseStrings;
#endif
#else
        public static LoadMode ResLoadMode = LoadMode.Mobile;
#endif

        private static bool _cleanUpResFlag;

        private Action<string> _logMessageHandler;
        private Action<string> _loadErrorHandler;

        private List<string> _cdnUrls;
        private string _cdnUrlRoot;
        private VersionConfig _localVersionConfig;
        private VersionConfig _versionConfig;
        private DllVersion _dllVersion;
        private ResConfig _packageResConfig;
        private ResConfig _curResConfig;

        //非WIFI情况下,询问玩家是否更新标记
        //当为true时,代表玩家已确认在非WIFI情况下更新数据了
        private bool _requestUpdateFlag;

        public VersionConfig CurVersionConfig
        {
            get { return _versionConfig; }
        }

        public ResConfig CurResConfig
        {
            get { return _curResConfig; }
        }

        private MiniResConfig _miniResConfig;

        public MiniResConfig MiniResConfig
        {
            get { return _miniResConfig; }
        }

        public AtlasManager _atlasManager { get; private set; }
        private UnloadBundleManager _unloadBundleManager;
        void Awake()
        {
            _atlasManager = new AtlasManager(this);
            _unloadBundleManager = new UnloadBundleManager(this);

        }

        void OnApplicationQuit()
        {
            _isQuit = true;
            Debug.Log("AssetManager OnApplicationQuit");
            //如果玩家直接终止游戏进程是不会触发OnApplicationQuit,
            //所以在资源更新时,玩家杀掉游戏进程,会导致玩家需要重新下载所有更新数据
            Dispose();
        }
        private void OnApplicationPause(bool paused)
        {
            //退出到后台时，判断在热更则保存ResConfig
            if (paused && isDownloadAsset)
            {
                SaveResConfig();
            }
        }
        #region 资源管理初始化流程

        public void Setup(List<string> cdnUrls, Action<string> logHandler, Action<bool> onFinish, Action<string> onError)
        {
#if UNITY_EDITOR
            //编辑器模式下,读取PlayerPrefs中记录的加载模式
            ResLoadMode = (LoadMode)EditorPrefs.GetInt("ResLoadMode", 0);
#if !USE_JSZ
            _assetErrorPause = EditorPrefs.GetBool("AssetErrorPause", false);
            assetErrorPauseStrings = EditorPrefs.GetString("AssetErrorPauseString", string.Empty).Split(';');
            assetErrorPauseStrings = assetErrorPauseStrings.Where(item => !string.IsNullOrEmpty(item)).ToArray();
#endif

#endif
            _cdnUrls = cdnUrls;
            _cdnUrlRoot = cdnUrls[0];
            _logMessageHandler = logHandler;
            _loadErrorHandler = onError;
            _fatalError = false;
            _requestUpdateFlag = false;

            if (_curResConfig != null)
                return;

            if (ResLoadMode == LoadMode.EditorLocal)
            {
                if (onFinish != null)
                    onFinish(false);
            }
            else
            {
                StartCoroutine(CheckOutGameRes(onFinish));
            }
        }

        /// <summary>
        /// 检查包外资源,如果包外缺失resConfig,dllVersion,miniResConfig将从包内拷贝至包外
        /// </summary>
        /// <param name="onFinish"></param>
        /// <returns></returns>
        private IEnumerator CheckOutGameRes(Action<bool> onFinish)
        {
            PrintInfo("验证游戏资源完整性...");
            string packageResUrlRoot = GameResPath.packageResUrlRoot;
            string url = null;
            var dllHasChanged = false;

            if (IsSupportUpdateDllPlatform())
            {
                DllVersion packageDllVersion = null;
                url = Path.Combine(packageResUrlRoot, GameResPath.DLLVERSION_FILE);
                using (var www = new WWW(url))
                {
                    yield return www;

                    if (!string.IsNullOrEmpty(www.error))
                    {
                        ThrowFatalException(string.Format("Load url:{0}\nerror:{1}", url, www.error));
                    }

                    packageDllVersion = JsonMapper.ToObject<DllVersion>(www.text);
                }

                var dllVersionPath = Path.Combine(GameResPath.dllRoot, GameResPath.DLLVERSION_FILE);
                if (File.Exists(dllVersionPath))
                {
                    _dllVersion = FileHelper.ReadJsonFile<DllVersion>(dllVersionPath);

                    if (_dllVersion.serverType == packageDllVersion.serverType && _dllVersion.Version < packageDllVersion.Version)
                    {
                        CleanUpDllFolder();
                        Debug.Log("包内的dll比较新，需要清空包外dll");

                        if (onFinish != null)
                        {
                            onFinish(true);
                        }
                        yield break;
                    }

                    dllHasChanged = _dllVersion.Version != packageDllVersion.Version;
                }
                else
                {
                    // 没陪dllversion，但是有dll的低概率事件，也做下清除以防万一
                    CleanUpDllFolder();

                    _dllVersion = packageDllVersion;
                    SaveDllVersion();
                    Debug.Log("拷贝包内dllVersion到包外");
                }
            }

            url = Path.Combine(packageResUrlRoot, GameResPath.RESCONFIG_FILE);
            using (var www = new WWW(url))
            {
                yield return www;

                if (!string.IsNullOrEmpty(www.error))
                {
                    ThrowFatalException(string.Format("Load url:{0}\nerror:{1}", url, www.error));
                }

                _packageResConfig = ResConfig.ReadFile(www.bytes, true);
                if (_packageResConfig == null)
                {
                    ThrowFatalException("包内资源配置信息丢失");
                    yield break;
                }
            }
            //尝试加载包外ResConfig
            string configPath = Path.Combine(GameResPath.persistentDataPath, GameResPath.RESCONFIG_FILE);
            if (FileHelper.IsExist(configPath))
            {
                _curResConfig = ResConfig.ReadFile(FileHelper.ReadAllBytes(configPath), true);
                if (_curResConfig == null)
                {
                    Debug.LogError("包外resConfig已损坏,将会重新从包内拷贝出去");
                }
            }
            if (IsNewerPackageRes())
            {
                CleanUpBundleResFolder();
                _curResConfig = _packageResConfig;
                SaveResConfig();
                Debug.Log("包内资源更新,清空包外资源,重新拷贝包内resConfig到包外!!!!");

                if (dllHasChanged)
                {
                    CleanUpDllFolder();
                    Debug.Log("包内资源更新，dll对应不上，需要清空重启");

                    if (onFinish != null)
                    {
                        onFinish(true);
                    }
                    yield break;
                }
            }

            //如果当前为小包资源,拷贝包内miniResConfig到包外
            //当小包升级为整包时,isMiniRes标志将置为false
            if (_curResConfig.isMiniRes)
            {
                string miniResConfigPath = Path.Combine(GameResPath.persistentDataPath, GameResPath.MINIRESCONFIG_FILE);
                if (FileHelper.IsExist(miniResConfigPath))
                {
                    _miniResConfig = FileHelper.ReadJsonFile<MiniResConfig>(miniResConfigPath, true);
                }
                else
                {
                    url = Path.Combine(packageResUrlRoot, GameResPath.MINIRESCONFIG_FILE);
                    using (var www = new WWW(url))
                    {
                        yield return www;

                        if (!string.IsNullOrEmpty(www.error))
                        {
                            ThrowFatalException(string.Format("Load url:{0}\nerror:{1}", url, www.error));
                        }

                        _miniResConfig = FileHelper.ReadJsonBytes<MiniResConfig>(www.bytes);
                        SaveMiniResConfig();
                        Debug.Log("拷贝包内miniResConfig到包外");
                    }
                }
            }

            if (onFinish != null)
                onFinish(false);
        }


        //满足以下其中一个条件,则认为玩家是首次安装游戏,需要清空包外资源目录,需要从包内拷贝resConfig到包外
        //1.首次安装游戏或者用户手动删除了包外的resConfig
        //2.包外版本号低于包内版本号,最为特殊的情况用户没有进入游戏更新的情况下,用新安装包覆盖了旧包,使得包内资源更新
        // 用buildtime来做包的更改的记录
        private bool IsNewerPackageRes()
        {
            return _curResConfig == null ||
            (_packageResConfig.BuildTime != _curResConfig.BuildTime &&
            (_curResConfig.lzmaCRC != _packageResConfig.lzmaCRC ||
            _curResConfig.lz4CRC != _packageResConfig.lz4CRC ||
            _curResConfig.tileTexCRC != _packageResConfig.tileTexCRC));
        }

        #endregion

        #region 获取VersionConfig信息

        /// <summary>
        /// 根据版本信息类型获取指定的VersionConfig信息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="onFinish"></param>
        public void FetchVersionConfig(VersionConfig.ServerType serverType, Action onFinish)
        {
            //编辑器本地模式下无需更新资源
            if (ResLoadMode == LoadMode.EditorLocal)
            {
                if (onFinish != null)
                    onFinish();
            }
            else
            {
                StartCoroutine(DownloadVersionConfig(serverType, onFinish));
            }
        }

        private IEnumerator DownloadVersionConfig(VersionConfig.ServerType serverType, Action onFinish)
        {
            PrintInfo("获取游戏版本信息...");

            if (_localVersionConfig == null)
            {
                _localVersionConfig = ReadLocalVersionConfig();
                if (_localVersionConfig != null)
                {
                    Debug.Log("Local_VersionConfig:\n" + _localVersionConfig.ToString());
                }
                else
                {
                    Debug.Log("Local_VersionConfig is null");
                }
            }

            string versionConfigFileName = null;
            if (serverType == VersionConfig.ServerType.Null)
            {
                versionConfigFileName = _localVersionConfig == null
                    ? VersionConfig.GetFileName(VersionConfig.ServerType.Default)
                    : _localVersionConfig.ToFileName();
            }
            else
            {
                versionConfigFileName = VersionConfig.GetFileName(serverType);
            }

            string versionConfigUrl = string.Format("{0}?ver={1}", _cdnUrlRoot + "/" + versionConfigFileName, DateTime.Now.Ticks);
            Debug.Log(string.Format("DownloadVersionConfig url={0}", versionConfigUrl));

            yield return StartCoroutine(DownloadByUnityWebRequest(versionConfigUrl, www =>
            {
                _versionConfig = JsonMapper.ToObject<VersionConfig>(www.downloadHandler.text);
                if (_versionConfig == null)
                {
                    ThrowFatalException("VersionConfig is null");
                }
                else
                {
                    Debug.Log("CDN_VersionConfig:\n" + _versionConfig.ToString());
                }

                // 如果服务器版本不一样，记录下来，这样子下次就不会取到不一样的服务器版本
                if (IsChangeVersionConfigType())
                {
                    SaveLocalVersionConfig();
                }

                if (onFinish != null)
                    onFinish();
            },
                msg =>
                {
                    int index = _cdnUrls.IndexOf(_cdnUrlRoot);
                    if (index + 1 >= _cdnUrls.Count)
                    {
                        var log = string.Format("游戏资源加载失败，请重试\n{0}", msg);
                        if (_loadErrorHandler != null)
                        {
                            _loadErrorHandler(log);
                        }
                        else
                        {
                            PrintInfo(log);
                        }
                        throw new Exception(msg);
                    }
                    else
                    {
                        CdnReportHelper.Report(versionConfigUrl);
                        _cdnUrlRoot = _cdnUrls[index + 1];
                        StartCoroutine(DownloadVersionConfig(serverType, onFinish));
                    }
                }));
        }

        /// <summary>
        /// 判断玩家是否变更过游戏服类型
        /// 注:如果变更过游戏服类型,后面只能用 "==" 来判断,而不能用 ">=" ,因为根据游戏服类型拿到不同的VersionConfig有可能出现回退版本的需要
        /// 例如从公测服资源版本号为10升级到Beta服资源版本号为20,然后玩家又切回公测服,这时候就要从20回退到10
        /// </summary>
        /// <returns></returns>
        private bool IsChangeVersionConfigType()
        {
            return _localVersionConfig == null || (_localVersionConfig.serverType != _versionConfig.serverType);
        }

        #endregion

        #region Dll更新流程

        /// <summary>
        /// 返回true代表不需要更新Dll
        /// </summary>
        /// <param name="onFinish">Dll更新完毕回调,一般用于提示用户重启游戏</param>
        /// <returns></returns>
        public bool ValidateDllVersion(Action onFinish)
        {
            if (!IsSupportUpdateDllPlatform())
                return true;

            if (IsChangeDllVersionType())
            {
                //变更了游戏服类型后,可能需要回退版本,所以只能已"=="来判断
                if (_dllVersion.Version == _versionConfig.dllVersion)
                {
                    SaveDllVersion(true);
                    return true;
                }
            }
            else
            {
                //一般更新流程下,本地版本号大于等于VersionConfig的值都直接跳过
                if (_dllVersion.Version >= _versionConfig.dllVersion)
                    return true;
            }

            PrintInfo("获取Dll版本信息...");
            string dllUrl = _cdnUrlRoot + "/" + GameResPath.DLL_VERSION_ROOT + "/" + DllVersion.GetFileName(_versionConfig.dllVersion);
            Debug.Log("DownloadDllVersion url=" + dllUrl);

            StartCoroutine(DownloadByUnityWebRequest(dllUrl, www =>
            {
                var newDllVersion = JsonMapper.ToObject<DllVersion>(www.downloadHandler.text);
                if (newDllVersion == null)
                {
                    ThrowFatalException("DllVersion is null");
                    return;
                }

                //构建dll更新下载队列
                long totalFileSize = 0L;
                var downloadQueue = new Queue<DllInfo>(newDllVersion.Manifest.Count);
                foreach (var pair in newDllVersion.Manifest)
                {
                    DllInfo newDllInfo = pair.Value;
                    DllInfo oldDllInfo;
                    if (_dllVersion.Manifest.TryGetValue(pair.Key, out oldDllInfo))
                    {
                        if (oldDllInfo.MD5 != newDllInfo.MD5 && !File.Exists(GetBackupDllPath(newDllInfo)))
                        {
                            totalFileSize += newDllInfo.size;
                            downloadQueue.Enqueue(newDllInfo);
                        }
                    }
                    else
                    {
                        Debug.LogError("Dll更新中不可能出现新增dll,请检查: " + pair.Key);
                    }
                }

                string netType = BaoyugameSdk.getNetworkType();
                if (netType == BaoyugameSdk.NET_STATE_WIFI)
                {
                    StartCoroutine(UpdateDllFile(newDllVersion, downloadQueue, totalFileSize, onFinish));
                }
                else
                {
                    //非WIFI情况下询问玩家是否更新,如果是强制更新,用户又选择不更新的话将会直接退出游戏
                    if (_requestUpdateFlag)
                    {
                        StartCoroutine(UpdateDllFile(newDllVersion, downloadQueue, totalFileSize, onFinish));
                    }
                    else
                    {
                        BuiltInDialogueViewController.OpenView(
                            string.Format("当前使用非WIFI网络， 有{0}的代码需要更新， 是否进行更新", FormatBytes(totalFileSize)),
                            () =>
                            {
                                _requestUpdateFlag = true;
                                StartCoroutine(UpdateDllFile(newDllVersion, downloadQueue, totalFileSize, onFinish));
                            }, () =>
                            {
                                if (_versionConfig.forceUpdate)
                                {
                                    if (_loadErrorHandler != null)
                                    {
                                        _loadErrorHandler(null);
                                    }
                                }
                                else
                                {
                                    if (onFinish != null)
                                        onFinish();
                                }
                            }, UIWidget.Pivot.Left, "更新", _versionConfig.forceUpdate ? "退出" : "取消");
                    }
                }
            }, ThrowFatalException));
            return false;
        }

        /// <summary>
        /// 根据最新DllVersion信息更新dll
        /// </summary>
        /// <param name="newDllVersion"></param>
        /// <param name="downloadQueue"></param>
        /// <param name="totalFileSize"></param>
        /// <param name="onFinish"></param>
        /// <returns></returns>
        private IEnumerator UpdateDllFile(DllVersion newDllVersion, Queue<DllInfo> downloadQueue, long totalFileSize, Action onFinish)
        {
            if (ValidateStorageSpace(totalFileSize))
            {
                long remainingSize = totalFileSize;
                int totalCount = downloadQueue.Count;
                PrintInfo(string.Format("下载Dll中，剩余{0}({1}/{2})...", FormatBytes(remainingSize), 0, totalCount));
                while (downloadQueue.Count > 0)
                {
                    var newDllInfo = downloadQueue.Dequeue();
                    string dllUrl = _cdnUrlRoot + "/" + GameResPath.DLL_FILE_ROOT + "/" + newDllInfo.ToFileName();
                    yield return StartCoroutine(DownloadByUnityWebRequest(dllUrl, www =>
                    {
                        // 备份的依旧使用md5命名，方便校验文件是否已经下载
                        var dllPath = GetBackupDllPath(newDllInfo);
                        try
                        {
                            FileHelper.WriteAllBytes(dllPath, www.downloadHandler.data);
                            remainingSize -= newDllInfo.size;
                            Debug.Log(string.Format("UpdateDll:{0}\nFinish", dllPath));
                        }
                        catch (Exception e)
                        {
                            ThrowFatalException("Save Dll Error: " + dllPath + "\n" + e.Message);
                        }
                    }, ThrowFatalException));

                    yield return null;
                    PrintInfo(string.Format("下载Dll中，剩余{0}({1}/{2})...", FormatBytes(remainingSize), totalCount - downloadQueue.Count, totalCount));
                }

                if (!_fatalError)
                {
                    try
                    {
                        UseNewDllFile(newDllVersion);
                    }
                    catch (Exception e)
                    {
                        ThrowFatalException(e.Message);
                    }
                }

                ClearBackupDllFile();

                //下载完毕,保存最新DllVersion到本地
                PrintInfo("Dll更新完毕,请重启游戏");

                if (onFinish != null)
                    onFinish();
            }
        }


        private void UseNewDllFile(DllVersion newDllVersion)
        {
            // 这里重新计算一遍，防止多拷贝文件
            foreach (var pair in newDllVersion.Manifest)
            {
                var newDllInfo = pair.Value;
                DllInfo oldDllInfo;
                if (_dllVersion.Manifest.TryGetValue(pair.Key, out oldDllInfo))
                {
                    if (oldDllInfo.MD5 != newDllInfo.MD5)
                    {
                        // dll服务器格式是先压缩，再加密
                        // 这里拿到dll得先解密，再解压缩，再加密
                        // 非项目dll不做加密处理
                        var fileBytes = FileHelper.ReadAllBytes(GetBackupDllPath(newDllInfo));
                        var zipBytes = DllHelper.IsProjectDll(newDllInfo.dllName) ? DllHelper.DecryptDll(fileBytes) : fileBytes;
                        var realBytes = ZipLibUtils.Uncompress(zipBytes);
                        var encryptBytes = DllHelper.IsProjectDll(newDllInfo.dllName) ? DllHelper.EncryptDll(realBytes) : realBytes;

                        var dllPath = GameResPath.dllRoot + "/" + newDllInfo.dllName + ".dll";
                        Debug.Log(dllPath);

                        FileHelper.WriteAllBytes(dllPath, encryptBytes);
                    }
                }
                else
                {
                    Debug.LogError("Dll更新中不可能出现新增dll,请检查: " + pair.Key);
                }
            }

            _dllVersion = newDllVersion;
            SaveDllVersion(true);
        }


        private string GetBackupDllPath(DllInfo info)
        {
            return GameResPath.dllBackupRoot + "/" + info.ToFileName();
        }

        /// <summary>
        /// 当保证已经更新完毕，做清空处理
        /// </summary>
        private void ClearBackupDllFile()
        {
            FileHelper.DeleteDirectory(GameResPath.dllBackupRoot, true);
        }


        private bool IsChangeDllVersionType()
        {
            return _dllVersion != null && _dllVersion.serverType != _versionConfig.serverType;
        }

        #endregion

        #region GameRes更新流程

        public event Action OnBeginResUpdate;
        public event Action OnFinishResUpdate;

        /// <summary>
        /// 返回true代表不需要更新游戏资源
        /// </summary>
        /// <param name="onFinish"></param>
        /// <returns></returns>
        public bool ValidateGameResVersion(Action onFinish)
        {
            //编辑器本地模式下无需更新资源
            if (ResLoadMode == LoadMode.EditorLocal)
            {
                return true;
            }

            if (IsChangeResConfigType())
            {
                //变更了游戏服类型后,可能需要回退版本,所以只能已"=="来判断
                if (_curResConfig.Version == _versionConfig.resVersion)
                {
                    // 不同服务器，保存一下服务器
                    SaveResConfig(true);
                    return true;
                }
            }
            else
            {
                //一般更新流程下,本地版本号大于等于VersionConfig的值都直接跳过
                if (_curResConfig.Version >= _versionConfig.resVersion)
                    return true;
            }

            //StartCoroutine(FetchResPatchInfo(onFinish));
            StartCoroutine(FetchLastestResConfig(onFinish));
            return false;
        }

        private IEnumerator FetchLastestResConfig(Action onFinish)
        {
            PrintInfo("获取最新游戏资源版本信息...");
            string resConfigUrl = _cdnUrlRoot + "/" + GameResPath.RESCONFIG_ROOT + "/" + ResConfig.GetRemoteFile(_versionConfig.resVersion);
            yield return StartCoroutine(DownloadByUnityWebRequest(resConfigUrl, www =>
            {
                var newResConfig = ResConfig.ReadFile(www.downloadHandler.data, true);
                if (newResConfig == null)
                {
                    ThrowFatalException("Fetch resConfig is null");
                    return;
                }

                //WIFI情况下不做任何提示，直接更新资源
                //手机移动网络下，提示资源更新大小，若强制更新，跳过直接退出游戏
                //小包未完全升级为整包时,必须走游戏更新流程,因为需要把新PatchInfo的信息覆盖到curResConfig,否则玩家进入游戏后下载的就是旧的资源了
                string netType = BaoyugameSdk.getNetworkType();
                AutoUpgrade = netType == BaoyugameSdk.NET_STATE_WIFI ? AutoUpgradeType.WIFI : AutoUpgradeType.NONE;
                var patchInfo = GeneratePatchInfo(_curResConfig, newResConfig);
                if (patchInfo != null)
                {
                    if (netType == BaoyugameSdk.NET_STATE_WIFI || _curResConfig.isMiniRes)
                    {
                        StartDownloadRes(patchInfo, onFinish);
                    }
                    else
                    {
                        //非WIFI情况下询问玩家是否更新,如果是强制更新,用户又选择不更新的话将会直接退出游戏
                        if (_requestUpdateFlag)
                        {
                            StartDownloadRes(patchInfo, onFinish);
                        }
                        else
                        {
                            BuiltInDialogueViewController.OpenView(
                                string.Format("当前使用非WIFI网络， 有{0}的资源需要更新， 是否进行更新", FormatBytes(patchInfo.TotalFileSize)),
                                () =>
                                {
                                    _requestUpdateFlag = true;
                                    StartDownloadRes(patchInfo, onFinish);
                                }, () =>
                                {
                                    if (_versionConfig.forceUpdate)
                                    {
                                        if (_loadErrorHandler != null)
                                        {
                                            _loadErrorHandler(null);
                                        }
                                    }
                                    else
                                    {
                                        if (onFinish != null)
                                            onFinish();
                                    }
                                }, UIWidget.Pivot.Left, "更新", _versionConfig.forceUpdate ? "退出" : "取消");
                        }
                    }
                }
                else
                {
                    PrintInfo("PatchInfo为空,没有需要更新的资源");
                    if (onFinish != null)
                        onFinish();
                }
            }, ThrowFatalException));
        }

        /// <summary>
        /// 游戏运行时生成两个版本间的版本更新信息
        /// </summary>
        /// <param name="oldResConfig"></param>
        /// <param name="newResConfig"></param>
        /// <returns></returns>
        private ResPatchInfo GeneratePatchInfo(ResConfig oldResConfig, ResConfig newResConfig)
        {
            if (oldResConfig == null || newResConfig == null)
            {
                return null;
            }

            //无需生成当前版本PatchInfo
            if (oldResConfig.Version == newResConfig.Version)
            {
                return null;
            }

            ResPatchInfo patchInfo = new ResPatchInfo
            {
                CurVer = oldResConfig.Version,
                CurLz4CRC = oldResConfig.lz4CRC,
                CurLzmaCRC = oldResConfig.lzmaCRC,
                CurTexCRC = oldResConfig.tileTexCRC,
                EndVer = newResConfig.Version,
                EndLz4CRC = newResConfig.lz4CRC,
                EndLzmaCRC = newResConfig.lzmaCRC,
                EndTexCRC = newResConfig.tileTexCRC,
            };

            //生成更新列表
            //CRC不为0，且CRC值发生变更的，加入更新列表
            //oldResConfig不存在的，直接加入更新列表
            foreach (var newRes in newResConfig.Manifest)
            {
                if (oldResConfig.Manifest.ContainsKey(newRes.Key))
                {
                    if (oldResConfig.Manifest[newRes.Key].CRC != newRes.Value.CRC)
                    {
                        patchInfo.updateList.Add(newRes.Value);
                        patchInfo.TotalFileSize += newRes.Value.size;
                    }
                }
                else
                {
                    patchInfo.updateList.Add(newRes.Value);
                    patchInfo.TotalFileSize += newRes.Value.size;
                }
            }

            //生成删除列表
            //oldResConfig的key在newResConfig中找不到对应key的，证明该资源已被删除
            foreach (var oldRes in oldResConfig.Manifest)
            {
                if (!newResConfig.Manifest.ContainsKey(oldRes.Key))
                {
                    patchInfo.removeList.Add(oldRes.Key);
                }
            }

            return patchInfo;
        }

        private IEnumerator FetchResPatchInfo(Action onFinish)
        {
            PrintInfo("获取游戏更新资源信息...");
            string patchInfoUrl = _cdnUrlRoot + "/" + GameResPath.PATCH_INFO_ROOT + "/" + ResPatchInfo.GetFileName(_curResConfig.Version, _versionConfig.resVersion);
            yield return StartCoroutine(DownloadByUnityWebRequest(patchInfoUrl, www =>
            {
                var resPatchInfo = JsonMapper.ToObject<ResPatchInfo>(www.downloadHandler.text);
                if (resPatchInfo == null)
                {
                    ThrowFatalException("ResPatchInfo is null");
                    return;
                }

                //WIFI情况下不做任何提示，直接更新资源
                //手机移动网络下，提示资源更新大小，若强制更新，跳过直接退出游戏
                //小包未完全升级为整包时,必须走游戏更新流程,因为需要把新PatchInfo的信息覆盖到curResConfig,否则玩家进入游戏后下载的就是旧的资源了
                string netType = BaoyugameSdk.getNetworkType();
                AutoUpgrade = netType == BaoyugameSdk.NET_STATE_WIFI ? AutoUpgradeType.WIFI : AutoUpgradeType.NONE;
                if (netType == BaoyugameSdk.NET_STATE_WIFI || _curResConfig.isMiniRes)
                {
                    StartDownloadRes(resPatchInfo, onFinish);
                }
                else
                {
                    BuiltInDialogueViewController.OpenView(
                        string.Format("当前使用非WIFI网络， 有{0}的资源需要更新， 是否进行更新", FormatBytes(resPatchInfo.TotalFileSize)),
                        () =>
                        {
                            StartDownloadRes(resPatchInfo, onFinish);
                        }, () =>
                        {
                            if (_versionConfig.forceUpdate)
                            {
                                if (_loadErrorHandler != null)
                                {
                                    _loadErrorHandler(null);
                                }
                            }
                            else
                            {
                                if (onFinish != null)
                                    onFinish();
                            }
                        }, UIWidget.Pivot.Left, "更新", _versionConfig.forceUpdate ? "退出" : "取消");
                }
            }, ThrowFatalException));
        }

        private void StartDownloadRes(ResPatchInfo resPatchInfo, Action onFinish)
        {
            //如果获取的patchInfo版本号与本地resConfig的版本号不一致，直接忽略
            if (resPatchInfo.CurVer == _curResConfig.Version)
            {
                //对比版本号信息，patch最终版本号与本地资源版本号不一致，需要下载更新资源包
                if (resPatchInfo.EndVer != _curResConfig.Version)
                {
                    RemoveOutdatedAsset(resPatchInfo);

                    StartCoroutine(DownloadAssetBatch(resPatchInfo, onFinish));
                }
                else
                {
                    if (onFinish != null)
                        onFinish();
                }
            }
            else
            {
                Debug.LogError("取得的patchInfo与本地resConfig版本号不一致，可能生成patchInfo流程有问题，请检查");
                if (onFinish != null)
                    onFinish();
            }
        }

        private void RemoveOutdatedAsset(ResPatchInfo resPatchInfo)
        {
            if (resPatchInfo.removeList.Count == 0)
                return;

            string resFileRoot = GameResPath.bundleRoot;
            //根据patchManifest信息删除旧版本冗余资源
            foreach (string bundleName in resPatchInfo.removeList)
            {
                ResInfo resInfo;
                if (_curResConfig.Manifest.TryGetValue(bundleName, out resInfo))
                {
                    string abFile = resInfo.GetABPath(resFileRoot);
                    if (FileHelper.IsExist(abFile))
                    {
                        File.Delete(abFile);
#if GAMERES_LOG
                        Debug.LogError(string.Format("AB Delete:{0}", abFile));
#endif
                    }
                    _curResConfig.Manifest.Remove(bundleName);
                }
            }
        }

        private int _finishedCount;
        private int _downloadingCount;
        private long _remainingSize;
        private bool isDownloadAsset = false;

        private IEnumerator DownloadAssetBatch(ResPatchInfo resPatchInfo, Action onFinish)
        {
            PrintInfo("准备游戏资源下载中...");

            //验证手机剩余空间大小
            if (ValidateStorageSpace(resPatchInfo.TotalFileSize))
            {
                isDownloadAsset = true;
                //TalkingDataHelper.OnEventSetp("GameUpdate", "Begin");
                SPSDK.gameEvent("10005");       //开始更新
                if (OnBeginResUpdate != null)
                    OnBeginResUpdate();

                var stopwatch = new Stopwatch();
                stopwatch.Start();

                string remoteRoot = GetCDNBundleRoot();
                string bundleRoot = GameResPath.bundleRoot;
                _finishedCount = 0;
                _downloadingCount = 0;
                _remainingSize = resPatchInfo.TotalFileSize;
                var downloadQueue = new Queue<ResInfo>(resPatchInfo.updateList.Count);

                int alreadyUpdateCount = 0;
                //构建资源下载队列，已下载过的资源直接忽略
                foreach (var newResInfo in resPatchInfo.updateList)
                {
                    string newResKey = newResInfo.bundleName;
                    ResInfo oldResInfo;
                    bool needDownload = true;
                    if (_curResConfig.Manifest.TryGetValue(newResKey, out oldResInfo))
                    {
                        //版本号一致，CRC一致，且存在该文件已经更新过了无需重复下载
                        //这种情况只存在于玩家在资源更新的过程中退出游戏了,重新进入游戏后要忽略已更新过的资源
                        if (oldResInfo.CRC == newResInfo.CRC)
                        {
                            var packageResInfo = _packageResConfig.GetResInfo(newResKey);
                            if (packageResInfo != null &&
                                                     packageResInfo.CRC == newResInfo.CRC)
                            {
                                //包内已有该资源了,就无需更新了
                                //这种情况很特殊,只有玩家不删除旧包的情况下安装了最新包才会这样
                                needDownload = false;
                                ++alreadyUpdateCount;
                                _remainingSize -= newResInfo.size;
                            }
                            else
                            {
                                //包内ResConfig没有该资源,判断包外目录是否存在该文件,有就跳过不下载
                                string filePath = newResInfo.GetABPath(bundleRoot);
                                if (FileHelper.IsExist(filePath))
                                {
                                    needDownload = false;
                                    ++alreadyUpdateCount;
                                    _remainingSize -= newResInfo.size;
                                }
                            }
                        }
                    }

                    //小包模式下跳过miniResConfig记录的资源ID,因为这部分资源可以游戏时动态下载
                    if (_curResConfig.isMiniRes && _miniResConfig.replaceResConfig.ContainsKey(newResKey))
                    {
                        needDownload = false;
                        _remainingSize -= newResInfo.size;
                        //小包未下载的资源,直接用最新版本的ResInfo覆盖,因为游戏运行时要根据这个ResInfo信息动态下载该资源
                        _curResConfig.Manifest[newResKey] = newResInfo;
                    }

                    if (needDownload)
                    {
                        downloadQueue.Enqueue(newResInfo);
                    }
                }

                int totalUpdateCount = downloadQueue.Count;
                Debug.Log(string.Format("Begin Download Asset:\n 已更新过的资源数:{0}\n 需要进入游戏前更新资源数:{1}\n 游戏运行时需下载的小包资源数:{2}\n 更新资源总数:{3}\n",
                    alreadyUpdateCount,
                    totalUpdateCount,
                    _miniResConfig != null ? _miniResConfig.replaceResConfig.Count : 0,
                    resPatchInfo.updateList.Count));

                SPSDK.gameEvent("10006");   //开始解压
                ZipManager.Instance.StarWork();
                //等待资源更新完毕
                while (_finishedCount < totalUpdateCount)
                {
                    while (downloadQueue.Count > 0 && _downloadingCount < MAX_DOWNLOADCOUNT)
                    {
                        var newResInfo = downloadQueue.Dequeue();
                        ++_downloadingCount;
                        StartCoroutine(DownloadAssetTask(remoteRoot, newResInfo));
                    }
                    yield return null;
                    PrintInfo(string.Format("下载资源中，剩余{0}({1}/{2})...", FormatBytes(_remainingSize),
                        _finishedCount, totalUpdateCount));
                }
                ZipManager.Instance.StopWork();
                SPSDK.gameEvent("10007");   //结束解压

                //更新过程中没有发生错误才将本地ResConfig版本号更新为最新版本
                if (!_fatalError)
                {
                    PrintInfo("下载资源完成，正在加载游戏资源...");
                    _curResConfig.Version = resPatchInfo.EndVer;
                    _curResConfig.lz4CRC = resPatchInfo.EndLz4CRC;
                    _curResConfig.lzmaCRC = resPatchInfo.EndLzmaCRC;
                    _curResConfig.tileTexCRC = resPatchInfo.EndTexCRC;
                }

                //无论更新过程是否出错都需要保存一下ResConfig,否则重走更新流程时又需要重新下载一遍没有出错的资源了
                SaveResConfig(!_fatalError);
                if (_curResConfig.isMiniRes)
                {
                    SaveMiniResConfig();
                }
                isDownloadAsset = false;
                ////每次下载完新版本资源，清空Cache缓存，保证Cache的旧资源不会占用磁盘空间
                //Debug.Log(string.Format("GameResource:Clean Cache {0}", Caching.CleanCache()));

                stopwatch.Stop();
                var elapsed = stopwatch.Elapsed;
                Debug.Log(string.Format("下载更新资源总耗时:{0:00}:{1:00}:{2:00}:{3:00}", elapsed.Hours, elapsed.Minutes,
                    elapsed.Seconds, elapsed.Milliseconds / 10));

                if (_fatalError)
                {
                    //更新过程中有资源更新失败,提示用户重启游戏重走更新流程
                    if (_loadErrorHandler != null)
                    {
                        _loadErrorHandler("资源更新失败,请重启游戏重试");
                    }
                }
                else
                {
                    if (onFinish != null)
                        onFinish();
                }

                if (OnFinishResUpdate != null)
                    OnFinishResUpdate();

                SPSDK.gameEvent("10008");       //结束更新
                //TalkingDataHelper.OnEventSetp("GameUpdate", "Finish");
            }
        }

        private IEnumerator DownloadAssetTask(string remoteRoot, ResInfo newResInfo)
        {
            string url = newResInfo.GetRemotePath(remoteRoot);
            string outputDir = GameResPath.bundleRoot;

            yield return StartCoroutine(DownloadByUnityWebRequest(url, www =>
            {
                var fileBytes = www.downloadHandler.data;
                if (!string.IsNullOrEmpty(newResInfo.MD5))
                {
                    var fileMD5 = MD5Hashing.HashBytes(fileBytes);
                    if (fileMD5 != newResInfo.MD5)
                    {
                        OnDownloadAssetError(newResInfo, string.Format("url:{0} MD5值不匹配\n{1}|{2}", www.url, newResInfo.MD5, fileMD5));
                        return;
                    }
                }

                if (newResInfo.remoteZipType == CompressType.CustomZip)
                {
                    string outFolder = Path.GetDirectoryName(outputDir + "/" + newResInfo.bundleName);
                    var ms = new MemoryStream(fileBytes, false);
                    //资源解压更新完成，覆盖resConfig旧资源信息
                    ZipManager.Instance.Extract(url, outFolder, ms,
                        proxy => OnDownloadAssetFinish(newResInfo),
                        e => ThrowFatalException(string.Format("ZipExtract url:{0} error:{1}", url, e.Message)));
                }
                else
                {
                    //下载的是AssetBundle直接保存到指定目录，覆盖resConfig旧资源信息
                    string filePath = newResInfo.GetABPath(outputDir);
                    try
                    {
                        FileHelper.WriteAllBytes(filePath, fileBytes);
                        OnDownloadAssetFinish(newResInfo);
#if UNITY_EDITOR
                        Debug.Log(string.Format("Name:{0}\nFinish", filePath));
#endif
                    }
                    catch (Exception e)
                    {
                        ThrowFatalException("Save Asset Error: " + filePath + "\n" + e.Message);
                    }
                }
            }, e => OnDownloadAssetError(newResInfo, e)));
        }

        /// <summary>
        /// 更新资源失败,
        /// </summary>
        /// <param name="newResInfo"></param>
        /// <param name="error"></param>
        private void OnDownloadAssetError(ResInfo newResInfo, string error)
        {
            ++_finishedCount;
            --_downloadingCount;
            _remainingSize -= newResInfo.size;
            ThrowFatalException(error);
        }

        private void OnDownloadAssetFinish(ResInfo newResInfo)
        {
            newResInfo.isPackageRes = false;

            // 删除包内同key不同后缀资源
            ResInfo oldResInfo = null;
            if (_curResConfig.Manifest.TryGetValue(newResInfo.bundleName, out oldResInfo))
            {
                if (!oldResInfo.isPackageRes)
                {
                    var filePath = oldResInfo.GetABPath(GameResPath.bundleRoot);
                    try
                    {
                        FileHelper.DeleteFile(filePath);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                }
            }

            _curResConfig.Manifest[newResInfo.bundleName] = newResInfo;
            ++_finishedCount;
            --_downloadingCount;
            _remainingSize -= newResInfo.size;
        }


        private bool IsChangeResConfigType()
        {
            return _curResConfig != null && _curResConfig.serverType != _versionConfig.serverType;
        }

        private string GetCDNBundleRoot()
        {
            return _cdnUrlRoot + "/" + GameResPath.REMOTE_BUNDLE_ROOT;
        }

        #region 小包资源运行时下载

        private StringBuilder _minResDownloadInfo;

        public event Action OnMinResUpdateBegin;
        //小包资源下载开始事件
        public event Action OnMinResUpdateFinish;
        //小包资源下载成功事件
        public AutoUpgradeType AutoUpgrade = AutoUpgradeType.WIFI;
        //小包自动下载资源开关
        public enum AutoUpgradeType
        {
            NONE,
            //禁用小包资源自动下载
            WIFI,
            //仅WIFI下自动下载
            ALL
            //所有网络下自动下载
        }

        private long _totalMiniResSize;
        private long _remainMiniResSize;
        private Coroutine _upgradeTotalResTask;
        private HashSet<string> _miniResDownloadingSet;
        //记录当前正在下载中的小包缺失资源Key

        /// <summary>
        /// 当前升级到整包缺少的资源总大小
        /// </summary>
        public long TotalMiniResSize
        {
            get { return _totalMiniResSize; }
        }

        /// <summary>
        /// 剩余小包资源大小
        /// </summary>
        public long RemainMiniResSize
        {
            get { return _remainMiniResSize; }
        }

        /// <summary>
        /// 标记是否正在升级为整包
        /// </summary>
        public bool IsUpgradeTotalRes
        {
            get { return _upgradeTotalResTask != null; }
        }

        /// <summary>
        /// 初始化小包资源下载信息,计算升级到整包还需下载多少资源
        /// </summary>
        private void SetupMiniResDownloadInfo()
        {
            if (_curResConfig == null || !_curResConfig.isMiniRes)
            {
                return;
            }

            _miniResDownloadingSet = new HashSet<string>();
            foreach (var pair in _miniResConfig.replaceResConfig)
            {
                var resInfo = _curResConfig.GetResInfo(pair.Key);
                if (resInfo != null)
                {
                    _totalMiniResSize += resInfo.size;
                }
                else
                {
                    Debug.LogError("当前ResConfig不存在该小包资源信息,请检查: " + pair.Key);
                }
            }

            _remainMiniResSize = _totalMiniResSize;
        }

        public bool StartUpgrade()
        {
            if (_upgradeTotalResTask != null)
                return false;

            _upgradeTotalResTask = StartCoroutine(UpgradeTotalRes());
            return true;
        }

        public bool StopUpgrade()
        {
            if (_upgradeTotalResTask == null)
                return false;

            StopCoroutine(_upgradeTotalResTask);
            _upgradeTotalResTask = null;
            return true;
        }

        /// <summary>
        /// 返回true,代表该资源是小包缺失资源,还没有下载下来
        /// </summary>
        /// <param name="bundleName"></param>
        /// <returns></returns>
        public bool ValidateMiniRes(string bundleName)
        {
            return _miniResConfig != null && _miniResConfig.replaceResConfig.ContainsKey(bundleName);
        }

        /// <summary>
        /// 返回true,代表该资源是小包资源,还没有下载下来,并返回其替代资源Key
        /// </summary>
        /// <param name="bundleName"></param>
        /// <param name="replaceKey"></param>
        /// <returns></returns>
        public bool TryGetReplaceRes(string bundleName, out string replaceKey)
        {
            replaceKey = "";
            return _miniResConfig != null && _miniResConfig.replaceResConfig.TryGetValue(bundleName, out replaceKey);
        }

        private IEnumerator UpgradeTotalRes()
        {
            var downloadQueue = new Queue<string>(_miniResConfig.replaceResConfig.Keys);
            string remoteRoot = GetCDNBundleRoot();

            while (downloadQueue.Count > 0)
            {
                while (downloadQueue.Count > 0 && _miniResDownloadingSet.Count < MAX_DOWNLOADCOUNT)
                {
                    string bundleName = downloadQueue.Dequeue();
                    StartCoroutine(DownloadMiniResTask(remoteRoot, bundleName));
                }
                yield return null;
            }

            while (_miniResConfig.replaceResConfig.Count > 0)
            {
                yield return new WaitForSeconds(0.5f);
            }

            //小包资源下载完毕,清空小包下载协程
            _upgradeTotalResTask = null;
            Debug.Log("Finish UpgradeTotalRes");
        }

        private IEnumerator DownloadMiniResTask(string remoteRoot, string bundleName)
        {
            //正在下载该小包资源或者已下载的直接跳过
            if (_miniResDownloadingSet.Contains(bundleName)
                         || !_miniResConfig.replaceResConfig.ContainsKey(bundleName))
            {
                yield break;
            }

            var resInfo = _curResConfig.GetResInfo(bundleName);
            if (resInfo != null)
            {
                string url = resInfo.GetRemotePath(remoteRoot);
                _miniResDownloadingSet.Add(bundleName);
                yield return StartCoroutine(DownloadByUnityWebRequest(url,
                    www => SaveMiniRes(bundleName, www),
                    error =>
                    {
                        //下载失败从下载列表中移除
                        _miniResDownloadingSet.Remove(bundleName);
                        Debug.LogError("DownloadMinResTask: " + url + " failed");
                    }));
            }
            else
            {
                Debug.LogError("当前ResConfig不存在该小包资源信息,请检查: " + bundleName);
            }
        }

        /// <summary>
        /// 小包资源下载完毕后缓存到本地,并将小包信息从miniResConfig中移除
        /// </summary>
        /// <param name="bundleName"></param>
        /// <param name="www"></param>
        private void SaveMiniRes(string bundleName, UnityWebRequest www)
        {
            if (!_miniResConfig.replaceResConfig.ContainsKey(bundleName))
                return;

            var newResInfo = _curResConfig.GetResInfo(bundleName);
            var fileBytes = www.downloadHandler.data;
            if (!string.IsNullOrEmpty(newResInfo.MD5))
            {
                var fileMD5 = MD5Hashing.HashBytes(fileBytes);
                if (fileMD5 != newResInfo.MD5)
                {
                    _miniResDownloadingSet.Remove(bundleName);
                    Debug.LogError(string.Format("url:{0} MD5值不匹配\n{1}|{2}", www.url, newResInfo.MD5, fileMD5));
                    return;
                }
            }

            string outputDir = GameResPath.bundleRoot;
            if (newResInfo.remoteZipType == CompressType.CustomZip)
            {
                ZipManager.Instance.StarWork();
                string outFolder = Path.GetDirectoryName(outputDir + "/" + newResInfo.bundleName);
                var ms = new MemoryStream(fileBytes, false);
                ZipManager.Instance.Extract(www.url, outFolder, ms,
                    proxy => OnSaveMiniResFinish(newResInfo),
                    e =>
                    {
                        _miniResDownloadingSet.Remove(newResInfo.bundleName);
                        Debug.LogError(string.Format("ZipExtract url:{0}\nerror:{1}", www.url, e.Message));
                    });
            }
            else
            {
                string filePath = newResInfo.GetABPath(GameResPath.bundleRoot);
                try
                {
                    FileHelper.WriteAllBytes(filePath, fileBytes);
                    OnSaveMiniResFinish(newResInfo);
                }
                catch (Exception e)
                {
                    _miniResDownloadingSet.Remove(newResInfo.bundleName);
                    Debug.LogError("Save MiniRes Error: " + filePath + "\n" + e.Message);
                }
            }
        }

        /// <summary>
        /// 小包资源下载保存成功后,更新本地miniResConfig和resConfig的信息
        /// </summary>
        /// <param name="newResInfo"></param>
        private void OnSaveMiniResFinish(ResInfo newResInfo)
        {
            //小包资源下载完毕,将包内资源标志置为false,并从小包配置信息中移除
            _miniResConfig.replaceResConfig.Remove(newResInfo.bundleName);
            newResInfo.isPackageRes = false;
#if UNITY_EDITOR
            if (_minResDownloadInfo == null)
                _minResDownloadInfo = new StringBuilder();
            _minResDownloadInfo.AppendLine(newResInfo.bundleName);
            Debug.Log(string.Format("=======小包资源下载成功:{0}", newResInfo.bundleName));
#endif

            long remainSize = _remainMiniResSize - newResInfo.size;
            _remainMiniResSize = remainSize > 0 ? remainSize : 0L;
            //每下载5个资源,保存一下配置,防止玩家意外终止游戏进程,丢失下载信息
            if (_miniResConfig.replaceResConfig.Count % 5 == 0)
            {
                //小包资源全部下载完毕,将当前小包标记置为false
                if (_miniResConfig.replaceResConfig.Count == 0)
                {
                    _curResConfig.isMiniRes = false;
                    ZipManager.Instance.StopWork();
                }
                SaveMiniResConfig();
                SaveResConfig();
            }

            if (OnMinResUpdateFinish != null)
                OnMinResUpdateFinish();

            //缓存完毕,从下载队列中移除
            _miniResDownloadingSet.Remove(newResInfo.bundleName);
        }

        /// <summary>
        /// 静默下载小包资源,等下载完毕后再次触发资源加载处理回调
        /// </summary>
        /// <param name="loadRequest"></param>
        private void SlientDownloadMiniRes(AssetLoadRequest loadRequest)
        {
            if (AutoUpgrade == AutoUpgradeType.NONE)
                return;
            if (AutoUpgrade == AutoUpgradeType.WIFI && BaoyugameSdk.getNetworkType() != BaoyugameSdk.NET_STATE_WIFI)
                return;
            if (_miniResDownloadingSet.Contains(loadRequest.bundleName))
                return;

            StartCoroutine(SlientDownloadMiniResTask(loadRequest));
        }

        private IEnumerator SlientDownloadMiniResTask(AssetLoadRequest loadRequest)
        {
            var newResInfo = _curResConfig.GetResInfo(loadRequest.bundleName);
            if (newResInfo != null)
            {
                if (OnMinResUpdateBegin != null)
                    OnMinResUpdateBegin();

                string remoteRoot = GetCDNBundleRoot();

                //必须先下载该资源,保证不会重复下载该资源,再下载其依赖资源
                StartCoroutine(DownloadMiniResTask(remoteRoot, loadRequest.bundleName));

                if (newResInfo.Dependencies.Count > 0)
                {
                    var allDependencies = _curResConfig.GetAllDependencies(loadRequest.bundleName);
                    foreach (string refBundleName in allDependencies)
                    {
                        StartCoroutine(DownloadMiniResTask(remoteRoot, refBundleName));
                    }

                    //等待所有依赖资源下载完毕
                    while (allDependencies.Any(s => _miniResDownloadingSet.Contains(s)))
                    {
                        yield return null;
                    }
                }

                //等待该资源下载完毕
                while (_miniResDownloadingSet.Contains(loadRequest.bundleName))
                {
                    yield return null;
                }

                //所有相关资源下载完毕,重新触发整包资源加载流程
                _asyncLoadingQueue.Enqueue(loadRequest.bundleName, 0);
                ProcessLoadQueue();
            }
        }

        #endregion

        #endregion

        #region WWW,UnityWebRequest读取数据接口

        /// <summary>
        /// 通过WWW读取或者下载文件,可用于读取本地文件,也可以下载服务器上的
        /// </summary>
        /// <param name="url"></param>
        /// <param name="onFinish"></param>
        /// <param name="onError"></param>
        /// <param name="maxRetry"></param>
        /// <param name="retryDelay"></param>
        /// <param name="converText">转换为text需要转换文件编码非常耗时，某些操作下不需要</param>
        public void LoadFileByWWW(string url, Action<WWW> onFinish, Action<string> onError = null, int maxRetry = 1, float retryDelay = 0.5f)
        {
            if (string.IsNullOrEmpty(url))
            {
                if (onError != null)
                    onError("LoadFileByWWW:url is null");
                else
                    Debug.LogError("LoadFileByWWW:url is null");
                return;
            }
            StartCoroutine(DownloadByWWW(url, onFinish, onError, maxRetry, retryDelay));
        }

        private IEnumerator DownloadByWWW(string url, Action<WWW> onFinish, Action<string> onError, int maxRetry = MAX_RETRYCOUNT, float retryDelay = 0.5f)
        {
            int retry = 0;
            while (retry++ < maxRetry)
            {
                using (var www = new WWW(url))
                {
                    yield return www;

                    if (!string.IsNullOrEmpty(www.error))
                    {
                        if (retry >= maxRetry)
                        {
                            string error = string.Format("Load url:{0}\nerror:{1}", www.url, www.error);
                            if (onError != null)
                                onError(error);
                            else
                                Debug.LogError(error);
                        }
                        else
                        {
                            Debug.Log(string.Format("Try again Link url : {0} , time : {1}", url, retry));
                            yield return new WaitForSeconds(retryDelay);
                        }
                    }
                    else
                    {
                        //转换为text需要转换文件编码非常耗时，某些操作下不需要
                        if (www.text.StartsWith("<html>"))
                        {
                            string error = string.Format("Load url:{0}\nerror:{1}", www.url, www.text);
                            if (onError != null)
                                onError(error);
                            else
                                Debug.LogError(error);
                        }
                        else
                        {
                            if (onFinish != null)
                                onFinish(www);
                        }

                        break; //跳出重试循环
                    }
                }
            }
        }

        public AssetHandler LoadSceneTileMapByWWW(string assetName, OnLoadFinish finish, OnLoadError error, int limitCount = 0)
        {
            AssetHandler handler = new AssetHandler(assetName, null, finish, error);
            string refBundleName = GetBundleName(assetName, ResGroup.TileMap);
            var refResInfo = _curResConfig.GetResInfo(refBundleName);
            string filePath = null;
            if (refResInfo != null)
            {
                filePath = refResInfo.isPackageRes
                    ? refResInfo.GetABPath(GameResPath.packageBundleUrlRoot)
                    : refResInfo.GetABPath(FileHelper.GetLocalFileUrl(GameResPath.bundleRoot));
            }
            else
            {
                Debug.LogError("refResInfo is null: " + refBundleName);
                handler.OnError();
                return handler;
            }
            StartCoroutine(LoadTileMapByWWW(filePath, handler, limitCount));
            return handler;
        }

        private int tileLastFrameCount;
        private int tileLoadCount;

        private IEnumerator LoadTileMapByWWW(string url, AssetHandler handler, int limitCount)
        {
            using (var www = new WWW(url))
            {
                yield return www;

                if (!string.IsNullOrEmpty(www.error))
                {
                    Debug.LogError("LoadSceneTileMapError:" + www.error);
                    handler.OnError();
                }
                else
                {
                    //限制每帧加载多少张贴图
                    if (limitCount != 0)
                    {
                        while (tileLastFrameCount == Time.frameCount && tileLoadCount >= limitCount)
                        {
                            yield return null;
                        }
                        if (tileLastFrameCount != Time.frameCount)
                        {
                            tileLastFrameCount = Time.frameCount;
                            tileLoadCount = 1;
                        }
                        else
                        {
                            tileLoadCount++;
                        }
                    }
                    if (handler.disposed == false)
                    {
                        Texture2D texture = www.textureNonReadable;
                        if (texture == null)
                        {
                            Debug.LogError("LoadSceneTileMap Texture Conver fail");
                            handler.OnError();
                        }
                        else
                        {
                            texture.wrapMode = TextureWrapMode.Clamp;
                            try
                            {
                                handler.Excute(texture);
                            }
                            catch
                            {
                                Destroy(texture);
                                throw;
                            }
                        }
                    }
                    www.Dispose();

                }
            }
        }

        /// <summary>
        /// 一般用于加载一些服务器上的临时资源,如:个人空间的图片,注意:不支持读取jar包内的文件
        /// </summary>
        /// <param name="url"></param>
        /// <param name="onFinish"></param>
        /// <param name="onError"></param>
        public void LoadFileByUnityWebRequest(string url, Action<UnityWebRequest> onFinish, Action<string> onError)
        {
            LoadFileByUnityWebRequest(UnityWebRequest.Get(url), onFinish, onError);
        }

        public void LoadFileByUnityWebRequest(UnityWebRequest www, Action<UnityWebRequest> onFinish, Action<string> onError)
        {
            if (www == null || string.IsNullOrEmpty(www.url))
            {
                if (onError != null)
                    onError("LoadFileByWWW:url is null");
                else
                    Debug.LogError("LoadFileByWWW:url is null");
                return;
            }

            StartCoroutine(LoadFileByUnityWebRequestTask(www, onFinish, onError));
        }

        private IEnumerator LoadFileByUnityWebRequestTask(UnityWebRequest www, Action<UnityWebRequest> onFinish, Action<string> onError)
        {
            yield return www.Send();

            if (www.isNetworkError)
            {
                string error = string.Format("Load url:{0}\nerror:{1}", www.url, www.error);
                if (onError != null)
                    onError(error);
                else
                    Debug.LogError(error);
            }
            else
            {
                if (onFinish != null)
                    onFinish(www);
            }
        }

        private IEnumerator DownloadByUnityWebRequest(string url, Action<UnityWebRequest> onFinish, Action<string> onError, int maxRetry = MAX_RETRYCOUNT, float retryDelay = 2.0f)
        {
            int retry = 0;
            while (retry++ < maxRetry)
            {
                using (var www = UnityWebRequest.Get(url))
                {
                    yield return www.Send();

                    if (www.isNetworkError)
                    {
                        if (retry >= maxRetry)
                        {
                            string error = string.Format("Load url:{0}\nerror:{1}", www.url, www.error);
                            if (onError != null)
                                onError(error);
                            else
                                Debug.LogError(error);
                        }
                        else
                        {
                            Debug.Log(string.Format("Try again Link url : {0} , time : {1}", www.url, retry));
                            yield return new WaitForSeconds(retryDelay);
                        }
                    }
                    else
                    {
                        if (www.downloadHandler.text.StartsWith("<html>"))
                        {
                            string error = string.Format("Load url:{0}\nerror:{1}", www.url, www.downloadHandler.text);
                            if (onError != null)
                                onError(error);
                            else
                                Debug.LogError(error);
                        }
                        else
                        {
                            if (onFinish != null)
                                onFinish(www);
                        }

                        break; //跳出重试循环
                    }
                }
            }
        }

        #endregion

        #region 资源加载接口

        private Dictionary<string, Shader> _shaderInfoDic;
        //记录已加载的所有Bundle内的Shader信息
        private Dictionary<string, AssetBundleInfo> _abInfoDic;
        //记录已加载AssetBundle信息
        private Dictionary<string, AssetLoadRequest> _loadRequestDic;
        //记录加载请求信息字典

#if UNITY_EDITOR
        public Dictionary<string, AssetBundleInfo> AbInfoDic
        {
            get { return _abInfoDic; }
        }
#endif

        public void LoadCommonAsset(Action onFinish)
        {
            //编辑器本地模式下无需加载公共资源
            if (ResLoadMode == LoadMode.EditorLocal)
            {
                if (onFinish != null)
                    onFinish();
            }
            else
            {
                StartCoroutine(PreloadCommonAsset(onFinish));
            }
        }

        private IEnumerator PreloadCommonAsset(Action onFinish)
        {
            if (PlayerPrefs.GetInt("FirstRun", 0) == 1)
            {
                PrintInfo("预加载游戏公共资源...");
            }
            else
            {
                PrintInfo("首次加载游戏可能需要等待较长时间（本过程不消耗流量）");
                PlayerPrefs.SetInt("FirstRun", 1);
                PlayerPrefs.Save();
            }
            string packageBundleRoot = GameResPath.packageBundleRoot;
            string bundleRoot = GameResPath.bundleRoot;
            _abInfoDic = new Dictionary<string, AssetBundleInfo>(_curResConfig.Manifest.Count);
            Stack<ResInfo> preloadStack = new Stack<ResInfo>();
            foreach (var item in _curResConfig.Manifest)
            {
                var resInfo = item.Value;
                if (!resInfo.preload)
                    continue;
                preloadStack.Push(resInfo);
                PushDependencies(resInfo, preloadStack);
            }
            while (preloadStack.Count > 0)
            {
                var resInfo = preloadStack.Pop();
                if (_abInfoDic.ContainsKey(resInfo.bundleName))
                    continue;
                var request = AssetBundle.LoadFromFileAsync(resInfo.isPackageRes
                  ? resInfo.GetABPath(packageBundleRoot)
                  : resInfo.GetABPath(bundleRoot));
                yield return request;

                var assetBundle = request.assetBundle;
                if (assetBundle != null)
                {
                    var commonBundleInfo = CreateABInfo(resInfo.bundleName, assetBundle);
                    if (resInfo.bundleName == GameResPath.AllShaderBundleName)
                    {
                        //加载所有Shader资源,并解析编译
                        var shaders = assetBundle.LoadAllAssets<Shader>();
                        if (shaders != null)
                        {
                            _shaderInfoDic = new Dictionary<string, Shader>(shaders.Length);
                            foreach (Shader shader in shaders)
                            {
                                if (shader != null)
                                    _shaderInfoDic.Add(shader.name, shader);
                            }
                        }
                        Shader.WarmupAllShaders();
                    }
                    else
                    {
                        yield return commonBundleInfo.assetBundle.LoadAllAssetsAsync();
                    }
                }
                else
                {
                    Debug.LogError("加载AssetBundle失败: " + resInfo.bundleName);
                }
            }
            SetupMiniResDownloadInfo();
            //更新流程结束,记录一下本次更新的VersionConfig信息
            SaveVersionConfig();

            PrintInfo("加载游戏公共资源完毕");
            if (onFinish != null)
                onFinish();
        }

        private void PushDependencies(ResInfo resInfo, Stack<ResInfo> preloadStack)
        {
            List<string> dependencies = resInfo.Dependencies;
            foreach (string resKey in dependencies)
            {
                ResInfo itemResInfo = _curResConfig.GetResInfo(resKey);
                preloadStack.Push(itemResInfo);
                PushDependencies(itemResInfo, preloadStack);
            }
        }

        public Shader FindShader(string shaderName)
        {
            Shader shader = null;
            if (_shaderInfoDic != null)
            {
                _shaderInfoDic.TryGetValue(shaderName, out shader);
            }

            return shader ?? Shader.Find(shaderName);
        }


        /// <summary>
        /// 获取已经加载的AssetBundle信息
        /// </summary>
        /// <param name="bundleName"></param>
        /// <returns></returns>
        public AssetBundleInfo GetAssetBundleInfo(string bundleName)
        {
            if (_abInfoDic == null)
                return null;

            AssetBundleInfo abInfo;
            _abInfoDic.TryGetValue(bundleName, out abInfo);
            return abInfo;
        }

        public static string GetBundleName(string assetName, ResGroup resGroup)
        {
            string bundleName = assetName.ToLower();
            if (resGroup == ResGroup.None)
                return bundleName;

            bundleName = resGrounpMap[(int)resGroup] + bundleName;
            return bundleName;
        }

        public static ResGroup GetResGroup(string bundleName)
        {
            foreach (KeyValuePair<int, string> keyValuePair in resGrounpMap)
            {
                if (bundleName.StartsWith(keyValuePair.Value))
                {
                    return (ResGroup)keyValuePair.Key;
                }
            }
            return ResGroup.None;
        }
        #region 同步加载Bundle资源接口

        /// <summary>
        /// 一般业务层只需要使用资源名和资源分组类型来加载资源,因为一般来说都是一个资源对应一个Bundle的
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetName"></param>
        /// <param name="resGroup"></param>
        /// <returns></returns>
        public T LoadAsset<T>(string assetName, ResGroup resGroup) where T : UnityEngine.Object
        {
            return LoadAsset<T>(GetBundleName(assetName, resGroup), assetName);
        }

        public T LoadAsset<T>(string bundleName, string assetName) where T : UnityEngine.Object
        {
            var asset = LoadAsset(bundleName, assetName, typeof(T)) as T;
            return asset;
        }

        /// <summary>
        /// 一般业务层只需要使用资源名和资源分组类型来加载资源,因为一般来说都是一个资源对应一个Bundle的
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="resGroup"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public Object LoadAsset(string assetName, ResGroup resGroup, Type type = null)
        {
            return LoadAsset(GetBundleName(assetName, resGroup), assetName, type);
        }

        public Object LoadAsset(string bundleName, string assetName, Type type = null)
        {
            if (type == null)
                type = typeof(UnityEngine.Object);

#if UNITY_EDITOR
            if (ResLoadMode == LoadMode.EditorLocal)
            {
                return LoadAssetFromProject(bundleName, assetName, type);
            }
#endif
            return LoadAssetBundleImmediate(bundleName, assetName, type);
        }

#if UNITY_EDITOR
        /// <summary>
        ///     同步方式加载资源,直接加载工程内的资源,无需处理依赖资源加载
        /// </summary>
        /// <param name="bundleName"></param>
        /// <param name="assetName"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private Object LoadAssetFromProject(string bundleName, string assetName, Type type)
        {
            //编辑器模式下无需加载依赖资源
            var assetPaths = string.IsNullOrEmpty(assetName) ? AssetDatabase.GetAssetPathsFromAssetBundle(bundleName) : AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(bundleName, assetName);
            if (type == typeof(GameObject))
            {
                assetPaths = assetPaths.Where(item => item.EndsWith(".prefab") || item.ToLower().EndsWith(".fbx") || item.EndsWith(".obj"))
                    .ToArray();
            }
            if (assetPaths.Length > 0)
            {
                var asset = AssetDatabase.LoadAssetAtPath(assetPaths[0], type);
                if (asset == null)
                {
                    string content = string.Format("Load <{0}> Asset is null,assetPath = {1}", bundleName, assetPaths);
                    Debug.LogError(content);
#if !USE_JSZ
                    if (_assetErrorPause && assetErrorPauseStrings.Any(content.Contains))
                        EditorApplication.isPaused = true;
#endif
                }
                return asset;
            }

            return null;
        }
#endif

        /// <summary>
        ///     同步方式加载AssetBundle,同步加载方式只适用于加载未经压缩的AssetBundle
        /// </summary>
        /// <param name="bundleName">Res info key.</param>
        /// <param name="assetName"></param>
        /// <param name="type"></param>
        private Object LoadAssetBundleImmediate(string bundleName, string assetName, Type type)
        {
            if (_curResConfig == null)
                return null;

            var resInfo = _curResConfig.GetResInfo(bundleName);
            if (resInfo == null)
            {
                Debug.LogError(string.Format("加载失败，没有<{0}>资源的信息", bundleName));
                return null;
            }

            //如果该资源已经加载，直接返回
            AssetBundleInfo abInfo;
            Object asset = null;
            if (_abInfoDic.TryGetValue(bundleName, out abInfo))
            {
                asset = abInfo.LoadAsset(assetName, type);
            }
            else
            {
                //获取该Bundle的所有依赖资源
                string packageBundleRoot = GameResPath.packageBundleRoot;
                string bundleRoot = GameResPath.bundleRoot;
                if (resInfo.Dependencies.Count > 0)
                {
                    var allDependencies = _curResConfig.GetAllDependencies(bundleName);
                    foreach (string refBundleName in allDependencies)
                    {
                        //如果该依赖资源已加载,只增加其引用计数直接跳过
                        if (_abInfoDic.ContainsKey(refBundleName))
                        {
                            _abInfoDic[refBundleName].AddRef(resInfo.bundleName);
                            continue;
                        }

                        var refResInfo = _curResConfig.GetResInfo(refBundleName);
                        if (refResInfo != null)
                        {
                            var refAb = AssetBundle.LoadFromFile(refResInfo.isPackageRes ? refResInfo.GetABPath(packageBundleRoot) : refResInfo.GetABPath(bundleRoot));
                            var refAbInfo = CreateABInfo(refBundleName, refAb);
                            refAbInfo.AddRef(bundleName);
                        }
                        else
                        {
                            Debug.LogError("refResInfo is null: " + refBundleName);
                        }
                    }
                }

                var ab = AssetBundle.LoadFromFile(resInfo.isPackageRes ? resInfo.GetABPath(packageBundleRoot) : resInfo.GetABPath(bundleRoot));

                if (ab == null)
                    Debug.LogError(string.Format("Load <{0}> AssetBundle is null", bundleName));
                else
                {
                    abInfo = CreateABInfo(bundleName, ab);
                    asset = abInfo.LoadAsset(assetName, type);
                }
            }

            return asset;
        }

        public AssetHandler LoadAssetAsyncByMainThread(string assetName, ResGroup resGroup, OnLoadFinish onFinish, OnLoadError onError = null, Type type = null, float priority = 100f)
        {
            return LoadAssetAsyncByMainThread(GetBundleName(assetName, resGroup), assetName, onFinish, onError, type, priority);
        }

        private SimplePriorityQueue<string> _syncLoadingQueue;
        private Stopwatch syncStopwatch;
        private int syncStopwatchFrame;
        const int syncLoadMillisconds = 30;
        public AssetHandler LoadAssetAsyncByMainThread(string bundleName, string assetName, OnLoadFinish onFinish, OnLoadError onError = null, Type type = null, float priority = 100f)
        {
            if (type == null)
                type = typeof(UnityEngine.Object);

            if (_syncLoadingQueue == null)
                _syncLoadingQueue = new SimplePriorityQueue<string>();

            if (_loadRequestDic == null)
                _loadRequestDic = new Dictionary<string, AssetLoadRequest>(32);

            if (syncStopwatch == null)
            {
                syncStopwatch = new Stopwatch();
                syncStopwatch.Reset();
                syncStopwatchFrame = Time.frameCount;
            }

            AssetHandler handler;
            AssetLoadRequest loadRequest;
            bool isMiniRes = ValidateMiniRes(bundleName);
            if (_loadRequestDic.TryGetValue(bundleName, out loadRequest))   //已经发起加载资源，如果一开始发起异步加载则走异步流程，否则走同步流程。
            {
                handler = loadRequest.AddHandler(assetName, type, onFinish, onError);
            }
            else
            {
                //非缺失资源,直接加入加载队列
                if (!isMiniRes)
                {
                    handler = CreateLoadAssetSyncRequest(bundleName, assetName, type, onFinish, onError, priority);
                }
                else //小包缺失资源走异步加载流程
                {
                    handler = CreateLoadAssetAsyncRequest(bundleName, assetName, type, onFinish, onError, priority);
                }
            }
            return handler;
        }

        private AssetHandler CreateLoadAssetSyncRequest(string bundleName, string assetName, Type type, OnLoadFinish onFinish, OnLoadError onError, float priority)
        {
            if (syncStopwatchFrame != Time.frameCount)
            {
                syncStopwatch.Reset();
                syncStopwatchFrame = Time.frameCount;
            }
            if (syncStopwatch.ElapsedMilliseconds <= syncLoadMillisconds && _syncLoadingQueue.Count == 0)    //如果同步加载总耗时小于30ms，直接加载。 否则加到队列中
            {
                syncStopwatch.Start();
                Object asset = LoadAsset(bundleName, assetName, type);
                AssetHandler handler = new AssetHandler(assetName, type, onFinish, onError);
                if (asset != null)
                    handler.Excute(asset);
                else
                    handler.OnError();
                syncStopwatch.Stop();
                return null;
            }
            else
            {
                AssetLoadRequest loadRequest = new AssetLoadRequest(bundleName);
                AssetHandler handler = loadRequest.AddHandler(assetName, type, onFinish, onError);
                _loadRequestDic.Add(bundleName, loadRequest);
                _syncLoadingQueue.Enqueue(bundleName, priority);
                ProcessLoadSyncQueue();
                return handler;
            }
        }
        private bool syncProcessingActive;

        private void ProcessLoadSyncQueue()
        {
            if (syncProcessingActive || _syncLoadingQueue.Count <= 0)
                return;
            syncProcessingActive = true;
            StartCoroutine(ProcessLoadSyncQueueCoroutine());
        }

        IEnumerator ProcessLoadSyncQueueCoroutine()
        {
            yield return null;  //第一次进 肯定超时直接等下一帧
            if (syncStopwatchFrame != Time.frameCount)
            {
                syncStopwatch.Reset();
                syncStopwatchFrame = Time.frameCount;
            }
            while (_syncLoadingQueue.Count > 0)
            {
                syncStopwatch.Start();
                string bundleName = _syncLoadingQueue.Dequeue();
                AssetLoadRequest loadRequest;
                if (_loadRequestDic.TryGetValue(bundleName, out loadRequest))
                {
                    _loadRequestDic.Remove(bundleName);
                    if (!loadRequest.isValid)
                        continue;
                    for (int i = 0; i < loadRequest.Handlers.Count; i++)
                    {
                        AssetHandler handler = loadRequest.Handlers[i];
                        if (handler.disposed)
                            continue;
                        Object asset = LoadAsset(bundleName, handler.assetName, handler.type);
                        if (asset != null)
                            handler.Excute(asset);
                        else
                            handler.OnError();
                    }
                }
                if (syncStopwatch.ElapsedMilliseconds > syncLoadMillisconds)
                {
                    syncStopwatch.Stop();
                    yield return null;
                    if (syncStopwatchFrame != Time.frameCount)
                    {
                        syncStopwatch.Reset();
                        syncStopwatchFrame = Time.frameCount;
                    }
                }
            }
            syncStopwatch.Stop();
            syncProcessingActive = false;
        }
        #endregion

        #region 异步加载Bundle资源接口

        public AssetHandler LoadAssetAsync(string assetName, ResGroup resGroup, OnLoadFinish onFinish, OnLoadError onError = null, Type type = null, float priority = 100f)
        {
            return LoadAssetAsync(GetBundleName(assetName, resGroup), assetName, onFinish, onError, type, priority);
        }

        public AssetHandler LoadAssetAsync(string bundleName, string assetName, OnLoadFinish onFinish, OnLoadError onError = null, Type type = null, float priority = 100f)
        {
            if (type == null)
                type = typeof(UnityEngine.Object);

            if (_asyncLoadingQueue == null)
                _asyncLoadingQueue = new SimplePriorityQueue<string>();

            if (_loadRequestDic == null)
                _loadRequestDic = new Dictionary<string, AssetLoadRequest>(32);

            return CreateLoadAssetAsyncRequest(bundleName, assetName, type, onFinish, onError, priority);
        }

        public delegate void OnLoadFinish(Object asset);

        public delegate void OnLoadError();

        /// <summary>
        /// disposed标记了该资源加载回调处理是否无效了
        /// 如果异步加载时间过长,相应的Controller已经销毁了,销毁前记得把diposed置为true
        /// </summary>
        public class AssetHandler
        {
            public bool disposed { get; private set; }
            //用于标记资源加载回调是否已丢弃
            public readonly string assetName;
            public readonly Type type;
            private OnLoadError onError;
            private OnLoadFinish onFinish;

            public AssetHandler(string assetName, Type type, OnLoadFinish onFinish, OnLoadError onError)
            {
                this.assetName = assetName;
                this.type = type;
                this.onFinish = onFinish;
                this.onError = onError;
            }

            public void Excute(Object asset)
            {
                if (disposed)
                    return;

                try
                {
                    //如果接收回调的脚本已销毁，不进行回调处理
                    if (onFinish != null)
                    {
                        onFinish(asset);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("AssetHandler.Excute error: " + assetName + "\n" + e.Message + "\n" + e.StackTrace);
                    OnError();
                }
            }

            public void OnError()
            {
                if (disposed)
                    return;

                if (onError != null)
                {
                    try
                    {
                        onError();
                    }
                    catch (Exception e)
                    {
                        CSGameDebuger.LogException(e);
                    }
                }
            }

            public void Dispose()
            {
                disposed = true;
                onFinish = null;
                onError = null;
            }

            public override string ToString()
            {
                return "Target: " + (this.onFinish == null ? "Null" : this.onFinish.Target.ToString())
                + ",Method: " + (this.onFinish == null ? "Null" : this.onFinish.Method.ToString());
            }
        }

        /// <summary>
        /// 资源加载请求实体,封装了每个Bundle的异步加载请求
        /// 在请求未处理之前,业务层请求加载相同的Bundle时只会增加AssetHandler
        /// </summary>
        public class AssetLoadRequest
        {
            public readonly string bundleName;
            internal readonly List<AssetHandler> Handlers;

            public AssetLoadRequest(string bundleName)
            {
                this.bundleName = bundleName;
                Handlers = new List<AssetHandler>();
            }

            public bool isValid
            {
                get { return Handlers.Count > 0 && Handlers.Any(t => !t.disposed); }
            }

            public AssetHandler AddHandler(string assetName, Type type, OnLoadFinish onFinish, OnLoadError onError)
            {
                var handler = new AssetHandler(assetName, type, onFinish, onError);
                Handlers.Add(handler);
                return handler;
            }

            public bool RemoveHandler(AssetHandler handler)
            {
                return Handlers.Remove(handler);
            }
        }

        //异步加载请求Key优先级队列
        private SimplePriorityQueue<string> _asyncLoadingQueue;

        /// <summary>
        /// 创建Bundle异步加载请求,如果已存在于加载队列中时,只添加回调处理方法
        /// 返回AssetHandler供业务层控制其diposed状态
        /// </summary>
        /// <param name="bundleName"></param>
        /// <param name="assetName"></param>
        /// <param name="type"></param>
        /// <param name="onFinish"></param>
        /// <param name="onError"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        private AssetHandler CreateLoadAssetAsyncRequest(string bundleName, string assetName, Type type, OnLoadFinish onFinish, OnLoadError onError, float priority)
        {
            //TODO 资源加载DebugLog
            //Debug.LogError(bundleName);
            //如果该请求也存在队列中，只需添加回调事件处理器
            AssetHandler handler;
            AssetLoadRequest loadRequest;
            bool isMiniRes = ValidateMiniRes(bundleName);
            if (_loadRequestDic.TryGetValue(bundleName, out loadRequest))
            {
                handler = loadRequest.AddHandler(assetName, type, onFinish, onError);
            }
            else
            {
                loadRequest = new AssetLoadRequest(bundleName);
                handler = loadRequest.AddHandler(assetName, type, onFinish, onError);
                _loadRequestDic.Add(bundleName, loadRequest);
                //非缺失资源,直接加入加载队列
                if (!isMiniRes)
                    _asyncLoadingQueue.Enqueue(bundleName, priority);
            }

            if (isMiniRes)
            {
                //这个小包资源的loadRequest会一直保留下来
                //小包资源下载期间,如果又触发了相同的资源加载请求,只会增加其Handler,等待下载完毕重新触发资源加载流程才会移除
                //如果玩家关闭了小包资源下载开关,这个loadRequest会一直保留下来
                SlientDownloadMiniRes(loadRequest);
            }
            RemoveUnloadBundleToQueue(bundleName);
            ProcessLoadQueue();
            return handler;
        }

        /// <summary>
        /// 最大同时处理加载资源请求数
        /// </summary>
        private int _maxProcessCount = 8;

        public int MaxProcessCount
        {
            get { return _maxProcessCount; }
            set { _maxProcessCount = value; }
        }

        private bool processingActive;
        //标记资源加载协程是否启动
        private HashSet<AssetLoadRequest> _processingRequests;
        //记录当前正在加载中的请求列表
        private Dictionary<string, AssetBundleCreateRequest> _createBundleRequestDic;
        //记录当前异步加载AssetBundle创建请求信息

        private void ProcessLoadQueue()
        {
            //检验是否需要启动处理资源加载请求协程
            if (processingActive || _asyncLoadingQueue.Count <= 0)
                return;

            this.processingActive = true;
            StartCoroutine(ProcessLoadQueueCoroutine());
        }

        private IEnumerator ProcessLoadQueueCoroutine()
        {
            if (_processingRequests == null)
                _processingRequests = new HashSet<AssetLoadRequest>();

            if (_createBundleRequestDic == null)
                _createBundleRequestDic = new Dictionary<string, AssetBundleCreateRequest>();

            while (_asyncLoadingQueue.Count > 0)
            {
                //先等待一帧,这样同一帧内的相同资源请求都会创建好
                yield return null;
                if (_processingRequests.Count < _maxProcessCount)
                {
                    //每帧同时开启多个协程处理加载请求
                    for (int i = 0, imax = Mathf.Min(_asyncLoadingQueue.Count, _maxProcessCount - _processingRequests.Count); i < imax; i++)
                    {
                        string bundleName = _asyncLoadingQueue.Dequeue();
                        AssetLoadRequest loadRequest;
                        if (_loadRequestDic.TryGetValue(bundleName, out loadRequest))
                        {
                            _processingRequests.Add(loadRequest);
#if UNITY_EDITOR
                            if (ResLoadMode == LoadMode.EditorLocal)
                            {
                                StartCoroutine(LoadAssetFromProjectAsync(loadRequest));
                            }
                            else
                            {
                                StartCoroutine(LoadAssetAsyncFromAssetBundle(loadRequest));
                            }
#else
                            StartCoroutine(LoadAssetAsyncFromAssetBundle(loadRequest));
#endif
                        }
                    }
                }
            }
            this.processingActive = false;
            yield return null;
        }

#if UNITY_EDITOR
        /// <summary>
        /// 编辑器下模拟异步加载资源，实际上还是同步加载,通过WaitForSeconds来模拟手机上异步加载情况
        /// </summary>
        /// <param name="loadRequest"></param>
        /// <returns></returns>
        private IEnumerator LoadAssetFromProjectAsync(AssetLoadRequest loadRequest)
        {
            int delay = UnityEngine.Random.Range(0, 7);
            while (delay >= 0)
            {
                yield return null;
                delay--;
            }
            if (EditorLoadDelay != 0f)
                yield return new WaitForSecondsRealtime(EditorLoadDelay);

            //编辑器模式下无需加载依赖资源
            if (loadRequest.isValid)
            {
                foreach (var handler in loadRequest.Handlers)
                {
                    if (handler.disposed)
                        continue;

                    var asset = LoadAssetFromProject(loadRequest.bundleName, handler.assetName, handler.type);
                    if (asset == null)
                    {
                        string content = string.Format("Load <{0}> Asset is null,assetName = {1}", loadRequest.bundleName, handler.assetName);
                        Debug.LogError(content); // todo fish:暂时调低警告等级，9.25重新改为Error
#if !USE_JSZ
                        if (_assetErrorPause && assetErrorPauseStrings.Any(content.Contains))
                            EditorApplication.isPaused = true;
#endif
                    }
                    handler.Excute(asset);
                }
            }
            else
            {
                //请求已失效，所有的AssetHandler都disposed了,跳过加载该资源
#if GAMERES_LOG
                Debug.LogError("AssetLoadRequest请求已失效: " + loadRequest.bundleName);
#endif
            }

            //处理完毕,从处理列表中移除
            _processingRequests.Remove(loadRequest);
            loadRequest.Handlers.Clear();
            _loadRequestDic.Remove(loadRequest.bundleName);
        }
#endif

        /// <summary>
        /// 1.整包资源->加载Bundle->加载Asset->触发回调
        /// 2.小包资源->开始静默下载->存在替代资源->加载替代资源->触发回调->等待资源下载完毕->跳转到整包资源加载流程
        ///                    └>不存在替代资源->跳过加载步骤─────────────────┘
        /// </summary>
        /// <param name="loadRequest"></param>
        /// <returns></returns>
        private IEnumerator LoadAssetAsyncFromAssetBundle(AssetLoadRequest loadRequest)
        {
            //请求已失效，所有的AssetHandler都disposed了,跳过加载该资源
            if (!loadRequest.isValid)
            {
#if GAMERES_LOG
                Debug.LogError("AssetLoadRequest请求已失效: " + loadRequest.bundleName);
#endif
                _processingRequests.Remove(loadRequest);
                _loadRequestDic.Remove(loadRequest.bundleName);
                yield break;
            }

            var resInfo = _curResConfig.GetResInfo(loadRequest.bundleName);
            if (resInfo != null)
            {
                AssetBundleInfo abInfo;
                if (!_abInfoDic.TryGetValue(loadRequest.bundleName, out abInfo))
                {
                    string packageBundleRoot = GameResPath.packageBundleRoot;
                    string bundleRoot = GameResPath.bundleRoot;
                    AssetBundleCreateRequest mainRequest;
                    //有可能出现主AB也是其他AB的依赖的情况，所以主AB也要加入CreateDic中保证不会重复加载
                    //同时主AB有可能已经被其他加载中的AB依赖，所以要看是否已经发起请求
                    if (!_createBundleRequestDic.TryGetValue(loadRequest.bundleName, out mainRequest))
                    {
                        mainRequest = AssetBundle.LoadFromFileAsync(resInfo.isPackageRes
                            ? resInfo.GetABPath(packageBundleRoot)
                            : resInfo.GetABPath(bundleRoot));
                        _createBundleRequestDic.Add(loadRequest.bundleName, mainRequest);
                    }

                    if (resInfo.Dependencies.Count > 0)
                    {
                        //获取该Bundle的所有依赖资源
                        var allDependencies = _curResConfig.GetAllDependencies(loadRequest.bundleName);
                        foreach (string refBundleName in allDependencies)
                        {
                            //如果该依赖资源已加载,直接跳过,否则创建所有依赖资源Bundle加载请求
                            if (!_abInfoDic.ContainsKey(refBundleName))
                            {
                                var refResInfo = _curResConfig.GetResInfo(refBundleName);
                                if (refResInfo != null)
                                {
                                    if (!_createBundleRequestDic.ContainsKey(refBundleName))
                                    {
                                        _createBundleRequestDic.Add(refBundleName,
                                            AssetBundle.LoadFromFileAsync(refResInfo.isPackageRes
                                                ? refResInfo.GetABPath(packageBundleRoot)
                                                : refResInfo.GetABPath(bundleRoot)));
                                    }
                                    else
                                    {
                                        //Debug.Log ("已存在AssetBundleCreateRequest,无需重复创建:" + refBundleName);
                                    }
                                }
                                else
                                {
                                    Debug.LogError("refResInfo is null: " + refBundleName);
                                }
                            }
                        }

                        //等待所有Bundle创建请求加载完毕
                        foreach (string refBundleName in allDependencies)
                        {
                            if (!_abInfoDic.ContainsKey(refBundleName))
                            {
                                AssetBundleCreateRequest request;
                                if (_createBundleRequestDic.TryGetValue(refBundleName, out request))
                                {
                                    //等待当前依赖资源Bundle加载完成
                                    while (!request.isDone)
                                    {
                                        yield return null;
                                    }
                                    AssetBundleInfo refBundleInfo;
                                    var refBundle = request.assetBundle;
                                    if (refBundle != null)
                                    {
                                        //可能同时会有多个请求都加载了同一Bundle,保证不重复添加相同AssetBundleInfo
                                        if (!_abInfoDic.TryGetValue(refBundleName, out refBundleInfo))
                                        {
                                            refBundleInfo = CreateABInfo(refBundleName, refBundle);
                                        }

                                        //依赖资源增加其引用计数
                                        refBundleInfo.AddRef(resInfo.bundleName);
                                    }
                                    else
                                    {
                                        Debug.LogError("加载AssetBundle失败: " + refBundleName);
                                    }
                                    _createBundleRequestDic.Remove(refBundleName);
                                }
                            }
                            else
                            {
                                _abInfoDic[refBundleName].AddRef(resInfo.bundleName);
                            }
                        }
                    }

                    //无依赖资源,直接等待Bundle
                    while (!mainRequest.isDone)
                    {
                        yield return mainRequest;
                    }
                    var assetBundle = mainRequest.assetBundle;
                    if (assetBundle != null)
                    {
                        //可能同时会有多个请求都加载了同一Bundle,保证不重复添加相同AssetBundleInfo
                        if (!_abInfoDic.TryGetValue(resInfo.bundleName, out abInfo))
                        {
                            abInfo = CreateABInfo(resInfo.bundleName, assetBundle);
                        }
                    }
                    else
                    {
                        Debug.LogError("加载AssetBundle失败: " + resInfo.bundleName);
                    }
                    //不管加载成功与否，都要将主AB从Dic中remove
                    _createBundleRequestDic.Remove(resInfo.bundleName);
                }

                if (abInfo != null)
                {
                    //相关Bundle加载完毕,异步加载Bundle内的所有资源,如果已经加载过直接跳过
                    if (abInfo.onlyAsset == null && abInfo.assetList == null)
                    {
                        yield return StartCoroutine(abInfo.CacheAllAssetAsync());
                    }

                    //根据disposed标记决定是否触发回调
                    foreach (var handler in loadRequest.Handlers)
                    {
                        if (handler.disposed)
                            continue;

                        if (abInfo.onlyAsset != null)
                        {
                            handler.Excute(abInfo.onlyAsset);
                        }
                        else if (abInfo.assetList != null)
                        {
                            Object asset = abInfo.FindAsset(handler.assetName, handler.type);
                            if (asset != null)
                            {
                                handler.Excute(asset);
                            }
                            else
                            {
                                handler.OnError();
                            }
                        }
                        else
                        {
                            handler.OnError();
                        }

#if GAMERES_LOG
                        Debug.LogError(handler.ToString());
#endif
                    }
                }
            }
            else
            {
                Debug.LogError(string.Format("加载失败，没有<{0}>资源的信息", loadRequest.bundleName));
                foreach (var handler in loadRequest.Handlers)
                {
                    if (handler.disposed)
                        continue;
                    handler.OnError();
                }
            }

            //处理完毕,从处理列表中移除
            _processingRequests.Remove(loadRequest);
            _loadRequestDic.Remove(loadRequest.bundleName);
        }

        #endregion

        #region 场景Bundle加载接口

        public event Action OnBeginLoadScene;

        public void LoadLevelAsync(string sceneName, bool addtive, Action onFinish, Action<float> onUpdateProgress = null, OnLoadError onError = null)
        {
            if (OnBeginLoadScene != null)
                OnBeginLoadScene();

            if (ResLoadMode == LoadMode.EditorLocal)
            {
                StartCoroutine(LoadLevelCoroutine(sceneName, addtive, onFinish, onUpdateProgress));
            }
            else
            {
                StartCoroutine(LoadSceneBundle(sceneName, addtive, onFinish, onUpdateProgress, onError));
            }
        }

        private IEnumerator LoadSceneBundle(string sceneName, bool addtive, Action onFinish, Action<float> onUpdateProgress, OnLoadError onError)
        {
            string bundleName = GetBundleName(sceneName, ResGroup.Scene);
            var resInfo = _curResConfig.GetResInfo(bundleName);
            if (resInfo == null)
            {
                Debug.LogError(string.Format("加载失败，没有<{0}>资源的信息", bundleName));
                if (onError != null)
                    onError();
                yield break;
            }

            //该场景未下载,先下载场景资源
            if (ValidateMiniRes(bundleName))
            {
                StartCoroutine(DownloadMiniResTask(GetCDNBundleRoot(), bundleName));
                //等待该资源下载完毕
                while (_miniResDownloadingSet.Contains(bundleName))
                {
                    yield return null;
                }
            }

            string packageBundleRoot = GameResPath.packageBundleRoot;
            string bundleRoot = GameResPath.bundleRoot;
            //切换场景时，修改异步加载优先级加快加载速度。
            var oldPriority = Application.backgroundLoadingPriority;
            Application.backgroundLoadingPriority = ThreadPriority.High;

            var createRequest = AssetBundle.LoadFromFileAsync(resInfo.isPackageRes ? resInfo.GetABPath(packageBundleRoot) : resInfo.GetABPath(bundleRoot));
            createRequest.priority = 10;
            yield return createRequest;

            var sceneBundle = createRequest.assetBundle;
            if (sceneBundle == null)
            {
                Debug.LogError(string.Format("Load <{0}> AssetBundle is null", bundleName));
                if (onError != null)
                    onError();
                Application.backgroundLoadingPriority = oldPriority;
                yield break;
            }

            yield return StartCoroutine(LoadLevelCoroutine(sceneName, addtive, onFinish, onUpdateProgress));
            //场景加载完毕，Unload相关AssetBundle
            sceneBundle.Unload(false);

            Application.backgroundLoadingPriority = oldPriority;
        }

        private IEnumerator LoadLevelCoroutine(string sceneName, bool addtive, Action onFinish, Action<float> onUpdateProgress)
        {
            AsyncOperation asyncOp;
            if (addtive)
            {
                asyncOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            }
            else
            {
                asyncOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            }

            while (!asyncOp.isDone)
            {
                if (onUpdateProgress != null)
                    onUpdateProgress(asyncOp.progress);
                yield return null;
            }

            Debug.Log(string.Format("Load Scene:{0} Finish", sceneName));
            if (onFinish != null)
                onFinish();
        }

        #endregion

        #endregion

        #region 资源释放接口
        public void UnLoadSceneResource(string assetName)
        {
            SceneManager.UnloadScene(assetName);
        }

        public bool UnloadBundle(string assetName, ResGroup resGroup, bool unloadAll = false)
        {
            return UnloadBundle(GetBundleName(assetName, resGroup), unloadAll);
        }

        /// <summary>
        ///     释放相关资源的AssetBundle
        /// </summary>
        /// <param name="bundleName"></param>
        /// <param name="unloadAll"></param>
        public bool UnloadBundle(string bundleName, bool unloadAll = false)
        {
            if (_curResConfig == null)
                return false;
            if (string.IsNullOrEmpty(bundleName))
                return false;

            var resInfo = _curResConfig.GetResInfo(bundleName);
            if (resInfo == null)
            {
                Debug.LogError(string.Format("加载失败，没有<{0}>资源的信息", bundleName));
                return false;
            }

            AssetBundleInfo abInfo;
            if (_abInfoDic.TryGetValue(bundleName, out abInfo))
            {
                if (abInfo.Unload(unloadAll))
                {
                    _abInfoDic.Remove(bundleName);
                    if (resInfo.Dependencies.Count > 0)
                    {
                        var allDependencies = _curResConfig.GetAllDependencies(bundleName);
                        foreach (string refBundleName in allDependencies)
                        {
                            AssetBundleInfo refBundleInfo;
                            if (_abInfoDic.TryGetValue(refBundleName, out refBundleInfo))
                            {
                                refBundleInfo.RemoveRef(bundleName);
                            }
                            else
                            {
                                Debug.LogError(string.Format("不存在该资源<{0}> 依赖的资源<{1}>,请检查加载流程是否有误!", bundleName, refBundleName));
                            }
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        public void UnloadDependencies(string assetName, ResGroup resGroup, bool unloadAll = false, bool refBundleUnloadAll = false)
        {
            UnloadDependencies(GetBundleName(assetName, resGroup), unloadAll, refBundleUnloadAll);
        }

        /// <summary>
        /// 尝试卸载该BundleName相关的所有Bundle,包括它本身及其依赖的Bundle
        /// </summary>
        /// <param name="bundleName"></param>
        /// <param name="unloadAll"></param>
        public void UnloadDependencies(string bundleName, bool unloadAll = false, bool refBundleUnloadAll = false)
        {
            if (_curResConfig == null)
                return;
            if (string.IsNullOrEmpty(bundleName))
                return;

            var resInfo = _curResConfig.GetResInfo(bundleName);
            if (resInfo == null)
            {
                Debug.LogError(string.Format("加载失败，没有<{0}>资源的信息", bundleName));
                return;
            }

            UnloadBundle(bundleName, unloadAll);
            if (resInfo.Dependencies.Count > 0)
            {
                var allDependencies = _curResConfig.GetAllDependencies(bundleName);
                foreach (string refBundleName in allDependencies)
                {
                    ResGroup resGroup = GetResGroup(refBundleName);
                    //图集和字体用引用计数管理
                    if (resGroup != ResGroup.UIAtlas && resGroup != ResGroup.UIFont)
                        UnloadBundle(refBundleName, refBundleUnloadAll);
                }
            }
        }
        public void AddUnloadBundleToQueue(string bundleName)
        {
            _unloadBundleManager.AddBundle(bundleName);
        }

        public void RemoveUnloadBundleToQueue(string bundleName)
        {
            _unloadBundleManager.RemoveBundle(bundleName);
        }
        private class UnloadBundleManager
        {
            private readonly HashSet<string> unloadBundleSet;
            private readonly AssetManager master;
            private Coroutine runCoroutine;
            public UnloadBundleManager(AssetManager master)
            {
                this.master = master;
                unloadBundleSet = new HashSet<string>();
            }

            public void AddBundle(string bundleName)
            {
                if (unloadBundleSet.Contains(bundleName))
                    return;

                unloadBundleSet.Add(bundleName);

                if (runCoroutine == null)
                {
                    runCoroutine = master.StartCoroutine(RemoveCoroutine());
                }
            }

            public void RemoveBundle(string bundleName)
            {
                unloadBundleSet.Remove(bundleName);
            }
            private IEnumerator RemoveCoroutine()
            {
                while (unloadBundleSet.Count > 0)
                {
                    yield return null;
                    //加载中Unload 会导致Unity BUG
                    if (master._processingRequests.Count > 0)
                        continue;
                    var tor = unloadBundleSet.GetEnumerator();
                    if (tor.MoveNext())
                    {
                        string bundleName = tor.Current;
                        unloadBundleSet.Remove(bundleName);
                        AssetManager.Instance.UnloadDependencies(bundleName);
                    }
                }
                runCoroutine = null;
            }

            public void Dispose()
            {
                if (runCoroutine != null)
                {
                    master.StopCoroutine(runCoroutine);
                    runCoroutine = null;
                }
            }
        }
        #endregion

        public void Dispose()
        {
#if UNITY_EDITOR
            if (_minResDownloadInfo != null)
            {
                string logPath = Path.Combine(GameResPath.EXPORT_FOLDER, "minResLog.txt");
                FileHelper.WriteAllText(logPath, _minResDownloadInfo.ToString());
            }
#endif

            if (ResLoadMode != LoadMode.EditorLocal)
            {
                if (_cleanUpResFlag)
                {
                    CleanUpResFolder();
                }
                else
                {
                    SaveResConfig();
                }
            }

            //清空异步加载信息
            this.StopAllCoroutines();
            if (_asyncLoadingQueue != null)
                _asyncLoadingQueue.Clear();
            if (_loadRequestDic != null)
                _loadRequestDic.Clear();
            if (_unloadBundleManager != null)
                _unloadBundleManager.Dispose();
            if (_atlasManager != null)
                _atlasManager.Dispose();
            _curResConfig = null;
        }

        #region 清空包外更新资源

        /// <summary>
        ///     设置清空包外游戏资源标记，在退出游戏时做清空处理
        /// </summary>
        public void MarkCleanUpResFlag()
        {
            _cleanUpResFlag = true;
        }

        /// <summary>
        ///     清空persistentDataPath下的所有资源目录，以及Cache目录下的资源
        /// </summary>
        private void CleanUpResFolder()
        {
            _cleanUpResFlag = false;

            CleanUpDllFolder();
            CleanUpBundleResFolder();
        }

        private void CleanUpBundleResFolder()
        {
            if (ResLoadMode == LoadMode.EditorLocal)
                return;

            try
            {
                //清空包外VersionConfig
                string verConfigPath = Path.Combine(GameResPath.persistentDataPath, GameResPath.VERSIONCONFIG_FILE);
                if (File.Exists(verConfigPath))
                {
                    File.Delete(verConfigPath);
                    Debug.Log("GameResource:Remove VersionConfig File: " + verConfigPath);
                }

                //清空包外ResConfig
                string resConfigPath = Path.Combine(GameResPath.persistentDataPath, GameResPath.RESCONFIG_FILE);
                if (File.Exists(resConfigPath))
                {
                    File.Delete(resConfigPath);
                    Debug.Log("GameResource:Remove ResConfig File: " + resConfigPath);
                }

                //清空包外miniResConfig
                string miniResConfigPath = Path.Combine(GameResPath.persistentDataPath, GameResPath.MINIRESCONFIG_FILE);
                if (FileHelper.IsExist(miniResConfigPath))
                {
                    File.Delete(miniResConfigPath);
                    Debug.Log("GameResource:Remove MiniResConfig File: " + miniResConfigPath);
                }

                //清空包外资源Bundle目录
                string gameResDir = GameResPath.bundleRoot;
                if (Directory.Exists(gameResDir))
                {
                    Directory.Delete(gameResDir, true);
                    Debug.Log("GameResource:Remove GameRes Folder: " + gameResDir);
                }

                //Debug.Log("GameResource:Clean Cache " + Caching.CleanCache());
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }


        private void CleanUpDllFolder()
        {
            if (ResLoadMode == LoadMode.EditorLocal)
                return;

            try
            {

                //清空包外dllVersion
                string dllVersionPath = Path.Combine(GameResPath.dllRoot, GameResPath.DLLVERSION_FILE);
                if (FileHelper.IsExist(dllVersionPath))
                {
                    File.Delete(dllVersionPath);
                    Debug.Log("GameResource:Remove DllVersion File: " + dllVersionPath);
                }

                //清空包外备份dll更新目录
                var dllBackupRootDir = GameResPath.dllBackupRoot;
                if (Directory.Exists(dllBackupRootDir))
                {
                    Directory.Delete(dllBackupRootDir, true);
                    Debug.Log("GameResource:Remove DllBackupRoot Folder: " + dllBackupRootDir);
                }

                //清空包外dll更新目录
                string dllRootDir = GameResPath.dllRoot;
                if (Directory.Exists(dllRootDir))
                {
                    Directory.Delete(dllRootDir, true);
                    Debug.Log("GameResource:Remove DllRoot Folder: " + dllRootDir);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        #endregion

        #region Helper Func

        ///// <summary>
        ///// 判断是否使用包内资源
        ///// </summary>
        ///// <param name="bundleName"></param>
        ///// <returns></returns>
        //private bool UsePackageRes(string bundleName)
        //{
        //    if (_packageResConfig == null || _curResConfig == null)
        //        return false;

        //    var packageResInfo = _packageResConfig.GetResInfo(bundleName);
        //    if (packageResInfo != null)
        //    {
        //        var curResInfo = _curResConfig.GetResInfo(bundleName);
        //        //这种情况是玩家安装了最新版本的游戏包,且没有发生过资源版本更新.
        //        if (packageResInfo.CRC == curResInfo.CRC)
        //        {
        //            return curResInfo.isPackageRes;
        //        }
        //        else
        //        {
        //            if (_curResConfig.Version > _packageResConfig.Version)
        //            {
        //                //这种情况下是玩家安装了旧版本游戏包,并且通过版本更新升级到最新版本
        //                //所以包外资源打包时间比包内的新
        //                return false;
        //            }
        //            else
        //            {
        //                //这种情况最为特殊,即包内资源比包外新,只会发生在玩家安装了旧版本游戏包,进入过游戏,然后AFK了一段时间,
        //                //期间发生过版本更新,然后玩家没有通过进入游戏走游戏内的版本更新流程,而是直接安装了最新版本游戏包,进行覆盖安装时就会出现这种情况
        //                return true;
        //            }
        //        }
        //    }

        //    return false;
        //}

        private void SaveResConfig(bool saveServerType = false)
        {
            if (_curResConfig == null)
                return;

            if (saveServerType && _versionConfig != null)
            {
                _curResConfig.serverType = _versionConfig.serverType;
            }

            string savePath = Path.Combine(GameResPath.persistentDataPath, GameResPath.RESCONFIG_FILE);
            _curResConfig.SaveFile(savePath, true);

        }

        private void SaveDllVersion(bool saveServerType = false)
        {
            if (_dllVersion == null)
                return;

            if (saveServerType && _versionConfig != null)
            {
                _dllVersion.serverType = _versionConfig.serverType;
            }

            string savePath = Path.Combine(GameResPath.dllRoot, GameResPath.DLLVERSION_FILE);
            FileHelper.SaveJsonObj(_dllVersion, savePath);
        }

        private void SaveMiniResConfig()
        {
            if (_miniResConfig == null)
                return;
            string savePath = Path.Combine(GameResPath.persistentDataPath, GameResPath.MINIRESCONFIG_FILE);
            FileHelper.SaveJsonObj(_miniResConfig, savePath, true);
        }


        /// <summary>
        /// 保存ServerType，保证哪怕没更新完，依旧使用的是上次选择的服务器类型
        /// </summary>
        private void SaveLocalVersionConfig()
        {
            if (_localVersionConfig == null)
            {
                _localVersionConfig = _versionConfig;
            }
            else
            {
                _localVersionConfig.serverType = _versionConfig.serverType;
            }

            var savePath = Path.Combine(GameResPath.persistentDataPath, GameResPath.VERSIONCONFIG_FILE);
            FileHelper.SaveJsonObj(_localVersionConfig, savePath);
        }

        private void SaveVersionConfig()
        {
            if (_versionConfig == null)
                return;

            string savePath = Path.Combine(GameResPath.persistentDataPath, GameResPath.VERSIONCONFIG_FILE);
            FileHelper.SaveJsonObj(_versionConfig, savePath);
        }

        /// <summary>
        /// 读取本地最近一次缓存的VersionConfig信息
        /// </summary>
        /// <returns></returns>
        private VersionConfig ReadLocalVersionConfig()
        {
            string savePath = Path.Combine(GameResPath.persistentDataPath, GameResPath.VERSIONCONFIG_FILE);
            if (FileHelper.IsExist(savePath))
            {
                return FileHelper.ReadJsonFile<VersionConfig>(savePath);
            }
            return null;
        }

        /// <summary>
        /// 判断当前运行时平台是否支持更新Dll
        /// </summary>
        /// <returns></returns>
        public static bool IsSupportUpdateDllPlatform()
        {
            var runtimePlatform = Application.platform;
            return runtimePlatform == RuntimePlatform.Android || runtimePlatform == RuntimePlatform.WindowsPlayer;
        }

        private bool ValidateStorageSpace(long needSize)
        {
            if (Application.isMobilePlatform)
            {
                long freeSpace = BaoyugameSdk.getExternalStorageAvailable() * 1024L;
                if (freeSpace < needSize)
                {
                    if (_loadErrorHandler != null)
                    {
                        _loadErrorHandler(string.Format("当前剩余手机空间不足，需要{0}，请清理手机空间再尝试。", FormatBytes(needSize - freeSpace)));
                    }
                    return false;
                }
            }

            return true;
        }

        public bool ContainBundleName(string assetName, ResGroup resGroup)
        {
            return ContainBundleName(GetBundleName(assetName, resGroup));
        }

        /// <summary>
        /// 检查是否存在指定BundleName
        /// </summary>
        /// <param name="bundleName"></param>
        /// <returns></returns>
        public bool ContainBundleName(string bundleName)
        {
#if UNITY_EDITOR
            //编辑器本地模式下直接跳过判断
            if (ResLoadMode == LoadMode.EditorLocal)
            {
                string[] paths = AssetDatabase.GetAssetPathsFromAssetBundle(bundleName);
                return paths != null && paths.Length > 0;
            }
#endif

            if (_curResConfig == null)
                return false;

            if (_curResConfig.Manifest.ContainsKey(bundleName))
                return true;

            return false;
        }

        public AssetBundleInfo CreateABInfo(string bundleName, AssetBundle ab)
        {
            AssetBundleInfo assetBundleInfo = new AssetBundleInfo(bundleName, ab);
            _abInfoDic.Add(bundleName, assetBundleInfo);
            if (bundleName.StartsWith(resGrounpMap[(int)ResGroup.UIAtlas]))
            {
                _atlasManager.AddAtlas(assetBundleInfo);
            }
            return assetBundleInfo;
        }


        public static long GetVisualDateTimeNow()
        {
            return Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
        }
        #endregion

        #region Debug Func

        private bool _fatalError;

        private void ThrowFatalException(string msg)
        {
            _fatalError = true;
            if (_loadErrorHandler != null)
            {
                _loadErrorHandler(msg);
            }
            else
            {
                PrintInfo(msg);
            }
            throw new Exception(msg);
        }

        private void PrintInfo(string msg)
        {
            Debug.Log(msg);
            if (_logMessageHandler != null)
                _logMessageHandler(msg);
        }

        public static void LogMemory()
        {
            if (Application.isMobilePlatform)
            {
                long freeMemory = BaoyugameSdk.getFreeMemory() / 1024;
                long totalMemory = BaoyugameSdk.getTotalMemory() / 1024;

                Debug.Log("memory " + freeMemory + "/" + totalMemory);
            }
        }

        public static string FormatBytes(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            int order = 0;
            double len = bytes;
            while (len >= 1024 && order + 1 < sizes.Length)
            {
                order++;
                len = len / 1024;
            }

            // Adjust the format string to your preferences. For example "{0:0.#}{1}" would
            // show a single decimal place, and no space.
            return string.Format("{0:0.##} {1}", len, sizes[order]);
        }

        #endregion

        #region AtlasManager

        public static void AddAtlasRef(UIAtlas uiAtlas, MonoBehaviour monoBehaviour)
        {
            //防止AssetManager还未创建时，被提前创建
            if (_instance != null && _instance._atlasManager != null)
            {
                Instance._atlasManager.AddAtlasRef(uiAtlas, monoBehaviour);
            }
        }

        public static void RemoveAtlasRef(UIAtlas uiAtlas, MonoBehaviour monoBehaviour)
        {
            if (_instance != null && _instance._atlasManager != null)
            {
                Instance._atlasManager.RemoveAtlasRef(uiAtlas, monoBehaviour);
            }
        }

        #endregion


    }
}
