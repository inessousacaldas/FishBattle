using UniRx;
using UnityEngine;

public abstract class MonoViewController<T> : MonoAutoCacher
    where T : BaseView, new()
{
    protected T _view;

    public T View {
        get { return _view; }
    }

    private Subject<Unit> closeStream;
    public IObservable<Unit> CloseEvt {
        get { return closeStream; }
    }
    
    protected float _setupStartTime;

    protected void LogSetupTime ()
    {
        #if UNITY_EDITOR
        GameDebuger.Log (string.Format ("启动 {0} 耗时：{1}", name, Time.realtimeSinceStartup - _setupStartTime));
        #endif
    }

    /// <summary>
    /// 业务不需要重载此函数
    /// </summary>
    void Awake ()
    {
        InitUICacheList();

        _setupStartTime = Time.realtimeSinceStartup;
        _view = BaseView.Create<T>(transform);
        closeStream = new Subject<Unit>();
        LateAwake();
    }

    protected virtual void LateAwake()
    {
        AfterInitView();
        RegistEvent();
        RegistCustomEvent();
    }

    // 仅在打开界面的时调用 用于播放动画
    protected virtual void OnAnimationStart()
    {
    }

    // 动画结束回调
    protected virtual void OnAnimationEnd()
    {
    }
    
    protected virtual void InitData()
    {

    }


    /// <summary>
    /// 处理U层初始化的，一个UI生命周期只会执行一次
    /// </summary>
    protected virtual void AfterInitView()
    {

    }

    /// <summary>
    /// 业务不需要重载此函数
    /// </summary>
    public sealed override void Dispose ()
    {
        JSTimer.Instance.CancelTimer("IinitView" + GetInstanceID());
        RemoveEvent();
        RemoveCustomEvent();
        OnDispose();
        DespawnUIList();

        if (_view != null) {
            _view.Dispose();
            _view = null;
        }

        closeStream.OnNext(new Unit());
        closeStream = closeStream.CloseOnceNull();
        //Dispose的时候自动销毁绑定脚本， 避免下次复用脚本导致无法再Awake进行Setup
        //UnityEngine.Object.Destroy(this);
        UnityEngine.Object.DestroyImmediate(this);
    }

    /// <summary>
    /// 释放数据，释放回调
    /// </summary>
    protected virtual void OnDispose()
    {
    }

    /// <summary>
    /// 注册自动生成事件订阅
    /// </summary>
    protected virtual void RegistEvent()
    {
    }

    protected virtual void RemoveEvent()
    {
    }

    /// <summary>
    /// 注册客户端事件
    /// </summary>
    protected virtual void RegistCustomEvent()
    {
    }

    protected virtual void RemoveCustomEvent()
    {
        
    }
}

