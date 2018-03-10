// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : DM-PC092
// **********************************************************************

using AppDto;
using UniRx;

public sealed partial class RoleSkillDataMgr
{

    public static partial class RoleSkillMainViewLogic
    {
        private static CompositeDisposable _disposable;

        public static void InitReactiveEvents(IRoleSkillMainViewController ctrl)
        {
            if(ctrl == null) return;
            if(_disposable == null)
                _disposable = new CompositeDisposable();
            else
            {
                _disposable.Dispose();
            }
            ctrl.TabMgr.Stream.Subscribe(e =>
            {
                int index = e + 1;
                ctrl.OnTabChange((Skill.SkillEnum)index, DataMgr._data.mainData);
                DataMgr._data.mainData.curTab = (Skill.SkillEnum)index;
                ctrl.TabMgr.SetBtnLblFont(22, "3f3007", 20, "bdbdbd");
            });
            int btnIndex = ((int)Skill.SkillEnum.Crafts) - 1;
            ctrl.TabMgr.SetTabBtn(btnIndex);
            ctrl.TabMgr.SetBtnLblFont(22, "3f3007", 20, "bdbdbd");
            ctrl.OnTabChange(Skill.SkillEnum.Crafts,DataMgr._data.mainData);
            DataMgr._data.mainData.curTab = Skill.SkillEnum.Crafts;
        }

        public static void Dispose()
        {
            if(_disposable != null)
            {
                _disposable = _disposable.CloseOnceNull();
            }
        }

        public static void InitReactiveEvents(IRoleSkillCraftsViewController ctrl)
        {
            _disposable.Add(ctrl.OnbtnUp_UIButtonClick.Subscribe(_ => btnUp_UIButtonClick()));
            _disposable.Add(ctrl.OnbtnWidget_UIButtonClick.Subscribe(_ => { OnCraftItemClick(ctrl.ScraftsItemCtrl, ctrl); }));
        }

        public static void InitReactiveEvents(RoleSkillCraftsItemController ctrl, IRoleSkillCraftsViewController viewCtrl)
        {
            _disposable.Add(ctrl.OnRoleSkillCraftsItem_UIButtonClick.Subscribe(_ => OnCraftItemClick(ctrl, viewCtrl)));
        }

        public static void InitReactiveEvents(RoleSkillMagicViewController ctrl)
        {
            _disposable.Add(ctrl.OnbtnOrbment_UIButtonClick.Subscribe(_ => btnOrbment_UIButtonClick()));
        }

        public static void InitReactiveEvents(RoleSkillCraftsItemController ctrl)
        {
            _disposable.Add(ctrl.OnRoleSkillCraftsItem_UIButtonClick.Subscribe(_ => OnMagicItemClick(ctrl)));
        }

        private static void btnUp_UIButtonClick()
        {
            var curVO = DataMgr.MainData.curSelCtrl.vo;
            int costId = 0;
            int consumNum = 0;
            if(curVO != null)
            {
                var costVO = DataMgr.MainData.GetCostByGradeDto(curVO);
                if(curVO.Grade == curVO.cfgVO.maxGrade)
                {
                    TipManager.AddTip(string.Format("{0}战技已达到最高级",curVO.Name.WrapColor(ColorConstantV3.Color_Green_Str)));
                }
                else if(curVO.LimitLevel > ModelManager.Player.GetPlayerLevel())
                {
                    TipManager.AddTip("人物等级不足，升级失败");
                }
                else if(CheckCostCanUpGrade(curVO,ref costId,ref consumNum))
                {
                    RoleSkillNetMsg.ReqSkillUpgrade(curVO.SkillMapId,costId,consumNum);
                }
            }else
            {
                TipManager.AddTip("请选择战技");
            }
        }

        private static bool CheckCostCanUpGrade(RoleSkillCraftsVO vo,ref int costId,ref int consumNum)
        {
            var costVO = DataMgr.MainData.GetCostByGradeDto(vo);

           
            if (costVO.silver > 0 && ExChangeHelper.CheckIsNeedExchange(AppVirtualItem.VirtualItemEnum.SILVER, costVO.silver))
            {
                //TipManager.AddTip(string.Format("身上的货币不足{0}，升级失败",costVO.silver));
                return false;
            }
            else if(costVO.itemCount1 > 0 && costVO.itemCount1 > BackpackDataMgr.DataMgr.GetItemCountByItemID(vo.cfgVO.consumeBook1))
            {
                TipManager.AddTip("身上的战技书数量不足，升级失败");
                GainWayTipsViewController.OpenGainWayTip(vo.cfgVO.consumeBook1, new UnityEngine.Vector3(164, -46));
                return false;
            }
            else if(costVO.itemCount2 > 0 && costVO.itemCount2 > BackpackDataMgr.DataMgr.GetItemCountByItemID(vo.cfgVO.consumeBook2))
            {
                TipManager.AddTip("身上的战技书数量不足，升级失败");
                GainWayTipsViewController.OpenGainWayTip(vo.cfgVO.consumeBook2, new UnityEngine.Vector3(164, -46));
                return false;
            }
            else
            {
                if (costVO.silver > 0)
                {
                    consumNum = costVO.silver;
                    costId = (int)AppVirtualItem.VirtualItemEnum.SILVER;
                }
                else if (costVO.itemCount1 > 0)
                {
                    consumNum = costVO.itemCount1;
                    costId = vo.cfgVO.consumeBook1;
                }
                else if (costVO.itemCount2 > 0)
                {
                    consumNum = costVO.itemCount2;
                    costId = vo.cfgVO.consumeBook2;
                }
                return true;
            }
        }

        private static void OnCraftItemClick(RoleSkillCraftsItemController ctrl, IRoleSkillCraftsViewController viewCtrl)
        {
            if(ctrl.isSCraftsCtrl)
            {
                DataMgr.MainData.sCraftsState = DataMgr.MainData.sCraftsState == RoleSkillMainSCraftsState.Select ? RoleSkillMainSCraftsState.Normal : RoleSkillMainSCraftsState.Select;
                ctrl.UpdateScraftsViewState(DataMgr.MainData);
                viewCtrl.UpdateOnSctrlClick(DataMgr.MainData);
            }
            else
            {
                viewCtrl.UpdateRight(DataMgr.MainData, ctrl);
                if(DataMgr.MainData.sCraftsState == RoleSkillMainSCraftsState.Select)
                {
                    if(ctrl.vo.IsSCrafts)
                    {
                        if (ctrl.vo.IsOpen)
                        {
                            if (ctrl.vo.Grade == 0)
                            {
                                TipManager.AddTip("请先将战技升级后再进行使用");
                                OnCraftItemClick(viewCtrl.ScraftsItemCtrl, viewCtrl);
                            }
                            else
                            {
                                GameDebuger.LogError("点击id: " + ctrl.vo.id + "\t当前id: " + DataMgr.MainData.CurSCrafts);
                               
                                if(ctrl.vo.id != DataMgr.MainData.CurSCrafts)
                                    RoleSkillNetMsg.ReqSkillDefaultSCrafts(ctrl.vo.id);
                                else
                                    OnCraftItemClick(viewCtrl.ScraftsItemCtrl, viewCtrl);
                            }
                        }
                        else
                        {
                            TipManager.AddTip("请先将战技解锁后再进行使用");
                            OnCraftItemClick(viewCtrl.ScraftsItemCtrl, viewCtrl);
                        }
                    }
                    else
                    {
                        OnCraftItemClick(viewCtrl.ScraftsItemCtrl, viewCtrl);
                    }
                }
                if (DataMgr.MainData.curSelCtrl != null)
                {
                    DataMgr.MainData.curSelCtrl.SetSel(false);
                }
                ctrl.SetSel(true);
                DataMgr.MainData.curSelCtrl = ctrl;
            }
        }

        public static void OnCloseSCraftsState()
        {
            DataMgr.MainData.sCraftsState = RoleSkillMainSCraftsState.Normal;
            FireData();
        }

        private static void btnOrbment_UIButtonClick()
        {
            ProxyQuartz.OpenQuartzMainView(QuartzDataMgr.TabEnum.InfoMagic);
        }

        private static void OnMagicItemClick(RoleSkillCraftsItemController ctrl)
        {
            if (ctrl.magicVo == null)
            {
                if (ctrl.IsLock)
                {
                    TipManager.AddTip("当前尚未解锁");
                    return;
                }
                if (ctrl.IsAdd)
                {
                    btnOrbment_UIButtonClick();
                    return;
                }
            }
            if(DataMgr.MainData.curSelMagicCtrl != null)
            {
                DataMgr.MainData.CurSelMagicCtrl.SetSel(false);
            }
            
            DataMgr.MainData.curSelMagicCtrl = ctrl;
            FireData();
        }
    }
}

