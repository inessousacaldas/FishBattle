using UniRx;

namespace StaticDispose
{
    public partial class StaticDispose
    {
        private StaticDelegateRunner disposeGuideMainViewDataMgr = new StaticDelegateRunner(
                GuideMainViewDataMgr.ExecuteDispose);
    }
}

public sealed partial class GuideMainViewDataMgr
{
    public static GuideMainViewDataMgr DataMgr {
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
           stream = new Subject<IGuideMainViewData>();
        }
            
        if (_ins != null) return;
        _ins = new GuideMainViewDataMgr();
        _ins.Init();
        stream.Hold(_ins._data);
    }
    
    public static void ExecuteDispose()
    {
        if (_ins == null) return;
        _ins.Dispose();
	    _ins = null;
    }
    
    private static GuideMainViewDataMgr _ins = null;
    private CompositeDisposable _disposable;
    private GuideMainViewData _data = null;
    public static IObservableExpand<IGuideMainViewData> Stream{
        get{
            InitDataAndStream();
            return stream;
        }
    }

    private static Subject<IGuideMainViewData> stream = null;

    private static void FireData()
    {
        stream.OnNext(DataMgr._data);
    }

    private GuideMainViewDataMgr()
    {

    }

    public void Init(){
        _disposable = new CompositeDisposable();
        _data = new GuideMainViewData();
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
