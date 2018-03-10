// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  GuildMemItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************
using AppDto;
using System.Text.RegularExpressions;

public partial interface IGuildMemItemController
{
    void UpdateView(GuildMemberDto dto, long selMemId);
    void OnSel(bool sel);
    GuildMemberDto MemberDto { get; }
    long Idx { get; }
}

public partial class GuildMemItemController
{
    private const string conColor = "FBF892";
    private const string onLineColor = "8CF05A";
    private const string offLineColor = "E35656";
    private const string selColor = "494C50";
    private const string whiteColor = "FFFFFF";
    private GuildMemberDto memberDto;
    public GuildMemberDto MemberDto { get { return memberDto; } }
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
        memberDto = null;
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

    public void UpdateView(GuildMemberDto dto, long selMemId)
    {
        memberDto = dto;
        OnSel(selMemId == dto.id); 
        View.NameLabel_UILabel.text = dto.name;
        View.lvLabel_UILabel.text = dto.grade + "级";
        var val = GuildMainDataMgr.DataMgr.GuildPosition.Find(e => e.id == dto.position);
        if (val != null)
            View.jobLabel_UILabel.text = val.name;
    }

    public void OnSel(bool sel)
    {
        WrapTxt(View.contributionLabel_UILabel, sel, false);
        WrapTxt(View.onlineLabel_UILabel, sel, true);
        string color = sel ? selColor : whiteColor;
        View.selGo.SetActive(sel);
        View.NameLabel_UILabel.color = ColorExt.HexStrToColor(color);
        View.lvLabel_UILabel.color = ColorExt.HexStrToColor(color);
        View.jobLabel_UILabel.color = ColorExt.HexStrToColor(color);
        View.contributionLabel_UILabel.color = ColorExt.HexStrToColor(color);
        View.onlineLabel_UILabel.color = ColorExt.HexStrToColor(color);
    }
    
    private void WrapTxt(UILabel label,bool sel,bool setOnline)
    {
        string txt = label.text;
        if (sel)
        {
            string pattern = @"\[.*?\]";
            var match = Regex.Matches(txt, pattern);
            foreach (Match m in match)
            {
                string newValue = m.Value;
                txt = txt.Replace(newValue, "");
            }
        }
        else
        {
            if(setOnline)
                txt = memberDto.online ? "[" + onLineColor + "]" + "在线" + "[-]" : "[" + offLineColor + "]" + "离线" + "[-]";
            else
                txt = "[" + conColor + "]" + memberDto.circulateCbute + "[-]" + "/" + memberDto.totalCbute;
        }
        label.text = txt;
    }

    public long Idx
    {
        get { return memberDto.id; }
    }
    
}
