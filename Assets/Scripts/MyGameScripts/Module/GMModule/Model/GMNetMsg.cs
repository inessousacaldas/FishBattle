// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : DM-PC092
// Created  : 7/29/2017 10:26:04 AM
// **********************************************************************

using System;
using System.Collections.Generic;
using AppDto;
using AppServices;

public sealed partial class GMDataMgr
{
    public static class GMNetMsg
    {
        public static void OnGMExecute(GMDataCodeVO vo, string cmd)
        {
            GameUtil.GeneralReq(Services.Gm_Execute(cmd),resp=> {
                if(vo != null)
                {
                    DataMgr._data.CacheUserCode(vo,cmd);
                    FireData();
                }else
                {
                    if(cmd.ToLower().Contains("call"))
                    {
                        DataMgr._data.CacheUserCode(null,cmd);
                    }
                }
                TipManager.AddTip(string.Format("成功调用指令，{1}",cmd));
            });
        }

    }
}
