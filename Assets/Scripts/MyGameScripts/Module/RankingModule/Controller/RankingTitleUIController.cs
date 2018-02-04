// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  RankingTitleUIController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using System.Collections.Generic;
using AppDto;
using UnityEngine;

public partial class RankingTitleUIController
{

    private List<UILabel> labels = new List<UILabel>();

    private readonly float[] COLUMN_POS_5 = { -260f, -148f, 16f, 120, 237 };
    private readonly float[] COLUMN_POS_4 = { -260f, -116, 61, 235, 0 };


    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        labels.Add(_view.FirstLabel_UILabel);
        labels.Add(_view.SecondLabel_UILabel);
        labels.Add(_view.ThirdLabel_UILabel);
        labels.Add(_view.FurthLabel_UILabel);
        labels.Add(_view.FifthLabel_UILabel);
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


    public void SetDate(Rankings rankings)
    {
        SetLablePosition(rankings.rankColShow);

        var titlelist = rankings.titleStr.Split(',');
        labels.ForEachI((t, i) =>
        {
            t.gameObject.SetActive(i < titlelist.Length);
            t.text = titlelist.TryGetValue(i);
        });
    }
    private void SetLablePosition(int column)
    {
        var pos = COLUMN_POS_4;
        if (column == 5)
        {
            pos = COLUMN_POS_5;
        }
        else if (column == 4)
        {
            pos = COLUMN_POS_4;
        }

        Vector3 vec;
        for (int i = 0; i < labels.Count; i++)
        {
            if (i >= column)
            {
                labels[i].gameObject.SetActive(false);
                continue;
            }
            vec = labels[i].transform.localPosition;
            vec.x = pos[i];
            labels[i].transform.localPosition = vec;
            labels[i].gameObject.SetActive(true);
        }
    }

}
