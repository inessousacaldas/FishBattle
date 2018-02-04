
using System;
using System.Collections.Generic;
using AppDto;
using UnityEngine;
using AssetPipeline;

/**
 *数据管理 
 * @author SK
 * 
 */

public class DataManager
{
    private static readonly HashSet<Type> PreLoadTypes = new HashSet<Type>
    {
        typeof(Faction),
        typeof(FunctionOpen),
        typeof(GeneralCharactor),
        typeof(StaticConfig)
    };

    private static DataManager _instance;

    private static bool _allDataLoadFinish;
    private List<Type> _allStaticDataTypes;
    private Type _curUpdateType;
    private List<Type> _finishUpdateList;

    private Action<string> _loadingMsgHandler;
    private Action<float> _loadingProcessHandler;
    private int _needUpdateAllCount;
    private int _needUpdatePreLoadCount;
    private Queue<Type> _needUpdateQueue;

    private DataList _newDataVersionList;
    private ByteArray _newDataVersionBytes;
    private DataList _oldDataVersionList;
    private Action _onAllLoadFinish;
    private Action _onPreLoadFinish;
    private Dictionary<Type, ByteArray> _staticDataByteArrayDic;

    private const string StaticDataVersion = "staticDataVersion";

    private DataManager()
    {
    }

    public static DataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new DataManager();
            }
            return _instance;
        }
    }

    #region 初始化静态数据

    /// <summary>
    ///     预先加载缓存的数据
    /// </summary>
    public void Setup()
    {
        _allStaticDataTypes = DataCacheMap.serviceList();
        //将预加载的静态数据排在最前,这样更新时就先更新预加载的数据
        _allStaticDataTypes.Sort((a, b) =>
        {
            if (a == b)
                return 0;
            if (a == typeof (DataListVersion))
                return -1;
            if (b == typeof (DataListVersion))
                return 1;
            bool existA = PreLoadTypes.Contains(a);
            bool existB = PreLoadTypes.Contains(b);
            if (existA != existB)
            {
                if (existA)
                {
                    return -1;
                }
                return 1;
            }
            return 0;
        });
        _newDataVersionList = null;
        _oldDataVersionList = null;
        string versionPath = GetDataPath(typeof(DataListVersion).FullName);

        // 如果包外不存在DataListVersion信息,导出包内静态数据到包外
        CopyDataFromPackage(versionPath);

        var ba = FileHelper.LoadByteArrayFromFile(versionPath);
        if (ba != null)
        {
            try
            {
                #if USE_JSZ
				_oldDataVersionList = JsHelper.ParseJsz<DataList>(ba, true);
                #else
                _oldDataVersionList = (DataList)JsHelper.ParseProtoObj(ba, true);
                #endif
            }
            catch (Exception e)
            {
                GameDebuger.LogError("JsHelper.ParseProtoObj失败，错误信息：" + e.Message);
            }

            //预加载开场剧情需要的静态数据
            LoadPreLoadStaticData();
        }
        else
        {
            Debug.LogError("Load Local <DataListVersion> failed");
        }
    }

    /// <summary>
    /// 从包内拷贝数据到包外
    /// </summary>
    private void CopyDataFromPackage(string versionPath)
    {
        if (FileHelper.IsExist(versionPath))
        {
            if(AssetManager.Instance.CurResConfig == null)
            {
                GameDebuger.Log("AssetManager.Instance.CurResConfig为空，直接返回");
                return;
            }

            string saveVersion = PlayerPrefs.GetString(StaticDataVersion);
            if (!string.IsNullOrEmpty(saveVersion))
            {
                try
                {
                    GameDebuger.Log("当前表版本：" + AssetManager.Instance.CurResConfig.Version.ToString());
                    GameDebuger.Log("包外表版本：" + saveVersion);
                    if (long.Parse(saveVersion) >= AssetManager.Instance.CurResConfig.Version)
                    {
                        return;
                    }
                }
                catch(Exception e)
                {
                    GameDebuger.LogError("解析外部静态数据版本staticDataVersion错误");
                }
            }
            else
            {
                GameDebuger.Log("不存在外部静态变量staticDataVersion版本，重新解压allstaticdata");
            }
        }

        Debug.LogError("包外找不到DataListVersion数据,可能被恶意删除");
        string fileName = GetFileName(typeof(DataListVersion).FullName);
        var datalistVerAsset = AssetManager.Instance.LoadAsset("config/allstaticdata", fileName) as TextAsset;
        if (datalistVerAsset != null)
        {
            if(AssetManager.Instance.CurResConfig != null)
            {
                PlayerPrefs.SetString(StaticDataVersion, AssetManager.Instance.CurResConfig.Version.ToString());
            }
            else
            {
                GameDebuger.LogError("错误，AssetManager.Instance.CurResConfig为空");
            }
            FileHelper.WriteAllBytes(versionPath, datalistVerAsset);

            foreach (var type in _allStaticDataTypes)
            {
                fileName = GetFileName(type.FullName);
                var textAsset = AssetManager.Instance.LoadAsset("config/allstaticdata", fileName) as TextAsset;
                if (textAsset != null)
                {
                    FileHelper.WriteAllBytes(StaticDataDir + fileName, textAsset);
                }
            }
        }
    }

    /// <summary>
    ///     预加载开场剧情需要的静态数据
    /// </summary>
    private void LoadPreLoadStaticData()
    {
        var caches = new Dictionary<Type, ByteArray>();

        foreach (var type in PreLoadTypes)
        {
            var byteArray = FileHelper.LoadByteArrayFromFile(GetDataPath(type.FullName));
            if (byteArray != null)
            {
                caches.Add(type, byteArray);
            }
        }

        DataCache.Setup(caches);
    }

    public static void CleanUp()
    {
        FileHelper.DeleteDirectory(StaticDataDir, true);
        Reset();
    }

    #endregion

    #region 更新静态数据

    public void UpdateStaticData(Action onPreLoadFinish, Action onAllFinish, Action<string> logHandler,
                                 Action<float> processHandler)
    {
        if (_allDataLoadFinish)
        {
            if (onAllFinish != null)
                onAllFinish();
            return;
        }

        _onPreLoadFinish = onPreLoadFinish;
        _loadingMsgHandler = logHandler;
        _loadingProcessHandler = processHandler;
        //短时间内进行多次UpdateStaticData,之前的下载任务未完成
        //延续上一次的下载任务，_onPreLoadFinish、_loadingMsgHandler、_loadingProcessHandler在下一个表处理完毕后再进行回调。
        if (_onAllLoadFinish != null)
        {
            _onAllLoadFinish = onAllFinish;
            return;
        }
        _onAllLoadFinish = onAllFinish;
        //更新静态数据之前,先清空DataCache
        DataCache.Dispose();

        _oldDataVersionList = null;
        _newDataVersionList = null;

        //载入_oldDataVersionList
        string versionPath = GetDataPath(typeof(DataListVersion).FullName);
        var ba = FileHelper.LoadByteArrayFromFile(versionPath);
        if (ba != null)
        {
            try
            {
                #if USE_JSZ
				_oldDataVersionList = JsHelper.ParseJsz<DataList>(ba, true);
                #else
                _oldDataVersionList = (DataList)JsHelper.ParseProtoObj(ba, true);
                #endif
            }
            catch (Exception e)
            {
                GameDebuger.LogError("JsHelper.ParseProtoObj失败，错误信息：" + e.Message);
            }
        }
        else
        {
            Debug.LogError("Load Local <DataListVersion> failed");
        }

        PrintInfo("获取静态数据版本信息，请稍后...");
        RequestStaticData(typeof(DataListVersion), byteArray =>
            {
                PrintInfo("读取版本信息成功，更新静态数据中...");
                _newDataVersionBytes = byteArray;
#if USE_JSZ
           _newDataVersionList = JsHelper.ParseJsz<DataList>(byteArray, true);
#else
                _newDataVersionList = (DataList)JsHelper.ParseProtoObj(byteArray, true);
#endif
                CheckDataVersion();
            }, exception =>
            {
                Debug.LogError("RequestStaticData: " + exception.Message);
                ShowLoadErrorMsg(typeof(DataListVersion));
                ClearOnDone();
            });
    }

    private void RequestStaticData(Type type, Action<ByteArray> onSuccess, Action<Exception> onError)
    {
        string url = GetStaticDataUrl(type);
        GameDebuger.Log("RequestStaticData " + url);
        HttpController.Instance.DownLoad(url, onSuccess, null, onError, false, SimpleWWW.ConnectionType.Short_Connect);
    }

    private void CheckDataVersion()
    {
        if (_newDataVersionList == null)
        {
            Debug.LogError("CheckDataVersion failed , newDataVersionList is null");
            return;
        }

        //GameStopwatch.Begin("StaticData");
        _needUpdateQueue = new Queue<Type>();
        _needUpdatePreLoadCount = 0;
        _needUpdateAllCount = 0;
        _staticDataByteArrayDic = new Dictionary<Type, ByteArray>(_allStaticDataTypes.Count);
        if (_oldDataVersionList != null)
        {
            for (int i = 0; i < _allStaticDataTypes.Count; i++)
            {
                var type = _allStaticDataTypes[i];
                if (type == typeof(DataListVersion))
                    continue;

                string typeName = type.Name;
                var oldVersionInfo = GetOldDataListVersion(typeName);
                var newVersionInfo = GetNewDataListVersion(typeName);

                if (newVersionInfo != null)
                {
                    if (oldVersionInfo != null && oldVersionInfo.ver == newVersionInfo.ver)
                    {
                        var byteArray = FileHelper.LoadByteArrayFromFile(GetDataPath(type.FullName));

                        if (byteArray != null)
                        {
                            //当前静态数据版本号相同,且文件MD5值与ver值一致,保存其ByteArray
                            string md5 = MD5Hashing.HashBytes(byteArray);
                            //GameDebuger.LogError("md5：" + md5 + "\tver: " + newVersionInfo.ver);
                            if (string.Equals(md5, newVersionInfo.ver, StringComparison.CurrentCultureIgnoreCase))
                            {
                                _staticDataByteArrayDic.Add(type, byteArray);
                            }
                            else
                            {
                                EnqueueUpdateList(type);
                            }
                        }
                        else
                        {
                            EnqueueUpdateList(type);
                        }
                    }
                    else
                    {
                        //本地不存在其版本信息,直接加入更新队列
                        EnqueueUpdateList(type);
                    }
                }
                else
                {
                    GameDebuger.LogError(string.Format("游戏数据({0})不在NewDataListVersion数据表里，如果未使用该配置表，不受影响", typeName));
                }
            }
        }
        else
        {
            //不存在本地静态数据版本信息时,所有静态数据都入列进行更新
            //如果通过系统设置清空了本地缓存会走这个流程
            for (int i = 0; i < _allStaticDataTypes.Count; i++)
            {
                var type = _allStaticDataTypes[i];
                if (type == typeof(DataListVersion))
                    continue;

                EnqueueUpdateList(type);
            }
        }

        _needUpdateAllCount = _needUpdateQueue.Count;
        _finishUpdateList = new List<Type>(_needUpdateQueue.Count);
        GameDebuger.Log("CheckDataVersion needUpdateCount=" + _needUpdateQueue.Count);
        CheckOutUpdateQueue();
    }

    private void EnqueueUpdateList(Type type)
    {
        if (PreLoadTypes.Contains(type))
            _needUpdatePreLoadCount++;

        _needUpdateQueue.Enqueue(type);
    }

    /// <summary>
    ///     检查更新队列状态,按次序下载静态数据文件
    /// </summary>
    private void CheckOutUpdateQueue()
    {
        if (_needUpdateQueue.Count > 0)
        {
            //预加载数据更新完毕,先设置DataCache
            if (_finishUpdateList.Count >= _needUpdatePreLoadCount)
            {
                if (_onPreLoadFinish != null)
                {
                    var preloadBytes = new Dictionary<Type, ByteArray>(PreLoadTypes.Count);
                    foreach (var type in PreLoadTypes)
                    {
                        try
                        {
                            preloadBytes.Add(type, ByteArray.Copy(_staticDataByteArrayDic[type]));
                        }
                        catch (Exception e)
                        {
                            GameDebuger.LogError(string.Format("CheckOutUpdateQueue failed ,type:{1},Exception:{0}", e, type));
                        }

                    }
                    DataCache.Setup(preloadBytes);
                    _onPreLoadFinish();
                    _onPreLoadFinish = null;
                }
            }

            float process = (float)_finishUpdateList.Count / _needUpdateAllCount;
            PrintLoadingPrecent(process);
            DownloadStaticData(_needUpdateQueue.Dequeue());
        }
        else
        {
            OnFinishUpdateStaticData();
        }
    }

    private void DownloadStaticData(Type type)
    {
        _curUpdateType = type;
        string typeName = _curUpdateType.Name;
        RequestStaticData(_curUpdateType, byteArray =>
            {
                GameDebuger.Log("OnDownloadStaticDataSuccess: " + typeName + " " + byteArray.Length);
                _staticDataByteArrayDic[_curUpdateType] = byteArray;
                _finishUpdateList.Add(_curUpdateType);
                CheckOutUpdateQueue();
            }, e =>
            {
                GameDebuger.Log("OnDownloadStaticDataError: " + e.Message);
                if (GameSetting.Release)
                {
                    //发布版本中,提示重登对话
                    ShowLoadErrorMsg(_curUpdateType);
                    ClearOnDone();
                }
                else
                {
                    GameDebuger.LogError(string.Format("游戏数据({0})加载出错，可能会影响使用", typeName));
                    TipManager.AddTip("连接服务器失败");
                    CheckOutUpdateQueue();
                }
            });
    }

    private void ShowLoadErrorMsg(Type type)
    {
        GameDebuger.Log(string.Format("游戏数据({0})加载出错，请重试", type.Name));
        ExitGameScript.OpenReloginTipWindow("连接服务器失败");
    }

    private void OnFinishUpdateStaticData()
    {
        SaveNewStaticData();
        DataCache.Setup(_staticDataByteArrayDic);

        PrintLoadingPrecent(1f);
        //无需更新静态数据时,不需要重复设置DataCache
        if (_onPreLoadFinish != null)
            _onPreLoadFinish();
        if (_onAllLoadFinish != null)
            _onAllLoadFinish();

        ClearOnDone();
        
        _allDataLoadFinish = true;
        
        //GameStopwatch.End("StaticData");
        GameDebuger.Log("DataManager finish update StaticData");
    }

    private void ClearOnDone()
    {
        //清空更新相关的成员变量
        _curUpdateType = null;
        _finishUpdateList = null;
        _needUpdateQueue = null;
        _staticDataByteArrayDic = null;

        _loadingMsgHandler = null;
        _loadingProcessHandler = null;
        _onPreLoadFinish = null;
        _onAllLoadFinish = null;

        _newDataVersionList = null;
        _newDataVersionBytes = null;
        _oldDataVersionList = null;
    }

    private void SaveNewStaticData()
    {
        //只需保存下载更新成功的数据
        if (_finishUpdateList != null)
        {
            for (int i = 0; i < _finishUpdateList.Count; i++)
            {
                var type = _finishUpdateList[i];
                var ba = _staticDataByteArrayDic[type];
                if (ba != null)
                {
                    //无需等待异步完成，因为每次读到内存中会判断MD5是否相同。如果保存有问题，直接重新下载。
                    FileHelper.WriteFileAsync(GetDataPath(type.FullName), ba, null);
                }
            }
        }

        if (_newDataVersionList != null)
        {
            string filePath = GetDataPath(typeof(DataListVersion).FullName);
            _newDataVersionBytes.Compress();
            FileHelper.WriteFileAsync(filePath, _newDataVersionBytes, null);
            _newDataVersionBytes = null;
        }
    }

    #endregion

    #region Helper

    private DataListVersion GetOldDataListVersion(string typeName)
    {
        if (_oldDataVersionList != null)
        {
            for (int i = 0; i < _oldDataVersionList.items.Count; i++)
            {
                DataListVersion version = (DataListVersion)_oldDataVersionList.items[i];
                if (version.type == typeName)
                {
                    return version;
                }
            }
        }
        return null;
    }

    private DataListVersion GetNewDataListVersion(string typeName)
    {
        if (_newDataVersionList != null)
        {
            for (int i = 0; i < _newDataVersionList.items.Count; i++)
            {
                DataListVersion version = (DataListVersion)_newDataVersionList.items[i];
                if (version.type == typeName)
                {
                    return version;
                }
            }
        }
        return null;
    }

    private string GetDataVersion(Type dataType)
    {
        if (_newDataVersionList != null)
        {
            string typeName = dataType.Name;
            for (int i = 0; i < _newDataVersionList.items.Count; i++)
            {
                DataListVersion version = (DataListVersion)_newDataVersionList.items[i];
                if (version.type == typeName)
                {
                    return version.ver;
                }
            }
        }
        return DateTime.Now.Ticks.ToString();
    }

    /// <summary>
    /// 获取静态数据文件名
    /// </summary>
    /// <param name="dataType"></param>
    /// <returns></returns>
    public static string GetFileName(string dataType)
    {
#if USE_JSZ
        return dataType + ".jsz.bytes";
#else
        return dataType + ".pbz.bytes";
#endif
    }

    /// <summary>
    /// 获取静态数据包外资源路径
    /// </summary>
    /// <param name="dataType"></param>
    /// <returns></returns>
    public static string GetDataPath(string dataType)
    {
        string fileName = GetFileName(dataType);
        if (AssetManager.ResLoadMode == AssetManager.LoadMode.EditorLocal)
        {
            return "Assets/GameResources/StaticData/" + fileName;
        }
        else
        {
            return StaticDataDir + fileName;
        }
    }

    private static string StaticDataDir
    {
        get { return GameResPath.persistentDataPath + "/StaticData/"; }
    }

    public static bool AllDataLoadFinish
    {
        get { return _allDataLoadFinish; }
    }

    public static void Reset()
    {
        _allDataLoadFinish = false;
    }

    private string GetStaticDataUrl(Type type)
    {
        string serverRoot = GameSetting.DATA_SERVER;
        //不太清楚为什么DataListVersion 要从CDN获取
        //if (type != typeof(DataListVersion))
        //{
        //    serverRoot = GameSetting.CDN_SERVER;
        //}
#if USE_JSZ
        string url = string.Format(serverRoot + "/staticData/{0}/{1}.jsz?ver={2}",
            ServerManager.Instance.GetServerInfo().destServerId, type.Name, GetDataVersion(type));
        return url;
#else
        string url = string.Format(serverRoot + "/staticData/{0}/{1}.pbz?ver={2}",
                         ServerManager.Instance.GetServerInfo().destServerId, type.Name, GetDataVersion(type));
        return url;
#endif
    }

    private void PrintInfo(string msg)
    {
        if (_loadingMsgHandler != null)
            _loadingMsgHandler(msg);
        else
            GameDebuger.Log(msg);
    }

    private void PrintLoadingPrecent(float basePercent)
    {
        int percent = Mathf.FloorToInt(basePercent * 100);
        PrintInfo(percent + "％");
        if (_loadingProcessHandler != null)
        {
            _loadingProcessHandler(basePercent);
        }
    }

    #endregion
}