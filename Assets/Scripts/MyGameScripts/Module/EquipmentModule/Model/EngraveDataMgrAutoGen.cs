using UniRx;

namespace StaticDispose
{
    public partial class StaticDispose
    {
        private StaticDelegateRunner disposeEngraveDataMgr = new StaticDelegateRunner(
                EngraveDataMgr.ExecuteDispose);
    }
}

public sealed partial class EngraveDataMgr
{
    public static EngraveDataMgr DataMgr {
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
           stream = new Subject<IEngraveData>();
        }
            
        if (_ins != null) return;
        _ins = new EngraveDataMgr();
        _ins.Init();
        stream.Hold(_ins._data);
    }
    
    public static void ExecuteDispose()
    {
        if (_ins == null) return;
        _ins.Dispose();
	    _ins = null;
    }
    
    private static EngraveDataMgr _ins = null;
    private CompositeDisposable _disposable;
    private EngraveData _data = null;
    public static IObservableExpand<IEngraveData> Stream{
        get{
            InitDataAndStream();
            return stream;
        }
    }

    private static Subject<IEngraveData> stream = null;

    private static void FireData()
    {
        stream.OnNext(DataMgr._data);
    }

    private EngraveDataMgr()
    {

    }

    public void Init(){
        _disposable = new CompositeDisposable();
        _data = new EngraveData();
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
