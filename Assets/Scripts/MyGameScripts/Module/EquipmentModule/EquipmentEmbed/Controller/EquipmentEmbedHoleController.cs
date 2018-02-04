// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  EquipmentEmbedItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using AppDto;
public partial class EquipmentEmbedHoleController
{

    public EquipmentEmbedHoleVo data;
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
    public void UpdateViewData(EquipmentEmbedHoleVo vo,bool isSelect)
    {
        this.data = vo;
        View.LockIcon_UISprite.gameObject.SetActive(!vo.isOpen);

        View.Select_Icon_UISprite.gameObject.SetActive(isSelect);
        //如果没有宝石在该游戏
        if (vo.embedid == -1)
        {
           //并且没有开放，则显示锁不显示加
            if(!vo.isOpen)
            {
                View.EmbedLbl_UILabel.text = string.Format("{0}级解锁", vo.holeInfo.openGrade);
                View.AddIcon_UISprite.gameObject.SetActive(false);
                View.EmbedIcon_UISprite.gameObject.SetActive(false);
                View.EmbedLbl_UILabel.gameObject.SetActive(true);
                View.Select_Icon_UISprite.gameObject.SetActive(false);
            }
            else
            {
                View.AddIcon_UISprite.gameObject.SetActive(true);
                View.EmbedLbl_UILabel.text = "";
                View.EmbedLbl_UILabel.gameObject.SetActive(false);
                View.EmbedIcon_UISprite.gameObject.SetActive(false);       
            }
            
        }
        else
        {
            View.AddIcon_UISprite.gameObject.SetActive(false);
            View.EmbedIcon_UISprite.gameObject.SetActive(true);
            View.EmbedLbl_UILabel.gameObject.SetActive(true);
            var embedInfo = DataCache.getDtoByCls<GeneralItem>(vo.embedid) as Props;
            UIHelper.SetItemIcon(View.EmbedIcon_UISprite, embedInfo.icon);
            View.EmbedLbl_UILabel.text = string.Format("{0}", embedInfo.name);
        }
    }

    public void UpdateView(EngraveDto data,bool isSelect = false)
    {
        View.AddIcon_UISprite.gameObject.SetActive(false);
        View.LockIcon_UISprite.gameObject.SetActive(false);
        View.Select_Icon_UISprite.gameObject.SetActive(false);

        View.EmbedIcon_UISprite.gameObject.SetActive(true);

        var itemData = ItemHelper.GetGeneralItemByItemId(data.itemId);
        UIHelper.SetItemIcon(View.EmbedIcon_UISprite, itemData.icon);

        var localData = ItemHelper.GetGeneralItemByItemId(data.itemId);
        var propsParam = (localData as Props).propsParam as PropsParam_3;
        string titleStr = localData.name.WrapColor(ColorConstantV3.Color_Purple) + "\n";
        string detailStr = "";
        if (propsParam.type == (int)PropsParam_3.EngraveType.ORDINARY)
            detailStr = (DataCache.getDtoByCls<CharacterAbility>(propsParam.cpId).name + "+" + data.effect.ToString()).WrapColor(ColorConstantV3.Color_Green);
        else
            detailStr = ("当前纹章属性*" + data.effect.ToString()).WrapColor(ColorConstantV3.Color_Green);

        var _propsStr = titleStr + detailStr;
    }

    public void UpdateEmpty()
    {
        View.LockIcon_UISprite.gameObject.SetActive(false);
        View.Select_Icon_UISprite.gameObject.SetActive(false);
        View.EmbedIcon_UISprite.gameObject.SetActive(false);
        View.EmbedLbl_UILabel.text = "";
        View.EmbedLbl_UILabel.gameObject.SetActive(false);

        View.AddIcon_UISprite.gameObject.SetActive(false);
    }
}
