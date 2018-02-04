// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  GuildRequesterController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************
using AppDto;

public partial interface IGuildRequesterController
{
    void UpdateView(GuildApprovalDto dto);
    GuildApprovalDto GuildApprovalDto { get; }
}

public partial class GuildRequesterController
{
    private GuildApprovalDto guildApprovalDto;
    public GuildApprovalDto GuildApprovalDto { get { return guildApprovalDto; } }
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
        guildApprovalDto = null;
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    public void UpdateView(GuildApprovalDto dto)
    {
        if (dto == null) return;
        guildApprovalDto = dto;
        View.rNameLabel_UILabel.text = dto.applyerName;
        View.rLvLabel_UILabel.text = dto.applyerGrade + "";
        View.rGenderLabel_UILabel.text = dto.applyerGender == 0 ? "女" : "男";
        View.rRefererLabel_UILabel.text = dto.inviterName;
        View.rOperateBtn.SetActive(dto.applyStat == (int)GuildApprovalDto.GuildApplyStat.Idler);
        View.rOperateLabel.SetActive(dto.applyStat == (int)GuildApprovalDto.GuildApplyStat.Competitor);
    }
}
