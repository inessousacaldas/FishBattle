// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : xjd
// Created  : 9/26/2017 5:45:14 PM
// **********************************************************************

using System;
using System.Collections.Generic;
using AppDto;
using AppServices;

public sealed partial class PlayerPropertyDataMgr
{
    public static class PlayerPropertyNetMsg
    {
        public static void ReqPlayerPropsInfo()
        {
            GameUtil.GeneralReq(Services.Charactor_Property(), resp =>
            {
                var dto = resp as CharactorPropertyUpdateDto;
                DataMgr._data.UpdateData(dto.properties);
                FireData();
            });
        }

        public static void ReqPlayerChangeName(string name)
        {
            GameUtil.GeneralReq(Services.Player_Rename(name), resp =>
            {
                TipManager.AddTip("改名成功！");
                DataMgr._data.ResultName = name;
                ModelManager.Player.SetPlayerName(name);
                FireData();
            });
        }
    }
}
