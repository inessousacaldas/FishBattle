// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  TeamBeInviteViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using DG.Tweening;
using UniRx;
using UnityEngine;

public sealed partial class TeamDataMgr
{
    public partial class TeamBeInviteViewController:FRPBaseController_V1<TeamBeInviteView, ITeamBeInviteView>
    {
        public static void Open()
        {
            UIModuleManager.Instance.OpenFunModule<TeamBeInviteViewController>(
                TeamBeInviteView.NAME
                , UILayerType.SubModule
                , true
                , false);
        }

        private IDisposable _disposable = null;
        private TeamInvitationItemController teamInvitationCtrl;

        ///时间控制倒计时
        private static int CountingTime = 30;

	    // 界面初始化完成之后的一些后续初始化工作
        protected override void AfterInitView ()
        {
            teamInvitationCtrl = AddController<TeamInvitationItemController, TeamInvitationItem>(View.TeamInfoGO);
            View.SetCancelLabel(CountingTime);
            JSTimer.Instance.SetupCoolDown("TeamBeInviteTime", CountingTime,
                (currTime) =>
                {
                    int t = Math.Max(0, (int)Math.Ceiling(currTime));
                    View.SetCancelLabel(t);
                },
                () =>
                {
                    if (UIModuleManager.Instance.IsModuleCacheContainsModule(TeamBeInviteView.NAME))
                    {
                        CloseView();
                    }
                }, 1f);
        }

	    //机器自动生成的事件订阅
        protected override void RegistEvent ()
        {
            var d = stream.SubscribeAndFire(teamData =>
            {
                teamInvitationCtrl.UpdateView(teamData.TeamBeInviteData.GetCurrentInvitation());
                View.UpdateView(teamData);
            });

            _disposable = _disposable.CombineRelease(d);

            View.OnShrinkBtn_UIButtonClick.Subscribe(_ => ShrinkBtn_UIButtonClickHandler());
            View.OnCancelButton_UIButtonClick.Subscribe(_ => CancelButton_UIButtonClickHandler());
            View.OnOKButton_UIButtonClick.Subscribe(_ => OKButton_UIButtonClickHandler());

        }

	    // 客户端自定义代码
	    protected override void RegistCustomEvent ()
        {
        
        }

        protected override void OnDispose()
        {
            teamInvitationCtrl = null;
            _disposable.Dispose();
            _disposable = null;
            StopAllCoroutines();
            JSTimer.Instance.CancelCd("TeamBeInviteTime");
            base.OnDispose();
        }

	    //在打开界面之前，初始化数据
	    protected override void InitData()
	    {

    	}

        private void ShrinkBtn_UIButtonClickHandler()
        {
            gameObject.transform.DOLocalMove(new Vector3(145.0f, 205.0f, 0), 0.4f);
            gameObject.transform.DOScale(Vector3.one / 100, 0.3f).SetEase(Ease.OutCubic).OnComplete(() =>
            {
                CloseView();
            });
        }
        private void CancelButton_UIButtonClickHandler()
        {
            CloseView();
        }
        private void OKButton_UIButtonClickHandler()
        {
            TeamNetMsg.ApproveInviteMember(DataMgr._data.TeamBeInviteData.GetCurrentInvitation());
            CloseView();
        }

        private void CloseView(){
            UIModuleManager.Instance.CloseModule(TeamBeInviteView.NAME);
            DataMgr._data.curInvitationNoti = null;
        }
    }
}
