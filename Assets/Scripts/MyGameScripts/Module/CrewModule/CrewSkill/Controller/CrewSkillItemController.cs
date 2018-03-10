// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  CrewSkillItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using AppDto;
using System;
using UnityEngine;

public partial interface ICrewSkillItemController
{
    void UpdateView(ICrewSkillVO data);
    void UpdateMagicView(bool isOpen, int openLv, CrewSkillMagicVO magicVO = null);
    //void UpdatePassiveView();
    CrewSkillItemController.MagicSkillState SkillState { get; }
    ICrewSkillVO SkillVO { get; }
    PsvItemData PsvItemData { get; }
    PassiveSkillBook PsvBookData { get; }
    bool GetItemInBag { get; }
}

public partial class CrewSkillItemController
{

    public enum MagicSkillState
    {
        None,
        Locked,     //还未解锁
        NoEquiped,   //只是开放，并没有装备技能
        Equiped  //装备了技能
    }

    private MagicSkillState _skillState = MagicSkillState.None;
    public MagicSkillState SkillState { get { return _skillState; } }

    private ICrewSkillVO _skillVO;
    public ICrewSkillVO SkillVO
    {
        get { return _skillVO; }
    }

    private PsvItemData _psvItemData = new PsvItemData();
    public PsvItemData PsvItemData
    {
        get { return _psvItemData; }
    }

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView()
    {

    }

    // 客户端自定义事件
    protected override void RegistCustomEvent()
    {

    }

    protected override void OnDispose()
    {
        _psvBookData = null;
        _skillState = MagicSkillState.None;
        _skillVO = null;
        _psvItemData = null;

    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent()
    {

    }

    #region 战技
    public void UpdateView(ICrewSkillVO data)
    {
        _skillVO = data;

        UIHelper.SetUITexture(View.SkillIcon_UITexture, data.Icon, false);
        _view.AddIcon_UISprite.gameObject.SetActive(false);
    }
    #endregion

    #region 魔法
    public void UpdateMagicView(bool isOpen, int openLv, CrewSkillMagicVO magicVO = null)
    {
        _skillVO = magicVO;
        _skillState = isOpen ? magicVO == null ? MagicSkillState.NoEquiped : MagicSkillState.Equiped
                                       : MagicSkillState.Locked;

        if (magicVO != null)
        {
            UIHelper.SetUITexture(View.SkillIcon_UITexture, magicVO.magicVO.icon, false);
        }
        else
        {
            View.SkillIcon_UITexture.mainTexture = null;
        }
    }
    #endregion
    #region 技巧
    public void UpdatePsvView(CrewPassiveSkill data, PassiveSkillDto dto)
    {

        _psvItemData.psvVO = data;
        _psvItemData.psvDto = dto;
        _psvItemData.state = PassiveState.HaveItem;
        UIHelper.SetUITexture(View.SkillIcon_UITexture, data.icon, false);
    }

    public void UpdatePsvView(int unLockNum, int idx)
    {
        if (unLockNum > idx)
        {
            //格子解锁
            _psvItemData.state = PassiveState.NeedItem;
            _view.AddIcon_UISprite.spriteName = "Btn_add_skill";

        }
        else
        {
            //格子未解锁
            _psvItemData.state = PassiveState.Lock;
            _view.AddIcon_UISprite.spriteName = "ect_Lock";
        }
    }
    public void ClearTexture()
    {
        View.SkillIcon_UITexture.mainTexture = null;
    }

    #region 技能书
    private PassiveSkillBook _psvBookData;
    public PassiveSkillBook PsvBookData { get { return _psvBookData; } }

    private bool isItemInBag = false;
    public void SetItemInBag(bool set)
    {
        isItemInBag = set;
    }
    public bool GetItemInBag
    {
        get { return isItemInBag; }
    }
    public void SetPsvBookData(PassiveSkillBook book)
    {
        _psvBookData = book;
    }
    public void UpdatePsvBookView(string icon, string num)
    {
        UIHelper.SetUITexture(View.SkillIcon_UITexture, _psvBookData.skillId.ToString(), false);
        View.NumLbl_UILabel.text = num;
        View.AddIcon_UISprite.gameObject.SetActive(false);
    }

    public void SetParent(Transform p)
    {
        View.transform.parent = p;
    }

    #endregion
    #endregion

    #region 研修

    public void UpdateTrainingView(CrewSkillCraftsVO before, CrewSkillCraftsVO after)
    {
        UIHelper.SetUITexture(View.SkillIcon_UITexture, after.Icon, false);

        SetQualityMark(after.cfgVO.quality);
        SetArrowMark(before.cfgVO.quality, after.cfgVO.quality);
    }
    public void UpdateTrainingView(CrewSkillCraftsVO before)
    {
        UIHelper.SetUITexture(View.SkillIcon_UITexture, before.Icon, false);

        SetQualityMark(before.cfgVO.quality);
        SetArrowMark(before.cfgVO.quality);
    }
        public void UpdateNone(CrewSkillCraftsVO before)
    {
        View.LevelMark_UISprite.gameObject.SetActive(false);
        View.ArrowsMark_UISprite.gameObject.SetActive(false);
        View.SkillIcon_UITexture.mainTexture = null;
    }
    private void SetQualityMark(int quality)
    {
        string icon = "ect_SkillLevel_1";
        if (quality == 6) icon = "ect_SkillLevel_3";
        if (quality == 5) icon = "ect_SkillLevel_2";
        View.LevelMark_UISprite.spriteName = icon;
        View.LevelMark_UISprite.gameObject.SetActive(true);
    }
    private void SetArrowMark(int befQuarlity, int aftQuarlity = -1)
    {
        if (aftQuarlity == -1)
        {
            View.ArrowsMark_UISprite.gameObject.SetActive(false);
            return;
        }
        if (aftQuarlity == befQuarlity)
            View.ArrowsMark_UISprite.gameObject.SetActive(false);
        else
        {
            View.ArrowsMark_UISprite.spriteName = aftQuarlity > befQuarlity ? "Arrow_Up" : "Arrow_Down";
            View.ArrowsMark_UISprite.gameObject.SetActive(true);
        }
    }

    #endregion

}
