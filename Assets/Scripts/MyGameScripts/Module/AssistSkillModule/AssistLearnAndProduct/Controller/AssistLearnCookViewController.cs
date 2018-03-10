// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  AssistLearnCookViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using System.Collections.Generic;
using AppDto;

public partial class AssistLearnCookViewController
{
    private List<RecipeItemViewController> _recipeitemList = new List<RecipeItemViewController>();
    private const int count = 4;
    private static Dictionary<int, AssistSkillMakeConsume> clsData = DataCache.getDicByCls<AssistSkillMakeConsume>();

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        for(int i=0;i< count;i++)
        {
            _recipeitemList.Add(AddChild<RecipeItemViewController, RecipeItemView>(View.Grid_UIGrid.gameObject, RecipeItemView.NAME));
        }
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

    public void UpdateView(IAssistSkillMainData data)
    {
        if(data.ChosedSkillId == 1)
            View.TitleTip_UILabel.text = "学习携带料理可学会以下：";
        else if(data.ChosedSkillId == 2)
            View.TitleTip_UILabel.text = "学习大盘料理可学会以下：";

        int index = 0;
        clsData.ForEach(item =>
        {
            if (item.Value.skillId == data.ChosedSkillId)
            {
                if (index >= count)
                    return;

                _recipeitemList[index].UpdateView(item.Value);
                index++;
                View.Grid_UIGrid.Reposition();
            }
        });

        for (int i = count; i > index; i--)
        {
            _recipeitemList[i - 1].gameObject.SetActive(false);
        }
    }

}
