// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : DM-PC092
// Created  : 1/20/2018 11:55:37 AM
// **********************************************************************

using System;
using System.Collections.Generic;
using AppDto;
using AppServices;

public sealed partial class CopyPanelDataMgr
{
    public static class CopyPanelNetMsg
    {
        public static void EnterCopy(int copyid)
        {
            GameUtil.GeneralReq(Services.Copy_Enter(copyid),e =>
            {
                ProxyCopyPanel.Close();
            });
        }
    }
}
