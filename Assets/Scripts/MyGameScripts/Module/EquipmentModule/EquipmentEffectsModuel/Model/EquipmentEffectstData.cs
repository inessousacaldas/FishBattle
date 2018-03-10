// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : Zijian
// Created  : 9/22/2017 3:49:56 PM
// **********************************************************************

using AppDto;
using System;
using System.Collections.Generic;
using UniRx;
public interface IEquipmentEffectsData
{
    EquipmentMainDataMgr.EquipmentHoldTab CurTab { get; }
    IEnumerable<EquipmentDto> EquipmentItems { get; }

    EquipmentDto CurChoiceEquipment { get; }

    IEnumerable<Eq_GoodItemVo> PropsList { get; }
}

public sealed partial class EquipmentEffectsDataMgr
{
    public sealed partial class EquipmentEffectsData : IEquipmentEffectsData
    {
       
        public IEnumerable<EquipmentDto> EquipmentItems
        {
            get
            {
                var equipList = EquipmentMainDataMgr.DataMgr.GetEquipmentDtoList(CurTab);
                equipList.Sort(
                    (a, b) =>
                    {
                        //装备等级
                        var eq_a = a.equip as Equipment;
                        var eq_b = b.equip as Equipment;
                        if (eq_a.grade != eq_b.grade)
                            return eq_a.grade - eq_b.grade;
                        //装备品质
                        if (eq_a.quality != eq_b.quality)
                            return eq_a.quality - eq_b.quality;
                        //TODO：带装备特效的

                        //部分排序 
                        var eqpart_a = eq_a.partType[0];
                        var eqpart_b = eq_b.partType[0];
                        if (eqpart_a != eqpart_b)
                            return eqpart_b - eqpart_a;

                        //装备ID排序
                        return a.equipId - b.equipId;
                    });
                return equipList;
            }
        }

        public EquipmentMainDataMgr.EquipmentHoldTab CurTab { get; set; }

        public EquipmentDto CurChoiceEquipment { get; set; }

        private List<Eq_GoodItemVo> _propsList = new List<Eq_GoodItemVo>();
        public IEnumerable<Eq_GoodItemVo> PropsList
        {
            get
            {
                _propsList.Clear();
                //获取特技石~
                var bagItemDto = BackpackDataMgr.DataMgr.GetBagItems( AppItem.ItemTypeEnum.Embed);
                var res = bagItemDto.Filter(x =>
                {
                    // var props = x.item as Props;
                    //var embedParam = props.propsParam as PropsParam_1;
                    //return embedParam.partTypes.Contains((int)CurChoicePartVo.part);
                    return false;
                });

                res.ForEach(x =>
                {
                    //TODO:更换为特效的形式
                    var props = x.item as Props;
                    var embedParam = props.propsParam as PropsParam_1;
                    var cbConfig = DataCache.getDtoByCls<CharacterAbility>(embedParam.caId);
                    string des = string.Format("{0}+{1}", cbConfig.name, embedParam.value);
                    var vo = new Eq_GoodItemVo(props, x.count, des);
                    _propsList.Add(vo);
                });

                return _propsList;
            }
        }

        public void InitData()
        {
            CurTab = EquipmentMainDataMgr.EquipmentHoldTab.Equip;
            //默认选中第一个~ // 注意，这里的数据层并非一开始就读取，所以可以在打开界面的时候直接获取数据~
            var tempList = EquipmentMainDataMgr.DataMgr.GetEquipmentDtoList(CurTab);
            CurChoiceEquipment = tempList.IsNullOrEmpty() ? null : tempList[0];
        }

        public void Dispose()
        {

        }
    }
}
