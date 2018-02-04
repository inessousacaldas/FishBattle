// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  GuildBuildItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;

public partial class GuildBuildItemController
{
    public BuildDetailMsg buildDetailMsg;
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
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    public void UpdateView(IGuildMainData data,int idx)
    {
        var list = data.BuildList;
        string name = list.TryGetValue(idx);
        View.titleLabel_UILabel.text = name;
        View.DesLabel_UILabel.text = GuildMainHelper.ShortDesList[idx];
        if (idx == 5)
            View.logoSp_UISprite.spriteName = "Building_1";
        else
            View.logoSp_UISprite.spriteName = "Building_" + idx;
        buildDetailMsg = new BuildDetailMsg(idx, name, GuildMainHelper.ShortDesList[idx], GuildMainHelper.LvDesList[idx], GetLv(idx,data).ToString(), idx == 5 ? "Building_1" : "Building_" + idx);
    }

    private int GetLv(int idx,IGuildMainData data)
    {
        int lv = 0;
        switch (idx)
        {
            case 1:
                lv = data.GuildBaseInfo.grade;
                break;
            case 2:
                lv = data.GuildDetailInfo.buildingInfo.barpubGrade;
                break;
            case 3:
                lv = data.GuildDetailInfo.buildingInfo.treasuryGrade;
                break;
            case 4:
                lv = data.GuildDetailInfo.buildingInfo.guardTowerGrade;
                break;
            case 5:
                lv = data.GuildDetailInfo.buildingInfo.workshopGrade;
                break;
        }
        return lv;
    }
}

public struct BuildDetailMsg
{
    private int idx;
    public int Idx { get { return idx; } }
    private string buildName;
    public string BuildName { get { return buildName; } }
    private string shortDes;
    public string ShortDes { get { return shortDes; } }
    private string des;
    public string Des { get { return des; } }
    private string lv;
    public string Lv { get { return lv; } }
    private string icon;
    public string Icon { get { return icon; } }

    public BuildDetailMsg(int idx,string buildName,string shortDes,string des,string lv,string icon)
    {
        this.idx = idx;
        this.buildName = buildName;
        this.shortDes = shortDes;
        this.des = des;
        this.lv = lv;
        this.icon = icon;
    }
    
}
