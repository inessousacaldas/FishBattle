// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : DM-PC092
// **********************************************************************

using AppDto;
using UniRx;

public sealed partial class RoleSkillDataMgr
{
    
    public static partial class RoleSkillSpecialityViewLogic
    {
        private static CompositeDisposable _disposable;

        public static void InitReactiveEvents(IRoleSkillSpecialityViewController ctrl)
        {
            if (ctrl == null) return;
            if (_disposable == null)
                _disposable = new CompositeDisposable();
            else
            {
                _disposable.Dispose();
            }
        
            _disposable.Add(ctrl.OnbtnReset_UIButtonClick.Subscribe(_=> btnReset_UIButtonClick()));
            _disposable.Add(ctrl.OnbtnUp_UIButtonClick.Subscribe(_ => btnUp_UIButtonClick()));
            _disposable.Add(ctrl.OnbtnCancel_UIButtonClick.Subscribe(_ => btnCancel_UIButtonClick()));
            _disposable.Add(ctrl.OnbtnRecommend_UIButtonClick.Subscribe(_ => btnRecommend_UIButtonClick()));
            _disposable.Add(ctrl.OnbtnConform_UIButtonClick.Subscribe(_=> btnConform_UIButtonClick()));           
        }
        public static void Dispose()
        {
            if (_disposable != null)
            {
                _disposable = _disposable.CloseOnceNull();
            }
        }
        public static void InitItemSingleReactiveEvents(RoleSkillSpecialityItemSingleController ctrl)
        {
            _disposable.Add(ctrl.OnbtnAdd_UIButtonClick.Subscribe(_ => OnItemSingleAddClick(ctrl)));
            _disposable.Add(ctrl.OnbtnSub_UIButtonClick.Subscribe(_ => OnItemSingleSubClick(ctrl)));
            _disposable.Add(ctrl.OnInputClick.Subscribe(_ => { OnInputClick(ctrl); }));
            _disposable.Add(ctrl.OnItemClick.Subscribe(_ => OnItemClick(ctrl)));
        }

        private static void OnInputClick(RoleSkillSpecialityItemSingleController ctrl)
        {
            var limitNum = ctrl.vo.cfgVO.maxGradeLimit;
            int grade = ctrl.vo.gradeDto.grade;
            if(grade == limitNum)
            {
                TipManager.AddTip("当前专精已达到上限");
                return;
            }
            var numberInputer = ProxySimpleNumberModule.Open();
            numberInputer.InitData(0, limitNum, ctrl.View.KeyTrans.gameObject);
            numberInputer.OnValueChangeStream.Subscribe(s =>
            {
                var val = s - grade;
                if (val < 0) val = 0;
                DataMgr.SpecData.AddGradeTempData(ctrl.vo.id, val);
                FireData();
            });
        }


        private static void btnReset_UIButtonClick()
        {
            var cost =  DataCache.GetStaticConfigValue(AppStaticConfigs.SPECIALITY_RESET_NEED_GOLD);
            if (!DataMgr.SpecData.CanReset())
            {
                TipManager.AddTip("当前没有分配专精点，重置失败");
                return;
            }
            ProxyWindowModule.OpenConfirmWindow(string.Format("确认消耗{0}金币重置专精吗？",cost),"",() =>
            {
                if(ModelManager.Player.GetPlayerWealthGold() < cost)
                {
                    TipManager.AddTip(string.Format("身上的金币不足{0}，重置失败",cost));
                }
                else
                {
                    RoleSkillNetMsg.ReqSpecialityReset();
                    
                }
            });
        }
        private static void btnUp_UIButtonClick()
        {
            RoleSkillSpecialityAddViewLogic.Open();
        }

        private static void btnCancel_UIButtonClick()
        {
            DataMgr.SpecData.ResetTempList();
            FireData();
        }

        private static void btnRecommend_UIButtonClick()
        {
            TipManager.AddTopTip("敬请期待");
            return;
            var msg = ModelManager.Player.GetPlayer().faction.recommendSpeciality;
            ProxyWindowModule.OpenOnlyMsgView(msg);
        }

        private static void btnConform_UIButtonClick()
        {
            var specPoint = (int)ModelManager.Player.GetPlayerWealth(AppDto.AppVirtualItem.VirtualItemEnum.SPECIALITYPOINT);
            var tmpPoint = DataMgr.SpecData.GetShowPoint();
            if(specPoint == tmpPoint)
            {
                TipManager.AddTip("分配方案没有发生变化");
                return;
            }
            RoleSkillNetMsg.ReqSpecialityAddPoint(DataMgr.SpecData.GetAddPointStr());
        }

        public static void OnItemSingleClick(RoleSkillSpecialityItemSingleController ctrl)
        {

        }

        private static void OnItemSingleAddClick(RoleSkillSpecialityItemSingleController ctrl)
        {
            if(DataMgr.SpecData.GetShowPoint() >= ctrl.vo.gradeUnit)
            {
                var grade = ctrl.vo.addGrade + ctrl.vo.gradeUnit;
                int tmp = grade + ctrl.vo.gradeDto.grade;
                if (tmp > ctrl.vo.cfgVO.maxGradeLimit || ctrl.vo.gradeDto.grade >= ctrl.vo.cfgVO.maxGradeLimit)
                {
                    return;
                }
                DataMgr.SpecData.AddGradeTempData(ctrl.vo.id,grade);
                FireData();
            }
        }

        private static void OnItemSingleSubClick(RoleSkillSpecialityItemSingleController ctrl)
        {
            if(ctrl.vo.addGrade > 0)
            {
                var grade = ctrl.vo.addGrade - ctrl.vo.gradeUnit;
                DataMgr.SpecData.AddGradeTempData(ctrl.vo.id,grade);
                FireData();
            }
        }

        private static void OnItemClick(RoleSkillSpecialityItemSingleController ctrl)
        {
            var msg = string.Format("{0}：同时提升人物和伙伴的{1}",ctrl.vo.Name,ctrl.vo.Name);
            ProxyWindowModule.OpenOnlyMsgView(msg);
        }
            
    }
}

