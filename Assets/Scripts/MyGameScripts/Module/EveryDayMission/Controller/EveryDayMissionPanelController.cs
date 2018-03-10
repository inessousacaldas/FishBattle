// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  EveryDayMissionPanelController.cs
// Author   : DM-PC092
// Created  : 11/10/2017 4:36:55 PM
// Porpuse  : 
// **********************************************************************

using System;
using UniRx;
using System.Collections.Generic;
using AppDto;
using UnityEngine;

public partial interface IEveryDayMissionPanelController
{
    UniRx.IObservable<int> IGetOptionClickEvt { get; }
}
public partial class EveryDayMissionPanelController {
    private Subject<int> _OptionClickEvt = new Subject<int>();
    public UniRx.IObservable<int> IGetOptionClickEvt { get { return _OptionClickEvt; } }

    private Dictionary<int,GameObject> missionOptionObj = new Dictionary<int, GameObject>();
    private Dictionary<int,FactionMissionRate> mEveryDayMissionList = new Dictionary<int, FactionMissionRate>();
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView()
    {

    }

    // 客户端自定义代码
    protected override void RegistCustomEvent()
    {
        EventDelegate.Set(_view.ClickBtnBoxCollider_UIButton.onClick,ClickDialogueBtn);
    }

    protected override void RemoveCustomEvent()
    {
    }

    protected override void OnDispose()
    {
        base.OnDispose();
        foreach(int key in missionOptionObj.Keys)
        {
            Destroy(missionOptionObj[key]);
        }
        missionOptionObj.Clear();
        mEveryDayMissionList.Clear();
    }

    //在打开界面之前，初始化数据
    protected override void InitData()
    {

    }

    // 业务逻辑数据刷新
    protected override void UpdateDataAndView(IEveryDayMissionData data) {
        ServerType GetmServerType=data.GetmServerType();
        mEveryDayMissionList = data.GetEveryDayMissionList();
        View.MissionNumber_UILabel.text = string.Format("今日完成日常委托次数:   {0}次",data.GetDailyFinishCount());
        switch(GetmServerType) {
            case ServerType.Init:
                InitEveryDayPanel(data.GetEveryDayMissionList());
                break;
            case ServerType.UpDate:
                break;
            case ServerType.PopUpTip:
                if(EveryDayMissionDataMgr.IsOpenCheckEveryMission)
                {
                    View.mEveryDayMissionPanel.enabled = false;
                    ProxyWindowModule.OpenConfirmWindow("今日已经做满4次游击士任务，继续完成只能获得少量奖励，是否继续？","每日委托",SureOpenPanel,ClickDialogueBtn,UIWidget.Pivot.Left,null,null,30);
                }
                else {
                    InitEveryDayPanel(mEveryDayMissionList);
                }
                break;
        }
    }


    private void InitEveryDayPanel(Dictionary<int,FactionMissionRate> EveryDayMissionList)
    {
        View.mEveryDayMissionPanel.enabled = true;
        EveryDayMissionList.ForEach(e => {
            if(!missionOptionObj.ContainsKey(e.Key))
            {
                EveryDayMissionOptionController ctrl = AddChild<EveryDayMissionOptionController,EveryDayMissionOption>(View.UIGrid_Grid.gameObject,EveryDayMissionOption.NAME);
                ctrl.InitDate(e.Key,e.Value.name);
                _disposable.Add(ctrl.GetClickEvt.Subscribe(d => { 
                    _OptionClickEvt.OnNext(d);
                    View.SelectEffect_Transform.position = ctrl.transform.position;
                }));
                missionOptionObj.Add(e.Key,ctrl.gameObject);
            }
        });
        View.UIGrid_Grid.enabled = true;
        View.UIGrid_Grid.Reposition();
        if(!View.SelectEffect_Transform.gameObject.activeSelf)
        {
            View.SelectEffect_Transform.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 点击背景关闭
    /// </summary>
    private void ClickDialogueBtn()
    {
        ProxyEveryDayMission.Close();
    }

    private void SureOpenPanel()
    {
        EveryDayMissionDataMgr.IsOpenCheckEveryMission = false;
        InitEveryDayPanel(mEveryDayMissionList);
    }
}
