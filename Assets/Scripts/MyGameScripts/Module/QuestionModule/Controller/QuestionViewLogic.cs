// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : xush
// Created  : 12/12/2017 11:38:32 AM
// **********************************************************************

using System.IO;
using AppDto;
using UniRx;

public sealed partial class QuestionDataMgr
{
    
    public static partial class QuestionViewLogic
    {
        private static CompositeDisposable _disposable;

        public static void Open()
        {
            // open的参数根据需求自己调整
            var layer = UILayerType.DefaultModule;
                var ctrl = QuestionViewController.Show<QuestionViewController>(
                QuestionView.NAME
                , layer
                , true
                , true
                , Stream);
            InitReactiveEvents(ctrl);
        }
        
        private static void InitReactiveEvents(IQuestionViewController ctrl)
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
            _disposable.Add(ctrl.OnRewardBoxBtn_UIButtonClick.Subscribe(_ =>
            {
                if (DataMgr._data.QuestionDto.giftBoxState == (int) QuestionType.GiftBoxState.HARVEST)
                {
                    TipManager.AddTip("宝箱已经领取");
                    return;
                }   
                QuestionNetMsg.QuestionHarvest((int) DataMgr._data.GetQuestionType);
            }));
            _disposable.Add(ctrl.OnHelpBtn_UIButtonClick.Subscribe(_=>HelpBtn_UIButtonClick()));
            _disposable.Add(ctrl.GetItemClickHandler.Subscribe(idx => { QuestionNetMsg.QuestionAnswer((int)DataMgr._data.GetQuestionType, idx); }));
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
            ProxyQuestion.CloseQuestionView();
        }
        private static void HelpBtn_UIButtonClick()
        {
        }

    
    }
}

