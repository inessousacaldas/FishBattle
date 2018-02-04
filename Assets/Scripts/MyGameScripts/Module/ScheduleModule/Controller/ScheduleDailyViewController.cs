// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  ScheduleDailyViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using AppDto;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public partial class ScheduleDailyViewController
{
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
 
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        WeekBtn_UIButtonEvt.Subscribe(_ =>
        {
            ScheduleActivityCalendarViewController.Show<ScheduleActivityCalendarViewController>(ScheduleActivityCalendarView.NAME, UILayerType.ThreeModule, true, false);
        });

        PushBtn_UIButtonEvt.Subscribe(_ =>
        {
            ScheduleActivityRemindViewController.Show<ScheduleActivityRemindViewController>(ScheduleActivityRemindView.NAME, UILayerType.ThreeModule, true, false);
        });

        TipsBtn_UIButtonEvt.Subscribe(_ =>
        {
            if(_curTab == ScheduleRightViewTab.DaliyActView)
                ProxyTips.OpenTextTips(24, new Vector3(171, 163));
            else if (_curTab == ScheduleRightViewTab.LimitActView)
                ProxyTips.OpenTextTips(25, new Vector3(171, 163));
        });
    }

    protected override void OnDispose()
    {
        _disposable = _disposable.CloseOnceNull();
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    private List<ScheduleRewardTypeBtnController> _typeBtnCtrlList = new List<ScheduleRewardTypeBtnController>();
    private List<ScheduleActivityItemController> _activityItemCtrlList = new List<ScheduleActivityItemController>();
    private List<ScheduleProgRewardItemController> _progItemCtrlList = new List<ScheduleProgRewardItemController>();
    private Dictionary<int, ScheduleActivity> _scheduleActivityDic = DataCache.getDicByCls<ScheduleActivity>();
    private Dictionary<int, ActiveReward> _scheduleActiveRewardDic = DataCache.getDicByCls<ActiveReward>();
    private readonly Dictionary<ScheduleActivity.ControlTypeEnum, string> _typeBtns = new Dictionary<ScheduleActivity.ControlTypeEnum, string>
    {
        {ScheduleActivity.ControlTypeEnum.All, "全部"},
        {ScheduleActivity.ControlTypeEnum.Exp, "经验"},
        {ScheduleActivity.ControlTypeEnum.Gold, "金币"},
        {ScheduleActivity.ControlTypeEnum.Silver, "银币"},
        {ScheduleActivity.ControlTypeEnum.Prop, "道具"},
    };

    CompositeDisposable _disposable = new CompositeDisposable();
    readonly UniRx.Subject<int> btnTypeStream = new UniRx.Subject<int>();
    public UniRx.IObservable<int> OnBtnTypeStream
    {
        get { return btnTypeStream; }
    }

    private ScheduleRightViewTab _curTab;

    public void UpdateView(IScheduleMainViewData data, ScheduleRightViewTab tab)
    {
        _disposable.Clear();
        _curTab = tab;
        //上方选择按钮
        _typeBtnCtrlList.ForEach(item => { item.Hide(); });
        _typeBtns.ForEachI((btnItem, index) =>
        {
            var ctrl = AddTypeItemIfNotExist(index);
            ctrl.UpdateView(btnItem.Key, btnItem.Value, data.CurTypeBtn == btnItem.Key);
            ctrl.Show();

            _disposable.Add(ctrl.OnClickItemStream.Subscribe(btnType =>
            {
                btnTypeStream.OnNext(btnType);
            }));
        });

        //活动
        _activityItemCtrlList.ForEach(item => { item.Hide(); });
        var curList = data.DailyActivityList;
        if(tab == ScheduleRightViewTab.LimitActView)
            curList = data.LimitActivityList;

        curList.ForEachI((itemDto, index) =>
        {
            var staticItemData = _scheduleActivityDic.Find(x => x.Value.id == itemDto.id);
            if (staticItemData.Value == null) return;
            if (data.CurTypeBtn == ScheduleActivity.ControlTypeEnum.All || staticItemData.Value.controlType.Contains((int)data.CurTypeBtn))
            {
                var ctrl = AddActivityItemIfNotExist(index);
                ctrl.UpdateView(itemDto, staticItemData.Value);
                ctrl.Show();

                _disposable.Add(ctrl.OnClickItemStream.Subscribe(id =>
                {
                    ShowItemTips(ctrl.gameObject, itemDto, staticItemData.Value);
                }));
            }
        });

        View.Grid_UIGrid.Reposition();

        //活跃度奖励
        View.ActiveNum_UILabel.text = data.ActiveValue.ToString();
        View.ActiveNumSlider_UISlider.value = (float)data.ActiveValue / (float)data.ActiveMaxValue;
        _progItemCtrlList.ForEach(item => { item.Hide(); });
        _scheduleActiveRewardDic.ForEachI((itemDto, index) =>
        {
            var ctrl = AddProgItemIfNotExist(index);
            ctrl.UpdateView(itemDto.Value, data.ActiveValue >= itemDto.Value.active, data.ActivityRewardList.ToList().Contains(itemDto.Value.id));
            ctrl.Show();
        });

        View.RewardGrid_UIGrid.Reposition();
    }

    private ScheduleRewardTypeBtnController AddTypeItemIfNotExist(int idx)
    {
        ScheduleRewardTypeBtnController ctrl = null;
        _typeBtnCtrlList.TryGetValue(idx, out ctrl);
        if (ctrl == null)
        {
            ctrl = AddChild<ScheduleRewardTypeBtnController, ScheduleRewardTypeBtn>(View.BtnGrid_UIGrid.gameObject, ScheduleRewardTypeBtn.NAME);
            _typeBtnCtrlList.Add(ctrl);
        }

        return ctrl;
    }

    private ScheduleActivityItemController AddActivityItemIfNotExist(int idx)
    {
        ScheduleActivityItemController ctrl = null;
        _activityItemCtrlList.TryGetValue(idx, out ctrl);
        if (ctrl == null)
        {
            ctrl = AddChild<ScheduleActivityItemController, ScheduleActivityItem>(View.Grid_UIGrid.gameObject, ScheduleActivityItem.NAME);
            _activityItemCtrlList.Add(ctrl);
        }

        return ctrl;
    }

    private ScheduleProgRewardItemController AddProgItemIfNotExist(int idx)
    {
        ScheduleProgRewardItemController ctrl = null;
        _progItemCtrlList.TryGetValue(idx, out ctrl);
        if (ctrl == null)
        {
            ctrl = AddChild<ScheduleProgRewardItemController, ScheduleProgRewardItem>(View.RewardGrid_UIGrid.gameObject, ScheduleProgRewardItem.NAME);
            _progItemCtrlList.Add(ctrl);
        }

        return ctrl;
    }

    private void ShowItemTips(GameObject gameobject, ActiveDto dto, ScheduleActivity data)
    {
        var scrollviewPosY = View.ScrollView_UIScrollView.panel.clipOffset.y;
        var top = -32;
        var leftX = -187;
        var rightX = 230;
        var pos = gameobject.transform.localPosition;
        var tipsPos = new Vector3(pos.x > 410 ? rightX : leftX, pos.y < -90 ? -120 : top + pos.y - scrollviewPosY);
        var ctrl = ScheduleActivityTipsController.Show<ScheduleActivityTipsController>(ScheduleActivityTips.NAME, UILayerType.ThreeModule, true);
        ctrl.UpdateView(dto, data);
        ctrl.SetTipsPosition(tipsPos);
    }
}
