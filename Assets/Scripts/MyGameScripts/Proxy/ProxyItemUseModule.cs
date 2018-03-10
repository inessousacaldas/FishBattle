// **********************************************************************
// Copyright (c) 2013 Baoyugame. All rights reserved.
// File     :  ProxyItemUseModule.cs
// Author   : willson
// Created  : 2015/3/14 
// Porpuse  : 
// **********************************************************************
using UnityEngine;
using System.Collections.Generic;
using System;
using AppDto;

public class ProxyItemUseModule
{
    public const string NAME = "ItemUseView";
    public static void OpenBattleItem(int itemUsedCount, GeneralCharactor.CharactorType charactorType, int battleType, Action<BagItemDto> callBackDelegate)
    {
        var controller = UIModuleManager.Instance.OpenFunModule<BattleItemUseController>(ItemUseView.NAME, UILayerType.FourModule, true);

        controller.SetOtherParam(itemUsedCount, callBackDelegate);
        GameDebuger.TODO(@"controller.UpdateView(null, ModelManager.Backpack.GetBattleItemList(charactorType, battleType), false, true);");
        controller.SetData(null, DemoSimulateHelper.SimulateBagItemDtoList()/**ModelManager.Backpack.GetBattleItemList(charactorType, battleType)*/, false, true);
    }

    public static void Close()
    {
        UIModuleManager.Instance.CloseModule (ItemUseView.NAME);
    }

    //    public static void Open(BagItemDto useDto,List<BagItemDto> efectItemList,bool isMultiple,UILayerType depth = UILayerType.FourModule)
    //	{
    //		GameObject ui = UIModuleManager.Instance.OpenFunModule(ItemUseView.NAME, depth, true);
    //		var controller = ui.GetMissingComponent<ItemUseViewController>();
    //		
    //		controller.UpdateView(useDto,efectItemList,isMultiple);
    //	}

    //	public static void OpenIdentifyItem(BagItemDto useDto,List<BagItemDto> efectItemList)
    //	{
    //		GameObject ui = UIModuleManager.Instance.OpenFunModule(ItemUseView.NAME, UILayerType.FourModule, true);
    //		var controller = ui.GetMissingComponent<IdentifyItemUseController>();
    //		
    //		controller.UpdateView(useDto,efectItemList,false,true);
    //		//controller.SetOptBtn("鉴定");

    #region 多物品提交物品相关代码
    // 多物品提交物品
    public static void OpenMissionSubmitItem(Dictionary<int,GeneralItem> itemDic,Action<List<BagItemDto>> callBackDelegate,Dictionary<int,int> tItemsNeetNumber)
    {
        GameObject ui = UIModuleManager.Instance.OpenFunModule(ItemUseView.NAME, UILayerType.FourModule, true);
        MissionSubmitItemController controller = ui.GetMissingComponent<MissionSubmitItemController>();


        controller.SetOtherParam(callBackDelegate);
        controller.SetData(null,GetMissionSubmitItemList(itemDic),tItemsNeetNumber,false,true);
    }

    public static List<BagItemDto> GetMissionSubmitItemList(Dictionary<int,GeneralItem> itemDic)
    {
        List<BagItemDto> tMissionSubmitItemList = new List<BagItemDto>();
        IEnumerable<BagItemDto> _bagDataList =  BackpackDataMgr.DataMgr.GetBagItems();
        _bagDataList.ForEachI((dto,idx) =>
        {
            if(dto.index >= 0)
            {
                if(itemDic.ContainsKey(dto.itemId))
                {
                    tMissionSubmitItemList.Add(dto);
                }
            }
        });
        return tMissionSubmitItemList;
    }
    #endregion

    //	门派提交宠物
    //	public static void OpenMissionSubmitPet(int petID, Action<GeneralCharactor> callBackDelegate) {
    //		GameObject ui = UIModuleManager.Instance.OpenFunModule(ItemUseView.NAME, UILayerType.FourModule, true);
    //		MissionSubmitPetController controller = ui.GetMissingComponent<MissionSubmitPetController>();
    //
    //		
    //		controller.SetOtherParam(callBackDelegate);
    //		controller.SetOtherData(null, ModelManager.Pet.GetNaturePetAndNotBattledList(petID), false, true);
    //	}

    // 海上贸易
    //    public static void OpenBusinessSubmitItem(int itemId, int count, Action<List<BagItemDto> > callBackDelegate)
    //    {
    //        GameObject ui = UIModuleManager.Instance.OpenFunModule(ItemUseView.NAME, UILayerType.FourModule, true);
    //        BusinessSubmitItemController controller = ui.GetMissingComponent<BusinessSubmitItemController>();
    //
    //        
    //        controller.SetOtherParam(callBackDelegate);
    //        controller.UpdateView(count, ModelManager.Backpack.GetItemsByItemId(itemId));
    //
    //        //if (controller.GetItemCellList().Count > 0)
    //        //{
    //        //    controller.OnItemClick(controller.GetItemCellList()[0]);
    //        //}
    //    }

    //  }


    //    public const string SelectItemUse_NAME = "SelectItemUseView";

    //    public static void OpenSelectItemUse(BagItemDto dto)
    //    {
    //        Props props = dto.item as Props;
    //        if(props != null && props.propsParam is PropsParam_38)
    //        {
    //            GameObject ui = UIModuleManager.Instance.OpenFunModule(SelectItemUse_NAME, UILayerType.FourModule, false);
    //            SelectItemUseViewController controller = ui.GetMissingComponent<SelectItemUseViewController>();
    //            
    //            controller.UpdateView(dto, props.propsParam as PropsParam_38);
    //        }
    //        else
    //        {
    //            // TipManager.AddTip("");
    //        }
    //    }

    //    public static void CloseSelectItemUse()
    //    {
    //        ProxyItemTipsModule.Close();
    //        UIModuleManager.Instance.CloseModule(SelectItemUse_NAME);
    //    }
}
