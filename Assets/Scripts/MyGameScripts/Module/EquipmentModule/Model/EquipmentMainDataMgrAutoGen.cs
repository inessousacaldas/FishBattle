using UniRx;

namespace StaticDispose
{
    public partial class StaticDispose
    {
        private StaticDelegateRunner disposeEquipmentMainDataMgr = new StaticDelegateRunner(
                EquipmentMainDataMgr.ExecuteDispose);
    }
}

public sealed partial class EquipmentMainDataMgr
{
    public static EquipmentMainDataMgr DataMgr {
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
           stream = new Subject<IEquipmentMainData>();
        }
            
        if (_ins != null) return;
        _ins = new EquipmentMainDataMgr();
        _ins.Init();
        stream.Hold(_ins._data);
    }
    
    public static void ExecuteDispose()
    {
        if (_ins == null) return;
        _ins.Dispose();
	    _ins = null;
    }
    
    private static EquipmentMainDataMgr _ins = null;
    private CompositeDisposable _disposable;
    private EquipmentMainData _data = null;
    public static IObservableExpand<IEquipmentMainData> Stream{
        get{
            InitDataAndStream();
            return stream;
        }
    }

    private static Subject<IEquipmentMainData> stream = null;

    private static void FireData()
    {
        stream.OnNext(DataMgr._data);
    }

    private EquipmentMainDataMgr()
    {

    }

    public void Init(){
        _disposable = new CompositeDisposable();
        _data = new EquipmentMainData();
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
