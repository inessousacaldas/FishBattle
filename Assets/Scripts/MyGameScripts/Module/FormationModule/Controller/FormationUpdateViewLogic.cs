// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : CL-PC007
// Created  : 7/21/2017 4:02:27 PM
// **********************************************************************

using System.IO;
using UniRx;

public sealed partial class TeamFormationDataMgr
{
    
    public static partial class FormationUpdateViewLogic
    {     
        private static CompositeDisposable _disposable;

        public static void Open()
        {
        // open的参数根据需求自己调整
            var ctrl = FormationUpdateViewController.Show<FormationUpdateViewController>(
                FormationUpdateView.NAME
                , UILayerType.SubModule
                , true
                , true
                ,Stream);
            InitReactiveEvents(ctrl);
        }

        private static void InitReactiveEvents(IFormationUpdateViewController ctrl)
        {
            if (ctrl == null) return;
            if (_disposable == null)
                _disposable = new CompositeDisposable();
            else
            {
                _disposable.Clear();
            }

            _disposable.Add(ctrl.CloseEvt.Subscribe(_ => Dispose()));
            _disposable.Add(ctrl.OnCloseBtn_UIButtonClick.Subscribe(_ => CloseBtn_UIButtonClick()));
            _disposable.Add(ctrl.GetOnClickHandler.Subscribe(id => UpgradeBtn_UIButtonClick(id)));
        }

        private static void CloseBtn_UIButtonClick()
        {
            UIModuleManager.Instance.CloseModule(FormationUpdateView.NAME);
        }
        
        private static void UpgradeBtn_UIButtonClick(int formationId)
        {
            TeamFormationNetMsg.Formation_UpgradeOrLearn(formationId);
        }

        private static void Dispose()
        {
            _disposable = _disposable.CloseOnceNull();
        }
    }
}

