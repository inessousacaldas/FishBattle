// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : Cilu
// Created  : 9/8/2017 9:50:40 AM
// **********************************************************************

using AppDto;
using System.Collections.Generic;
using System;

/// <summary>
/// 装备纹章界面数据
/// </summary>

public interface IEquipmentInsetMedallionViewData
{
    //当前选择纹章id
    long SelMedallionId { get; }

    //当前是否打开medallionPanel
    bool isOpenMedallionPanel { set; get; }

    //纹章list
    IEnumerable<BagItemDto> MedallionList { get; }
    //获取对应id纹章的数据
    BagItemDto GetMedallionDataById(long id);
    //当前选择纹章数据
    BagItemDto CurSelMedallionBagDto { get; }
    //装备栏tab
    EquipmentMainDataMgr.EquipmentHoldTab CurTab { get; }
    //当前选择装备
    EquipmentDto CurChoiceEquipment { get; }

    //当前的装备列表
    IEnumerable<EquipmentDto> EquipmentItems { get; }



    //原属性
    EquipmentMedallionContentVo OldAttrContentVo { get; }
    //新属性
    EquipmentMedallionContentVo NewAttrContentVo { get; }
}

public class EquipmentInsetMedallionViewData : IEquipmentInsetMedallionViewData
{
    public bool _isOpenMedallionPanel = false;

    private long _curSelMedallionId = 0;

    private long _curSelEquipmentId = 0;

    private List<BagItemDto> _medallionList = new List<BagItemDto>();

    private EquipmentMedallionContentVo _oldAttrContentVo, _newAttrContentVo;
    public static int MinOpenGrade { private set; get; }

    public long SelMedallionId
    {
        set
        {
            _curSelMedallionId = value;
            UpdateRightAttr();
        }
        get
        {            
            return _curSelMedallionId;
        }
    }

    public bool isOpenMedallionPanel
    {
        set
        {
            _isOpenMedallionPanel = value;
        }
        get
        {
            return _isOpenMedallionPanel;
        }
    }

    public IEnumerable<BagItemDto> MedallionList
    {
        get
        {
            return BackpackDataMgr.DataMgr.GetMedallionItems();
        }
    }

    public BagItemDto GetMedallionDataById(long id)
    {
        return BackpackDataMgr.DataMgr.GetMedallionItems().Find(x => x.uniqueId == id);
    }

    public BagItemDto CurSelMedallionBagDto
    {
        get
        {
            return BackpackDataMgr.DataMgr.GetMedallionItems().Find(x => x.uniqueId == SelMedallionId);
        }
    }

    public void IninData()
    {
        //todo
        //var eq_resetGradeConfig = DataCache.getDtoByCls<StaticConfig>(AppStaticConfigs.EQUIP_RESET_GRADE_LIMIT);
        //MinOpenGrade = StringHelper.ToInt(eq_resetGradeConfig.value);

       
        _oldAttrContentVo = new EquipmentMedallionContentVo("原属性", true, "");
        _newAttrContentVo = new EquipmentMedallionContentVo("新属性", true, "");
        CurTab = EquipmentMainDataMgr.EquipmentHoldTab.Equip;
        UpdateData();
    }

    public void UpdateData()
    {
        _medallionList = BackpackDataMgr.DataMgr.GetMedallionItems().ToList();

        SelMedallionId = _medallionList.ToList().IsNullOrEmpty() ? 0 : _medallionList.ToList()[0].uniqueId;

        UpdateLeftAttr();
        UpdateRightAttr();
    }

    public void UpdateCurMedallion()
    {
        var list = BackpackDataMgr.DataMgr.GetMedallionItems().ToList();

        if (list.IsNullOrEmpty())
            SelMedallionId = 0;
        else if (list.Find(item => item.uniqueId == SelMedallionId) == null)
            SelMedallionId = list.IsNullOrEmpty() ? 0 : list[0].uniqueId;
    }

    public EquipmentMainDataMgr.EquipmentHoldTab CurTab { get; set; }

    public IEnumerable<EquipmentDto> EquipmentItems
    {
        get
        {
            return EquipmentMainDataMgr.DataMgr.GetEquipmentDtoList(CurTab);
        }
    }

    //更新当前标签装备列表
    public void UpdateCurEquipmentItemList()
    {
        var tempList = EquipmentMainDataMgr.DataMgr.GetEquipmentDtoList(CurTab);

        if (tempList.IsNullOrEmpty())
            CurChoiceEquipment = null;
        else if(tempList.Find(item => item.equipUid == _curSelEquipmentId) == null)
            CurChoiceEquipment = tempList.IsNullOrEmpty() ? null : tempList[0];
        else if (tempList.Find(item => item.equipUid == _curSelEquipmentId) != null)
            CurChoiceEquipment = tempList.Find(item => item.equipUid == _curSelEquipmentId);

        _curSelEquipmentId = CurChoiceEquipment == null ? 0 : CurChoiceEquipment.equipUid;
        UpdateLeftAttr();
        UpdateRightAttr();
    }
    private void UpdateLeftAttr()
    {
        MedallionDto medallionDto = null;
        if (CurChoiceEquipment == null)
        {
            _oldAttrContentVo.SetEmpty(true, "请选择一件装备");
        }
        else if (CurChoiceEquipment.property.medallion == null)
        {
            _oldAttrContentVo.SetEmpty(true, "无纹章");
        }
        else
        {
            _oldAttrContentVo.isEmpty = false;
            medallionDto = CurChoiceEquipment.property.medallion;
        }
        
        _oldAttrContentVo.UpdateAttrItemVo(CurChoiceEquipment, medallionDto);
    }
    private void UpdateRightAttr()
    {
        BagItemDto itemData = CurSelMedallionBagDto;
        if (itemData == null || itemData.extra == null)
        {
            _newAttrContentVo.SetEmpty(true, "点击镶嵌开始镶嵌纹章");
            return;
        }
        else if(CurChoiceEquipment == null)
        {
            _newAttrContentVo.SetEmpty(true, "请选择一件装备");
            return;
        }
        else
        {
            _newAttrContentVo.SetEmpty(false, "");
        }
        _newAttrContentVo.UpdateAttrItemVo(CurChoiceEquipment, (itemData.extra) as MedallionDto);
    }
    private void OnCurEquipmentChoiceChange()
    {
        UpdateLeftAttr();
        UpdateRightAttr();
    }

    private EquipmentDto curChoiceEquipment = null;
    public EquipmentDto CurChoiceEquipment
    {
        get
        {
            //curChoiceEquipment = EquipmentMainDataMgr.DataMgr.GetEquipmentDtoList(CurTab).Find(item => item.equipUid == _curSelEquipmentId);
            return curChoiceEquipment;
        }
        set
        {
            var lastValue = curChoiceEquipment;
            curChoiceEquipment = value;
            if(curChoiceEquipment != lastValue)
            {
                OnCurEquipmentChoiceChange();
                if(curChoiceEquipment != null)
                    _curSelEquipmentId = curChoiceEquipment.equipUid;
            }
            
        }
    }

    public EquipmentMedallionContentVo OldAttrContentVo
    {
        get
        {
            return _oldAttrContentVo;
        }
    }

    public EquipmentMedallionContentVo NewAttrContentVo
    {
        get
        {
            return _newAttrContentVo;
        }
    }
}
