using UniRx;

namespace StaticDispose
{
    public partial class StaticDispose
    {
        private StaticDelegateRunner disposeScheduleMainViewDataMgr = new StaticDelegateRunner(
                ScheduleMainViewDataMgr.ExecuteDispose);
    }
}

public sealed partial class ScheduleMainViewDataMgr
{
    public static ScheduleMainViewDataMgr DataMgr {
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
           stream = new Subject<IScheduleMainViewData>();
        }
            
        if (_ins != null) return;
        _ins = new ScheduleMainViewDataMgr();
        _ins.Init();
        stream.Hold(_ins._data);
    }
    
    public static void ExecuteDispose()
    {
        if (_ins == null) return;
        _ins.Dispose();
	    _ins = null;
    }
    
    private static ScheduleMainViewDataMgr _ins = null;
    private CompositeDisposable _disposable;
    private ScheduleMainViewData _data = null;
    public static IObservableExpand<IScheduleMainViewData> Stream{
        get{
            InitDataAndStream();
            return stream;
        }
    }

    private static Subject<IScheduleMainViewData> stream = null;

    private static void FireData()
    {
        stream.OnNext(DataMgr._data);
    }

    private ScheduleMainViewDataMgr()
    {

    }

    public void Init(){
        _disposable = new CompositeDisposable();
        _data = new ScheduleMainViewData();
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
