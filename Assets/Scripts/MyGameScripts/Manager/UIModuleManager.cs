// **********************************************************************
// Copyright (c) 2013 Baoyugame. All rights reserved.
// File     :  UIModuleManager.cs
// Author   : jiaye.lin
// Created  : 2014/11/26
// Purpose  : 
// **********************************************************************
using UnityEngine;
using System.Collections.Generic;
using AssetPipeline;
using System;
using UniRx;

public class UIModuleManager
{
    private static readonly UIModuleManager instance = new UIModuleManager();
    public static UIModuleManager Instance
    {
        get
        {
            return instance;
        }
    }

    private static readonly Dictionary<string, UILayerType> uiInfoDic = new Dictionary<string, UILayerType> (); //以后改成配置表 todo fish
    private Dictionary<UILayerType, List<Tuple<string, GameObject>>> _moduleCacheDic; //用于缓存当前UI模块的GameObject对象
    private Dictionary<string, int> _layerCacheDic;           //用于缓存不同层当前模块的名称

    public delegate void OnModuleOpen(IViewController vc);
    private Dictionary<string, OnModuleOpen> _openEventDic;
    private Subject<string> openModuleStream;
    //模块的堆栈管理
    private List<string> _moduleOpenSeque = new List<string>();
    public string TopModule
    {
        get
        {
            if(_moduleOpenSeque.Count > 0)
            {
                return _moduleOpenSeque[_moduleOpenSeque.Count - 1];
            }
            return string.Empty;
        }
    }
    public UniRx.IObservable<string> TopModuleEvt
    {
        get { return openModuleStream; }
    }

    private UIModuleManager()
    {
        _moduleCacheDic = new Dictionary<UILayerType, List<Tuple<string, GameObject>>> ();
        _layerCacheDic = new Dictionary<string, int>();
        _openEventDic = new Dictionary<string, OnModuleOpen>();

        _returnModuleList = new List<ModuleRecord>();
        openModuleStream = new Subject<string>();
        openModuleStream.Hold(string.Empty);
    }

    private GameObject GetExistModule(string moduleName){
        GameObject module = null;

        if (IsModuleCacheContainsModule(moduleName))
        {
            module = GetModuleByName (moduleName);
        }
        return module;
    }
    private GameObject CreateModule(string moduleName, bool addBgMask, bool bgMaskClose = true){
        GameObject module = null;

        if (UIModulePool.Instance.HasModuleInPool(moduleName))
        {
            module = UIModulePool.Instance.OpenModule(moduleName);
            GameObjectExt.AddPoolChild(LayerManager.Root.UIModuleRoot, module);
        }
        else
        {
            module = ResourcePoolManager.Instance.LoadUI(moduleName);
            module = NGUITools.AddChild(LayerManager.Root.UIModuleRoot, module);
            if (module != null) {
                module.AddMissingComponent<UIPanel>();
                if (addBgMask)
                {
                    AddBgMask(moduleName, module, bgMaskClose);
                }
            }
        }

        if (module != null) {
            module.SetActive (true);
        }

        return module;
    }

    public T OpenFunModule<T>(
        string moduleName
        , UILayerType layerType
        , bool addBgMask
        , Vector3 worldPos
        , bool bgMaskClose = true
        )
        where T : MonoController{

        var ui = OpenFunModule(moduleName, layerType, addBgMask, bgMaskClose);
        var controller = ui.GetMissingComponent<T>();

        var pos = LayerManager.Root.UIModuleRoot.transform.InverseTransformPoint(worldPos);
        var module = GetModuleByName(moduleName);
        module.transform.localPosition = pos;

        return controller;
    }

    public T OpenFunModule<T>(
        string moduleName
        , UILayerType layerType
        , bool addBgMask
        , bool bgMaskClose = true)
    where T : MonoController
    {
        var validate = moduleName != TopModule && (layerType == UILayerType.BaseModule || layerType == UILayerType.DefaultModule);
        var ui = OpenFunModule(moduleName, layerType, addBgMask, bgMaskClose);
        var controller = ui.GetMissingComponent<T>();
        if (validate)
            FireOpenModuleData(moduleName);
        return controller;
    }

    private void FireOpenModuleData(string moduleName)
    {
        openModuleStream.OnNext(moduleName);
    }

    public GameObject OpenFunModule(string moduleName, UILayerType layerType, bool addBgMask, bool bgMaskClose = true)
    {
        if (string.IsNullOrEmpty (moduleName)) {
            return null;
        }

        uiInfoDic [moduleName] = layerType; 
        if (layerType == UILayerType.BaseModule || layerType == UILayerType.DefaultModule)
        {
            _moduleOpenSeque.Remove(moduleName);
            _moduleOpenSeque.Add(moduleName);
        }
       
        var depth = GetCurDepthByLayerType (layerType);

        OpenModuleEx(moduleName, depth);

        var module = GetExistModule (moduleName);
        if (module == null)
        {
            module = CreateModule(moduleName, addBgMask, bgMaskClose);
            _layerCacheDic[moduleName] = depth;
        }
            
        if (module != null) {
            depth = GetCurDepthByLayerType(layerType);
            module.ResetPanelsDepth(depth);
            AddModuleToCacheDic(moduleName, module);
            module.SetActive (true);
        }

        AdjustLayerDepth(layerType);
        return module;
    }

    private void AdjustLayerDepth(UILayerType layertype){
        List<Tuple<string, GameObject>> list = null;
        _moduleCacheDic.TryGetValue(layertype, out list);

        Comparison<Tuple<string, GameObject>> sort = delegate(Tuple<string, GameObject> x, Tuple<string, GameObject> y)
        {
            return x.p2.GetComponentInChildren<UIPanel>().depth - y.p2.GetComponentInChildren<UIPanel>().depth;
        };
        
        list.Sort(sort);

        var originDepth = LayerManager.Instance.GetOriginDepthByLayerType (layertype);
        list.ForEach(s=>{
            if (s != null && s.p2 != null && s.p2.activeSelf){
                s.p2.ResetPanelsDepth(originDepth);
//                GameDebuger.LogError(string.Format("name {0} depth = {1}", s.p2.name, s.p2.GetComponentInChildren<UIPanel>().depth));
                originDepth = UIHelper.GetMaxDepthWithPanelAndWidget(s.p2) + 1;    
            }
        });
    }

    public void CloseModule(string moduleName, bool withEX = true)
    {
        var module = GetModuleByName(moduleName);
        if (module != null)
        {
            module.SetActive(false);
            var viewController = GetViewController(module);
            RemoveElementFromModuleCache (moduleName);

            if (withEX)
            {
                int depth;
                if (!_layerCacheDic.TryGetValue(moduleName, out depth))
                {
                    var panel = module.GetComponent<UIPanel>();
                    if (panel != null)
                    {
                        depth = panel.depth;
                    }
                }
                CloseModuleEx(moduleName, depth);
            }

            if (viewController != null)
            {
                viewController.Dispose();
            }

            //            RemoveBgMask(module);
            UIModulePool.Instance.CloseModule(moduleName, module);
            var b = _moduleOpenSeque.Remove(moduleName);
            if (b && uiInfoDic [moduleName] == UILayerType.DefaultModule || uiInfoDic [moduleName] == UILayerType.BaseModule)
                FireOpenModuleData(TopModule);
            //            GameDebuger.GC();
        }

        //      if (withEX)
        //      {
        //          CloseModuleEx(moduleName, depth);
        //      }
    }

    private IViewController GetViewController(GameObject module)
    {
        var list = module.GetComponents<MonoBehaviour>();
        for (int i = 0, len = list.Length; i < len; i++)
        {
            var mono = list[i];
            if (mono is IViewController)
            {
                return mono as IViewController;
            }
        }
        return null;
    }

    /// <summary>
    /// 判断模块是否加载了
    /// 但是有可能是隐藏的状态
    /// </summary>
    /// <param name="moduleName"></param>
    /// <returns></returns>
    public bool IsModuleCacheContainsModule(string moduleName)
    {
        var check = false;
        UpdateModuleCache (moduleName, delegate(List<Tuple<string, GameObject>> set) {
            check = set.Find<Tuple<string, GameObject>>(s=>s.p1 == moduleName) != null;
        });
        return check;
    }

    private void AddModuleToCacheDic(string moduleName, GameObject moduleGO){
        UpdateModuleCache (moduleName, delegate(List<Tuple<string, GameObject>> set) {
            if (set == null){
                var ty = UILayerType.Invalid;
                uiInfoDic.TryGetValue(moduleName, out ty);
                if (ty != UILayerType.Invalid){
                    _moduleCacheDic[ty] = new List<Tuple<string, GameObject>>{Tuple.Create<string, GameObject>(moduleName, moduleGO)};  
                }
            }
            else{
                set.ReplaceOrAdd (s=>s.p1 == moduleName, Tuple.Create<string, GameObject>(moduleName, moduleGO));   
            }
        });

    }
    /// <summary>
    /// 模块打开了并且激活中
    /// </summary>
    /// <param name="moduleName"></param>
    /// <returns></returns>
    public bool IsModuleOpened(string moduleName)
    {
        var module = GetModuleByName(moduleName);
        if (module != null)
        {
            return module.activeSelf;
        }
        return false;
    }

    public GameObject HideModule(string moduleName)
    {
        var module = GetModuleByName (moduleName);

        if (module != null)
        {
            module.SetActive(false);

            int depth;
            if (!_layerCacheDic.TryGetValue(moduleName, out depth))
            {
                var panel = module.GetComponent<UIPanel>();
                if (panel != null)
                {
                    depth = panel.depth;
                }
            }

            HideModuleEx(moduleName, depth);
        }

        return module;
    }


    /// <summary>
    /// 代替 DataMgr 做法
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="moduleName"></param>
    /// <returns></returns>
    public T GetModuleController<T>(string moduleName) where T : MonoBehaviour, IViewController
    {
        var go = GetModuleByName(moduleName);
        if (go != null)
        {
            return go.GetComponent<T>();
        }

        return null;
    }

    private GameObject GetModuleByName(string moduleName)
    {
        GameObject go = null;
        UpdateModuleCache (moduleName, delegate(List<Tuple<string, GameObject>> set){
            var tuple = set.Find<Tuple<string, GameObject>> (s=>s.p1 == moduleName);    
            if (tuple != null){
                go = tuple.p2;  
            }
        });
        return go;
    }

    private void UpdateModuleCache(string moduleName, Action<List<Tuple<string, GameObject>>> handler){
        var ty = UILayerType.Invalid;
        uiInfoDic.TryGetValue (moduleName, out ty);

        List<Tuple<string, GameObject>> set = null;
        _moduleCacheDic.TryGetValue (ty, out set);
        GameUtil.SafeRun(handler, set);
    }

    private void RemoveElementFromModuleCache(string moduleName){
        UpdateModuleCache (moduleName, delegate(List<Tuple<string, GameObject>> set) {
            set.Remove (s=>s.p1 == moduleName);
        }); 
    }

    public void RegisterOpenEvent(string moduleName, OnModuleOpen openCallback)
    {
        _openEventDic[moduleName] = openCallback;
    }

    public void SendOpenEvent(string moduleName, IViewController vc)
    {
        if (_openEventDic.ContainsKey(moduleName))
        {
            OnModuleOpen openAction = _openEventDic[moduleName];
            _openEventDic[moduleName] = null;
            if (openAction != null)
                openAction(vc);
        }
    }

    private void AddBgMask(string moduleName, GameObject module, bool bgMaskClose)
    {
        var bgMask = NGUITools.AddChild(module, ResourcePoolManager.Instance.LoadUI("ModuleBgBoxCollider"));

        if (bgMaskClose)
        {
            var button = bgMask.GetMissingComponent<UIEventTrigger>();
            EventDelegate.Set(button.onClick, () =>
                {
                    CloseModule(moduleName);
                });
        }
        var uiWidget = bgMask.GetMissingComponent<UIWidget>();
        uiWidget.depth = -1;
        uiWidget.autoResizeBoxCollider = true;
        uiWidget.SetAnchor(module, -10, -10, 10, 10);
        NGUITools.AddWidgetCollider(bgMask);
    }

    //    private void RemoveBgMask(GameObject module)
    //    {
    //        var child = module.transform.Find("ModuleBgBoxCollider(Clone)");
    //        if (child != null)
    //        {
    //            Object.Destroy(child.gameObject);
    //        }
    //    }

    public void CloseOtherModuleWhenNpcDialogue()
    {
        var names = new List<string>{
            ProxyLoginModule.NAME
            , ProxyWindowModule.NAME_WindowPrefabForTop
            , ProxyWindowModule.SIMPLE_NAME_WindowPrefabForTop
            };

        FilterMoudleCacheDic (
            delegate(string name) {
            var str = names.Find(s=>s == name); 
            return string.IsNullOrEmpty(str);   
        }
            , delegate(string name) {
            CloseModule(name);
        });
    }

    public void CloseOtherModuleWhenRelogin()
    {
        var names = new List<string>{ 
            ProxyRoleCreateModule.NAME
            , ProxyLoginModule.NAME};

        FilterMoudleCacheDic (
            delegate(string name) {
            var str = names.Find(s=>s == name); 
            return string.IsNullOrEmpty(str);   
        }
            , delegate(string name) {
            CloseModule(name);
        });
    }

    private IEnumerable<string> FindAllModuleNamesInMoudleCacheDic(Predicate<string> predicate){
        List<string> keys = new List<string>();

        _moduleCacheDic.ForEach(
            delegate(KeyValuePair<UILayerType, List<Tuple<string, GameObject>>> kv) {
                kv.Value.ForEach(delegate(Tuple<string, GameObject> tuple) {
                    if (predicate != null && predicate(tuple.p1)){
                        keys.Add(tuple.p1);
                    }
                });
            });
        return keys;
    }
    private void FilterMoudleCacheDic(
        Predicate<string> predicate
        , Action<string> handler = null){

        IEnumerable<string> keys = FindAllModuleNamesInMoudleCacheDic (predicate);

        keys.ForEach (s=>GameUtil.SafeRun(handler, s));
    }

    public void CloseOtherButThis(string moduleName)
    {
        List<string> names = new List<string>{
            ProxyRoleCreateModule.NAME
            , ProxyLoginModule.NAME
//            ,ProxyMainUI.MAINUI_VIEW
//            , BattleController.Prefab_Path
//            , ProxyLoseGuideModule.NAME
            , moduleName};

        FilterMoudleCacheDic (
            delegate(string name) {
            var str = names.Find(s=>s == name); 
            return string.IsNullOrEmpty(str);   
        }
            , delegate(string name) {
            CloseModule(name);
        });
    }

    public void CloseOtherModuleWhenGuide()
    {
        List<string> names = new List<string>{
//            ProxyMainUI.MAINUI_VIEW
//            , BattleController.Prefab_Path
//            , ProxyChatModule.NAME
//            , ProxyNewbieGuideModule.HIGHLIGHT_VIEW
//            , ProxyMainUI.FUNCTIONOPEN_VIEW
//            , SceneFadeEffectController.NAME
//            , ProxyTournamentModule.TournamentPath
//            , ProxyTournamentModule.TournamentV2Path
//            , ProxyGuildCompetitionModule.guildCompetitionExpandPath
//            , ProxyCampWarModule.EXPAND_VIEW_PATH
//            , EscortExpandView.NAME
//            , ProxySnowWorldBossExpandModule.NAME
//            , ProxyDaTangModule.NAME
//            , CSPKMainView.NAME
        };

        FilterMoudleCacheDic (
            delegate(string name) {
            var str = names.Find(s=>s == name); 
            return string.IsNullOrEmpty(str);   
        }
            , delegate(string name) {
            CloseModule(name);
        });
    }

    public void ResetWhenExit()
    {
        _openEventDic.Clear();
        CloseOtherModuleWhenRelogin();
        _returnModuleList.Clear();
        openModuleStream.Hold(String.Empty);
    }

    public void Dispose()
    {
        ResetWhenExit();   
        openModuleStream = openModuleStream.CloseOnceNull();
    }

    #region 弹窗的数据存储

    private class ModuleRecord
    {
        public string ModuleName;
        public int Depth;
        public bool Active;

        public ModuleRecord(string moduleName, int depth, bool active)
        {
            ModuleName = moduleName;
            Depth = depth;
            Active = active;
        }
    }

    private List<ModuleRecord> _returnModuleList;

    /// <summary>
    /// 判断返回界面队列里面是否有该界面
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private int FindReturnModuleIndexByName(string name)
    {
        return _returnModuleList.FindIndex(record => record.ModuleName == name);
    }

    private int FindMainModuleIndex()
    {
        return _returnModuleList.FindIndex(record =>
            GetUIModuleType(record.ModuleName) ==
            UIModuleDefinition.ModuleType.MainModule);
    }

    private int FindMainModuleLastIndex()
    {
        return _returnModuleList.FindLastIndex(record =>
            GetUIModuleType(record.ModuleName) ==
            UIModuleDefinition.ModuleType.MainModule);
    }
    #endregion

    #region 为了弹窗返回做的临时处理
    // don't need to use depth to check if module can return, check it by layer configuration -- modified by fish
    private bool IsModuleCanReturn(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return false;
        }

        return (LayerManager.Instance != null
            && LayerManager.Instance.CurUIMode >= UIMode.GAME
            && (UIModuleDefinition.IsMainModule(name)
                || CheckLayerInRange(name, UILayerType.DefaultModule, UILayerType.FiveModule)));
    }

    private bool CheckLayerInRange(string name, UILayerType low, UILayerType high){
        UILayerType ty = UILayerType.Invalid;
        bool check = uiInfoDic.TryGetValue (name, out ty);
        if (!check) {
            return false;
        } else {
            return ty >= low && ty <= high;
        }
    }

    private UIModuleDefinition.ModuleType GetUIModuleType(string name)
    {
        if (!IsModuleCanReturn (name)) {
            return UIModuleDefinition.ModuleType.None;
        } else {

            var layer = UILayerType.Invalid;
            uiInfoDic.TryGetValue(name, out layer);
            return UIModuleDefinition.GetUIModuleType(name, layer); 
        }

    }

    private void OpenModuleEx(string name, int depth)
    {
        GameLog.LOGModule ("OpenModuleEx-------"+name);
        if (!IsModuleCanReturn(name))
        {
            return;
        }

        var index = FindReturnModuleIndexByName(name);
        var moduleTy = GetUIModuleType (name);

        switch (moduleTy)
        {
            case UIModuleDefinition.ModuleType.MainModule:
                {
                    var firstIndex = FindMainModuleIndex();
                    var lastIndex = FindMainModuleLastIndex();

                    do
                    {
                        if (index >= 0)
                        {
                            // 打开最上层
                            if (index == lastIndex)
                            {
                                _returnModuleList[lastIndex].Depth = depth;
                                var moduleName = _returnModuleList[lastIndex].ModuleName;
                                var module = GetModuleByName(moduleName);
                                module.ResetPanelsDepth(depth);
                                break;
                            }
                            // 打开某个已经隐藏掉的
                            else if (index == firstIndex)
                            {
                                var lastList = _returnModuleList.GetRange(1, lastIndex - 1);
                                _returnModuleList.RemoveRange(0, lastIndex);
                                for (int i = 0; i < lastList.Count; i++)
                                {
                                    var record = lastList[i];
                                    CloseModule(record.ModuleName);
                                }
                                firstIndex = FindMainModuleIndex();
                                lastIndex = FindMainModuleLastIndex();
                            }
                        }
                        // 列表里面什么都没有
                        if (_returnModuleList.Count == 0)
                        {
                            _returnModuleList.Add(new ModuleRecord(name, depth, true));
                        }
                        // 列表里面存着激活的一连串窗口
                        else if (firstIndex < 0 || firstIndex == lastIndex)
                        {
                            var tList = _returnModuleList.ShallowCopyCollection<ModuleRecord, List<ModuleRecord>>();
                            _returnModuleList.Clear();
                            for (int i = 0; i < tList.Count; i++)
                            {
                                var record = tList[i];
                                HideModule(record.ModuleName);
                            }
                            OpenModuleEx(name, depth);

                            for (int i = 0;i< tList.Count; i++)
                            {
                                var record = tList[i];
                                record.Active = false;
                            }
                            _returnModuleList.InsertRange(0, tList);
                        }
                        // 列表里面新旧的列表都有
                        else if (firstIndex != lastIndex)
                        {
                            var tList = _returnModuleList.ToList();
                            for (int i = 0; i < firstIndex + 1; i++)
                            {
                                CloseModule(tList[i].ModuleName);
                            }
                            _returnModuleList = tList.GetRange(lastIndex, tList.Count - lastIndex);
                            // 回调自己，执行上面一个else if
                            OpenModuleEx(name, depth);
                        }
                    } while (false);
                    break;
                }
            case UIModuleDefinition.ModuleType.SubModule:
                {
                    // 对于已经存在过的，交给Open去处理
                    // 这里移除记录就好
                    if (index >= 0)
                    {
                        _returnModuleList.RemoveAt(index);
                    }
                    _returnModuleList.Add(new ModuleRecord(name, depth, true));

                    break;
                }
        }
    }

    private void CloseModuleEx(string name, int depth)
    {
        if (!IsModuleCanReturn(name))
        {
            return;
        }

        var index = FindReturnModuleIndexByName(name);
        if (index < 0)
        {
            return;
        }
        switch (GetUIModuleType(name))
        {
            case UIModuleDefinition.ModuleType.MainModule:
                {
                    //var firstIndex = FindMainModuleIndex();
                    var lastIndex = FindMainModuleLastIndex();

                    if (index >= 0)
                    {
                        // 关闭隐藏掉的，直接移除就好
                        if (index != lastIndex)
                        {
                            _returnModuleList.RemoveAt(index);
                        }
                        // 关闭当前打开的
                        else
                        {
                            // 后一个主面板丢弃
                            var lastList = _returnModuleList.GetRange(lastIndex + 1, _returnModuleList.Count - lastIndex - 1);
                            var firstList = _returnModuleList.GetRange(0, lastIndex);
                            _returnModuleList.Clear();
                            // 前面的重新打开一边
                            for (int i = 0; i < firstList.Count; i++)
                            {
                                var record = firstList[i];
                                // 存在列表中的才打开，否则会引发bug
                                if (IsModuleCacheContainsModule(record.ModuleName))
                                {
                                    var layerType = uiInfoDic[record.ModuleName];
                                    OpenFunModule(record.ModuleName, layerType, false);
                                }
                            }
                            // 把后面的加回去
                            _returnModuleList.AddRange(lastList);
                        }
                    }
                    // 没记录，丢弃
                    break;
                }
            case UIModuleDefinition.ModuleType.SubModule:
                {
                    // 这里移除记录就好
                    if (index >= 0)
                    {
                        _returnModuleList.RemoveAt(index);
                    }

                    break;
                }
        }
    }

    private void HideModuleEx(string name, int depth)
    {
        if (!IsModuleCanReturn(name))
        {
            return;
        }

        var index = FindReturnModuleIndexByName(name);
        if (index < 0)
        {
            return;
        }
        switch (GetUIModuleType(name))
        {
            case UIModuleDefinition.ModuleType.MainModule:
                {
                    //var firstIndex = FindMainModuleIndex();
                    //var lastIndex = FindMainModuleLastIndex();

                    // 暂时移除处理
                    if (index >= 0)
                    {
                        //                        _returnModuleList.RemoveRange(0, index + 1);
                        var list = _returnModuleList.GetRange(0, index);
                        _returnModuleList.RemoveRange(0, index + 1);
                        for (int i = 0; i < list.Count; i++)
                        {
                            var record = list[i];
                            CloseModule(record.ModuleName);
                        }
                    }
                    break;
                }
            case UIModuleDefinition.ModuleType.SubModule:
                {
                    // 这里移除记录就好
                    if (index >= 0)
                    {
                        _returnModuleList.RemoveAt(index);
                    }

                    break;
                }
        }
    }
    #endregion

    public int GetCurDepthByLayerType(UILayerType type){
        List<Tuple<string, GameObject>> set = new List<Tuple<string, GameObject>>();
        _moduleCacheDic.TryGetValue (type, out set);

        if (set.IsNullOrEmpty ()) {
            return LayerManager.Instance.GetOriginDepthByLayerType (type);
        } else {
            var tuple = set[set.Count - 1];
            return tuple.p2.GetMaxDepthWithPanelAndWidget() + 1;
        }
    }

    public bool IsDefaultType(string moduleName)
    {
        if (!uiInfoDic.ContainsKey(moduleName))
            return false;
        return uiInfoDic[moduleName] == UILayerType.DefaultModule;
    }

    public bool IsBaseType(string moduleName)
    {
        if (!uiInfoDic.ContainsKey(moduleName))
            return false;
        return uiInfoDic[moduleName] == UILayerType.BaseModule;
    }

}