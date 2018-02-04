// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  GuildManageViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using AppDto;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public partial interface IGuildManageViewController
{
    void UpdateView(IGuildMainData data);   //更新数据
}

public partial class GuildManageViewController
{

    private CompositeDisposable _disposable;

    private int _messageItemMax = 9;
    private int _guildMemberMax = 200;
    

    private Dictionary<GameObject, GuildMessageItemCellController> _messageItemDic = new Dictionary<GameObject, GuildMessageItemCellController>();

    List<messageInfoDto> _messageInfoList = GuildManageDto.messageInfo;

    //private GuildEventViewController _guildMessageCtr;

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView()
    {
        if (_disposable == null)
            _disposable = new CompositeDisposable();
        else
        {
            _disposable.Clear();
        }
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent()
    {
        _view.GuildMessageContent_UIRecycledList.onUpdateItem += UpdateMesageItem;
    }

    protected override void OnDispose()
    {
        _disposable = _disposable.CloseOnceNull();
        base.OnDispose();
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent()
    {
        _view.GuildMessageContent_UIRecycledList.onUpdateItem -= UpdateMesageItem;
    }

    public void UpdateView(IGuildMainData data)
    {
        InitGuildMessage();
        InitNotice(data);
        InitGuildInfo(data);
    }

    private void InitNotice(IGuildMainData data)
    {
        var list = data.GuildPosition;
        var pos = list.Find(e => e.id == data.PlayerGuildInfo.positionId);
        View.NoticeModificationBtn_UIButton.gameObject.SetActive(pos.updateNotice);
        View.NoticdContentLabel_UILabel.text = data.GuildBaseInfo.notice;
        View.ManifestoModificationBtn_UIButton.gameObject.SetActive(pos.updateMemo);
        View.ManifestoContentLabel_UILabel.text = data.GuildBaseInfo.memo;
    }

    #region 帮派信息
    private void InitGuildMessage()
    {
        for (int i = 0; i < _messageItemMax; i++)
        {
            var ctrl = AddChild<GuildMessageItemCellController, GuildMessageItemCell>(
                _view.GuildMessageContent_UIRecycledList.gameObject
                , GuildMessageItemCell.NAME);
            _messageItemDic.Add(ctrl.gameObject, ctrl);
        }
        UpdateScrollViewPos(_messageInfoList);
    }
    private void UpdateMesageItem(GameObject go, int itemIndex, int dataIndex)
    {
        if (_messageItemDic == null) return;
        GuildMessageItemCellController item = null;
        if (_messageItemDic.TryGetValue(go, out item))
        {
            var info = _messageInfoList.TryGetValue(dataIndex);
            if (info == null) return;
            item.SetData(info);
        }
    }
    public void UpdateScrollViewPos(IEnumerable<messageInfoDto> ShopItems)
    {
        View.GuildMessageContent_UIRecycledList.UpdateDataCount(ShopItems.ToList().Count, true);
        View.ScrollView_UIScrollView.ResetPosition();
    }
    #endregion

    private void InitGuildInfo(IGuildMainData data)
    {
        View.GuildIcon_UISprite.spriteName = "Big_Icon_2";
        View.GuildNameLbl_UILabel.text = data.GuildDetailInfo.baseInfo.name;
        View.GuildBossLbl_UILabel.text = data.GuildDetailInfo.baseInfo.bossName;
        View.MemberCountLabel_UILabel.text = string.Format("{0}/{1}", data.GuildDetailInfo.baseInfo.memberCount, data.GuildDetailInfo.baseInfo.maxMemberCount);
        View.GuildGradeLabel_UILabel.text = data.GuildDetailInfo.baseInfo.grade.ToString();
        View.GuildIdLabel_UILabel.text = data.GuildDetailInfo.baseInfo.showId.ToString();
        View.GuildAssetsLabel_UILabel.text = data.GuildDetailInfo.wealthInfo.assets.ToString();
        View.MaintainCostLabel_UILabel.text = data.GuildDetailInfo.wealthInfo.maintainAssets.ToString();
        View.ProsperityLabel_UILabel.text = data.GuildDetailInfo.wealthInfo.prosperity.ToString();
        View.PopularityLabel_UILabel.text = data.GuildDetailInfo.wealthInfo.activity.ToString();

    }

}

public class GuildManageDto
{
    public static List<messageInfoDto> messageInfo = new List<messageInfoDto> {
        new messageInfoDto("2017-03-09 05:30","恭喜！势力建筑升级到了1级！"),
        new messageInfoDto("2017-03-09 05:30","建筑升级消耗了33万势力资金，2700000粮草"),
        new messageInfoDto("2017-03-09 05:30","城主aaaaa和bbbbb喜结连理！"),
        new messageInfoDto("2017-03-09 05:30","恭喜！势力建筑升级到了2级！"),
        new messageInfoDto("2017-03-09 05:30","恭喜！势力建筑升级到了3级！"),
        new messageInfoDto("2017-03-09 05:30","恭喜！势力建筑升级到了4级！"),
        new messageInfoDto("2017-03-09 05:30","恭喜！势力建筑升级到了5级！"),
        new messageInfoDto("2017-03-09 05:30","恭喜！势力建筑升级到了6级！"),
        new messageInfoDto("2017-03-09 05:30","恭喜！势力建筑升级到了7级！"),
        new messageInfoDto("2017-03-09 05:30","恭喜！势力建筑升级到了8级！"),
        new messageInfoDto("2017-03-09 05:30","恭喜！势力建筑升级到了9级！"),
        new messageInfoDto("2017-03-09 05:30","恭喜！势力建筑升级到了10级！"),
        new messageInfoDto("2017-03-09 05:30","恭喜！势力建筑升级到了11级！"),
        new messageInfoDto("2017-03-09 05:30","恭喜！势力建筑升级到了12级！"),
        new messageInfoDto("2017-03-09 05:30","恭喜！势力建筑升级到了13级！"),
        new messageInfoDto("2017-03-09 05:30","恭喜！势力建筑升级到了14级！"),
        new messageInfoDto("2017-03-09 05:30","恭喜！势力建筑升级到了15级！"),
        new messageInfoDto("2017-03-09 05:30","恭喜！势力建筑升级到了级16！")
    };
    public static GuildInfoDto guildInfoDto = new GuildInfoDto("baafaf", "baoufba", 55,20, 4, 10841, 131, 20, 15, 51);
    public static string guildIcon = "Big_Icon_2";

}
public class messageInfoDto
{
    public string time;
    public string msg;

    public messageInfoDto(string time,string msg)
    {
        this.time = time;
        this.msg = msg;

    }
}

public class GuildInfoDto
{
    public string guildName;
    public string boss;
    public int totelMember;
    public int onlineMember;
    public int grade;
    public int id;
    public int assets;
    public int maintainCost;
    public int prosperity;
    public int popularity;

    public GuildInfoDto(string guildName, string boss, int totelMember,int onlineMember, int grade, int id, int assets, int maintainCost, int prosperity, int popularity)
    {
        this.guildName = guildName;
        this.boss = boss;
        this.totelMember = totelMember;
        this.onlineMember = onlineMember;
        this.grade = grade;
        this.assets = assets;
        this.maintainCost = maintainCost;
        this.prosperity = prosperity;
        this.popularity = popularity;
    }
}
