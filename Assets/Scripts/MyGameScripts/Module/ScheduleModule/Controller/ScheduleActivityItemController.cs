// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  ScheduleActivityItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using AppDto;
using UniRx;

public partial class ScheduleActivityItemController
{
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
 
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        JoinBtn_UIButtonEvt.Subscribe(_ =>
        {
            SmartGuideHelper.GuideTo(_itemData.smartGuideId, -1, ()=>UIModuleManager.Instance.CloseModule(ScheduleMainView.NAME));
        });

        IconBg_UIButtonEvt.Subscribe(_ =>
        {
            clickItemStream.OnNext(_itemData.id);
        });
    }

    protected override void OnDispose()
    {

    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    private ActiveDto _dto;
    private ScheduleActivity _itemData;
    public void UpdateView(ActiveDto dto, ScheduleActivity itemData)
    {
        if (dto == null || itemData == null) return;

        _dto = dto;
        _itemData = itemData;
        UIHelper.SetOtherIcon(View.Icon_UISprite, itemData.icon);
        View.Name_UILabel.text = itemData.name;
        var detailStr = string.Empty;
        if (itemData.rewardCount == -1)
            detailStr = string.Format("次数 不限  活跃度 {0}/{1}", dto.active, itemData.activeDegree);
        else
            detailStr = string.Format("次数 {0}/{1}  活跃度 {2}/{3}", dto.count > itemData.rewardCount ? itemData.rewardCount : dto.count,
                itemData.rewardCount, dto.active, itemData.activeDegree);
        View.DetailLabel_UILabel.text = detailStr.WrapColor(ColorConstantV3.Color_Green);

        switch(ScheduleMainViewDataMgr.DataMgr.GetActivityState(dto, itemData))
        {
            case (int)ScheduleActivityState.Activate:
                View.JoinBtn_UIButton.gameObject.SetActive(true);
                View.Complete_UISprite.enabled = false;
                View.TimeLabel_UILabel.enabled = false;
                break;
            case (int)ScheduleActivityState.Complete:
                View.JoinBtn_UIButton.gameObject.SetActive(false);
                View.Complete_UISprite.enabled = true;
                View.TimeLabel_UILabel.enabled = false;
                break;
            case (int)ScheduleActivityState.Miss:
                View.JoinBtn_UIButton.gameObject.SetActive(false);
                View.Complete_UISprite.enabled = false;
                View.TimeLabel_UILabel.enabled = true;
                View.TimeLabel_UILabel.text = string.Format("{0}\n（已过）", ScheduleMainViewDataMgr.DataMgr.GetActivityStartTime(dto, itemData));
                View.Bg_UISprite.isGrey = true;
                break;
            case (int)ScheduleActivityState.Normal:
                View.JoinBtn_UIButton.gameObject.SetActive(false);
                View.Complete_UISprite.enabled = false;
                View.TimeLabel_UILabel.enabled = true;
                View.TimeLabel_UILabel.text = ScheduleMainViewDataMgr.DataMgr.GetActivityStartTime(dto, itemData);
                break;
        }
    }

    readonly UniRx.Subject<int> clickItemStream = new UniRx.Subject<int>();
    public UniRx.IObservable<int> OnClickItemStream
    {
        get { return clickItemStream; }
    }
}
