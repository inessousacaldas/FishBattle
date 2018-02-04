// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  ResetAttrItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using AppDto;
using System;

public class ResetAttrItemVo
{
    //属性值
    public CharacterPropertyDto property;
    //属性范围
    public EquipmentPropertyRange eq_range;
    //属性对比的差值
    public float value_offset;
}
public partial class ResetAttrItemController
{
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
 
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {

    }

    protected override void OnDispose()
    {

    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }
    public void UpdateView(CharacterPropertyDto property,EquipmentPropertyRange eq_ranges)
    {
        var cb = DataCache.getDtoByCls<CharacterAbility>(property.propId);
        View.AttrLbl_UILabel.text = string.Format("{0}:{1}", cb.name, property.propValue);

        //Todo:设置滑动条
    }

    public void UpdateView(ResetAttrItemVo vo)
    {
        var cb = DataCache.getDtoByCls<CharacterAbility>(vo.property.propId);
        View.AttrLbl_UILabel.text = string.Format("{0}", cb.name);
        //测试公式~
        float fillValue = 0.3f+ (1.0f - 0.3f) * ((int)vo.property.propValue - vo.eq_range.minValue) / (vo.eq_range.maxValue - vo.eq_range.minValue);

        View.Slider_UISlider.value = fillValue;

        //设置对比
        string color = "";
        if(vo.value_offset > 0)
        {
            color = "00ff00";
            View.Value_UILabel.text = string.Format("{0}#up1{1}",(int)vo.property.propValue,vo.value_offset.ToString().WrapColor(color));
        }
        else if(vo.value_offset < 0)
        {
            color = "FF0000";
            View.Value_UILabel.text = string.Format("{0}#down1{1}", (int)vo.property.propValue, vo.value_offset.ToString().WrapColor(color));
        }
        else if(vo.value_offset == 0)
        {
            View.Value_UILabel.text = ((int)vo.property.propValue).ToString();
        }
    }
}
