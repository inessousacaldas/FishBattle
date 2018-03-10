// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : xush
// Created  : 7/27/2017 7:53:13 PM
// **********************************************************************

using System;
using System.Collections.Generic;
using AppDto;
using UniRx;
using UnityEngine;

public sealed partial class CrewViewDataMgr
{

    public static partial class CrewMainViewLogic
    {
        private static CompositeDisposable _disposable;
        private static CompositeDisposable _trainDisposable;
        public static void Open()
        {
            if (!FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum.FUN_10, true)) return;

            // open的参数根据需求自己调整
            var ctrl = CrewMainViewController.Show<CrewMainViewController>(
                CrewMainView.NAME
                , UILayerType.DefaultModule
                , true
                , true
                , Stream);

            InitReactiveEvents(ctrl);
        }

        private static void InitReactiveEvents(ICrewMainViewController ctrl)
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
            _disposable.Add(ctrl.OnPullBtn_UIButtonClick.Subscribe(_ =>
            {
                ctrl.ShowBackground(false);
                ctrl.PullExtendPanel(true);
            }));
            _disposable.Add(ctrl.OnPullbackBtn_UIButtonClick.Subscribe(_ =>
            {
                ctrl.PullExtendPanel(false);
                ctrl.ShowBackground(true);
                ctrl.UpdateView(DataMgr._data.GetCurCrewTab);
                
            }));
            _disposable.Add(ctrl.OnCrewVoiceBtn_UIButtonClick.Subscribe(_=>CallBtn_UIButtonClick()));
            _disposable.Add(ctrl.OnCrewTalkBtn_UIButtonClick.Subscribe(_=>CommentBtn_UIButtonClick()));
            _disposable.Add(ctrl.OnFormationBtn_UIButtonClick.Subscribe(_ => FormationBtn_UIButtonClick()));
            _disposable.Add(ctrl.OnFollowBtn_UIButtonClick.Subscribe(_=>FollowBtn_UIButtonClick()));
            _disposable.Add(ctrl.OnAddExpBtn_UIButtonClick.Subscribe(_=>AddExpBtn_UIButtonClick()));
            _disposable.Add(ctrl.OnOtherAddExpBtn_UIButtonClick.Subscribe(_=>OtherAddExpBtn_UIButtonClick()));
            _disposable.Add(ctrl.OnChangeNameBtn_UIButtonClick.Subscribe(_ => OnChangeNameBtn_UIButtonClick()));
            _disposable.Add(ctrl.OnCrewAddBtn_UIButtonClick.Subscribe(_ => OnCrewAddBtn_UIButtonClick()));
            _disposable.Add(ctrl.OnShowTypeBtn_UIButtonClick.Subscribe(_ => { ctrl.OpenShowTypeList(); }));
            _disposable.Add(ctrl.OnpageSprite_UIButtonClick.Subscribe(_ =>
            {
                DataMgr._data.UpdateInfoTab(CrewInfoTab.InfoTab);
                ctrl.ChangeInfoTab(CrewInfoTab.InfoTab);
            }));
            _disposable.Add(ctrl.OnpageSprite_1_UIButtonClick.Subscribe(_ =>
            {
                DataMgr._data.UpdateInfoTab(CrewInfoTab.FetterTab);
                ctrl.ChangeInfoTab(CrewInfoTab.FetterTab);
            }));

            #region 伙伴进阶强化
            _disposable.Add(ctrl.IStrengthenCtrl.OnCheckBtn_UIButtonClick.Subscribe(_ => ctrl.IStrengthenCtrl.OnCheckDetailBtnClick()));
            _disposable.Add(ctrl.IStrengthenCtrl.OnStrengthButton_UIButtonClick.Subscribe(_ => StrengthButton_UIButtonClick()));
            _disposable.Add(ctrl.IStrengthenCtrl.OnTipButton_UIButtonClick.Subscribe(_ => ctrl.IStrengthenCtrl.OnDevelopTipBtnClick()));
            _disposable.Add(ctrl.IStrengthenCtrl.OnDevelopButton_UIButtonClick.Subscribe(_ => DevelopButton_UIButtonClick()));
            _disposable.Add(ctrl.IStrengthenCtrl.GetEnterSaveHandler.Subscribe(_ => OnSureBtnClcik()));
            _disposable.Add(ctrl.IStrengthenCtrl.OnStrengthTipBtn_UIButtonClick.Subscribe(_ => ctrl.IStrengthenCtrl.StrengthTips()));
            _disposable.Add(ctrl.TrainSaveBtnUp.Subscribe(_ => OnSaveBtnClick(_)));
            _disposable.Add(ctrl.TrainBtnUp.Subscribe(_ => OnTrainingBtnClick()));

            ctrl.IStrengthenCtrl.OnTabChange(CrewStrengthenTab.Phase, DataMgr._data.GetCrewSkillTrainData());
            _disposable.Add(ctrl.IStrengthenCtrl.TabMgr.Stream.Subscribe(e =>
            {
                ctrl.IStrengthenCtrl.OnTabChange((CrewStrengthenTab)e, DataMgr._data.GetCrewSkillTrainData());
            }));
            #endregion

            _disposable.Add(ctrl.OnExtendCloseBtn_UIButtonClick.Subscribe(_=> CloseBtn_UIButtonClick()));
            _disposable.Add(ctrl.GetTabMgr.Stream.Subscribe(pageIdx =>
            {
                DataMgr._data.SetCurCrewTab((PartnerTab)pageIdx);
                var needInit = ctrl.UpdateView((PartnerTab)pageIdx);
                if (needInit)
                {
                    var skillCtrl = ctrl.ICrewSkillViewCrrl;
                    CrewSkillDataMgr.CrewSkillViewLogic.InitReactiveEvents(skillCtrl);
                }
            }));

            _disposable.Add(ctrl.GetIconClickEvt.Subscribe(data =>
            {
                DataMgr._data.SetCurCrewId(data.GetId, data.GetCrewId);

                var crewDto = DataMgr._data.IsHadCurPantner(data.GetId);
                if (crewDto == null)
                {
                    DataMgr._data.UpdateInfoTab(CrewInfoTab.InfoTab);
                    DataMgr._data.SetCurCrewTab(PartnerTab.Info);
                    ctrl.UpdateView(PartnerTab.Info);
                }

                if (crewDto != null && crewDto.fetterDto != null && crewDto.fetterDto.Count > 0)
                {
                    DataMgr._data.SetCurCrewFetterId(crewDto.fetterDto[0].crewFetterId);
                    DataMgr._data.SetNextPhaseAndRaise(crewDto.phase, crewDto.raise);
                }
                ctrl.GetCrewInfoIdx(data.GetIdx);
                //当选中伙伴未拥有时,隐藏伙伴技能,伙伴培养,伙伴好感度三个标签页
                ctrl.PullExtendPanel(false);
                ctrl.ShowBackground(true);
                ctrl.HideTabBtn(crewDto == null);

                FireData();
            }));

            _disposable.Add(ctrl.IGreIconScrollEvt.Subscribe(index =>
            {
                CrewIconController cc=ctrl.GetCurrCrewData(index);
                if(cc != null)
                {
                    DataMgr._data.SetCurCrewId(cc.GetCrewId,cc.GetUid);

                    var crewDto = DataMgr._data.IsHadCurPantner(cc.GetCrewId);
                    if(crewDto == null)
                    {
                        DataMgr._data.UpdateInfoTab(CrewInfoTab.InfoTab);
                        DataMgr._data.SetCurCrewTab(PartnerTab.Info);
                        ctrl.UpdateView(PartnerTab.Info);
                    }

                    if(crewDto != null && crewDto.fetterDto != null && crewDto.fetterDto.Count > 0)
                    {
                        DataMgr._data.SetCurCrewFetterId(crewDto.fetterDto[0].crewFetterId);
                        DataMgr._data.SetNextPhaseAndRaise(crewDto.phase,crewDto.raise);
                    }


                    //当选中伙伴未拥有时,隐藏伙伴技能,伙伴培养,伙伴好感度三个标签页
                    ctrl.HideTabBtn(crewDto == null);

                    FireData();
                }
            }));

            _disposable.Add(ctrl.ICrewFetterCtrl.OnClickCrewFetterStream.Subscribe(data =>
            {
                DataMgr._data.SetCurCrewFetterId(data.CrewFetterId);
                FireData();
            }
            ));
            _disposable.Add(ctrl.ICrewFetterCtrl.OnactivateBtn_UIButtonClick.Subscribe(x =>
            {
                long tempUId = DataMgr._data.GetCurCrewUId;
                var currentCrewFetterVo = DataMgr._data.GetCurCrewFetterVo;
                if(currentCrewFetterVo != null)
                    CrewViewNetMsg.CrewFetterActive(tempUId, currentCrewFetterVo.CrewFetterId, !currentCrewFetterVo.Acitve);
            }));
            _disposable.Add(ctrl.ICrewFetterCtrl.OnCrewFetterTipBtn_UIButtonlick.Subscribe(x =>
            {
                ProxyTips.OpenTextTips(4, new Vector3(174, 95, 0));
            }));

            _disposable.Add(ctrl.GetPageChange.Subscribe(pageIdx =>
            {
                DataMgr._data.UpdateInfoTab((CrewInfoTab)pageIdx);
                ctrl.UpdateInfoTabState((CrewInfoTab)pageIdx);
            }));
            _disposable.Add(ctrl.GetChangeCrewType.Subscribe(type =>
            {
                var data = DataMgr._data.GetCrewByType(type);
                DataMgr._data.SetCurCrewId(data.GetCrewId, data.GetInfoDto == null ? 0 : data.GetInfoDto.id);
                var crewDto = data.GetInfoDto;
                if (crewDto != null && crewDto.fetterDto != null && crewDto.fetterDto.Count > 0)
                {
                    DataMgr._data.SetCurCrewFetterId(crewDto.fetterDto[0].crewFetterId);
                    DataMgr._data.SetNextPhaseAndRaise(crewDto.phase, crewDto.raise);
                }
                
                //当选中伙伴未拥有时,隐藏伙伴技能,伙伴培养,伙伴好感度三个标签页
                ctrl.GetCrewInfoId(data.GetCrewId);
                ctrl.HideTabBtn(crewDto == null);
                ctrl.UpdateInfoTabState(CrewInfoTab.InfoTab);

                if (crewDto == null)
                {
                    DataMgr._data.UpdateInfoTab(CrewInfoTab.InfoTab);
                    DataMgr._data.SetCurCrewTab(PartnerTab.Info);
                    //ctrl.UpdateView(PartnerTab.Info);
                }
                FireData();
            }));

            #region 伙伴好感度
            _disposable.Add(ctrl.OnFavorableBtn_UIButtonClick.Subscribe(_ =>
            {
                var idx = DataMgr._data.GetSelfCrew().FindElementIdx(d => d.crewId == DataMgr._data.GetCurCrewId);
                DataMgr._data.UpdateCurFavorableIdx(idx);
                CrewProxy.OpenCrewFavorableView();
            }));
            _disposable.Add(ctrl.OnMoreOperationBtn_UIButtonClick.Subscribe(_ =>
            {
                ctrl.OnMoreOperationBtnClick();
            }));
            _disposable.Add(ctrl.OnCrewNameBtn_UIButtonClick.Subscribe(_ =>
            {

            }));

            #endregion
            #region 羁绊
            _disposable.Add(ctrl.OnCrewFetterItemClick.Subscribe(data =>
            {
                DataMgr._data.SetCurCrewFetterId(data.CrewFetterId);
                FireData();
            }));
            _disposable.Add(ctrl.OnCrewFetterItemActiveClick.Subscribe(_ =>
            {
                long tempUId = DataMgr._data.GetCurCrewUId;
                var currentCrewFetterVo = DataMgr._data.GetCurCrewFetterVo;
                if (currentCrewFetterVo != null)
                    CrewViewNetMsg.CrewFetterActive(tempUId, currentCrewFetterVo.CrewFetterId, !currentCrewFetterVo.Acitve);
            }));
            #endregion
        }

        #region---------------------------------------------------------研修---------------------------------------------------------
        
        private static void OnSaveBtnClick(IStrengthContainerViewController strCtrl)
        {
            strCtrl.ShowWindows(true,DataMgr._data.GetCrewSkillTrainData(),DataMgr._data.GetCurCrewId);
        }
        private static void OnSureBtnClcik()
        {
            CrewViewNetMsg.ReqSkillTrainSave(DataMgr._data.GetCurCrewUId);
        }
        private static void OnTrainingBtnClick()
        {
            var info = DataMgr._data.GetCrewSkillTrainData().GetTrainList(DataMgr.GetCurCrewID);
            var consumeID = DataCache.GetStaticConfigValue(AppStaticConfigs.CREW_TRAINNING_CONSUME_ITEM);
            var crew = DataMgr._data.GetCrewDataById(DataMgr.GetCurCrewID);
            var rare = crew.rare;
            if (rare != -1)
            {
                var training = DataMgr._data.GetCrewSkillTrainData().GetCrewTraining(rare);
                if (BackpackDataMgr.DataMgr.GetItemCountByItemID(consumeID) >= training.consume)
                {
                    if (info.aftMaxLevel || info.befMaxLevel)
                    {
                        TipManager.AddTip("伙伴的战技和成长率已研修到最高境界，无需继续研修");
                    }
                    else
                    {
                        CrewViewNetMsg.ReqSkillTrain(DataMgr._data.GetCurCrewUId);
                    }
                }
                else
                {
                    var item = ItemHelper.GetGeneralItemByItemId(consumeID);
                    if (item != null)
                    {
                        GainWayTipsViewController.OpenGainWayTip(consumeID, new Vector3(164, -46));
                        TipManager.AddTip(item.name + "不足，无法研修");
                    }
                }
            }
        }

        #endregion

        private static void Dispose()
        {
            _disposable = _disposable.CloseOnceNull();
            OnDispose();
        }

        // 如果有自定义的内容需要清理，在此实现
        private static void OnDispose()
        {
            _trainDisposable = _trainDisposable.CloseOnceNull();
            CrewSkillDataMgr.CrewSkillViewLogic.Dispose();
            DataMgr._data.SetCurCrewTab(PartnerTab.Info);   //默认恢复属性界面
            DataMgr._data.UpdateInfoTab(CrewInfoTab.InfoTab);//默认恢复属性界面
        }

        private static void CloseBtn_UIButtonClick()
        {
            UIModuleManager.Instance.CloseModule(CrewMainView.NAME);
        }
        
        private static void CallBtn_UIButtonClick()
        {
            TipManager.AddTip("===打开喊话界面===");
        }
        
        private static void CommentBtn_UIButtonClick()
        {
            TipManager.AddTip("===打开评论界面===");
        }
        
        private static void FormationBtn_UIButtonClick()
        {
            //TipManager.AddTip("===敬请期待===");
            ProxyFormation.OpenCrewFormation();
            CloseBtn_UIButtonClick();
        }
        
        private static void FollowBtn_UIButtonClick()
        {
            CrewViewNetMsg.Crew_FollowChange(DataMgr._data.GetCurCrewUId);
        }
        
        private static void AddExpBtn_UIButtonClick()
        {
            CrewProxy.OpenCrewUpGradeView();
        }
        private static void OtherAddExpBtn_UIButtonClick()
        {
            TipManager.AddTip("==增加经验==");
        }

        private static void OnChangeNameBtn_UIButtonClick()
        {
            TipManager.AddTip("====选择名称===");
        }

        private static void CheckBtn_UIButtonClick()
        {
            TipManager.AddTip("查看详情");
        }
        private static void StrengthButton_UIButtonClick()
        {
            CrewViewNetMsg.Crew_Raise(DataMgr._data.GetCurCrewUId,DataMgr._data.GetCurCrewId);
        }
        private static void TipButton_UIButtonClick()
        {
            ProxyTips.OpenTextTips(6, new Vector3(174, 125, 0));
        }
        private static void DevelopButton_UIButtonClick()
        {
            CrewViewNetMsg.Crew_Phase(DataMgr._data.GetCurCrewUId,DataMgr._data.GetCurCrewId);
            GameDebuger.Log(DataMgr._data.GetCurCrewUId);
        }

        private static void OnDevelopEffectBtn_UIButtonClick()
        {
            TipManager.AddTip("查看进阶效果");
        }
        private static void OnCrewAddBtn_UIButtonClick()
        {
            CrewViewNetMsg.Crew_Add(DataMgr._data.GetCurCrewId);
        }

        public static void FireDatas()
        {
            FireData();
        }
    }
}

