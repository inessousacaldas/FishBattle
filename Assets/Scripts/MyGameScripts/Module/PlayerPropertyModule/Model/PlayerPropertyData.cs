// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : Cilu
// Created  : 8/28/2017 11:18:56 AM
// **********************************************************************

using AppDto;
using System.Collections.Generic;
using MyGameScripts.Gameplay.Player;

public enum PlayerViewTab : int
{
    PropertyView = 0, //属性
    InfoView = 1 //
}

public interface IPlayerPropertyData
{
    List<CharacterPropertyDto> Properties { get; }
    float GetPropertyById(int id);

    PlayerViewTab CurTab { get; }

    string ChangeReadyName { get; set; }

    string ResultName { get; set; }
}

public enum PlayerPropertyTipType
{
    BaseType = 1,
    FightType
}

public sealed partial class PlayerPropertyDataMgr
{
    public sealed partial class PlayerPropertyData:IPlayerPropertyData
    {
        public PlayerViewTab CurTab { get; set; }
        public string ChangeReadyName { get; set; }
        public string ResultName { get; set; }

        public static readonly List<ITabInfo> _TabInfos = new List<ITabInfo>()
        {
            TabInfoData.Create((int)PlayerViewTab.PropertyView,"属性"),
            TabInfoData.Create((int)PlayerViewTab.InfoView,"信息"),
        };

        private List<CharacterPropertyDto> properties = new List<CharacterPropertyDto>();

        public List<CharacterPropertyDto> Properties { get { return properties; } }

        public float GetPropertyById(int id)
        {
            return properties.Find(x => x.propId == id) == null ? 0.0f : properties.Find(x => x.propId == id).propValue;
        }

        public void InitData()
        {
            //基础属性
            GlobalAttr.PANTNER_BASE_ATTRS.ForEach(x =>
            {
                if (ModelManager.Player.GetPropertyDtoById(x) != null)
                    properties.Add(ModelManager.Player.GetPropertyDtoById(x));
            });

            //战斗属性
            GlobalAttr.SECOND_ATTRS_TIPS.ForEach(x =>
            {
                if(ModelManager.Player.GetPropertyDtoById(x) != null)
                    properties.Add(ModelManager.Player.GetPropertyDtoById(x));
            });

            CurTab = PlayerViewTab.PropertyView;
        }

        public void UpdateData(List<CharacterPropertyDto> list)
        {
            list.ForEach(x =>
            {
                if (properties.Find(y => y.propId == x.propId) != null)
                    properties.Find(y => y.propId == x.propId).propValue = x.propValue;
            });

            //playermodel data
            list.ForEach(x =>
            {
                ModelManager.Player.SetPropertyDtoById(x.propId, x.propValue);
            });

            //切换标签
            CurTab = PlayerViewTab.PropertyView;
        }

        public void Dispose()
        {

        }

    }
}
