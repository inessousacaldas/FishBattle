using UniRx;

namespace StaticDispose
{
    public partial class StaticDispose
    {
        private StaticDelegateRunner disposeTremChallengeConfirmDataMgr = new StaticDelegateRunner(
                TremChallengeConfirmDataMgr.ExecuteDispose);
    }
}

public sealed partial class TremChallengeConfirmDataMgr
{
    public static TremChallengeConfirmDataMgr DataMgr {
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
           stream = new Subject<ITremChallengeConfirmData>();
        }
            
        if (_ins != null) return;
        _ins = new TremChallengeConfirmDataMgr();
        _ins.Init();
        stream.Hold(_ins._data);
    }
    
    public static void ExecuteDispose()
    {
        if (_ins == null) return;
        _ins.Dispose();
	    _ins = null;
    }
    
    private static TremChallengeConfirmDataMgr _ins = null;
    private CompositeDisposable _disposable;
    private TremChallengeConfirmData _data = null;
    public static IObservableExpand<ITremChallengeConfirmData> Stream{
        get{
            InitDataAndStream();
            return stream;
        }
    }

    private static Subject<ITremChallengeConfirmData> stream = null;

    private static void FireData()
    {
        stream.OnNext(DataMgr._data);
    }

    private TremChallengeConfirmDataMgr()
    {

    }

    public void Init(){
        _disposable = new CompositeDisposable();
        _data = new TremChallengeConfirmData();
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
