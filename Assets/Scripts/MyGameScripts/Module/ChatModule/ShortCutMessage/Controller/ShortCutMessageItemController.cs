// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  ShortCutMessageItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;

public partial class ShortCutMessageItemController
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

    public void UpdateView(ShortCutMessage data,bool isEditMode)
    {
        View.Input_UIInput.value = data.content;
        View.MsgLabel_UILabel.text = data.content;

        View.DeleteBtn_UIButton.gameObject.SetActive(false);
        View.AddBtn_UIButton.gameObject.SetActive(false);
        if (data.shortCutType == ChatShortCutType.Empty)
        {   
            if (isEditMode)
            {
                View.Input_UIInput.gameObject.SetActive(true);
                View.MsgLabel_UILabel.gameObject.SetActive(false);
                View.AddBtn_UIButton.gameObject.SetActive(false);
            }
            else
            {
                View.AddBtn_UIButton.gameObject.SetActive(true);
                View.Input_UIInput.gameObject.SetActive(false);
                View.MsgLabel_UILabel.gameObject.SetActive(true);
            }
        }
        else if(data.shortCutType == ChatShortCutType.Custom)
        {
            if (isEditMode)
            {
                View.Input_UIInput.gameObject.SetActive(true);
                View.MsgLabel_UILabel.gameObject.SetActive(false);
                View.DeleteBtn_UIButton.gameObject.SetActive(true);
            }
            else
            {
                View.Input_UIInput.gameObject.SetActive(false);
                View.MsgLabel_UILabel.gameObject.SetActive(true);
                View.DeleteBtn_UIButton.gameObject.SetActive(false);
            }
        }
        else if(data.shortCutType == ChatShortCutType.System)
        {
            View.Input_UIInput.gameObject.SetActive(false);
            View.MsgLabel_UILabel.gameObject.SetActive(true);
        }
    }
}
