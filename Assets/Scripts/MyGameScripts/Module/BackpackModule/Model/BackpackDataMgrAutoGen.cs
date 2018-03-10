using UniRx;

namespace StaticDispose
{
    public partial class StaticDispose
    {
        private StaticDelegateRunner disposeBackpackDataMgr = new StaticDelegateRunner(
            BackpackDataMgr.ExecuteDispose);
    }
}
namespace StaticInit
{
    public partial class StaticInit
    {
        private StaticDispose.StaticDelegateRunner disposeBackpackDataMgr = new StaticDispose.StaticDelegateRunner(
           ()=> { var mgr = BackpackDataMgr.DataMgr; } );
    }
}

public sealed partial class BackpackDataMgr
{
    public static BackpackDataMgr DataMgr {
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
           stream = new Subject<IBackpackData>();
        }

        if (_ins != null) return;
        _ins = new BackpackDataMgr();
        _ins.Init();
        stream.Hold(_ins._data);
    }
    
    public static void ExecuteDispose()
    {
        if (_ins == null) return;
        _ins.Dispose();
        _ins = null;
    }
    
    private static BackpackDataMgr _ins = null;
    private CompositeDisposable _disposable;
    private BackpackData _data = null;
    public static IObservableExpand<IBackpackData> Stream{
        get{
            InitDataAndStream();
            return stream;
        }
    }

    private static Subject<IBackpackData> stream = null;

    private static void FireData()
    {
        stream.OnNext(DataMgr._data);
    }

    private BackpackDataMgr()
    {

    }

    public void Init(){
        _disposable = new CompositeDisposable();
        _data = new BackpackData();
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
