// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  PropertyTipItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using AppDto;
using System;
using System.Collections.Generic;
using System.Text;
using MyGameScripts.Gameplay.Player;

public partial class PropertyTipItemController
{

    public void Init(PlayerPropertyTipType type, int propertyID, bool isPlayer)
    {
        View.TitleLabel_UILabel.text = DataCache.getDtoByCls<CharacterAbility>(propertyID)==null ? 
            "" : DataCache.getDtoByCls<CharacterAbility>(propertyID).name;

        //主属性 褐色
        if (type == PlayerPropertyTipType.BaseType && propertyID == DataCache.getDtoByCls<Faction>(ModelManager.Player.FactionID).mainProperty)
        {
            View.TitleLabel_UILabel.text = View.TitleLabel_UILabel.text.WrapColor(ColorConstantV3.Color_Brown);
        }

        if (type == PlayerPropertyTipType.BaseType)
        {
            View.DesLabel_UILabel.text = ModelManager.IPlayer.GetPropertyDesc(propertyID, isPlayer);
        }
        else if (type == PlayerPropertyTipType.FightType)
        {
            View.DesLabel_UILabel.text = DataCache.getDtoByCls<CharacterAbility>(propertyID)==null?
                "" : DataCache.getDtoByCls<CharacterAbility>(propertyID).desc;
        }
    }

    public void InitPersonality(int personality)
    {
        var person = DataCache.getDtoByCls<CrewPersonality>(personality);
        StringBuilder sb = new StringBuilder();
        _view.TitleLabel_UILabel.text = "性格";
        person.desc.Split(';').ForEach(d =>
        {
            sb.AppendLine(d);
        });
        _view.DesLabel_UILabel.text = sb.ToString();
    }

    
}
