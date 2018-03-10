// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : fish
// **********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using AppDto;
using AppServices;
using Assets.Scripts.MyGameScripts.Utils;
using UniRx;

public sealed partial class BattleDataManager
{

    public static partial class BattleDemoViewLogic
    {
        private static string Battle_MainView_Timer = "Battle_MainView_Timer";
        private static CompositeDisposable _disposable;
        private static CompositeDisposable _skillSelDisposable;

        private static readonly Dictionary<string, Action<IBattleDemoViewController>> battleOrderUICallbackDictionary 
            = new Dictionary<string, Action<IBattleDemoViewController>>() {
            {"AttackButton", ctrl =>
            {
                ctrl.RemoveItemView();
                ctrl.RemoveRollSkillView();
                attackBtn_UIButtonClick();
            }}  // 普攻
            , {"MagicButton", ctrl=>
                    {
                        ctrl.RemoveItemView();
                        MagicAtkBtn_UIButtonClick(ctrl);
                    }
                } //魔法攻击按钮事件回调
            , {"SkillButton", ctrl=>
                    {
                        ctrl.RemoveItemView();
                        CraftBtn_UIButtonClick(ctrl);
                    }
                } //使用战技事件回调
            , {"MoveButton", ctrl=>
                    {
                        ctrl.RemoveItemView();
                        ctrl.RemoveRollSkillView();
                        QuickAtkBtn_UIButtonClick();
                    }
                } //使用快捷事件回调
            , {"UserItemButton", ctrl=>
                    {
                        ctrl.RemoveRollSkillView();
                        ItemButton_UIButtonClick(ctrl);
                    }
                } //逃跑事件回调
            , {"EscapeButton", ctrl=>
                    {
                        ctrl.RemoveItemView();
                        ctrl.RemoveRollSkillView();
                        RetreatButton_UIButtonClick(ctrl);
                    }
                } 
                //召唤事件回调
                , {"CallButton", ctrl=>
                    {
                        ctrl.RemoveItemView();
                        ctrl.RemoveRollSkillView();
                        OnCallCrewBtn_UIButtonClick(ctrl);
                    }
                }
                
            };

        public static void Open()
        {
            DemoSimulateHelper.SimulateRoundStart();
            GameDebuger.TODO(@"if (DataMgr._data.IsNeedSynSettingVideo() && !IsGuideCatchPetBattle() && !IsGuideHeroAndPetBattle())
        {
            Instance._data.isAIManagement = ModelManager.SystemData.autoFramToggle;
        }
            else
            {
                DataMgr._data.isAIManagement = false;
            }");

        // open的参数根据需求自己调整
            var ctrl = BattleDemoViewController.Show<BattleDemoViewController>(
                BattleDemoView.NAME
                , UILayerType.BaseModule
                , false
                , true
                , Stream);

            ctrl.StartGameVideo();

            InitReactiveEvents(ctrl);
            
        }

        private static void InitReactiveEvents(IBattleDemoViewController ctrl)
        {
            if (ctrl == null) return;
            if (_disposable == null)
                _disposable = new CompositeDisposable();
            else
            {
                _disposable.Clear();
            }
            _disposable.Add(ctrl.CloseEvt.Subscribe(_=>Dispose()));
            _disposable.Add(Stream.Select((data, state)=> data.battleState).Subscribe(state =>
            {
                if (state == BattleSceneStat.Battle_OVER)
                    UIModuleManager.Instance.CloseModule(BattleDemoView.NAME);
            }));
            _disposable.Add(ctrl.OnCancelSkillBtn_UIButtonClick.Subscribe(_ => CancelButton_UIButtonClick()));
            _disposable.Add(ctrl.OnAutoButton_UIButtonClick.Subscribe(_=>AutoButton_UIButtonClick()));
            _disposable.Add(ctrl.OnManualButton_UIButtonClick.Subscribe(_=>ManualButton_UIButtonClick()));
            _disposable.Add(ctrl.OnBtnCommand_UIButtonClick.Subscribe(_ => BtnCommand_UIButtonClick()));
            _disposable.Add(ctrl.OnItemButton_UIButtonClick.Subscribe(_ => ItemButton_UIButtonClick(ctrl)));

            var optionCtrl = ctrl.BattleOptionCtrl;
            _disposable.Add(optionCtrl.BtnClickEvt.Subscribe(btnName =>
            {
                Action<IBattleDemoViewController> cb = null;
                battleOrderUICallbackDictionary.TryGetValue(btnName, out cb);
                GameUtil.SafeRun(cb, ctrl);
            }));

            #region PlayerSkill OPT
            _disposable.Add(ctrl.OnPlayerBtnCurrentHeadClick.Subscribe(id=>PlayerHeadClicked(id)));
            #endregion

            ctrl.OnAutoSkillBtnClick = ShowAllSoldierSkills;
        }

        private static void ShowAllSoldierSkills(IBattleDemoViewController ctrl, long id)
        {
            var mc = MonsterManager.Instance.GetMonsterFromSoldierID(id);
            var skills = mc.videoSoldier.skillIds;
            var skillList = new List<Skill>();
            skills.ForEach(sid =>
            {
                var skill = DataCache.getDtoByCls<Skill>(sid);
                if(skill != null)
                    skillList.Add(skill);
            });
            var controller = ctrl.ShowAutoSkillPanel(skillList);

            InitSkillSelReactiveEvents(
                controller
                , sid =>
                {
                    //sid代表点击的技能id
                    BattleNetworkManager.ReqChangeDefaultSkill(id, sid);
                    ctrl.RemveAutoSkillView();
                });
        }

        private static void PlayerHeadClicked(long id)
        {
            var data = DataMgr._data;
            var tip = string.Empty;
            var valide = data.CheckMonsterCanUseSCraft(id, ref tip);
            if (!valide)
            {
                TipManager.AddTip(tip);
                return;
            }

            BattleNetworkManager.Req_Use_S_Craft(
                data.GameVideo.id
                , id);
        }

        private static void InitSkillSelReactiveEvents(
            IBattleSkillViewController ctrl
            , Action<int> cellClickCallback)
        {
            if (ctrl == null) return;
            if (_skillSelDisposable == null)
                _skillSelDisposable = new CompositeDisposable();
            else
            {
                _skillSelDisposable.Clear();
            }
            //_skillSelDisposable.Add(ctrl.CloseEvt.Subscribe(_ =>
            //{
            //    _skillSelDisposable = _skillSelDisposable.CloseOnceNull();
            //}));

            //_skillSelDisposable.Add(ctrl.TabStream.Subscribe(e =>
            //{
            //    var tab = ctrl.GetTabByIdx(e);
            //    var tmp = GetSkills(tab);
            //    ctrl.UpdateView(tmp);
            //}));

            _skillSelDisposable.Add(ctrl.GetCellClickHandler.Subscribe(id =>
            {
                GameUtil.SafeRun<int>(cellClickCallback, id);
            }));
        }

        private static void Dispose()
        {
            OnDispose();
            _disposable = _disposable.CloseOnceNull();
        }

        private static void OnDispose()
        {
            _skillSelDisposable = _skillSelDisposable.CloseOnceNull();
            JSTimer.Instance.CancelTimer(Battle_MainView_Timer);
        }

        private static void ItemButton_UIButtonClick(IBattleDemoViewController ctrl)
        {
            var view = ctrl.ShowItemPanel();

            var battleType = 0;
//                    if (_gameVideo is PvpVideo)
//                    {
//                        battleType = (_gameVideo as PvpVideo).type;
//                    }

            var mc = DataMgr._data.CurActMonsterController;    
            var items = BackpackDataMgr.DataMgr.GetBattleItems(mc.GetCharactorType(),battleType);
            view.InitView(items);
            view.OnConfirmButton_UIButtonClick.Subscribe(_ =>
                {
                    if (!DataMgr.BattleDemo.CanUseCommand()
                        || DataMgr.BattleDemo.LockUI)
                        return;

                    var skill = CreateItemUseSkill(view.CurItem);
                    if (skill == null) return;
                    DataMgr._data.CurActMonsterController.SetItemSkill(skill, view.CurItem);
                    ctrl.RemoveItemView();
                    FireData();
                }
            );
        }

        private static Skill CreateItemUseSkill(BagItemDto packItem)
        {
            if (null == packItem)
                return null;
//            else
//            {
////        CancelAutoButton();
            var skill = new Skill();
            skill.id = GetUseItemSkillId();
            skill.logicId = packItem.index;
            skill.name = packItem.item.name;
            skill.shortDescription = packItem.item.description;
            var props = packItem.item as Props;
            if (props != null)
                skill.skillAiId = props.targetType;   

            return skill;
        }

        private static void QuickAtkBtn_UIButtonClick()
        {
            DataMgr._data.CurActMonsterController.SetMagicOrCraftSkill();
            FireData();
        }

        private static IEnumerable<Skill> GetSkills(Skill.SkillEnum skillType)
        {
//            System.Type type = skillType == Skill.SkillEnum.Magic
//                ? typeof(Magic)
//                : typeof(Crafts);

            if (DataMgr._data.CurActMonsterController == null)
            {
                GameLog.Log_Battle("DataMgr._data.CurActMonsterController == null");
                return null;
            }

            var set = DataMgr._data.CurActMonsterController.videoSoldier.skillIds
                .Map(id => DataCache.getDtoByCls<Skill>(id))
                .Filter(item => 
                    item != null && item.type == (int) skillType);

            return set;
        }

        private static void MagicAtkBtn_UIButtonClick(IBattleDemoViewController ctrl)
        {
            ShowSkillSelectedView(ctrl, Skill.SkillEnum.Magic);
        }

        private static void CraftBtn_UIButtonClick(IBattleDemoViewController ctrl)
        {
            ShowSkillSelectedView(ctrl, Skill.SkillEnum.Crafts);
        }

        private static void ShowSkillSelectedView(
            IBattleDemoViewController ctrl
            , Skill.SkillEnum skillEnum)
        {

            var controller = ctrl.ShowSkillPanel(GetSkills(skillEnum), skillEnum);

            InitSkillSelReactiveEvents(
                controller
                , id =>
                {
                    var mc = DataMgr._data.CurActMonsterController;
                    if (mc == null)
                        return;
                    mc.SetMagicOrCraftSkill(id);
                    FireData();
                });
        }

        private static void attackBtn_UIButtonClick()
        {
            DataMgr._data.CurActMonsterController.SetNormalSkill();
            FireData();
        }

        private static void CancelButton_UIButtonClick()
        {
            if (DataMgr._data.CurActMonsterController == null) return;
            DataMgr._data.CurActMonsterController.ClearSkill();
            FireData();
        }

        private static void RetreatButton_UIButtonClick(IBattleDemoViewController ctrl)
        {
            var data = DataMgr._data;
            if (data.IsGameOver || data.LockUI)
            {
                return;
            }

            GameDebuger.TODO(@"if (_guideBattle)
        {
            return;
        }");
            var tip = string.Empty;
            if (!DataMgr.CanRetreat(out tip))
            {
                if (!tip.IsNullOrEmpty())
                    TipManager.AddTip(tip);
                return;
            }
            //在进行竞技场的战斗时，点击逃跑按钮需要弹出确认提示框
            GameDebuger.TODO(@"if (DataMgr._data.GameVideo is PvpVideo && DataMgr._data.actionState == ActionState.HERO)
        {
            if ((DataMgr._data.GameVideo as PvpVideo).type == PvpVideo.PvpTypeEnum_Challenge)
            {
                ProxyWindowModule.OpenConfirmWindow('在竞技场中逃跑视为战败，确定要逃跑吗？', '', delegate
                    {
                        DoRetreatButtonClick();
                    });
                return;
            }
        }");

            ProxyWindowModule.OpenConfirmWindow("逃跑会导致战斗失败，是否继续逃跑？", "", delegate
            {
                DoRetreatButtonClick(ctrl);
            });
        }

        private static void DoRetreatButtonClick(IBattleDemoViewController ctrl)
        {
            if (!DataMgr._data.IsCurActMonsterCanbeOpt)
            {
                TipManager.AddTip("操作超时");
                return;
            }

            if (DataMgr._data.LockUI)
            {
                return;
            }

            GameDebuger.TODO(@"if (_guideBattle)
        {
            return;
        }");

            ctrl.CancelAutoButton();

            var requestInfo = Services.Battle_Escape(DataMgr._data.GameVideo.id);

            DataMgr._data.LockUI = true;

            BattleNetworkManager.ReqServerWithSimulate(requestInfo, "", e =>
            {
                DataMgr._data.LockUI = false;
            });
        }

        #region 召唤
        private static void OnSummonPet(long petId)
        {
            if ( !DataMgr.BattleDemo.CanUseCommand()
            || DataMgr.BattleDemo.LockUI)
            {
                return;
            }

            if (DataMgr._data.usableCallTime <= 0)
            {
                TipManager.AddTip("召唤次数已经用完");
                return;
            }
            
            var requestInfo = Services.Battle_ChangeCrew(DataMgr.BattleDemo.GameVideo.id, DataMgr.BattleDemo.CurActMonsterController.GetId(), petId);

            DataMgr.BattleDemo.LockUI = true;

            ServiceRequestAction.requestServerWithSimulate(requestInfo, "", e =>
                {
                    DataMgr._data.LockUI = false;
                    DataMgr._data.CurActMonsterController.NeedReady = false;
                    DataMgr._data.usableCallTime -= 1;
                    var playerInfoDto = DataMgr._data._gameVideo.playerInfos.Find(info=>info.playerId == ModelManager.IPlayer.GetPlayerId());
                    if (playerInfoDto == null)
                    {
                        playerInfoDto = new BattlePlayerInfoDto();
                        playerInfoDto.playerId = DataMgr.BattleDemo.CurActMonsterController.GetId();
                        playerInfoDto.allCrewSoldierIds = new List<long>(4);
                        DataMgr._data._gameVideo.playerInfos.Add(playerInfoDto);
                    }
                    playerInfoDto.allCrewSoldierIds.Add(petId);
                },
                e =>
                {
                    DataMgr.BattleDemo.LockUI = false;
                    DataMgr.CheckBattleOver(e);
                });
        }

        private static void OnCallCrewBtn_UIButtonClick(IBattleDemoViewController ctrl)
        {
            var crewDataList = CrewViewDataMgr.DataMgr.GetCrewListInfo();
            if (!crewDataList.ToList().Any())
            {
                CrewViewDataMgr.CrewViewNetMsg.ResCrewList(() =>
                {
                    ShowBattleSummonPanel(ctrl);
                });
            }
            else
            {
                ShowBattleSummonPanel(ctrl);
            }
        }

        private static void ShowBattleSummonPanel(IBattleDemoViewController ctrl)
        {
            var crewDataList = CrewViewDataMgr.DataMgr.GetCrewListInfo();
            var playerInfoDto =
                        DataMgr._data._gameVideo.playerInfos.Find(
                            info => info.playerId == ModelManager.IPlayer.GetPlayerId());
            var cnt = 4 - DataMgr._data.usableCallTime;
            Func<long, bool> p = null;
            if (playerInfoDto == null)
                p = (id) => true;
            else
                p = (id) => playerInfoDto.allCrewSoldierIds.IndexOf(id) < 0;

            crewDataList = CrewViewDataMgr.DataMgr.GetCrewListInfo().Filter(d =>
                p(d.id)
                );

            var l = crewDataList.ToList();
            l.Sort((x, y) =>
            {
                if (x.grade != y.grade)
                    return y.grade - x.grade;
                return (int)(x.id - y.id);
            });
            if (l.Count == 0)
            {
                TipManager.AddTip("没有可以召唤的伙伴");
            }
            else
            {
                var summonCtrl = ctrl.ShowBattleSummonPanel();
                summonCtrl.OpenSummonView(CrewBattleSummonData(l, cnt, 4));
                _disposable.Add(summonCtrl.OnSummonBtnHandler.Subscribe(id => { OnSummonPet(id); }));
            }
        }

        private static IBattleSummomData CrewBattleSummonData(IEnumerable<CrewInfoDto> dataList, int fightTimes, int total)
        {
            var crewDataList = new List<IBattleCrewData>();
            dataList.ForEach(d =>
            {
                var state = DataMgr.GetCrewBattleState(d.id);
                var crewdata = BattleCrewData.Create(d, state);
                crewDataList.Add(crewdata);
            });
            var battleSummomData = BattleSummonData.Create(crewDataList, fightTimes, total);
            return battleSummomData;
        }
        #endregion

        private static void AutoButton_UIButtonClick()
        {
            if (DataMgr.BattleDemo.IsGameOver)
            {
                return;
            }
            if (DataMgr.BattleDemo.LockUI)
            {
                return;
            }

            GameDebuger.TODO(@"if (_guideBattle)
        {
            return;
        }");

            GameDebuger.TODO(@"if (IsGuideHeroAndPetBattle() && (battleState == BattleSceneStat.ON_PROGRESS || battleState == BattleSceneStat.ON_WAITING_ActTime_Update))
        {
            return;
        }");

            DataMgr.BattleDemo.LockUI = true;

            ServiceRequestAction.requestServerWithSimulate(Services.Battle_Auto(DataMgr.BattleDemo.GameVideo.id), "", e =>
            {
                DataMgr.BattleDemo.LockUI = false;
                DataMgr.BattleDemo.isAIManagement = true;
                FireData();
            }, (e) =>
            {
                DataMgr.CheckBattleOver(e);
            });
        }

        private static void ManualButton_UIButtonClick()
        {
            if (DataMgr.BattleDemo.IsGameOver)
            {
                return;
            }

            GameDebuger.TODO(@"if (_guideBattle)
        {
            return;
        }");


            if (/**DataMgr.BattleDemo.battleState == BattleSceneStat.FINISH_COMMAND ||*/ 
                DataMgr.BattleDemo.battleState == BattleSceneStat.ON_WAITING_ActTime_Update)
            {
                return;
            }

            DataMgr.BattleDemo.LockUI = true;
            BattleNetworkManager.ReqServerWithSimulate(Services.Battle_CancelAuto(DataMgr.BattleDemo.GameVideo.id), "", e =>
            {
                DataMgr.BattleDemo.LockUI = false;
                DataMgr.BattleDemo.isAIManagement = false;

                if (DataMgr.BattleDemo.battleState == BattleSceneStat.BATTLE_VIDEOROUND_PLAYING )
                {
                    TipManager.AddTip("下回合开始时显示操作菜单");
                }
                else
                {
                    GameEventCenter.SendEvent(GameEvent.BATTLE_UI_HIDE_AUTO_ROUND_TIME_LABEL);
                }
            }, e =>
            {
                //TipManager.AddTip(e.message);
                DataMgr.CheckBattleOver(e);
                DataMgr.BattleDemo.LockUI = true;
            });
        }
        private static void BtnCommand_UIButtonClick()
        {
        }
    }


}


