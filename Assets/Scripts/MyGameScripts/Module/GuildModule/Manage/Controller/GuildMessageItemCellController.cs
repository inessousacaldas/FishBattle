// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  GuildMessageItemCellController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;

public partial class GuildMessageItemCellController
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

    //[c][7EE830]0.05%[-][/c]
    public void SetData(messageInfoDto message)
    {
        View.MessageLabel_UILabel.text = string.Format("{0}  {1}", message.time, message.msg);
    }

}
