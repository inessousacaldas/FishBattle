using UniRx;

namespace StaticDispose
{
    public partial class StaticDispose
    {
        private StaticDelegateRunner disposeCopyPanelDataMgr = new StaticDelegateRunner(
                CopyPanelDataMgr.ExecuteDispose);
    }
}

public sealed partial class CopyPanelDataMgr
{
    public static CopyPanelDataMgr DataMgr {
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
           stream = new Subject<ICopyPanelData>();
        }
            
        if (_ins != null) return;
        _ins = new CopyPanelDataMgr();
        _ins.Init();
        stream.Hold(_ins._data);
    }
    
    public static void ExecuteDispose()
    {
        if (_ins == null) return;
        _ins.Dispose();
	    _ins = null;
    }
    
    private static CopyPanelDataMgr _ins = null;
    private CompositeDisposable _disposable;
    private CopyPanelData _data = null;
    public static IObservableExpand<ICopyPanelData> Stream{
        get{
            InitDataAndStream();
            return stream;
        }
    }

    private static Subject<ICopyPanelData> stream = null;

    private static void FireData()
    {
        stream.OnNext(DataMgr._data);
    }

    private CopyPanelDataMgr()
    {

    }

    public void Init(){
        _disposable = new CompositeDisposable();
        _data = new CopyPanelData();
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
