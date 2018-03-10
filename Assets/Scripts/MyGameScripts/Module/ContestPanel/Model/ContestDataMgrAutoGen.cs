using UniRx;

namespace StaticDispose
{
    public partial class StaticDispose
    {
        private StaticDelegateRunner disposeContestDataMgr = new StaticDelegateRunner(
                ContestDataMgr.ExecuteDispose);
    }
}

public sealed partial class ContestDataMgr
{
    public static ContestDataMgr DataMgr {
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
           stream = new Subject<IContestData>();
        }
            
        if (_ins != null) return;
        _ins = new ContestDataMgr();
        _ins.Init();
        stream.Hold(_ins._data);
    }
    
    public static void ExecuteDispose()
    {
        if (_ins == null) return;
        _ins.Dispose();
	    _ins = null;
    }
    
    private static ContestDataMgr _ins = null;
    private CompositeDisposable _disposable;
    private ContestData _data = null;
    public static IObservableExpand<IContestData> Stream{
        get{
            InitDataAndStream();
            return stream;
        }
    }

    private static Subject<IContestData> stream = null;

    private static void FireData()
    {
        stream.OnNext(DataMgr._data);
    }

    private ContestDataMgr()
    {

    }

    public void Init(){
        _disposable = new CompositeDisposable();
        _data = new ContestData();
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
