// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : DM-PC092
// Created  : 12/13/2017 4:15:20 PM
// **********************************************************************

using AppDto;
using AppServices;
using UniRx;

public sealed partial class TreasureMissionDataMgr
{
    
    public static partial class TreasurePanelLogic
    {
        private static CompositeDisposable _disposable;

        public static void Open()
        {
        // open的参数根据需求自己调整
            var ctrl = TreasurePanelController.Show<TreasurePanelController>(
                TreasurePanel.NAME
                , UILayerType.DefaultModule
                , true
                , true
                , Stream);
            InitReactiveEvents(ctrl);
        }
        
        private static void InitReactiveEvents(ITreasurePanelController ctrl)
        {
            if (ctrl == null) return;
            if (_disposable == null)
                _disposable = new CompositeDisposable();
            else
            {
                _disposable.Clear();
            }
        
            _disposable.Add(ctrl.CloseEvt.Subscribe(_=>Dispose()));
            _disposable.Add(ctrl.OnTreasureBtn_UIButtonClick.Subscribe(_=>TreasureBtn_UIButtonClick()));
            _disposable.Add(ctrl.OnOnClose_UIButtonClick.Subscribe(_=>OnClose_UIButtonClick()));
           
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
        private static void TreasureBtn_UIButtonClick()
        {
            int limit= DataCache.getDtoByCls<DailyLimit>((int)DailyLimit.DailyFuncion.DAILY_15).limit;
            bool number = DataMgr._data.GetTreasureNumber() < limit;
            bool isHasItem = DataMgr._data.ItemNumberBool();
            string tipErr = "道具数量不足";
            if(!number)
                tipErr = "今天寻宝次数已达上限";
            if(number && isHasItem)
                BackpackDataMgr.BackPackNetMsg.BackpackApply(DataMgr._data.GetBagIndex(),1);
            else
                TipManager.AddTip(tipErr);
        }
        private static void OnClose_UIButtonClick()
        {
            Close();
        }

        public static void Close()
        {
            UIModuleManager.Instance.CloseModule(TreasurePanel.NAME);
        }


    }
}

