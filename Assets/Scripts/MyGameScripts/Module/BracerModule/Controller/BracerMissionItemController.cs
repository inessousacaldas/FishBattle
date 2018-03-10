// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  BracerMissionItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using AppDto;
using System.Collections.Generic;


public partial class BracerMissionItemController
{
    private static Dictionary<int, string> StartToPicName = new Dictionary<int, string>
    {
        {0, "youjishi-zhizhangputong"},
        {1, "youjishi-zhizhanglvse"},
        {2, "youjishi-zhizhanglanse"},
        {3, "youjishi-zhizhangzise"},
    };

    private static string[] WeekNumToCh = new string[]
    {
        "日","一","二","三","四","五","六",
    };

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
 
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {

    }

    protected override void OnDispose()
    {

    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    public void UpdateView(bool isOpen, int weekOfDay=0, BracerMissionDto dto=null, BracerMissionCfg missionData=null)
    {
        if(!isOpen)
        {
            View.Bg_UISprite.spriteName = StartToPicName[0];
            View.WeekPublish_Transform.gameObject.SetActive(true);
            View.MissionDetail_Transform.gameObject.SetActive(false);
            View.WeekLabel_UILabel.text = string.Format("周{0}发布", WeekNumToCh[weekOfDay]);

            return;
        }

        View.WeekPublish_Transform.gameObject.SetActive(false);
        View.MissionDetail_Transform.gameObject.SetActive(true);
        View.Bg_UISprite.spriteName = StartToPicName[missionData.missionGrade];
        View.Label_UILabel.text = missionData.missionDesc;
        View.RewardLabel_UILabel.text = missionData.rewardExp.ToString();
        if(dto.progress < missionData.targetVal)
        {
            if (dto.progress == 0)
            {
                View.Progress_Transform.gameObject.SetActive(false);
                View.CompleteSprite_UISprite.gameObject.SetActive(false);
            }
            else
            {
                View.Progress_Transform.gameObject.SetActive(true);
                View.CompleteSprite_UISprite.gameObject.SetActive(false);
                View.ProgressLabel_UILabel.text = string.Format("{0}/{1}", dto.progress, missionData.targetVal);
            }
        }
        else
        {
            View.Progress_Transform.gameObject.SetActive(false);
            View.CompleteSprite_UISprite.gameObject.SetActive(true);
        }
    }
}
