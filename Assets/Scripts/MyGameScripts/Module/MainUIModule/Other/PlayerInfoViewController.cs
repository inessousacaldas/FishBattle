using UnityEngine;
using AppDto;
using AppServices;

public class PlayerInfoViewController : MonoViewController<PlayerInfoView>
{

    private ScenePlayerDto _playerDto;

    #region IViewController implementation

    public void UpdateView(ScenePlayerDto playerDto)
    {
        _playerDto = playerDto;

        View.NameLbl_UILabel.text = string.Format("{0}", _playerDto.name).WrapColor(ColorConstantV3.Color_White_Str);
        UIHelper.SetPetIcon(View.Icon, string.Format("large_{0}", _playerDto.charactor.texture));
        View.LvLbl_UILabel.text = string.Format("[b]{0}[-]", _playerDto.grade).WrapColor(ColorConstantV3.Color_White_Str);
        View.FactionIcon_UISprite.spriteName = string.Format("factionIcon_{0}",_playerDto.factionId);
        UIHelper.SetOtherIcon(View.FactionIcon_UISprite, "small_faction_" + _playerDto.factionId);
        

        GameDebuger.TODO(@"View.VipSprite_UISprite.isGrey = !_playerDto.vip;");  

        View.IdLbl_UILabel.text = string.Format("ID：{0}", _playerDto.id).WrapColor(ColorConstantV3.Color_White_Str);

        GameDebuger.TODO(@"if (_playerDto.guildId != 0 && string.IsNullOrEmpty(_playerDto.guildName))
        {
            ServiceRequestAction.requestServer(GuildService.query(_playerDto.guildId), "", (e) =>
                {
                    if (View == null)
                        return;
                    GuildBaseDto dto = e as GuildBaseDto;
                    if (dto != null)
                    {
                        View.GuildNameLbl_UILabel.text = string.Format('帮派：{0}', dto.name).WrapColor(ColorConstantV3.Color_White_Str);
                    }
                });
        }
        else
            View.GuildNameLbl_UILabel.text = string.Format('帮派：{0}', _playerDto.guildName).WrapColor(ColorConstantV3.Color_White_Str);if (!ModelManager.Friend.IsMyFriend(_playerDto.id))
        {
            View.PositionLbl_UILabel.text = '位置：仅好友可见'.WrapColor(ColorConstantV3.Color_White_Str);
        }
        else if (FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum_LBS, false) && !_playerDto.closedLocation)
        {
            if (!string.IsNullOrEmpty(_playerDto.locationInfo))
                View.PositionLbl_UILabel.text = string.Format('位置：{0}', _playerDto.locationInfo).WrapColor(ColorConstantV3.Color_White_Str);
            else
                View.PositionLbl_UILabel.text = '位置：未知'.WrapColor(ColorConstantV3.Color_White_Str);
        }
        else");
        {
            View.PositionLbl_UILabel.text = "位置：未分享".WrapColor(ColorConstantV3.Color_White_Str);
        }

        if (TeamDataMgr.DataMgr.HasTeam())
        {
            if (_playerDto.teamStatus == (int)TeamMemberDto.TeamMemberStatus.NoTeam)
            {
                View.InviteTeamLbl.text = "邀请入队"; //自己有，对方没有
            }
            else
            {
                View.InviteTeamBtn.gameObject.SetActive(false); //自己有，对方也有
            }
        }
        else
        {
            if (_playerDto.teamStatus != (int)TeamMemberDto.TeamMemberStatus.NoTeam)
            {
                View.InviteTeamLbl.text = "申请入队";   //自己没有，对方有
            }
            else if (_playerDto.teamStatus == (int)TeamMemberDto.TeamMemberStatus.NoTeam)
                View.InviteTeamLbl.text = "邀请入队";   //自己没有，对方也没有
        }
        View.FactionNameLbl_UILabel.text = string.Empty;
            
        if (FriendDataMgr.DataMgr.IsMyFriend(playerDto.id))          //判断这个角色是否是自己好友， 注意这里id要对应
        {
            isStrange = false;
            View.AddFriendLbl.text = "删除好友";
        }
        else
        {
            isStrange = true;
            View.AddFriendLbl.text = "添加好友";
        }

        GameDebuger.TODO(@"if (_playerDto.teamStatus != (int)TeamMemberDto.TeamMemberStatus.NoTeam)
        {
             ServiceRequestAction.requestServer(PlayerService.tipInfo(_playerDto.id), "", (e) =>
                {
                    if (View != null && null != View.gameObject)
                    {
                        PlayerTipDto dto = e as PlayerTipDto;
                        if (dto != null)
                        {
                            int teamCount = dto.teamSize;
                            if (teamCount > 0)
                            {
                                View.FactionNameLbl_UILabel.text = string.Format('队伍:{0}/5', teamCount).WrapColor(ColorConstantV3.Color_White_Str);
                            }
                        }
                        else
                            UpdateFactionName(_playerDto.faction.name);
                    }
                }, (e) =>
                {
                    if (View != null && null != View.gameObject)
                    {
                        UpdateFactionName(_playerDto.faction.name);
                    }
                });
        }
        else
        {
            UpdateFactionName(_playerDto.faction.name);
        }

        if (FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum_JoinGuild, false, '', null))
        {
            if ((_playerDto.grade < 8)
                || (!ModelManager.Player.HasGuild() && _playerDto.guildId == 0)
                || (ModelManager.Player.HasGuild() && _playerDto.guildId > 0))
            {
                View.GuildBtn.gameObject.SetActive(false);
            }
            else if (ModelManager.Player.HasGuild() && _playerDto.guildId == 0)
            {
                View.GuildBtn.gameObject.SetActive(true);
                View.GuildBtnLabel_UILabel.text = '邀请入帮';
            }
            else if (!ModelManager.Player.HasGuild() && _playerDto.guildId > 0)
            {
                View.GuildBtn.gameObject.SetActive(true);
                View.GuildBtnLabel_UILabel.text = '申请入帮';
            }

        }
        else
        {
            View.GuildBtn.gameObject.SetActive(false);
        }

        if (ModelManager.Friend.IsMyFriend(_playerDto.id))
        {
            isStrange = false;
            View.AddFriendLbl.text = '删除好友';
        }
        else
        {
            isStrange = true;
            View.AddFriendLbl.text = '添加好友';
        }

        View.BeginChatBtn.gameObject.SetActive(FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum_ChatPrivate, false));

        View.GiveBtn.gameObject.SetActive(FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum_Give, false));


        View.PKBtn.gameObject.SetActive(ModelManager.Player.GetPlayerId() != _playerDto.id && FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum_CompareNotes, false) && WorldManager.Instance.CheckPlayerAtBattleScope(_playerDto.id));


        View.WatchBattleBtn_UIButton.gameObject.SetActive(WorldManager.Instance.GetModel().GetPlayerBattleStatus(_playerDto.id));

        View.AchievemnetBtn.gameObject.SetActive(FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum_Achievement, false));

        bool canShowReport = ModelManager.Player.GetPlayerLevel() >= DataCache.getDtoByCls<ReportConfig>(1).minGrade;
        View.ReportBtn_UIButton.gameObject.SetActive(canShowReport);
        View.InviteTeamBtn.gameObject.SetActive(!ModelManager.BridalSedan.IsMe());
        //屏蔽vip
        View.VipSprite_UISprite.gameObject.SetActive(false);

        View.VisitHomeBtn.gameObject.SetActive(FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum_Homeworld, false));

        //查看空间
        View.CheckZoneBtn_UIButton.gameObject.SetActive(FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum_MyZone, false));");

        int btnCount = View.ButtonGrid.GetChildList().Count;
        if (btnCount % 2 == 1)
            View.Bg.height = 198 + 66 * (btnCount / 2 + 1);
        else
            View.Bg.height = 198 + 66 * (btnCount / 2);

        View.ButtonGrid.Reposition();
        
    }

    private void UpdateFactionName(string name)
    {
        View.FactionNameLbl_UILabel.text =
            string.Format("{0}", name).WrapColor(ColorConstantV3.Color_White_Str);
    }

    protected override void RegistCustomEvent()
    {
        base.RegistCustomEvent();
        UICamera.onClick += ClickEventHandler;
        EventDelegate.Set(View.InviteTeamBtn.onClick, OnClickTeamBtn);
        EventDelegate.Set(View.GiveBtn.onClick, OnGiveBtnClick);
        EventDelegate.Set(View.AddFriendBtn.onClick, OnClickAddFriendBtn);
        EventDelegate.Set(View.BeginChatBtn.onClick, OnBeginChatBtnClick);
        EventDelegate.Set(View.PKBtn.onClick, OnPKBtnClick);
        EventDelegate.Set(View.VisitHomeBtn.onClick, OnVisitHomeBtnClick);
        EventDelegate.Set(View.GuildBtn.onClick, OnGuildBtnClick);
        EventDelegate.Set(View.WatchBattleBtn_UIButton.onClick, OnWatchBattleBtnClick);
        EventDelegate.Set(View.AchievemnetBtn.onClick, OnAchievemnetBtnClick);
        EventDelegate.Set(View.ReportBtn_UIButton.onClick, OnClickReportBtn);
        EventDelegate.Set(View.CheckZoneBtn_UIButton.onClick, OnClickCheckZoneBtn);
        EventDelegate.Set(View.DeFriendBtn_UIButton.onClick, OnClickDeFriendBtn);
    }

    protected override void OnDispose()
    {
        base.OnDispose();
        UICamera.onClick -= ClickEventHandler;
    }

    #endregion

    #region Button Event Handler

    public void OnClickTeamBtn()
    {
        GameDebuger.TODO(@"
        if (ModelManager.CSPK.IsInCSPKScene())
        {
            if (ModelManager.CSPK.IsOptionForbiddenForCSPK(FunctionOpen.FunctionOpenEnum_Team))
                return;
            if (ModelManager.CSPK.IsWatchingModel || _playerDto.cspkWatcher)
            {
                TipManager.AddTip('观众无法进行组队');
                return;
            }
        }
        ");

        if ((TeamMemberDto.TeamMemberStatus)_playerDto.teamStatus == TeamMemberDto.TeamMemberStatus.NoTeam)
        {
            TeamDataMgr.TeamNetMsg.InviteMember(_playerDto.id, _playerDto.name);
        }
        else
        {
            TeamDataMgr.TeamNetMsg.JoinTeam(_playerDto.id);
        }

        CloseView();
    }

    public void OnBeginChatBtnClick()
    {
        GameDebuger.TODO(@"if (ModelManager.CSPK.IsOptionForbiddenForCSPK(FunctionOpen.FunctionOpenEnum_ChatPrivate))
            return;

        UIPanel panel = UIPanel.Find(transform);
        ModelManager.Friend.BeginTalkTo(
            _playerDto.id
            , _playerDto.grade
            , _playerDto.nickname
            , _playerDto.charactorId
            , _playerDto.factionId);");
        
        CloseView();
    }

    public void OnPKBtnClick()
    {
        GameDebuger.TODO(@"if (ModelManager.CSPK.IsOptionForbiddenForCSPKSpecial())
            return;

        int grade = _playerDto.grade;
        if (_playerDto.teamStatus == PlayerDto.PlayerTeamStatus_Member)
        {
            ScenePlayerDto leader = WorldManager.Instance.GetModel().GetTeamLeader(_playerDto.teamUniqueId);
            if (leader != null)
            {
                grade = leader.grade;
            }
        }

        int checkGrade = DataCache.GetStaticConfigValue(AppStaticConfigs.ARENA_PLAYER_GRADE_DIFF);

        if (grade > ModelManager.Player.GetPlayerLevel() + checkGrade)
        {
            TipManager.AddTip(string.Format('等级低于对方{0}级以上，无法切磋', checkGrade));
            return;
        }
");
        if (BattleDataManager.DataMgr.IsInBattle)
        {
            TipManager.AddTip("战斗中不能同时切磋");
            return;
        }

        if (WorldManager.Instance.GetModel().GetPlayerBattleStatus(_playerDto.id ))
        {
            TipManager.AddTip("对方正在战斗， 不能发起切磋");
            return;
        }

        if (WorldManager.Instance.CheckPlayerAtBattleScope(_playerDto.id))
        {
            if (WorldManager.Instance.IsFollowLeader())
            {
                TipManager.AddTip("组队状态需要队长才可以发起切磋");
            }
            GameDebuger.TODO(@"else
            {
                ServiceRequestAction.requestServer(ArenaService.challenge(_playerDto.id));
                CloseView();
            }}");
        }
        else
        {
            GameDebuger.TODO(@"TipManager.AddTip('需要双方都处于擂台上才能进行切磋');");
        }
        GameDebuger.LogError("[DEMO/非错误]DEMO版本的PVP走模拟配置战斗的方式，不设专门的PVP协议！");
        BattleDataManager.BattleNetworkManager.ReqBattleDemo(_playerDto.id, CloseView);
    }

    private void OnWatchBattleBtnClick()
    {
        GameDebuger.TODO(@"if (ModelManager.CSPK.IsOptionForbiddenForCSPKSpecial())
            return;");
		
        BattleHelper.WatchPlayerBattle(_playerDto.id, 0, 0, BattleHelper.BattleWatchType.WatchType_Nothing, CloseView);
    }

    public void OnVisitHomeBtnClick()
    {
        TipManager.AddTip("该功能正在研发中");
    }

    public void OnGuildBtnClick()
    {
        GameDebuger.TODO(@"if (ModelManager.CSPK.IsOptionForbiddenForCSPK(FunctionOpen.FunctionOpenEnum_JoinGuild))
            return;

        if (ModelManager.Player.HasGuild() && _playerDto.guildId == 0)
        {
            // 邀请入帮
            ModelManager.Guild.Invite(_playerDto.id, _playerDto.nickname);
        }
        else if (!ModelManager.Player.HasGuild() && _playerDto.guildId >= 0)
        {
            // 申请入帮
            ServiceRequestAction.requestServer(GuildService.applyJoin(_playerDto.guildId), "", (e) =>
                {
                    TipManager.AddTip(string.Format('申请{0}成功', _playerDto.guildName));
                });
        }");
        CloseView();
    }

    public void OnGiveBtnClick()
    {
        GameDebuger.TODO(@"if (ModelManager.CSPK.IsOptionForbiddenForCSPK(FunctionOpen.FunctionOpenEnum_Give))
            return;

        if (!ModelManager.Player.IsPlayerBandMode(false))
        {
            UIPanel panel = UIPanel.Find(transform);
            ProxyGiftPropsModule.OpenGiftView(_playerDto, 0);
            CloseView();
        }
        else");
        {
            TipManager.AddTip("您被封号不能进行赠送");
        }
       
    }

    #endregion

    #region 添加/删除好友

    bool isStrange = true;

    public void OnClickAddFriendBtn()
    {
        GameDebuger.TODO(@"if (ModelManager.CSPK.IsOptionForbiddenForCSPK(FunctionOpen.FunctionOpenEnum_Friend))
            return;");

        if (isStrange)
        {
            //添加好友                  暂时通过事件系统添加，不太好访问模块数据

        }
        else
        {
           //删除好友
        }
    }

    #endregion

    /// <summary>
    /// 查看成就
    /// </summary>
    private void OnAchievemnetBtnClick()
    {
        GameDebuger.TODO(@"if (ModelManager.CSPK.IsOptionForbiddenForCSPK(FunctionOpen.FunctionOpenEnum_Achievement))
            return;

        ServiceRequestAction.requestServer(AchievementService.viewOther(_playerDto.id), "", (e) =>
            {
                AchievementDetailDto _dto = e as AchievementDetailDto;
                ;
                _dto.achievementDto.playerAchievementDtos = ModelManager.Achievement.GetPureAchievementDtoList(_dto.achievementDto.playerAchievementDtos);
                ProxyRankingModule.OpenLookAchievementView(_dto);
            });");
        CloseView();
    }

    void ClickEventHandler(GameObject clickGo)
    {
        UIPanel panel = UIPanel.Find(clickGo.transform);
        if (panel != View.uiPanel)
            CloseView();
    }

    private void OnClickReportBtn()
    {
        GameDebuger.TODO(@"if (ModelManager.CSPK.IsOptionForbiddenForCSPKSpecial())
            return;

        ProxyReportModule.Open(_playerDto.id, _playerDto.nickname, _playerDto.grade);");
        CloseView();
    }

    //查看空间
    private void OnClickCheckZoneBtn()
    {
        GameDebuger.TODO(@"if (ModelManager.CSPK.IsOptionForbiddenForCSPK(FunctionOpen.FunctionOpenEnum_MyZone))
            return;

        ModelManager.SelfZone.OpenZone(_playerDto.id, false);");
        CloseView();
    }

    private void OnClickDeFriendBtn()
    {
        //拉黑            TODO

        CloseView();
    }

    public void CloseView()
    {
        ProxyMainUI.ClosePlayerInfoView();
    }
}
