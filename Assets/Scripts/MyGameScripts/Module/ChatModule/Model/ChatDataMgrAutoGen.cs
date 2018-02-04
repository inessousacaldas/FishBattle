using UniRx;

public sealed partial class ChatDataMgr
{
    public static ChatDataMgr DataMgr {
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
            stream = new Subject<IChatData>();
        }
        if (_ins == null){
            _ins = new ChatDataMgr();
            _ins.Init();
        }
    }
    private static ChatDataMgr _ins = null;
    private CompositeDisposable _disposable;
    private ChatData _data = null;
    public static IObservableExpand<IChatData> Stream{
        get{
            InitDataAndStream();
            return stream;
        }
    }

    private static Subject<IChatData> stream = null;

    private static void FireData()
    {
        stream.OnNext(DataMgr._data);
    }

    private ChatDataMgr()
    {

    }
    
    public void Init(){
        _data = new ChatData();
        _data.InitData();
        _disposable = new CompositeDisposable();
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
