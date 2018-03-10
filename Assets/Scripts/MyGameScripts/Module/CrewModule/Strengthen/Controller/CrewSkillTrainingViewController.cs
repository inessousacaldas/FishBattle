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

    private List<CrewSkillItemController> _skillItemList = new List<CrewSkillItemController>();

    private int _id;

    #region 自动生成
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        InitSkillItem();
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
        UpdateBtn(list);
    }

    #region 实例化战技
    private void InitSkillItem()
    {
        if (_skillItemList.Count > 0) return;
        for (int i = 0; i < _view.SkillGroup_Transform.childCount; i++)
        {
            var lb = _view.SkillGroup_Transform.GetChild(i).gameObject;
            var com = AddController<CrewSkillItemController, CrewSkillItem>(lb);
            _skillItemList.Add(com);
        }
    }

    #endregion
    //更新数据
    private void UpdateItem(ICrewSkillTrainData trainData,CrewSkillTrainingVO list, int id)
    {
        UpdateConsume(trainData);
        UpdateSkillItem(list);

    }

    private void UpdateBtn(CrewSkillTrainingVO list)
    {
        _view.btnSave_UIButton.gameObject.SetActive(list.aftCraDto.Count != 0);
        _view.traingBtnLabel_UILabel.text = list.aftCraDto.Count == 0 ? "研修" : "继续研修";
        _view.BtnGrid_UIGrid.Reposition();
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
        int count = BackpackDataMgr.DataMgr.GetItemCountByItemID(consumeID);
        string cc = count >= training.consume ? "[ffffff]" + count + "[-]" : "[ff0000]" + count + "[-]";
        View.lblNum_UILabel.text = cc + "/" + training.consume;
        UIHelper.SetItemIcon(View.spIcon_UISprite, item.icon);
        View.spIcon_UISprite.isGrey = count < training.consume;

    }

    private void UpdateSkillItem(CrewSkillTrainingVO list)
    {
        View.BeforeLbl_UILabel.text = "成长率+" + ((float)list.befGrow / (float)1000).ToString();
        View.AfterLbl_UILabel.text = list.aftCraDto.Count == 0 ? "" : "成长率+" + ((float)list.aftGrow / (float)1000).ToString();
        _skillItemList.ForEachI((item, idx) =>
        {
            if (list.aftCraDto.Count == 0)
                item.UpdateTrainingView(list.befCraDto[idx]);
            else
                item.UpdateTrainingView(list.befCraDto[idx], list.aftCraDto[idx]);

        });
    }
}
