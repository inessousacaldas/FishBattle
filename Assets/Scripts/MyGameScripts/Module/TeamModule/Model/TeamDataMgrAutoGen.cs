using UniRx;

namespace StaticDispose
{
    public partial class StaticDispose
    {
        private StaticDelegateRunner disposeTeamDataMgr = new StaticDelegateRunner(
                TeamDataMgr.ExecuteDispose);
    }
}

public sealed partial class TeamDataMgr
{
    public static TeamDataMgr DataMgr {
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
           stream = new Subject<ITeamData>();
        }

        if (_ins != null) return;
        _ins = new TeamDataMgr();
        _ins.Init();
        stream.Hold(_ins._data);
    }
    
    public static void ExecuteDispose()
    {
        if (_ins == null) return;
        _ins.Dispose();
        _ins = null;
    }
    
    private static TeamDataMgr _ins = null;
    private CompositeDisposable _disposable;
    private TeamData _data = null;
    public static IObservableExpand<ITeamData> Stream{
        get{
            InitDataAndStream();
            return stream;
        }
    }

    private static Subject<ITeamData> stream = null;

    private static void FireData()
    {
        stream.OnNext(DataMgr._data);
    }

    private TeamDataMgr()
    {

    }

    public void Init(){
        _disposable = new CompositeDisposable();
        _data = new TeamData();
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
