
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BaseClassNS;
using AssetPipeline;
using AppProtobuf;
using LITJson;
using UnityEditor;
using UnityEngine;
using DataList = AppDto.DataList;
using DataListVersion = AppDto.DataListVersion;

public class GameDataEditorWindow : BaseEditorWindow
{
    //private GameSetting.DomainType _selectedServerEnum;

    private static readonly string[] fileFormatList = new string[]
    {
        "pbz",
        "jsz"
    };
    private int _fileFormatIndex;

    public string FileFormat
    {
        get { return fileFormatList[_fileFormatIndex]; }
    }
    public static readonly string DataFolder = Application.dataPath + "/GameResources/StaticData/";

    private DataList _newDataVersionList;
    private List<Type> _dataTypes = new List<Type>();
    private int _currentIndex;

    private Type CurType
    {
        get
        {
            if (_dataTypes.Count > _currentIndex && _currentIndex >= 0)
            {
                return _dataTypes[_currentIndex];
            }
            return null;
        }
    }

    [MenuItem("Custom/GameData/Window")]
    private static void Open()
    {
        Open<GameDataEditorWindow>();
    }


    protected override void CustomOnGUI()
    {
        Space();
        //_selectedServerEnum = EnumPopup<GameSetting.DomainType>("选择服务器：", _selectedServerEnum);

        _fileFormatIndex = EditorGUILayout.Popup(_fileFormatIndex, fileFormatList);
        Space();
        Button("开始下载", () => EditorHelper.Run(StartDownload, false));
    }


    private Dictionary<string, DomainInfo> _domainHttpDic;

    private void GetGameSettingData()
    {
        _domainHttpDic = FileHelper.ReadJsonFile<Dictionary<string, DomainInfo>>("Assets/Editor/BuildTools/DomainHttpConfig.json");
    }

    private DomainInfo _selectDomainInfo;
    private void StartDownload()
    {
        GetGameSettingData();

//        string domainKey = _selectedServerEnum.ToString();
//        if (_domainHttpDic.ContainsKey(domainKey))
//        {
//            _selectDomainInfo = _domainHttpDic[domainKey];
//        }
//        else
//        {
//            Debug.LogError("当前DomainType没有对应域名信息");
//            return;
//        }
//
//        ResetDataList();
//        _dataTypes = DataCacheMap.serviceList();
//
//        FileHelper.CreateDirectory(DataFolder);
//
//        string suffix = string.Format("*.{0}.bytes", FileFormat);
//        var files = Directory.GetFiles(DataFolder, suffix);
//        foreach (var file in files)
//        {
//            FileHelper.DeleteFile(file);
//        }
//
//        LoadNextData();
    }

    private void LoadNextData()
    {
        _currentIndex++;
        if (_currentIndex < _dataTypes.Count)
        {
            try
            {
                var url = string.Format("{0}/{1}/staticData/{2}/{3}.{4}?ver={5}",
                                         _selectDomainInfo.url,
										 _selectDomainInfo.resdir,
                                            1,
                                            CurType.Name,
                                            FileFormat,
                                            GetDataVersion(CurType));

                if (!EditorUtility.DisplayCancelableProgressBar("获取数据中：", url,
                    1f * _currentIndex / _dataTypes.Count))
                {
                    Download(url);
                }
                else
                {
                    OnLoadDataFinished();
                    throw new Exception("用户取消了操作！");
                }
            }
            catch (Exception e)
            {
                OnLoadDataFinished();
                throw e;
            }
        }
        else
        {
            OnLoadDataFinished();
        }
    }


    private void Download(string url)
    {
        byte[] datas = null;
        try
        {
            datas = EditorHelper.HttpRequest(url);
        }
        catch (Exception e)
        {
            Debug.LogError(url + "\n" + e.Message);
        }
        if (datas != null && datas.Length > 0)
        {
            OnDownloadSuccess(datas);
        }
        else
        {
            OnDownloadSuccess(null);
        }
    }


    private void OnDownloadSuccess(byte[] dataBytes, string error = null)
    {
        if (dataBytes != null && CurType != null)
        {
            var filePath = DataFolder + string.Format("{0}.{1}.bytes", CurType.FullName, FileFormat);

            // 这里取巧了，假设了DataListVersion必定是第一个文件
            // 在Editor里面就不做优化了，降低复杂度
            if (CurType == typeof(DataListVersion))
            {
                if (FileFormat == "pbz")
                {
                    _newDataVersionList = JsHelper.ParseProtoObj(new ByteArray(dataBytes), true) as DataList;
                }
                else
                {
                    _newDataVersionList = ParseJsz<DataList>(new ByteArray(dataBytes), true);
                }
            }
            File.WriteAllBytes(filePath, dataBytes);
            LoadNextData();
        }
        else
        {
            if (_newDataVersionList != null)
            {
                var item = _newDataVersionList.items.FirstOrDefault(o => CurType == o.GetType());
                if (item == null)
                {
                    Debug.LogError("服务器列表不存在：" + CurType);
                    LoadNextData();
                    return;
                }
            }
            error = error ?? string.Format("{0} 下载失败，是否继续？", CurType);
            if (EditorUtility.DisplayDialog("错误：", error,
                "确定", "取消"))
            {
                LoadNextData();
            }
            else
            {
                OnLoadDataFinished();
                throw new Exception("出现错误，用户取消了操作！");
            }
        }
    }

    public static T ParseJsz<T>(ByteArray ba, bool needUnCompress)
    {
        if (needUnCompress)
        {
            ba.UnCompress();
        }

        JsonReader reader = new JsonReader(ba.ToUTF8String());
        reader.TypeHinting = true;
        reader.TypeReader = typeParam =>
        {
            int typeId = Convert.ToInt32(typeParam);
            return ProtobufMap.getClass(typeId);
        };
        return JsonMapper.ToObject<T>(reader);
    }


    private void OnLoadDataFinished()
    {
        EditorUtility.ClearProgressBar();
        ResetDataList();
    }


    private string GetDataVersion(Type dataType)
    {
        if (_newDataVersionList == null)
        {
            return DateTime.Now.Ticks.ToString();
        }

        foreach (DataListVersion version in _newDataVersionList.items)
        {
            if (version.type == dataType.Name)
            {
                return version.ver;
            }
        }
        return DateTime.Now.Ticks.ToString();
    }

    private void ResetDataList()
    {
        _newDataVersionList = null;
        _dataTypes.Clear();
        _currentIndex = -1;
    }
}

