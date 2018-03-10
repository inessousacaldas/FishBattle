using System.Collections.Generic;
using AppDto;
using UniRx;
using UnityEngine;

public sealed partial class TeamDataMgr
{
    
    public static partial class TeamApplicationViewLogic
    {
        private static CompositeDisposable _disposable;

        private const int _colNum = 2;    //每行2个
        private static string _taskName = "OnRefreshHandler";
        private static bool _canRefresh = true;

        public static void Open()
        {
        // open的参数根据需求自己调整
            var ctrl = TeamApplicationViewController.Show<TeamApplicationViewController>(
                TeamApplicationView.NAME
                , UILayerType.SubModule
                , true
                , dataUpdator:Stream);
            InitReactiveEvents(ctrl);
        }
        
        private static void InitReactiveEvents(ITeamApplicationViewController ctrl)
        {
            if (ctrl == null) return;
            if (_disposable == null)
                _disposable = new CompositeDisposable();
            else
            {
                _disposable.Clear();
            }

            _disposable.Add(ctrl.CloseEvt.Subscribe(_ => Disposable()));

            _disposable.Add(ctrl.OnIgnoreBtn_UIButtonClick.Subscribe(_ => IgnoreBtn_UIButtonClick()));
            _disposable.Add(ctrl.OnCloseBtn_UIButtonClick.Subscribe(_ => CloseBtn_UIButtonClick()));
            _disposable.Add(ctrl.OnRefreshBtn_UIButtonClick.Subscribe(_ => RefreshBtn_UIButtonClick()));     
            
            _disposable.Add(stream.Subscribe(data =>
            {
                if (data == null || data.TeamApplyViewData.GetApplicationCnt() <= 0)
                    CloseBtn_UIButtonClick();
            }));
            
            _disposable.Add(ctrl.OnSubject.Subscribe(idx =>
            {
                var noti = DataMgr._data.GetJoinTeamRequestByIndex(idx);
                if (noti != null)
                {
                    TeamNetMsg.ApproveJoinTeam(noti.playerId);
                }
            }));

            _disposable.Add(ctrl.ListStream.Subscribe(tupe =>
            {
                var data = DataMgr._data.GetJoinTeamTwoRequestByIndex(tupe.p3 * _colNum);
                ctrl.RefreshItemList(data, tupe);
            }));

//            ctrl.SetApplicationItem(DataMgr._data);
        }

        private static void IgnoreBtn_UIButtonClick()
        {
            TipManager.AddTip("清空申请列表");
            ////DataMgr.CleanUpApplicationInfo();
            DataMgr.CleanUpDefauleShow();
        }

        private static void Disposable()
        {
            _disposable = _disposable.CloseOnceNull();
        }

        private static void CloseBtn_UIButtonClick()
        {
            UIModuleManager.Instance.CloseModule (TeamApplicationView.NAME);
            //_disposable = _disposable.CloseOnceNull();
        }
        
        private static void RefreshBtn_UIButtonClick()
        {
            if (!_canRefresh)
            {
                var remaintime = JSTimer.Instance.GetRemainTime(_taskName);
                TipManager.AddTip(string.Format("刷新太急了，请等{0}秒后再试试吧。", (int) remaintime));
                return;
            }

            _canRefresh = false;
            TipManager.AddTip("===刷新列表===");
            TeamDataMgr.TeamNetMsg.RefreshApply();
            
            JSTimer.Instance.SetupCoolDown(_taskName, 30f, null, () =>
            {
                _canRefresh = true;
            });
        }        
    }  
}

