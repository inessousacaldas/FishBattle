using UniRx;

namespace StaticDispose
{
    public partial class StaticDispose
    {
        private StaticDelegateRunner disposePartnerViewDataMgr = new StaticDelegateRunner(
                CrewViewDataMgr.ExecuteDispose);
    }
}

namespace StaticInit
{
    public partial class StaticInit
    {
        private StaticDispose.StaticDelegateRunner disposeCrewDataMgr = new StaticDispose.StaticDelegateRunner(
            () => { var mgr = CrewViewDataMgr.DataMgr; });
    }
}
    

public sealed partial class CrewViewDataMgr
{
    public static CrewViewDataMgr DataMgr {
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
           stream = new Subject<ICrewViewData>();
        }
            
        if (_ins != null) return;
        _ins = new CrewViewDataMgr();
        _ins.Init();
        stream.Hold(_ins._data);
    }
    
    public static void ExecuteDispose()
    {
        if (_ins == null) return;
        _ins.Dispose();
	    _ins = null;
    }
    
    private static CrewViewDataMgr _ins = null;
    private CompositeDisposable _disposable;
    private CrewViewData _data = null;
    public static IObservableExpand<ICrewViewData> Stream{
        get{
            InitDataAndStream();
            return stream;
        }
    }

    private static Subject<ICrewViewData> stream = null;

    private static void FireData()
    {
        stream.OnNext(DataMgr._data);
    }

    private CrewViewDataMgr()
    {

    }

    public void Init(){
        _disposable = new CompositeDisposable();
        _data = new CrewViewData();
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
