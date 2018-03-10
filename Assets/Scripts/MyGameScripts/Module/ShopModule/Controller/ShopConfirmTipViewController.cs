// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  ShopConfirmTipViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;

public partial class ShopConfirmTipViewController:MonolessViewController<CommonTipWin>
{
    JSTimer.CdTask task;
    Action OnConfirmEvent;
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        //JSTimer.Instance.remo
        EventDelegate.Add(View.OKButton_UIButton.onClick, OnClickOkButton);
        EventDelegate.Add(View.CancelButton_UIButton.onClick, OnCancelButton);
    }

    protected override void OnDispose()
    {
        if (task != null)
        {
            task.Dispose();
        }
        OnConfirmEvent = null;
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        EventDelegate.Remove(View.OKButton_UIButton.onClick, OnClickOkButton);
        EventDelegate.Remove(View.CancelButton_UIButton.onClick, OnCancelButton);
    }
    public void InitView(long shopRefreshTime,int price,Action OnConfirm)
    {
        this.OnConfirmEvent = OnConfirm;
        var serverDateTime = SystemTimeManager.Instance.GetServerTime();
        var timespan = (DateUtil.UnixTimeStampToDateTime(shopRefreshTime) - serverDateTime);
        var remainSecond = (int)timespan.TotalSeconds;
        task = JSTimer.Instance.SetupCoolDown("ShopConfirmTimer", remainSecond, x=> {
            View.ContentLb_UILabel.text = string.Format("物品在{0}秒后刷新，是否消耗{1}刷新", (int)x, price);
        }, 
        ()=> {
            View.ContentLb_UILabel.text = string.Format("是否消耗{0}刷新", price);
        },1);
    }
    private void OnClickOkButton()
    {
        if (OnConfirmEvent != null)
            OnConfirmEvent();
    }
    private void OnCancelButton()
    {
        ProxyBaseWinModule.Close();
    }
}
