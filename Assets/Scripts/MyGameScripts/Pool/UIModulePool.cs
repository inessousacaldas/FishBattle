using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AssetPipeline;

/// <summary>
/// 对窗口进行缓存
/// </summary>
public class UIModulePool : MonoBehaviour
{
    public const string CheckTaskName = "CheckModuleTime";
    public const int CheckFrequence = 1;
    public const int Capacity = 5;

    private static UIModulePool _instance;

    /// <summary>
    ///     定时释放的
    /// </summary>
    private Dictionary<string, ModuleInstance> _moduleDict;

    /// <summary>
    ///     不会释放的
    /// </summary>
    private Dictionary<string, GameObject> _neverDestroyModuleDict;

    private Transform _root;

    /// <summary>
    ///     检查的时候用于缓存记录的列表，减少重复创建
    /// </summary>
    private List<string> _tempCheckList;

    public static UIModulePool Instance
    {
        get
        {
            CreateInstance();
            return _instance;
        }
    }

    private static void CreateInstance()
    {
        if (_instance == null)
        {
            var go = new GameObject("_UIModulePool");
            DontDestroyOnLoad(go);
            _instance = go.AddComponent<UIModulePool>();
            _instance.Init();
        }
    }

    private void Init()
    {
        _root = transform;

        _tempCheckList = new List<string>();
        _moduleDict = new Dictionary<string, ModuleInstance>();
        _neverDestroyModuleDict = new Dictionary<string, GameObject>();
        SetupTimer();
    }

    public void SetupTimer()
    {
        JSTimer.Instance.SetupTimer(CheckTaskName, CheckModuleTime, CheckFrequence);
    }

    public void Dispose()
    {
        JSTimer.Instance.CancelCd(CheckTaskName);

        Clear();
        if (_root != null)
        {
            Destroy(_root.gameObject);
            _instance = null;
        }
    }

    public void Clear()
    {
        foreach (var moduleInstance in _moduleDict)
        {
            DestroyModule(moduleInstance.Value.Module);
        }
        _moduleDict.Clear();

        foreach (var go in _neverDestroyModuleDict)
        {
            DestroyModule(go.Value);
        }
        _neverDestroyModuleDict.Clear();
    }

    public GameObject OpenModule(string moduleName)
    {
        CheckSharePrefabModule(moduleName);

        if (_moduleDict.ContainsKey(moduleName))
        {
            var module = _moduleDict[moduleName];
            _moduleDict.Remove(moduleName);
            module.Module.SetActive(true);

            return module.Module;
        }
        if (_neverDestroyModuleDict.ContainsKey(moduleName))
        {
            var module = _neverDestroyModuleDict[moduleName];
            _neverDestroyModuleDict.Remove(moduleName);
            module.SetActive(true);

            return module;
        }
        return null;
    }


    public bool HasModuleInPool(string moduleName)
    {
        return _moduleDict.ContainsKey(moduleName) || _neverDestroyModuleDict.ContainsKey(moduleName);
    }


    public void CloseModule(string moduleName, GameObject module)
    {
        var time = UIModulePoolDefinition.GetModulePoolTime(moduleName);

        if (!_moduleDict.ContainsKey(moduleName) && !_neverDestroyModuleDict.ContainsKey(moduleName))
        {
            if (time == (int) UIModulePoolDefinition.ModulePoolType.Destroy)
            {
                DestroyModule(module);
            }
            else if (time == (int) UIModulePoolDefinition.ModulePoolType.NeverDestroy)
            {
                module.SetActive(false);
                module.transform.SetParent(_root);
                _neverDestroyModuleDict.Add(moduleName, module);
            }
            else
            {
                GameDebuger.TODO(" if (!SystemSetting.UsePool)");
                if (false)
                {
                    DestroyModule(module);
                    return;
                }

                if (_moduleDict.Count >= Capacity)
                {
                    var minModule =
                        _moduleDict.Select(pair => pair).OrderBy(pair => pair.Value.LeftTime).FirstOrDefault();
                    if (minModule.Value.LeftTime > time)
                    {
                        DestroyModule(module);
                    }
                    else
                    {
                        _moduleDict.Remove(minModule.Key);
                        DestroyModule(minModule.Value.Module);

                        module.SetActive(false);
                        module.transform.SetParent(_root);
                        _moduleDict.Add(moduleName, new ModuleInstance(module, time));
                    }
                }
                else
                {
                    module.SetActive(false);
                    module.transform.SetParent(_root);
                    _moduleDict.Add(moduleName, new ModuleInstance(module, time));
                }
            }
        }
        // 奇葩的状况，多个
        else
        {
            GameDebuger.LogError(string.Format("{0} 情况特殊，需要特殊处理！", moduleName));
            DestroyModule(module);
            //            var moduleInstance = _moduleDict[moduleName];
            //            // 同一个则刷新时间
            //            if (moduleInstance.Module == module)
            //            {
            //                moduleInstance.LeftTime = time;
            //            }
            //            // 新的则删除旧的
            //            else
            //            {
            //                DestroyModule(moduleInstance.Module);
            //                moduleInstance.Module = module;
            //            }
        }
    }

    public GameObject CreateModule(string moduleName)
    {
        return AssetPipeline.ResourcePoolManager.Instance.LoadUI(moduleName) as GameObject;
    }

    public void DestroyModule(GameObject go, bool gc = true)
    {
        if (go != null)
        {
            Destroy(go);
            AssetPipeline.ResourcePoolManager.UnloadAssetsAndGC();
        }
    }

    #region Nested type: ModuleInstance

    private class ModuleInstance
    {
        public readonly GameObject Module;
        public float LeftTime;

        public ModuleInstance(GameObject module, float leftTime)
        {
            Module = module;
            LeftTime = leftTime;
        }
    }

    #endregion

    #region 检查是否需要释放

    private void CheckModuleTime()
    {
        _tempCheckList.Clear();
        _tempCheckList.AddRange(_moduleDict.Keys);

        for (int i = 0; i < _tempCheckList.Count; i++)
        {
            string moduleName = _tempCheckList[i];
            if (_moduleDict.ContainsKey(moduleName))
            {
                var moduleInstance = _moduleDict[moduleName];
                moduleInstance.LeftTime -= CheckFrequence;
                if (moduleInstance.LeftTime <= 0)
                {
                    _moduleDict.Remove(moduleName);
                    DestroyModule(moduleInstance.Module);
                }
            }
        }

        _tempCheckList.Clear();
    }


    /// <summary>
    ///     对于使用同一个prefab的，暂时做特殊处理
    ///     检查到，则移除
    ///     但是不直接释放，还是有一定的增速
    /// </summary>
    /// <param name="moduleName"></param>
    private void CheckSharePrefabModule(string moduleName)
    {
        if (!IsInSharePrefabDict(moduleName))
        {
            return;
        }

        // 不过度设计，假设NeverDestroy里面没有
        if (_moduleDict.ContainsKey(moduleName))
        {
            var ins = _moduleDict[moduleName];
            _moduleDict.Remove(moduleName);
            DestroyModule(ins.Module, false);
        }
    }

    private bool IsInSharePrefabDict(string moduleName)
    {
        return UIModulePoolDefinition.SharePrefabDict.ContainsKey(moduleName);
    }

    #endregion
}