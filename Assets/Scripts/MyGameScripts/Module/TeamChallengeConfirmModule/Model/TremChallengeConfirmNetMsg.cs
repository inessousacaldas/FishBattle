// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : DM-PC092
// Created  : 1/22/2018 3:17:47 PM
// **********************************************************************

using System;
using System.Collections.Generic;
using AppDto;
using AppServices;

public sealed partial class TremChallengeConfirmDataMgr
{
    public static class TremChallengeConfirmNetMsg
    {
        public static void Team_PlayerConfirm(bool sure)
        {
            GameUtil.GeneralReq(Services.Team_PlayerConfirm(sure),e =>
            {
                //ProxyContest.Close();
            });
        }
    }
}
