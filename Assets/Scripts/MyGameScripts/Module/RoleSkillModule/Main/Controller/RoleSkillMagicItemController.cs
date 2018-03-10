// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  RoleSkillCraftsItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using Assets.Scripts.MyGameScripts.Module.RoleSkillModule;
using System;
using UniRx;

public partial class RoleSkillMagicItemController
{
    public RoleSkillMagicVO vo;
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
        UIHelper.DisposeUITexture(View.spIcon_UISprite);
        base.OnDispose();
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent()
    {

    }

    public void UpdateView(bool isOpen, int openLv, RoleSkillMagicVO data = null)
    {
        vo = data;
        View.spLock_UISprite.gameObject.SetActive(!isOpen);
        View.spAdd_UISprite.gameObject.SetActive(data == null && isOpen);
        if (data != null)
        {
            UIHelper.SetUITexture(View.spIcon_UISprite, data.cfgVO.icon, false);
            View.spIcon_UISprite.gameObject.SetActive(true);
        }
        else
        {
            View.spIcon_UISprite.gameObject.SetActive(false);
        }
        View.lblName_UILabel.text = isOpen ?
                                            data == null ? "" : data.cfgVO.name
                                            : openLv + "级解锁";
        int width = View.lblName_UILabel.width;
        View.lblBG_UISprite.width = width + 12;
    }

    public void UpdateView(RoleSkillMagicVO data = null)
    {
        vo = data;
        View.spIcon_UISprite.gameObject.SetActive(false);
        View.spLock_UISprite.gameObject.SetActive(false);
        if(vo.IsOpen)
        {
            if(vo.isEquip)
            {
                View.lblName_UILabel.text = string.Format("{0}\n等级：{1}  {2}",vo.Name,vo.cfgVO.grade,RoleSkillUtils.GetElementPropertyTypeName(vo.ElementId));
                View.spIcon_UISprite.gameObject.SetActive(true);
                UIHelper.SetUITexture(View.spIcon_UISprite, data.cfgVO.icon,false);
            }
            else
            {
                View.lblName_UILabel.text = "装备魔法技能";
                View.spLock_UISprite.gameObject.SetActive(true);
                View.spLock_UISprite.spriteName = "btn_crewadd";
            }
        }
        else
        {
            View.lblName_UILabel.text = "";
            View.spLock_UISprite.gameObject.SetActive(true);
            View.spLock_UISprite.spriteName = "Ect_Lock";
        }
        int width = View.lblName_UILabel.width;
        View.lblBG_UISprite.width = width + 12;
        View.spLock_UISprite.MakePixelPerfect();
    }

    public void SetSel(bool _isSel)
    {
        if(View!=null)
            View.spSelected_UISprite.gameObject.SetActive(_isSel);
    }

    private void SetSpSelect(bool _isSel)
    {
        //View.spSelected_UISprite.spriteName = spName;
    }

    public bool IsLock
    {
        get { return View.spLock_UISprite.gameObject.activeSelf; }
    }
    public bool IsAdd
    {
        get { return View.spAdd_UISprite.gameObject.activeSelf; }
    }
}
