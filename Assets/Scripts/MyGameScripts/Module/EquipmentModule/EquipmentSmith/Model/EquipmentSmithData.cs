// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : Cilu
// Created  : 9/8/2017 9:50:40 AM
// **********************************************************************

using AppDto;
using System;
using System.Collections.Generic;
/// <summary>
/// 装备打造界面数据
/// </summary>
public interface IEquipmentSmithViewData
{
    int CurSelectFaction { get; }
    int CurSelcetGrade { get; }
    int CurSelectQuality { get; }
    //是否快捷打造
    bool FastSmith { get; }
    Equipment curSelectEquipement { get; }

    EquipmentDto lastSmithEquipment { get; }
    IEnumerable<Equipment> CurEquipmentCells { get; }

    IEnumerable<SmithItemVo> CurSmithItems { get; }


    int GetEquipmentAtrifactMaxValue(int grade, int part, int quality);
    int GetGetEquipmentAtrifactCurValue(int grade, int part, int quality);
    Equipment GetEquipmentInfoByPartType(Equipment.PartType part);
}
public sealed partial class EquipmentMainDataMgr
{
    public class EquipmentSmithViewData : IEquipmentSmithViewData
    {
        /// <summary>
        /// 装备打造开发等级
        /// </summary>
        public static int MinOpenValue { private set; get; }

        public static int MinChoiceGrade { private set; get; }
        /// <summary>
        /// 等级跨度~每10级算一个装备
        /// </summary>
        public static int GradeStep { private set; get; }

        /// <summary>
        /// 没有新手引导前~用来搞第一次展示的~
        /// </summary>
        public bool isFisrtOpen;
        List<EquipmentAtrifact> eq_atrifactConfigs;
        /// <summary>
        /// 设置默认值
        /// </summary>
        public void InitData()
        {
            var smithGradeConfig = DataCache.getDtoByCls<StaticConfig>(AppStaticConfigs.EQUIP_SMITH_GRADE_LIMIT);
            eq_atrifactConfigs = DataCache.getArrayByCls<EquipmentAtrifact>();

            MinOpenValue = StringHelper.ToInt(smithGradeConfig.value);
            GradeStep = 10;
            MinChoiceGrade = (MinOpenValue / 10) * 10;

            //TODO:等于人物等级
            var minGrade = (MinOpenValue / 10) * 10;
            int playerLevel = ModelManager.Player.GetPlayerLevel();
            _curSelcetGrade = (playerLevel / 10) * 10;
            _curSelcetGrade = _curSelcetGrade >= minGrade ? _curSelcetGrade : minGrade;
            _curSelectFaction = ModelManager.IPlayer.GetPlayer().factionId;
            var euipmentQuality = DataCache.getArrayByCls<EquipmentQuality>();
            _curSelectQuality = euipmentQuality.Find(x => x.id == (int)AppItem.QualityEnum.BLUE).id;
            FastSmith = false;
            UpdateCurEquipmentCells();
        }

        public int GetEquipmentAtrifactMaxValue(int grade,int part,int quality)
        {
            var res = eq_atrifactConfigs.Find(x => x.part == part && x.grade == grade && x.quality == quality);

            return res == null ? 0 : res.topval;
        }
        public int GetGetEquipmentAtrifactCurValue(int grade, int part, int quality)
        {
            var res = eq_atrifactConfigs.Find(x => x.part == part && x.grade == grade && x.quality == quality);
            if(res != null)
            {
                var atrifactCur = DataMgr._data.CurrentEquipmentInfo.atrifacts.Find(x => x.aid == res.id);
                return atrifactCur == null ? 0 : atrifactCur.curval;
            }
            else
            {
                return 0;
            }
            
        }
        public Equipment GetEquipmentInfoByPartType(Equipment.PartType part)
        {
            var eq_dic = EquipmentMainDataMgr.DataMgr.GetEquipmentList();
            var playerGrade = ModelManager.Player.GetPlayerLevel();
            //var playerGrade = PlayerModel.Stream.LastValue.GetPlayer().grade;
            var eq = eq_dic.Find(x =>
            x.smith
            && x.factionIds.Contains(CurSelectFaction)
            && x.partType.Contains((int)part)
            && x.grade >= CurSelcetGrade
            && x.grade < CurSelcetGrade + GradeStep
            //&& x.grade <= playerGrade    //TODO:如44级只开放 40级的装备~~      
            );


            return eq;
        }
        private Equipment _curSelectEquipment;
        public Equipment curSelectEquipement
        {
            get
            { return _curSelectEquipment; }
            set
            {
                var lastValue = _curSelectEquipment;
                _curSelectEquipment = value;
                if (lastValue != _curSelectEquipment)
                    UpdateCurSmithItems();
            }
        }
        private int _curSelcetGrade;
        public int CurSelcetGrade
        {
            get
            {
                return _curSelcetGrade;
            }
            set
            {
                var lastValue = _curSelcetGrade;
                _curSelcetGrade = value;
                if (lastValue != _curSelcetGrade)
                    UpdateCurEquipmentCells();
            }
        }

        private int _curSelectFaction;
        public int CurSelectFaction
        {
            get { return _curSelectFaction; }
            set
            {
                var lastValue = _curSelectFaction;
                _curSelectFaction = value;
                if (lastValue != _curSelectFaction)
                    UpdateCurEquipmentCells();
            }
        }

        private int _curSelectQuality;
        public int CurSelectQuality
        {
            get { return _curSelectQuality; }
            set
            {
                var lastValue = _curSelectQuality;
                _curSelectQuality = value;
                if (lastValue != _curSelectQuality)
                    UpdateCurSmithItems();
            }
        }
        public List<Equipment> _curSmithCells = new List<Equipment>();
        public IEnumerable<Equipment> CurEquipmentCells
        {
            get
            {
                return _curSmithCells;
            }
        }

        private List<SmithItemVo> _curSmithItems = new List<SmithItemVo>();
        public IEnumerable<SmithItemVo> CurSmithItems
        {
            get
            {
                return _curSmithItems;
            }
        }

        public bool FastSmith { get; set; }

        private EquipmentDto _lastSmithEquipment;

        /// <summary>
        /// 上一件打造的装备
        /// </summary>
        public EquipmentDto lastSmithEquipment
        {
            get
            {
                return _lastSmithEquipment;
            }
            set
            {
                _lastSmithEquipment = value;
            }
        }


        private void UpdateCurEquipmentCells()
        {
            _curSmithCells.Clear();
            var weapon = GetEquipmentInfoByPartType(Equipment.PartType.Weapon);
            if (weapon != null)
                _curSmithCells.Add(weapon);
            var clothes = GetEquipmentInfoByPartType(Equipment.PartType.Clothes);
            if (clothes != null)
                _curSmithCells.Add(clothes);
            var glove = GetEquipmentInfoByPartType(Equipment.PartType.Glove);
            if (glove != null)
                _curSmithCells.Add(glove);
            var shoe = GetEquipmentInfoByPartType(Equipment.PartType.Shoe);
            if (shoe != null)
                _curSmithCells.Add(shoe);
            var accone = GetEquipmentInfoByPartType(Equipment.PartType.AccOne);
            if (accone != null)
                _curSmithCells.Add(accone);

            //_curSmithCells = _curSmithCells.Filter(x => x != null).ToList();
            /// =====设置默认选中
            Equipment curChoice;
            curChoice = _curSmithCells.Find(x => x != null);
            curSelectEquipement = curChoice;
        }
        public void UpdateCurSmithItems()
        {
            _curSmithItems.Clear();
            if (curSelectEquipement == null)
                return;
            //陨铁
            var eq_quality = DataCache.getDtoByCls<EquipmentQuality>(CurSelectQuality);
            var ironItemId = eq_quality.itemId;
            var item = DataCache.getDtoByCls<GeneralItem>(ironItemId) as Props;
            if (item != null)
            {
                int ownerCount = BackpackDataMgr.DataMgr.GetItemCountByItemID(ironItemId);
                SmithItemVo ironVo = new SmithItemVo() { props = item, needCount = curSelectEquipement.smithIronAmount, currentCount = ownerCount };
                _curSmithItems.Add(ironVo);
            }


            _curSmithItems.AddRange(EquipmentMainDataMgr.DataMgr.GetEquipmentSmithItems(curSelectEquipement));
        }



    }
}

