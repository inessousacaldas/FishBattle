// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  AssistFriendItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using AppDto;
using UniRx;

public partial class AssistFriendItemController
{
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
 
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        InviteBtn_UIButtonEvt.Subscribe(_ =>
        {
            if(_isHelped)
            {
                TipManager.AddTip("该玩家今天已协助过你了");
                return;
            }
            clickInviteStream.OnNext(_id);
        });
    }

    protected override void OnDispose()
    {

    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    private long _id = 0;
    private bool _isHelped = false;
    public void UpdateView(FriendInfoDto dto, bool isHelped)
    {
        _id = dto.friendId;
        _isHelped = isHelped;
        UIHelper.SetPetIcon(View.Icon_UISprite, (dto.charactor as MainCharactor).gender == 1 ? "101" : "103");
        View.Name_UILabel.text = dto.name;
    }

    readonly UniRx.Subject<long> clickInviteStream = new UniRx.Subject<long>();
    public UniRx.IObservable<long> OnClickInviteStream
    {
        get { return clickInviteStream; }
    }
}
