using AppDto;
using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Text;

public partial interface IEquipmentTipsController
{
    void UpdateView(EquipmentDto equipmentDto, EquipmentEmbedCellVo embedVo, EquipmentDto compareEquipmentDto = null);
}

public class EquipmentTipsController : BaseTipsController, IEquipmentTipsController
{
    bool isShowEffect = false;
    public void UpdateView(EquipmentDto equipmentDto,EquipmentEmbedCellVo embedVo, EquipmentDto compareEquipmentDto = null)
    {
        if (equipmentDto == null || equipmentDto.equip==null || equipmentDto.equip as AppItem == null)
        {
            Close();
            return;
        }     

        var generalDto = equipmentDto.equip;
        var appDto = equipmentDto.equip as AppItem;

        int lv = appDto as Equipment == null ? 0 : (appDto as Equipment).grade;
        bool isBind = equipmentDto.circulationType == (int)RealItem.CirculationType.Bind;
        var titleCtrl = SetTitle(generalDto.icon, equipmentDto.property.quality, generalDto.name, (int)AppItem.ItemTypeEnum.Equipment, lv, isBind: isBind);
        if(EquipmentMainDataMgr.DataMgr.IsEquipmentEquip(equipmentDto))
        {
            titleCtrl.SetTitleSign("已装备", TipsTitleViewController.TitleSign.red);
        }
        else
            titleCtrl.HideTitleSign();
        SetLineView(false);
        //战斗力
        //SetLabelView(("战斗力"+equipmentDto.property.power.ToString()).WrapColor(ColorConstantV3.Color_Yellow_Str));
        //属性行
        SetPropsExtro(equipmentDto);
       // SetLineView(true);
        SetEquipmentGroup(equipmentDto.property.groupId);
        if(EquipmentMainDataMgr.DataMgr.IsEquipmentEquip(equipmentDto))
        {
            //宝石属性
            SetEmbed(embedVo);
        }
       
        //纹章属性
        SetMedallion(equipmentDto);
        //描述
        SetLabelView(appDto.description);
        //SetLabelView(time.ToString());
        
        //对比tips
        SetCompareToOnWear(equipmentDto, embedVo,compareEquipmentDto);
    }

    delegate void deleAddProperty(List<CharacterPropertyDto> list,string color= ColorConstantV3.Color_White_Str);
    delegate void deleSetLineView(bool isLine=true);
    delegate void deleSetLabAndSprView(string str, List<string> list, List<int> qualityList=null);
    delegate void deleSetLabelView(string str);

    private void SetPropsExtro(EquipmentDto equipmentDto, bool isCompare=false)
    {
        deleAddProperty AddPropertyFunc = new deleAddProperty(AddProperty);
        deleSetLineView SetLineFunc = new deleSetLineView(SetLineView);
        if (isCompare)
        {
            AddPropertyFunc = new deleAddProperty(AddCompareProperty);
            SetLineFunc = new deleSetLineView(SetCompareLineView);
        } 

        var equipExtraDto = equipmentDto.property.currentProperty;
        AddPropertyFunc(equipExtraDto.baseProps);
        AddPropertyFunc(equipExtraDto.secondProps, ColorConstantV3.Color_Blue_Str);
        SetLineFunc();
        AddPropertyFunc(equipExtraDto.extraProps);
        SetEquipmentEffect(equipmentDto.property.effectId,isCompare);
        if (equipExtraDto.extraProps.Count > 0 || equipmentDto.property.effectId != 0)
            SetLineFunc();
    }

    private void SetEmbed(EquipmentEmbedCellVo embedVo, bool isCompare=false)
    {
        deleAddProperty AddPropertyFunc = new deleAddProperty(AddProperty);
        deleSetLabAndSprView SetLabAndSprFunc = new deleSetLabAndSprView(SetLabAndSprView);
        if (isCompare)
        {
            AddPropertyFunc = new deleAddProperty(AddCompareProperty);
            SetLabAndSprFunc = new deleSetLabAndSprView(SetCompareLabAndSprView);
        }

        if (embedVo != null && embedVo.embedCount > 0)
        {
            List<string> embedIconList = new List<string>();
            List<int> embedQualityList = new List<int>();
            List<string> embedPropertList = new List<string>();
            embedVo.EmbedHoleVoList.ForEachI((x, i) =>
            {
                if (x.embedid == -1)
                    return;
                var item = DataCache.getDtoByCls<GeneralItem>(x.embedid);
                embedIconList.Add(item.icon);
                if (item as AppItem != null)
                    embedQualityList.Add((item as AppItem).quality);
            });

            SetLabAndSprFunc("宝石：", embedIconList);
            AddPropertyFunc(embedVo.TotalProperty.ToList(),ColorConstantV3.Color_Blue_Str);
        }
    }

    private void SetMedallion(EquipmentDto equipmentDto, bool isCompare=false)
    {
        deleSetLineView SetLineFunc = new deleSetLineView(SetLineView);
        deleSetLabAndSprView SetLabAndSprFunc = new deleSetLabAndSprView(SetLabAndSprView);
        deleSetLabelView SetLabelFunc = new deleSetLabelView(SetLabelView);
        if (isCompare)
        {
            SetLineFunc = new deleSetLineView(SetCompareLineView);
            SetLabAndSprFunc = new deleSetLabAndSprView(SetCompareLabAndSprView);
            SetLabelFunc = new deleSetLabelView(SetCompareLabelView);
        }

        if (equipmentDto.property.medallion != null && !equipmentDto.property.medallion.engraves.IsNullOrEmpty())
        {
            List<EngraveDto> engravesList = equipmentDto.property.medallion.engraves;
            List<string> effectStrList = new List<string>();
            List<string> iconNameStrList = new List<string>();
            List<int> QualityList = new List<int>();
            float propsTimes = 1.0f;
            Dictionary<int, float> idToEffect = new Dictionary<int, float>();
            var attrStr = new StringBuilder();

            if (!engravesList.IsNullOrEmpty())
            {
                engravesList.ForEach(ItemDto =>
                {
                    var localData = ItemHelper.GetGeneralItemByItemId(ItemDto.itemId);
                    var propsParam = (localData as Props).propsParam as PropsParam_3;
                    //强化铭刻符提升属性总倍数
                    if (propsParam.type == (int)PropsParam_3.EngraveType.STRENGTHEN)
                    {
                        propsTimes *= ItemDto.effect;
                    }
                    else if (propsParam.type == (int)PropsParam_3.EngraveType.ORDINARY) //相同属性叠加
                    {
                        if (idToEffect.ContainsKey(ItemDto.itemId))
                            idToEffect[ItemDto.itemId] = idToEffect[ItemDto.itemId] + ItemDto.effect;
                        else
                            idToEffect.Add(ItemDto.itemId, ItemDto.effect);
                    }

                    //Icon
                    iconNameStrList.Add(localData.icon);
                    if (localData as AppItem != null)
                        QualityList.Add((localData as AppItem).quality);
                });

                SetLabAndSprFunc("纹章", iconNameStrList, QualityList);

                idToEffect.ForEachI((item, index) =>
                {
                    var localData = ItemHelper.GetGeneralItemByItemId(item.Key);
                    var propsParam = (localData as Props).propsParam as PropsParam_3;
                    attrStr.Append(DataCache.getDtoByCls<CharacterAbility>(propsParam.cpId).name);
                    attrStr.Append("+");
                    if(index == idToEffect.Count - 1)
                        attrStr.Append((item.Value * propsTimes).ToString());
                    else
                        attrStr.AppendLine((item.Value * propsTimes).ToString());
                });

                SetLabelFunc(attrStr.ToString().WrapColor(ColorConstantV3.Color_Blue_Str));
                SetLineFunc();
            }
        }
    }
    private void SetEquipmentGroup(int groupId, bool isCompare = false)
    {
        if (groupId == 0)
            return;
        GameObject go = null;
        if (!isCompare)
            go = View.Table_UITable.gameObject;
        else
            go = View.ComTable_UITable.gameObject;
        var ctrl = AddChild<TipsEquipmentGroupController, TipsEquipmentGroup>(go, TipsEquipmentGroup.NAME);
        ctrl.UpdateView(groupId);
    }
    private void SetCompareToOnWear(EquipmentDto equipDto, EquipmentEmbedCellVo embedVo,EquipmentDto compareDto)
    {
        //var wearEquipDto = EquipmentMainDataMgr.DataMgr.GetCurPv_EquipmentVo((Equipment.PartType)equipmentDto.partType);
        ////判断是不是点击身上的装备
        //if (equipmentDto == null || wearEquipDto == null || wearEquipDto.equipmentDto==null || equipmentDto.equip==null
        //    || equipmentDto.equip as AppItem==null ||equipmentDto.equipUid == wearEquipDto.equipmentDto.equipUid)
        //    return;
        if (compareDto == null)
        {
            SetCompareActive(false);
            return;
        }
        SetCompareActive(true);

        //SetCompareIsShow(true);
        //equipmentDto = wearEquipDto.equipmentDto;
        //var embedVo = wearEquipDto.embedVo;
        var generalDto = compareDto.equip;
        var appDto = compareDto.equip as AppItem;
        int lv = appDto as Equipment == null ? 0 : (appDto as Equipment).grade;
        bool isBind = compareDto.circulationType == (int)RealItem.CirculationType.Bind;
        var titleCtrl = SetCompareTitle(generalDto.icon, compareDto.property.quality, generalDto.name, appDto.itemType,lv,isBind: isBind);
        if (equipDto.property.power > compareDto.property.power)
        {
            titleCtrl.SetTitleSign("推荐", TipsTitleViewController.TitleSign.gerrn);
        }
        else if (EquipmentMainDataMgr.DataMgr.IsEquipmentEquip(compareDto))
        {
            titleCtrl.SetTitleSign("已装备", TipsTitleViewController.TitleSign.red);
        }
        else
            titleCtrl.HideTitleSign();
        SetCompareLineView(false);
        //战斗力
       // SetCompareLabelView("战斗力" + compareDto.property.power.ToString());
        //属性行
        SetPropsExtro(compareDto, true);
        //SetCompareLineView(true);
        SetEquipmentGroup(compareDto.property.groupId,true);
        if(EquipmentMainDataMgr.DataMgr.IsEquipmentEquip(compareDto))
        {
            //宝石属性
            SetEmbed(embedVo, true);
        }
        
        //纹章属性
        SetMedallion(compareDto, true);
        //描述
        SetCompareLabelView(appDto.description);
    }
    
    private void SetEquipmentEffect(int effectId,bool isCompare = false)
    {
        if (effectId == 0)
            return;
        GameObject go = null;
        if (!isCompare)
            go = View.Table_UITable.gameObject;
        else
            go = View.ComTable_UITable.gameObject;

        var ctrl = AddChild<TipsEquipmentEffectItemController, TipsEquipmentEffectItem>(go, TipsEquipmentEffectItem.NAME);
        ctrl.UpdateView(effectId);
        ctrl.OnBtn_UIButtonClick.Subscribe(_ => {
            //isShowEffect = !isShowEffect;
            ctrl.SetDesActive();
            var table = go.GetComponent<UITable>();
            if (table != null)
                table.Reposition();
        });
    }
}