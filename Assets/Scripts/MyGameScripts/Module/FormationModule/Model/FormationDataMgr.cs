//using System;
//using AppDto;
//using UniRx;
//
//public sealed partial class FormationDataMgr
//{
//    public static FormationDataMgr Instance {
//        get
//        {
//            if (stream == null)
//                stream = new Subject<IFormationData>();
//            
//            if (_ins == null){
//                _ins = new FormationDataMgr();
//                _ins.Init();
//                stream.Hold(_ins._data);
//            }
//
//            return _ins;
//        }
//    }
//
//    private static FormationDataMgr _ins = null;
//    private IDisposable _disposable = null;
//
//    private FormationData _data = null;
//    public static IObservableExpand<IFormationData> Stream{
//        get{
//            if (stream == null)
//                stream = new Subject<IFormationData>();
//            return stream;
//        }
//    }
//
//    private static Subject<IFormationData> stream = null;
//
//    private static void FireData()
//    {
//        stream.OnNext(Instance._data);
//    }
//
//    private FormationDataMgr()
//    {
//
//    }
//
//    public void Init(){
//        _data = new FormationData();
//        _data.InitData();
//    }
//
//    public FormationState GetFormationState(Formation _formation) {
//        if (_formation.id == (int)Formation.FormationType.Regular
//            || _data.HasAcquiredFormation(_formation.id))
//            return FormationState.Learned;
//        var learn = true; // todo fish:  判断学习条件
//        return learn ? FormationState.Enable : FormationState.UnEnable;
//    }
//
//    public void Dispose(){
//        _data.Dispose();
//	    _data = null;
//	    stream.Dispose();
//	    stream = null;
//        if (_disposable != null)
//            _disposable.Dispose();
//        _disposable = null;
//    }
//
//}
