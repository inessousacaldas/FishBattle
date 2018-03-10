// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  RoleSkillSpecialityAddViewController.cs
// Author   : DM-PC092
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using AppDto;
using System;
using UniRx;

public partial interface IRoleSkillSpecialityAddViewController
{

}
public partial class RoleSkillSpecialityAddViewController
{
    ItemCellController ctrl;

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView()
    {
        //ctrl = AddChild<ItemCellController, ItemCell>(View.ItemAnchor, ItemCell.Prefab_BagItemCell);
        ctrl = AddController<ItemCellController, ItemCell>(View.ItemUse);
    }

    // 客户端自定义代码
    protected override void RegistCustomEvent()
    {

    }

    protected override void RemoveCustomEvent()
    {
    }

    protected override void OnDispose()
    {
        base.OnDispose();
        RoleSkillDataMgr.RoleSkillSpecialityAddViewLogic.DisposeModule();
    }

    //在打开界面之前，初始化数据
    protected override void InitData()
    {

    }

    // 业务逻辑数据刷新
    protected override void UpdateDataAndView(IRoleSkillData data)
    {
        View.lblLevel_UILabel.text = "专精等级：" + data.SpecData.GetCurGrade();
        View.lblexp_UILabel.text = "专精经验：" + data.SpecData.GetCurExp() + "/" + data.SpecData.GetExpGradeVO(Math.Min(data.SpecData.GetCurGrade() + 1, data.SpecData.MaxGrade)).specialityExp;
        float value = (float)(data.SpecData.GetCurExp()) / data.SpecData.GetExpGradeVO(Math.Min(data.SpecData.GetCurGrade() + 1, data.SpecData.MaxGrade)).specialityExp;
        View.Bar.value = value;
        View.lbllimit_UILabel.text = "等级上限：" +data.SpecData.GetLevelLimit();
        var costMedal =  DataCache.GetStaticConfigValue(AppStaticConfigs.SPECIALITY_ADDEXP_CONSUME_MEDAL);
        var costSilver =  DataCache.GetStaticConfigValue(AppStaticConfigs.SPECIALITY_ADDEXP_CONSUME_SILVER);
        View.lblMedal_1_UILabel.text =  costMedal.ToString();
        View.lblSilver_1_UILabel.text = costSilver.ToString();
        View.lblMedal_10_UILabel.text = (costMedal*10).ToString();
        View.lblSilver_10_UILabel.text =  (costSilver*10).ToString();
        View.lblHas_UILabel.text = string.Format("{0}",data.SpecData.GetHasAddExpTime());
        View.btnTrain1_UIButton.isEnabled = data.SpecData.GetHasAddExpTime() >= 1;
        View.btnTrain10_UIButton.isEnabled = data.SpecData.GetHasAddExpTime() >= 10;
        UpdateItem(data);
    }

    public void UpdateItem(IRoleSkillData data)
    {
        var item = ItemHelper.GetGeneralItemByItemId(data.SpecData.GetTrainItemID());
        int count = BackpackDataMgr.DataMgr.GetItemCountByItemID(data.SpecData.GetTrainItemID());
        if (count == 0)
        {
            UIHelper.SetCommonIcon(View.IconSprite_UISprite, "Ect_Add3");
            View.lblItemName_UILabel.text = "";
        }
        else
        {
            UIHelper.SetItemIcon(View.IconSprite_UISprite, item.icon);
            View.lblItemName_UILabel.text = count.ToString();
        }

        /*string.Format("{0}×{1}", ItemHelper.GetItemName(data.SpecData.GetTrainItemID()), BackpackDataMgr.DataMgr.GetItemCountByItemID(data.SpecData.GetTrainItemID()));*/
        //View.IconSprite_UISprite.spriteName = item.icon;
        //ctrl.UpdateView_ItemUse(item);
        //ctrl.SetCountTxt(0, "");

        var itemT = ItemHelper.GetGeneralItemByItemId(data.SpecData.GetTrainItemID());
        ctrl.UpdateSkillSpecialView(itemT, count);
    }
}

