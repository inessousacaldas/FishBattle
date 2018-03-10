using UniRx;

public sealed partial class SocialityDataMgr
{
    public static SocialityDataMgr DataMgr {
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
            stream = new Subject<ISocialityData>();
        }
        if (_ins == null){
            _ins = new SocialityDataMgr();
            _ins.Init();
        }
    }
    private static SocialityDataMgr _ins = null;
    private CompositeDisposable _disposable;
    private SocialityData _data = null;
    public static IObservableExpand<ISocialityData> Stream{
        get{
            InitDataAndStream();
            return stream;
        }
    }

    private static Subject<ISocialityData> stream = null;

    private static void FireData()
    {
        stream.OnNext(DataMgr._data);
    }

    private SocialityDataMgr()
    {

    }
    
    public void Init(){
        _disposable = new CompositeDisposable();
        _data = new SocialityData();
        _data.InitData();
        stream.Hold(_data);
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
