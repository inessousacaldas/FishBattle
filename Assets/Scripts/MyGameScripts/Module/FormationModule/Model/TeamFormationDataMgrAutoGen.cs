using UniRx;

namespace StaticDispose
{
    public partial class StaticDispose
    {
        private StaticDelegateRunner disposeTeamFormationDataMgr = new StaticDelegateRunner(
                TeamFormationDataMgr.ExecuteDispose);
    }
}

public sealed partial class TeamFormationDataMgr
{
    public static TeamFormationDataMgr DataMgr {
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
           stream = new Subject<ITeamFormationData>();
        }
            
        if (_ins != null) return;
        _ins = new TeamFormationDataMgr();
        _ins.Init();
        stream.Hold(_ins._data);
    }
    
    public static void ExecuteDispose()
    {
        if (_ins == null) return;
        _ins.Dispose();
        _ins = null;
    }
    
    private static TeamFormationDataMgr _ins = null;
    private CompositeDisposable _disposable;

    private TeamFormationData _data
    {
        get { return data; }
        set
        {
            data = value;
        }
    }
    
    private TeamFormationData data = null;
    public static IObservableExpand<ITeamFormationData> Stream{
        get{
            InitDataAndStream();
            return stream;
        }
    }

    private static Subject<ITeamFormationData> stream = null;

    private static void FireData()
    {
        stream.OnNext(DataMgr._data);
    }

    private TeamFormationDataMgr()
    {

    }

    public void Init(){
        _disposable = new CompositeDisposable();
        _data = new TeamFormationData();
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
