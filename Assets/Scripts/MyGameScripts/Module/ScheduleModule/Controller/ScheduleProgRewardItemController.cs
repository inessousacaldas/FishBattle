// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  ScheduleProgRewardItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using UniRx;
using AppDto;

public partial class ScheduleProgRewardItemController
{
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
 
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        IconBg_UIButtonEvt.Subscribe(_ =>
        {
            //if(_canGet)
                ScheduleMainViewDataMgr.ScheduleMainViewNetMsg.ReqActivityReward(_id);
        });
    }

    protected override void OnDispose()
    {

    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    private int _id = 0;
    private bool _canGet = false;
    public void UpdateView(ActiveReward dto, bool canGet, bool isGet)
    {
        if (dto == null) return;
        _id = dto.id;
        _canGet = canGet;
        if (!dto.items.IsNullOrEmpty() && ItemHelper.GetGeneralItemByItemId(dto.items[0].itemId) != null)
            UIHelper.SetItemIcon(View.Icon_UISprite, ItemHelper.GetGeneralItemByItemId(dto.items[0].itemId).icon);
        View.Num_UILabel.text = dto.active.ToString();
        //View.HadGet_UISprite.enabled = isGet;
        View.HadGet_UILabel.enabled = isGet;
    }
}
