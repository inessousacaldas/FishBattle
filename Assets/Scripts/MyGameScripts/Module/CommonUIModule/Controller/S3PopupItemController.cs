// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  WareHousePopupItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using UnityEngine;

public class S3PopUpItemData
{
    public string bgSprite;
    public Vector2 rect;
    public int fontSize;
}
public class PopUpItemInfo
{
    public string Name;
    public int EnumValue;
    public PopUpItemInfo(string Name,int EnumValue)
    {
        this.Name = Name;
        this.EnumValue = EnumValue;
    }
}
public partial class S3PopupItemController
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
    public void UpdateView(S3PopUpItemData onChoiceData,S3PopUpItemData onUnChoiceData,string name,bool isChoce)
    {
        
        if(onChoiceData != null)
        {
            View.Bg_UISprite.spriteName = isChoce ? onChoiceData.bgSprite : (onUnChoiceData == null ? onChoiceData.bgSprite : onUnChoiceData.bgSprite);
            View.Bg_UISprite.width = isChoce ? (int)onChoiceData.rect.x : (onUnChoiceData == null ? (int)onChoiceData.rect.x : (int)onUnChoiceData.rect.x);
            View.Bg_UISprite.height = isChoce ? (int)onChoiceData.rect.y : (onUnChoiceData == null ? (int)onChoiceData.rect.y : (int)onUnChoiceData.rect.y);
            View.Bg_UISprite.MakePixelPerfect();
            View.Label_UILabel.fontSize = isChoce ? onChoiceData.fontSize : (onUnChoiceData == null ? onChoiceData.fontSize : onUnChoiceData.fontSize);
        }

        View.Label_UILabel.text = name;
        View.Bg_UIButton.normalSprite = View.Bg_UISprite.spriteName;
    }

    public void SetName(string name)
    {
        View.Label_UILabel.text = name;
    }
}
