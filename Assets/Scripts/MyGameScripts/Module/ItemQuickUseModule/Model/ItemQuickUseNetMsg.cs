// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : xush
// Created  : 3/3/2018 4:15:19 PM
// **********************************************************************

using System;
using System.Collections.Generic;
using AppDto;
using AppServices;

public sealed partial class ItemQuickUseDataMgr
{
    public static class ItemQuickUseNetMsg
    {
        public static void ApplyBackpack(BagItemDto dto)
        {
            ServiceRequestAction.requestServer(Services.Backpack_Apply(dto.index, dto.count, ""), "", res =>
            {
                DataMgr._data.RemoveItemList(dto);
                FireData();
            });
        }

        public static void ApplyEquipMent(BagItemDto dto)
        {
            var equipmentDto = EquipmentMainDataMgr.DataMgr.MakeEquipmentDto(dto);
            if (equipmentDto == null)
            {
                GameDebuger.LogError("====物品快速使用数据有问题===");
                return;
            }
            var equip = equipmentDto.equip as Equipment;
            if (equip == null)
            {
                GameDebuger.LogError("====物品快速使用数据有问题===");
                return;
            }

            GameUtil.GeneralReq(Services.Equip_Wear(equipmentDto.equipUid, equip.equipType), resp =>
            {
                DataMgr._data.RemoveItemList(dto);
                FireData();
            });
        }
    }
}
