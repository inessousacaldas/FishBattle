// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  RoleSkillCraftsItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using AppDto;
using UniRx;

public partial interface IRoleSkillCraftsItemController
{
    UniRx.IObservable<Unit> GetPressHandler { get; }
    UniRx.IObservable<Unit> GetItemClickHandler { get; }
}

public partial class RoleSkillCraftsItemController
{
    public bool isSCraftsCtrl;
    public bool isSel;
    public RoleSkillCraftsVO vo;
    public RoleSkillMagicVO magicVo;

    private CompositeDisposable _disposable;
    private Subject<Unit> _itemClick = new Subject<Unit>();
    public UniRx.IObservable<Unit> GetItemClickHandler { get { return _itemClick; } }  

    private Subject<Unit> _onPress;
    public UniRx.IObservable<Unit> GetPressHandler { get { return _onPress;} }  
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        _disposable = new CompositeDisposable();
        _onPress = new Subject<Unit>();
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        EventDelegate.Set(_view.RoleSkillCraftsItem_UIEventTrigger.onPress, () =>
        {
            JSTimer.Instance.SetupCoolDown("RoleSkillCraftsItemController", 1f, null, () => { _onPress.OnNext(new Unit()); });
        });
        EventDelegate.Set(_view.RoleSkillCraftsItem_UIEventTrigger.onRelease,
            () => { JSTimer.Instance.CancelCd("RoleSkillCraftsItemController"); });
        _disposable.Add(OnRoleSkillCraftsItem_UIButtonClick.Subscribe(_ => { _itemClick.OnNext(new Unit());}));
        //EventDelegate.Set(_view.RoleSkillCraftsItem_UIEventTrigger.onClick, () => { _itemClick.OnNext(new Unit()); });
    }

    protected override void OnDispose()
    {
        UIHelper.DisposeUITexture(View.spIcon_UITexture);
        base.OnDispose();
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    public void UpdateView(RoleSkillCraftsVO data, RoleSkillMainSCraftsState sCraftsState = RoleSkillMainSCraftsState.Normal)
    {
        vo = data;
        UpdateViewDetail(data);
    }
    public void UpdateViewDetail(RoleSkillCraftsVO data)
    {
        if (data == null) return;
        View.lblName_UILabel.text = data.Name;
        SetBGWidth(View.lblName_UILabel.width);
        if (data.IsOpen)
        {
            View.lblGrade_UILabel.text = data.Grade.ToString();
            View.lblGrade_UILabel.gameObject.SetActive(true);
            View.lblNum_UILabel.text = "";
            View.spIcon_UITexture.isGrey = false;
            //IsShowBig(sCraftsState);
        }
        else
        {
            View.lblGrade_UILabel.gameObject.SetActive(false);
            View.lblNum_UILabel.text = string.Format("{0}级\n解锁", data.LimitOpenLevel);
            View.spIcon_UITexture.isGrey = true;
            //SetSpSelect(false);
        }
        UIHelper.SetUITexture(View.spIcon_UITexture, data.cfgVO.icon,false);
        //UIHelper.SetSkillIcon(View.spIcon_UITexture, data.cfgVO.icon);
    }

    void SetBGWidth(int width)
    {
        View.spBG_UISprite.width = width + 12;
    }

    public void UpdateSCraftsView(RoleSkillCraftsVO data)
    {
        UIHelper.SetUITexture(View.spIcon_UITexture, data.cfgVO.icon,false);
        //UIHelper.SetSkillIcon(View.spIcon_UITexture, data.cfgVO.icon);
    }
    public void UpdateScraftsViewState(IRoleSkillMainData data)
    {
        View.lblName_UILabel.text = data.SCraftsState == RoleSkillMainSCraftsState.Normal ? "快捷S技" : "选择快捷S技";
        SetBGWidth(View.lblName_UILabel.width);
    }
    public void UpdateOnSctrlClick(IRoleSkillMainData data, IRoleSkillCraftsViewController viewCtrl)
    {
        if (!vo.IsOpen || !vo.cfgVO.superCrafts)
        {
            View.spIcon_UITexture.isGrey = true;
        }
        SetSel(false);
    }
    
    public void UpdateSel(IRoleSkillMainData data)
    {
        if (vo != null)
        {
            if(data.CurSelCtrl.vo.id == vo.id)
            {
                SetSel(true);
            }
        }
    }

    public void SetSel(bool _isSel,string spName = "choice_01")
    {
        isSel = _isSel;
        if (View != null)
            View.spSelected_UISprite.gameObject.SetActive(_isSel);
    }

    #region 魔法

    private bool IsOpen = false;
    public void UpdateView(bool isOpen, int openLv, RoleSkillMagicVO data = null)
    {
        magicVo = data;
        IsOpen = !isOpen;
        View.spAdd_UISprite.gameObject.SetActive(data == null && isOpen);
        if (data != null)
        {
            UIHelper.SetUITexture(View.spIcon_UITexture, data.cfgVO.icon, false);
            View.spIcon_UITexture.gameObject.SetActive(true);
        }
        else
        {
            View.spIcon_UITexture.gameObject.SetActive(false);
        }

        View.lblNum_UILabel.text = isOpen ? "" : string.Format("{0}级\n解锁", openLv);
        View.lblName_UILabel.text =  data == null ? "" : data.cfgVO.name;
        int width = View.lblName_UILabel.width;
        SetBGWidth(width);
    }

    public void SetBgSprite(string sprite, int height, int width)
    {
        _view.BG_UISprite.spriteName = sprite;
        _view.BG_UISprite.height = height;
        _view.BG_UISprite.width = width;
    }

    public bool IsLock
    {
        get { return IsOpen; }
    }
    public bool IsAdd
    {
        get { return View.spAdd_UISprite.gameObject.activeSelf; }
    }

    #endregion

    #region 战斗中的技能

    public void SetBattleSkill(Skill skill)
    {
        UIHelper.SetUITexture(View.spIcon_UITexture, skill.icon, false);
        _view.lblName_UILabel.text = skill.name;
    }
    #endregion
}
