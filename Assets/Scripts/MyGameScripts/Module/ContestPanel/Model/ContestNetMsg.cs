// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : DM-PC092
// Created  : 12/17/2017 2:57:47 PM
// **********************************************************************

using System;
using System.Collections.Generic;
using AppDto;
using AppServices;

public sealed partial class ContestDataMgr
{
    public static class ContestNetMsg
    {
        public static void BackpackApply(int index,int count,string opt = "")
        {
            GameUtil.GeneralReq(Services.Backpack_Apply(index,count,opt),e =>
            {
                ProxyContest.Close();
            });
        }
    }
}
