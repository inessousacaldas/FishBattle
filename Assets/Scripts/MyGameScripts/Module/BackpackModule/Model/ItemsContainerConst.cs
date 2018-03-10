using System;
using AppDto;
// **********************************************************************
// Copyright (c) 2013 Baoyugame. All rights reserved.
// File     :  ItemsContainerConst.cs
// Author   : willson
// Created  : 2015/1/16 
// Porpuse  : 
// **********************************************************************
using System.Collections.Generic;

public class ItemsContainerConst
{
	// 1.背包 2.仓库 用于 BackpackOrWarehouseItemCellController 判读双击事件
	public static int ModuleType = 0;
    public const int ModuleType_Other = 0;
	public const int ModuleType_Backpack = 1;
	public const int ModuleType_Warehouse = 2;
    public const int ModuleType_Fashion = 3;
    public const int ModuleType_MagicWare = 4;

    public const int RowCapability = 5;  //每行5个
    public const int PageCapability = 25;

    static public int SortBagItemDto(BagItemDto lhs, BagItemDto rhs)
	{
        AppItem lhsItem = lhs.item as AppItem;
        AppItem rhsItem = rhs.item as AppItem;
        int diff = lhsItem.sort - rhsItem.sort;
        if (diff != 0)
            return diff;

	    //todo fish
//        if (rhsItem is Equipment && lhsItem is Equipment)
//        {
//            diff = ModelManager.Equipment.GetEquipLevel(rhs) - ModelManager.Equipment.GetEquipLevel(lhs);
//            if (diff != 0)
//                return diff;
//        }
        diff = lhsItem.id - rhsItem.id;
        if (diff != 0)
            return diff;
        diff = lhs.circulationType - rhs.circulationType;
        if (diff != 0)
            return diff;
        diff = lhs.tradePrice - rhs.tradePrice;
        if (diff != 0)
            return diff;
        diff = rhs.count - lhs.count;
        if (diff != 0)
            return diff;
        return lhs.index - rhs.index;
	}

	static public void SortPackItemList(List<BagItemDto> list, List<BagItemDto> allList, int startIndex)
	{
		list.Sort(SortBagItemDto);

		int index = 0;
	    BagItemDto overPackItem = null;

		for (int i = 0; i < list.Count; i++)
		{
		    BagItemDto pageItemAdapter = list[i];
			if (pageItemAdapter.uniqueId > 0)
			{// 不可叠加的物品
				pageItemAdapter.index = (startIndex + index);
				index++;
			}
			else
			{// 可叠加的物品
				RealItem realItem = pageItemAdapter.item as RealItem;
				int maxOverlay = realItem.maxOverlay;
				if (pageItemAdapter.count >= maxOverlay)
				{// 已达到叠加上限
					pageItemAdapter.index = (startIndex + index);
					index++;
				}
				else
				{// 未达到叠加上限
					if (overPackItem != null)
					{
						if ((overPackItem.tradePrice == pageItemAdapter.tradePrice && pageItemAdapter.tradePrice == 0)
							|| (overPackItem.tradePrice > 0 && pageItemAdapter.tradePrice > 0))
						{// 仅从价格上判断有可能是同一种属性物品
							if (overPackItem.itemId == pageItemAdapter.itemId
								&& overPackItem.circulationType == pageItemAdapter.circulationType)
							{// 同一属性物品
								int originalItemCount = overPackItem.count;
								int canAddItemCount = maxOverlay - originalItemCount;
								if (canAddItemCount >= pageItemAdapter.count)
								{// 可全合并
									allList.Remove(pageItemAdapter);// 删除旧的
									overPackItem.count = (originalItemCount + pageItemAdapter.count);
								}
								else
								{// 可合并部分
									overPackItem.count = (maxOverlay);
									overPackItem = pageItemAdapter;
									pageItemAdapter.count = (pageItemAdapter.count - canAddItemCount);
									pageItemAdapter.index = (startIndex + index);
									index++;
								}
							}
							else
							{//// 不同属性物品
								overPackItem = pageItemAdapter;
								pageItemAdapter.index = (startIndex + index);
								index++;
							}
						}
						else
						{// 单从价格上判断不是同一属性物品
							overPackItem = pageItemAdapter;
							pageItemAdapter.index = (startIndex + index);
							index++;
						}
					}
					else
					{
						overPackItem = pageItemAdapter;
						pageItemAdapter.index = (startIndex + index);
						index++;
					}
				}
			}
		}
	}
}

public static class PackItemDtoComparer
{
	public static Comparison<BagItemDto> Compare = ItemsContainerConst.SortBagItemDto;

    // id 从小到大》 品质 》 包裹位置 》 堆叠数量
	public static Comparison<BagItemDto> ComposeItemSorter = delegate(BagItemDto x, BagItemDto y)
	{
        var differ =  y.itemId - x.itemId; 
		if (differ != 0)
			return differ;
		differ = x.item.quality - y.item.quality;
		if (differ != 0)
			return differ;
		if (x.bagId != y.bagId)
			return x.bagId == (int) AppItem.BagEnum.Backpack ? 1 : -1;
		else 
		{
			return x.count - y.count;
		}
	};
	
    // id从小到大 》 品质 》 包裹位置 》 堆叠数量
    public static Comparison<BagItemDto> DecomposeMaterialSorter = delegate(BagItemDto x, BagItemDto y)
        {
            var differ = y.itemId - x.itemId; 
            if (differ != 0)
                return differ;
            differ = x.item.quality - y.item.quality;
            if (differ != 0)
                return differ;
            if (x.bagId != y.bagId)
                return x.bagId == (int) AppItem.BagEnum.Backpack ? 1 : -1;
            else 
            {
                return x.count - y.count;
            }
        };
}