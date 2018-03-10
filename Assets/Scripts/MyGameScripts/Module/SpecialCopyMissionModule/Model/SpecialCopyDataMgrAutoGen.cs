using UniRx;

namespace StaticDispose
{
    public partial class StaticDispose
    {
        private StaticDelegateRunner disposeSpecialCopyDataMgr = new StaticDelegateRunner(
                SpecialCopyDataMgr.ExecuteDispose);
    }
}

public sealed partial class SpecialCopyDataMgr
{
    public static SpecialCopyDataMgr DataMgr {
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
           stream = new Subject<ISpecialCopyData>();
        }
            
        if (_ins != null) return;
        _ins = new SpecialCopyDataMgr();
        _ins.Init();
        stream.Hold(_ins._data);
    }
    
    public static void ExecuteDispose()
    {
        if (_ins == null) return;
        _ins.Dispose();
	    _ins = null;
    }
    
    private static SpecialCopyDataMgr _ins = null;
    private CompositeDisposable _disposable;
    private SpecialCopyData _data = null;
    public static IObservableExpand<ISpecialCopyData> Stream{
        get{
            InitDataAndStream();
            return stream;
        }
    }

    private static Subject<ISpecialCopyData> stream = null;

    private static void FireData()
    {
        stream.OnNext(DataMgr._data);
    }

    private SpecialCopyDataMgr()
    {

    }

    public void Init(){
        _disposable = new CompositeDisposable();
        _data = new SpecialCopyData();
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
