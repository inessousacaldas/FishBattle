// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  TipsEquipmentEffectItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using AppDto;
public partial class TipsEquipmentEffectItemController
{
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        View.Des_UILabel.gameObject.SetActive(false);
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

    public void UpdateView(int effectId)
    {
        var effectConfig = DataCache.getDtoByCls<AppDto.EquipmentEffects>(effectId);
        View.ProLabel_UILabel.text = string.Format("特效:{0}",effectConfig.name);
    }

    public void SetDesActive()
    {
        View.Des_UILabel.gameObject.SetActive(!View.Des_UILabel.gameObject.activeSelf);
    }
}
