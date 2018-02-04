// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : DM-PC092
// **********************************************************************

using AppDto;
using UniRx;

public sealed partial class RoleSkillDataMgr
{
    
    public static partial class RoleSkillSpecialityAddViewLogic
    {
        private static CompositeDisposable _disposable;

        public static void Open()
        {
        // open的参数根据需求自己调整
            var ctrl = RoleSkillSpecialityAddViewController.Show<RoleSkillSpecialityAddViewController>(
                RoleSkillSpecialityAddView.NAME
                , UILayerType.DefaultModule
                , true
                , true
                , Stream);
            InitReactiveEvents(ctrl);
        }
        
        private static void InitReactiveEvents(IRoleSkillSpecialityAddViewController ctrl)
        {
            if (ctrl == null) return;
            if (_disposable == null)
                _disposable = new CompositeDisposable();
            else
            {
                _disposable.Dispose();
            }
        
            _disposable.Add(ctrl.OnbtnItem_UIButtonClick.Subscribe(_=>btnItem_UIButtonClick()));
            _disposable.Add(ctrl.OnbtnTrain1_UIButtonClick.Subscribe(_=>btnTrain1_UIButtonClick()));
            _disposable.Add(ctrl.OnbtnTrain10_UIButtonClick.Subscribe(_ => btnTrain10_UIButtonClick()));
            _disposable.Add(ctrl.OnCloseBtn_UIButtonClick.Subscribe(_ => CloseBtn_UIButtonClick()));
            
            FireData();           
        }

        private static void CloseBtn_UIButtonClick()
        {
            UIModuleManager.Instance.CloseModule(RoleSkillSpecialityAddView.NAME);
            DisposeModule();
        }

        public static void DisposeModule()
        {
            Dispose();
        }

        private static void Dispose()
        {
            if(_disposable != null)
            {
                _disposable = _disposable.CloseOnceNull();
            }
        }

        private static void btnItem_UIButtonClick()
        {
            ToExpByItem();
        }
        private static void btnTrain1_UIButtonClick()
        {
            ToExp(1);
        }
        private static void btnTrain10_UIButtonClick()
        {
            ToExp(10);
        }

        private static void ToExpByItem()
        {
            var itemID = DataMgr.SpecData.GetTrainItemID();
            if(DataMgr.SpecData.GetCurGrade() > DataMgr.SpecData.GetLevelLimit())
            {
                TipManager.AddTip("当前专精已达到等级上限");
            }
            else if(BackpackDataMgr.DataMgr.GetItemCountByItemID(itemID) == 0)
            {
                TipManager.AddTip(string.Format("身上{0}数量不足",ItemHelper.GetItemName(itemID)));
            }
            else
            {
                RoleSkillNetMsg.ReqSpecialityAddExp(SpecialityExpGrade.AddExpType.ApplyProps, 1);
            }
        }

        private static void ToExp(int time)
        {
            var costSilver =  DataCache.GetStaticConfigValue(AppStaticConfigs.SPECIALITY_ADDEXP_CONSUME_SILVER) * time;
            var costMedal =  DataCache.GetStaticConfigValue(AppStaticConfigs.SPECIALITY_ADDEXP_CONSUME_MEDAL) * time;
            if(DataMgr.SpecData.GetCurGrade() > DataMgr.SpecData.GetLevelLimit())
            {
                TipManager.AddTip("当前专精已达到等级上限");
            }
            else if(ModelManager.Player.GetPlayerWealthSilver() < costSilver)
            {
                TipManager.AddTip(string.Format("身上的银币不足以进行训练"));
            }
            else if(ModelManager.Player.GetPlayerWealth(AppVirtualItem.VirtualItemEnum.MEDAL) < costMedal)
            {
                TipManager.AddTip(string.Format("身上的公会勋章不足以进行训练"));
            }
            else
            {
                RoleSkillNetMsg.ReqSpecialityAddExp(SpecialityExpGrade.AddExpType.Training,time);
            }
        }
     
    }
}

