// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  MedallionGrooveRuneItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using AppDto;
using UniRx;

public partial class MedallionGrooveRuneItemController
{

    private string _propsStr = "";

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
 
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        Icon_UIButtonEvt.Subscribe(_ =>
        {
            TipManager.AddTip(_propsStr);
        });
    }

    protected override void OnDispose()
    {

    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        Icon_UIButtonEvt.CloseOnceNull();
    }

    public void UpdateView(EngraveDto data)
    {
        var itemData = ItemHelper.GetGeneralItemByItemId(data.itemId);
        UIHelper.SetItemIcon(View.Icon_UIButton.sprite, itemData.icon);
        UIHelper.SetItemQualityIcon(View.Quality_UISprite, (itemData as Props).quality);

        var localData = ItemHelper.GetGeneralItemByItemId(data.itemId);
        var propsParam = (localData as Props).propsParam as PropsParam_3;
        string titleStr = string.Format("{0}\n", localData.name.WrapColor(ColorConstantV3.Color_Purple));
        string detailStr = "";
        if (propsParam.type == (int)PropsParam_3.EngraveType.ORDINARY)
            detailStr = string.Format("{0}+{1}", DataCache.getDtoByCls<CharacterAbility>(propsParam.cpId).name, data.effect.ToString().WrapColor(ColorConstantV3.Color_Green));
        else
            detailStr = string.Format("当前纹章属性*{0}", data.effect).WrapColor(ColorConstantV3.Color_Green);

        _propsStr = titleStr + detailStr;
    }
}
