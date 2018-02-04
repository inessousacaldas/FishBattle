using UniRx;

namespace StaticDispose
{
    public partial class StaticDispose
    {
        private StaticDelegateRunner disposeEmailDataMgr = new StaticDelegateRunner(
                EmailDataMgr.ExecuteDispose);
    }
}

public sealed partial class EmailDataMgr
{
    public static EmailDataMgr DataMgr {
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
           stream = new Subject<IEmailData>();
        }
            
        if (_ins != null) return;
        _ins = new EmailDataMgr();
        _ins.Init();
        stream.Hold(_ins._data);
    }
    
    public static void ExecuteDispose()
    {
        if (_ins == null) return;
        _ins.Dispose();
	    _ins = null;
    }
    
    private static EmailDataMgr _ins = null;
    private CompositeDisposable _disposable;
    private EmailData _data = null;
    public static IObservableExpand<IEmailData> Stream{
        get{
            InitDataAndStream();
            return stream;
        }
    }

    private static Subject<IEmailData> stream = null;

    private static void FireData()
    {
        stream.OnNext(DataMgr._data);
    }

    private EmailDataMgr()
    {

    }

    public void Init(){
        _disposable = new CompositeDisposable();
        _data = new EmailData();
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
