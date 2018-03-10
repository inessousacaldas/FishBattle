using UniRx;
using AppDto;

namespace StaticDispose
{
    public partial class StaticDispose
    {
        private StaticDelegateRunner disposeMainUIDataMgr = new StaticDelegateRunner(
                MainUIDataMgr.ExecuteDispose);
    }
}

public sealed partial class MainUIDataMgr
{
    public static MainUIDataMgr DataMgr {
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
           stream = new Subject<IMainUIData>();
        }
            
        if (_ins != null) return;
        _ins = new MainUIDataMgr();
        _ins.Init();
        stream.Hold(_ins._data);
    }
    
    public static void ExecuteDispose()
    {
        if (_ins == null) return;
        _ins.Dispose();
	    _ins = null;
    }
    
    private static MainUIDataMgr _ins = null;
    private CompositeDisposable _disposable;
    private MainUIData _data = null;
    public static IObservableExpand<IMainUIData> Stream{
        get{
            InitDataAndStream();
            return stream;
        }
    }
    private static Subject<IMainUIData> stream = null;

    private static void FireData()
    {
        stream.OnNext(DataMgr._data);
    }
    private MainUIDataMgr()
    {

    }

    public void Init(){
        _disposable = new CompositeDisposable();
        _data = new MainUIData();
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
