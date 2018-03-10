// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  ScheduleRewardBackItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using System.Collections.Generic;
using UniRx;
using AppDto;
public partial interface IScheduleRewardBackItemController
{
}
public partial class ScheduleRewardBackItemController
{
    private ItemCellController _itemCellCtrl = null;
    private RegainInfoDto regainInfoDto = null;
    private AppVirtualItem.VirtualItemEnum normalCostEnum = AppVirtualItem.VirtualItemEnum.NONE;
    private AppVirtualItem.VirtualItemEnum perfectCostEnum = AppVirtualItem.VirtualItemEnum.NONE;
    private int normalCost = -1;
    private int perfectCost = -1;
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
 
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        NormalBackBtn_UIButtonEvt.Subscribe(_ =>
        {
        if (regainInfoDto != null)
        {
            ExChangeHelper.CheckIsNeedExchange(normalCostEnum, normalCost,()=>
            {
                ScheduleMainViewDataMgr.ScheduleMainViewNetMsg.ReqRewardBack((int)ScheduleActivity.RegainTypeEnum.RegainType_1, regainInfoDto.regainId);
            });
            }
        });

        PrefectBackBtn_UIButtonEvt.Subscribe(_ =>
        {
            if (regainInfoDto != null)
            {
                ExChangeHelper.CheckIsNeedExchange(perfectCostEnum, perfectCost, () =>
                {
                    ScheduleMainViewDataMgr.ScheduleMainViewNetMsg.ReqRewardBack((int)ScheduleActivity.RegainTypeEnum.RegainType_2, regainInfoDto.regainId);
                });
            }
        });
        FinishedBtn_UIButtonEvt.Subscribe(_ =>
        {
            TipManager.AddTip("找回次数已用完");
        });
    }

    protected override void OnDispose()
    {
        _itemCellCtrl = null;
        regainInfoDto = null;
        normalCostEnum = AppVirtualItem.VirtualItemEnum.NONE;
        perfectCostEnum = AppVirtualItem.VirtualItemEnum.NONE;
        normalCost = -1;
        perfectCost = -1;
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    public void UpdateView(RegainInfoDto dto, IScheduleMainViewData data)
    {
        regainInfoDto = dto;
        var schedule = data.ScheduleActivityList;
        var scheduleData = schedule.Find(e => e.id == dto.id);
        if (scheduleData != null)
        {
            View.Name_UILabel.text = scheduleData.name;
            View.NormalBackBtn_UIButton.gameObject.SetActive(!dto.receive);
            View.PrefectBackBtn_UIButton.gameObject.SetActive(!dto.receive);
            View.finishedBtn_UIButton.gameObject.SetActive(dto.receive);
            View.TimeNum_UILabel.gameObject.SetActive(!dto.receive);
            if (!dto.receive)
            {
                normalCostEnum = (AppVirtualItem.VirtualItemEnum)scheduleData.normalVirtualItemId;
                perfectCostEnum = (AppVirtualItem.VirtualItemEnum)scheduleData.perfectVirtualItemId;
                normalCost = scheduleData.normalRegainCost;
                perfectCost = scheduleData.perfectRegainCost;
                UIHelper.SetAppVirtualItemIcon(View.NormalCost_UISprite, normalCostEnum);
                View.NormalCost_UILabel.text = scheduleData.normalRegainCost.ToString();
                UIHelper.SetAppVirtualItemIcon(View.PerfectCost_UISprite, perfectCostEnum);
                View.PrefectCost_UILabel.text = scheduleData.perfectRegainCost.ToString();
            }
            View.TimeNum_UILabel.text = GetTimer(dto.expiredTime);
            View.ExpNum_UILabel.text = dto.exp.ToString();
            var item = ItemHelper.GetGeneralItemByItemId(StringHelper.ToInt(scheduleData.regainItemsView));
            if (_itemCellCtrl == null)
            {
                _itemCellCtrl = AddChild<ItemCellController, ItemCell>(View.Grid_UIGrid.gameObject, ItemCell.NAME);
            }
            if(item!=null)
                _itemCellCtrl.UpdateView(item);
        }
    }
    
    private string GetTimer(long ms)
    {
        var _unixTimeStamp = DateUtil.DateTimeToUnixTimestamp(DateTime.Now);
        var res = ms - _unixTimeStamp;
        var timeSpan = System.TimeSpan.FromMilliseconds(res);
        int d = timeSpan.Days;
        int h = timeSpan.Hours;
        int m = timeSpan.Minutes;

        //1、当前剩余时间超过1天，即大于等于24小时，则显示:xx天xx小时
        if(d >= 1)
        {
            if (h == 0) h = 1;
            return d + "天" + h + "小时";
        }
        //2、当前剩余时间少于1天，即小于24小时并大于等于60分钟。 则显示：XX小时xx分钟
        if (h >= 1)
        {
            return h + "小时" + m + "分钟";
        }
        //3、当前剩余时间少于60分钟小时并且大于60秒。 则显示：0小时xx分钟
        if (m >= 1)
        {
            return 0 + "小时" + m + "分钟";
        }
        else
        {
            return 0 + "小时" + 1 + "分钟";
        }
    }
}
public struct ScheduleRewardBackStruct
{
    private RegainInfoDto _regainInfoDto;
    public RegainInfoDto _RegainInfoDto
    {
        get { return _regainInfoDto; }
    }
    public ScheduleRewardBackStruct(RegainInfoDto dto)
    {
        _regainInfoDto = dto;
        //_timer = timer;
    }
}
