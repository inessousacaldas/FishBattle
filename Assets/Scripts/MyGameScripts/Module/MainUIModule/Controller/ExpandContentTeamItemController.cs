// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  ExpandContentTeamItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using AppDto;
using MyGameScripts.Gameplay.Player;

public partial class ExpandContentTeamItemController
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

    //用特定接口类型数据刷新界面
    public void UpdateView(TeamMemberDto memberInfo)
    {
        if (memberInfo == null)
            this.gameObject.SetActive(false);
        else
        {
            this.gameObject.SetActive(true);
            UIHelper.SetPetIcon(View.Icon_UISprite, memberInfo.playerDressInfo.charactor.texture.ToString());
            View.Icon_UISprite.isGrey = false;
            _view.magicIcon_UISprite.spriteName = GlobalAttr.GetMagicIcon(memberInfo.slotsElementLimit);
            _view.factionIcon_UISprite.spriteName = string.Format("faction_{0}", memberInfo.factionId);
            View.playerLevel_UILabel.text = string.Format("Lv.{0}", memberInfo.grade);
            View.playerName_UILabel.text = memberInfo.nickname;

            if (memberInfo.memberStatus == (int) TeamMemberDto.TeamMemberStatus.Away)
            {
                _view.Leave_UISprite.gameObject.SetActive(true);
                _view.Leave_UISprite.spriteName = "Team_leave";
            }
            else if (memberInfo.memberStatus == (int) TeamMemberDto.TeamMemberStatus.Offline)
            {
                _view.Leave_UISprite.gameObject.SetActive(true);
                _view.Leave_UISprite.spriteName = "Team_leaveForever";
                View.Icon_UISprite.isGrey = true;
            }
            else
            {
                _view.Leave_UISprite.gameObject.SetActive(false);
            }
        }
    }

}
