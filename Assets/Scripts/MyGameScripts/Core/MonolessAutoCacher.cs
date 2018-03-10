using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 能自动管理所属UI缓存的Controller。
/// @MarsZ 2016-11-25 16:29:45
/// </summary>
public class MonolessAutoCacher:IViewController
{
	//缓存起来了的UI GameObject列表
	private List<GameObject> mCachedUIList = null;
    private List<IViewController> mCachedUIControllerList = null;
    private List<GameObject> mUIList = null;
    #region 重写的方法，用以实现管理的自动化。

    protected virtual void InitView()
    {
    }

    protected virtual void OnDispose()
    {
    }

    public virtual void Dispose()
    {
        
    }

    //UI事件的移除，会统一处理，参见：UIHelper.RemoveNGUIEvent
    protected virtual void RegistCustomEvent()
    {
    }

    #endregion

    #region 缓存自动管理相关辅助方法

    protected void InitUICacheList()
    {
        AutoCacherHelper.InitUIList(ref mUIList);
        AutoCacherHelper.InitUICacheList(ref mCachedUIList);
        AutoCacherHelper.InitUIControllerCacheList(ref mCachedUIControllerList);
    }

    protected void DespawnUIList()
    {
        AutoCacherHelper.DespawnUIControllerList(ref mCachedUIControllerList);
        AutoCacherHelper.DespawnUIList(ref mCachedUIList);
        AutoCacherHelper.ClearUIList(ref mUIList);
    }

    #endregion

    #region 缓存自动管理相关接口，给子类调用
    protected T AddController<T,V>(GameObject module)
        where V : BaseView, new()
        where T : MonolessViewController<V>, new()
    {
        return AutoCacherHelper.AddController<T,V>(module, ref mCachedUIControllerList);
    }
    /// <summary>
    /// 根据预设名字创建一个实例并添加到目标父级下。尝试从缓存池中取，且会在本Controller dispose时自动释放到缓存池中。
    /// </summary>
    /// <returns>The cached child.</returns>
    /// <param name="pParent">P parent.</param>
    /// <param name="pPrefabName">P prefab name.</param>
    private GameObject AddCachedChild(GameObject pParent, string pPrefabName)
    {
        return AutoCacherHelper.AddCachedChild(pParent, pPrefabName, ref mCachedUIList);
    }

    /// <summary>
    /// 根据预设名字创建一个实例并添加到目标父级下。不从缓存池中取，也不会自动释放到缓存池中。
    /// </summary>
    /// <returns>The un cache child.</returns>
    /// <param name="pParent">P parent.</param>
    /// <param name="pPrefabName">P prefab name.</param>
    /// to be delete


    protected T AddChild<T, V>(
        GameObject pParent
        , string pPrefabName
        , string nameStr = "")
        where T : MonolessViewController<V>, new()
        where V : BaseView, new()
    {
        var ctrl = AutoCacherHelper.AddChild<T, V>(
            pParent
            , pPrefabName
            , ref mUIList
            , ref mCachedUIControllerList);
        if (ctrl != null && !string.IsNullOrEmpty(nameStr))
            ctrl.gameObject.name = nameStr;
        return ctrl;
    }

    protected void RemoveChild<T,V>(T ctrl)
        where T:MonolessViewController<V>, new()
        where V:BaseView, new()
    {
        AutoCacherHelper.RemoveChild<T, V>(
            ctrl
            , ref mUIList
            , ref mCachedUIControllerList);
    }

    protected T AddCachedChild<T, V>(
        GameObject pParent
        , string pPrefabName
        , string nameStr = "")
        where V : BaseView, new()
        where T : MonolessViewController<V>, new()
    {
        var ctrl = AutoCacherHelper.AddCachedChild<T, V>(
            pParent
            , pPrefabName
            , ref mCachedUIList
            , ref mCachedUIControllerList);
        if (ctrl != null && !string.IsNullOrEmpty(nameStr))
            ctrl.gameObject.name = nameStr;
        return ctrl;
    }

    protected void RemoveCachedChild<T,V>(T ctrl)
        where T:MonolessViewController<V>, new()
        where V:BaseView, new()
    {
        AutoCacherHelper.RemoveCachedChild<T, V>(
            ctrl
            , ref mCachedUIList
            , ref mCachedUIControllerList);
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