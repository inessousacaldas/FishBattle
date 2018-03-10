using UniRx;

namespace StaticDispose
{
    public partial class StaticDispose
    {
        private StaticDelegateRunner disposeMisRewardTipDataMgr = new StaticDelegateRunner(
                MisRewardTipDataMgr.ExecuteDispose);
    }
}

public sealed partial class MisRewardTipDataMgr
{
    public static MisRewardTipDataMgr DataMgr {
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
           stream = new Subject<IMisRewardTipData>();
        }
            
        if (_ins != null) return;
        _ins = new MisRewardTipDataMgr();
        _ins.Init();
        stream.Hold(_ins._data);
    }
    
    public static void ExecuteDispose()
    {
        if (_ins == null) return;
        _ins.Dispose();
	    _ins = null;
    }
    
    private static MisRewardTipDataMgr _ins = null;
    private CompositeDisposable _disposable;
    private MisRewardTipData _data = null;
    public static IObservableExpand<IMisRewardTipData> Stream{
        get{
            InitDataAndStream();
            return stream;
        }
    }

    private static Subject<IMisRewardTipData> stream = null;

    private static void FireData()
    {
        stream.OnNext(DataMgr._data);
    }

    private MisRewardTipDataMgr()
    {

    }

    public void Init(){
        _disposable = new CompositeDisposable();
        _data = new MisRewardTipData();
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
