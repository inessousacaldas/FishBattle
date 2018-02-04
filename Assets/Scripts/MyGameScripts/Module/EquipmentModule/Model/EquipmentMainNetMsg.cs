// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : Zijian
// Created  : 8/29/2017 4:01:34 PM
// **********************************************************************

using System;
using System.Collections.Generic;
using AppDto;
using AppServices;
using System.Linq;
using UniRx;
using UnityEngine;

public sealed partial class EquipmentMainDataMgr
{
   
    public static class EquipmentMainNetMsg
    {

        /// <summary>
        /// 获取玩家身上的装备信息
        /// </summary>
        public static void ReqEquipmentInfo()
        {
            GameUtil.GeneralReq(Services.Equip_Info(), resp =>
            {
                var dto = resp as EquipmentInfoDto;
                DataMgr._data.UpdateEquipmentInfo(dto);
                DataMgr._data.ResetData.UpdateCurEquipmentItemList();
                DataMgr._data.MedallionData.UpdateCurEquipmentItemList();
                FireData();
            });
        }
        /// <summary>
        /// 切换穿戴方案
        /// </summary>
        /// <param name="caseId"></param>
        public static void ReqEquip_CaseSwitch(int caseId)
        {
            GameUtil.GeneralReq(Services.Equip_CaseSwitch(caseId), resp => {
                DataMgr._data.CurrentEquipmentInfo.activeId = caseId;
            });
        }
        /// <summary>
        /// 请求穿上装备
        /// </summary>
        /// <param name="equipmentDto"></param>
        /// <param name="partType"></param>
        public static void ReqEquip_Wear(EquipmentDto equipmentDto,int partType = -1)
        {
            var _equipId = equipmentDto.equipUid;
            var equip = equipmentDto.equip as Equipment;
            if(partType == -1)
            {
                partType = equip.partType[0];
                //饰品需要额外处理
                if(partType == (int)Equipment.PartType.AccOne || partType == (int)Equipment.PartType.AccTwo)
                {
                    var curAccEquip =  DataMgr.GetAccEmptyHole();
                    partType = (int)curAccEquip;
                }
                
            }
            GameUtil.GeneralReq(Services.Equip_Wear(_equipId, partType), resp => {
                TipManager.AddTopTip("装备成功");
                var dto = resp as EquipmentDto;
                DataMgr._data.UpdateEqupiment_Wear(dto);

                DataMgr._onEquipmentWearStream.OnNext(dto);
                FireData();
            });
        }

        /// <summary>
        /// 请求脱下装备
        /// </summary>
        /// <param name="equipId"></param>
        /// <param name="itemIndex"></param>
        public static void ReqTakeOffEquipment(EquipmentDto dto)
        {
            GameUtil.GeneralReq(Services.Equip_TakeOff(dto.partType,-1), resp =>
            {
                DataMgr._data.UpdateEquipment_TakeOff(dto.partType);

                DataMgr._onEquipmentTakeOffStream.OnNext(dto);
                FireData();
            });
        }
        //打造请求
        public static void ReqEquipmentSmith(int equipId,int quality,bool fastsmith)
        {
            GameUtil.GeneralReq(Services.Equip_Smith(equipId, quality, fastsmith), resp => {
                var dto = resp as OnSmithEquipemntDto;
                var equipmentDto = dto.edto;
                var atrifactChange = dto.adto;
                //更替神器值
                DataMgr._data.CurrentEquipmentInfo.atrifacts.ReplaceOrAdd(x => x.aid == atrifactChange.aid, atrifactChange);

                var equip = DataCache.getDtoByCls<GeneralItem>(equipmentDto.equipId) as Equipment;

                TipManager.AddTopTip("恭喜你成功打造" + equip.name);

                //DataMgr.ShowPv_Equipment(equipmentDto);
                OpenEquipmentItemTips(equipmentDto);
                FireData();
            });
        }
        private static void OpenEquipmentItemTips(EquipmentDto _equipmentDto)
        {
            Vector3 tipsShowPos = new Vector3(0, 168, 0.0f);
            if(DataMgr.GetSameEquipmentByPart(_equipmentDto)==null)
            {
                tipsShowPos = new Vector3(-110, 168);
            }
            var _tipsCtrl = ProxyTips.OpenEquipmentTips_FromBag(_equipmentDto, isShowCompare: true);
            _tipsCtrl.SetTipsPosition(tipsShowPos);
        }

        /// <summary>
        /// 装备洗练
        /// </summary>
        public static void ReqEquipmentReset(long uid, EquipmentHoldTab tab)
        {
            GameUtil.GeneralReq(Services.Equip_ResetPreview(uid), resp => {
                //var dto = resp as EquipmentDto;
                var dto = resp as EquipmentExtraDto;
                if (tab == EquipmentHoldTab.Equip)
                {
                    DataMgr._data.UpdateEquipmentExtraDto(uid, dto);
                    DataMgr._data.ResetData.UpdateCurEquipmentPropertRange();
                }
                else
                {
                    var equipments = DataMgr._data.ResetData.EquipmentItems;
                    var equipment = equipments.Find(x => x.equipUid == uid);
                    DataMgr._data.ResetData.CurChoiceEquipment = equipment;
                }
                    
               
                FireData();
                TipManager.AddTopTip("洗练成功");
                //DataMgr._data.ResetData.CurResetEquipmentResult = dto.equipExtraDto;
            });
        }
            
        /// <summary>
        /// 洗练保存
        /// </summary>
        public static void ReqEquipmentResetSave(long uid, EquipmentHoldTab tab)
        {
            GameUtil.GeneralReq(Services.Equip_ResetSave(uid), resp =>
            {
                var dto = resp as EquipmentExtraDto;
                if (tab == EquipmentHoldTab.Equip)
                {
                    DataMgr._data.UpdateEquipmentExtraDto(uid, dto);
                    DataMgr._data.ResetData.UpdateCurEquipmentPropertRange();
                }
                else
                {
                    var equipments = DataMgr._data.ResetData.EquipmentItems;
                    var equipment = equipments.Find(x => x.equipUid == uid);
                    DataMgr._data.ResetData.CurChoiceEquipment = equipment;
                }
                FireData();
                TipManager.AddTopTip("洗练保存成功");
            });
        }


        /// <summary>
        /// 获取玩家宝石部位信息
        /// </summary>
        public static void ReqEmbedInfo()
        {
            GameUtil.GeneralReq(Services.Embed_Info(), resp =>
            {
                var dto = resp as EmbedHolesDto;
                DataMgr._data.EmbedData.UpdateHolesDto(dto);
            });
        }

        /// <summary>
        /// 宝石镶嵌
        /// <param name="aperture">对应的宝石孔</param>
        /// </summary>
        public static void ReqEmbed_In(int itemid,Equipment.PartType part,int aperture)
        {
            var EmbedData = DataMgr._data.EmbedData;
            GameUtil.GeneralReq(Services.Embed_In(itemid,(int)part,aperture), resp => {
                TipManager.AddTopTip("宝石镶嵌成功");
                var dto = resp as EmbedApertureDto;
                EmbedData.UpdateApertureDto((int)part,dto,true);
                FireData();
            });
        }
        /// <summary>
        /// 宝石脱下
        /// </summary>
        public static void ReqEmbed_Off(EmbedApertureDto dto, Equipment.PartType part)
        {
            var EmbedData = DataMgr._data.EmbedData;
            GameUtil.GeneralReq(Services.Embed_Pluck((int)part, dto.apId), resp => {
                TipManager.AddTopTip("宝石脱下成功");
                EmbedData.UpdateApertureDto((int)part, dto, false);
                FireData();
            });
        }

		///装备纹章
        public static void ReqInsetMedallionInBag(long muid, long eqid)
        {
            GameUtil.GeneralReq(Services.Medallion_Inset(muid, eqid), resp => 
            {
                DataMgr._data.MedallionData.UpdateData();
                DataMgr._data.MedallionData.UpdateCurEquipmentItemList();
                FireData();
                TipManager.AddTopTip("装备镶嵌成功");
            });
        }

        public static void ReqInsetMedallionInWear(long muid, int part)
        {
            GameUtil.GeneralReq(Services.Medallion_InsetWear(muid, part), resp =>
            {
                var dto = resp as EquipmentExtraDto;
                var equipment = DataMgr._data.GetCurEquipmentByType((Equipment.PartType)part);
                if(equipment!=null)
                {
                    DataMgr._data.UpdateEquipmentExtraDto(equipment.equipUid, dto);
                }
                
                DataMgr._data.MedallionData.UpdateCurEquipmentItemList();
                FireData();

                TipManager.AddTopTip("装备镶嵌成功");
            });
        }
    }
}
