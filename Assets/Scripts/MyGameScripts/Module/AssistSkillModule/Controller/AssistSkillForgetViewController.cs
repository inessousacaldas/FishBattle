// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  AssistSkillForgetViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;

public partial class AssistSkillForgetViewController
{
    private Action _enterCallback;
    private Action _cancelCallback;

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
 
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        EventDelegate.Add(_view.CancelButton_UIButton.onClick, OnCancelClick);
        EventDelegate.Add(_view.OKButton_UIButton.onClick, OnEnterClick);
    }

    protected override void OnDispose()
    {

    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    public void UpdateView(string str, int constId, int cost, bool isFirst, Action enterCallback, Action cancelCallback)
    { 
        str = str.WrapColor(ColorConstantV3.Color_Green);
        View.ContentLb_UILabel.text = string.Format("确定遗忘已学习的技能{0}?", str);

        View.CostLabel_UILabel.text = cost.ToString();

        _enterCallback = enterCallback;
        _cancelCallback = cancelCallback;

        View.Forget_Transform.gameObject.SetActive(!isFirst);
        View.FirstForget_UILabel.gameObject.SetActive(isFirst);
    }

    private void OnCancelClick()
    {
        ProxyBaseWinModule.Close();
    }

    private void OnEnterClick()
    {
        if (_enterCallback != null)
            _enterCallback();
        OnCancelClick();
    }
}
