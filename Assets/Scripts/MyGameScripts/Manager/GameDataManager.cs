using System;
using System.Collections.Generic;
using AppDto;
using AssetPipeline;
using UnityEngine;

using AbstractAsynInit = Asyn.AbstractAsynInit;
using IAsynInit = Asyn.IAsynInit;

namespace StaticInit
{
    public partial class StaticInit
    {
        private StaticDispose.StaticDelegateRunner gameDataMgr = new StaticDispose.StaticDelegateRunner(
            ()=> { var mgr = MissionDataMgr.DataMgr; } );
    }
}

public class GameDataManager :AbstractAsynInit
{
    private static readonly GameDataManager _instance = new GameDataManager();

    private Action _cacheDataSuccess;
    private Action<IAsynInit> _onComplete;
    private Queue<string> _cacheDataQueue;
    private Dictionary<string, object> _dtoDataDic;

    private GameDataManager()
    {
    }

    public static GameDataManager Instance
    {
        get { return _instance; }
    }

    public static string Data_Self_PackDto_Backpack
    {
        get { return "Data_PackDto_Backpack_" + ModelManager.Player.GetPlayerId() + "_V" + AppGameVersion.ShortBundleVersion; }
    }

    public static string Data_Self_PackDto_Warehouse
    {
        get { return "Data_PackDto_Warehouse_" + ModelManager.Player.GetPlayerId() + "_V" + AppGameVersion.ShortBundleVersion; }
    }

    public static string Data_Self_PackDto_Wardrobe
    {
        get { return "Data_PackDto_Wardrobe_" + ModelManager.Player.GetPlayerId() + "_V" + AppGameVersion.ShortBundleVersion; }
    }

    public override void StartAsynInit(Action<IAsynInit> onComplete){
        _dtoDataDic = new Dictionary<string, object>();

        _cacheDataQueue = new Queue<string>(3);
        _cacheDataQueue.Enqueue(Data_Self_PackDto_Backpack);
        _cacheDataQueue.Enqueue(Data_Self_PackDto_Warehouse);
        _cacheDataQueue.Enqueue(Data_Self_PackDto_Wardrobe);

        _onComplete = onComplete;

        LoadNextData();
    }

    private void LoadNextData()
    {
        if (_cacheDataQueue.Count > 0)
        {
            FileHelper.ReadFileAsync(GetDataPath(_cacheDataQueue.Dequeue()), file =>
            {
                var obj = JsHelper.ParseProtoObj(file.ByteArray, true);
                _dtoDataDic[file.FilePath] = obj;
                LoadNextData();
            }, (filePath, error) =>
            {
                Debug.LogWarning(error);
                LoadNextData();
            });
        }
        else
        {
            BackpackDataMgr.DataMgr.InitBagAndTemp();
            //                FashionModel.DataMgr.Setup ();
            //                MagicEquipmentModel.DataMgr.Setup();
            GameUtil.SafeRun<IAsynInit>(_onComplete, this);
        }
    }

    private string GetDataPath(string dataType)
    {
        return GameResPath.persistentDataPath + "/gamedata/" + dataType + ".pbz.bytes";
    }

    public T GetDataObj<T>(string dataName)
    {
        string key = GetDataPath(dataName);
        if (_dtoDataDic.ContainsKey(key))
        {
            return (T)_dtoDataDic[key];
        }
        return default(T);
    }

    public void SaveData()
    {
        GameDebuger.Log("GameDataMaanger SaveData");

        if (_dtoDataDic != null)
        {
            if (_dtoDataDic == null) return;
            foreach (var item in _dtoDataDic)
            {
                string filePath = item.Key;
                GameDebuger.Log("GameDataMaanger SaveData path=" + filePath);

                var byteArray = JsHelper.EncodeObjToPbz(item.Value);
                FileHelper.WriteAllBytes(filePath, byteArray);
            }
        }
    }

    public void PushData(string dataName, object obj)
    {
        string key = GetDataPath(dataName);
        if (_dtoDataDic.ContainsKey(key))
        {
            _dtoDataDic[key] = obj;
        }
        else
        {
            _dtoDataDic.Add(key, obj);
        }
    }

    public long GetDataVersion(string dataName)
    {
        string key = GetDataPath(dataName);
        if (_dtoDataDic.ContainsKey(key))
        {
            var obj = _dtoDataDic[key];
            if (dataName == Data_Self_PackDto_Backpack)
            {
                GameDebuger.TODO(@"
                var packDto = obj as BackpackDto;
                if (packDto == null || packDto.packDto == null)
                {
                    return -1;
                }
                return packDto.packDto.version;
                    ");
                return -1;
            }
            if (dataName == Data_Self_PackDto_Warehouse)
            {
                var warehouseDto = obj as BagDto;
                if (warehouseDto == null)
                {
                    return -1;
                }
                GameDebuger.TODO(@"return warehouseDto.version;");
            }
            if (dataName == Data_Self_PackDto_Wardrobe)
            {
                //先去掉
                //var fashionDto = obj as WardrobeDto;
                //fashionDto.packDto.version;
                return -1;
            }
            return -1;
        }
        return -1;
    }

    public void CleanUp()
    {
        _dtoDataDic = null;
        string path = GameResPath.persistentDataPath + "/gamedata/";
        FileHelper.DeleteDirectory(path, true);
    }
}