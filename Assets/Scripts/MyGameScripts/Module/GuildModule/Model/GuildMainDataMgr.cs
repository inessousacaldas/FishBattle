// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : DM-PC092
// Created  : 1/10/2018 2:11:20 PM
// **********************************************************************


using System;
using AppDto;
using Asyn;
using System.Collections.Generic;
using UniRx;

namespace StaticInit
{
    public partial class StaticInit
    {
        private StaticDispose.StaticDelegateRunner guildDataMgr = new StaticDispose.StaticDelegateRunner(() => { var mgr = GuildMainDataMgr.DataMgr; });
    }
}

public sealed partial class GuildMainDataMgr : AbstractAsynInit
{
    public override void StartAsynInit(Action<IAsynInit> onComplete)
    {
        Action act = delegate ()
        {
            onComplete(this);
        };
        GuildMainNetMsg.ReqEnterGuildInfo(act);
    }

    // 初始化
    private void LateInit()
    {
        _disposable.Add(NotifyListenerRegister.RegistListener<GuildInvitationNotify>(GuildInvitation));
        _disposable.Add(WorldModel.PlayerGuildInfoStream.Subscribe(e =>  _data.UpdateGuildState()));
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
    public PlayerGuildInfoDto PlayerGuildInfo
    {
        get { return _data.PlayerGuildInfo; }
    }
    public void FindToNpc()
    {
        WorldManager.Instance.FlyToByNpc(_data.GuildManager, 0);
    }

    public GuildState GuildState { get { return _data.GuildState; } }

    //需要做判空处理,自身公会信息（为什么不从scenepalyer.guildinfodto去取自身公会信息，是因为有些数据例如人数，取不到，所以需要从这边去请求）
    public  GuildBaseInfoDto PlayerGuildBase { get { return _data.GuildBaseInfo; } }

    public IEnumerable<GuildDonate> GuildDonateList { get { return _data.GuildDonateList; } }
}
