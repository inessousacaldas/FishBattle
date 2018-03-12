using UniRx;

namespace StaticDispose
{
    public partial class StaticDispose
    {
        private StaticDelegateRunner disposeRedPointDataMgr = new StaticDelegateRunner(
                RedPointDataMgr.ExecuteDispose);
    }
}

public sealed partial class RedPointDataMgr
{
    public static RedPointDataMgr DataMgr {
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
           stream = new Subject<IRedPointData>();
        }
            
        if (_ins != null) return;
        _ins = new RedPointDataMgr();
        _ins.Init();
        stream.Hold(_ins._data);
    }
    
    public static void ExecuteDispose()
    {
        if (_ins == null) return;
        _ins.Dispose();
	    _ins = null;
    }
    
    private static RedPointDataMgr _ins = null;
    private CompositeDisposable _disposable;
    private static RedPointData _data = null;
    public static IObservableExpand<IRedPointData> Stream{
        get{
            InitDataAndStream();
            return stream;
        }
    }

    private static Subject<IRedPointData> stream = null;

    private static void FireData()
    {
	if (_ins == null) return;
        stream.OnNext(DataMgr._data);
    }

    private RedPointDataMgr()
    {

    }

    public void Init(){
        _disposable = new CompositeDisposable();
        _data = new RedPointData();
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
