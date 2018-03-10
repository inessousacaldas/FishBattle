// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Created  : 1/16/2018 12:11:22 PM
// **********************************************************************

public class ProxyRank
{
    public static void OpenRankView(int rankId = 100001)
    {
        RankDataMgr.RankNetMsg.RankingsInfo(rankId);
        RankDataMgr.RankingViewLogic.Open();
    }

    public static void CloseRankView()
    {
        UIModuleManager.Instance.CloseModule(RankingView.NAME);
    }
}

