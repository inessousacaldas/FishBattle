// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Created  : 8/29/2017 7:33:42 PM
// **********************************************************************

using UniRx;
using UnityEngine;
using AppDto;
using System.Collections.Generic;
public class ProxyNpcDialogueView
{

    //private static BaseDialogueViewController CreateDialogueController(SceneNpcDto npcStateDto)
    //{
    //    GameObject view = UIModuleManager.Instance.OpenFunModule(NpcDialogueView.NAME, UILayerType.FourModule, true);
    //    //if(WorldManager.Instance.GetModel().GetSceneDto().guildId != 0)
    //    //{

    //    //}
    //    //为不同的NPC添加不同的脚本
    //    return view.GetMissingComponent<SceneNpcDialogueViewController>();
    //}


    public static void OpenNpcDialogue(BaseNpcInfo npcinfo)
    {
        Close();
        //if(npcinfo.npcStateDto == null)
        //{
        //    return;
        //}

        //BaseDialogueViewController controller=CreateDialogueController(npcinfo.npcStateDto);
        //if(controller != null)
        //{
        //    controller.Open(npcinfo);
        //}
    }

    public static void Open(BaseNpcInfo npcInfo)
    {
        MissionDataMgr.NpcDialogueViewLogic.Open(npcInfo);
    }

    public static void OpenCustomPanel(Npc npcinfo,string content,List<string> optionList,System.Action<int> onSelect)
    {
        MissionDataMgr.NpcDialogueViewLogic.OpenCustomPanel(npcinfo,content,optionList,onSelect);
    }



    public static void Close()
    {
        MissionDataMgr.NpcDialogueViewLogic.Close();
    }

}

