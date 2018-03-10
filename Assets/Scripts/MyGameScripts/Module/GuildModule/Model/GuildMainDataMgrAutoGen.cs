using UniRx;

namespace StaticDispose
{
    public partial class StaticDispose
    {
        private StaticDelegateRunner disposeGuildMainDataMgr = new StaticDelegateRunner(
                GuildMainDataMgr.ExecuteDispose);
    }
}

public sealed partial class GuildMainDataMgr
{
    public static GuildMainDataMgr DataMgr {
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
           stream = new Subject<IGuildMainData>();
        }
            
        if (_ins != null) return;
        _ins = new GuildMainDataMgr();
        _ins.Init();
        stream.Hold(_ins._data);
    }
    
    public static void ExecuteDispose()
    {
        if (_ins == null) return;
        _ins.Dispose();
	    _ins = null;
    }
    
    private static GuildMainDataMgr _ins = null;
    private CompositeDisposable _disposable;
    private GuildMainData _data = null;
    public static IObservableExpand<IGuildMainData> Stream{
        get{
            InitDataAndStream();
            return stream;
        }
    }

    private static Subject<IGuildMainData> stream = null;

    private static void FireData()
    {
        stream.OnNext(DataMgr._data);
    }

    private GuildMainDataMgr()
    {

    }

    public void Init(){
        _disposable = new CompositeDisposable();
        _data = new GuildMainData();
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
