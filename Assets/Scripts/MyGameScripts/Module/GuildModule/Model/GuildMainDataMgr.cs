// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : DM-PC092
// Created  : 1/10/2018 2:11:20 PM
// **********************************************************************


using AppDto;
using System.Collections.Generic;
using UniRx;

namespace StaticInit
{
    public partial class StaticInit
    {
        private StaticDispose.StaticDelegateRunner guildDataMgr = new StaticDispose.StaticDelegateRunner(() => { var mgr = GuildMainDataMgr.DataMgr; });
    }
}

public sealed partial class GuildMainDataMgr
{
    
    // 初始化
    private void LateInit()
    {
        _disposable.Add(NotifyListenerRegister.RegistListener<GuildInvitationNotify>(GuildInvitation));
        _disposable.Add(WorldModel.Stream.Subscribe(e =>  _data.UpdateGuildState(e.PlayerSceneDto)));
        if (WorldManager.WorkdModelStream != null)
        {
            _disposable.Add(WorldManager.WorkdModelStream.Subscribe(e => _data.UpdateGuildState(e.PlayerGuildInfoDto)));
        }
    }
    
    private void OnDispose(){
            
    }

    private void GuildInvitation(GuildInvitationNotify notify)
    {
        string des = notify.inviterName.WrapColor(ColorConstantV3.Color_Green) + "邀请您加入" + notify.guildName.WrapColor(ColorConstantV3.Color_Green) + "公会";
        string title = "";
        var ctrl = ProxyBaseWinModule.Open();
        BaseTipData data = BaseTipData.Create(title, des, 0, () =>
           {
               GuildMainNetMsg.ReqAcceptInvite(notify.inviterId);
           }, ()=> 
           {
               GameDebuger.LogError("拒绝");
           }, "我再看看", "确定加入");
    }

    public IEnumerable<GuildPosition> GuildPosition
    {
        get { return _data.GuildPosition; }
    }
}
