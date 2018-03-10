// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  GuildItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************
using AppDto;
using System.Text.RegularExpressions;

public partial interface IGuildItemController
{
}

public partial class GuildItemController
{
    private const string selColor = "494C50";
    private const string whiteColor = "FFFFFF";
    private GuildBaseInfoDto itemInfoDto;
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
    public void UpdateBg(int idx)
    {
        int res = idx % 2;
        View.Background_UISprite.alpha = res == 0 ? 1f : 0.01f;
    }

    public void UpdateView(GuildBaseInfoDto dto, long selId)
    {
        itemInfoDto = dto;
        OnSel(selId == dto.showId);
        View.idLabel_UILabel.text = dto.showId + "";
        View.NameLabel_UILabel.text = dto.name;
        View.lvLabel_UILabel.text = dto.grade + "级";
        View.memCountLabel_UILabel.text = dto.memberCount + "/" + dto.maxMemberCount;
        View.bossNameLabel_UILabel.text = dto.bossName;
    }

    public void OnSel(bool sel)
    {
        string color = sel ? selColor : whiteColor;
        View.selGo.SetActive(sel);
        View.idLabel_UILabel.color = ColorExt.HexStrToColor(color);
        View.NameLabel_UILabel.color = ColorExt.HexStrToColor(color);
        View.lvLabel_UILabel.color = ColorExt.HexStrToColor(color);
        View.memCountLabel_UILabel.color = ColorExt.HexStrToColor(color);
        View.bossNameLabel_UILabel.color = ColorExt.HexStrToColor(color);
    }
    
    public GuildBaseInfoDto ItemInfoDto
    {
        get { return itemInfoDto; }
    }

}
