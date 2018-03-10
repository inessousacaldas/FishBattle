// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : xjd
// Created  : 9/1/2017 2:37:22 PM
// **********************************************************************

using System;
using AppDto;
using AppServices;

public sealed partial class EngraveDataMgr
{
    public static class EngraveNetMsg
    {
        //铭刻
        public static void ReqEngraveRune(long muid, int eiid)
        {
            if(muid == 0)
            {
                TipManager.AddTopTip("请选择纹章");
                return;
            }
            GameUtil.GeneralReq(Services.Engrave_Rune(muid, eiid), resp =>
            {
                DataMgr._data.UpdateData();
                FireData();
                
                TipManager.AddTopTip(string.Format("恭喜您成功添加{0}", ItemHelper.GetGeneralItemByItemId(eiid).name));
            });
        }
    }
}
