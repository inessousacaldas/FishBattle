using UniRx;
using UnityEngine;
public interface IMonolessViewController
{
    void Show();
    void Hide();
    bool IsActive();
}
public abstract class MonolessViewController<T1> : MonolessAutoCacher,IMonolessViewController
    where T1 : BaseView, new()
{
    protected T1 _view;

    public GameObject gameObject { get { return _gameObject; }}
    public Transform transform{ get { return _transform;}}    //   可能会废弃

    private GameObject _gameObject;
    private Transform _transform;

    private Subject<Unit> cameraClickStream;
    public IObservable<Unit> CloseEvt {
        get { return cameraClickStream; }
    }
    
    public static T Create<T>(GameObject go)
        where T : MonolessViewController<T1>, new()
    {
        if (go == null)
            return null;
        var t = new T
        {
            _setupStartTime = Time.realtimeSinceStartup,
            _gameObject = go,
            _transform = go.transform
        };
        t.Setup();
        return t;
    }

    protected MonolessViewController()
    {

    }

    public T1 View
    {
        get { return _view; }
    }

    protected float _setupStartTime;

    protected virtual void LogSetupTime()
    {
        //GameDebuger.Log(string.Format("启动 {0} 耗时：{1}", name, Time.realtimeSinceStartup - _setupStartTime));
    }

    /// <summary>
    /// 业务不需要重载此函数
    /// </summary>
    private void Setup()
    {
        InitUICacheList();
        _view = BaseView.Create<T1>(transform);
        cameraClickStream = new Subject<Unit>(); 
        AfterInitView();
        
        InitReactiveEvents();
        RegistCustomEvent();
#if UNITY_EDITOR
        gameObject.GetMissingComponent<OpenScript>().type = GetType();
#endif
    }

    protected virtual void AfterInitView()
    {
    }

    protected virtual void InitReactiveEvents(){
        
    }

    protected virtual void ClearReactiveEvents(){}

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected virtual void RemoveCustomEvent(){}

    /// <summary>
    /// 业务不需要重载此函数
    /// </summary>
    public sealed override void Dispose()
    {
        ClearReactiveEvents();
        RemoveCustomEvent();
        cameraClickStream.OnNext(new Unit());
        cameraClickStream = cameraClickStream.CloseOnceNull();
        OnDispose();

        DespawnUIList();
        if (_view != null)
        {
            _view.Dispose();
            _view = null;
        }

        base.Dispose();
    }

    public bool IsActive()
    {
        return _gameObject.activeSelf;
    }

    public void Hide()
    {
        OnHide();
        _gameObject.SetActive(false);
    }

    public void Show()
    {
        OnShow();
        _gameObject.SetActive(true);
    }

    protected virtual void OnHide()
    {
    }
    
    protected virtual void OnShow()
    {
        
    }
}