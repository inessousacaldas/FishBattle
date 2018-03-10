// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  AssistForceUpgradeViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using System.Collections.Generic;
using AppDto;

public partial class AssistForceUpgradeViewController
{
    private const int recipeCount = 10;
    private const int materialCount = 6;
    private static Dictionary<int, AssistSkillMakeConsume> clsConsumeData = DataCache.getDicByCls<AssistSkillMakeConsume>();
    private static Dictionary<int, AssistSkillGradeConsume> clsGradeData = DataCache.getDicByCls<AssistSkillGradeConsume>();
    private List<AssistProductItemController> _itemList = new List<AssistProductItemController>();
    private List<MaterailItemViewController> _materialList = new List<MaterailItemViewController>();

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        for (int i = 0; i < recipeCount; i++)
        {
            var ctrl = AddChild<AssistProductItemController, AssistProductItem>(View.Grid_UIGrid.gameObject, AssistProductItem.NAME);
            _itemList.Add(ctrl);
        }

        for (int i = 0; i < materialCount; i++)
        {
            var ctrl = AddChild<MaterailItemViewController, MaterailItemView>(View.Tabel_UITable.gameObject, MaterailItemView.NAME);
            _materialList.Add(ctrl);
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
        View.LevelNum_UILabel.text = data.SkillLevel.ToString();
        int skillLevel = data.SkillLevel;

        #region 产品
        int index = 0;
        Dictionary<int, int> getList = new Dictionary<int, int>();
        //获取产品id list
        clsConsumeData.ForEach(item =>
        {
            if (item.Value.skillId == data.SkillId)
            {
                item.Value.gradeMaitchItem.ForEach(x =>
                {
                    int level = x.level;
                    x.itemId.ForEach(y =>
                    {
                        if(getList.Count < recipeCount)
                            getList.Add(y, level);
                    });
                });
            }
        });

        getList.ForEachI((item, i) =>
        {
            _itemList[i].UpdateView(item.Key);
            index++;
        });

        for (int i = index; i < recipeCount; i++)
        {
            _itemList[i].gameObject.SetActive(false);
        }
        #endregion

        #region 材料
        index = 0;
        List<int> virtualList = new List<int>();
        //消耗
        if (clsGradeData.Find(x => x.Value.id == data.SkillLevel).Value != null)
        {
            clsGradeData.Find(x => x.Value.id == data.SkillLevel).Value.studyConsumeItem.ForEach(item =>
            {
                //界面显示4个
                if(index < materialCount)
                {
                    _materialList[index].UpdateView(item.itemId, item.count);
                    _materialList[index].gameObject.SetActive(true);
                    virtualList.Add(item.itemId);
                    index++;
                }
            });
        }

        //拥有
        if(index*2 <= materialCount)
        {
            int temp = index;
            for (int j = temp; j < temp * 2; j++)
            {
                _materialList[j].UpdateView(virtualList[j - temp], ModelManager.Player.GetPlayerWealthById(virtualList[j - temp]),true);
                _materialList[j].gameObject.SetActive(true);
                index++;
            }
        }

        for (int i = index; i < materialCount; i++)
        {
            _materialList[i].gameObject.SetActive(false);
        }
        #endregion

        View.Grid_UIGrid.Reposition();
        View.Tabel_UITable.Reposition();
    }

}
