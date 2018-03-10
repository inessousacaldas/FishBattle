// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  PropsMagicItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using System.Text;
using AppDto;
using MyGameScripts.Gameplay.Player;

public partial class PropsMagicItemController
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

    public void UpdateView(int magicId, string typeStr, int factionId)
    {
        UIHelper.SetCommonIcon(View.Icon_UISprite, GlobalAttr.GetMagicIcon(magicId));
        if(factionId <= 0)
        {
            View.Line_UISprite.gameObject.SetActive(false);
            View.proDesLabel_UILabel.gameObject.SetActive(false);

            UIHelper.SetCommonIcon(View.FactionIcon_UISprite, typeStr);
        }
        else
        {
            View.Line_UISprite.gameObject.SetActive(true);
            View.proDesLabel_UILabel.gameObject.SetActive(true);

            var factionData = DataCache.getDtoByCls<Faction>(factionId);
            UIHelper.SetFactionIcon(View.FactionIcon_UISprite, factionId);
            View.proDesLabel_UILabel.text = factionData.description.WrapColor(ColorConstantV3.COlor_TextCommonColor_Str);
        }

        GetElementDesc(magicId);
    }

    public void UpdateView(int magicId, string faction)
    {
        UIHelper.SetQuartyIcon(View.Icon_UISprite, GlobalAttr.GetMagicIcon(magicId), false);
        UIHelper.SetCommonIcon(View.FactionIcon_UISprite, faction);
        GetElementDesc(magicId);
    }

    public void HideProDescLb(bool b)
    {
        View.Line_UISprite.gameObject.SetActive(b);
        View.proDesLabel_UILabel.gameObject.SetActive(b);
    }

    private void GetElementDesc(int magicId)
    {
        var element = DataCache.getDtoByCls<ElementRelative>(magicId);
        if (element == null)
            View.DesLabel_UILabel.text = element == null ? "" : element.enhanceDesc;
        else
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(element.enhanceDesc);
            sb.Append(element.reductionDesc);
            View.DesLabel_UILabel.text = sb.ToString();
        }
    }
}
