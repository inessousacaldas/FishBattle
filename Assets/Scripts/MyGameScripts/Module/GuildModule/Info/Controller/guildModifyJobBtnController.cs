// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  guildModifyJobBtnController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using AppDto;
public partial interface IguildModifyJobBtnController
{
}

public partial class guildModifyJobBtnController
{
    public GuildPosition posDto;
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
        posDto = null;
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    public void UpdateView(GuildPosition dto)
    {
        if (dto == null) return;
        posDto = dto;
        View.Label_UILabel.text = dto.name;
    }

    public void UpdateBtnState(bool show)
    {
        View.Background_UISprite.spriteName = show ? "btn_Select" : "btn_Normal";
    }
    
}
