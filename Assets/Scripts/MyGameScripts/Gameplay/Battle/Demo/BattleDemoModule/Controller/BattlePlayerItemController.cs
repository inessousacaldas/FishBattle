// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  BattlePlayerItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using MyGameScripts.Gameplay.Player;

public partial class BattlePlayerItemController
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

    public void SetPlayerInfo()
    {
        UIHelper.SetPetIcon(_view.IconSprite_UISprite, "");
        _view.MagicSprite_UISprite.spriteName = GlobalAttr.GetMagicIcon(5);
        _view.SkillSprite_UISprite.spriteName = "312002";
        _view.NameLb_UILabel.text = "aaaaaaaa";
        _view.LvLb_UILabel.text = string.Format("Lv.{0}", 11);
        _view.BattlePlayerItem_UISprite.spriteName = string.Format("item_ib_{0}", 1);
    }

}
