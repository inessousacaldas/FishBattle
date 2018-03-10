// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  ChatSystemItem2Controller.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using AppDto;
using ChatChannelEnum = AppDto.ChatChannel.ChatChannelEnum;
using LableTypeEnum = AppDto.ChatChannel.LableTypeEnum;
using UnityEngine;
using System.Text.RegularExpressions;

public partial class ChatSystemItem2Controller
{
    private ChatNotify _chatNotify;
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
 
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        View.ContentLbl_UIEventListener.onClick += OnContentClick;
    }

    protected override void OnDispose()
    {
        _chatNotify = null;
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        View.ContentLbl_UIEventListener.onClick -= OnContentClick;
    }

    private void OnContentClick(GameObject go)
    {
        string msg = View.ContentLbl_UILabel.GetUrlAtPosition(UICamera.lastWorldPosition);
        if (!string.IsNullOrEmpty(msg))
        {
            ChatHelper.DecodeUrlMsg(msg);
        }
    }

    //用特定接口类型数据刷新界面
    public void UpdateView(ChatNotify chatNotify)
    {
        if (chatNotify == null)
        {
            Hide();
            return;
        }
        _chatNotify = chatNotify;
        Show();

        View.ChannelBg_UISprite.spriteName = GetNameStr(
            (ChatChannelEnum)chatNotify.channelId
            , (LableTypeEnum)chatNotify.lableType);
        //_view.ChannelNameLbl_UILabel.text = GetNameStr(
        //    (ChatChannelEnum)chatNotify.channelId
        //    , (LableTypeEnum)chatNotify.lableType);
        _view.ContentLbl_UILabel.text = GetContentStr(
            (ChatChannelEnum)chatNotify.channelId
            , (LableTypeEnum)chatNotify.lableType
            , chatNotify.content);
        if (View.ContentLbl_UILabel.printedSize.x > 336)
            View.ContentLbl_UILabel.overflowMethod = UILabel.Overflow.ResizeHeight;

    }

    private string GetContentStr(ChatChannelEnum channelId, LableTypeEnum lableType, string content)
    {
        return ChatHelper.ChangeStrColor( content).WrapColor(ColorConstantV3.Color_SealBrown_Str);
    }

    private string GetNameStr(ChatChannelEnum channelId, LableTypeEnum lableType)
    {
        var name = string.Empty;
        //var colorSymbol = ColorConstantV3.Color_White_Str;
        if (channelId == ChatChannelEnum.System)
        {
            name = ChatHelper.GetLabelName(lableType);
        }
        else
        {
            name = ChatHelper.GetTitleName(channelId);
        }

        //if (!string.IsNullOrEmpty(name))
        //{
        //    var color = ChatHelper.GetChannelNameColor(channelId, lableType);
        //    name = name.WrapColor(color);
        //}
        return name;
    }
    
    
}
