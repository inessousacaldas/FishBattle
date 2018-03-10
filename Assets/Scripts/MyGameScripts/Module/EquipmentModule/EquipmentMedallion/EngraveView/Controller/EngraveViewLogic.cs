// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : xjd
// Created  : 9/1/2017 2:37:22 PM
// **********************************************************************

using UniRx;
using System.Linq;
using UnityEngine;

public sealed partial class EngraveDataMgr
{
    
    public static partial class EngraveViewLogic
    {
        private static CompositeDisposable _disposable;

        public static void Open()
        {
        // open的参数根据需求自己调整
            var ctrl = EngraveViewController.Show<EngraveViewController>(
                EngraveView.NAME
                , UILayerType.DefaultModule
                , true
                , false
                , Stream);
            InitReactiveEvents(ctrl);
        }
        
        private static void InitReactiveEvents(IEngraveViewController ctrl)
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
            _disposable.Add(ctrl.OnSaleBtn_UIButtonClick.Subscribe(_=>SaleBtn_UIButtonClick()));
            _disposable.Add(ctrl.OnEngraveBtn_UIButtonClick.Subscribe(_=>EngraveBtn_UIButtonClick()));
            _disposable.Add(ctrl.OnTipBtn_UIButtonClick.Subscribe(_=>TipBtn_UIButtonClick()));

            _disposable.Add(ctrl.OnMedallionIdStream.Subscribe(item =>
            {
                if(!item.isAddbtn)
                {
                    DataMgr._data.CurSelMedallionId = item.id;
                    DataMgr._data.isShowSaleBtn = true;
                    ctrl.SetCurSelMedallionSpr(item.id);
                    ctrl.UpdateGroovePanel(DataMgr._data);
                }
                else
                {
                    TipManager.AddTip("打开商城");
                    //FireData();
                    //ctrl.UpdateMedallionList(DataMgr._data,true);
                }
            }));

            _disposable.Add(ctrl.OnRuneIdStream.Subscribe(item =>
            {
                if (!item.isAddbtn)
                {
                    DataMgr._data.CurSelRuneId = item.id;
                    
                    ctrl.SetCurSelRuneSpr(item.id);
                }
                else
                {
                    TipManager.AddTip("打开商城");
                    //FireData();
                    //ctrl.UpdateRuneList(DataMgr._data);
                }
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
            UIModuleManager.Instance.CloseModule(EngraveView.NAME);
            ProxyWindowModule.CloseWindowOnlyMsgPanel();
        }
        private static void SaleBtn_UIButtonClick()
        {
            var time = SystemTimeManager.Instance.GetUTCTimeStamp();
            var sevenDay = 7 * 24 * 60 * 60 * 1000;//7天=  毫秒
            bool isPush = true;
            if (string.IsNullOrEmpty(PlayerPrefs.GetString("SevenDayNoPushTips")))
                isPush = true;
            else if (double.Parse(PlayerPrefs.GetString("SevenDayNoPushTips")) - time < sevenDay)
                isPush = false;

            if (!isPush)
                OnSellMedallion();
            else
            {
                var ctrl = BuiltInDialogueViewController.OpenView("你是否确认出售选中的纹章？",
                null, OnSellMedallion,
                UIWidget.Pivot.Left);
                ctrl.SetIsPush(isPush, time);
            }
        }
        private static void EngraveBtn_UIButtonClick()
        {
            EngraveNetMsg.ReqEngraveRune(DataMgr._data.CurSelMedallionId, DataMgr._data.CurSelRuneIdForItemId);
        }
        private static void TipBtn_UIButtonClick()
        {
            ProxyTips.OpenTextTips(18, new Vector3(110, -172), true);
        }

        private static void OnSellMedallion()
        {
            //int id = DataMgr._data.CurSelMedallionId;
            //EngraveViewNetMsg.ReqSell(id);
            
            TipManager.AddTip("出售");
            //FireData();
        }
    }
}

