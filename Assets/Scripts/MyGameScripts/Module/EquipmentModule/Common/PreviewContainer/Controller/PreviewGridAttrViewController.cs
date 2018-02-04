// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  PreviewGridAttrViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using System.Collections.Generic;
public partial class PreviewGridAttrViewController
{
    List<UILabel> labelPools = new List<UILabel>();
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        labelPools.AddRange(View.GridContent_UIGrid.GetComponentsInChildren<UILabel>());
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {

    }

    protected override void OnDispose()
    {
        labelPools.Clear();
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }
    public void InitViewData(List<string> attrList)
    {
        labelPools.ForEach(x => x.gameObject.SetActive(false));
        attrList.ForEachI((x, i) => {
            labelPools[i].text = x;
            labelPools[i].gameObject.SetActive(true);
        });
    }
}
