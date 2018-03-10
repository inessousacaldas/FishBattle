// **********************************************************************
// Copyright (c) 2013 Baoyugame. All rights reserved.
// File     :  ProxyItemTipsModule.cs
// Author   : willson
// Created  : 2015/1/13 
// Porpuse  : 
// **********************************************************************
using AppDto;
using UnityEngine;
using System.Collections.Generic;

public class ProxyItemTipsModule
{
	public const string NAME = "ItemTipsView";

//    private static IitemCellController _SelectItem;
//
//    public static IitemCellController SelectItem
//    {
//        set
//        {
//            _SelectItem = value;
//            _SelectItem.isSelect = true;
//        }
//        get
//        {
//            return _SelectItem;
//        }
//    }

    /**
     * 固定位置显示Tips
     */
//    public static void OpenTipsByPos(ItemCellController cell, GameObject pos, bool hasOpt = true, bool needCompare = true, System.Func<BagItemDto, bool> onUseCallback = null, ItemTipSide side = ItemTipSide.Left, bool externalClose = false)
//    {
//        if (cell.GetData() != null)
//        {
////            SelectItem = cell;
//            ItemTipsViewController controller = Open(cell.GetData(), pos, hasOpt, needCompare, onUseCallback, side);
//            controller.SetExternalClose(externalClose);
//        }
//    }
//    public static void OpenTipsByPos(
//        BagItemDto dto
//        , GameObject pos
//        , bool hasOpt = true
//        , bool needCompare = true
//        , System.Func<BagItemDto, bool> onUseCallback = null
//        , ItemTipSide side = ItemTipSide.Left
//        , bool externalClose = false)
//    {
//        if (dto != null)
//        {
////            SelectItem = cell;
//            ItemTipsViewController controller = Open(dto, pos, hasOpt, needCompare, onUseCallback, side);
//            controller.SetExternalClose(externalClose);
//        }
//    }
//
//    /**
//     * 固定位置显示Tips
//     */
//    public static void OpenTipsByPos(
//        BagItemDto dto
//        , GameObject pos
//        , bool hasOpt = true
//        , bool needCompare = true
//        , System.Func<BagItemDto, bool> onUseCallback = null
//        , ItemTipSide side = ItemTipSide.Left)
//    {
//        if (dto != null)
//        {
//            Open(dto, pos, hasOpt, needCompare, onUseCallback, side);
//        }
//    }
//
//    public static void OpenEquipMaterialSimpleInfo(BagItemDto dto, GameObject anchor)
//    {
//        GameObject view = UIModuleManager.Instance.OpenFunModule(NAME, UILayerType.FourModule, false);
//        EquipSimpleInfoTipsViewController controller = view.GetMissingComponent<EquipSimpleInfoTipsViewController>();
//
//        controller.SetData(dto, anchor, false, null, ItemTipSide.Left);
//    }
//
//    public static ItemTipsViewController OpenEquipmentMateiralTips(
//        BagItemDto dto
//        , GameObject anchor
//        , bool hasOpt = false
//        , bool needCompare = true
//        , System.Func<BagItemDto, bool> onUseCallback = null
//        , ItemTipSide side = ItemTipSide.Left)
//    {
//        if (dto == null)
//            return null;
//        ItemTipsViewController controller = OpenView(dto);
//
//        if (needCompare && dto.item.itemType == AppItem.ItemTypeEnum_Equipment)
//        {
//            BagItemDto compareItem = ModelManager.Backpack.GetEquipByPartType((dto.item as Equipment).equipPartType);
//            if (compareItem != null && compareItem != dto)
//            {
//                controller.SetCompareData(dto, compareItem, anchor, hasOpt, onUseCallback, side);
//                controller.SetEquipmentMaterialCellSize();
//                return controller;
//            }
//        }

//        controller.SetData(dto, anchor, hasOpt, onUseCallback, side);
//        UIModuleManager.Instance.SendOpenEvent(ProxyItemTipsModule.NAME, controller);
//        return controller;
//    }
//
//    /**
//     * 带选中框
//     */
//    public static void Open(ItemCellController cell, bool hasOpt = true,bool needCompare = true, System.Func<BagItemDto,bool> onUseCallback = null,ItemTipSide side= ItemTipSide.Left)
//    {
//        if (cell.GetData() != null)
//        {
////            SelectItem = cell;
//			Open(cell.GetData(),cell.gameObject, hasOpt,needCompare, onUseCallback,side);
//        }
//    }
//    public static void Open(BagItemDto cell, bool hasOpt = true, bool needCompare = true, System.Func<BagItemDto, bool> onUseCallback = null, ItemTipSide side = ItemTipSide.Left)
//    {
//        if (cell.GetData() != null)
//        {
////            SelectItem = cell;
//            Open(cell.GetData(), cell.gameObject, hasOpt, needCompare, onUseCallback, side);
//        }
//    }
	//两个物品对比查看的
//    public static ItemTipsViewController OpenCompareItemTip(BagItemDto dto, BagItemDto compareItem, GameObject anchor, bool hasOpt = true, System.Func<BagItemDto, bool> onUseCallback = null, ItemTipSide side = ItemTipSide.Left)
//    {
//		if(dto == null)
//            return null;
//		ItemTipsViewController controller = OpenView(dto);
//
//		controller.SetCompareData(dto,compareItem,anchor,hasOpt,onUseCallback,side);
//        return controller;
//	}

    /**
     *  bool hasOpt : 是否显示底部操作按钮
     */
//    public static ItemTipsViewController Open(BagItemDto dto, GameObject anchor, bool hasOpt = true, bool needCompare = false, System.Func<BagItemDto, bool> onUseCallback = null, ItemTipSide side = ItemTipSide.Left)
//	{
//		if(dto == null)
//            return null;
//		ItemTipsViewController controller = OpenView(dto);
//
////		if(needCompare && dto.item.itemType == AppItem.ItemTypeEnum_Equipment && dto.item is Equipment)
////        {
////            BagItemDto compareItem = ModelManager.Backpack.GetEquipByPartType((dto.item as Equipment).equipPartType);
////			if(compareItem != null && compareItem != dto){
////				controller.SetCompareData(dto,compareItem,anchor,hasOpt,onUseCallback,side);
////                return controller;
////			}
////		}
//
//        controller.SetData(dto,anchor,hasOpt,onUseCallback,side);
//
//        UIModuleManager.Instance.SendOpenEvent(ProxyItemTipsModule.NAME, controller);
//
//        return controller;
//	}
//
//    public static void OpenGainWayByItemIds(List<AppItem> itemList, Vector3 localPos){
//        if (itemList == null || itemList.Count == 0) return;
//        GameObject view = UIModuleManager.Instance.OpenFunModule(NAME, UILayerType.ItemTip, false);
//        var controller = view.GetMissingComponent<MultiItemGainwayTipsViewController>();
//
//        controller.SetData(itemList);
//
//        view.transform.localPosition = localPos;
//    }
//
//    public static void OpenGainWayByItemIds(List<AppItem> itemList){
//		if(itemList == null ||  itemList.Count == 0) return;
//		GameObject view = UIModuleManager.Instance.OpenFunModule(NAME,UILayerType.ItemTip, false);
//		var controller = view.GetMissingComponent<MultiItemGainwayTipsViewController>();
//
//		controller.SetData(itemList);
//	}
//
//	public static void OpenGainWay(int itemId){
//	    BagItemDto itemDto = ItemHelper.ItemIdToPackItemDto(itemId);
//		OpenGainWay(itemDto);
//	}
//
//	public static void OpenGainWay(BagItemDto dto)
//	{
//		if (dto == null) return;
//		ItemTipsViewController controller = OpenView(dto);
//
//		controller.SetGainWay(dto);
//
//        UIModuleManager.Instance.SendOpenEvent(ProxyItemTipsModule.NAME, controller);
//    }
//
//	private static ItemTipsViewController OpenView(BagItemDto dto){
//	    GameDebuger.LogError("adsfsafs");
//		GameObject view = UIModuleManager.Instance.OpenFunModule(NAME,UILayerType.ItemTip,false);
//		switch (dto.item.itemType)
//		{
//		case AppItem.ItemTypeEnum_Props:
//		case AppItem.ItemTypeEnum_PetSkillBook:
//			return view.GetMissingComponent<PropsTipsViewController>();
//		case AppItem.ItemTypeEnum_Equipment:
//                {
//                    if(dto.item is Equipment)
//			return view.GetMissingComponent<EquipmentTipsViewController>();
//                    else
//                        return view.GetMissingComponent<MagicEquipmentTipsViewController>();
//                }
//
//		case AppItem.ItemTypeEnum_PetEquipment:
//			return view.GetMissingComponent<PetEquipmentTipsViewController>();
//        case AppItem.ItemTypeEnum_FashionDress:
//            return view.GetMissingComponent<FashionTipsViewController>();
//		default:
//			return view.GetMissingComponent<ItemTipsViewController>();
//		}
//	}
//
//    public static IceBoxItemTipsViewController OpenIceBoxItemTipsView(BagItemDto dto, GameObject anchor,bool myFereIceBox, bool hasOpt = true, bool needCompare = false, System.Func<PackItemDto, bool> onUseCallback = null, ItemTipSide side = ItemTipSide.Left)
//    {
//        GameObject view = UIModuleManager.Instance.OpenFunModule(NAME, UILayerType.ItemTip, false);
//        IceBoxItemTipsViewController con = view.GetMissingComponent<IceBoxItemTipsViewController>();
//
//        con.SetData(dto, anchor, hasOpt, onUseCallback, side);
//        con.SetMyIceBox(myFereIceBox);
//        return con;
//    }
//
//
//    public static void OpenVirtualItemTip(AppVirtualItem virtualItem,GameObject anchor,ItemTipSide side= ItemTipSide.Left){
//
//		GameObject view = UIModuleManager.Instance.OpenFunModule(NAME,UILayerType.ItemTip, false);
//
//		VirtualItemTipsViewController controller = view.GetMissingComponent<VirtualItemTipsViewController>();
//
//		controller.SetData(virtualItem,anchor,side);
//	}

//	public static void OpenMissionItemTip(AppMissionItem missionItem,GameObject anchor,ItemTipSide side= ItemTipSide.Left){
//
//		GameObject view = UIModuleManager.Instance.OpenFunModule(NAME,UILayerType.ItemTip, false);
//
//		BaseItemTipsViewController controller = view.GetMissingComponent<BaseItemTipsViewController>();
//
//		controller.SetData(missionItem,anchor,side);
//	}

//	public static void Open(int itemId,GameObject anchor,ItemTipSide side= ItemTipSide.Left)
//	{
//		AppVirtualItem tAppVirtualItem = DataCache.getDtoByCls<GeneralItem>(itemId) as AppVirtualItem;
//		if (tAppVirtualItem == null) {
//		    BagItemDto dto = ItemHelper.ItemIdToPackItemDto(itemId);
//			ProxyItemTipsModule.Open(dto,anchor,false,false,null,side);
//		} else {
//			ProxyItemTipsModule.OpenVirtualItemTip(tAppVirtualItem, anchor, side);
//		}
//	}

	//	摆摊信息
//	public static void Open(StallGoodsDto stallGoodsDto,GameObject anchor,ItemTipSide side= ItemTipSide.Left) {
//		AppVirtualItem tAppVirtualItem = DataCache.getDtoByCls<GeneralItem>(stallGoodsDto.id) as AppVirtualItem;
//		if (tAppVirtualItem == null) {
//		    BagItemDto dto = ItemHelper.ItemIdToPackItemDto(stallGoodsDto.id);
//			dto.extra = stallGoodsDto.extra;
//			ProxyItemTipsModule.Open(dto,anchor,false,false,null,side);
//		} else {
//			ProxyItemTipsModule.OpenVirtualItemTip(tAppVirtualItem, anchor, side);
//		}
//	}

	//	拍卖信息
//	public static void Open(AuctionGoodsDto auctionGoodsDto, GameObject anchor, ItemTipSide side = ItemTipSide.Left) {
//		if(auctionGoodsDto.extra is PetCharactorDto){
//			ProxyPetTipsModule.OpenPetInfo(auctionGoodsDto.extra as PetCharactorDto);
//		}else{
//			AppVirtualItem tAppVirtualItem = DataCache.getDtoByCls<GeneralItem>(auctionGoodsDto.id) as AppVirtualItem;
//			if (tAppVirtualItem == null) {
//			    BagItemDto dto = ItemHelper.ItemIdToPackItemDto(auctionGoodsDto.id);
//				if (dto != null) {
//					dto.extra = auctionGoodsDto.extra;
//					ProxyItemTipsModule.Open(dto, anchor, false, false, null, side);
//				} else {
//					Pet tPet = DataCache.getDtoByCls<GeneralCharactor>(auctionGoodsDto.id) as Pet;
//					ProxyItemTipsModule.OpenPetTip(tPet, anchor, side);
//				}
//			} else {
//				ProxyItemTipsModule.OpenVirtualItemTip(tAppVirtualItem, anchor, side);
//			}
//		}
//	}

//	public static void OpenPetEqInfo(PetEquipmentExtraDto petEqInfo,GameObject anchor){
//	    BagItemDto dto = ItemHelper.PetEqExtraDtoToPackItemDto(petEqInfo);
//		ProxyItemTipsModule.Open(dto,anchor,false,false,null);
//	}

//	public static void OpenPetTipById(int petId,GameObject anchor,ItemTipSide side=ItemTipSide.Left){
//		Pet pet = DataCache.getDtoByCls<GeneralCharactor>(petId) as Pet;
//		OpenPetTip(pet,anchor,side);
//	}
//
//	public static void OpenPetTip(Pet pet, GameObject anchor,ItemTipSide side=ItemTipSide.Left){
//		if(pet == null) return;
//
//		GameObject view = UIModuleManager.Instance.OpenFunModule(NAME,UILayerType.ItemTip, false);
//		PetTipsViewController controller = view.GetMissingComponent<PetTipsViewController>();
//
//		controller.SetData(pet,anchor,side);
//	}
//
//	public static void OpenCrewTipById(int crewId,GameObject anchor,ItemTipSide side=ItemTipSide.Left){
//		Crew crew = DataCache.getDtoByCls<GeneralCharactor>(crewId) as Crew;
//		OpenCrewTip(crew,anchor,side);
//	}

//	public static void OpenCrewTip(Crew crew, GameObject anchor,ItemTipSide side=ItemTipSide.Left){
//		if(crew == null) return;
//
//		GameObject view = UIModuleManager.Instance.OpenFunModule(NAME,UILayerType.ItemTip, false);
//		CrewTipsViewController controller = view.GetMissingComponent<CrewTipsViewController>();
//
//		controller.SetData(crew,anchor,side);
//	}

//	public static void OpenEquipSimpleInfo(BagItemDto dto,GameObject anchor)
//	{
//		GameObject view = UIModuleManager.Instance.OpenFunModule(NAME,UILayerType.FourModule,false);
//		EquipSimpleInfoTipsViewController controller = view.GetMissingComponent<EquipSimpleInfoTipsViewController>();
//
//		controller.SetData(dto,anchor,false,null,ItemTipSide.Left);
//	}

//	public static void OpenEquipSimpleInfo(Equipment equipment,GameObject anchor)
//	{
//	    BagItemDto dto = ItemHelper.AppItemToPackItemDto(equipment);
//		OpenEquipSimpleInfo(dto,anchor);
//    }

  

//    public static void Show()
//	{
//		UIModuleManager.Instance.OpenFunModule(NAME,UILayerType.FourModule,false);
//	}
//
//	public static void Hide()
//	{
//		UIModuleManager.Instance.HideModule(NAME);
//	}
//
//	public static void Close()
//	{
//        if (_SelectItem != null)
//        {
//            _SelectItem.isSelect = false;
//            _SelectItem = null;
//        }
//		UIModuleManager.Instance.CloseModule(NAME);
//	}
}