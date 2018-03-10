// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  AssistCookUpgradeViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using System.Collections.Generic;
using AppDto;

public partial class AssistCookUpgradeViewController
{
    private const int recipeCount = 4;
    private const int materialCount = 6;
    private static Dictionary<int, AssistSkillMakeConsume> clsConsumeData = DataCache.getDicByCls<AssistSkillMakeConsume>();
    private static Dictionary<int, AssistSkillGradeConsume> clsGradeData = DataCache.getDicByCls<AssistSkillGradeConsume>();
    private List<AssistProductItemController> _itemList = new List<AssistProductItemController>();
    private List<MaterailItemViewController> _materialList = new List<MaterailItemViewController>();
    private Dictionary<int, string> AssistSkillIdToName = new Dictionary<int, string>
        {
            {1, "携带料理" },
            {2, "大盘料理"},
            {3, "导力技术" }
        };

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView()
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
        View.Name_UILabel.text = AssistSkillMainDataMgr.AssistSkillMainViewLogic.AssistSkillIdToName[data.SkillId];
        View.LevelNum_UILabel.text = data.SkillLevel.ToString();
        int skillLevel = data.SkillLevel;

        #region 配方
        var index = 0;
        clsConsumeData.ForEach(item =>
        {
            if (item.Value.skillId == data.SkillId)
            {
                _itemList[index].UpdateView(item.Value.id, item.Value.name, true, item.Value.icon); //配方
                index++;
            }
        });

        for(var i =index; i< recipeCount; i++)
        {
            _itemList[i].gameObject.SetActive(false);
        }
        #endregion

        #region 材料
        index = 0;
        List<int> virtualList = new List<int>();
        List<ItemDto> studyItem = clsGradeData.Find(x => x.Value.id == data.SkillLevel).Value.studyConsumeItem;
        //消耗
        studyItem.ForEach(item =>
        {
            _materialList[index].UpdateView(item.itemId, item.count);
            _materialList[index].gameObject.SetActive(true);
            virtualList.Add(item.itemId);
            index++;
        });

        //最多显示6个
        if (index*2 > materialCount)
            return;

        //拥有
        var temp = index;
        for (var j = temp; j< temp * 2; j++)
        {
            _materialList[j].UpdateView(virtualList[j- temp], ModelManager.Player.GetPlayerWealthById(virtualList[j- temp]),true);
            _materialList[j].gameObject.SetActive(true);
            index++;
        }

        for (var i = index; i < materialCount; i++)
        {
            _materialList[i].gameObject.SetActive(false);
        }
        #endregion

        View.Grid_UIGrid.Reposition();
        View.Tabel_UITable.Reposition();
    }

}
