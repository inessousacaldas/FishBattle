using UniRx;

namespace StaticDispose
{
    public partial class StaticDispose
    {
        private StaticDelegateRunner disposeRoleSkillDataMgr = new StaticDelegateRunner(
            delegate{
                RoleSkillDataMgr.ExecuteDispose();
            });
    }
}

public sealed partial class RoleSkillDataMgr
{
    public static RoleSkillDataMgr DataMgr {
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
           stream = new Subject<IRoleSkillData>();
        }
            
        if (_ins == null){
            _ins = new RoleSkillDataMgr();
            _ins.Init();
            stream.Hold(_ins._data);
        }
    }
    
    public static void ExecuteDispose()
    {
        if (_ins == null) return;
            _ins.Dispose();
    }
    
    private static RoleSkillDataMgr _ins = null;
    private CompositeDisposable _disposable;
    private RoleSkillData _data = null;
    public static IObservableExpand<IRoleSkillData> Stream{
        get{
            InitDataAndStream();
            return stream;
        }
    }

    private static Subject<IRoleSkillData> stream = null;

    private static void FireData()
    {
        stream.OnNext(DataMgr._data);
    }

    private RoleSkillDataMgr()
    {

    }

    public void Init(){
        _disposable = new CompositeDisposable();
        _data = new RoleSkillData();
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
