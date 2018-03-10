using UniRx;

namespace StaticDispose
{
    public partial class StaticDispose
    {
        private StaticDelegateRunner disposeItemQuickBuyDataMgr = new StaticDelegateRunner(
                ItemQuickBuyDataMgr.ExecuteDispose);
    }
}

public sealed partial class ItemQuickBuyDataMgr
{
    public static ItemQuickBuyDataMgr DataMgr {
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
           stream = new Subject<IItemQuickBuyData>();
        }
            
        if (_ins != null) return;
        _ins = new ItemQuickBuyDataMgr();
        _ins.Init();
        stream.Hold(_ins._data);
    }
    
    public static void ExecuteDispose()
    {
        if (_ins == null) return;
        _ins.Dispose();
	    _ins = null;
    }
    
    private static ItemQuickBuyDataMgr _ins = null;
    private CompositeDisposable _disposable;
    private ItemQuickBuyData _data = null;
    public static IObservableExpand<IItemQuickBuyData> Stream{
        get{
            InitDataAndStream();
            return stream;
        }
    }

    private static Subject<IItemQuickBuyData> stream = null;

    private static void FireData()
    {
        stream.OnNext(DataMgr._data);
    }

    private ItemQuickBuyDataMgr()
    {

    }

    public void Init(){
        _disposable = new CompositeDisposable();
        _data = new ItemQuickBuyData();
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
