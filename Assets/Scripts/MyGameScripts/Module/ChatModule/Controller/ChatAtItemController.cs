// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  ChatAtItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using AppDto;

public partial class ChatAtItemController
{
    public TeamMemberDto teamMemDto;
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
        base.OnDispose();
        teamMemDto = null;
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    public void UpdateView(TeamMemberDto dto)
    {
        teamMemDto = dto;
        View.Name_UILabel.text = dto.nickname;
    }

}
