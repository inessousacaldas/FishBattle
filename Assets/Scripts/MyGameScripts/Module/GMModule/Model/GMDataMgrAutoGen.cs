using UniRx;

namespace StaticDispose
{
    public partial class StaticDispose
    {
        private StaticDelegateRunner disposeGMDataMgr = new StaticDelegateRunner(
                GMDataMgr.ExecuteDispose);
    }
}

public sealed partial class GMDataMgr
{
    public static bool isOpenDtoConn;
    public static GMDataMgr DataMgr {
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
           stream = new Subject<IGMData>();
        }
            
        if (_ins != null) return;
        _ins = new GMDataMgr();
        _ins.Init();
        stream.Hold(_ins._data);
    }
    
    public static void ExecuteDispose()
    {
        if (_ins == null) return;
        _ins.Dispose();
	    _ins = null;
    }
    
    private static GMDataMgr _ins = null;
    private CompositeDisposable _disposable;
    private GMData _data = null;
    public static IObservableExpand<IGMData> Stream{
        get{
            InitDataAndStream();
            return stream;
        }
    }

    private static Subject<IGMData> stream = null;

    private static void FireData()
    {
        stream.OnNext(DataMgr._data);
    }

    private GMDataMgr()
    {

    }

    public void Init(){
        _disposable = new CompositeDisposable();
        _data = new GMData();
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
