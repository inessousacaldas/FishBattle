// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : xush
// Created  : 12/18/2017 5:53:51 PM
// **********************************************************************

using UniRx;

public sealed partial class CrewViewDataMgr
{
    
    public static partial class CrewUpGradeViewLogic
    {
        private static CompositeDisposable _disposable;

        public static void Open()
        {
        // open的参数根据需求自己调整
	    var layer = UILayerType.SubModule;
            var ctrl = CrewUpGradeViewController.Show<CrewUpGradeViewController>(
                CrewUpGradeView.NAME
                , layer
                , true
                , true
                , Stream);
            InitReactiveEvents(ctrl);
        }
        
        private static void InitReactiveEvents(ICrewUpGradeViewController ctrl)
        {
            if (ctrl == null) return;
            if (_disposable == null)
                _disposable = new CompositeDisposable();
            else
            {
                _disposable.Clear();
            }
        
            _disposable.Add(ctrl.CloseEvt.Subscribe(_=>Dispose()));
            _disposable.Add(ctrl.OnCloseBtn_UIButtonClick.Subscribe(_=>CloseBtn_UIButtonClick()));
            _disposable.Add(ctrl.OnUpGradeBtn_UIButtonClick.Subscribe(_=>UpGradeBtn_UIButtonClick()));
            _disposable.Add(ctrl.OnUseAllBtn_UIButtonClick.Subscribe(_=>UseAllBtn_UIButtonClick()));
           
        }
            
        private static void Dispose()
        {
            _disposable = _disposable.CloseOnceNull();
            OnDispose();    
        }
        
        // 如果有自定义的内容需要清理，在此实现
        private static void OnDispose()
        {
            
        }

        private static void CloseBtn_UIButtonClick()
        {
            CrewProxy.CloseCrewUpGradeView();
        }

        private static void UpGradeBtn_UIButtonClick()
        {
            var uid = DataMgr._data.GetCurCrewUId;
            CrewViewNetMsg.CrewUpGrade(uid);
        }

        private static void UseAllBtn_UIButtonClick()
        {
            var uid = DataMgr._data.GetCurCrewUId;
            CrewViewNetMsg.UseAllProps(uid);
        }

    
    }
}

