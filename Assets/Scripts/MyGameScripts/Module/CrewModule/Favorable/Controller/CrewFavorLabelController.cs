// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  CrewFavorLabelController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

public partial class CrewFavorLabelController
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

    public void SetLabelInfo(int lv, string content, string desc, string color = "F0F0F0")
    {
        _view.Grade_UILabel.text = string.Format("Lv.{0}", lv).WrapColor(color);
        _view.Content_UILabel.text = content.WrapColor(color);
        _view.Desc_UILabel.text = desc.WrapColor(color);
        _view.Sprite_UISprite.UpdateAnchors();
    }

}
