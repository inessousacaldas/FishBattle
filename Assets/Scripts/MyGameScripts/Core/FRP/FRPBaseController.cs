
using UniRx;

public abstract class FRPBaseController_V1<ViewClass, IView>
    : MonoViewController<ViewClass>
    where ViewClass : BaseView, IView, new()
{
    public new IView View{get{ return _view;}}

    protected sealed override void LateAwake()
    {
        InitData();
        AfterInitView();
        RegistEvent();
        RegistCustomEvent();
        
        
        JSTimer.Instance.SetupCoolDown(
            "IinitView" + GetInstanceID()
            , 0.1f
            , null
            , delegate {
                InitViewWithStream();
            });
    }

    protected virtual void InitViewWithStream()
    {
    }
}

public interface ICloseView
{
    IObservable<Unit> CloseEvt { get; }
}

//public interface IView
//{
//    string Name { get; }
//}

public abstract class FRPBaseController<ControllerClass, ViewClass, IControllerClass, IData>
    : MonoViewController<ViewClass>
    where ControllerClass : FRPBaseController<ControllerClass, ViewClass, IControllerClass, IData>, IControllerClass
    where ViewClass : BaseView, /**IView,**/ new()
    where IControllerClass : ICloseView
{
    protected IObservableExpand<IData> _dataUpdator;   // 为以后反复开关，或者获取数据的序列，保留一个扩展性
    protected CompositeDisposable _disposable;

    public static IControllerClass Show<T>(
        string moduleName
        , UILayerType layerType
        , bool addBgMask
        , bool bgMaskClose = false
        , IObservableExpand<IData> dataUpdator = null)

        where T : ControllerClass
    {
        bgMaskClose = false;  // 需求改为点空白处不自动关闭 fish 2017.11.3
        var controller = UIModuleManager.Instance.OpenFunModule<T>(
            moduleName
            , layerType
            , addBgMask
            , bgMaskClose) as ControllerClass;

        if (controller != null)
        {
            controller.InitWithStream(dataUpdator);
        }

        return controller;
    }

    protected sealed override void LateAwake()
    {
        InitData();
        _disposable = new CompositeDisposable();
        AfterInitView();
    }

    private void InitWithStream(IObservableExpand<IData> dataUpdator)
    {
        _dataUpdator = dataUpdator;
        
        RegistEvent();
        RegistCustomEvent();

        JSTimer.Instance.SetupCoolDown(
            "IinitView" + GetInstanceID()
            , 0.1f
            , null
            , delegate
            {
                if (_dataUpdator != null)
                    _disposable.Add(_dataUpdator.Subscribe(UpdateDataAndView));
                if (_dataUpdator != null)
                { 
                    if (_dataUpdator.LastValue != null)
                        UpdateDataAndView(_dataUpdator.LastValue);
                    LogSetupTime();
                }
            });
    }

    protected abstract void UpdateDataAndView(IData data);

    protected override void OnDispose()
    {
        JSTimer.Instance.CancelTimer("IinitView" + GetInstanceID());
        _dataUpdator = null;

        _disposable = _disposable.CloseOnceNull();
    }
}
