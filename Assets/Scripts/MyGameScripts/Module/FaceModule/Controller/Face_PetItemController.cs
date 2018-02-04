// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  Face_PetItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using AppDto;

public partial class Face_PetItemController
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
    public void UpdateView(CrewShortDto dto, Crew crewConfig)
    {
       
        UIHelper.SetPetIcon(View.Icon_UISprite, crewConfig.icon);
        View.Name_UILabel.text = crewConfig.name;
        View.Lv_UILabel.text = string.Format("等级: {0}", dto.grade);
    }
}
