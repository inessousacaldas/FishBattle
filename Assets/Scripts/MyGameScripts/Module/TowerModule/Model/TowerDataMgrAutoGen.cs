using UniRx;

namespace StaticDispose
{
    public partial class StaticDispose
    {
        private StaticDelegateRunner disposeTowerDataMgr = new StaticDelegateRunner(
                TowerDataMgr.ExecuteDispose);
    }
}

public sealed partial class TowerDataMgr
{
    public static TowerDataMgr DataMgr {
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
           stream = new Subject<ITowerData>();
        }
            
        if (_ins != null) return;
        _ins = new TowerDataMgr();
        _ins.Init();
        stream.Hold(_ins._data);
    }
    
    public static void ExecuteDispose()
    {
        if (_ins == null) return;
        _ins.Dispose();
	    _ins = null;
    }
    
    private static TowerDataMgr _ins = null;
    private CompositeDisposable _disposable;
    private TowerData _data = null;
    public static IObservableExpand<ITowerData> Stream{
        get{
            InitDataAndStream();
            return stream;
        }
    }

    private static Subject<ITowerData> stream = null;

    private static void FireData()
    {
        stream.OnNext(DataMgr._data);
    }

    private TowerDataMgr()
    {

    }

    public void Init(){
        _disposable = new CompositeDisposable();
        _data = new TowerData();
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
