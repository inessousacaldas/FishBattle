// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : DM-PC092
// Created  : 11/25/2017 10:53:42 AM
// **********************************************************************

using AppDto;
using UniRx;
using System.Collections.Generic;

public sealed partial class CrewObtainPanelDataMgr
{
    
    public static partial class CrewObtainPanelLogic
    {
        private static CompositeDisposable _disposable;

        public static void Open(List<CrewInfoDto> tCrewInfoDtoList,List<CrewChipDto> tCrewChipDtoList,int tBuyType)
        {
        // open的参数根据需求自己调整
            var ctrl = CrewObtainPanelController.Show<CrewObtainPanelController>(
                CrewObtainPanel.NAME
                , UILayerType.DefaultModule
                , true
                , true
                ,null);
            ctrl.Open(tCrewInfoDtoList,tCrewChipDtoList,tBuyType);
            InitReactiveEvents(ctrl,tBuyType);
        }
        
        private static void InitReactiveEvents(ICrewObtainPanelController ctrl,int tBuyType)
        {
            if (ctrl == null) return;
            if (_disposable == null)
                _disposable = new CompositeDisposable();
            else
            {
                _disposable.Clear();
            }
        
            _disposable.Add(ctrl.CloseEvt.Subscribe(_=>Dispose()));
            List<CrewRecruitType> tCrewDataDic = DataCache.getArrayByCls<CrewRecruitType>();
            int tCast =  tCrewDataDic[tBuyType - 1].virtualItemCount;
            _disposable.Add(ctrl.OnOneBuy_UIButtonClick.Subscribe(_=>OneBuy_UIButtonClick(tBuyType,tCast)));
            _disposable.Add(ctrl.OnCloseBtn_UIButtonClick.Subscribe(_=>CloseBtn_UIButtonClick()));
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
        private static void OneBuy_UIButtonClick(int BuyType,int cost)
        {
            ProxyWindowModule.OpenConfirmWindow("是否再次购买","伙伴招募",delegate
            {
                if(BuyType < 3)
                {
                    ExChangeHelper.CheckIsNeedExchange(AppVirtualItem.VirtualItemEnum.GOLD,cost,() => CrewReCruitDataMgr.CrewReCruitNetMsg.AddCrewSend(BuyType,false));
                }
                else {
                    CrewReCruitDataMgr.CrewReCruitNetMsg.AddCrewSend(BuyType,false);
                }
            },null,UIWidget.Pivot.Left,null,null);

        }
        private static void CloseBtn_UIButtonClick()
        {
            ProxyCrewReCruit.CloseCrewObtainPanel();
        }
    }
}

