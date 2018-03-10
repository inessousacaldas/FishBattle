// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : DM-PC092
// Created  : 1/22/2018 3:17:47 PM
// **********************************************************************

using UniRx;

public sealed partial class TremChallengeConfirmDataMgr
{
    public static partial class TeamChallengeConfirmViewLogic
    {
        private static CompositeDisposable _disposable;
        public static void Open()
        {
        // open的参数根据需求自己调整
            var ctrl = TeamChallengeConfirmViewController.Show<TeamChallengeConfirmViewController>(
                TeamChallengeConfirmView.NAME
                ,UILayerType.FloatTip
                , true
                , true
                , Stream);
            InitReactiveEvents(ctrl);
        }
        
        private static void InitReactiveEvents(ITeamChallengeConfirmViewController ctrl)
        {
            if (ctrl == null) return;
            if (_disposable == null)
                _disposable = new CompositeDisposable();
            else
            {
                _disposable.Clear();
            }
            _disposable.Add(ctrl.CloseEvt.Subscribe(_=>Dispose()));
            _disposable.Add(ctrl.OnSurelBtn_UIButtonClick.Subscribe(_=>SurelBtn_UIButtonClick()));
            _disposable.Add(ctrl.OnCanelBtn_UIButtonClick.Subscribe(_=>CanelBtn_UIButtonClick()));
        }
            
        private static void Dispose()
        {
            _disposable = _disposable.CloseOnceNull();
            OnDispose();    
        }
        
        // 如果有自定义的内容需要清理，在此实现
        private static void OnDispose()
        {
            DataMgr._data.OnCloseData();
        }
        private static void SurelBtn_UIButtonClick()
        {
            TremChallengeConfirmNetMsg.Team_PlayerConfirm(true);
        }
        private static void CanelBtn_UIButtonClick()
        {
            TremChallengeConfirmNetMsg.Team_PlayerConfirm(false);
        }

        public static void OnClose()
        {
            UIModuleManager.Instance.CloseModule(TeamChallengeConfirmView.NAME);
        }
    }
}

