// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  EveryDayMissionOptionController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using UniRx;

public partial class EveryDayMissionOptionController
{
    public int missionID;
    private Subject<int> _clickEvt = new Subject<int>();
    public UniRx.IObservable<int> GetClickEvt { get { return _clickEvt; } }
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView()
    {

    }

    // 客户端自定义事件
    protected override void RegistCustomEvent()
    {
        if(_view.EveryDayMissionOption_UIButton != null)
        {
            EventDelegate.Add(_view.EveryDayMissionOption_UIButton.onClick,OnClickHandler);
        }
        if(_view.EveryDayMissionStyle_UIButton != null) {
            EventDelegate.Add(_view.EveryDayMissionStyle_UIButton.onClick,OnSwichStyle);
        }
    }

    protected override void OnDispose()
    {

    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent()
    {

    }

    public void InitDate(int MissionID,string des)
    {
        missionID = MissionID;
        View.Label_UILabel.text = des;
    }

    void OnClickHandler()
    {
        _clickEvt.OnNext(missionID);
    }

    void OnSwichStyle() {
        _clickEvt.OnNext(0);
    }

}
