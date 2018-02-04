using UniRx;

namespace StaticDispose
{
    public partial class StaticDispose
    {
        private StaticDelegateRunner disposeTradeDataMgr = new StaticDelegateRunner(
                TradeDataMgr.ExecuteDispose);
    }
}

public sealed partial class TradeDataMgr
{
    public static TradeDataMgr DataMgr {
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
           stream = new Subject<ITradeData>();
        }
            
        if (_ins != null) return;
        _ins = new TradeDataMgr();
        _ins.Init();
        stream.Hold(_ins._data);
    }
    
    public static void ExecuteDispose()
    {
        if (_ins == null) return;
        _ins.Dispose();
	    _ins = null;
    }
    
    private static TradeDataMgr _ins = null;
    private CompositeDisposable _disposable;
    private TradeData _data = null;
    public static IObservableExpand<ITradeData> Stream{
        get{
            InitDataAndStream();
            return stream;
        }
    }

    private static Subject<ITradeData> stream = null;

    private static void FireData()
    {
        stream.OnNext(DataMgr._data);
    }

    private TradeDataMgr()
    {

    }

    public void Init(){
        _disposable = new CompositeDisposable();
        _data = new TradeData();
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
