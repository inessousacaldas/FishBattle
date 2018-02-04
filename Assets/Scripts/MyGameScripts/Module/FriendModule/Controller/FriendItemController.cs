// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  FriendItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using AppDto;
using System;
using UnityEngine;
using UniRx;

public partial class FriendItemController
{
    private long _id;
    private long _offLineTime;
    private const int hourToDay = 24;
    private const int minToHour = 60;
    private const int secToMin = 60;

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView()
    {

    }

    // 客户端自定义事件
    protected override void RegistCustomEvent()
    {
        Bg_UIButtonEvt.Subscribe(_ =>
        {
            clickChatStream.OnNext(_id);
        });

        ArrowBtn_UIButtonEvt.Subscribe(_ =>
        {
            clickItemStream.OnNext(_id);
        });
    }

    protected override void OnDispose()
    {

    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent()
    {

    }

    //用特定接口类型数据刷新界面
    public void UpdateView(FriendViewTab tab, FriendInfoDto dto)
    {
        _id = dto.friendId;
        _offLineTime = dto.offlineTime;

        //玩家头像
        if (dto.charactor as MainCharactor != null)
            UIHelper.SetPetIcon(View.FriendIcon_UISprite, (dto.charactor as MainCharactor).gender == 1 ? "101" : "103");
        //View.CampIcon_UISprite
        View.LevelLabel_UILabel.text = dto.grade.ToString();
        View.NameLabel_UILabel.text = dto.name;
        UIHelper.SetCommonIcon(View.FactionIcon_UISprite, string.Format("faction_{0}", dto.factionId));
        View.FactionLabel_UILabel.text = dto.faction.name;
        View.DegreeLabel_UILabel.text = dto.degree.ToString();

        switch(tab)
        {
            case FriendViewTab.BlackFriend:
                SetShowDegreeAndOfftime(false);
                SetGray(true);
                break;
            case FriendViewTab.RecentlyTeammates:
                SetShowDegreeAndOfftime(false);
                SetGray(!dto.online);
                break;
            case FriendViewTab.MyFriend:
                SetShowDegreeAndOfftime(true);
                SetGray(!dto.online);
                break;
        }
    }

    private void SetGray(bool isGray)
    {
        View.FriendIcon_UISprite.isGrey = isGray;
        View.IconBG_UISprite.isGrey = isGray;
        View.ArrowBtn_UIButton.sprite.isGrey = isGray;
        View.ActiveName_UILabel.text = "空闲";
        View.ActiveName_UILabel.pivot = UIWidget.Pivot.Center;
        View.ActiveName_UILabel.transform.localPosition = new Vector3(146, View.ActiveName_UILabel.transform.localPosition.y);

        if (isGray)
            SetOffLineTime();
    }

    private void SetShowDegreeAndOfftime(bool isShow)
    {
        View.DegreeIcon_UISprite.gameObject.SetActive(isShow);
        View.ActiveName_UILabel.gameObject.SetActive(isShow);
        View.LevelLabel_UILabel.gameObject.SetActive(isShow);
    }

    private void SetOffLineTime()
    {
        var timeSecond = (SystemTimeManager.Instance.GetUTCTimeStamp() - _offLineTime) / 1000;
        string str = string.Empty;
        var day = timeSecond / (hourToDay * minToHour * secToMin);
        var hour = timeSecond / (minToHour * secToMin);
        var min = timeSecond / secToMin;

        if(day > 0)
            str = day>10 ? "离线 十天以上" : string.Format("离线 {0}天", day);
        else if (hour > 0)
            str = string.Format("离线 {0}小时", hour);
        else
            str = min > 0 ? string.Format("离线 {0}分钟", min) : "离线 刚刚";

        View.ActiveName_UILabel.text = str;
        View.ActiveName_UILabel.pivot = UIWidget.Pivot.Right;
        View.ActiveName_UILabel.transform.localPosition = new Vector3(192, View.ActiveName_UILabel.transform.localPosition.y);
    }

    public int GetHeigt()
    {
        return View.Bg_UIButton.sprite.height;
    }

    readonly UniRx.Subject<long> clickItemStream = new UniRx.Subject<long>();
    public UniRx.IObservable<long> OnClickItemStream
    {
        get { return clickItemStream; }
    }

    readonly UniRx.Subject<long> clickChatStream = new UniRx.Subject<long>();
    public UniRx.IObservable<long> OnClickChatStream
    {
        get { return clickChatStream; }
    }
}
