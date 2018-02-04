using UniRx;

namespace StaticDispose
{
    public partial class StaticDispose
    {
        private StaticDelegateRunner disposeMyPropertyDataMgr = new StaticDelegateRunner(
                PlayerPropertyDataMgr.ExecuteDispose);
    }
}

public sealed partial class PlayerPropertyDataMgr
{
    public static PlayerPropertyDataMgr DataMgr {
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
           stream = new Subject<IPlayerPropertyData>();
        }
            
        if (_ins != null) return;
        _ins = new PlayerPropertyDataMgr();
        _ins.Init();
        stream.Hold(_ins._data);
    }
    
    public static void ExecuteDispose()
    {
        if (_ins == null) return;
        _ins.Dispose();
	    _ins = null;
    }
    
    private static PlayerPropertyDataMgr _ins = null;
    private CompositeDisposable _disposable;
    private PlayerPropertyData _data = null;
    public static IObservableExpand<IPlayerPropertyData> Stream{
        get{
            InitDataAndStream();
            return stream;
        }
    }

    private static Subject<IPlayerPropertyData> stream = null;

    private static void FireData()
    {
        stream.OnNext(DataMgr._data);
    }

    private PlayerPropertyDataMgr()
    {

    }

    public void Init(){
        _disposable = new CompositeDisposable();
        _data = new PlayerPropertyData();
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
