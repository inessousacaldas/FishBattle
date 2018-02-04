// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : Zijian
// Created  : 8/29/2017 4:01:34 PM
// **********************************************************************

using AppDto;
using System.Collections.Generic;
using UniRx;
using System;

/// <summary>
/// 装备属性的范围
/// </summary>
public class EquipmentPropertyRange
{
    public int abilityId;
    public float maxValue;
    public float minValue;
    public EquipmentPropertyRange() { }
    public EquipmentPropertyRange(int id,float maxValue,float minValue) {
        this.abilityId = id;
        this.maxValue = maxValue;
        this.minValue = minValue;
    }
}




public sealed partial class EquipmentMainDataMgr
{
    /// <summary>
    /// 装备所在的位置~
    /// </summary>
    public enum EquipmentHoldTab
    {
        //已装备
        Equip,
        //背包
        Bag,
    }

    #region Event
    Subject<EquipmentDto> _onEquipmentWearStream = new Subject<EquipmentDto>();
    Subject<EquipmentDto> _onEquipmentTakeOffStream = new Subject<EquipmentDto>();


    //当有装备穿上
    public UniRx.IObservable<EquipmentDto> OnEquipmentWear { get { return _onEquipmentWearStream; } }
    //当有装备卸下
    public UniRx.IObservable<EquipmentDto> OnEquipmentTakeOff { get { return _onEquipmentTakeOffStream; } }
    
    #endregion
    /// <summary>
    /// 获取装备列表
    /// </summary>
    /// <param name="tab"></param>
    /// <returns></returns>
    public List<EquipmentDto> GetEquipmentDtoList(EquipmentHoldTab tab)
    {
        List<EquipmentDto> res = new List<EquipmentDto>();
        switch (tab)
        {
            case EquipmentHoldTab.Bag:
                //equipmentList_bag.Clear();
                var backList = BackpackDataMgr.DataMgr.GetEquipmentItems();
                backList.ForEach(x =>
                {
                    if (x.extra == null || !(x.extra is EquipmentExtraDto))
                        return;
                    res.Add(MakeEquipmentDto(x));
                });
                //按照战力从高往底排序
                res.Sort((a, b) =>
                {
                    return a.property.power - b.property.power;
                });
                break;
            case EquipmentHoldTab.Equip:
                //equipmentList_equip.Clear();
                var weapon = _data.GetCurEquipmentByType(Equipment.PartType.Weapon);
                if (weapon != null)
                    res.Add(weapon);
                var clothes = _data.GetCurEquipmentByType(Equipment.PartType.Clothes);
                if (clothes != null)
                    res.Add(clothes);
                var glove = _data.GetCurEquipmentByType(Equipment.PartType.Glove);
                if (glove != null)
                    res.Add(glove);
                var shoe = _data.GetCurEquipmentByType(Equipment.PartType.Shoe);
                if (shoe != null)
                    res.Add(shoe);
                var accone = _data.GetCurEquipmentByType(Equipment.PartType.AccOne);
                if (accone != null)
                    res.Add(accone);
                var acctwo = _data.GetCurEquipmentByType(Equipment.PartType.AccTwo);
                if (acctwo != null)
                    res.Add(acctwo);
                break;
        }
        return res;
    }

    public EquipmentDto MakeEquipmentDto(BagItemDto bagItemDto)
    {
        if (bagItemDto.extra == null || !(bagItemDto.extra is EquipmentExtraDto))
            return null;
        var tempdto = new EquipmentDto();
        tempdto.property = (EquipmentExtraDto)bagItemDto.extra;
        tempdto.equipId = bagItemDto.itemId;
        tempdto.equipUid = bagItemDto.uniqueId;
        tempdto.circulationType = bagItemDto.circulationType;
        return tempdto;
    }


    /// <summary>
    /// 解析装备打造表的SmithItem字符串
    /// </summary>
    /// <param name="eq"></param>
    /// <returns></returns>
    public IEnumerable<SmithItemVo> GetEquipmentSmithItems(Equipment eq)

    {
        List<SmithItemVo> res = new List<SmithItemVo>();
        if (eq == null)
            return res;
        var str_1 = eq.smithItem.Split(',');
        foreach (var str_2 in str_1)
        {
            var str_3 = str_2.Split(':');
            int id = StringHelper.ToInt(str_3[0]);
            int count = StringHelper.ToInt(str_3[1]);
            var item = DataCache.getDtoByCls<GeneralItem>(id) as Props;

            int ownerCount = BackpackDataMgr.DataMgr.GetItemCountByItemID(id);
            SmithItemVo vo = new SmithItemVo() { props = item, needCount = count, currentCount = ownerCount };
            res.Add(vo);
        }
        return res;
    }

    /// <summary>
    /// 获取装备导表的信息
    /// </summary>
    /// <returns></returns>
    public IEnumerable<Equipment> GetEquipmentList()
    {
        return _data.eq_List;
    }


    /// <summary>
    /// 获取该部位的已装备
    /// </summary>
    /// <param name="part">装备的部位</param>
    /// <returns></returns>
    public int GetCurEquipIDByType(Equipment.PartType part)
    {
        var dto = _data.GetCurEquipmentByType(part);
        return dto == null ? 0 : dto.equipId;
    }
    public EquipmentDto GetSameEquipmentByPart(EquipmentDto _equipmentDto)
    {
        if (_equipmentDto == null) return null;
        var partType = (Equipment.PartType)(_equipmentDto.equip as Equipment).partType[0];
        if (_equipmentDto.partType > 0)
        {
            partType = (Equipment.PartType)_equipmentDto.partType;
        }
        return _data.GetCurEquipmentByType(partType);
    }

    public bool IsEquipmentEquip(EquipmentDto equipmentDto)
    {
        if (equipmentDto == null)
            return false;
        var res = GetSameEquipmentByPart(equipmentDto);
        if(res != null)  
            return res .equipUid== equipmentDto.equipUid;
        return false;
    }
    /// <summary>
    /// 同样获取同样该部位的装备，不同于在饰品的筛选处理,取战斗力更小的
    /// </summary>
    /// <param name="part">装备的部位</param>
    /// <returns></returns>
    public Equipment.PartType GetAccEmptyHole()
    {
        //如果是饰品，则取饰品战力高的一个
       
        var eq_1 = _data.GetCurEquipmentByType(Equipment.PartType.AccOne);
        var eq_2 = _data.GetCurEquipmentByType(Equipment.PartType.AccTwo);

        var eq_power_1 = eq_1 == null ? -1 : eq_1.property.power;
        var eq_power_2 = eq_2 == null ? -1 : eq_2.property.power;

        //要求选取战斗力更小的
        if (eq_power_1 < eq_power_2)
            return Equipment.PartType.AccOne;
        else
            return Equipment.PartType.AccTwo;
    }
    

    /// <summary>
    /// 获取装备属性的范围
    /// <param name="equipId">装备ID</param>
    /// <param name="quality">质量</param>
    /// </summary>
    public List<EquipmentPropertyRange> GetEquipmentPropertyRange(int equipId,int quality)
    {
        List<EquipmentPropertyRange> res = new List<EquipmentPropertyRange>();

        var equipment = DataCache.getDtoByCls<GeneralItem>(equipId) as Equipment;
        var eqType = DataCache.getDtoByCls<EquipmentType>(equipment.equipType);
        
        var attr_maths = eqType.math.Split(',');
        var eqQuality = DataCache.getDtoByCls<EquipmentQuality>(quality);
        foreach (var attr_math in attr_maths)
        {
            var raw_str = attr_math.Split(':');
            int attr_Id = StringHelper.ToInt(raw_str[0]);

            string math = raw_str[1]; //公式
            float min = (float)eqQuality.range[0] / 100.0f;
            float max = (float)eqQuality.range[1] / 100.0f;
            var minValue = ExpressionManager.PreviewEquipmentSmithAttr(eqType.id, attr_Id, math, equipment.grade, min);
            var maxValue = ExpressionManager.PreviewEquipmentSmithAttr(eqType.id, attr_Id, math, equipment.grade, max);
            var cb = DataCache.getDtoByCls<CharacterAbility>(attr_Id);

            EquipmentPropertyRange eq_range = new EquipmentPropertyRange(attr_Id, maxValue, minValue);
            res.Add(eq_range);
        }
        return res;
    }
    /// <summary>
    /// 获取范围，只要一级属性
    /// </summary>
    /// <param name="equipId"></param>
    /// <param name="quality"></param>
    /// <returns></returns>
    public List<EquipmentPropertyRange> GetEquipmentPropertyRange_BaseProperty(int equipId, int quality)
    {
        var res = GetEquipmentPropertyRange(equipId, quality);
        var equipment = DataCache.getDtoByCls<GeneralItem>(equipId) as Equipment;
        var eqPartConfig = DataCache.getDtoByCls<EquipmentPart>(equipment.partType[0]);
        var trueRes = res.Filter(x => eqPartConfig.baseProperty.Contains(x.abilityId)).ToList();
        return trueRes;
    }

    /// <summary>
    /// 获取装备的 附加属性范围
    /// </summary>
    /// <returns></returns>
    public EquipmentPropertyRange GetEquipmentExtraProperty(int attrId,int quality, int lv, bool isDiscount=false)
    {
        var eqQuality = DataCache.getDtoByCls<EquipmentQuality>(quality);
        EquipmentExtraProperty eqExtraProperty = DataCache.getDtoByCls<EquipmentExtraProperty>(attrId);
        string math = eqExtraProperty.math;
        float min = (float)eqQuality.range[0] / 100.0f;
        float max = (float)eqQuality.range[1] / 100.0f;
        var minValue = ExpressionManager.PreviewEquipmentExtraAttr(attrId, math, lv, min);
        minValue = isDiscount ? minValue * 0.6f : minValue;
        var maxValue = ExpressionManager.PreviewEquipmentExtraAttr(attrId, math, lv, max);
        maxValue = isDiscount ? (float)maxValue * 0.6f : (float)maxValue;
        EquipmentPropertyRange eq_range = new EquipmentPropertyRange(attrId, maxValue, minValue);
        return eq_range;
    }

    public List<Equipment.PartType> GetEquipmentGroup(int groupId)
    {
        List<Equipment.PartType> res = new List<Equipment.PartType>();
        var groupConfig = DataCache.getDtoByCls<EquipmentGroup>(groupId);
        List<Equipment> equipmentGroup = new List<Equipment>();
        if(groupConfig != null)
            groupConfig.equipIds.ForEach(equipId =>
            {
                var equipment = DataCache.getDtoByCls<GeneralItem>(equipId) as Equipment;
                equipmentGroup.Add(equipment);
            });
        var weapon = _data.GetCurEquipmentByType(Equipment.PartType.Weapon);
        if (CheckEquipmentGroup(groupId,weapon, equipmentGroup))
            res.Add((Equipment.PartType)weapon.partType);
        var clothes = _data.GetCurEquipmentByType(Equipment.PartType.Clothes);
        if (CheckEquipmentGroup(groupId, clothes, equipmentGroup))
            res.Add((Equipment.PartType)clothes.partType);
        var glove = _data.GetCurEquipmentByType(Equipment.PartType.Glove);
        if (CheckEquipmentGroup(groupId, glove, equipmentGroup))
            res.Add((Equipment.PartType)glove.partType);
        var shoe = _data.GetCurEquipmentByType(Equipment.PartType.Shoe);
        if (CheckEquipmentGroup(groupId, shoe, equipmentGroup))
            res.Add((Equipment.PartType)shoe.partType);
        var accone = _data.GetCurEquipmentByType(Equipment.PartType.AccOne);
        if (CheckEquipmentGroup(groupId, accone, equipmentGroup))
            res.Add((Equipment.PartType)accone.partType);
        var acctwo = _data.GetCurEquipmentByType(Equipment.PartType.AccTwo);
        if (CheckEquipmentGroup(groupId, acctwo, equipmentGroup))
            res.Add((Equipment.PartType)acctwo.partType);

        return res;
    }
    
    private bool CheckEquipmentGroup(int groupId, EquipmentDto equipmentDto ,List<Equipment> equipmentGroupList)
    {
        if (equipmentDto == null) return false;
        if(equipmentDto != null && equipmentGroupList.Find(x => x.id == equipmentDto.equip.id) != null && equipmentDto.property.groupId == groupId)
            return true;
        return false;
    }
    /// <summary>
    /// 获取某个部位的装备完整信息 包含宝石，套装等信息
    /// </summary>
    /// <param name="isGetMaxAcc">饰品部分是否取战力更高的</param>
    /// <returns></returns>
    public Pv_EquipmentVo GetCurPv_EquipmentVo(Equipment.PartType part,bool isGetMaxAcc = false)
    {
        
        var equipmentDto = _data.GetCurEquipmentByType(part);//获取装备信息
        var embedCellVo = GetEmbedInfoByPart(part);//获取宝石信息
        

        if(isGetMaxAcc && (part == Equipment.PartType.AccOne) || (part == Equipment.PartType.AccTwo))
        {
            equipmentDto = _data.GetCurEquipmentByType(part);
        }
        if (equipmentDto == null)
            return null;

        Pv_EquipmentVo vo = new Pv_EquipmentVo(equipmentDto, embedCellVo);
        return vo;
    }

    /// <summary>
    /// 展示带参照物/不带参照物的装备提示~用在打造
    /// </summary>
    /// <param name="refDto"></param>
    public void ShowPv_Equipment(EquipmentDto refDto, bool isShowCom = true)
    {
        if (refDto == null)
            return;

        ////1.获取当前该部位上正在装备中的装备
        Pv_EquipmentVo currentEquipVo = null;

        
        var equip = DataCache.getDtoByCls<GeneralItem>(refDto.equipId) as Equipment;
        currentEquipVo = DataMgr.GetCurPv_EquipmentVo((Equipment.PartType)equip.partType[0]);
        if (refDto.partType != 0)
        {
            currentEquipVo = DataMgr.GetCurPv_EquipmentVo((Equipment.PartType)refDto.partType);
        }

        var rightBtn = PreviewShowHelper.GetPvBtn(PvBtnType.Equip);
        if (currentEquipVo != null && currentEquipVo.equipmentDto.equipUid == refDto.equipUid)
        {
            rightBtn = PreviewShowHelper.GetPvBtn(PvBtnType.TakeOff);
        }
        var leftBtn = new List<Pv_ButtonVo>()
        {
            PreviewShowHelper.GetPvBtn(PvBtnType.Split)
        };
       
        Pv_EquipmentVo refEquipVo = new Pv_EquipmentVo(refDto, null, leftBtn,rightBtn);
        if(currentEquipVo != null)
            refEquipVo.powerChange = currentEquipVo.equipmentDto.property.power - refEquipVo.equipmentDto.property.power;
        ////2.打开展示的界面
        if (!isShowCom)
            currentEquipVo = null;
        var ctrl = PreviewShowHelper.ShowEquipment(currentEquipVo, refEquipVo);
        var ctrlIndex = ctrl.ContainerCtrls.Count > 1 ? 1 : 0;
        ctrl.ContainerCtrls[ctrlIndex].OnButtonClickStream.Subscribe(i => {
            switch((PvBtnType)i)
            {
                //分解
                case PvBtnType.Split:
                    break;
                case PvBtnType.TakeOff:
                    EquipmentMainNetMsg.ReqTakeOffEquipment(refDto);
                    break;
                case PvBtnType.Equip:
                    EquipmentMainNetMsg.ReqEquip_Wear(refDto);
                    break;
            }
            PreviewShowHelper.Close();
        });
    }
    // 初始化
    private void LateInit()
    {
        //获取玩家个人的穿戴装备信息
        EquipmentMainNetMsg.ReqEquipmentInfo();
        //获取玩家部分宝石信息
        EquipmentMainNetMsg.ReqEmbedInfo();

        //
        _disposable.Add(NotifyListenerRegister.RegistListener<EmbedPhaseNotify>(OnEmbedPhase));
        _disposable.Add(NotifyListenerRegister.RegistListener<BagItemNotify>(OnBagItemNotify));

        GameEventCenter.AddListener(GameEvent.BACK_PACK_ITEM_CHANGE, OnBackPackItemChange);
        DataMgr._data.SmithData.UpdateCurSmithItems();
        DataMgr._data.ResetData.UpdateCurSmith();
        //_disposable.Add(BackpackDataMgr)
    }

    private void OnBackPackItemChange()
    {
        DataMgr._data.SmithData.UpdateCurSmithItems();
        DataMgr._data.ResetData.UpdateCurSmith();
    }
    #region NoityNetUpdate
    private void OnEmbedPhase(EmbedPhaseNotify phase)
    {
        _data.EmbedData.UpdateEmbedPhase(phase.curephaseId);
    }

    private void OnBagItemNotify(BagItemNotify notify)
    {
        DataMgr._data.SmithData.UpdateCurSmithItems();
        FireData();
    }
    #endregion
    public void OnDispose(){
        GameEventCenter.RemoveListener(GameEvent.BACK_PACK_ITEM_CHANGE, OnBackPackItemChange);
    }

    public int GetWeaponEffectIndex(int weaponEffId)
    {
//        throw new NotImplementedException();
        return 1;
    }
}

