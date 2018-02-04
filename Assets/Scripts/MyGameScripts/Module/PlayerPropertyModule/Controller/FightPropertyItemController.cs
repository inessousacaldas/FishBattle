// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  FightPropertyItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using UnityEngine;


public partial class FightPropertyItemController
{
    #region 工具生成
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

    //用特定接口类型数据刷新界面
    public void UpdateView()
    {

    }
    #endregion


    public void Init(string fightPropertyName, int fightPropertyNum)
    {
        View.Label_UILabel.text = fightPropertyName;
        View.Num_UILabel.text = fightPropertyNum.ToString();
    }
}
