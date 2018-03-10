// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  EquipmentResetController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using AppDto;
using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
public partial interface IEquipmentResetController
{
    IEquipmentChoiceContentController EquipmentChoiceCtrl { get; }
}
public partial class EquipmentResetController
{
    //Dictionary<GameObject, EquipmentSmithCellController> cellItem_dic = new Dictionary<GameObject, EquipmentSmithCellController>();
    EquipmentChoiceContentController _equipmentChoiceCtrl;
    public IEquipmentChoiceContentController EquipmentChoiceCtrl { get { return _equipmentChoiceCtrl; } }
    //装备的头像
    EquipmentItemCellController itemCellCtrl;

    //原属和新属
    EquipmentResetAttrContentCtrl oldAttrContentCtrl, newAttrContentCtrl;
    SmithItemCellController smithItemCtrl;
    /// <summary>
    /// 列表最大的数量
    /// </summary>
    //int listMax = 7;
    IEquipmentResetViewData _data;


    CompositeDisposable _disposble;

    
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        _disposble = new CompositeDisposable();
        itemCellCtrl = AddController<EquipmentItemCellController, EquipmentItemCell>(View.EquipmentItemCell);
        oldAttrContentCtrl = AddController<EquipmentResetAttrContentCtrl, EquipmentAttrContent>(View.EquipmentAttrContent_Old);
        newAttrContentCtrl = AddController<EquipmentResetAttrContentCtrl, EquipmentAttrContent>(View.EquipmentAttrContent_New);

        _equipmentChoiceCtrl =  AddChild<EquipmentChoiceContentController, EquipmentChoiceContent>(View.EquipmentChoiceContent, EquipmentChoiceContent.NAME);
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        //View.EquipmentCellContent_UIRecycledList.onUpdateItem = OnUpdateCellItem;
    }

    protected override void OnDispose()
    {

    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        //View.EquipmentCellContent_UIRecycledList.onUpdateItem = null;
    }

   
    public void UpdateDataView(IEquipmentResetViewData data)
    {
        _data = data;

        View.TimesLb_UILabel.text = string.Format("今日剩余洗练次数:{0}", data.CurrentEquipmentInfo.curResetCount);
        UpdateChoiceList(data);
        UpdateInfo(data);
    }
    protected override void OnShow()
    {
        base.OnShow();
    }

    /// <summary>
    /// 更新左侧选项的信息
    /// </summary>
    /// <param name="data"></param>
    private void UpdateChoiceList(IEquipmentResetViewData data)
    {
        _disposble.Clear();
        _equipmentChoiceCtrl.UpdateViewData(data.CurTab, data.EquipmentItems, data.CurChoiceEquipment);
    }


    /// <summary>
    /// 更新中间的洗练信息
    /// </summary>
    /// <param name="data"></param>
    private void UpdateInfo(IEquipmentResetViewData data)
    {
        if (smithItemCtrl == null)
            smithItemCtrl = AddChild<SmithItemCellController, SmithItemCell>(View.ItemCells, SmithItemCell.NAME);
        UIHelper.SetAppVirtualItemIcon(View.MoneyIcon_UISprite, AppVirtualItem.VirtualItemEnum.SILVER);
        if (data.CurChoiceEquipment!=null)
        {
            var equipment = data.CurChoiceEquipment.equip as Equipment;
            itemCellCtrl.UpdateViewData(equipment, data.CurChoiceEquipment.property.quality);
            View.Eq_name_UILabel.text = equipment.name;
            var ownerMoney = ModelManager.IPlayer.GetPlayerWealth(AppVirtualItem.VirtualItemEnum.SILVER);
            var needMoney = (data.CurChoiceEquipment.equip as Equipment).resetSilver;
            string color = "";
            if (ownerMoney > needMoney)
                color = "FFFFFF";
            else
                color = "FF0000";
            string SilverIcon = ItemIconConst.GetIconConstByItemId(AppVirtualItem.VirtualItemEnum.SILVER);
            View.MoneyLbl_UILabel.text = string.Format("{0}", needMoney).WrapColor(color);
            
            smithItemCtrl.Show();
            smithItemCtrl.SetGrey(false, "");
            smithItemCtrl.UpdateViewData(data.CurSmithItemVo);
            _equipmentChoiceCtrl.View.EmptyPanel.SetActive(false);
            View.ResetBtn_UIButton.sprite.isGrey = false;
            View.ResetBtn_UIButton.enabled = true;

            //如果新洗练获取的数据不为空，设置按钮
            if (data.CurChoiceEquipment.property.resetProperty != null)
            {
                View.ResetSaveBtn_UIButton.gameObject.SetActive(true);
                View.ResetContinue_UIButton.gameObject.SetActive(true);
                View.ResetBtn_UIButton.gameObject.SetActive(false);
            }
            else
            {
                View.ResetSaveBtn_UIButton.gameObject.SetActive(false);
                View.ResetContinue_UIButton.gameObject.SetActive(false);
                View.ResetBtn_UIButton.gameObject.SetActive(true);
            }
        }
        else
        {
            if (smithItemCtrl != null)
            {
                smithItemCtrl.SetEmpty();
            }
            string SilverIcon = ItemIconConst.GetIconConstByItemId(AppVirtualItem.VirtualItemEnum.SILVER);
            View.MoneyLbl_UILabel.text = string.Format("{0}", 0).WrapColor("FFFFFF");


            View.ResetSaveBtn_UIButton.gameObject.SetActive(false);
            View.ResetContinue_UIButton.gameObject.SetActive(false);
            View.ResetBtn_UIButton.gameObject.SetActive(true);
            View.ResetBtn_UIButton.enabled = false;
            View.ResetBtn_UIButton.sprite.isGrey = true;
            _equipmentChoiceCtrl.View.EmptyPanel.SetActive(true);
            _equipmentChoiceCtrl.View.EmptyLabel_UILabel.text = "没有40级以上的装备可以洗练哦！";
        }

        oldAttrContentCtrl.UpdateViewData(data.OldAttrContent);
        newAttrContentCtrl.UpdateViewData(data.NewAttrContent);
    }
}

#region 暂时废弃 ReclyList
//private void InitEquipemntCell()
//{
//    for(int i=0;i<listMax;i++)
//    {
//        var ctrl = AddChild<EquipmentSmithCellController, EquipmentSmithCell>(View.EquipmentCellContent, EquipmentSmithCell.NAME);
//        cellItem_dic.Add(ctrl.View.gameObject, ctrl);
//        ctrl.OnEquipmentSmithCell_UIButtonClick.Subscribe(_ => {
//            _onEquipmentChoiceStream.OnNext(ctrl.index);
//        });
//    }
//}

//private void OnUpdateCellItem(GameObject go,int index,int realIndex)
//{
//    if (_data.EquipmentItems.Count <= realIndex)
//    {
//        GameDebuger.Log("OnUpdateCellItem  " + realIndex);
//        return;
//    }

//    EquipmentSmithCellController ctrl;
//    if (cellItem_dic.TryGetValue(go, out ctrl))
//    {
//        var equpiment = _data.EquipmentItems[realIndex].equip as Equipment;
//        var equipmentDto = _data.EquipmentItems[realIndex];
//        ctrl.UpdateViewData(realIndex, equpiment, _data.CurChoiceEquipment == equipmentDto,  EquipmentSmithCellController.ShowType.Two);
//    }
//}
#endregion