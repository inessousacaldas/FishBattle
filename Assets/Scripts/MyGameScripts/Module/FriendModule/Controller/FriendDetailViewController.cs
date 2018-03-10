// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  FriendDetailViewController.cs
// Author   : xjd
// Created  : 10/16/2017 6:09:27 PM
// Porpuse  : 
// **********************************************************************

using System.Collections.Generic;
using UniRx;
using UnityEngine;
using AppDto;
using AppServices;


public partial interface IFriendDetailViewController
{
    void UpdateView(ShortPlayerDto dto);
    void UpdateView(FriendInfoDto dto);
    void UpdateGuildView(GuildMemberDto dto);
    void UpdateView(ScenePlayerDto dto);
    void SetPosition(Vector3 pos);
    void UpdateRankView(PlayerTipDto dto);
    void Close();
}
public partial class FriendDetailViewController
{
    private bool _isMyFriend = true;
    private bool _isMyBlack = true;
    private List<UILabel> _tagLbList = new List<UILabel>();
    private FriendInfoDto _infoDto = new FriendInfoDto();
    private const int TeamMaxCnt = 4;

    public static IFriendDetailViewController Show<T>(
            string moduleName
            , UILayerType layerType
            , bool addBgMask
            , bool bgMaskClose = true)
            where T : MonoController, IFriendDetailViewController
    {
        var controller = UIModuleManager.Instance.OpenFunModule<T>(
                moduleName
                , layerType
                , addBgMask
                , bgMaskClose) as IFriendDetailViewController;
            
        return controller;        
    }         
        
	// 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        for (int i = 0; i < _view.Grid_UIGrid.transform.childCount; i++)
        {
            var lb = _view.Grid_UIGrid.GetChild(i).GetComponent<UILabel>();
            _tagLbList.Add(lb);
        }
    }

	// 客户端自定义代码
	protected override void RegistCustomEvent ()
    {
        UICamera.onClick += OnClose;

        ZoneBtn_UIButtonEvt.Subscribe(_ =>
        {
            TipManager.AddTip("进入空间");

            Close();
        });

        FlowerBtn_UIButtonEvt.Subscribe(_ =>
        {
            ProxyFlowerMainView.Open(_infoDto);

            Close();
        });

        WatchBtn_UIButtonEvt.Subscribe(_ =>
        {
            TipManager.AddTip("观察");

            Close();
        });

        //添加删除好友
        DeleteBtn_UIButtonEvt.Subscribe(_ =>
        {
            if (_isMyFriend)     //删除好友
            {
                if(_infoDto.degree > 0)
                    BuiltInDialogueViewController.OpenView(string.Format("删除该好友将会清空与该好友的友好度（{0}点），确认删除好友{1}吗？", _infoDto.degree, _infoDto.name),
                    null, (() => { FriendDataMgr.FriendNetMsg.ReqDeleteFriend(_infoDto.friendId); }),
                        UIWidget.Pivot.Left, "取消", "确定");
                else
                    BuiltInDialogueViewController.OpenView(string.Format("确定删除好友{0}吗？", _infoDto.name),
                    null, (() => { FriendDataMgr.FriendNetMsg.ReqDeleteFriend(_infoDto.friendId); }),
                        UIWidget.Pivot.Left, "取消", "确定");
            }
            else
                FriendDataMgr.FriendNetMsg.ReqAddFriend(_infoDto.friendId);

            Close();
        });

        JoinTeamBtn_UIButtonEvt.Subscribe(_ =>
        {
            TeamDataMgr.TeamNetMsg.JoinTeam(_infoDto.friendId);

            Close();
        });

	    InviteTeamBtn_UIButtonEvt.Subscribe(_ =>
	    {
            TeamDataMgr.TeamNetMsg.InviteMember(_infoDto.friendId, _infoDto.name);
	        Close();
	    });

        JoinFactionBtn_UIButtonEvt.Subscribe(_ =>
        {
            Close();
        });

        ReportBtn_UIButtonEvt.Subscribe(_ =>
        {
            Close();
        });

        FightBtn_UIButtonEvt.Subscribe(_ =>
        {
            if (TeamDataMgr.DataMgr.HasTeam())
            {
                if (!TeamDataMgr.DataMgr.IsLeader())
                    TipManager.AddTip("只有队长才可以发起战斗哦");
                else
                {
                    var controller = ProxyBaseWinModule.Open();
                    var title = "切磋";
                    string txt = "是否向对方发出切磋邀请";
                    if (TeamDataMgr.DataMgr.HasAwayTeamMember())
                        txt = "己方队伍有队员离队/暂离,是否继续发起切磋";

                    BaseTipData data = BaseTipData.Create(title, txt, 0, () =>
                    {
                        BattleDataManager.BattleNetworkManager.BattleBefore(_infoDto.friendId);
                    }, null);
                    controller.InitView(data);
                }
            }
            else
                BattleDataManager.BattleNetworkManager.BattleBefore(_infoDto.friendId);
            Close();
        });

        //添加删除黑名单
        BlackBtn_UIButtonEvt.Subscribe(_ => 
        {
            if (_isMyBlack)     //取消拉黑
                FriendDataMgr.FriendNetMsg.ReqDeleteBlack(_infoDto.friendId);
            else if(_isMyFriend)    //拉黑好友
            {
                BuiltInDialogueViewController.OpenView("拉黑对象为你的好友，拉黑后将从好友名单中删除，是否确定要拉黑？",
                    null, (() => { FriendDataMgr.FriendNetMsg.ReqAddBlack(_infoDto.friendId); }),
                        UIWidget.Pivot.Left, "取消", "确定");
            }
            else
                FriendDataMgr.FriendNetMsg.ReqAddBlack(_infoDto.friendId);

            Close();
        });

        ChatBtn_UIButtonEvt.Subscribe(_ =>
        {
            Close();
            //处理主界面打开好友操作面板
            PrivateMsgDataMgr.DataMgr.SetCurFriendDto(_infoDto);
            ProxySociality.OpenChatMainView(ChatPageTab.PrivateMsg);

            //关闭排行榜界面,因为有可能是通过排行榜界面跳到私聊界面,所以需要关闭排行榜
            ProxyRank.CloseRankView();
        });
    }

    protected override void RemoveCustomEvent()
    {
        UICamera.onClick -= OnClose;
    }
        
    protected override void OnDispose()
    {
        base.OnDispose();
    }

	//在打开界面之前，初始化数据
	protected override void InitData()
    {
            
    }

    //刷新头像 国家 组队等信息 
    private void UpdateInfo(PlayerTipDto dto)
    {
        if (dto == null) return;
        View.TeamLabel_UILabel.text = dto.teamSize.ToString().WrapColor(ColorConstantV3.Color_Orange) + "/" + TeamMaxCnt.ToString().WrapColor("404040");
        var hasTeam = TeamDataMgr.DataMgr.HasTeam();
        if (!_isMyBlack)
        {
            View.JoinTeamBtn_UIButton.gameObject.SetActive(!hasTeam && dto.teamSize > 0);
            View.InviteTeamBtn_UIButton.gameObject.SetActive(hasTeam || dto.teamSize <= 0);
        }

        View.Table_UITable.Reposition();
        View.Name_UILabel.text = dto.name;
        View.NumLabel_UILabel.text = dto.id.ToString();
        View.CountryName_UILabel.text = dto.guildInfoDto == null ? "无" : dto.guildInfoDto.guildName;
        View.LvLabel_UILabel.text = dto.grade.ToString();
        if (dto.charactor as MainCharactor != null)
            UIHelper.SetPetIcon(View.Icon_UISprite, (dto.charactor as MainCharactor).gender == 1 ? "101" : "103");
        _tagLbList.ForEachI((lb, idx) =>
        {
            lb.gameObject.SetActive(idx < dto.labelInfos.Count);
            if (idx < dto.labelInfos.Count)
                lb.text = dto.labelInfos[idx];
        });
    }

    public void UpdateRankView(PlayerTipDto dto)
    {
        _isMyBlack = FriendDataMgr.DataMgr.isMyBlack(dto.id);
        _isMyFriend = FriendDataMgr.DataMgr.IsMyFriend(dto.id);
        _infoDto.friendId = dto.id;
        _infoDto.name = dto.name;
        _infoDto.grade = dto.grade;
        _infoDto.factionId = dto.factionId;
        _infoDto.countryId = 0;
        _infoDto.degree = 0;
        _infoDto.online = true;
        _infoDto.charactorId = dto.charactorId;
        _infoDto.charactor = dto.charactor;
        UpdateInfo(dto);
        SetBtnState();
        View.FightBtn_UIButton.gameObject.SetActive(false);
        View.Table_UITable.Reposition();
    }

    public void UpdateGuildView(GuildMemberDto dto)
    {
        _isMyFriend = FriendDataMgr.DataMgr.IsMyFriend(dto.id);
        _isMyBlack = FriendDataMgr.DataMgr.isMyBlack(dto.id);

        if (FriendDataMgr.DataMgr.GetFriendDtoById(dto.id) != null)
            _infoDto = FriendDataMgr.DataMgr.GetFriendDtoById(dto.id);
        else
        {
            _infoDto.friendId = dto.id;
            _infoDto.name = dto.name;
            _infoDto.grade = dto.grade;
            _infoDto.factionId = dto.factionId;
            _infoDto.countryId = 0;
            _infoDto.degree = 0;
            _infoDto.online = true;
        }
        View.NumLabel_UILabel.text = dto.id.ToString();

        GameUtil.GeneralReq(Services.Player_TipInfo(_infoDto.friendId), resp =>
        {
            var tipsDto = resp as PlayerTipDto;
            UpdateInfo(tipsDto);
        });
        SetBtnState();
    }

    //聊天界面
    public void UpdateView(ShortPlayerDto dto)
    {
        _isMyFriend = FriendDataMgr.DataMgr.IsMyFriend(dto.id);
        _isMyBlack = FriendDataMgr.DataMgr.isMyBlack(dto.id);

        if (FriendDataMgr.DataMgr.GetFriendDtoById(dto.id) != null)
            _infoDto = FriendDataMgr.DataMgr.GetFriendDtoById(dto.id);
        else
        {
            _infoDto.friendId = dto.id;
            _infoDto.name = dto.nickname;
            _infoDto.grade = dto.grade;
            _infoDto.factionId = dto.factionId;
            _infoDto.countryId = 0;
            _infoDto.degree = 0;
            _infoDto.online = true;
            _infoDto.charactorId = dto.charactorId;
            _infoDto.charactor = dto.charactor;
        }
        View.NumLabel_UILabel.text = dto.id.ToString();

        GameUtil.GeneralReq(Services.Player_TipInfo(_infoDto.friendId), resp =>
        {
            var tipsDto = resp as PlayerTipDto;
            UpdateInfo(tipsDto);
        });
        SetBtnState();
    }

    //好友列表玩家信息框
    public void UpdateView(FriendInfoDto dto)
    {
        _infoDto = dto;
        _isMyFriend = FriendDataMgr.DataMgr.IsMyFriend(_infoDto.friendId);
        _isMyBlack = FriendDataMgr.DataMgr.isMyBlack(_infoDto.friendId);
        View.NumLabel_UILabel.text = dto.friendId.ToString();

        GameUtil.GeneralReq(Services.Player_TipInfo(_infoDto.friendId), resp =>
        {
            var tipsDto = resp as PlayerTipDto;
            UpdateInfo(tipsDto);
        });
        SetBtnState();
    }

    //场景中玩家信息框
    public void UpdateView(ScenePlayerDto dto)
    {
        _isMyFriend = FriendDataMgr.DataMgr.IsMyFriend(dto.id);
        _isMyBlack = FriendDataMgr.DataMgr.isMyBlack(dto.id);

        if (FriendDataMgr.DataMgr.GetFriendDtoById(dto.id) != null)
            _infoDto = FriendDataMgr.DataMgr.GetFriendDtoById(dto.id);
        else
        {
            _infoDto.friendId = dto.id;
            _infoDto.name = dto.name;
            _infoDto.grade = dto.grade;
            _infoDto.factionId = dto.factionId;
            _infoDto.countryId = 0;
            _infoDto.degree = 0;
            _infoDto.online = true;
            _infoDto.charactorId = dto.charactorId;
            _infoDto.charactor = dto.charactor;
        }
        View.NumLabel_UILabel.text = dto.id.ToString();

        GameUtil.GeneralReq(Services.Player_TipInfo(_infoDto.friendId), resp =>
        {
            var tipsDto = resp as PlayerTipDto;
            UpdateInfo(tipsDto);
        });
        SetBtnState();
    }

    private void SetBtnState()
    {
        View.DeleteLabel_UILabel.text = _isMyFriend ? "删除好友" : "添加好友";
        View.BlackLabel_UIlabel.text = _isMyBlack ? "取消拉黑" : "拉黑";
        //UIHelper.SetFactionIcon(View.Faction_UISprite, _infoDto.factionId);

        if (_isMyBlack)
        {
            View.ZoneBtn_UIButton.gameObject.SetActive(false);
            View.FlowerBtn_UIButton.gameObject.SetActive(false);
            View.WatchBtn_UIButton.gameObject.SetActive(false);
            View.DeleteBtn_UIButton.gameObject.SetActive(false);
            View.JoinFactionBtn_UIButton.gameObject.SetActive(false);
            View.ReportBtn_UIButton.gameObject.SetActive(false);
            View.FightBtn_UIButton.gameObject.SetActive(false);
            View.JoinTeamBtn_UIButton.gameObject.SetActive(false);
            View.InviteTeamBtn_UIButton.gameObject.SetActive(false);

            View.Table_UITable.Reposition();
            View.Bg_UISprite.SetDimensions(View.Bg_UISprite.width, 256);
            View.BtnBg_UISprite.SetDimensions(View.BtnBg_UISprite.width, 47);
        }
        else
        {
            //false 暂时隐藏 todo xjd
            View.ZoneBtn_UIButton.gameObject.SetActive(false);
            View.FlowerBtn_UIButton.gameObject.SetActive(true);
            View.WatchBtn_UIButton.gameObject.SetActive(false);
            View.DeleteBtn_UIButton.gameObject.SetActive(true);
            View.JoinFactionBtn_UIButton.gameObject.SetActive(false);
            View.ReportBtn_UIButton.gameObject.SetActive(false);
            View.FightBtn_UIButton.gameObject.SetActive(true);
            if(View.JoinTeamBtn_UIButton.gameObject.activeSelf == View.InviteTeamBtn_UIButton.gameObject.activeSelf)
            {
                View.JoinTeamBtn_UIButton.gameObject.SetActive(false);
                View.InviteTeamBtn_UIButton.gameObject.SetActive(true);
            }

            View.Table_UITable.Reposition();
            View.Bg_UISprite.SetDimensions(View.Bg_UISprite.width, /*416*/348);
            View.BtnBg_UISprite.SetDimensions(View.BtnBg_UISprite.width, /*207*/127);
        }
    }

    public void SetPosition(Vector3 pos)
    {
        View.Bg_UISprite.transform.localPosition = pos;
    }

    public void OnClose(GameObject go)
    {
        var panel = UIPanel.Find(go.transform);
        if (panel != View.transform.GetComponent<UIPanel>())
            Close();
    }

    public void Close()
    {
        UIModuleManager.Instance.CloseModule(FriendDetailView.NAME);
    }

    readonly UniRx.Subject<FriendInfoDto> clickChatStream = new UniRx.Subject<FriendInfoDto>();
    public UniRx.IObservable<FriendInfoDto> OnClickChatStream
    {
        get { return clickChatStream; }
    }
}
