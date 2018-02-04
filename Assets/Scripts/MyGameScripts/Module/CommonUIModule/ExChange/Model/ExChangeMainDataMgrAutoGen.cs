using UniRx;

namespace StaticDispose
{
    public partial class StaticDispose
    {
        private StaticDelegateRunner disposeExChangeMainDataMgr = new StaticDelegateRunner(
                ExChangeMainDataMgr.ExecuteDispose);
    }

}

public sealed partial class ExChangeMainDataMgr
{
    public static ExChangeMainDataMgr DataMgr {
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
           stream = new Subject<IExChangeMainData>();
        }
            
        if (_ins != null) return;
        _ins = new ExChangeMainDataMgr();
        _ins.Init();
        stream.Hold(_ins._data);
    }
    
    public static void ExecuteDispose()
    {
        if (_ins == null) return;
        _ins.Dispose();
	    _ins = null;
    }
    
    private static ExChangeMainDataMgr _ins = null;
    private CompositeDisposable _disposable;
    private ExChangeMainData _data = null;
    public static IObservableExpand<IExChangeMainData> Stream{
        get{
            InitDataAndStream();
            return stream;
        }
    }

    private static Subject<IExChangeMainData> stream = null;

    private static void FireData()
    {
        stream.OnNext(DataMgr._data);
    }

    private ExChangeMainDataMgr()
    {

    }

    public void Init(){
        _disposable = new CompositeDisposable();
        _data = new ExChangeMainData();
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
