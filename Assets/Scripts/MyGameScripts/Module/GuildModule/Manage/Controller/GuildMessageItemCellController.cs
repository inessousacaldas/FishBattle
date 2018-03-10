// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  GuildMessageItemCellController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using AppDto;
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
    public void SetData(GuildEventDto message)
    {
        var time = GetTimer(message.time);
        View.MessageLabel_UILabel.text = string.Format("{0}  {1}", time, message.content);
    }

    private string GetTimer(long ms)
    {
        var time = DateUtil.UnixTimeStampToDateTime(ms);
        int year = time.Year;
        int m = time.Month;
        string month = m < 10 ? "0" + m : m.ToString();
        int d = time.Day;
        string day = d < 10 ? "0" + d : d.ToString();
        int h = time.Hour;
        string hour = h < 10 ? "0" + h : h.ToString();
        int mn = time.Minute;
        string minute = mn < 10 ? "0" + mn : mn.ToString();
        return string.Format("{0}.{1}.{2} {3}:{4}", year, month, day, hour, minute);
    }

}
