using UniRx;

namespace StaticDispose
{
    public partial class StaticDispose
    {
        private StaticDelegateRunner disposeBattleDataManager = new StaticDelegateRunner(
            BattleDataManager.ExecuteDispose);
    }
}

public sealed partial class BattleDataManager
{
    public static BattleDataManager DataMgr {
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
           stream = new Subject<IBattleDemoModel>();
        }
            
        if (_ins != null) return;
        _ins = new BattleDataManager();
        _ins.Init();
        stream.Hold(_ins._data);
    }
    
    public static void ExecuteDispose()
    {
        if (_ins == null) return;
         _ins.Dispose();
        _ins = null;
        stream.Hold(null);
    }
    
    private static BattleDataManager _ins = null;
    private CompositeDisposable _disposable;
    private BattleDemoModel _data = null;
    public static IObservableExpand<IBattleDemoModel> Stream{
        get{
            InitDataAndStream();
            return stream;
        }
    }

    private static Subject<IBattleDemoModel> stream = null;

    // 太多权限要改，改不动，临时改为public －－fish 2017.7.19
    public static void FireData()
    {
        DataMgr.UpdateBattleState();
        stream.OnNext(DataMgr._data);
    }

    private BattleDataManager()
    {

    }

    public void Init(){
        _disposable = new CompositeDisposable();
        _data = new BattleDemoModel();
        _data.InitData();
        LateInit();
    }

    public void Dispose(){
        OnDispose();
        _data.Dispose();
	    _data = null;
	    
        _disposable.Dispose();
        _disposable = null;
    }
}
