using UniRx;

namespace StaticDispose
{
    public partial class StaticDispose
    {
        private StaticDelegateRunner disposeEveryDayMissionDataMgr = new StaticDelegateRunner(
                EveryDayMissionDataMgr.ExecuteDispose);
    }
}

public sealed partial class EveryDayMissionDataMgr
{
    public static EveryDayMissionDataMgr DataMgr {
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
           stream = new Subject<IEveryDayMissionData>();
        }
            
        if (_ins != null) return;
        _ins = new EveryDayMissionDataMgr();
        _ins.Init();
        stream.Hold(_ins._data);
    }
    
    public static void ExecuteDispose()
    {
        if (_ins == null) return;
        _ins.Dispose();
	    _ins = null;
    }
    
    private static EveryDayMissionDataMgr _ins = null;
    private CompositeDisposable _disposable;
    private EveryDayMissionData _data = null;
    public static IObservableExpand<IEveryDayMissionData> Stream{
        get{
            InitDataAndStream();
            return stream;
        }
    }

    private static Subject<IEveryDayMissionData> stream = null;

    private static void FireData()
    {
        stream.OnNext(DataMgr._data);
    }

    private EveryDayMissionDataMgr()
    {

    }

    public void Init(){
        _disposable = new CompositeDisposable();
        _data = new EveryDayMissionData();
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
