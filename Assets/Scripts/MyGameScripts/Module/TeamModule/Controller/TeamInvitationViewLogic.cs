using UniRx;
using UnityEngine;

public sealed partial class TeamDataMgr
{
    
    public static partial class TeamInvitationViewLogic
    {
        private static CompositeDisposable _disposable;

        private static string _taskName = "OnRefreshInviteHandler";
        private static bool _canRefresh = true;

        public static void Open()
        {
            if (!DataMgr._data.HasInvitationInfo())
            {
                TipManager.AddTip("现在没有组队邀请");
                return;
            }

            // open的参数根据需求自己调整
            var ctrl = TeamInvitationViewController.Show<TeamInvitationViewController>(
                TeamInvitationView.NAME
                , UILayerType.SubModule
                , true
                , true
                , Stream);
            InitReactiveEvents(ctrl);
        }
        
        private static void InitReactiveEvents(ITeamInvitationViewController ctrl)
        {
            if (ctrl == null) return;

            if (_disposable == null)
                _disposable = new CompositeDisposable();

            _disposable.Add(ctrl.OnCloseBtn_UIButtonClick.Subscribe(_ => CloseBtn_UIButtonClick()));
            _disposable.Add(ctrl.OnClearBtn_UIButtonClick.Subscribe(_ => ClearBtn_UIButtonClick()));
            _disposable.Add(ctrl.OnRefreshBtn_UIButtonClick.Subscribe(_=> RefreshBtn_UIButtonClick()));
            _disposable.Add(ctrl.OnNotifyEvt.Subscribe(notify =>
            {
                TeamNetMsg.ApproveInviteMember(notify);
            }));
        }

        private static void CloseBtn_UIButtonClick()
        {
            UIModuleManager.Instance.CloseModule(TeamInvitationView.NAME);
            _disposable = _disposable.CloseOnceNull();
        }

        private static void ClearBtn_UIButtonClick()
        {
            TipManager.AddTip("===清空列表===");
            DataMgr.ClearInviteList();
            stream.OnNext(DataMgr._data);
       
        }

        private static void RefreshBtn_UIButtonClick()
        {
            if (!_canRefresh)
            {
                var remainTime = Mathf.Floor(JSTimer.Instance.GetRemainTime(_taskName));
                if(remainTime > 0)
                    TipManager.AddTip(string.Format("刷新太急了，请等{0}秒后再试试吧。", remainTime));
                return;
            }

            _canRefresh = false;
            TipManager.AddTip("===刷新列表===");

            TeamDataMgr.TeamNetMsg.RefreshInvite();
            stream.OnNext(DataMgr._data);
            JSTimer.Instance.SetupCoolDown(_taskName, 30f, null, () =>
            {
                _canRefresh = true;
            });
        }
    }    
}

