// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Created  : 1/22/2018 3:17:47 PM
// **********************************************************************

public class ProxyTremChallengeConfirm
{
    public static void Open(){
        TremChallengeConfirmDataMgr.TeamChallengeConfirmViewLogic.Open();
    }
    public static void Close()
    {
        TremChallengeConfirmDataMgr.TeamChallengeConfirmViewLogic.OnClose();
    }
}

