// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  EmailAttatchItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using AppDto;

public partial class EmailAttatchItemController
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

    //用特定接口类型数据刷新界面
    
    //更新邮件附件显示
    public void UpdateView(MailAttachmentDto attachmentDto)
    {
        var gItem = ItemHelper.GetGeneralItemByItemId(attachmentDto.itemId);
        UIHelper.SetItemIcon(View.Icon_UISprite, gItem.icon);
        if(gItem as AppItem != null)
            UIHelper.SetItemQualityIcon(View.IconBg_UISprite, (gItem as AppItem).quality);
    }
}
