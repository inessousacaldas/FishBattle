using AppDto;
using UniRx;

public sealed partial class RoleSkillDataMgr
{

    public static partial class RoleSkillTalentViewLogic
    {
        private static CompositeDisposable _disposable;

        public static void InitReactiveEvents(IRoleSkillTalentViewController ctrl)
        {
            if(ctrl == null) return;
            if(_disposable == null)
                _disposable = new CompositeDisposable();
            else
            {
                _disposable.Dispose();
            }
            GameEventCenter.RemoveListener(GameEvent.ROLE_SKILL_SHOW_TALENT_GRADE_VIEW);
            DataMgr._data.talentData.lastItem = null;
            _disposable.Add(ctrl.OnbtnReset_UIButtonClick.Subscribe(_ => btnReset_UIButtonClick()));
            _disposable.Add(ctrl.OnbtnAdd_UIButtonClick.Subscribe(_ => btnAdd_UIButtonClick()));
            GameEventCenter.AddListener(GameEvent.ROLE_SKILL_SHOW_TALENT_GRADE_VIEW,ctrl.ShowTalentGradeView);
        }

        public static void Dispose()
        {
            if(_disposable != null)
            {
                _disposable = _disposable.CloseOnceNull();
            }
        }

        public static void InitGradeCtrl(IRoleSkillTalentGradeViewController ctrl)
        {
            _disposable.Add(ctrl.OnbtnUp_UIButtonClick.Subscribe(_ => btnUp_UIButtonClick()));
        }

        private static void btnReset_UIButtonClick()
        {
            var cost =  DataCache.GetStaticConfigValue(AppStaticConfigs.TALENT_RESET_CONSUME_GOLD);
            ProxyWindowModule.OpenConfirmWindow(string.Format("确认消耗{0}金币重置天赋吗？",cost),"",() =>
            {
                if (DataMgr.TalentData.GetAssignedPoint() == 0)
                {
                    TipManager.AddTip("当前没有分配天赋点，重置失败");
                }
                else
                {
                    if (ModelManager.Player.GetPlayerWealthGold() < cost)
                    {
                        TipManager.AddTip(string.Format("身上的金币不足{0}，重置失败", cost));
                    }
                    else
                    {
                        RoleSkillNetMsg.ReqTalentReset();
                    }
                }
            });
        }
        private static void btnAdd_UIButtonClick()
        {
            var maxPoint =  DataCache.GetStaticConfigValue(AppStaticConfigs.TALENT_POINT_MAX_LIMIT);
            if(DataMgr.TalentData.GetTotalPoint() >= maxPoint)
            {
                TipManager.AddTip(string.Format("已经获得全部{0}点天赋点",maxPoint));
            }else
            {
                ProxyTips.OpenTextTips(19, new UnityEngine.Vector3(260, -148), true);
            }
        }

        public static void OnSelectItem(RoleSkillTalentSingleItemController ctrl)
        {
            if(DataMgr._data.talentData.lastItem != null)
            {
                DataMgr._data.talentData.lastItem.SetSelected(false);
            }
            ctrl.SetSelected(true);
            DataMgr._data.talentData.lastItem = ctrl;
            GameEventCenter.SendEvent(GameEvent.ROLE_SKILL_SHOW_TALENT_GRADE_VIEW,ctrl);
            GameEventCenter.SendEvent(GameEvent.ROLE_SKILL_RESET_MAIN_VIEW_DEPTH);
        }

        private static void btnUp_UIButtonClick()
        {
            var curItem = DataMgr.TalentData.lastItem;
            var curTalent = ModelManager.Player.GetPlayerWealth(AppVirtualItem.VirtualItemEnum.TALENTPOINT);
            if(curTalent == 0)
            {
                TipManager.AddTip("当前天赋点不足，升级失败");
            }
            else
            {
                if(curItem.isCanSelect == false)
                {
                    TipManager.AddTip(string.Format("学习{0}天赋需要到达{1}级",curItem.vo.Name,curItem.vo.limitLv));
                }
                else
                {
                    if(DataMgr.TalentData.CheckTalentOpenByLevel(curItem.vo))
                    {
                        if(DataMgr.TalentData.CheckTalentOpenByBefore(curItem.vo))
                        {
                            RoleSkillNetMsg.ReqTalentAddPoint(curItem.vo.id);
                        }
                        else
                        {
                            var beforeCfgVO = DataMgr.TalentData.GetCfgVOById(curItem.vo.cfgVO.beforeTalentId);
                            if(beforeCfgVO != null)
                            {
                                TipManager.AddTip(string.Format("学习{0}天赋需要将前置天赋{1}天赋升到{2}级",curItem.vo.Name,beforeCfgVO.name,beforeCfgVO.maxGrade));
                            }
                        }
                    }
                    else
                    {
                        TipManager.AddTip(string.Format("学习{0}级天赋需要已分配{1}点天赋点",curItem.vo.cfgVO.playerGradeLimit,DataMgr.TalentData.GetNeedAddPointByOpen(curItem.vo.limitLv)));
                    }
                }
            }
        }
    }
}

