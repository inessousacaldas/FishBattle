using UniRx;

namespace StaticDispose
{
    public partial class StaticDispose
    {
        private StaticDelegateRunner disposeWorldMapDataMgr = new StaticDelegateRunner(
                WorldMapDataMgr.ExecuteDispose);
    }
}

public sealed partial class WorldMapDataMgr
{
    public static WorldMapDataMgr DataMgr {
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
           stream = new Subject<IWorldMapData>();
        }
            
        if (_ins != null) return;
        _ins = new WorldMapDataMgr();
        _ins.Init();
        stream.Hold(_ins._data);
    }
    
    public static void ExecuteDispose()
    {
        if (_ins == null) return;
        _ins.Dispose();
	    _ins = null;
    }
    
    private static WorldMapDataMgr _ins = null;
    private CompositeDisposable _disposable;
    private WorldMapData _data = null;
    public static IObservableExpand<IWorldMapData> Stream{
        get{
            InitDataAndStream();
            return stream;
        }
    }

    private static Subject<IWorldMapData> stream = null;

    private static void FireData()
    {
        stream.OnNext(DataMgr._data);
    }

    private WorldMapDataMgr()
    {

    }

    public void Init(){
        _disposable = new CompositeDisposable();
        _data = new WorldMapData();
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
