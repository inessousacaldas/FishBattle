// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  SpecialCopyMissionPanelController.cs
// Author   : DM-PC092
// Created  : 2/8/2018 3:10:11 PM
// Porpuse  : 
// **********************************************************************

using AppDto;
using System;
using UniRx;
using DG.Tweening;
using UnityEngine;

public partial interface ISpecialCopyMissionPanelController
{
    void SetData(int missionId);
}
public partial class SpecialCopyMissionPanelController    {
    private UISprite[] StarsArr;
    private String timeName = "CloseSpecialCopyUI";
	// 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        int timer = 30;
        View.TweenBG_Transform.localScale = new Vector3(0,1,0);
        JSTimer.Instance.CancelTimer(timeName);
        JSTimer.Instance.SetupTimer(timeName,() =>
        {
            if(timer <= 0) {
                View.TweenBG_Transform.DOScale(Vector3.zero,0.2f).OnComplete(delegate
                {
                    ProxySpecialCopy.Close();
                });
                JSTimer.Instance.CancelTimer(timeName);

            }
            timer--;
            View.GoToLabel_UILabel.text = "前往(" + timer + ")";
        },1f);
    }

	// 客户端自定义代码
	protected override void RegistCustomEvent ()
    {
        
    }

    protected override void RemoveCustomEvent ()
    {
    }
        
    protected override void OnDispose()
    {
        JSTimer.Instance.CancelTimer(timeName);
        JSTimer.Instance.CancelTimer("LightStar");
        base.OnDispose();
    }

	//在打开界面之前，初始化数据
	protected override void InitData()
    {
        
    }

    // 业务逻辑数据刷新
    protected override void UpdateDataAndView(ISpecialCopyData data){

    }

    public void SetData(int missionId) {
        var t = DataCache.getDtoByCls<CopyExtra>(missionId);
        View.TitleLabel_UILabel.text = t.name;
        View.ContentLabel_UILabel.text = t.desc;
        View.LevelLabel_UILabel.text = t.grade+"级";
        StarsArr = new UISprite[5];
        for(int i = 0;i < 5;i++) {
           GameObject go =  NGUITools.AddChild(View.Stars.transform.parent.gameObject,View.Stars);
            go.SetActive(true);
            go.transform.localPosition = new Vector3(23 + 26 * i,2,0);
            StarsArr[i] = go.GetComponent<UISprite>();
        }
        int index = 0;
        JSTimer.Instance.SetupTimer("LightStar",() =>
        {
            if(index >= t.level) {
                JSTimer.Instance.CancelTimer("LightStar");
                return;
            }
            StarsArr[index].spriteName = "Star_Light";
            index++;
        },0.4f);
        View.TweenBG_Transform.DOScale(Vector3.one,0.2f);
    }

}
