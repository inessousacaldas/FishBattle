using UniRx;

namespace StaticDispose
{
    public partial class StaticDispose
    {
        private StaticDelegateRunner disposeMartialDataMgr = new StaticDelegateRunner(
                MartialDataMgr.ExecuteDispose);
    }
}

namespace StaticInit
{
    public partial class StaticInit
    {
        private StaticDispose.StaticDelegateRunner MartialDataMgr = new StaticDispose.StaticDelegateRunner(
           () => { var mgr = global::MartialDataMgr.DataMgr; });
    }
}

public sealed partial class MartialDataMgr
{
    public static MartialDataMgr DataMgr {
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
           stream = new Subject<IMartialData>();
        }
            
        if (_ins != null) return;
        _ins = new MartialDataMgr();
        _ins.Init();
        stream.Hold(_ins._data);
    }
    
    public static void ExecuteDispose()
    {
        if (_ins == null) return;
        _ins.Dispose();
	    _ins = null;
    }
    
    private static MartialDataMgr _ins = null;
    private CompositeDisposable _disposable;
    private MartialData _data = null;
    public static IObservableExpand<IMartialData> Stream{
        get{
            InitDataAndStream();
            return stream;
        }
    }

    private static Subject<IMartialData> stream = null;

    private static void FireData()
    {
        stream.OnNext(DataMgr._data);
    }

    private MartialDataMgr()
    {

    }

    public void Init(){
        _disposable = new CompositeDisposable();
        _data = new MartialData();
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
