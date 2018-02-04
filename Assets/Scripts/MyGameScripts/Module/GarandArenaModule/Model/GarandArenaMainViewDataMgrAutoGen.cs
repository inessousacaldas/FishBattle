using UniRx;

namespace StaticDispose
{
    public partial class StaticDispose
    {
        private StaticDelegateRunner disposeGarandArenaMainViewDataMgr = new StaticDelegateRunner(
                GarandArenaMainViewDataMgr.ExecuteDispose);
    }
}

public sealed partial class GarandArenaMainViewDataMgr
{
    public static GarandArenaMainViewDataMgr DataMgr {
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
           stream = new Subject<IGarandArenaMainViewData>();
        }
            
        if (_ins != null) return;
        _ins = new GarandArenaMainViewDataMgr();
        _ins.Init();
        stream.Hold(_ins._data);
    }
    
    public static void ExecuteDispose()
    {
        if (_ins == null) return;
        _ins.Dispose();
	    _ins = null;
    }
    
    private static GarandArenaMainViewDataMgr _ins = null;
    private CompositeDisposable _disposable;
    private GarandArenaMainViewData _data = null;
    public static IObservableExpand<IGarandArenaMainViewData> Stream{
        get{
            InitDataAndStream();
            return stream;
        }
    }

    private static Subject<IGarandArenaMainViewData> stream = null;

    private static void FireData()
    {
        stream.OnNext(DataMgr._data);
    }

    private GarandArenaMainViewDataMgr()
    {

    }

    public void Init(){
        _disposable = new CompositeDisposable();
        _data = new GarandArenaMainViewData();
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
