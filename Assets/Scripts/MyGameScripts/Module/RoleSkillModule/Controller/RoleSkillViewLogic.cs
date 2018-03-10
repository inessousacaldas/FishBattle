using Assets.Scripts.MyGameScripts.Module.RoleSkillModule.Model;
using UniRx;

public sealed partial class RoleSkillDataMgr
{
    
    public partial class RoleSkillViewLogic
    {
        private static CompositeDisposable _disposable;

        public static void Open(RoleSkillTab tab = RoleSkillTab.Potential)
        {
            //DataMgr.mainTab = tab;
            // open的参数根据需求自己调整
            var ctrl = RoleSkillViewController.Show<RoleSkillViewController>(
                RoleSkillView.NAME
                , UILayerType.DefaultModule
                , true
                , false
                , Stream);
            InitReactiveEvents(ctrl);
        }

        private static void InitReactiveEvents(IRoleSkillViewController ctrl)
        {
            if (ctrl == null) return;
            if (_disposable == null)
                _disposable = new CompositeDisposable();
            else
            {
                _disposable.Dispose();
            }
            GameEventCenter.RemoveListener(GameEvent.ROLE_SKILL_RESET_MAIN_VIEW_DEPTH);//不移除，会把上一次销毁的模块的事件也触发了
            ctrl.TabMgr.Stream.Subscribe(e =>
            {
                ctrl.OnTabChange((RoleSkillTab)e,DataMgr._data);
                DataMgr._data.mainTab = (RoleSkillTab)e;
            });

            ctrl.TabMgr.SetTabBtn((int)RoleSkillTab.Skill);
            ctrl.OnTabChange(RoleSkillTab.Skill,DataMgr._data);
            DataMgr._data.mainTab = RoleSkillTab.Skill;
            _disposable.Add(QuartzDataMgr.Stream.Subscribe(e => 
            {
                var val = e.QuartzInfoData.GetCurOrbmentInfoDto;
                if (val.crewId < 0)
                {
                    DataMgr._data.mainData.UpdateMagicList(val.magic);
                    FireData();
                }
            }));
            _disposable.Add(ctrl.OnCloseBtn_UIButtonClick.Subscribe(_ => CloseBtn_UIButtonClick()));
            GameEventCenter.AddListener(GameEvent.ROLE_SKILL_RESET_MAIN_VIEW_DEPTH,ctrl.ResetDepth);
        }

        private static void CloseBtn_UIButtonClick()
        {
            UIModuleManager.Instance.CloseModule(RoleSkillView.NAME);
            ProxyWindowModule.CloseWindowOnlyMsgPanel();
            DisposeModule();
        }

        public static void DisposeModule()
        {
            Dispose();
            RoleSkillPotentialViewLogic.Dispose();
            RoleSkillTalentViewLogic.Dispose();
            RoleSkillSpecialityViewLogic.Dispose();
            RoleSkillMainViewLogic.Dispose();
        }

        private static void Dispose()
        {
            if(_disposable != null)
            {
                _disposable = _disposable.CloseOnceNull();
            }
        }
    }

    
}

