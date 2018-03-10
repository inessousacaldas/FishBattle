using UniRx;

namespace StaticDispose
{
    public partial class StaticDispose
    {
        private StaticDelegateRunner disposeBracerMainViewDataMgr = new StaticDelegateRunner(
                BracerMainViewDataMgr.ExecuteDispose);
    }
}

public sealed partial class BracerMainViewDataMgr
{
    public static BracerMainViewDataMgr DataMgr {
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
           stream = new Subject<IBracerMainViewData>();
        }
            
        if (_ins != null) return;
        _ins = new BracerMainViewDataMgr();
        _ins.Init();
        stream.Hold(_ins._data);
    }
    
    public static void ExecuteDispose()
    {
        if (_ins == null) return;
        _ins.Dispose();
	    _ins = null;
    }
    
    private static BracerMainViewDataMgr _ins = null;
    private CompositeDisposable _disposable;
    private BracerMainViewData _data = null;
    public static IObservableExpand<IBracerMainViewData> Stream{
        get{
            InitDataAndStream();
            return stream;
        }
    }

    private static Subject<IBracerMainViewData> stream = null;

    private static void FireData()
    {
        stream.OnNext(DataMgr._data);
    }

    private BracerMainViewDataMgr()
    {

    }

    public void Init(){
        _disposable = new CompositeDisposable();
        _data = new BracerMainViewData();
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
