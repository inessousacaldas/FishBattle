// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  ProgressBarViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using UnityEngine;

public partial class ProgressBarViewController
{
    // 界面初始化完成之后的一些后续初始化工作
    private bool mIsMoveClose = false;//是否移动才关闭进度条
    protected override void AfterInitView ()
    {
        SetMissionUsePropsProgress(0);
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        EventDelegate.Set(View.BgBoxllider_UIEventTrigger.onClick,OnMissionUseClickCallBack);
    }

    protected override void OnDispose()
    {
        missionUseClickCallBack = null;
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }


    public void SetMissionUsePropsProgress(float floatValue)
    {
        View.MissionUsePropsProgress_UISlider.value = floatValue;
    }

    private System.Action missionUseClickCallBack;
    public void SetMissionUsePropsProgress(bool activeState,string icon,string str = "",System.Action clickCallBack = null,bool pIsMoveClose = false)
    {
        mIsMoveClose = pIsMoveClose;
        OnHindeBgBoxllide(mIsMoveClose);
        View.gameObject.SetActive(activeState);
        missionUseClickCallBack = clickCallBack;
        if(activeState)
        {
            View.ProgressLabel_UILabel.text = str;
            SetUsePopsData(icon);
        }
    }

    private void SetUsePopsData(string icon)
    {
        string[] icons = icon.Split(':');
        switch(Int32.Parse(icons[0]))
        {
            case 1:
                UIHelper.SetItemIcon(View.ItemIcon_UISprite,icons[1]);
                break;
            case 2:
                UIHelper.SetPetIcon(View.ItemIcon_UISprite,icons[1]);
                break;
            case 3:
                UIHelper.SetSkillIcon(View.ItemIcon_UISprite,icons[1]);
                break;
            case 4:
                UIHelper.SetOtherIcon(View.ItemIcon_UISprite,icons[1]);
                break;
        }
    }


    private void OnMissionUseClickCallBack()
    {
        GameUtil.SafeRun(missionUseClickCallBack);
        ProxyProgressBar.Close(mIsMoveClose);
    }

    /// <summary>
    /// 用于隐藏进度条子节点的BoxCollder，使得玩家点击UI层的物品时能触发点击回掉事件
    /// </summary>
    private void OnHindeBgBoxllide(bool pIsHindeBoxlider = false)
    {
        if(pIsHindeBoxlider)
        {
            View.BgBoxllider_UIEventTrigger.GetComponent<BoxCollider>().enabled = false;
        }
        else
        {
            View.BgBoxllider_UIEventTrigger.GetComponent<BoxCollider>().enabled = true;
        }
    }
}
