// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : xjd
// Created  : 1/19/2018 5:35:39 PM
// **********************************************************************

using UniRx;
using AppDto;

public sealed partial class ScheduleMainViewDataMgr
{
    
    public static partial class ScheduleMainViewLogic
    {
        private static CompositeDisposable _disposable;
        public const string ScheduleRewardBackItemStr = "ScheduleRewardBackItem_";
        public static void Open()
        {

        if (!FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum.FUN_38, true)) return;

        // open的参数根据需求自己调整
	    var layer = UILayerType.DefaultModule;
            var ctrl = ScheduleMainViewController.Show<ScheduleMainViewController>(
                ScheduleMainView.NAME
                , layer
                , true
                , true
                , Stream);
            InitReactiveEvents(ctrl);
        }
        
        private static void InitReactiveEvents(IScheduleMainViewController ctrl)
        {
            if (ctrl == null) return;
            if (_disposable == null)
                _disposable = new CompositeDisposable();
            else
            {
                _disposable.Clear();
            }
        
            _disposable.Add(ctrl.CloseEvt.Subscribe(_=>Dispose()));
            _disposable.Add(ctrl.OnCloseBtn_UIButtonClick.Subscribe(_=>CloseBtn_UIButtonClick()));
            _disposable.Add(ctrl.TabbtnMgr.Stream.Subscribe(i =>
            {
                DataMgr._data.CurRightTab = (ScheduleRightViewTab)i;
                FireData();
            }));
            _disposable.Add(ctrl.ScheduleRewardBackItemDisapear.Subscribe(e =>
            {
                if (e._RegainInfoDto == null) return;
                 OnRewardBackIsFinshed(e._RegainInfoDto.regainId);
            }));
            ctrl.OnChildCtrlAdd += Ctrl_OnChildCtrlAdd;

            ScheduleMainViewNetMsg.ReqScheduleInfo();
            //重设 全部 经验 ..上方标签  和 右方标签
            DataMgr._data.CurTypeBtn = ScheduleActivity.ControlTypeEnum.All;
            DataMgr._data.CurRightTab = ScheduleRightViewTab.DaliyActView;
        }
            
        private static void Dispose()
        {
            _disposable = _disposable.CloseOnceNull();
            OnDispose();    
        }
        
        // 如果有自定义的内容需要清理，在此实现
        private static void OnDispose()
        {
            var list = DataMgr._data.RewardBackList;
            list.ForEach(e =>
            {
                if (!e.receive)
                    JSTimer.Instance.CancelCd(ScheduleRewardBackItemStr + e.regainId);
            });
        }
        private static void CloseBtn_UIButtonClick()
        {
            ProxyScheduleMainView.Close();
        }

        private static void Ctrl_OnChildCtrlAdd(ScheduleRightViewTab tab, IMonolessViewController ctrl)
        {
            switch (tab)
            {
                case ScheduleRightViewTab.DaliyActView:
                    InitReactiveEvents((IScheduleDailyViewController)ctrl);
                    break;
                case ScheduleRightViewTab.LimitActView:
                    InitReactiveEvents((IScheduleDailyViewController)ctrl);
                    break;
            }
        }

        private static void InitReactiveEvents(IScheduleDailyViewController ctrl)
        {
            _disposable.Add(ctrl.OnBtnTypeStream.Subscribe(i =>
            {
                DataMgr._data.CurTypeBtn = (ScheduleActivity.ControlTypeEnum)i;
                FireData();
            }));
        }
        //奖励到期无法找回
        private static void OnRewardBackIsFinshed(long id)
        {
            DataMgr._data.UpdateRewardBack(id);
            FireData();
        }
    }
}

