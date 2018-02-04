
using UniRx;
using AppDto;


public sealed partial class CrewSkillDataMgr
{

    public static class CrewSkillViewLogic
    {
        private static CompositeDisposable _disposable;
        private static bool isLearn = false;
        private static PassiveSkillBook book;

        public static void InitReactiveEvents(ICrewSkillViewController ctrl, CrewSkillTab tab = CrewSkillTab.Crafts)
        {
            if (ctrl == null) return;
            if (_disposable == null)
                _disposable = new CompositeDisposable();
            else
            {
                _disposable.Dispose();
            }
            _disposable.Add(ctrl.TabMgr.Stream.Subscribe(e =>
            {
                DataMgr._data.MainTab = (CrewSkillTab)e;
                ctrl.OnTabChange((CrewSkillTab)e, DataMgr._data);
            }));
            //ctrl.TabMgr.SetTabBtn((int)CrewSkillTab.Crafts);
            ctrl.OnTabChange(CrewSkillTab.Crafts, DataMgr._data);
            DataMgr._data.MainTab = CrewSkillTab.Crafts;
            ctrl.InitWindow();
            _disposable.Add(ctrl.GetBGClickEvt.Subscribe(_ => InitWindowEvenst(_)));
            _disposable.Add(ctrl.PsvTabMgr.Subscribe(_ => ctrl.GetPsvTipCtrl.OnTabChange((PassiveType)_,DataMgr._data)));
            _disposable.Add(ctrl.PsvBtnAdd.Subscribe(_ => ctrl.GetPsvTipCtrl.OnBtnAddClick()));
            _disposable.Add(ctrl.PsvBtnMinus.Subscribe(_ => ctrl.GetPsvTipCtrl.OnBtnMinusClick()));
            _disposable.Add(ctrl.PsvBtnForget.Subscribe(_ => ctrl.GetPsvTipCtrl.ShowForgetWindow()));
            _disposable.Add(ctrl.PsvBtnMax.Subscribe(_ => ctrl.GetPsvTipCtrl.OnBtnMaxClick()));
            _disposable.Add(ctrl.PsvBtnUp.Subscribe(_ => OnPsvBtnUpClick()));
            _disposable.Add(ctrl.PsvBtnUse.Subscribe(_ => OnPsvBtnUseClick(ctrl.GetPsvTipCtrl.GetConsumNum)));
            _disposable.Add(ctrl.InputValueChange.Subscribe(_ => ctrl.GetPsvTipCtrl.OnValueChange()));
            _disposable.Add(ctrl.PsvBtnLearn.Subscribe(_ => OnPsvTipBtnClick(_)));
            _disposable.Add(ctrl.PsvBtnForgetSure.Subscribe(_ => OnPsvBtnForgetClick()));
            _disposable.Add(ctrl.PsvBtnCancel.Subscribe(_ => ctrl.GetPsvTipCtrl.CloseForgetWindow()));
            _disposable.Add(ctrl.PsvBtnBlackBG.Subscribe(_ => ctrl.GetPsvTipCtrl.CloseForgetWindow()));
            _disposable.Add(ctrl.CraftsBtnUp.Subscribe(_ => BtnCraftsUpClick()));
            _disposable.Add(ctrl.MagicBtn.Subscribe(_ => 
                            {
                                ProxyQuartz.OpenQuartzMainView();
                                ctrl.InitWindow();
                            }));
            _disposable.Add(ctrl.GetCraftsViewCtrl.CraftsViewBtnUp.Subscribe(_ => OnCraftsBtnClick(_,ctrl)));
            _disposable.Add(ctrl.GetMagicViewCtrl.MagicViewBtnUp.Subscribe(_ => OnMagicBtnClick(_,ctrl)));
            _disposable.Add(ctrl.GetPsvViewCtrl.PsvViewBtnUp.Subscribe(_ => OnPsvBtnClick(_,ctrl)));
            _disposable.Add(QuartzDataMgr.Stream.Subscribe(e =>
            {
                var val = e.QuartzInfoData.GetCurOrbmentInfoDto;
                if (val.crewId > 0)
                {
                    DataMgr._data.UpdateMagicData(val.crewId, val.magic);
                    CrewViewDataMgr.CrewMainViewLogic.FireDatas();
                }
            }));
        }

        public static void InitWindowEvenst(ICrewSkillWindowController ctrl)
        {
            ctrl.HideWindow();
        }

        private static void OnMagicBtnClick(ICrewSkillItemNController itemCtrl, ICrewSkillViewController ctrl)
        {
            if (itemCtrl.SkillVO == null)
            {
                if( itemCtrl.SkillState == CrewSkillItemNController.MagicSkillState.NoEquiped)
                {
                    ProxyQuartz.OpenQuartzMainView();
                }
                else if(itemCtrl.SkillState == CrewSkillItemNController.MagicSkillState.Locked)
                {
                    TipManager.AddTip("该伙伴的这个技能格尚未解锁");
                }
            }
            else
                ctrl.ShowWindowsView(itemCtrl.SkillVO);
        }

        #region ---------------------------------------------------------战技---------------------------------------------------------

        
        private static void OnCraftsBtnClick(ICrewSkillItemNController itemCtrl, ICrewSkillViewController ctrl)
        {
            DataMgr.CraftsData.curSelCrafVO = itemCtrl.SkillVO as CrewSkillCraftsVO;
            ctrl.ShowWindowsView(itemCtrl.SkillVO);
        }

        private static void BtnCraftsUpClick()
        {
            CrewSkillCraftsVO curVO = null;
            if (DataMgr.CraftsData.curSelCrafVO != null)
                curVO = DataMgr.CraftsData.curSelCrafVO;

            if (curVO != null)
            {
                var cost = DataMgr.CraftsData.GetCostByGradeDto(curVO);
                if (curVO.Grade == curVO.cfgVO.maxGrade)
                {
                    TipManager.AddTip(curVO.Name + "战技已达到最高级");
                }
                else if (curVO.Grade * 5 + curVO.cfgVO.playerGradeLimit > CrewViewDataMgr.DataMgr.GetCurCrewGrade)
                {

                    TipManager.AddTip("伙伴等级不足，升级失败");
                }
                else if (CheckCostCanUpGrade(curVO))
                {
                    var dic = DataMgr._data.GetCrewInfo();
                    if (dic.ContainsKey(curVO.belongCrew))
                    {
                        CrewSkillNetMsg.ReqSkillUpgrade(dic[curVO.belongCrew].Id, curVO.cfgVO.skillMapId);
                    }
                }
            }
            else
            {
                TipManager.AddTip("请选择战技");
            }
        }

        private static bool CheckCostCanUpGrade(CrewSkillCraftsVO vo)
        {
            var cost = DataMgr.CraftsData.GetCostByGradeDto(vo);
            if (cost.silver > 0 && cost.silver > ModelManager.Player.GetPlayerWealthSilver())
            {
                TipManager.AddTip(string.Format("身上的货币不足{0}，升级失败", cost.silver));
                TipManager.AddTip("弹出货币快捷兑换界面(还没实现，先弹提示)");
                return false;
            }
            else if (cost.itemCount1 > 0 && cost.itemCount1 > BackpackDataMgr.DataMgr.GetItemCountByItemID(vo.cfgVO.consumeBook1))
            {
                TipManager.AddTip("身上的战技书数量不足，升级失败");
                return false;
            }
            else if (cost.itemCount2 > 0 && cost.itemCount2 > BackpackDataMgr.DataMgr.GetItemCountByItemID(vo.cfgVO.consumeBook2))
            {
                TipManager.AddTip("身上的战技书数量不足，升级失败");
                return false;
            }
            else
            {
                return true;
            }
        }
        
        
        #endregion

        #region---------------------------------------------------------魔法---------------------------------------------------------


        //魔法左侧导力器点击
        public static void InitMagicBtnDLQEvents(ICrewSkillMagicViewController ctrl)
        {
            _disposable.Add(ctrl.OnbtnDLQ_UIButtonClick.Subscribe(_ => BtnMagicDLQClick()));
        }

        private static void BtnMagicDLQClick()
        {
            TipManager.AddTip("魔法导力器按钮点击");
        }

        #endregion

        #region---------------------------------------------------------技巧---------------------------------------------------------
        //点击出现弹窗
        private static void OnPsvBtnClick(ICrewSkillItemNController itemCtrl, ICrewSkillViewController ctrl)
        {
            if(itemCtrl.PsvItemData.state == PassiveState.Lock)
            {
                TipManager.AddTip("该伙伴的这个技能格尚未解锁");
            }
            DataMgr._data.SetPsvItemData(itemCtrl.PsvItemData);
            ctrl.ShowWindowsView(itemCtrl.PsvItemData, DataMgr._data);
        }

        //遗忘
        private static void OnPsvBtnForgetClick()
        {
            PsvItemData psvItem = DataMgr._data.GetPsvItemData;
            if (psvItem == null) return;
            CrewTmpInfo crewTmp = DataMgr._data.GetCrewInfo()[CrewSkillHelper.CrewID];
            CrewSkillNetMsg.ReqPassiveForget(crewTmp.Id, psvItem.psvVO.skillMapId);
        }

        //使用点击
        private static void OnPsvBtnUseClick(int consumNum)
        {
            PsvItemData psvItem = DataMgr._data.GetPsvItemData;
            if (psvItem == null || consumNum == 0)
            {
                TipManager.AddTip("请选择数量");
                return;
            }
            int count = BackpackDataMgr.DataMgr.GetItemCountByItemID(psvItem.psvVO.itemId);
            if (count <= 0)
            {
                TipManager.AddTip(psvItem.psvVO.item.name + "不足");
            }
            else
            {
                CrewTmpInfo crewTmp = DataMgr._data.GetCrewInfo()[CrewSkillHelper.CrewID];
                CrewSkillNetMsg.ReqPassiveUse(crewTmp.Id, psvItem.psvVO.skillMapId, consumNum);
            }
        }

        //升级点击
        private static void OnPsvBtnUpClick()
        {
            PsvItemData psvItem = DataMgr._data.GetPsvItemData;
            if (psvItem == null) return;
            int count = BackpackDataMgr.DataMgr.GetItemCountByItemID(psvItem.psvVO.itemId);
            if (count <= 0)
            {
                TipManager.AddTip(psvItem.psvVO.item.name + "不足");
            }
            else
            {
                CrewTmpInfo crewTmp = DataMgr._data.GetCrewInfo()[CrewSkillHelper.CrewID];
                CrewSkillNetMsg.ReqPassiveUp(crewTmp.Id, psvItem.psvVO.skillMapId);
            }
        }
        
        private static void OnPsvTipBtnClick(ICrewSkillPassiveTipsController ctrl)
        {
            if (ctrl.LastCtrl.GetItemInBag)
            {
                var dic = DataMgr._data.GetCrewInfo();
                if (dic.ContainsKey(CrewSkillHelper.CrewID))
                {
                    CrewTmpInfo crewTmp = dic[CrewSkillHelper.CrewID];
                    CrewSkillNetMsg.ReqPassiveLearn(crewTmp.Id, ctrl.LastCtrl.PsvBookData.id);
                }
                else
                {
                }
            }
            else
            {
                //TipManager.AddTip("获取途径");
            }
        }
        #endregion
        public static void Dispose()
        {
            book = null;
            if (_disposable != null)
            {
                _disposable = _disposable.CloseOnceNull();
            }
        }
    }
}

