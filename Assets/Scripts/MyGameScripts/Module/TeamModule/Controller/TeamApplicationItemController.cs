// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  TeamApplicationItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using System.Collections.Generic;
using AppDto;
using MyGameScripts.Gameplay.Player;
using UniRx;

public interface ITeamApplicationItemController
{
    UniRx.IObservable<Unit> OnClickHandler { get; }
}

public partial class TeamApplicationItemController: ITeamApplicationItemController
{

    private CompositeDisposable _disposable;
    private Subject<Unit> _onclickEvt = new Subject<Unit>();
    public UniRx.IObservable<Unit> OnClickHandler { get { return _onclickEvt; } }  
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        _disposable = new CompositeDisposable();
        View.agreeBtn_UIButton.gameObject.SetActive(true);
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        _disposable.Add(agreeBtn_UIButtonEvt.Subscribe(_ => { _onclickEvt.OnNext(new Unit()); }));
    }

    protected override void OnDispose()
    {
        _disposable = _disposable.CloseOnceNull();
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
    }

    public void UpdateInfo(TeamRequestNotify joinRequest, int idx, string btntext)
    {
        _view.agreeLabel_UILabel.text = btntext;
        if (joinRequest == null)
        {
            View.nameLabel_UILabel.text = "";
            View.lvLabel_UILabel.text = "";
            View.factionSprite_UISprite.spriteName = "";
            return;
        }
        
        View.nameLabel_UILabel.text = joinRequest.playerNickname;
        View.lvLabel_UILabel.text = string.Format("Lv.{0}",joinRequest.playerGrade);
        _view.MagicSprite_UISprite.spriteName = GlobalAttr.GetMagicIcon(joinRequest.slotsElementLimit);
        _view.factionSprite_UISprite.spriteName = string.Format("faction_{0}", joinRequest.playerFactionId);
    }

    public void UpdateInfo(TeamPlayerDto playerDto)
    {
        _view.agreeLabel_UILabel.text = "邀请";
        if (playerDto == null)
        {
            View.nameLabel_UILabel.text = "";
            View.lvLabel_UILabel.text = "";
            View.factionSprite_UISprite.spriteName = "";
            return;
        }
        
        View.nameLabel_UILabel.text = playerDto.nickname;
        View.lvLabel_UILabel.text = string.Format("Lv.{0}", playerDto.grade);

        _view.factionSprite_UISprite.spriteName = string.Format("faction_{0}", playerDto.factionId);
        _view.MagicSprite_UISprite.spriteName = GlobalAttr.GetMagicIcon(playerDto.slotsElementLimit);
        if (playerDto.playerDressInfo.charactor == null)
            return; 

        UIHelper.SetPetIcon(View.iconSprite_UISprite, playerDto.playerDressInfo.charactor.texture.ToString());
    }

    public void SetGuildMember(GuildMemberDto dto)
    {
        _view.agreeLabel_UILabel.text = "邀请";
        if (dto == null)
        {
            View.nameLabel_UILabel.text = "";
            View.lvLabel_UILabel.text = "";
            View.factionSprite_UISprite.spriteName = "";
            return;
        }

        View.nameLabel_UILabel.text = dto.name;
        View.lvLabel_UILabel.text = string.Format("Lv.{0}", dto.grade);
        _view.factionSprite_UISprite.spriteName = string.Format("faction_{0}", dto.factionId);
        _view.MagicSprite_UISprite.spriteName = GlobalAttr.GetMagicIcon(1); //数据没有,先写死一个
        _view.factionSprite_UISprite.spriteName = string.Format("faction_{0}", dto.factionId);
        var maincharactor = DataCache.getDtoByCls<GeneralCharactor>(dto.charactorId);
        UIHelper.SetPetIcon(View.iconSprite_UISprite, maincharactor.texture.ToString());
    }
}
