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

    private List<GuildEventDto> _messageInfoList = null;

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
        for (int i = 0; i < _messageItemMax; i++)
        {
            var ctrl = AddChild<GuildMessageItemCellController, GuildMessageItemCell>(
                _view.GuildMessageContent_UIRecycledList.gameObject
                , GuildMessageItemCell.NAME);
            _messageItemDic.Add(ctrl.gameObject, ctrl);
        }
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent()
    {
        _view.GuildMessageContent_UIRecycledList.onUpdateItem += UpdateMesageItem;
    }

    protected override void OnDispose()
    {
        _messageItemDic.Clear();
        _disposable = _disposable.CloseOnceNull();
        _messageInfoList = null;
        base.OnDispose();
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent()
    {
        _view.GuildMessageContent_UIRecycledList.onUpdateItem -= UpdateMesageItem;
    }

    public void UpdateView(IGuildMainData data)
    {
        UpdateGuildMessage(data);
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

    #region 帮派事件
    private void UpdateGuildMessage(IGuildMainData data)
    {
        _messageInfoList = data.GuildDetailInfo.magEvents.events;
        View.GuildMessageContent_UIRecycledList.UpdateDataCount(_messageInfoList.Count, true);
    }
    private void UpdateMesageItem(GameObject go, int itemIndex, int dataIndex)
    {
        if (_messageItemDic == null || _messageInfoList == null) return;
        GuildMessageItemCellController item = null;
        if (!_messageItemDic.TryGetValue(go, out item)) return;
        var info = _messageInfoList.TryGetValue(dataIndex);
        if (info == null) return;
        item.SetData(info);
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

