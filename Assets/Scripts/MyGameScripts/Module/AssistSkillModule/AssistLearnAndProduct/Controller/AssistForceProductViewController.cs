// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  AssistForceProductViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using System.Collections.Generic;
using AppDto;
using UniRx;

public partial class AssistForceProductViewController
{
    private const int getCount = 10;
    private const int costCount = 4;
    private Dictionary<int, string> _idToName = new Dictionary<int, string>
    {
        {0,"可随机获得" },
        {1,"生产消耗" }
    };
    private static Dictionary<int, AssistSkillMakeConsume> clsConsumeData = DataCache.getDicByCls<AssistSkillMakeConsume>();
    private static Dictionary<int, AssistSkillGradeConsume> clsGradeData = DataCache.getDicByCls<AssistSkillGradeConsume>();
    private static Dictionary<int, AssistSkill> clsSkillData = DataCache.getDicByCls<AssistSkill>();
    private List<AssistProductItemController> _getList = new List<AssistProductItemController>();
    private List<AssistProductItemController> _costList = new List<AssistProductItemController>();

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        for(int i = 0 ; i< getCount; i++)
        {
            var ctrl = AddChild<AssistProductItemController, AssistProductItem>(View.Grid_UIGrid.gameObject, AssistProductItem.NAME);
            _getList.Add(ctrl);
        }

        for (int i = 0; i < costCount; i++)
        {
            var ctrl = AddChild<AssistProductItemController, AssistProductItem>(View.GridCost_UIGrid.gameObject, AssistProductItem.NAME);
            _costList.Add(ctrl);
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
        int skillLevel = data.SkillLevel;
        int index = 0;

        #region 获得产品list
        Dictionary<int, int> getItemIdToLv = new Dictionary<int, int>();
        clsConsumeData.ForEach(item =>
        {
            if (item.Value.id == data.CurRecipeId)
            {
                item.Value.gradeMaitchItem.ForEach(x =>
                {
                    x.itemId.ForEach(y =>
                    {
                        getItemIdToLv.Add(y, x.level);
                    });
                });
            }
        });

        bool isGrey = false;
        getItemIdToLv.ForEachI((item, i) =>
        {
            _getList[i].UpdateView(item.Key, string.Format("{0}级", item.Value));

            if (!isGrey && skillLevel < item.Value)
                isGrey = true;

            if (isGrey)
                _getList[index].IsLock();

            index++;
        });

        for (int i = index; i < getCount; i++)
        {
            _getList[i].gameObject.SetActive(false);
        }

        View.Grid_UIGrid.Reposition();
        #endregion

        #region 生产消耗list
        index = 0;
        List<int> materialIdList = new List<int>();
        List<int> countList = new List<int>();
        clsConsumeData.ForEach(item =>
        {
            if (item.Value.id == data.CurRecipeId)
            {
                item.Value.materials.ForEach(x =>
                {
                    materialIdList.Add(x.itemId);
                });
            }
        });

        if (clsGradeData.Find(x => x.Value.id == data.SkillLevel).Value != null)
            clsGradeData.Find(x => x.Value.id == data.SkillLevel).Value.orbmentTechnolgyMakeConsume.ForEach(count =>
            {
                countList.Add(count);
            });

        if (materialIdList.Count != countList.Count)
            return;

        materialIdList.ForEachI((id,i) =>
        {
            var haveCount = BackpackDataMgr.DataMgr.GetItemCountByItemID(id);
            var haveStr = haveCount < countList[i] ? haveCount.ToString().WrapColor(ColorConstantV3.Color_Red) : haveCount.ToString();
            _costList[i].UpdateView(id, string.Format("{0}/{1}", haveStr, countList[i]));
            _costList[i].SetScale();
            index++;
        });

        if (index < costCount)
        {
            var itemData = clsGradeData[data.SkillLevel].makeVirtualConsumeItem;
            var haveCount = ModelManager.Player.GetPlayerWealthById(itemData[0].itemId);
            var haveStr = haveCount < itemData[0].count ? haveCount.ToString().WrapColor(ColorConstantV3.Color_Red) : haveCount.ToString();
            _costList[index].UpdateView(itemData[0].itemId, string.Format("{0}/{1}", haveStr, itemData[0].count));
            _costList[index].SetScale();
            _costList[index].SetVirtualIcon(itemData[0].itemId);
            index++;
        }

        for (int i = index; i < costCount; i++)
        {
            _costList[i].gameObject.SetActive(false);
        }
        View.GridCost_UIGrid.Reposition();
        #endregion
    }
}

