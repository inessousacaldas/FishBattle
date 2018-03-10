// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : xush
// Created  : 8/25/2017 6:05:18 PM
// **********************************************************************

using System;
using System.Collections.Generic;
using AppDto;
using AppServices;

public sealed partial class QuartzDataMgr
{
    public static class QuartzNetMsg
    {
        //结晶回路突破
        //OrbmentInfoDto
        public static void Quartz_Break(long id, long itemUid)
        {
            GameUtil.GeneralReq(Services.Quartz_Break(id, itemUid), response =>
            {
                var orbmentInfoDto = response as OrbmentInfoDto;
                if (orbmentInfoDto != null)
                {
                    TipManager.AddTip("突破成功");
                    DataMgr._data.UpdateOrbmentDto(orbmentInfoDto);
                    DataMgr._data.SetCurOrbmentData(orbmentInfoDto);
                    FireData();
                }
            }, () =>
            {
                FireData();
            });
        }

        //结晶回路打造
        //QuartzSmithDto
        public static void Quartz_Smith(int grade, bool strengSmith)
        {
            GameUtil.GeneralReq(Services.Quartz_Smith(grade, strengSmith), response =>
            {
                QuartzSmithDto dto = response as QuartzSmithDto;
                if (dto == null)
                {
                    GameDebuger.Log("Quartz_Smith返回的数据有问题,请检查");
                    return;
                }
                DataMgr._data.GetOrbmentDto.curSmithCount = dto.curSmithCount;
                TipManager.AddTip("打造成功");
                FireData();
            });
        }
        
        //结晶回路强化
        //BagItemDto
        public static void Quartz_Streng(long id,long itemUid)
        {
            GameUtil.GeneralReq(Services.Quartz_Streng(id, itemUid), response =>
            {
                var itemDto = response as BagItemDto;
                if (itemDto != null)
                {
                    var orbmentInfoDto = DataMgr._data.GetOrbmentById(id);
                    var dto = orbmentInfoDto.slotsDto.Find(d => d.bagItemDto.uniqueId == itemDto.uniqueId);
                    if (dto != null)
                    {
                        TipManager.AddTip("强化成功");
                        SlotsDto slotsDto = new SlotsDto();
                        slotsDto.position = dto.position;
                        slotsDto.bagItemDto = itemDto;
                        orbmentInfoDto.slotsDto.ReplaceOrAdd(d=>d.bagItemDto.uniqueId == itemDto.uniqueId, slotsDto);

                        DataMgr._data.UpdateOrbmentDto(orbmentInfoDto);
                        DataMgr._data.SetCurOrbmentData(orbmentInfoDto);
                        FireData();
                    }
                }
            }, () =>
            {
                FireData();
            });
        }

        //结晶回路卸下
        //OrbmentInfoDto
        public static void Quartz_TakeOff(long id, int position)
        {
            GameUtil.GeneralReq(Services.Quartz_TakeOff(id, position), response =>
            {
                var dto = response as OrbmentInfoDto;
                if (dto != null)
                {
                    DataMgr._data.UpdateOrbmentDto(dto);
                    DataMgr._data.SetCurOrbmentData(DataMgr._data.GetCurOrbmentIdx);
                    FireData();
                }
                else
                    TipManager.AddTip("=====返回数据有误=====");
            });
        }

        //装备结晶回路
        //OrbmentInfoDto
        public static void Quartz_Wear(long id,long itemUid,int position)
        {
            GameUtil.GeneralReq(Services.Quartz_Wear(id, itemUid, position), response =>
            {
                var dto = response as OrbmentInfoDto;
                if (dto != null)
                {
                    DataMgr._data.UpdateOrbmentDto(dto);
                    DataMgr._data.SetCurOrbmentData(DataMgr._data.GetCurOrbmentIdx);
                    FireData();
                }
                else
                    TipManager.AddTip("=====返回数据有误=====");
            });
        }

        //导力器信息
        //OrbmentDto
        public static void Orbment_Info(Action callback)
        {
            GameUtil.GeneralReq(Services.Orbment_Info(), respose =>
            {
                var dto = respose as OrbmentDto;
                if (dto != null)
                {
                    DataMgr._data.RefreshOrbmentDto(dto);
                    DataMgr._data.CurTabPage = TabEnum.Info;
                    DataMgr._data.SetCurOrbmentData(0); //默认选中第一个
                }
                else
                    TipManager.AddTip("==========返回数据有误=========");

                if (callback != null)
                    callback();
            });
        }

        //魔法装配
        //MagicChangeDto
        public static void Magic_Wear(long id, int takeOffId, int wearId)
        {
            GameUtil.GeneralReq(Services.Magic_Wear(id, takeOffId, wearId), respose =>
            {
                var orbmentInfoDto = DataMgr._data.GetOrbmentById(id);
                var changetDto = respose as MagicChangeDto;
                if (changetDto == null)
                {
                    TipManager.AddTip("======返回数据有误======");
                    return;
                }
                TipManager.AddTip("成功装备魔法技能");
                orbmentInfoDto.magic = changetDto.magic;
                DataMgr._data.SetCurOrbmentData(orbmentInfoDto);
                FireData();
            });
        }

        //魔法卸下
        //MagicChangeDto
        public static void Magic_TakeOff(long id, int skillMapId)
        {
            GameUtil.GeneralReq(Services.Magic_TakeOff(id, skillMapId), respose =>
            {
                var orbmentInfoDto = DataMgr._data.GetOrbmentById(id);
                var changetDto = respose as MagicChangeDto;
                if (changetDto == null)
                {
                    TipManager.AddTip("======返回数据有误======");
                    return;
                }
                TipManager.AddTip("成功卸下魔法技能");
                orbmentInfoDto.magic = changetDto.magic;
                DataMgr._data.SetCurOrbmentData(orbmentInfoDto);
                FireData();
            });
        }
    }
}
