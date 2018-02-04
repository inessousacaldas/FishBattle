using UniRx;

namespace StaticDispose
{
    public partial class StaticDispose
    {
        private StaticDelegateRunner disposeMissionDataMgr = new StaticDelegateRunner(
                MissionDataMgr.ExecuteDispose);
    }
}

public sealed partial class MissionDataMgr
{
    public static MissionDataMgr DataMgr {
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
           stream = new Subject<IMissionData>();
        }
            
        if (_ins != null) return;
        _ins = new MissionDataMgr();
        _ins.Init();
        stream.Hold(_ins._data);
    }
    
    public static void ExecuteDispose()
    {
        if (_ins == null) return;
        _ins.Dispose();
	    _ins = null;
    }
    
    private static MissionDataMgr _ins = null;
    private CompositeDisposable _disposable;
    private MissionData _data = null;
    public static IObservableExpand<IMissionData> Stream{
        get{
            InitDataAndStream();
            return stream;
        }
    }

    private static Subject<IMissionData> stream = null;

    private static void FireData()
    {
        stream.OnNext(DataMgr._data);
    }

    private MissionDataMgr()
    {

    }

    public void Init(){
        _disposable = new CompositeDisposable();
        _data = new MissionData();
        _data.InitData();
        stream.Hold(_ins._data);
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
