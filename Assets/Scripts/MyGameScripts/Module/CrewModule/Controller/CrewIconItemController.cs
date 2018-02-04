// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  CrewIconItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using AppDto;
using UniRx;
using UnityEngine;

public partial class CrewIconItemController
{
    private CompositeDisposable _disposable = new CompositeDisposable();
    private Subject<ICrewItemData> _clickEvt = new Subject<ICrewItemData>();
    public UniRx.IObservable<ICrewItemData> GetClickHandler { get { return _clickEvt; } }

    private CrewItemData _data;
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
 
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        _disposable.Add(CrewIconItem_UIButtonEvt.Subscribe(_ => _clickEvt.OnNext(_data)));
    }

    protected override void OnDispose()
    {
        _disposable = _disposable.CloseOnceNull();
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    public void SetCrewInfo(CrewInfoDto dto, int idx, Crew.BattleCrewType state)
    {
        Crew crew = DataCache.getDtoByCls<GeneralCharactor>(dto.crewId) as Crew;
        _data = CrewItemData.Create(idx, dto.id, dto.crewId);
        //_view.LvLb_UILabel.text = string.Format("Lv.{0}", dto.grade);
        //_view.NameLb_UILabel.text = crew.name;
        UIHelper.SetPetIcon(_view.Icon_UISprite, crew.icon);
        _view.State_UISprite.gameObject.SetActive(state != (int) Crew.BattleCrewType.Default);

        switch (state)
        {
            case Crew.BattleCrewType.Default:
                _view.State_UISprite.spriteName = "";
                break;
            case Crew.BattleCrewType.MainCrew:
                _view.State_UISprite.spriteName = "flag_general";
                break;
            case Crew.BattleCrewType.AssistCrew:
                _view.State_UISprite.spriteName = "flag_auxiliary";
                break;
        }
    }

    public void SetMemberInfo(TeamMemberDto member)
    {
        SetNameLv(member.nickname);
        _view.LvLb_UILabel.text = string.Format("Lv.{0}", member.grade);
        var isPlayer = member.id == ModelManager.Player.GetPlayerId();
        SetIconBg(isPlayer);
        UIHelper.SetPetIcon(_view.Icon_UISprite, member.playerDressInfo.charactor.texture.ToString());
    }

    public void SetCasePosition(CasePositionDto dto)
    {
        Crew crew = DataCache.getDtoByCls<GeneralCharactor>(dto.crewSufaceId) as Crew;
        if (crew == null)
            SetNameLv(ModelManager.Player.GetPlayerName());
        else
            SetNameLv(crew.name);
        _view.LvLb_UILabel.text = string.Format("Lv.{0}", dto.grade);
        var playerIcon = ModelManager.Player.GetPlayer().charactor.texture;
        UIHelper.SetPetIcon(_view.Icon_UISprite, crew == null ? playerIcon.ToString() : crew.icon);
        var isPlayer = dto.crewId == ModelManager.Player.GetPlayerId();
        SetIconBg(isPlayer);
    }

    public void SetCrewPosition(CrewPositionNotify dto)
    {
        Crew crew = DataCache.getDtoByCls<GeneralCharactor>(dto.crewId) as Crew;
        if (crew == null)
            SetNameLv(ModelManager.Player.GetPlayerName());
        else
            SetNameLv(crew.name);

        _view.LvLb_UILabel.text = string.Format("Lv.{0}", dto.grade);
        var playerIcon = ModelManager.Player.GetPlayer().charactor.texture;
        UIHelper.SetPetIcon(_view.Icon_UISprite, crew == null ? playerIcon.ToString() : crew.icon);
        var isPlayer = dto.crewId == ModelManager.Player.GetPlayerId();
        SetIconBg(isPlayer);
    }

    public void SetNullItem()
    {
        _view.NameLb_UILabel.text = "";
        _view.LvLb_UILabel.text = "";
        _view.Icon_UISprite.spriteName = "";
        _view.BackGround_UISprite.spriteName = "IconBgPurple";
        _view.IconBg_UISprite.spriteName = "crewIconBgPurple";
    }

    private void SetIconBg(bool isPlayer)
    {
        _view.BackGround_UISprite.spriteName = isPlayer ? "IconBgYellow" : "IconBgBlue";
        _view.IconBg_UISprite.spriteName = isPlayer ? "crewIconBgYellow" : "crewIconBgBlue";
    }

    private void SetNameLv(string name)
    {
        if (name.Length > 5)
            _view.NameLb_UILabel.text = string.Format("{0}...", name.Substring(0, 4));
        else
            _view.NameLb_UILabel.text = name;
    }

    public Vector3 GetAnchorPos { get { return _view.Anchor_Transform.position; } }

    public bool Selected { set { _view.Select_UISprite.gameObject.SetActive(value);} }
}
