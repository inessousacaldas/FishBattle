using UnityEngine;
using System.Collections.Generic;
using AssetPipeline;

public class AutoCacherHelper
{
    #region 缓存自动管理相关辅助方法

    public static void InitUIList(ref List<GameObject> pUIList)
    {
        pUIList = new List<GameObject>();
    }

    public static void InitUICacheList(ref List<GameObject> pCachedUIList)
    {
        pCachedUIList = new List<GameObject>();
    }

    public static void InitUIControllerCacheList(ref List<IViewController> pCachedUIList)
    {
        pCachedUIList = new List<IViewController>();
    }

    private static void AddToUIList(GameObject pGo, ref List<GameObject> pUIList)
    {
        pUIList.AddIfNotExist(pGo);
    }

    private static void AddToUICacheList(GameObject pGo, ref List<GameObject> pCachedUIList)
    {
        pCachedUIList.AddIfNotExist(pGo);
    }

    private static void AddToUIControllerCacheList(IViewController pMonolessViewController, ref List<IViewController> pCachedUIList)
    {
        pCachedUIList.AddIfNotExist(pMonolessViewController);
    }


    public static void ClearUIList(ref List<GameObject> pUIList)
    {
        if (pUIList.IsNullOrEmpty())
            return;
        var tNewList = pUIList.ToArray();
        for (int tCounter = 0, tLen = tNewList.Length; tCounter < tLen; tCounter++)
        {
            RemoveUI(tNewList[tCounter], ref pUIList);
        }
        tNewList = null;
        pUIList.Clear();
    }

    public static void RemoveUI(GameObject ui, ref List<GameObject> pUIList){
        if (null == ui)
            return;
        pUIList.RemoveItem(ui);
        GameObject.DestroyObject(ui);
    }

    public static void DespawnUIList(ref List<GameObject> pCachedUIList)
    {
		if (pCachedUIList.IsNullOrEmpty())
            return;
        var tNewList = pCachedUIList.ToArray();
        for (int tCounter = 0, tLen = tNewList.Length; tCounter < tLen; tCounter++)
        {
            DespawnUI(tNewList[tCounter], ref pCachedUIList);
        }
        tNewList = null;
        pCachedUIList.Clear();
    }

    private static void DespawnUI(GameObject pGo, ref List<GameObject> pCachedUIList)
    {
        if (null == pGo)
            return;
        pCachedUIList.RemoveItem(pGo);
        ResourcePoolManager.Instance.DespawnUI(pGo);
    }

    #endregion

    #region 缓存自动管理相关接口，给子类调用
    public static T AddController<T, V>(GameObject module, ref List<IViewController> mControllerCachedUIList)
        where V : BaseView, new()
        where T : MonolessViewController<V>, new()
    {
        if (module == null)
        {
            GameDebuger.LogError("module is null");
            return null;
        }
        var ctrl = MonolessViewController<V>.Create<T>(module);
        AddToUIControllerCacheList(ctrl, ref mControllerCachedUIList);
        return ctrl;
    }

    public static GameObject AddCachedChild(GameObject pParent, string pPrefabName, ref List<GameObject> pCachedUIList)
    {
        if (string.IsNullOrEmpty(pPrefabName))
        {
            GameDebuger.LogError("AddChild failed , pPrefabName is IsNullOrEmpty !");
            return null;
        }

        var tGameObject = SpawnUIGo(pPrefabName, ref pCachedUIList);
        if (null == tGameObject)
            return null;
        GameObjectExt.AddPoolChild(pParent, tGameObject);
		tGameObject.ResetPanelsDepth();
        return tGameObject;
    }

    public static T AddChild<T,V>(
        GameObject pParent
        , string pPrefabName
        , ref List<GameObject> mUIList
        , ref List<IViewController> mControllerCachedUIList)
        where V : BaseView, new()
        where T : MonolessViewController<V>, new()
    {
        if (string.IsNullOrEmpty(pPrefabName))
        {
            GameDebuger.LogError("AddChild failed , pPrefabName is IsNullOrEmpty !");
            return null;
        }
        var module = pParent.AddChildAndAdjustDepth(pPrefabName);
        var pMonolessViewController = module == null ? null : MonolessViewController<V>.Create<T>(module);
        AddToUIList(module, ref mUIList);
        AddToUIControllerCacheList(pMonolessViewController, ref mControllerCachedUIList);
        return pMonolessViewController;
    }

    public static T AddChild<T,V>(
        GameObject pParent
        , GameObject pPrefab
        , ref List<GameObject> mUIList
        , ref List<IViewController> mControllerCachedUIList)
        where V : BaseView, new()
        where T : MonolessViewController<V>, new()
    {
		var module = pParent.AddChildAndAdjustDepth(pPrefab);
        var pMonolessViewController = module == null ? null : MonolessViewController<V>.Create<T>(module);
        AddToUIList(module, ref mUIList);
        AddToUIControllerCacheList(pMonolessViewController, ref mControllerCachedUIList);
        return pMonolessViewController;
    }

    public static void RemoveChild<T, V>(
        T ctrl
        , ref List<GameObject> mUIList
        , ref List<IViewController> mControllerCachedUIList )
        where V : BaseView, new()
        where T : MonolessViewController<V>, new()
    {
        if (ctrl == null)
        {
            GameDebuger.LogError("ctrl is null");
            return;
        }
        DespawnUIController(ctrl, ref mControllerCachedUIList);
        RemoveUI(ctrl.gameObject, ref mUIList);
        GameObject.Destroy(ctrl.gameObject);
    }

    public static T AddCachedChild<T,V>(
        GameObject pParent
        , string pPrefabName
        , ref List<GameObject> mCachedUIList
        , ref List<IViewController> mControllerCachedUIList)
        where V : BaseView, new()
        where T : MonolessViewController<V>, new()
    {
        var module = AddCachedChild(pParent, pPrefabName, ref mCachedUIList);
        var ctrl = module == null ? null : MonolessViewController<V>.Create<T>(module);
        AddToUIControllerCacheList(ctrl, ref mControllerCachedUIList);
        return ctrl;
    }

    public static void RemoveCachedChild<T, V>(
        T ctrl
    , ref List<GameObject> mCachedUIList
    , ref List<IViewController> mControllerCachedUIList)
        where T : MonolessViewController<V>, new()
        where V : BaseView, new()
    {
        if (ctrl == null)
        {
            GameDebuger.LogError("ctrl is null");
            return;
        }
        DespawnUIController(ctrl, ref mControllerCachedUIList);
        DespawnUI(ctrl.gameObject, ref mCachedUIList);
    }

    private static Transform SpawnUITrans(string pPrefabName, ref List<GameObject> pCachedUIList)
    {
        var tGameObject = SpawnUIGo(pPrefabName, ref pCachedUIList);
        return null == tGameObject
			? null
			: tGameObject.transform;
    }

    private static GameObject SpawnUIGo(string pPrefabName, ref List<GameObject> pCachedUIList, GameObject parent = null)
    {
        if (string.IsNullOrEmpty(pPrefabName))
        {
            GameDebuger.LogError("SpawnUIGo failed , pPrefabName is null !");
            return null;		
        }
        var tGameObject = ResourcePoolManager.Instance.SpawnUIGo(pPrefabName, parent);
        AddToUICacheList(tGameObject, ref pCachedUIList);
        return tGameObject;
    }

    #endregion

    #region 缓存自动管理相关辅助方法



    public static void DespawnUIControllerList(ref List<IViewController> pCachedUIList)
    {
        if (pCachedUIList.IsNullOrEmpty())
        {
            return;
        }

        pCachedUIList.ForEach<IViewController>(s=>s.Dispose());
        pCachedUIList.Clear();
    }

    private static void DespawnUIController(IViewController pGo, ref List<IViewController> pCachedUIList)
    {
        if (null == pGo)
            return;
        pGo.Dispose();//调用目标Controller的Dispose，触发其OnDispose
        pCachedUIList.RemoveItem(pGo);
    }

    #endregion
}