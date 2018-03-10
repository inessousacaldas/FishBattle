// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  BaseProgressBarController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public partial class BaseProgressBarController
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

    public void UpdateView(int cur, int max, string progressStr = "")
    {
        var progress = 0f;
        if (max <= 0)
            progress = 0f;
        else if (cur >= max)
            progress = 1f;
        else
            progress = cur * 1.0f / max;

        _view.BaseProgressBar_CProgressBar.value = progress;
        _view.Label_UILabel.gameObject.SetActive( !string.IsNullOrEmpty(progressStr));
        _view.Label_UILabel.text = progressStr;
            
    }

}
