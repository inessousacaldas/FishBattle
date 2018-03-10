using UniRx;

namespace StaticDispose
{
    public partial class StaticDispose
    {
        private StaticDelegateRunner disposeFriendDataMgr = new StaticDelegateRunner(
                FriendDataMgr.ExecuteDispose);
    }
}

public sealed partial class FriendDataMgr
{
    public static FriendDataMgr DataMgr
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
            stream = new Subject<IFriendData>();
        }

        if (_ins != null) return;
        _ins = new FriendDataMgr();
        _ins.Init();
        stream.Hold(_ins._data);
    }

    public static void ExecuteDispose()
    {
        if (_ins == null) return;
        _ins.Dispose();
        _ins = null;
    }

    private static FriendDataMgr _ins = null;
    private CompositeDisposable _disposable;
    private FriendData _data = null;
    public static IObservableExpand<IFriendData> Stream
    {
        get
        {
            InitDataAndStream();
            return stream;
        }
    }

    private static Subject<IFriendData> stream = null;

    private static void FireData()
    {
        stream.OnNext(DataMgr._data);
    }

    private FriendDataMgr()
    {

    }

    public void Init()
    {
        _disposable = new CompositeDisposable();
        _data = new FriendData();
        _data.InitData();
        stream.Hold(_ins._data);
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
