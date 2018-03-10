using UniRx;

namespace StaticDispose
{
    public partial class StaticDispose
    {
        private StaticDelegateRunner disposeCrewSkillDataMgr = new StaticDelegateRunner(
                CrewSkillDataMgr.ExecuteDispose);
    }
}

public sealed partial class CrewSkillDataMgr
{
    public static CrewSkillDataMgr DataMgr {
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
           stream = new Subject<ICrewSkillData>();
        }
            
        if (_ins != null) return;
        _ins = new CrewSkillDataMgr();
        _ins.Init();
        stream.Hold(_ins._data);
    }
    
    public static void ExecuteDispose()
    {
        if (_ins == null) return;
        _ins.Dispose();
	    _ins = null;
    }
    
    private static CrewSkillDataMgr _ins = null;
    private CompositeDisposable _disposable;
    private CrewSkillData _data = null;
    public static IObservableExpand<ICrewSkillData> Stream{
        get{
            InitDataAndStream();
            return stream;
        }
    }

    private static Subject<ICrewSkillData> stream = null;

    private static void FireData()
    {
        stream.OnNext(DataMgr._data);
    }

    private CrewSkillDataMgr()
    {

    }

    public void Init(){
        _disposable = new CompositeDisposable();
        _data = new CrewSkillData();
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
