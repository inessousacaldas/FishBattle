// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : xjd
// Created  : 9/20/2017 7:57:58 PM
// **********************************************************************

using UniRx;
using System.Collections.Generic;
using AppDto;
using VirtualItemEnum = AppDto.AppVirtualItem.VirtualItemEnum;

public sealed partial class AssistSkillMainDataMgr
{
    
    public static partial class AssistSkillMainViewLogic
    {
        public static Dictionary<int, string> AssistSkillIdToName = new Dictionary<int, string>
        {
            {1, "携带料理" },
            {2, "大盘料理"},
            {3, "导力技术" }
        };

        private static CompositeDisposable _disposable;

        public static void Open()
        {
            if(FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum.FUN_69))
            {
                AssistSkillMainNetMsg.ReqCrewShortInfo();
                AssistSkillMainNetMsg.ReqDelegateMissionMsg();
            }
        // open的参数根据需求自己调整
	    var layer = UILayerType.DefaultModule;
            var ctrl = AssistSkillMainViewController.Show<AssistSkillMainViewController>(
                AssistSkillMainView.NAME
                , layer
                , true
                , true
                , Stream);
            InitReactiveEvents(ctrl);
        }
        
        private static void InitReactiveEvents(IAssistSkillMainViewController ctrl)
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

            _disposable.Add(ctrl.TabbtnMgr.Stream.Subscribe(i =>
            {
                SetRightCurTab((RightViewTab)AssistSkillMainData._RightViewTabInfos[i].EnumValue);
            }));
            
            ctrl.OnChildCtrlAdd += Ctrl_OnChildCtrlAdd;
            SetRightCurTab(RightViewTab.AssistSkillView);
            
            //未学习技能时 打开界面默认到初次选择界面
            if (DataMgr._data.SkillId <= 0)
                DataMgr._data.CurTab = AssistViewTab.ChooseView;
        }

        //右侧大标签界面选择
        private static void SetRightCurTab(RightViewTab i)
        {
            DataMgr._data.CurRightTab = i;
            if(i == RightViewTab.AssistSkillView && !DataMgr._data.IsResp)
            {
                AssistSkillMainNetMsg.ReqAssistInfo();
                return;
            } 
            else if (i == RightViewTab.AssistDelegateView)
            {
                DataMgr._data.CurDelegateTab = AssistViewTab.AssistDelegateMain;
            }

            FireData();
        }

        #region 添加监听
        private static void Ctrl_OnChildCtrlAdd(AssistViewTab tab, IMonolessViewController ctrl)
        {
            switch (tab)
            {
                case AssistViewTab.ChooseView:
                    InitReactiveEvents((IAssistChooseViewController)ctrl);
                    break;
                case AssistViewTab.LearnCookView:
                    InitReactiveEvents((IAssistLearnCookViewController)ctrl);
                    break;
                case AssistViewTab.LearnForceView:
                    InitReactiveEvents((IAssistLearnForceViewController)ctrl);
                    break;
                case AssistViewTab.CookUpGradeView:
                    InitReactiveEvents((IAssistCookUpgradeViewController)ctrl);
                    break;
                case AssistViewTab.CookProductView:
                    InitReactiveEvents((IAssistCookProductViewController)ctrl);
                    break;
                case AssistViewTab.ForceUpGradeView:
                    InitReactiveEvents((IAssistForceUpgradeViewController)ctrl);
                    break;
                case AssistViewTab.ForceProductView:
                    InitReactiveEvents((IAssistForceProductViewController)ctrl);
                    break;
                case AssistViewTab.LearnBaseView:
                    InitReactiveEvents((IAssistLearnViewController)ctrl);
                    break;
                case AssistViewTab.AssistDelegateMain:
                    InitReactiveEvents((IAssistDelegateMainViewController)ctrl);
                    break;
            }
        }
        #endregion

        #region 初次选择技能界面
        private static void InitReactiveEvents(IAssistChooseViewController ctrl)
        {
            _disposable.Add(ctrl.OnCarryCook_UIButtonClick.Subscribe(_ =>
            {
                DataMgr._data.ChosedSkillId = (int)AssistSkill.AssistSkillEnum.CarryCook;
                DataMgr._data.CurTab = AssistViewTab.LearnCookView;
                FireData();
            }));

            _disposable.Add(ctrl.OnGrailCook_UIButtonClick.Subscribe(_ =>
            {
                DataMgr._data.ChosedSkillId = (int)AssistSkill.AssistSkillEnum.GrailCook;
                DataMgr._data.CurTab = AssistViewTab.LearnCookView;
                FireData();
            }));

            _disposable.Add(ctrl.OnLeadForce_UIButtonClick.Subscribe(_ =>
            {
                DataMgr._data.ChosedSkillId = (int)AssistSkill.AssistSkillEnum.LeadForceSkill;
                DataMgr._data.CurTab = AssistViewTab.LearnForceView;
                FireData();
            }));
        }
        #endregion

        //学习界面 baseview
        private static void InitReactiveEvents(IAssistLearnViewController ctrl)
        {
            _disposable.Add(ctrl.TabbtnMgr.Stream.Subscribe(i =>
            {
                SetCurRightUpTab((RightUpTab)AssistSkillMainData._TabInfos[i].EnumValue);
            }));
        }

        #region 已学习技能后的 升级/生产界面
        private static void SetCurRightUpTab(RightUpTab i)
        {
            //进入界面默认选择配方id
            Dictionary<int, AssistSkillMakeConsume> clsConsumeData = DataCache.getDicByCls<AssistSkillMakeConsume>();
            DataMgr._data.CurRecipeId = clsConsumeData.Find(item => item.Value.skillId == DataMgr._data.SkillId).Value.id;

            if (DataMgr._data.SkillId == (int)AssistSkill.AssistSkillEnum.CarryCook
                || DataMgr._data.SkillId == (int)AssistSkill.AssistSkillEnum.GrailCook)
            {
                if (i == RightUpTab.Learn)
                    DataMgr._data.CurTab = AssistViewTab.CookUpGradeView;
                else if (i == RightUpTab.Product)
                    DataMgr._data.CurTab = AssistViewTab.CookProductView;
            }
            else if(DataMgr._data.SkillId == (int)AssistSkill.AssistSkillEnum.LeadForceSkill)
            {
                if (i == RightUpTab.Learn)
                    DataMgr._data.CurTab = AssistViewTab.ForceUpGradeView;
                else if (i == RightUpTab.Product)
                    DataMgr._data.CurTab = AssistViewTab.ForceProductView;
            }

            FireData();
        }
        #endregion

        #region 料理技术选择界面
        private static void InitReactiveEvents(IAssistLearnCookViewController ctrl)
        {
            _disposable.Add(ctrl.OnNoBtn_UIButtonClick.Subscribe(_ =>
            {
                DataMgr._data.ChosedSkillId = 0;
                DataMgr._data.CurTab = AssistViewTab.ChooseView;
                FireData();
            }));

            _disposable.Add(ctrl.OnYesBtn_UIButtonClick.Subscribe(_ =>
            {
                //第一次学习 次数为 1
                string carryStr = AssistSkillIdToName[(int)AssistSkill.AssistSkillEnum.CarryCook];
                string GrailStr = AssistSkillIdToName[(int)AssistSkill.AssistSkillEnum.GrailCook];
                string strName = DataMgr._data.ChosedSkillId == (int)AssistSkill.AssistSkillEnum.CarryCook ? carryStr : GrailStr;
                BuiltInDialogueViewController.OpenView(string.Format("你只能学习一个生活技能，是否确定学习【{0}】？",strName),
                    null, (() => { AssistSkillMainNetMsg.ReqLearnSkill(DataMgr._data.ChosedSkillId, 1); }),
                        UIWidget.Pivot.Left, "我再想想", "确定");
            }));
        }
        #endregion

        #region 导力技术选择界面
        private static void InitReactiveEvents(IAssistLearnForceViewController ctrl)
        {
            _disposable.Add(ctrl.OnNoBtn_UIButtonClick.Subscribe(_ =>
            {
                DataMgr._data.ChosedSkillId = 0;
                DataMgr._data.CurTab = AssistViewTab.ChooseView;
                FireData();
            }));

            _disposable.Add(ctrl.OnYesBtn_UIButtonClick.Subscribe(_ =>
            {
                //第一次学习 次数为 1
                BuiltInDialogueViewController.OpenView(string.Format("你只能学习1个生活技能，是否确定学习【导力技术】？"),
                    null, (() => { AssistSkillMainNetMsg.ReqLearnSkill(DataMgr._data.ChosedSkillId, 1); }),
                        UIWidget.Pivot.Left, "我再想想", "确定");
            }));
        }
        #endregion

        #region 学习技能 货币兑换
        private static void ReqUpgrade(int count)
        {
            int silverCount = 0;
            int medalCount = 0;
            if(count == 1)
            {
                DataMgr._data.GetCostList.ForEach(x =>
                {
                    if (x.Key == (int)VirtualItemEnum.SILVER)
                        silverCount = x.Value;
                    else if (x.Key == (int)VirtualItemEnum.MEDAL)
                        medalCount = x.Value;
                });
            }
            else if(count == 5)
            {
                DataMgr._data.GetCostFiveList.ForEach(x =>
                {
                    if (x.Key == (int)VirtualItemEnum.SILVER)
                        silverCount = x.Value;
                    else if (x.Key == (int)VirtualItemEnum.MEDAL)
                        medalCount = x.Value;
                });
            }

            if (ExChangeHelper.CheckIsNeedExchange(VirtualItemEnum.SILVER, silverCount, () =>
                 AssistSkillMainNetMsg.ReqLearnSkill(DataMgr._data.SkillId, count)))
                return;

            ExChangeHelper.CheckIsNeedExchange(VirtualItemEnum.MEDAL, medalCount, () =>
                 AssistSkillMainNetMsg.ReqLearnSkill(DataMgr._data.SkillId, count));
        }
        #endregion

        #region 大盘料理/携带料理 技能升级界面
        private static void InitReactiveEvents(IAssistCookUpgradeViewController ctrl)
        {
            _disposable.Add(ctrl.OnForgetBtn_UIButtonClick.Subscribe(_ =>
            {
                var gradeData = DataCache.getDtoByCls<AssistSkillGradeConsume>(DataMgr._data.SkillLevel);
                var ctrlWin = ProxyBaseWinModule.Open();
                ctrlWin.InitAssistForget(string.Format("（{0} {1}级）", AssistSkillIdToName[DataMgr._data.SkillId], DataMgr._data.SkillLevel), 1, gradeData.forgetConsume,
                    DataMgr._data.FirstForget, (() => { AssistSkillMainNetMsg.ReqForgetSkill(DataMgr._data.SkillId); }));
            }));

            _disposable.Add(ctrl.OnTipBtn_UIButtonClick.Subscribe(_ =>
            {
                ProxyTips.OpenTextTips(8, new UnityEngine.Vector3(187 ,115));
            }));

            _disposable.Add(ctrl.OnUngradeBtn_UIButtonClick.Subscribe(_ =>
            {
                ReqUpgrade(1);
            }));

            _disposable.Add(ctrl.OnUpgradeTimesBtn_UIButtonClick.Subscribe(_ =>
            {
                ReqUpgrade(5);
            }));
        }
        #endregion

        #region 大盘料理/携带料理 生产界面
        private static void InitReactiveEvents(IAssistCookProductViewController ctrl)
        {
            _disposable.Add(ctrl.OnProductBtn_UIButtonClick.Subscribe(_ =>
            {
                AssistSkillMainNetMsg.ReqProduct(DataMgr._data.CurRecipeId, 1);
            }));

            _disposable.Add(ctrl.OnProducTimesBtn_UIButtonClick.Subscribe(_ =>
            {
                AssistSkillMainNetMsg.ReqProduct(DataMgr._data.CurRecipeId, 5);
            }));

            _disposable.Add(ctrl.OnChooseRecipeStream.Subscribe(id =>
            {
                DataMgr._data.CurRecipeId = id;

                FireData();
            }));
        }
        #endregion

        #region 导力技术 技能升级界面
        private static void InitReactiveEvents(IAssistForceUpgradeViewController ctrl)
        {
            _disposable.Add(ctrl.OnForgetBtn_UIButtonClick.Subscribe(_ =>
            {
                var gradeData = DataCache.getDtoByCls<AssistSkillGradeConsume>(DataMgr._data.SkillLevel);

                //string str = "确定遗忘已学习的技能？\n" + AssistSkillIdToName[DataMgr._data.SkillId] + "\n" + "消耗#w1" + 
                //    gradeData.forgetConsume.ToString() + "\n" + "(返还部分银币及工会勋章)".WrapColor(ColorConstantV3.Color_Red);

                //BuiltInDialogueViewController.OpenView(str,
                //    null, (() => { AssistSkillMainNetMsg.ReqForgetSkill(DataMgr._data.SkillId); }),
                //        UIWidget.Pivot.Left, "取消", "确定");

                var ctrlWin = ProxyBaseWinModule.Open();
                ctrlWin.InitAssistForget("（" + AssistSkillIdToName[DataMgr._data.SkillId] + " " + DataMgr._data.SkillLevel + "级" + "）", 1, gradeData.forgetConsume,
                    DataMgr._data.FirstForget, (() => { AssistSkillMainNetMsg.ReqForgetSkill(DataMgr._data.SkillId); }));
            }));

            _disposable.Add(ctrl.OnTipBtn_UIButtonClick.Subscribe(_ =>
            {
                ProxyTips.OpenTextTips(8, new UnityEngine.Vector3(187, 115));
            }));

            _disposable.Add(ctrl.OnUngradeBtn_UIButtonClick.Subscribe(_ =>
            {
                ReqUpgrade(1);
            }));

            _disposable.Add(ctrl.OnUpgradeTimesBtn_UIButtonClick.Subscribe(_ =>
            {
                ReqUpgrade(5);
            }));
        }
        #endregion

        #region 导力技术 生产界面
        private static void InitReactiveEvents(IAssistForceProductViewController ctrl)
        {
            _disposable.Add(ctrl.OnProductBtn_UIButtonClick.Subscribe(_ =>
            {
                AssistSkillMainNetMsg.ReqProduct(DataMgr._data.CurRecipeId, 1);
            }));

            _disposable.Add(ctrl.OnProducTimesBtn_UIButtonClick.Subscribe(_ =>
            {
                AssistSkillMainNetMsg.ReqProduct(DataMgr._data.CurRecipeId, 5);
            }));
        }
        #endregion

        #region 委托任务主界面
        private static void InitReactiveEvents(IAssistDelegateMainViewController ctrl)
        {
            #region 添加伙伴和好友按钮
            _disposable.Add(ctrl.OnCrewItem_1_UIButtonClick.Subscribe(_ =>
            {
                if (IsCurMisStart()) return;
                if (DataMgr._data.RemoveChoseCrewByIndex(0))
                {
                    FireData();
                    return;
                }
                OpenAssistCrewView();
            }));

            _disposable.Add(ctrl.OnCrewItem_2_UIButtonClick.Subscribe(_ =>
            {
                if (IsCurMisStart()) return;
                if (DataMgr._data.RemoveChoseCrewByIndex(1))
                {
                    FireData();
                    return;
                }
                OpenAssistCrewView();
            }));

            _disposable.Add(ctrl.OnCrewItem_3_UIButtonClick.Subscribe(_ =>
            {
                if (IsCurMisStart()) return;
                if (DataMgr._data.RemoveChoseCrewByIndex(2))
                {
                    FireData();
                    return;
                }
                OpenAssistCrewView();
            }));

            _disposable.Add(ctrl.OnCrewItem_4_UIButtonClick.Subscribe(_ =>
            {
                if (IsCurMisStart()) return;
                if (DataMgr._data.RemoveChoseCrewByIndex(3))
                {
                    FireData();
                    return;
                }
                OpenAssistCrewView();
            }));

            _disposable.Add(ctrl.OnCrewItem_5_UIButtonClick.Subscribe(_ =>
            {
                if (IsCurMisStart()) return;
                if (DataMgr._data.ChoseFriendId > 0)
                {
                    DataMgr._data.ChoseFriendId = 0;
                    FireData();
                    return;
                }
                var delegateFriendViewCtrl = AssistDelegateFriendViewController.Show<AssistDelegateFriendViewController>(AssistDelegateFriendView.NAME, UILayerType.ThreeModule, true, false);
                delegateFriendViewCtrl.UpdateView(DataMgr._data.AllHelpedFriendIdList);
            }));
            #endregion

            _disposable.Add(ctrl.OnDelegateChoseMissionStream.Subscribe(missionId =>
            {
                DataMgr._data.CurMissionId = missionId;
                DataMgr._data.ResetChoseCrewAndFriend();
                FireData();
            }));

            _disposable.Add(ctrl.OnUpdateMissionBtn_UIButtonClick.Subscribe(_ =>
            {
                if (DataMgr._data.AcceptNum >= DataMgr._data.AcceptLimit)
                {
                    TipManager.AddTip("今天领取委托任务已达上限");
                    return;
                }

                var name = ItemHelper.GetGeneralItemByItemId(100013) == null ? "刷新委托书" : ItemHelper.GetGeneralItemByItemId(100013).name;
                if (DataMgr._data.RefreshIsNoTips)
                {
                    if (BackpackDataMgr.DataMgr.GetItemCountByItemID(100013) <= 0)
                    {
                        TipManager.AddTip(string.Format("{0}数量不足", name));
                        GainWayTipsViewController.OpenGainWayTip(100013, new UnityEngine.Vector3(18, 19));
                        var itemDto = DataCache.getDtoByCls<GeneralItem>(100013);
                        ProxyTips.OpenTipsWithGeneralItem(itemDto, new UnityEngine.Vector3(-420, 122, 0));
                    }
                    else
                        AssistSkillMainNetMsg.ReqRefresh();//刷新委托书

                    return;
                }               

                var controller = ProxyBaseWinModule.Open();
                BaseTipData tipData = BaseTipData.Create("提示", string.Format("是否消耗一个{0}刷新未进行的委托任务？", name), 0, () =>
                {
                    if (BackpackDataMgr.DataMgr.GetItemCountByItemID(100013) <= 0)
                    {
                        TipManager.AddTip(string.Format("{0}数量不足", name));
                        GainWayTipsViewController.OpenGainWayTip(100013, new UnityEngine.Vector3(18, 19));
                        var itemDto = DataCache.getDtoByCls<GeneralItem>(100013);
                        ProxyTips.OpenTipsWithGeneralItem(itemDto, new UnityEngine.Vector3(-420, 122, 0));
                    }
                    else
                        AssistSkillMainNetMsg.ReqRefresh();
                }, null);

                var contentCtrl = controller.InitView(tipData);
                contentCtrl.SetToggle("本次登录不再提示", (b) => { DataMgr._data.RefreshIsNoTips = b; });
            }));

            _disposable.Add(ctrl.OnAcceptBtn_UIButtonClick.Subscribe(_ =>
            {
                AssistSkillMainNetMsg.ReqAcceptMission(DataMgr._data.CurMissionId, DataMgr._data.ChoseFriendId, DataMgr._data.ChoseCrewStr);
            }));

            _disposable.Add(ctrl.OnCancelBtn_UIButtonClick.Subscribe(_ =>
            {
                var controller = ProxyBaseWinModule.Open();
                BaseTipData tipData = BaseTipData.Create("提示", "是否放弃委托任务（放弃后无法获得任务奖励）。", 0, () =>
                {
                    AssistSkillMainNetMsg.ReqAbandonMission(DataMgr._data.CurMissionId);
                }, null);

                controller.InitView(tipData);
            }));
            
            _disposable.Add(ctrl.OnFastBtn_UIButtonClick.Subscribe(_ =>
            {
                var controller = ProxyBaseWinModule.Open();
                var name = ItemHelper.GetGeneralItemByItemId(100014) == null ? "加速胶囊" : ItemHelper.GetGeneralItemByItemId(100014).name;
                BaseTipData tipData = BaseTipData.Create("提示", string.Format("是否消耗一个{0}立即完成委托任务？", name), 0, () =>
                {
                    if(BackpackDataMgr.DataMgr.GetItemCountByItemID(100014) <= 0)
                    {
                        TipManager.AddTip(string.Format("{0}数量不足", name));
                        GainWayTipsViewController.OpenGainWayTip(100014, new UnityEngine.Vector3(18, 19));
                    }
                    else
                        AssistSkillMainNetMsg.ReqFastComplete(DataMgr._data.CurMissionId);
                }, null);

                controller.InitView(tipData);
            }));

            _disposable.Add(ctrl.OnGetRewardBtn_UIButtonClick.Subscribe(_ =>
            {
                AssistSkillMainNetMsg.ReqGetReward(DataMgr._data.CurMissionId);
            }));

            _disposable.Add(ctrl.OnTipsBtn_UIButtonClick.Subscribe(_ =>
            {
                ProxyTips.OpenTextTips(21, new UnityEngine.Vector3(-32, -144));
            }));
        }

        private static bool IsCurMisStart()
        {
            var curMisDto = DataMgr._data._missionList.Find(x => x.id == DataMgr._data.CurMissionId);
            //已开始或已完成
            if (curMisDto != null && curMisDto.finishTime > 0) return true;
            return false;
        }

        private static void OpenAssistCrewView()
        {
            var delegateFriendViewCtrl = AssistDelegateCrewViewController.Show<AssistDelegateCrewViewController>(AssistDelegateCrewView.NAME, UILayerType.ThreeModule, true, false);
            delegateFriendViewCtrl.UpdateView(DataMgr._data.CrewInfoList, DataMgr._data.ChoseCrewIdList, DataMgr._data.CurMissionId, DataMgr._data.AllIngCrewIdList);
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
            
        }
        private static void CloseBtn_UIButtonClick()
        {
            ProxyAssistSkillMain.CloseAssistSkillModule();
        }
    }
}

