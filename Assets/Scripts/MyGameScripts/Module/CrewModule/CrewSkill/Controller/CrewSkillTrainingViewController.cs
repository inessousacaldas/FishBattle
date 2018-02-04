// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  CrewSkillCraftsTrainingViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using UniRx;
using UnityEngine;
using System.Collections.Generic;
using AppDto;
public interface ICrewSkillCraftsTrainingViewController
{
    IObservable<Unit> OnbtnSave_UIButtonClick { get; }
    IObservable<Unit> OnbtnTraining_UIButtonClick { get; }
    IObservable<Unit> OnbtnTips_UIButtonClick { get; }
}

public partial class CrewSkillTrainingViewController
{
    private List<CrewSkillItemCellController> leftItemList = new List<CrewSkillItemCellController>();
    private List<CrewSkillItemCellController> rightItemList = new List<CrewSkillItemCellController>();
    #region 自动生成
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        CreateItem();
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {

    }

    protected override void OnDispose()
    {
        base.OnDispose();
        if (leftItemList.Count > 0) leftItemList.Clear();
        if (rightItemList.Count > 0) rightItemList.Clear();
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }
    #endregion

    public void UpdateView(ICrewSkillTrainData trainData, int id)
    {
        var list = trainData.GetTrainList(id);
        if (list == null) return;
        UpdateItem(trainData,list, id);
    }

    #region 实例化战技
    private void CreateItem()
    {
        CreateLeftItem();
        CreateRightItem();
    }

    private void CreateLeftItem()
    {
        if (leftItemList.Count > 0) return;
        for(int i = 0; i < 4; i++)
        {
            var itemCtrl = AddChild<CrewSkillItemCellController, CrewSkillItemCell>(
                View.LeftTable_UITable.gameObject,
                CrewSkillItemCell.NAME
                );
            leftItemList.Add(itemCtrl);
            View.LeftTable_UITable.Reposition();
        }
    }

    private void CreateRightItem()
    {
        if (rightItemList.Count > 0) return;
        for (int i = 0; i < 4; i++)
        {
            var itemCtrl = AddChild<CrewSkillItemCellController, CrewSkillItemCell>(
                View.RightTable_UITable.gameObject,
                CrewSkillItemCell.NAME
                );
            rightItemList.Add(itemCtrl);
            View.RightTable_UITable.Reposition();
        }
    }

    #endregion
    //更新数据
    private void UpdateItem(ICrewSkillTrainData trainData,CrewSkillTrainingVO list, int id)
    {
        UpdateConsume(trainData);
        UpdateLeftItem(list);
        UpdateRightItem(list);
    }

    //消耗物品
    private void UpdateConsume(ICrewSkillTrainData trainData)
    {
        var consumeID = DataCache.GetStaticConfigValue(AppStaticConfigs.CREW_TRAINNING_CONSUME_ITEM);
        int quality = CrewViewDataMgr.DataMgr.GetCurCrewQuality;
        if (quality == -1)
        {
            GameDebuger.LogError(string.Format("找不到{0}对应的quality,请检查!!(这一行不算报错)", CrewViewDataMgr.DataMgr.GetCurCrewID));
            return;
        }

        var crew = DataCache.getDtoByCls<GeneralCharactor>(CrewViewDataMgr.DataMgr.GetCurCrewID) as Crew;
        if (crew == null)
        {
            GameDebuger.LogError(string.Format("Crew表找不到{0},请检查", CrewViewDataMgr.DataMgr.GetCurCrewID));
            return;
        }

        var training = trainData.GetCrewTraining(crew.rare);
        var item = ItemHelper.GetGeneralItemByItemId(consumeID);
        View.lblName_UILabel.text = ItemHelper.GetItemName(consumeID);
        int count = BackpackDataMgr.DataMgr.GetItemCountByItemID(consumeID);
        string cc = count >= training.consume ? "[ffffff]" + count + "[-]" : "[ff0000]" + count + "[-]";
        View.lblNum_UILabel.text = cc + "/" + training.consume;
        UIHelper.SetItemIcon(View.spIcon_UISprite, item.icon);
        _view.bg_UISprite.spriteName = string.Format("item_ib_{0}", quality);
        View.spIcon_UISprite.isGrey = count < training.consume;
        View.AddSp.SetActive(count < training.consume);
    }

    private void UpdateLeftItem(CrewSkillTrainingVO list)
    {
        List<CrewSkillCraftsVO> tmpList  = list.befCraDto;
        View.leftLbl_UILabel.text = ((float)list.befGrow / (float)1000).ToString();
        leftItemList.ForEachI((item, idx) =>
        {
            if (idx < tmpList.Count)
                item.UpdateView(tmpList[idx]);
            else
                item.UpdateNone();
        });
    }

    //研修后的战技
    private void UpdateRightItem(CrewSkillTrainingVO list)
    {
        List<CrewSkillCraftsVO> tmpList = list.aftCraDto;
        View.rightLbl_UILabel.text = tmpList.Count == 0 ? "": ((float)list.aftGrow / (float)1000).ToString(); 

        rightItemList.ForEachI((item, idx) =>
        {
            if(idx < tmpList.Count)
                item.UpdateView(tmpList.TryGetValue(idx));
            else
                item.UpdateNone();
        });
    }
}
