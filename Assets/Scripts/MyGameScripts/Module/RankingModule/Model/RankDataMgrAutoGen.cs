using UniRx;

namespace StaticDispose
{
    public partial class StaticDispose
    {
        private StaticDelegateRunner disposeRankDataMgr = new StaticDelegateRunner(
                RankDataMgr.ExecuteDispose);
    }
}

public sealed partial class RankDataMgr
{
    public static RankDataMgr DataMgr {
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
           stream = new Subject<IRankData>();
        }
            
        if (_ins != null) return;
        _ins = new RankDataMgr();
        _ins.Init();
        stream.Hold(_ins._data);
    }
    
    public static void ExecuteDispose()
    {
        if (_ins == null) return;
        _ins.Dispose();
	    _ins = null;
    }
    
    private static RankDataMgr _ins = null;
    private CompositeDisposable _disposable;
    private RankData _data = null;
    public static IObservableExpand<IRankData> Stream{
        get{
            InitDataAndStream();
            return stream;
        }
    }

    private static Subject<IRankData> stream = null;

    private static void FireData()
    {
        stream.OnNext(DataMgr._data);
    }

    private RankDataMgr()
    {

    }

    public void Init(){
        _disposable = new CompositeDisposable();
        _data = new RankData();
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
