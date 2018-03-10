// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : Zijian
// Created  : 8/29/2017 8:21:33 PM
// **********************************************************************

using AppDto;
using System;
using System.Collections.Generic;
using UniRx;

public sealed partial class EquipmentMainDataMgr
{
    
    public static partial class EquipmentMainViewLogic
    {
        private static CompositeDisposable _disposable;

        public static void Open(EquipmentViewTab tab = EquipmentViewTab.Smith)
        {
        // open的参数根据需求自己调整
            var ctrl = EquipmentMainViewController.Show<EquipmentMainViewController>(
                EquipmentMainView.NAME
                , UILayerType.DefaultModule
                , true
                , true
                , Stream);
            DataMgr._data.SmithData.UpdateCurSmithItems();
            InitReactiveEvents(ctrl, tab);
        }
        
        private static void InitReactiveEvents(IEquipmentMainViewController ctrl, EquipmentViewTab tab)
        {
            if (ctrl == null) return;
            if (_disposable == null)
                _disposable = new CompositeDisposable();
            else
            {
                _disposable.Clear();
            }
            _disposable.Add(ctrl.OnCloseBtn_UIButtonClick.Subscribe(_ => OnCloseBtn_UIButtonClick()));
            _disposable.Add(ctrl.CloseEvt.Subscribe(_=>Dispose()));

            _disposable.Add( ctrl.TabbtnMgr.Stream.Subscribe(i => {
                SetCurTab((EquipmentViewTab)EquipmentMainData._TabInfos[i].EnumValue);
            }));

            _disposable.Add(PlayerModel.Stream.Subscribe(x =>
            {
                GameDebuger.Log("财富发生变动");
                DataMgr._data.SmithData.UpdateCurSmithItems();
                DataMgr._data.ResetData.UpdateCurSmith();
                FireData();
            }));
            
            ctrl.OnChildCtrlAdd += Ctrl_OnChildCtrlAdd;
            SetCurTab(tab);
        }

        /// <summary>
        /// 子页面在加载的时候再进行监听
        /// </summary>
        /// <param name="tab"></param>
        /// <param name="ctrl"></param>
        private static void Ctrl_OnChildCtrlAdd(EquipmentViewTab tab, IMonolessViewController ctrl)
        {
            switch(tab)
            {
                case EquipmentViewTab.Smith:
                    InitReactiveEvents((IEquipmentSmithController)ctrl);
                    break;
                case EquipmentViewTab.EquipmentReset:
                    InitReactiveEvents((IEquipmentResetController)ctrl);
                    break;
                case EquipmentViewTab.EquipmentEmbed:
                    InitReactiveEvents((IEquipmentEmbedController)ctrl);
                    break;
                case EquipmentViewTab.EquipmentMedallion:
                    InitReactiveEvents((IEquipmentInsetMedallionViewController)ctrl);
                    break;
            }
        }

        /// <summary>
        /// 打造界面逻辑
        /// </summary>
        /// <param name="ctrl"></param>
        private static void InitReactiveEvents(IEquipmentSmithController ctrl)
        {
            var SmithData = DataMgr._data.SmithData;
            //下拉选择职业
            _disposable.Add(ctrl.PopUp_factionCtrl.OnChoiceIndexStream.Subscribe(o=> {
                SmithData.CurSelectFaction = o.EnumValue;
                FireData();
            }));
            //下拉选择等级
            _disposable.Add(ctrl.PopUp_gradeCtrl.OnChoiceIndexStream.Subscribe(o =>
            {
                SmithData.CurSelcetGrade = o.EnumValue;
                FireData();
            }));
            //下拉选择质量
            _disposable.Add(ctrl.PopUp_quliatyCtrl.OnChoiceIndexStream.Subscribe(o =>
            {
                SmithData.CurSelectQuality = o.EnumValue;
                FireData();
            }));
            //快捷打造的按钮
            _disposable.Add(ctrl.OnClickFastSmithToggle.Subscribe(x => {
                if (SmithData.FastSmith == false)
                {
                    ProxyWindowModule.OpenConfirmWindow("便捷打造将直接消耗钻石进行打造，是否开启便捷打造？", 
                        okLabelStr:"确定",
                        onHandler: () =>
                        {
                            SmithData.FastSmith = true;
                            FireData();
                        },
                        cancelLabelStr:"取消");
                }
                else
                    SmithData.FastSmith = x;
                FireData();
            }));
            //点击选择装备
            _disposable.Add(ctrl.OnClickSmithCell.Subscribe(x => {
                var equipment = SmithData._curSmithCells[x];
                SmithData.curSelectEquipement = equipment;
                FireData();
            }));
            _disposable.Add(ctrl.OntipsBtn_UIButtonClick.Subscribe(x => {
                ProxyTips.OpenTextTips(11, new UnityEngine.Vector3(18, 37));
            }));
            _disposable.Add(ctrl.OnTipsBtn_UIButtonClick.Subscribe(_ =>
            {
                ProxyTips.OpenTextTips(41, new UnityEngine.Vector3(139, -204), true);
            }));
            //打造按钮
            _disposable.Add(ctrl.SmithHandler.Subscribe(x =>
            {
                if (DataMgr._data.CurrentEquipmentInfo.curSmithCount <= 0)
                {
                    TipManager.AddTip("今天打造已经达到50次，请明天再来打造");
                    return;
                }
                int id = SmithData.curSelectEquipement.id;
                int quality = SmithData.CurSelectQuality;
                bool fastSmith = SmithData.FastSmith;
                //如果为橙装 或者 红装，则不支持快捷打造
                fastSmith = fastSmith && (SmithData.CurSelectQuality == (int)AppItem.QualityEnum.ORANGE || SmithData.CurSelectQuality == (int)AppItem.QualityEnum.RED);
                if (ItemHelper.GetGeneralItemByItemId(id) != null && ItemHelper.GetGeneralItemByItemId(id) as Equipment != null)
                {
                    ExChangeHelper.CheckIsNeedExchange(AppVirtualItem.VirtualItemEnum.SILVER, (ItemHelper.GetGeneralItemByItemId(id) as Equipment).smithSilver,
                        () => EquipmentMainNetMsg.ReqEquipmentSmith(id, quality, fastSmith));
                }
                else
                    GameDebuger.Log("equipment未找到该装备");

            }));        
            FireData();
        }
        

        /// <summary>
        /// 洗练Event
        /// </summary>
        /// <param name="ctrl"></param>
        private static void InitReactiveEvents(IEquipmentResetController ctrl)
        {
            var ResetData = DataMgr._data.ResetData;

            //设置界面默认值
            ResetData.UpdateCurEquipmentItemList();


            //切换页
            _disposable.Add(ctrl.EquipmentChoiceCtrl.Tabbtn.Stream.Subscribe(i => {
                var tab = (EquipmentMainDataMgr.EquipmentHoldTab)EquipmentResetViewData.tabinfos[i].EnumValue;
                ResetData.CurTab = tab;
                ResetData.UpdateCurEquipmentItemList();
                FireData();
            }));
            _disposable.Add(ctrl.EquipmentChoiceCtrl.OnChoiceStream.Subscribe(i => {
                //var equipmentDto = ResetData.EquipmentItems[i];
                ResetData.CurChoiceEquipment = i;
                FireData();
            }));
            //洗练按钮
            _disposable.Add(ctrl.OnResetBtn_UIButtonClick.Subscribe(_ => {
                if (ResetData.CurChoiceEquipment == null)
                    return;

                if (ResetData.CurrentEquipmentInfo.curResetCount <= 0)
                {
                    TipManager.AddTip("今天洗炼已经达到50次，请明天再来打造");
                    return;
                }

                if(ResetData.CurChoiceEquipment.circulationType == (int)BagItemDto.CirculationType.Bind)
                {
                    var uid = ResetData.CurChoiceEquipment.equipUid;
                    var id = ResetData.CurChoiceEquipment.equipId;
                    if (ItemHelper.GetGeneralItemByItemId(id) != null && ItemHelper.GetGeneralItemByItemId(id) as Equipment != null)
                    {
                        ExChangeHelper.CheckIsNeedExchange(AppVirtualItem.VirtualItemEnum.SILVER, (ItemHelper.GetGeneralItemByItemId(id) as Equipment).resetSilver,
                            () => EquipmentMainNetMsg.ReqEquipmentReset(uid, ResetData.CurTab));
                    }
                }
                else
                {
                    ProxyWindowModule.OpenConfirmWindow("装备洗炼之后，将会绑定，是否确定洗炼？",
                        okLabelStr: "确定",
                        onHandler: () => {
                            var uid = ResetData.CurChoiceEquipment.equipUid;
                            var id = ResetData.CurChoiceEquipment.equipId;
                            if (ItemHelper.GetGeneralItemByItemId(id) != null && ItemHelper.GetGeneralItemByItemId(id) as Equipment != null)
                            {
                                ExChangeHelper.CheckIsNeedExchange(AppVirtualItem.VirtualItemEnum.SILVER, (ItemHelper.GetGeneralItemByItemId(id) as Equipment).resetSilver,
                                    () => EquipmentMainNetMsg.ReqEquipmentReset(uid, ResetData.CurTab));
                            }
                        },
                        cancelLabelStr: "取消"
                        );
                   // BuiltInDialogueViewController.OpenView(
                   //"装备洗炼之后，将会绑定，是否确定洗炼？",
                   //null,
                   //() =>
                   //{
                   //    var uid = ResetData.CurChoiceEquipment.equipUid;
                   //    EquipmentMainNetMsg.ReqEquipmentReset(uid, ResetData.CurTab);
                   //},
                   //UIWidget.Pivot.Left,
                   //"取消",
                   //"确定");
                }
               
                
            }));
            _disposable.Add(ctrl.OnResetContinue_UIButtonClick.Subscribe(_ => {
                var uid = ResetData.CurChoiceEquipment.equipUid;
                var id = ResetData.CurChoiceEquipment.equipId;
                if (ItemHelper.GetGeneralItemByItemId(id) != null && ItemHelper.GetGeneralItemByItemId(id) as Equipment != null)
                {
                    ExChangeHelper.CheckIsNeedExchange(AppVirtualItem.VirtualItemEnum.SILVER, (ItemHelper.GetGeneralItemByItemId(id) as Equipment).resetSilver,
                        () => EquipmentMainNetMsg.ReqEquipmentReset(uid, ResetData.CurTab));
                }
            }));
            //洗练保存按钮
            _disposable.Add(ctrl.OnResetSaveBtn_UIButtonClick.Subscribe(_ => {
                var uid = ResetData.CurChoiceEquipment.equipUid;
                EquipmentMainNetMsg.ReqEquipmentResetSave(uid, ResetData.CurTab);
            }));
            //tips按钮
            _disposable.Add(ctrl.OnTipBtn_1_UIButtonClick.Subscribe(_ => {
                TipManager.AddTopTip("Tips按钮");
            }));
            _disposable.Add(ctrl.OnTipBtn_2_UIButtonClick.Subscribe(_ =>
            {
                ProxyTips.OpenTextTips(12, new UnityEngine.Vector3(139, -204), true);
            }));

            FireData();
        }

        /// <summary>
        /// 宝石
        /// </summary>
        /// <param name="ctrl"></param>
        private static void InitReactiveEvents(IEquipmentEmbedController ctrl)
        {
            var EmbedData = DataMgr._data.EmbedData;
            //点击装备选择
            _disposable.Add(ctrl.OnEquipmentChoiceStream.Subscribe(x=> {
                EmbedData.CurChoicePartVo = x;                 
            }));
            //点击宝石孔
            _disposable.Add(ctrl.OnClickEmbedHole.Subscribe(x => {
                if(!x.isOpen)
                {
                    TipManager.AddTopTip(string.Format("需要{0}级才能解锁此宝石孔",x.holeInfo.openGrade));
                    return;
                }
                var lastChoice = EmbedData.CurChoiceHoleVo;
                EmbedData.CurChoiceHoleVo = x;

                //如果已经有宝石，则弹出点击脱下
                if (x.embedid != -1 && lastChoice == x)
                {
                    var part = EmbedData.CurChoicePartVo.part;
                    var pos = EmbedData.CurChoiceHoleVo.holeInfo.holePos;
                    EquipmentMainNetMsg.ReqEmbed_Off(x.dto,part);
                    return;
                }

               
                //TipManager.AddTopTip("点击宝石孔 :" + x.holeInfo.holePos);
                FireData();
            }));
            //点击选择宝石道具
            _disposable.Add(ctrl.OnClickEmbedItem.Subscribe(x => {
                var part = EmbedData.CurChoicePartVo.part;
                var pos = EmbedData.CurChoiceHoleVo.holeInfo.holePos;
                EquipmentMainNetMsg.ReqEmbed_In(x.id, part, pos);
            }));
        }

        #region  纹章

        /// <summary>
        /// 纹章Event
        /// </summary>
        /// <param name="ctrl"></param>
        private static void InitReactiveEvents(IEquipmentInsetMedallionViewController ctrl)
        {
            var MedallionData = DataMgr._data.MedallionData;
            //切换页
            _disposable.Add(ctrl.EquipmentChoiceCtrl.Tabbtn.Stream.Subscribe(i =>
            {
                var tab = (EquipmentMainDataMgr.EquipmentHoldTab)EquipmentResetViewData.tabinfos[i].EnumValue;
                MedallionData.CurTab = tab;
                MedallionData.UpdateCurEquipmentItemList();
                FireData();
            }));

            _disposable.Add(ctrl.EquipmentChoiceCtrl.OnChoiceStream.Subscribe(dto =>
            {
                MedallionData.CurChoiceEquipment = dto;
                FireData();
            }));

            _disposable.Add(ctrl.SmithItemCellCtrl.OnSmithItem_UIButtonClick.Subscribe(_ => {
                
                MedallionData.isOpenMedallionPanel = true;
                MedallionData.UpdateCurMedallion();
                FireData();
            }));
            _disposable.Add(ctrl.SmithItemCellCtrl.itemCellCtrl.OnCellClick.Subscribe(_ => {
               
                MedallionData.isOpenMedallionPanel = true;
                MedallionData.UpdateCurMedallion();
                FireData();
            }));
            //_disposable.Add(ctrl.OnItemBtn_UIButtonClick.Subscribe(_ =>
            //{
            //    MedallionData.isOpenMedallionPanel = !MedallionData.isOpenMedallionPanel;
            //    MedallionData.UpdateCurMedallion();
            //    FireData();
            //}));

            _disposable.Add(ctrl.OnMedallionPenelIsOpenStream.Subscribe(isOpen =>
            {
                MedallionData.isOpenMedallionPanel = isOpen;
                FireData();
            }));

            _disposable.Add(ctrl.OnMedallionPanelInsetStream.Subscribe(item =>
            {
                //MedallionData.isOpenMedallionPanel = false;
                MedallionData.SelMedallionId = item.id;
                FireData();
            }));

            _disposable.Add(ctrl.OnMakeBtn_UIButtonClick.Subscribe(_ =>
            {
                if (BackpackDataMgr.DataMgr.GetMedallionItems() == null || BackpackDataMgr.DataMgr.GetMedallionItems().ToList().Count <=0)
                {
                    ProxyWindowModule.OpenConfirmWindow("身上没有纹章可制作，是否前往购买纹章？", 
                        onHandler: () =>
                        {
                            ProxyShop.OpenShop(ShopTypeTab.ArenaShop, ShopTypeTab.ArenaScroeShopId);
                        }, okLabelStr: "前往购买", cancelLabelStr: "取消");

                    return;
                }
                ProxyEquipmentMain.OpenEngraveView();
            }));

            _disposable.Add(ctrl.OnInsetBtn_UIButtonClick.Subscribe(_ =>
            {
                if(MedallionData.EquipmentItems.ToList().IsNullOrEmpty())
                {
                    TipManager.AddTip("请选择一件装备进行操作。");
                    return;
                }
                if(MedallionData.CurChoiceEquipment.circulationType == (int)BagItemDto.CirculationType.Bind)
                {
                    OnInsetIsHad();
                }
                else
                {
                    ProxyWindowModule.OpenConfirmWindow("您当前选中装备为非绑定装备，执行操作后，装备将变为绑定，是否继续？",
                        onHandler:()=> {
                            OnInsetIsHad();
                        },okLabelStr: "确定", cancelLabelStr:"取消");
                }
            }));

            _disposable.Add(ctrl.OnTipsBtn_UIButtonClick.Subscribe(_ =>
            {
                ProxyTips.OpenTextTips(13, new UnityEngine.Vector3(87, -205), true);
            }));
        }

        private static void OnInsetIsHad()
        {
            var MedallionData = DataMgr._data.MedallionData;
            if (MedallionData.CurChoiceEquipment.property.medallion != null)
                ProxyWindowModule.OpenConfirmWindow("选中装备已有镶嵌纹章效果，重新镶嵌纹章将会覆盖原有镶嵌纹章效果，是否继续操作？",
                    onHandler: () =>
                    {
                        OnInsetReq();
                    }, okLabelStr: "确定", cancelLabelStr: "取消");
            else
                OnInsetReq();
        }
        private static void OnInsetReq()
        {
            var MedallionData = DataMgr._data.MedallionData;
            if (MedallionData.CurTab == EquipmentMainDataMgr.EquipmentHoldTab.Equip)
                EquipmentMainNetMsg.ReqInsetMedallionInWear(MedallionData.SelMedallionId, MedallionData.CurChoiceEquipment.partType);
            else if (MedallionData.CurTab == EquipmentMainDataMgr.EquipmentHoldTab.Bag)
                EquipmentMainNetMsg.ReqInsetMedallionInBag(MedallionData.SelMedallionId, MedallionData.CurChoiceEquipment.equipUid);
        }

        #endregion


        private static void SetCurTab(EquipmentViewTab i)
        {
            DataMgr._data.CurTab = i;
            FireData();
        }
        private static void OnCloseBtn_UIButtonClick()
        {
            UIModuleManager.Instance.CloseModule(EquipmentMainView.NAME);
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

    
    }
}

