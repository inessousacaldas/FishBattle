// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  QuartzMagicSliderController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using System.Collections.Generic;
using AppDto;

public partial class QuartzMagicSliderController
{
    private const float InfoMax = 7f;  //有且只有7个属性
    private static List<string> _sliderColor = new List<string> {"Pb1_Fg","Pb1_Fg_3","Pb1_Fg_4","Pb1_Fg_2"}; 
    public void UpdateView(QuartzPropertyListDto dataList, int idx)
    {
        _view.QuartzMagicSlider_UISlider.value = 0.15f; //策划要求等长
        if (dataList.quartzPropertyDtos.Count == 0)
        {
            UpdateView();
            return;
        }

        for (int i = 0; i < _view.NumGrid_UIGrid.transform.childCount; i++)
        {
            var lb = _view.NumGrid_UIGrid.GetChild(i).GetComponent<UILabel>();
            var data = dataList.quartzPropertyDtos.Find(d => d.elementId == i + 1);
            lb.text = data == null
                ? "0"
                : data.count.ToString();
        }
        _view.QuartzMagicSlider_UISprite.spriteName = _sliderColor[idx];
    }

    public void UpdateView()
    {
        //gameObject.SetActive(false);
        _view.QuartzMagicSlider_UISlider.value = 0.15f;//策划要求等长

        for (int i = 0; i < _view.NumGrid_UIGrid.transform.childCount; i++)
        {
            var lb = _view.NumGrid_UIGrid.GetChild(i).GetComponent<UILabel>();
            lb.text = "0";
        }
    }
}
