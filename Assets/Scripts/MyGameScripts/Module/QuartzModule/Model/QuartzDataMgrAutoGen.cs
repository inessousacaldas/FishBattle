using UniRx;

namespace StaticDispose
{
    public partial class StaticDispose
    {
        private StaticDelegateRunner disposeQuartzDataMgr = new StaticDelegateRunner(
                QuartzDataMgr.ExecuteDispose);
    }
}

public sealed partial class QuartzDataMgr
{
    public static QuartzDataMgr DataMgr {
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
           stream = new Subject<IQuartzData>();
        }
            
        if (_ins != null) return;
        _ins = new QuartzDataMgr();
        _ins.Init();
        stream.Hold(_ins._data);
    }
    
    public static void ExecuteDispose()
    {
        if (_ins == null) return;
        _ins.Dispose();
	    _ins = null;
    }
    
    private static QuartzDataMgr _ins = null;
    private CompositeDisposable _disposable;
    private QuartzData _data = null;
    public static IObservableExpand<IQuartzData> Stream{
        get{
            InitDataAndStream();
            return stream;
        }
    }

    private static Subject<IQuartzData> stream = null;

    private static void FireData()
    {
        stream.OnNext(DataMgr._data);
    }

    private QuartzDataMgr()
    {

    }

    public void Init(){
        _disposable = new CompositeDisposable();
        _data = new QuartzData();
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
