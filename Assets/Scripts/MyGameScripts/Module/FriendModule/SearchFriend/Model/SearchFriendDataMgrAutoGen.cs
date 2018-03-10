using UniRx;

namespace StaticDispose
{
    public partial class StaticDispose
    {
        private StaticDelegateRunner disposeSearchFriendDataMgr = new StaticDelegateRunner(
                SearchFriendDataMgr.ExecuteDispose);
    }
}

public sealed partial class SearchFriendDataMgr
{
    public static SearchFriendDataMgr DataMgr {
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
           stream = new Subject<ISearchFriendData>();
        }
            
        if (_ins != null) return;
        _ins = new SearchFriendDataMgr();
        _ins.Init();
        stream.Hold(_ins._data);
    }
    
    public static void ExecuteDispose()
    {
        if (_ins == null) return;
        _ins.Dispose();
	    _ins = null;
    }
    
    private static SearchFriendDataMgr _ins = null;
    private CompositeDisposable _disposable;
    private SearchFriendData _data = null;
    public static IObservableExpand<ISearchFriendData> Stream{
        get{
            InitDataAndStream();
            return stream;
        }
    }

    private static Subject<ISearchFriendData> stream = null;

    private static void FireData()
    {
        stream.OnNext(DataMgr._data);
    }

    private SearchFriendDataMgr()
    {

    }

    public void Init(){
        _disposable = new CompositeDisposable();
        _data = new SearchFriendData();
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
