// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : xush
// Created  : 12/13/2017 8:36:31 PM
// **********************************************************************

using System.IO;
using UniRx;

public sealed partial class QuestionDataMgr
{
    
    public static partial class QuizariumViewLogic
    {
        private static CompositeDisposable _disposable;

        public static void Open()
        {
            // open的参数根据需求自己调整
            var layer = UILayerType.DefaultModule;
                var ctrl = QuizariumViewController.Show<QuizariumViewController>(
                QuizariumView.NAME
                , layer
                , true
                , true
                , Stream);
            InitReactiveEvents(ctrl);
        }
        
        private static void InitReactiveEvents(IQuizariumViewController ctrl)
        {
            if (ctrl == null) return;
            if (_disposable == null)
                _disposable = new CompositeDisposable();
            else
            {
                _disposable.Clear();
            }
        
            _disposable.Add(ctrl.CloseEvt.Subscribe(_=>Dispose()));
            _disposable.Add(ctrl.GetCloseHanlder.Subscribe(_=>CloseBtn_UIButtonClick()));
            _disposable.Add(ctrl.GetItemClickHandler.Subscribe(idx =>
            {
                QuestionNetMsg.QuizariumQuestion(idx);
            }));
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
            ProxyQuestion.CloseQuizariumView();
        }

    
    }
}

