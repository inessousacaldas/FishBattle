// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  EmbebGroupFristShowAttrController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using AppDto;

public partial class EmbebGroupFristShowAttrController
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
    public void UpdateData(int id,int curLast,int valueChange)
    {
        var cab = DataCache.getDtoByCls<CharacterAbility>(id);
        View.AttrLabel_UILabel.text = string.Format("{0}+{1}", cab.name, curLast);

        if(valueChange >= 0)
        {
            View.AttrChangeLabel_UILabel.gameObject.SetActive(true);
            View.AttrChangeLabel_UILabel.text = "+"+valueChange;
        }
        else
        {
            View.AttrChangeLabel_UILabel.gameObject.SetActive(false);
        }
    }
}
