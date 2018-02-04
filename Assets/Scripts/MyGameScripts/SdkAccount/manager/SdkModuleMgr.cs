using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using System.Linq;

//IViewController, ResourcePoolManager处理
public class SdkModuleMgr
{
    public static readonly SdkModuleMgr Instance = new SdkModuleMgr();

    private static int _layer;
    public static int SdkAcountLayer
    {
        get
        {
            if(_layer == 0)
                _layer = SdkLoginMessage.Instance.GetLayer();
            return _layer;
        }
    }
    public static int SdkTopLayer
    {
        get { return SdkAcountLayer + 100; }
    }
    
    public enum ModuleType
    {
        preLogin,
        gameEnter,
    }
    
    public class ModuleCache
    {
        private Dictionary<string, GameObject> _name2Module;
        private List<string> _nameList;
        public int ModuleCount
        {
            get { return _name2Module.Count; }
        }

        public List<string> NameList { get { return _nameList; } }

        public ModuleCache()
        {
            _name2Module = new Dictionary<string, GameObject>();
            _nameList = new List<string>();
        }

        public void Add(string name, GameObject module)
        {
            _name2Module[name] = module;
            if (_nameList.Contains(name))
            {
                _nameList.Remove(name);
            }
            _nameList.Add(name);
        }

        public void Remove(string name)
        {
            if (!_nameList.Contains(name)) return;

            _name2Module.Remove(name);
            _nameList.Remove(name);
        }

        public GameObject GetModule(string name)
        {
            if (_name2Module.ContainsKey(name))
            {
                return _name2Module[name];
            }
            return null;
        }

        public string GetModuleName(GameObject module)
        {
            string moduleName = string.Empty;
            foreach (string sName in _name2Module.Keys)
            {
                if (_name2Module[sName] == module)
                {
                    moduleName = sName;
                    break;
                }
            }
            return moduleName;
        }

        public int GetNextLayer()
        {
            GameObject module = null;
            if(_nameList.Count > 0)
            {
                module = _name2Module[_nameList[_nameList.Count - 1]];
            }

            if(module != null)
            {
                return module.GetComponent<UIPanel>().depth;
            }

            return SdkAcountLayer;
        }
    }

    private ModuleCache _moduleCache;
    private GameObject _singleModule;

    public SdkModuleMgr()
    {
        _moduleCache = new ModuleCache();
    }

    public void InitRoot(GameObject root)
    { 
        if (SdkBaseController.Instance != null) return;
        
        GameObject module = AssetPipeline.ResourcePoolManager.Instance.LoadUI(SdkBaseView.NAME) as GameObject;
        module.SetActive(true);
        module = NGUITools.AddChild(root, module);
        module.AddMissingComponent<UIPanel>();
        NGUITools.AdjustDepth(module, SdkAcountLayer);

        SdkBaseController.Setup(module);
        SdkLoadingTipController.Setup(module);
        SdkNotifyTipController.Setup(module);
    }
    
    public GameObject OpenModule(string name, ModuleType type)
    {
        GameObject module = _moduleCache.GetModule(name);

        if (module != null)
        {
            CloseTopperModule(name);
            return module;
        }

        GameObject parent = (type == ModuleType.preLogin) ? SdkBaseController.PreLoginObject : SdkBaseController.GameCenterObject;
        module = AssetPipeline.ResourcePoolManager.Instance.LoadUI(name) as GameObject;
        module.SetActive(true);
        module = NGUITools.AddChild(parent, module);
        module.AddMissingComponent<UIPanel>();

        int uiLayer = _moduleCache.GetNextLayer();
        NGUITools.AdjustDepth(module, uiLayer);
        
        _moduleCache.Add(name, module);
        if (uiLayer != SdkAcountLayer)
        {
            MoveIn(module, type);
        }
        return module;
    }

    /// <summary>
    /// 关闭比当前模块更上层的界面
    /// </summary>
    /// <param moduleName="moduleName">当前模块名</param>
    public void CloseTopperModule(string moduleName)
    {
        List<string> closeList = new List<string>();
        for(int i =  _moduleCache.NameList.Count-1; i >=0; --i)
        { 
            if (_moduleCache.NameList[i] == moduleName) break;

            closeList.Add(_moduleCache.NameList[i]);
        }
        
        foreach(var closeName in closeList)
        {
            CloseModule(closeName);
        }
    }

    public void MoveIn(GameObject module, ModuleType type)
    {
        module.transform.position = (type == ModuleType.preLogin) ? SdkBaseController.PreLoginHidePos:SdkBaseController.GameCenterHidePos;
        Vector3 des = (type == ModuleType.preLogin) ? SdkBaseController.PreLoginPos : SdkBaseController.GameCenterPos;
        var com = TweenPosition.Begin(module, 0.5f, des, true);
        com.method = UITweener.Method.Linear;
        com.onFinished.Clear();
    }

    public void MoveOut(GameObject module, ModuleType type)
    {
        Vector3 des = (type == ModuleType.preLogin) ? SdkBaseController.PreLoginHidePos : SdkBaseController.GameCenterHidePos; 
        var com = TweenPosition.Begin(module, 0.5f, des, true);
        com.method = UITweener.Method.Linear;
        EventDelegate.Set(com.onFinished,()=> { CloseModule(module); });
    }

    public void CloseModuleSlow(string name, ModuleType type)
    {
        GameObject module = _moduleCache.GetModule(name);
        if (module != null)
        {
            if (_moduleCache.ModuleCount == 1)
                CloseModule(module);
            else
                MoveOut(module, type);
        }
    }

    public void CloseModule(string moduleName)
    {
        GameObject module = _moduleCache.GetModule(moduleName);
        if (module != null)
        {
            IViewController viewController = GetViewController(module);
            _moduleCache.Remove(moduleName);

            if (viewController != null)
            {
                viewController.Dispose();
            }

            DestroyModule(module);
        }
    }

    public void CloseModule(GameObject module)
    {
        string moduleName = _moduleCache.GetModuleName(module);

        if (moduleName != string.Empty)
        {
            IViewController viewController = GetViewController(module);
            _moduleCache.Remove(moduleName);

            if (viewController != null)
            {
                viewController.Dispose();
            }

            DestroyModule(module);
        }
    }

    public void ClearModule()
    {
        List<string> list = new List<string>();
        foreach(var name in _moduleCache.NameList)
        {
            list.Add(name);
        }
        
        foreach (var name in list)
        {
            CloseModule(name);
        }
    }

    public void DestroyModule(GameObject go, bool gc = true)
    {
        if (go != null)
        {
            Object.Destroy(go);
            AssetPipeline.ResourcePoolManager.UnloadAssetsAndGC();
        }
    }

    private IViewController GetViewController(GameObject module)
    {
        MonoBehaviour[] list = module.GetComponents<MonoBehaviour>();
        for (int i = 0, len = list.Length; i < len; i++)
        {
            MonoBehaviour mono = list[i];
            if (mono is IViewController)
            {
                return mono as IViewController;
            }
        }
        return null;
    }

    public bool IsModuleOpen(string name)
    {
        GameObject module = _moduleCache.GetModule(name);
        return module != null;
    }
}
