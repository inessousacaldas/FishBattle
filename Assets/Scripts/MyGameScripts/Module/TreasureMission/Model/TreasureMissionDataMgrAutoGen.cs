using UniRx;

namespace StaticDispose
{
    public partial class StaticDispose
    {
        private StaticDelegateRunner disposeTreasureMissionDataMgr = new StaticDelegateRunner(
                TreasureMissionDataMgr.ExecuteDispose);
    }
}

public sealed partial class TreasureMissionDataMgr
{
    public static TreasureMissionDataMgr DataMgr {
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
           stream = new Subject<ITreasureMissionData>();
        }
            
        if (_ins != null) return;
        _ins = new TreasureMissionDataMgr();
        _ins.Init();
        stream.Hold(_ins._data);
    }
    
    public static void ExecuteDispose()
    {
        if (_ins == null) return;
        _ins.Dispose();
	    _ins = null;
    }
    
    private static TreasureMissionDataMgr _ins = null;
    private CompositeDisposable _disposable;
    private TreasureMissionData _data = null;
    public static IObservableExpand<ITreasureMissionData> Stream{
        get{
            InitDataAndStream();
            return stream;
        }
    }

    private static Subject<ITreasureMissionData> stream = null;

    private static void FireData()
    {
        stream.OnNext(DataMgr._data);
    }

    private TreasureMissionDataMgr()
    {

    }

    public void Init(){
        _disposable = new CompositeDisposable();
        _data = new TreasureMissionData();
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
