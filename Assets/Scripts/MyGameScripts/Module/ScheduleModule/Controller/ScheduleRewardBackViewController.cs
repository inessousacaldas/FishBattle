// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  ScheduleRewardBackViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using System.Collections.Generic;
using AppDto;
using UniRx;
using UnityEngine;
public partial interface IScheduleRewardBackViewController
{
    UniRx.IObservable<ScheduleRewardBackStruct> ScheduleRewardBackItemDisapear { get; }
}
public partial class ScheduleRewardBackViewController
{
    private CompositeDisposable _disposable;
    private AppVirtualItem.VirtualItemEnum normalCostEnum = AppVirtualItem.VirtualItemEnum.NONE;
    private AppVirtualItem.VirtualItemEnum perfectCostEnum = AppVirtualItem.VirtualItemEnum.NONE;
    private int oneKeyNormalCost = -1;
    private int oneKeyPerfectConst = -1;

    private const string ScheduleRewardBackItemStr = ScheduleMainViewDataMgr.ScheduleMainViewLogic.ScheduleRewardBackItemStr;
    private Subject<ScheduleRewardBackStruct> scheduleRewardBackItemEvt = new Subject<ScheduleRewardBackStruct>();
    public UniRx.IObservable<ScheduleRewardBackStruct> ScheduleRewardBackItemDisapear { get { return scheduleRewardBackItemEvt; } }
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        if (_disposable == null)
            _disposable = new CompositeDisposable();
        else
        {
            _disposable.Clear();
        }

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
            ProxyTips.OpenTextTips(26, new Vector3(171, 163));
        });

        OneKeyBackBtn_UIButtonEvt.Subscribe(_ =>
        {
            ExChangeHelper.CheckIsNeedExchange(normalCostEnum, oneKeyNormalCost, () => 
            {
                ScheduleMainViewDataMgr.ScheduleMainViewNetMsg.ReqRewardBack((int)ScheduleActivity.RegainTypeEnum.RegainType_3, 0);
            });
        });

        PrefectBackBtn_UIButtonEvt.Subscribe(_ =>
        {
            ExChangeHelper.CheckIsNeedExchange(perfectCostEnum, oneKeyPerfectConst, () =>
            {
                ScheduleMainViewDataMgr.ScheduleMainViewNetMsg.ReqRewardBack((int)ScheduleActivity.RegainTypeEnum.RegainType_4, 0);
            });
        });
        
    }
    
    protected override void OnDispose()
    {
        normalCostEnum = AppVirtualItem.VirtualItemEnum.NONE;
        perfectCostEnum = AppVirtualItem.VirtualItemEnum.NONE;
        oneKeyNormalCost = -1;
        oneKeyPerfectConst = -1;
        _disposable = _disposable.CloseOnceNull();
        _itemCtrlList.Clear();
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    private List<ScheduleRewardBackItemController> _itemCtrlList = new List<ScheduleRewardBackItemController>();
    public void UpdateView(IScheduleMainViewData data)
    {
        _itemCtrlList.ForEach(item => { item.Hide(); });
        oneKeyNormalCost = 0;
        oneKeyPerfectConst = 0;
        CancelTimer(data);          //在遍历之前停止倒计时器，防止下面遍历时列表有删除操作导致报错
        List<ScheduleRewardBackStruct> _timerList = new List<ScheduleRewardBackStruct>();
        data.RewardBackList.ForEachI((btnItem, index) =>
        {
            _timerList.Add(new ScheduleRewardBackStruct(btnItem));
            var ctrl = AddActivityItemIfNotExist(index);
            ctrl.UpdateView(btnItem, data);
            ctrl.Show();
            var list = data.ScheduleActivityList;
            var item = list.Find(e => e.id == btnItem.id);
            if(normalCostEnum == AppVirtualItem.VirtualItemEnum.NONE || perfectCostEnum ==  AppVirtualItem.VirtualItemEnum.NONE)
            {
                if (item != null)
                {
                    normalCostEnum = (AppVirtualItem.VirtualItemEnum)item.normalVirtualItemId;
                    perfectCostEnum = (AppVirtualItem.VirtualItemEnum)item.perfectVirtualItemId;
                }
            }
            if (!btnItem.receive && item != null) 
            {
                oneKeyNormalCost += item.normalRegainCost;
                oneKeyPerfectConst += item.perfectRegainCost;
            }
        });
        UIHelper.SetAppVirtualItemIcon(View.OneKeyNormmalBack_UISprite, normalCostEnum);
        UIHelper.SetAppVirtualItemIcon(View.OneKeyPerfectBack_UISprite, perfectCostEnum);
        View.OneKeyNormmalBack_UISprite.isGrey = oneKeyNormalCost == 0;
        View.OneKeyPerfectBack_UISprite.isGrey = oneKeyPerfectConst == 0;
        View.normal_UISprite.isGrey = oneKeyNormalCost == 0;
        View.perfect_UISprite.isGrey = oneKeyPerfectConst == 0;
        View.normal_BoxCollider.enabled = oneKeyNormalCost != 0;
        View.perfect_BoxCollider.enabled = oneKeyPerfectConst != 0;
        SetTimer(_timerList);
        View.Grid_UIGrid.Reposition();
    }

    private ScheduleRewardBackItemController AddActivityItemIfNotExist(int idx)
    {
        ScheduleRewardBackItemController ctrl = null;
        _itemCtrlList.TryGetValue(idx, out ctrl);
        if (ctrl == null)
        {
            ctrl = AddChild<ScheduleRewardBackItemController, ScheduleRewardBackItem>(View.Grid_UIGrid.gameObject, ScheduleRewardBackItem.NAME);
            _itemCtrlList.Add(ctrl);
        }
        return ctrl;
    }

    //设置奖励找回倒计时
    private void SetTimer(List<ScheduleRewardBackStruct> _timerList)
    {
        _timerList.ForEach(e =>
        {
            var dto = e._RegainInfoDto;
            float timer = GetTimer(dto.expiredTime);
            if (timer > 0)
                JSTimer.Instance.SetupCoolDown(ScheduleRewardBackItemStr + dto.regainId, timer, null, () => scheduleRewardBackItemEvt.OnNext(e));
        });
    }

    private float GetTimer(long ms)
    {
        var _unixTimeStamp = DateUtil.DateTimeToUnixTimestamp(DateTime.Now);
        var res = ms - _unixTimeStamp;
        var timeSpan = System.TimeSpan.FromMilliseconds(res);
        var timer = timeSpan.TotalSeconds;
        if (timer <= 3600)
            return (float)timer;
        return 0f;
    }

    //取消倒计时
    private void CancelTimer(IScheduleMainViewData data)
    {
        var list = data.RewardBackList;
        list.ForEach(e =>
        {
            JSTimer.Instance.CancelCd(ScheduleRewardBackItemStr + e.regainId);
        });
    }
}
