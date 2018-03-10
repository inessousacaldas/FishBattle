using UniRx;

namespace StaticDispose
{
    public partial class StaticDispose
    {
        private StaticDelegateRunner disposeQuestionDataMgr = new StaticDelegateRunner(
                QuestionDataMgr.ExecuteDispose);
    }
}

public sealed partial class QuestionDataMgr
{
    public static QuestionDataMgr DataMgr {
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
           stream = new Subject<IQuestionData>();
        }
            
        if (_ins != null) return;
        _ins = new QuestionDataMgr();
        _ins.Init();
        stream.Hold(_ins._data);
    }
    
    public static void ExecuteDispose()
    {
        if (_ins == null) return;
        _ins.Dispose();
	    _ins = null;
    }
    
    private static QuestionDataMgr _ins = null;
    private CompositeDisposable _disposable;
    private QuestionData _data = null;
    public static IObservableExpand<IQuestionData> Stream{
        get{
            InitDataAndStream();
            return stream;
        }
    }

    private static Subject<IQuestionData> stream = null;

    private static void FireData()
    {
        stream.OnNext(DataMgr._data);
    }

    private QuestionDataMgr()
    {

    }

    public void Init(){
        _disposable = new CompositeDisposable();
        _data = new QuestionData();
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
