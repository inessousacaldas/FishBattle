using AppDto;
using UniRx;

namespace StaticDispose
{
    public partial class StaticDispose
    {
        private StaticDelegateRunner disposeCrewReCruitDataMgr = new StaticDelegateRunner(
                CrewReCruitDataMgr.ExecuteDispose);
    }
}

public sealed partial class CrewReCruitDataMgr
{
    public static CrewReCruitDataMgr DataMgr {
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
           stream = new Subject<ICrewReCruitData>();
        }
            
        if (_ins != null) return;
        _ins = new CrewReCruitDataMgr();
        _ins.Init();
        stream.Hold(_ins._data);
    }
    
    public static void ExecuteDispose()
    {
        if (_ins == null) return;
        _ins.Dispose();
	    _ins = null;
    }
    
    private static CrewReCruitDataMgr _ins = null;
    private CompositeDisposable _disposable;
    private CrewReCruitData _data = null;
    public static IObservableExpand<ICrewReCruitData> Stream{
        get{
            InitDataAndStream();
            return stream;
        }
    }

    private static Subject<ICrewReCruitData> stream = null;

    private static void FireData()
    {
        stream.OnNext(DataMgr._data);
    }

    private CrewReCruitDataMgr()
    {

    }

    public void Init(){
        _disposable = new CompositeDisposable();
        _data = new CrewReCruitData();
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

    public CrewInfoDto GetMianCrew() {
        return _data.GetMainCrewInfoDto();
    }
}
