using UniRx;

namespace StaticDispose
{
    public partial class StaticDispose
    {
        private StaticDelegateRunner disposeAssistSkillMainDataMgr = new StaticDelegateRunner(
                AssistSkillMainDataMgr.ExecuteDispose);
    }
}

public sealed partial class AssistSkillMainDataMgr
{
    public static AssistSkillMainDataMgr DataMgr {
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
           stream = new Subject<IAssistSkillMainData>();
        }
            
        if (_ins != null) return;
        _ins = new AssistSkillMainDataMgr();
        _ins.Init();
        stream.Hold(_ins._data);
    }
    
    public static void ExecuteDispose()
    {
        if (_ins == null) return;
        _ins.Dispose();
	    _ins = null;
    }
    
    private static AssistSkillMainDataMgr _ins = null;
    private CompositeDisposable _disposable;
    private AssistSkillMainData _data = null;
    public static IObservableExpand<IAssistSkillMainData> Stream{
        get{
            InitDataAndStream();
            return stream;
        }
    }

    private static Subject<IAssistSkillMainData> stream = null;

    private static void FireData()
    {
        stream.OnNext(DataMgr._data);
    }

    private AssistSkillMainDataMgr()
    {

    }

    public void Init(){
        _disposable = new CompositeDisposable();
        _data = new AssistSkillMainData();
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
