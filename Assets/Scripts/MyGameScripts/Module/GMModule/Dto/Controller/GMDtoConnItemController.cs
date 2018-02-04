// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  GMDtoConnItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;

public partial class GMDtoConnItemController
{
    public GMDtoConnVO vo;
    public bool isSel;
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

    public void UpdateView(GMDtoConnVO _vo)
    {
        vo = _vo;
        View.lblName_UILabel.text = vo.Name;
        View.lblTime_UILabel.text = vo.time;
        View.lblSize_UILabel.text = vo.size.ToString();
    }

    public void SetSel(bool _isSel,string spName = "choice_01")
    {
        isSel = _isSel;
        SetSpSelect(isSel,spName);
    }

    private void SetSpSelect(bool _isSel,string spName = "choice_01")
    {
        View.spSelected_UISprite.gameObject.SetActive(_isSel);
        View.spSelected_UISprite.spriteName = spName;
    }

}
