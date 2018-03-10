// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  AssistLearnForceViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using AppDto;
using System.Collections.Generic;
using UniRx;

public partial class AssistLearnForceViewController
{
    private List<AssistProductItemController> _itemList = new List<AssistProductItemController>();
    private List<int> _idList = new List<int>();
    private static Dictionary<int, AssistSkillMakeConsume> clsData = DataCache.getDicByCls<AssistSkillMakeConsume>();

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

    public void UpdateView(IAssistSkillMainData data)
    {
        _idList.Clear();

        clsData.ForEach(item =>
        {
            if (item.Value.skillId == 3)
            {
                item.Value.gradeMaitchItem.ForEach(maitchItem =>
                {
                    maitchItem.itemId.ForEachI((id, i) =>
                    {
                        _idList.Add(id);
                    });
                });
            }
        });

        #region item数量计算
        int dif = _idList.Count - _itemList.Count;
        if (dif > 0)
        {
            for (int i = 0; i < dif; i++)
            {
                var ctrl = AddChild<AssistProductItemController, AssistProductItem>(View.Grid_UIGrid.gameObject, AssistProductItem.NAME);
                _itemList.Add(ctrl);
            }
        }
        else if (dif < 0)
        {
            for (int i = 0; i < Math.Abs(dif); i++)
            {
                _itemList[_itemList.Count - i - 1].gameObject.SetActive(false);
            }
        }
        #endregion

        _idList.ForEachI((id,i) =>
        {
            _itemList[i].UpdateView(id);
        });

        View.Grid_UIGrid.Reposition();
    }

}
