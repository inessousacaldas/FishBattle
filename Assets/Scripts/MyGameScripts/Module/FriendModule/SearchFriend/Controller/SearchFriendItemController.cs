// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  SearchFriendItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using AppDto;
using UniRx;

public partial class SearchFriendItemController
{
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        AddBtn_UIButtonEvt.Subscribe(_ =>
        {
            clickItemStream.OnNext(_id);
        });

        FriendIcon_UIButtonEvt.Subscribe(_ =>
        {
            var ctrl = FriendDetailViewController.Show<FriendDetailViewController>(FriendDetailView.NAME, UILayerType.ThreeModule, false, true);
            ctrl.UpdateView(_infoDto);
        });
    }

    protected override void OnDispose()
    {

    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        AddBtn_UIButtonEvt.CloseOnceNull();
    }

    private long _id = 0;
    private FriendInfoDto _infoDto;

    public void UpdateView(FriendInfoDto dto)
    {
        //View.CampIcon_UISprite = dto.countryId;
        //View.FactionNameLabel_UILabel.text = ;

        if (dto.charactor as MainCharactor != null)
            UIHelper.SetPetIcon(View.FriendIcon_UIButton.sprite, (dto.charactor as MainCharactor).gender == 1 ? "101" : "103");
        View.NameLabel_UILabel.text = dto.name;
        View.FactionLvLabel_UILabel.text = dto.grade.ToString();
        if (dto.faction != null)
            View.FactionNameLabel_UILabel.text = dto.faction.name;
        View.AddBtn_UIButton.gameObject.SetActive(!FriendDataMgr.DataMgr.IsMyFriend(dto.friendId));
        View.AddedLabel_UILabel.gameObject.SetActive(FriendDataMgr.DataMgr.IsMyFriend(dto.friendId));
        View.RelationshipLabel_UILabel.text = FriendDataMgr.DataMgr.IsMyFriend(dto.friendId) ? "好友" : "陌生人";

        _id = dto.friendId;
        _infoDto = dto;
    }

    readonly UniRx.Subject<long> clickItemStream = new UniRx.Subject<long>();
    public UniRx.IObservable<long> OnClickItemStream
    {
        get { return clickItemStream; }
    }

}
