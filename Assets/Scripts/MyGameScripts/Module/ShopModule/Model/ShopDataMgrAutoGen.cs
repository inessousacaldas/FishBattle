using UniRx;

namespace StaticDispose
{
    public partial class StaticDispose
    {
        private StaticDelegateRunner disposeShopDataMgr = new StaticDelegateRunner(
                ShopDataMgr.ExecuteDispose);
    }
}

public sealed partial class ShopDataMgr
{
    public static ShopDataMgr DataMgr {
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
           stream = new Subject<IShopData>();
        }
            
        if (_ins != null) return;
        _ins = new ShopDataMgr();
        _ins.Init();
        stream.Hold(_ins._data);
    }
    
    public static void ExecuteDispose()
    {
        if (_ins == null) return;
        _ins.Dispose();
	    _ins = null;
    }
    
    private static ShopDataMgr _ins = null;
    private CompositeDisposable _disposable;
    private ShopData _data = null;
    public static IObservableExpand<IShopData> Stream{
        get{
            InitDataAndStream();
            return stream;
        }
    }

    private static Subject<IShopData> stream = null;

    private static void FireData()
    {
        stream.OnNext(DataMgr._data);
    }

    private ShopDataMgr()
    {

    }

    public void Init(){
        _disposable = new CompositeDisposable();
        _data = new ShopData();
        _data.InitData();
        stream.Hold(_ins._data);
        LateInit();
    }

    public void Dispose(){
        OnDispose();
        _data.ClearData();
	    _data = null;
	    stream = stream.CloseOnceNull();
        _disposable.Dispose();
        _disposable = null;
    }
}
