// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  CrewFetterItemIconController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using AppDto;
using System;
using System.Collections.Generic;

public partial class CrewFetterItemIconController
{
    private UISprite[] childSprite = new UISprite []{}; 
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        childSprite = this.transform.GetComponentsInChildren<UISprite>();
        
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
    public void InitAngle(int angle)
    {
        _view.transform.Rotate(UnityEngine.Vector3.forward, -angle);
    }
    public void UpdateView(Crew crew,bool isOwner,bool CheckMark)
    {
        UIHelper.SetPetIcon(View.Icon_UISprite, crew.icon);

        View.CheckMark_UISprite.gameObject.SetActive(CheckMark);
        childSprite.ForEach(x => x.isGrey = !isOwner);
        View.CheckMark_UISprite.isGrey = !CheckMark;
    }

    protected override void OnShow()
    {
        base.OnShow();
       
    }

    protected override void OnHide()
    {
        base.OnHide();
        _view.transform.rotation = UnityEngine.Quaternion.identity;
    }
}
