// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  FlowerFriendItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using UniRx;
using AppDto;

public partial class FlowerFriendItemController
{
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
 
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        Bg_UIButtonEvt.Subscribe(_ =>
        {
            clickItemStream.OnNext(_id);
        });
    }

    protected override void OnDispose()
    {
        
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    private long _id;
    public void UpdateView(FriendInfoDto dto, bool isChose)
    {
        _id = dto.friendId;
        if(dto.charactor != null)
            UIHelper.SetPetIcon(View.Icon_UISprite, (dto.charactor as MainCharactor).gender == 1 ? "101" : "103");
        else
            UIHelper.SetPetIcon(View.Icon_UISprite, "101");
        View.Name_UILabel.text = dto.name;
        //View.CampIcon_UISprite
        View.DegreeLabel_UILabel.text = dto.degree.ToString();
        View.Lv_UILabel.text = dto.grade.ToString();
        View.ChoseBg_UISprite.enabled = isChose;
    }

    readonly UniRx.Subject<long> clickItemStream = new UniRx.Subject<long>();
    public UniRx.IObservable<long> OnClickItemStream
    {
        get { return clickItemStream; }
    }
}
