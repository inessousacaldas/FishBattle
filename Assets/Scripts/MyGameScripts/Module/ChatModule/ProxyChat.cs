using System;
using AppDto;
using UnityEngine;
public class ProxyChat
{
    // 打开聊天框过滤选项面板
    public static void OpenChannelToggleView()
    {
        UIModuleManager.Instance.OpenFunModule<ChatDataMgr.ChatChannelToggleViewController>(
            ChatChannelToggleView.NAME
            , UILayerType.DefaultModule
            , true);
    }

    //关闭聊天频道切换界面
    public static void CloseChatChannelView()
    {
        UIModuleManager.Instance.CloseModule(ChatChannelToggleView.NAME);
    }
}

