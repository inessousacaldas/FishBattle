using UniRx;

namespace StaticDispose
{
    public partial class StaticDispose
    {
        private StaticDelegateRunner disposeEquipmentSpecialEffectDataMgr = new StaticDelegateRunner(
                EquipmentEffectsDataMgr.ExecuteDispose);
    }
}

public sealed partial class EquipmentEffectsDataMgr
{
    public static EquipmentEffectsDataMgr DataMgr {
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
           stream = new Subject<IEquipmentEffectsData>();
        }
            
        if (_ins != null) return;
        _ins = new EquipmentEffectsDataMgr();
        _ins.Init();
        stream.Hold(_ins._data);
    }
    
    public static void ExecuteDispose()
    {
        if (_ins == null) return;
        _ins.Dispose();
	    _ins = null;
    }
    
    private static EquipmentEffectsDataMgr _ins = null;
    private CompositeDisposable _disposable;
    private EquipmentEffectsData _data = null;
    public static IObservableExpand<IEquipmentEffectsData> Stream{
        get{
            InitDataAndStream();
            return stream;
        }
    }

    private static Subject<IEquipmentEffectsData> stream = null;

    private static void FireData()
    {
        stream.OnNext(DataMgr._data);
    }

    private EquipmentEffectsDataMgr()
    {

    }

    public void Init(){
        _disposable = new CompositeDisposable();
        _data = new EquipmentEffectsData();
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
