// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Created  : 2/27/2018 4:27:42 PM
// **********************************************************************

public class ProxyRedPack
{
    public static void OpenRedPackMainView(RedPackChannelType type)
    {
        var info = GuildMainDataMgr.DataMgr.PlayerGuildBase;
        var guildID = info != null ? info.showId : 0l;
        RedPackDataMgr.RedPackNetMsg.ReqRedPackInfo(guildID, () =>
        {
            RedPackDataMgr.RedPackViewLogic.Open(type);
        });
    }
    public static void OpenRedPackSendView()
    {
//        RedPackDataMgr.RedPackSendViewLogic.Open();
    }    
}

