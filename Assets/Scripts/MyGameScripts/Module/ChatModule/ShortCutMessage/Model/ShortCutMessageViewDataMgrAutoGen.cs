using UniRx;

namespace StaticDispose
{
    public partial class StaticDispose
    {
        private StaticDelegateRunner disposeShortCutMessageViewDataMgr = new StaticDelegateRunner(
                ShortCutMessageViewDataMgr.ExecuteDispose);
    }
}

public sealed partial class ShortCutMessageViewDataMgr
{
    public static ShortCutMessageViewDataMgr DataMgr {
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
           stream = new Subject<IShortCutMessageViewData>();
        }
            
        if (_ins != null) return;
        _ins = new ShortCutMessageViewDataMgr();
        _ins.Init();
        stream.Hold(_ins._data);
    }
    
    public static void ExecuteDispose()
    {
        if (_ins == null) return;
        _ins.Dispose();
	    _ins = null;
    }
    
    private static ShortCutMessageViewDataMgr _ins = null;
    private CompositeDisposable _disposable;
    private ShortCutMessageViewData _data = null;
    public static IObservableExpand<IShortCutMessageViewData> Stream{
        get{
            InitDataAndStream();
            return stream;
        }
    }

    private static Subject<IShortCutMessageViewData> stream = null;

    private static void FireData()
    {
        stream.OnNext(DataMgr._data);
    }

    private ShortCutMessageViewDataMgr()
    {

    }

    public void Init(){
        _disposable = new CompositeDisposable();
        _data = new ShortCutMessageViewData();
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
