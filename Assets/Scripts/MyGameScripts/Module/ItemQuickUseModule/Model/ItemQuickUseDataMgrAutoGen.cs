using UniRx;

namespace StaticDispose
{
    public partial class StaticDispose
    {
        private StaticDelegateRunner disposeItemQuickUseDataMgr = new StaticDelegateRunner(
                ItemQuickUseDataMgr.ExecuteDispose);
    }
}

namespace StaticInit
{
    public partial class StaticInit
    {
        private StaticDispose.StaticDelegateRunner disposeItemQuickUseDataMgr = new StaticDispose.StaticDelegateRunner(
           () => { var mgr = ItemQuickUseDataMgr.DataMgr; });
    }
}

public sealed partial class ItemQuickUseDataMgr
{
    public static ItemQuickUseDataMgr DataMgr {
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
           stream = new Subject<IItemQuickUseData>();
        }
            
        if (_ins != null) return;
        _ins = new ItemQuickUseDataMgr();
        _ins.Init();
        stream.Hold(_ins._data);
    }
    
    public static void ExecuteDispose()
    {
        if (_ins == null) return;
        _ins.Dispose();
	    _ins = null;
    }
    
    private static ItemQuickUseDataMgr _ins = null;
    private CompositeDisposable _disposable;
    private ItemQuickUseData _data = null;
    public static IObservableExpand<IItemQuickUseData> Stream{
        get{
            InitDataAndStream();
            return stream;
        }
    }

    private static Subject<IItemQuickUseData> stream = null;

    private static void FireData()
    {
        stream.OnNext(DataMgr._data);
    }

    private ItemQuickUseDataMgr()
    {

    }

    public void Init(){
        _disposable = new CompositeDisposable();
        _data = new ItemQuickUseData();
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
