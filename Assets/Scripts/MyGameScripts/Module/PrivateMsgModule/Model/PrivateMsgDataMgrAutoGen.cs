using UniRx;

namespace StaticDispose
{
    public partial class StaticDispose
    {
        private StaticDelegateRunner disposePrivateMsgDataMgr = new StaticDelegateRunner(
                PrivateMsgDataMgr.ExecuteDispose);
    }
}

public sealed partial class PrivateMsgDataMgr
{
    public static PrivateMsgDataMgr DataMgr
    {
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
            stream = new Subject<IPrivateMsgData>();
        }

        if (_ins != null) return;
        _ins = new PrivateMsgDataMgr();
        _ins.Init();
        stream.Hold(_ins._data);
    }

    public static void ExecuteDispose()
    {
        if (_ins == null) return;
        _ins.Dispose();
        _ins = null;
    }

    private static PrivateMsgDataMgr _ins = null;
    private CompositeDisposable _disposable;
    private PrivateMsgData _data = null;
    public static IObservableExpand<IPrivateMsgData> Stream
    {
        get
        {
            InitDataAndStream();
            return stream;
        }
    }

    private static Subject<IPrivateMsgData> stream = null;

    private static void FireData()
    {
        stream.OnNext(DataMgr._data);
    }

    private PrivateMsgDataMgr()
    {

    }

    public void Init()
    {
        _disposable = new CompositeDisposable();
        _data = new PrivateMsgData();
        _data.InitData();
        LateInit();
    }

    public void Dispose()
    {
        OnDispose();
        _data.Dispose();
        _data = null;
        stream = stream.CloseOnceNull();
        _disposable.Dispose();
        _disposable = null;
    }
}
