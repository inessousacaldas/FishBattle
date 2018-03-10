using UniRx;

namespace StaticDispose
{
    public partial class StaticDispose
    {
        private StaticDelegateRunner disposeRedPackDataMgr = new StaticDelegateRunner(
                RedPackDataMgr.ExecuteDispose);
    }
}

public sealed partial class RedPackDataMgr
{
    public static RedPackDataMgr DataMgr {
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
           stream = new Subject<IRedPackData>();
        }
            
        if (_ins != null) return;
        _ins = new RedPackDataMgr();
        _ins.Init();
        stream.Hold(_ins._data);
    }
    
    public static void ExecuteDispose()
    {
        if (_ins == null) return;
        _ins.Dispose();
	    _ins = null;
    }
    
    private static RedPackDataMgr _ins = null;
    private CompositeDisposable _disposable;
    private RedPackData _data = null;
    public static IObservableExpand<IRedPackData> Stream{
        get{
            InitDataAndStream();
            return stream;
        }
    }

    private static Subject<IRedPackData> stream = null;

    private static void FireData()
    {
        stream.OnNext(DataMgr._data);
    }

    private RedPackDataMgr()
    {

    }

    public void Init(){
        _disposable = new CompositeDisposable();
        _data = new RedPackData();
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
