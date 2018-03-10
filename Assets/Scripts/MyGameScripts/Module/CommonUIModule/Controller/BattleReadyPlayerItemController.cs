// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  BattleReadyPlayerItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using AppDto;
using MyGameScripts.Gameplay.Player;

public partial class BattleReadyPlayerItemController
{
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
 
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {

    }

    protected override void OnDispose()
    {

    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    public void SetPlayerInfo(BattleReadyPlayerInfoDto dto)
    {
        _view.NameLb_UILabel.text = dto.nickname;
        _view.LvLb_UILabel.text = string.Format("Lv.{0}", dto.grade);
        var charactor = DataCache.getDtoByCls<GeneralCharactor>(dto.charactorId);
        if (charactor is MainCharactor)
            _view.Icon_UISprite.spriteName = string.Format("Player_{0}", (charactor as MainCharactor).texture);
        _view.Faction_UISprite.spriteName = string.Format("faction_{0}", dto.factionId);
        _view.Attribute_UISprite.spriteName = GlobalAttr.GetMagicIcon(dto.quartzPropertyId);
    }

}
