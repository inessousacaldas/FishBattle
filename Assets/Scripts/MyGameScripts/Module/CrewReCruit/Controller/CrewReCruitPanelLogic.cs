// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : DM-PC092
// Created  : 11/20/2017 5:02:26 PM
// **********************************************************************

using AppDto;
using System.Collections.Generic;
using UniRx;

public sealed partial class CrewReCruitDataMgr
{
    
    public static partial class CrewReCruitPanelLogic
    {
        private static CompositeDisposable _disposable;
        public static void Open()
        {
        // open的参数根据需求自己调整
            var ctrl = CrewReCruitPanelController.Show<CrewReCruitPanelController>(
                CrewReCruitPanel.NAME
                , UILayerType.DefaultModule
                , true
                , true
                , Stream);

            InitReactiveEvents(ctrl);
        }
        
        private static void InitReactiveEvents(ICrewReCruitPanelController ctrl)
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
            int mNormalOneCast =  tCrewDataDic[0].virtualItemCount;
            int mSeniorOneCase = tCrewDataDic[2].virtualItemCount;
            _disposable.Add(ctrl.OnSeniorOneButton_UIButtonClick.Subscribe(_=>SeniorOneButton_UIButtonClick(mSeniorOneCase)));
            _disposable.Add(ctrl.OnNormalOneButton_UIButtonClick.Subscribe(_=>NormalOneButton_UIButtonClick(mNormalOneCast)));
            _disposable.Add(ctrl.OnCloseButton_UIButtonClick.Subscribe(_ => OnCloseButton_UIButtonClick()));
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
        private static void SeniorOneButton_UIButtonClick(int cost)
        {
            int mSeniorOneCase =  cost;
            ProxyWindowModule.OpenConfirmWindow("是否消费" + mSeniorOneCase + "钻石，获得招募契约，并且马上使用招募契约","伙伴招劵",delegate
            {
                CrewReCruitNetMsg.AddCrewSend((int)AppDto.CrewRecruitType.CrewRecruitTypeEnum.DiamondOnce,false);
            },null,UIWidget.Pivot.Left,null,null);
        }
        private static void SeniorTenButton_UIButtonClick(int cost)
        {
            int mSeniorOneCase = cost;
            ProxyWindowModule.OpenConfirmWindow("是否消费" + mSeniorOneCase * 10 + "钻石，获得10张招募契约，并且马上使用招募契约","伙伴招劵",delegate
            {
                CrewReCruitNetMsg.AddCrewSend((int)AppDto.CrewRecruitType.CrewRecruitTypeEnum.DiamondTenTimes,false);
            },null,UIWidget.Pivot.Left,null,null);
        }
        private static void NormalTenButton_UIButtonClick(int cost)
        {
            int mNormalOneCast = cost;
            ProxyWindowModule.OpenConfirmWindow("是否消费" + mNormalOneCast *10 + "金币，获得10张招募契约，并且马上使用招募契约","伙伴招募",delegate
            {
                ExChangeHelper.CheckIsNeedExchange(AppVirtualItem.VirtualItemEnum.GOLD,mNormalOneCast,() => CrewReCruitNetMsg.AddCrewSend((int)AppDto.CrewRecruitType.CrewRecruitTypeEnum.GoldTenTimes,false));
            },null,UIWidget.Pivot.Left,null,null);
        }
        private static void NormalOneButton_UIButtonClick(int cost)
        {
            int mNormalOneCast =  cost;
            ProxyWindowModule.OpenConfirmWindow("是否消费" + mNormalOneCast  + "金币，获得招募契约，并且马上使用招募契约","伙伴招募",delegate
            {
                ExChangeHelper.CheckIsNeedExchange(AppVirtualItem.VirtualItemEnum.GOLD,mNormalOneCast,() => CrewReCruitNetMsg.AddCrewSend((int)AppDto.CrewRecruitType.CrewRecruitTypeEnum.GoldOnce,false));
            },null,UIWidget.Pivot.Left,null,null);
        }

        private static void OnCloseButton_UIButtonClick()
        {
            ProxyCrewReCruit.Close();
        }
    }
}

