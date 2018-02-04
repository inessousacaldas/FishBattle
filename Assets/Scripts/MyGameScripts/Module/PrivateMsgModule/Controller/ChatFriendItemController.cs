// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  ChatFriendItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using UniRx;
using AppDto;

public partial class ChatFriendItemController
{
    private long _id = 0;
    private FriendInfoDto _infoDto = new FriendInfoDto();

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
 
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        Bg_UIButtonEvt.Subscribe(_ =>
        {
            clickItemStream.OnNext(_infoDto);
        });
    }

    protected override void OnDispose()
    {

    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    public void UpdateView(FriendInfoDto dto, bool showRed, bool isChose)
    {
        _id = dto.friendId;
        _infoDto = dto;
        if(dto.charactor as MainCharactor != null)
            UIHelper.SetPetIcon(View.Icon_UISprite, (dto.charactor as MainCharactor).gender==1?"101":"103");
        View.Name_UILabel.text = dto.name;
        View.Lv_UILabel.text = dto.grade.ToString();
        View.RedPot_UISprite.gameObject.SetActive(showRed);
        View.ChoseBg_UISprite.gameObject.SetActive(isChose);
    }

    readonly UniRx.Subject<FriendInfoDto> clickItemStream = new UniRx.Subject<FriendInfoDto>();
    public UniRx.IObservable<FriendInfoDto> OnClickItemStream
    {
        get { return clickItemStream; }
    }
}
