// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Created  : 9/20/2017 7:57:58 PM
// **********************************************************************

public class ProxyAssistSkillMain
{
    public static void OpenAssistSkillModule()
    {
        AssistSkillMainDataMgr.AssistSkillMainViewLogic.Open();
    }

    public static void CloseAssistSkillModule()
    {
        UIModuleManager.Instance.CloseModule(AssistSkillMainView.NAME);
    }

    public static void CloseDelegateFriendView()
    {
        UIModuleManager.Instance.CloseModule(AssistDelegateFriendView.NAME);
    }
}

