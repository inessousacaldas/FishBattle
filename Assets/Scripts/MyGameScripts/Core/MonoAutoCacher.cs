using UnityEngine;
using System.Collections.Generic;
using AssetPipeline;

/// <summary>
/// 能自动管理所属UI缓存的Controller。
/// @MarsZ 2016-11-25 16:29:45
/// </summary>
public class MonoAutoCacher:MonoController
{
    //缓存起来了的UI GameObject列表
    private List<GameObject> mCachedUIList = null;

    private List<IViewController> mControllerCachedUIList = null;

    //无需缓存的gameobject，界面关闭时会destroy
    private List<GameObject> mUIList = null;

    #region 缓存自动管理相关辅助方法

    protected void InitUICacheList()
    {
        AutoCacherHelper.InitUIList(ref mUIList);
        AutoCacherHelper.InitUICacheList(ref mCachedUIList);
        AutoCacherHelper.InitUIControllerCacheList(ref mControllerCachedUIList);
    }

    protected void DespawnUIList()
    {
        AutoCacherHelper.DespawnUIControllerList(ref mControllerCachedUIList);
        AutoCacherHelper.DespawnUIList(ref mCachedUIList);
        AutoCacherHelper.ClearUIList(ref mUIList);

        DisposeCDTaskManager();
    }

    #endregion

    #region 缓存自动管理相关接口，给子类调用

    /// <summary>
    /// 根据预设名字创建一个实例并添加到目标父级下。尝试从缓存池中取，且会在本Controller dispose时自动释放到缓存池中。
    /// </summary>
    /// <returns>The cached child.</returns>
    /// <param name="pParent">P parent.</param>
    /// <param name="pPrefabName">P prefab name.</param>
    ///
    // to be delete
    protected GameObject AddCachedChild(GameObject pParent, string pPrefabName)
    {
        return AutoCacherHelper.AddCachedChild(pParent, pPrefabName, ref mCachedUIList);
    }

    protected T AddController<T,V>(GameObject module)
        where V : BaseView, new()
        where T : MonolessViewController<V>, new()
    {
        return AutoCacherHelper.AddController<T,V>(module, ref mControllerCachedUIList);
    }

    protected T AddChild<T,V>(
        GameObject pParent
        , string pPrefabName
        , string nameStr = "")
        where V : BaseView, new()
        where T : MonolessViewController<V>, new()
    {
        var ctrl = AutoCacherHelper.AddChild<T, V>(
            pParent
            , pPrefabName
            , ref mUIList
            , ref mControllerCachedUIList);
        
        if (ctrl != null && !string.IsNullOrEmpty(nameStr))
        {
            ctrl.gameObject.name = nameStr;
        }

        return ctrl;
    }

    protected T AddChild<T,V>(
        GameObject pParent
        , GameObject pPrefab
        , string nameStr = "")
        where V : BaseView, new()
        where T : MonolessViewController<V>, new()
    {
        var ctrl = AutoCacherHelper.AddChild<T, V>(
            pParent
            , pPrefab
            , ref mUIList
            , ref mControllerCachedUIList);

        if (ctrl != null && !string.IsNullOrEmpty(nameStr))
        {
            ctrl.gameObject.name = nameStr;
        }

        return ctrl;
    }

    protected void RemoveChild<T, V>(T ctrl)
        where T : MonolessViewController<V>, new()
        where V : BaseView, new()
    {
        AutoCacherHelper.RemoveChild<T, V>(
            ctrl
            , ref mUIList
            , ref mControllerCachedUIList);
    }

    protected T AddCachedChild<T, V>(
        GameObject pParent
        , string pPrefabName
        , string nameStr = "")
        where T : MonolessViewController<V>, new()
        where V : BaseView, new()
    {
        var ctrl = AutoCacherHelper.AddCachedChild<T, V>(
            pParent
            , pPrefabName
            , ref mCachedUIList
            , ref mControllerCachedUIList);

        if (ctrl != null && !string.IsNullOrEmpty(nameStr))
        {
            ctrl.gameObject.name = nameStr;
        }

        return ctrl;
    }

    protected void RemoveCachedChild<T, V>(T ctrl)
        where T : MonolessViewController<V>, new()
        where V : BaseView, new()
    {
        AutoCacherHelper.RemoveCachedChild<T, V>(
            ctrl
        , ref mCachedUIList
        , ref mControllerCachedUIList);
    }

    #endregion

    #region CDTask统一管理

    private CDTaskManager mCDTaskManager;

    private CDTaskManager CDTaskManager
    {
        get
        {
            if (null == mCDTaskManager)
                mCDTaskManager = new CDTaskManager();
            return mCDTaskManager;
        }
    }

    private void DisposeCDTaskManager()
    {
        if (null != mCDTaskManager)
        {
            mCDTaskManager.Dispose();
            mCDTaskManager = null;
        }
    }

    protected JSTimer.CdTask AddOrResetCDTask(string taskName, float totalTime, JSTimer.CdTask.OnCdUpdate onUpdate = null, JSTimer.CdTask.OnCdFinish onFinished = null, float updateFrequence = 0.1f, bool timeScale = false)
    {
        return CDTaskManager.AddOrResetCDTask(taskName, totalTime, onUpdate, onFinished, updateFrequence, timeScale);
    }

    protected void RemoveCDTask(string taskName)
    {
        CDTaskManager.RemoveCDTask(taskName);
    }

    #endregion
}