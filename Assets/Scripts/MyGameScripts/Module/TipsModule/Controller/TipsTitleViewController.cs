// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  TipsTitleViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using AppDto;
using System.Collections.Generic;
using UnityEngine;

public partial class TipsTitleViewController
{
    public enum TitleSign
    {
        gerrn,//绿色
        yellow,
        red,
    }
    private const int StarsCount = 5;
    private List<UISprite> starsList = new List<UISprite>();

    public static readonly Dictionary<int, string> ItemTypeToName = new Dictionary<int, string>
    {
        {101, "任务道具"},{102,"货币"},{103,"契约碎片"},{104,"积分"},{105,"经验"},{106,"活力"},{400,"材料道具"},{401,"原石"},
        {402,"纹章"},{403,"印记"},{404,"战技书"},{405,"携带料理"},{406,"大盘料理"},{407,"藏宝图"},{408,"技巧书"},{409,"喇叭"},{410,"礼包"},
        {411,"伙伴礼物"},{412,"切磋邀请函"},{413,"队形书"},{500,"装备"},{ 600,"结晶回路"}
    };

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        starsList.Add(View.Star_1_UISprite);
        starsList.Add(View.Star_2_UISprite);
        starsList.Add(View.Star_3_UISprite);
        starsList.Add(View.Star_4_UISprite);
        starsList.Add(View.Star_5_UISprite);
        HideTitleSign();
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

    public void UpdateView(string icon,int quality, string name, int type, int lv,bool isEquip,bool isBind)
    {
        View.IconBg_UISprite.gameObject.SetActive(true);
        View.Icon_UITexture.gameObject.SetActive(false);
        View.QuartzIcon_UISprite.gameObject.SetActive(false);
        if (type == (int)AppItem.ItemTypeEnum.MissionItem)
        {
            string[] icons = icon.Split(':');
            switch (Int32.Parse(icons[0]))
            {
                case 1:
                    UIHelper.SetItemIcon(View.Icon_UISprite, icons[1]);
                    break;
                case 2:
                    UIHelper.SetPetIcon(View.Icon_UISprite, icons[1]);
                    break;
                case 3:
                    UIHelper.SetSkillIcon(View.Icon_UISprite, icons[1]);
                    break;
                case 4:
                    UIHelper.SetOtherIcon(View.Icon_UISprite, icons[1]);
                    break;
            }
        }
        else if(type == (int)AppItem.ItemTypeEnum.Quartz)
        {
            UIHelper.SetQuartyIcon(View.Icon_UISprite, icon);
        }
        else if(type == (int)AppItem.ItemTypeEnum.VirtualItem)
            UIHelper.SetAppVirtualItemIcon(View.Icon_UISprite, (AppVirtualItem.VirtualItemEnum)StringHelper.ToInt(icon));
        else if(type >= 0)
            UIHelper.SetItemIcon(View.Icon_UISprite, icon);

        quality = quality < 1 ? 1 : quality;
        UIHelper.SetItemQualityIcon(View.IconBg_UISprite, quality);
        //背景 todo
        //UIHelper.SetItemIcon(View.Bg_UISprite, dto.icon);

        View.Name_UILabel.text = name.WrapColor(ItemHelper.GetItemNameColorByRank(quality));
        UIHelper.SetTextQualityColor(View.Name_UILabel, quality);
        View.Lv_UILabel.text = string.Format("{0}级", lv);
        View.Lv_UILabel.gameObject.SetActive(lv>0);
        if (lv < 0) //显示星星
        {
            View.Lv_UILabel.gameObject.SetActive(false);
            View.Type_UILabel.gameObject.SetActive(false);
            View.Stars.gameObject.SetActive(true);

            starsList.ForEachI((item, index) =>
            {
                if(index < -lv)
                {
                    item.gameObject.SetActive(true);
                }
            });
        }

        if (ItemTypeToName.ContainsKey(type))
            View.Type_UILabel.text = ItemTypeToName[type];
        else
            //View.Type_UILabel.text = ItemTypeToName[400];
            View.Type_UILabel.gameObject.SetActive(false);

        View.Lock_UISprite.gameObject.SetActive(isBind);
    }

    public void SetQuartzIcon(string icon)
    {
        View.QuartzIcon_UISprite.gameObject.SetActive(true);
        UIHelper.SetQuartyIcon(View.QuartzIcon_UISprite, icon);
    }

    public void SetMagicIcon(string icon)
    {
        UIHelper.SetSkillIcon(View.Icon_UISprite, icon); 
    }

    public void SetSkillIcon(string icon)
    {
        View.IconBg_UISprite.gameObject.SetActive(false);
        View.Icon_UITexture.gameObject.SetActive(true);
        UIHelper.SetUITexture(View.Icon_UITexture, icon, false);
    }

    public void SetTitleSign(string name, TitleSign color)
    {
        View.Sign_UISprite.spriteName = color.ToString();
        UIHelper.SetCommonIcon(View.Sign_UISprite, string.Format("infor_{0}", color));
        View.SignName_UILabel.text = name;
        View.Sign_UISprite.gameObject.SetActive(true);
    }
    public void HideTitleSign()
    {
        View.Sign_UISprite.gameObject.SetActive(false);
    }
    public int GetHeight()
    {
        return this.gameObject.GetComponent<UIWidget>().height;
    }
}
