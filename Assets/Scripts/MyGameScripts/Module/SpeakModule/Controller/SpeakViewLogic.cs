// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : DM-PC092
// Created  : 7/21/2017 10:17:48 AM
// **********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using UniRx;

public sealed partial class SpeakViewDataMgr
{
    public static partial class SpeakViewLogic
    {
        private static CompositeDisposable _disposable;

        private static ISpeakData _data;

        private static string _taskname = "OnSendHandler";
        private static bool _canSpeak = true;

        public static void Open()
        {
        // open的参数根据需求自己调整
            var ctrl = SpeakViewController.Show<SpeakViewController>(
                SpeakView.NAME
                , UILayerType.ThreeModule
                , true
                , true
                , Stream);
            InitReactiveEvents(ctrl);

        }

        public static IObservableExpand<ISpeakViewData> Stream { get; set; }

        private static void InitReactiveEvents(ISpeakViewController ctrl)
        {
            if (ctrl == null) return;
            if (_disposable == null)
                _disposable = new CompositeDisposable();
            else
            {
                _disposable.Clear();
            }
            _disposable.Add(ctrl.CloseEvt.Subscribe(_ => Dispose()));

            _disposable.Add(ctrl.OnCloseBtn_UIButtonClick.Subscribe(_=>CloseBtn_UIButtonClick()));
            _disposable.Add(ctrl.OnSendBtn_UIButtonClick.Subscribe(_ => SendBtn_UIButtonClick(ctrl,ctrl.Message)));
            _disposable.Add(ctrl.OnHistoryBtn_UIButtonClick.Subscribe(_=> ctrl.SetHistoryPanelState()));
           
        }

        private static void Dispose()
        {
            OnDispose();
            _disposable = _disposable.CloseOnceNull();
        }

        private static void OnDispose()
        {
            
        }

        private static void CloseBtn_UIButtonClick()
        {
            ProxySpeakViewModule.Close();
        }

        private static void SendBtn_UIButtonClick(ISpeakViewController ctrl,string s)
        {
            if (!_canSpeak)
            {
                var remaintime = JSTimer.Instance.GetRemainTime(_taskname);
                TipManager.AddTip(string.Format("发送太急了，请等{0}秒后再试试吧.", (int)remaintime));
                return;
            }
            _canSpeak = false;
            JSTimer.Instance.SetupCoolDown(_taskname, 60f, null, () =>
            {
                _canSpeak = true;
            });
            ctrl.SendMessage();
            ctrl.AddHistoryMsg();
        }
    }
}

