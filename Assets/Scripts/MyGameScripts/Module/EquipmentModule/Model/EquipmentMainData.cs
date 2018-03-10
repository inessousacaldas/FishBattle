// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : Zijian
// Created  : 8/29/2017 4:01:34 PM
// **********************************************************************
using System;
using AppDto;
using System.Collections.Generic;
using UniRx;
namespace StaticInit
{
    public partial class StaticInit
    {
        private StaticDispose.StaticDelegateRunner initEquipmentMainDataMgr = new StaticDispose.StaticDelegateRunner
            (()=> { var data = EquipmentMainDataMgr.DataMgr; });
    }
}
public enum EquipmentViewTab : int
{
    Smith = 0, //打造
    EquipmentReset = 1,//洗练
    EquipmentMedallion = 2, //纹章
    EquipmentEmbed =3,//宝石
}
public interface IEquipmentMainData
{
    EquipmentViewTab CurTab { get; }

    /// <summary>
    /// 当前玩家的装备穿戴信息 
    /// </summary>
    EquipmentInfoDto CurrentEquipmentInfo { get; }
    IEquipmentSmithViewData SmithViewData { get; }

    IEquipmentResetViewData ResetViewData { get; }

    IEquipmentInsetMedallionViewData MedallionViewData { get; }

    IEquipmentEmbedViewData EmbedViewData { get; }
}

public sealed partial class EquipmentMainDataMgr
{
    public sealed partial class EquipmentMainData:IEquipmentMainData
    {
        public static readonly List<ITabInfo> _TabInfos = new List<ITabInfo>()
        {
            TabInfoData.Create((int)EquipmentViewTab.Smith,"打造"),
            TabInfoData.Create((int)EquipmentViewTab.EquipmentReset,"洗炼"),
            TabInfoData.Create((int)EquipmentViewTab.EquipmentEmbed,"宝石"),
            TabInfoData.Create((int)EquipmentViewTab.EquipmentMedallion,"纹章"),
        };
        /// <summary>
        /// 装备表的缓存
        /// </summary>
        public List<Equipment> eq_List = new List<Equipment>();

        public EquipmentViewTab CurTab { get; set; }
        public EquipmentSmithViewData SmithData;
        public EquipmentResetViewData ResetData;
        public EquipmentInsetMedallionViewData MedallionData;
        public EquipmentEmbedViewData EmbedData;

        private DailyLimit _smithLimit;
        private DailyLimit _resetLimit;
        EquipmentInfoDto _currentEquipmentInfo;
        public EquipmentInfoDto CurrentEquipmentInfo
        {
            get
            {
                return _currentEquipmentInfo;
            }
        }
        #region NetUpdate
        public void UpdateEquipmentInfo(EquipmentInfoDto dto)
        {
            _currentEquipmentInfo = dto;
            _currentEquipmentInfo.curSmithCount = _smithLimit.limit - dto.curSmithCount;
            _currentEquipmentInfo.curResetCount = _resetLimit.limit - dto.curResetCount;
        }

        public void UpdateCurSmithCount(int count)
        {
            _currentEquipmentInfo.curSmithCount = _smithLimit.limit - count;
        }

        public void UpdateCurResetCount(int count)
        {
            _currentEquipmentInfo.curResetCount = _resetLimit.limit - count;
        }

        /// <summary>
        /// 更新玩家身上装备上的信息
        /// 背包里面的装备无需更新，因为增删改 都会有BagItemNotify进行返回操作~
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="part"></param>
        /// <param name="dto"></param>
        public void UpdateEquipmentExtraDto(long uid, EquipmentExtraDto dto)
        {
            var equipmentList = DataMgr.GetEquipmentDtoList(EquipmentHoldTab.Equip);
            var equipment = equipmentList.Find(x => x.equipUid == uid);
            if (equipment != null)
            {
                equipment.property = dto;
            }
            else
            {
                GameLog.LogEquipment("UpdateEquipmentExtraDto failed 因为没有在身上找到这件装备");
            }
        }
        public void UpdateEqupiment_Wear(EquipmentDto dto)
        {
            CurrentEquipmentInfo.current.ReplaceOrAdd(x=>x.partType == dto.partType,dto);
        }
        public void UpdateEquipment_TakeOff(int part)
        {
            var equipmentIndex = CurrentEquipmentInfo.current.FindIndex(x => x.partType == (int)part);
            if(equipmentIndex >= 0)
                CurrentEquipmentInfo.current.RemoveAt(equipmentIndex);
        }
        #endregion
        public EquipmentDto GetCurEquipmentByType(Equipment.PartType part)
        {
            if (CurrentEquipmentInfo == null)
            {
                GameLog.LogEquipment("还没向服务器获取个人装备信息");
                return null;
            }

            var equipment = CurrentEquipmentInfo.current.Find(x => x.partType == (int)part);
            return equipment;
        }
        public void InitData()
        {
            eq_List = DataCache.getArrayByCls<GeneralItem>()
                .Filter(x => x is Equipment).Map(x => x as Equipment).ToList();

            SmithData = new EquipmentSmithViewData();
            SmithData.InitData();
            ResetData = new EquipmentResetViewData();
            ResetData.InitData();
            EmbedData = new EquipmentEmbedViewData();
            EmbedData.InitData();
            MedallionData = new EquipmentInsetMedallionViewData();
            MedallionData.IninData();

            _smithLimit = DataCache.getDtoByCls<DailyLimit>(22);
            _resetLimit = DataCache.getDtoByCls<DailyLimit>(23);
        }

        public void Dispose()
        {

        }

        #region ViewData
        public IEquipmentSmithViewData SmithViewData
        {
            get
            {
                return SmithData;
            }
        }

        public IEquipmentResetViewData ResetViewData
        {
            get
            {
                return ResetData;
            }
        }

        public IEquipmentInsetMedallionViewData MedallionViewData
        {
            get
            {
                return MedallionData;
            }
        }

        public IEquipmentEmbedViewData EmbedViewData
        {
            get
            {
                return EmbedData;
            }
        }
        #endregion
    }
}
