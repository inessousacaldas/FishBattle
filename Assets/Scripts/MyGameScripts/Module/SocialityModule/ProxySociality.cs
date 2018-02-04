using ChatChannelEnum = AppDto.ChatChannel.ChatChannelEnum;
public class ProxySociality
{
    public static void OpenChatMainView(ChatPageTab tab, ChatChannelEnum channel = ChatChannelEnum.Unknown)
    {
        //        if (!FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum_Chat, true))
//        {
//            return;
//        }
// todo fish: 校验枚举有效值
//        if (channelId == -1)
//            channelId = ModelManager.Chat.CurChannelId;

        SocialityDataMgr.SocialityViewLogic.Open(tab, channel);

//        controller.gameObject.transform.localPosition = new Vector3(-1346f,0f,0f);
//        controller.Open(channelId);
        
    }
    
    public static void CloseChatInfoView()
    {
        UIModuleManager.Instance.CloseModule(ChatInfoView.NAME);
    }

    public static void OpenChatBoxSetting()
    {
        ChatSettingPanelController.Show<ChatSettingPanelController>(ChatSettingPanel.NAME, UILayerType.FourModule, true,true);
    }

    public static void CloseChatBoxSetting()
    {
        UIModuleManager.Instance.CloseModule(ChatSettingPanel.NAME);
    }
}

