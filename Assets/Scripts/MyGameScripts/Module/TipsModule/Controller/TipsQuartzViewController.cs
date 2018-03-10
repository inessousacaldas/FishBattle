// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  TipsQuartzViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using AppDto;
using System.Collections.Generic;

public partial class TipsQuartzViewController
{
    private const int count = 3;
    private List<UISprite> _spriteList = new List<UISprite>();
    private List<UILabel> _labelList = new List<UILabel>();

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        _spriteList.Add(View.Sprite_1_UISprite);
        _spriteList.Add(View.Sprite_2_UISprite);
        _spriteList.Add(View.Sprite_3_UISprite);
        _labelList.Add(View.Label_1_UILabel);
        _labelList.Add(View.Label_2_UILabel);
        _labelList.Add(View.Label_3_UILabel);
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

    public void UpdateView(QuartzExtraDto dto)
    {
        var tempDtos = new List<QuartzPropertyDto>();
        //最多三个限制
        if (dto.quartzProperties.Count > count)
        {
            for(int i=0;i<count;i++)
            {
                tempDtos.Add(dto.quartzProperties[i]);
            }
            dto.quartzProperties = tempDtos;
        }

        dto.quartzProperties.ForEachI((item, index) =>
        {
            _spriteList[index].gameObject.SetActive(true);
            _labelList[index].gameObject.SetActive(true);
            _spriteList[index].spriteName = string.Format("ect_quartz_{0}", item.elementId);
            _labelList[index].text = string.Format("X{0}", item.count);
        });
    }
}
