using UniRx;

namespace StaticDispose
{
    public partial class StaticDispose
    {
        private StaticDelegateRunner disposeFlowerMainViewDataMgr = new StaticDelegateRunner(
                FlowerMainViewDataMgr.ExecuteDispose);
    }
}

public sealed partial class FlowerMainViewDataMgr
{
    public static FlowerMainViewDataMgr DataMgr {
        get
        {
            InitDataAndStream();
            return _ins;
        }
    }

    private static void InitDataAndStream()
    {
        if (stream == null)
        {
           stream = new Subject<IFlowerMainViewData>();
        }
            
        if (_ins != null) return;
        _ins = new FlowerMainViewDataMgr();
        _ins.Init();
        stream.Hold(_ins._data);
    }
    
    public static void ExecuteDispose()
    {
        if (_ins == null) return;
        _ins.Dispose();
	    _ins = null;
    }
    
    private static FlowerMainViewDataMgr _ins = null;
    private CompositeDisposable _disposable;
    private FlowerMainViewData _data = null;
    public static IObservableExpand<IFlowerMainViewData> Stream{
        get{
            InitDataAndStream();
            return stream;
        }
    }

    private static Subject<IFlowerMainViewData> stream = null;

    private static void FireData()
    {
        stream.OnNext(DataMgr._data);
    }

    private FlowerMainViewDataMgr()
    {

    }

    public void Init(){
        _disposable = new CompositeDisposable();
        _data = new FlowerMainViewData();
        _data.InitData();
        LateInit();
    }

    public void Dispose(){
        OnDispose();
        _data.Dispose();
	    _data = null;
	    stream = stream.CloseOnceNull();
        _disposable.Dispose();
        _disposable = null;
    }
}
