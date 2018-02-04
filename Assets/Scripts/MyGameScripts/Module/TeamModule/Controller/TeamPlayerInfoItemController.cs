// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  TeamPlayerInfoItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using System.Collections.Generic;
using AppDto;
using MyGameScripts.Gameplay.Player;
using UniRx;
using UnityEngine;

public partial class TeamPlayerInfoItemController
{
    private CompositeDisposable _disposable;

    private ModelDisplayController _modelController;
    private Subject<Unit> TeamPlayerInfoItem_UIButtonEvt;
    private TeamMemberDto teamMemberDto;
    public TeamMemberDto MemberDto{get { return teamMemberDto;}}
    private ModelDisplayController ModelCom
    {
        get { return _modelController; }
    }

    public Action<TeamPlayerInfoItemController, GameObject, Vector3> OnDragDropReleaseCallBack;
    
    private Subject<Unit> _addPlyerEvt = new Subject<Unit>();
    

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        _disposable = new CompositeDisposable();

        _modelController = AddChild<ModelDisplayController, ModelDisplayUIComponent>(
            View.ModelAnchor_Transform
            , ModelDisplayUIComponent.NAME);

        _modelController.Init (200, 200);
        _modelController.SetBoxColliderEnabled(false);
        _view.TeamPlayerInfoItem_UIDragDropItem.OnDragDropReleaseHandler = OnDragDropReleaseHandler;
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        TeamPlayerInfoItem_UIButtonEvt = View.TeamPlayerInfoItem_UIButton.AsObservable();
        _addPlyerEvt = View.AddBtn_UIButton.AsObservable();
    }

    protected override void OnDispose()
    {
        _disposable = _disposable.CloseOnceNull();
        View.TeamPlayerInfoItem_UIDragDropItem.enabled = false;
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        TeamPlayerInfoItem_UIButtonEvt.CloseOnceNull();
    }

    public UniRx.IObservable<Unit> OnTeamPlayerInfoItem_UIButtonClick{
        get {return TeamPlayerInfoItem_UIButtonEvt;}
    }

    public UniRx.IObservable<Unit> GetAddEvt { get { return _addPlyerEvt; } }

    public void ResetItem(bool b = false){
        teamMemberDto = null;
        _modelController.CleanUpModel();

        _view.infoGroup.SetActive(b);
        _view.Position.gameObject.SetActive(b);
        _view.MainCrewIcon_UISprite.gameObject.SetActive(b);
        _view.ModelAnchor_Transform.gameObject.SetActive(b);

        _view.AddBtn_UIButton.gameObject.SetActive(!b);
        _view.DescLb.gameObject.SetActive(!b);
    }

    private void OnDragDropReleaseHandler(GameObject pSurface, Vector3 localPos)
    {
        if (OnDragDropReleaseCallBack != null)
            OnDragDropReleaseCallBack(this, pSurface, localPos);
    }

    public bool EnableDrag {
        get { return View.TeamPlayerInfoItem_UIDragDropItem.enabled;}
        set { View.TeamPlayerInfoItem_UIDragDropItem.enabled = value; }
    }

    #region  UpdateView func  //用特定接口类型数据刷新界面

    public void UpdateView(PlayerDto playerDto, 
        int mainCrewId = 0, 
        int grade = 0,
        int crewQuality =0,
        int crewslots = 0)
    {
        teamMemberDto = null;
        if (playerDto == null)
        {
            ResetItem();
        }
        else
        {
            ResetItem(true);

            UpdateInfo(playerDto.name, grade);
            UpdateTeamPosInfo(MemberPos.None);
            _modelController.SetupModel(playerDto.charactor);
            ModelCom.SetModelScale(1f);
            ModelCom.SetModelOffset(-0.15f);

            Crew crew = DataCache.getDtoByCls<GeneralCharactor>(mainCrewId) as Crew;
            _view.MainCrewIcon_UISprite.gameObject.SetActive(crew != null);
            UIHelper.SetPetIcon(_view.MainCrewIcon_UISprite, crew == null ? "" : crew.icon);
            _view.MagicType_UISprite.spriteName = GlobalAttr.GetMagicIcon(ModelManager.Player.GetSlotsElementLimit);
            _view.Faction_UISprite.spriteName = string.Format("faction_{0}", playerDto.factionId);
            _view.Position_UISprite.enabled = true;
            _view.CrewFaction_UISprite.spriteName = crew == null ? "" : crew.typeIcon;
            _view.CrewMagic_UISprite.spriteName = crew == null ? "" : GlobalAttr.GetMagicIcon(crewslots);
            _view.CrewIconBG_UISprite.spriteName = GetCrewQuality(crewQuality);
        }
    }

    //玩家
    public void UpdateView(TeamMemberDto memberDto, MemberPos memberpos, bool isCommander)
    {
        
        if (memberDto == null)
        {
            ResetItem();
        }
        else
        {
            ResetItem(true);
            teamMemberDto = memberDto;
            SetMaskLbl("");
            UpdateInfo(memberDto.nickname, memberDto.grade);
            _modelController.SetupModel(memberDto.playerDressInfo.charactor);
            ModelCom.mUITexture.color = Color.white;
            ModelCom.SetModelScale(1f);
            ModelCom.SetModelOffset(-0.15f);
            _view.MagicType_UISprite.spriteName = GlobalAttr.GetMagicIcon(memberDto.slotsElementLimit);
            _view.Faction_UISprite.spriteName = string.Format("faction_{0}", memberDto.factionId);
            _view.Position_UISprite.enabled = true;

            if (isCommander)
                UpdateTeamPosInfo(MemberPos.Command);
            else
                UpdateTeamPosInfo((int)memberpos > memberDto.memberStatus ? memberpos : (MemberPos)memberDto.memberStatus);

            Crew crew;
            var dto = memberDto.crewPositionNotifys.Find(d =>
                        d.position%TeamDataMgr.TeamDefaultMemberCnt ==
                        memberDto.position%TeamDataMgr.TeamDefaultMemberCnt);
            if (dto != null)
            {
                crew = DataCache.getDtoByCls<GeneralCharactor>(dto.crewId) as Crew;
                _view.MainCrewIcon_UISprite.gameObject.SetActive(crew != null);
                UIHelper.SetPetIcon(_view.MainCrewIcon_UISprite, crew == null ? "" : crew.icon);
                _view.CrewFaction_UISprite.spriteName = crew == null ? "" : crew.typeIcon;
                _view.CrewMagic_UISprite.spriteName = crew == null ? "" : GlobalAttr.GetMagicIcon(dto.slotsElementLimit);
                _view.CrewIconBG_UISprite.spriteName = GetCrewQuality(dto.quality);
            }
            else
                _view.MainCrewIcon_UISprite.gameObject.SetActive(false);

            bool b = memberDto.memberStatus == (int)TeamMemberDto.TeamMemberStatus.Away
                || memberDto.memberStatus == (int)TeamMemberDto.TeamMemberStatus.Offline;

            _view.Position_UISprite.gameObject.SetActive(!b);
            _view.StateSprite_UISprite.gameObject.SetActive(b);
            _view.StateSprite_UISprite.spriteName = memberDto.memberStatus == (int) TeamMemberDto.TeamMemberStatus.Away
                ? "flag_away"
                : "flag_offline";
        }
    }


    //伙伴
    public void UpdateView(Crew crew, int grade = 0, int solts = 0)
    {
        ResetItem(true);

        _modelController.SetupModel(InitModelStyleInfo(crew.modelId));
        UpdateInfo(crew.name, grade);
        ModelCom.SetModelScale(1f);
        ModelCom.SetModelOffset(-0.15f);
        UpdateTeamPosInfo(MemberPos.Crew);
        _view.MainCrewIcon_UISprite.gameObject.SetActive(false);
        _view.MagicType_UISprite.spriteName = GlobalAttr.GetMagicIcon(solts);
        _view.Faction_UISprite.spriteName = crew.typeIcon;
        _view.Position_UISprite.enabled = false;
    }
    #endregion

    private void UpdateTeamPosInfo(MemberPos status)
    {
        var flagName = GetPosFlagName(status);
        SetupPosFlag(flagName);
    }

    private string GetPosFlagName(MemberPos status)
    {
        var flagName = string.Empty;

        switch (status)
        {
            case MemberPos.None:
                flagName = "自己";
                break;
            case MemberPos.Leader:
                flagName = "队长";
                break;
            case MemberPos.Crew:
                flagName = "伙伴";
                break;
            case MemberPos.Command:
                flagName = "指挥";
                break;
            case MemberPos.Member:
                flagName = "队员";
                break;
        }
        return flagName;
    }

    private string GetCrewQuality(int quality)
    {
        var str = string.Empty;
        switch ((Crew.CrewQuantityEnum)quality)
        {
            case Crew.CrewQuantityEnum.Unknown:
                str = "CommonRing";
                break;
            case Crew.CrewQuantityEnum.BLUE:
                str = "crew_ib_3_r";
                break;
            case Crew.CrewQuantityEnum.PURPLE:
                str = "crew_ib_4_r";
                break;
            case Crew.CrewQuantityEnum.ORANGE:
                str = "crew_ib_5_r";
                break;
            case Crew.CrewQuantityEnum.RED:
                str = "crew_ib_6_r";
                break;
            default:
                str = "CommonRing";
                break;
        }
        return str;
    }

    public void SetMaskLbl(string tips){
        if(string.IsNullOrEmpty(tips)){
            View.maskLbl_UILabel.cachedGameObject.SetActive(false);
        }else{
            View.maskLbl_UILabel.text = tips;
            View.maskLbl_UILabel.cachedGameObject.SetActive(true);
        }
    }

    private void SetupPosFlag(string flagName)
    {
        if (string.IsNullOrEmpty(flagName))
        {
            View.PositionLb_UILabel.enabled = false;
        }
        else
        {
            View.PositionLb_UILabel.enabled = true;
            View.PositionLb_UILabel.text = flagName;
        }
    }

    private void UpdateInfo(string name,int lv)
    {
        View.nameLabel_UILabel.text = name;
        if (lv > 0)
            View.lvLabel_UILabel.text = string.Format("Lv.{0}", lv);
        else
            View.lvLabel_UILabel.text = "";
    }

    private ModelStyleInfo InitModelStyleInfo(int id)
    {
        ModelStyleInfo model = new ModelStyleInfo();
        model.defaultModelId = id;
        return model;
    }

    public enum MemberPos
    {
        Crew = -1,
        None = 0,
        Leader = 1,
        Member = 2,
        Command = 5
    }

    public bool HideDragDropItem
    {
        get { return _view.TeamPlayerInfoItem_DragDropItem; }
        set { _view.TeamPlayerInfoItem_DragDropItem.enabled = value; }
    }

    public GameObject Anchor { get { return _view.Anchor; } }
}
