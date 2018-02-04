// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  RecommendTeamViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppDto;
using AppServices;
using UniRx;
using UnityEngine;

public partial class RecommendTeamViewController
{
    private CompositeDisposable _disposable;
    
    private RecommendViewTab _curTab = RecommendViewTab.MyFriend;

    private bool _isCanRefresh = true;

    private int _colNum = 2;    //单行2个

    private readonly int _targetId = 0; //默认目标id
    private readonly int _minLv = 0;    //默认最小等级
    private readonly int _maxLv = 100;  //默认最大等级

    private Dictionary<GameObject, TeamEasyGroupItemController> _teamApplicationItemsDic  = new Dictionary<GameObject, TeamEasyGroupItemController>();
    private Dictionary<GameObject, TeamApplicationUnitedItemController> _playerApplicationItemDic = new Dictionary<GameObject, TeamApplicationUnitedItemController>(); 
    private Dictionary<GameObject, TeamApplicationUnitedItemController> _friendApplicationItemDic = new Dictionary<GameObject, TeamApplicationUnitedItemController>(); 

    private static readonly ITabInfo[] TabName =
    {
        TabInfoData.Create((int) RecommendViewTab.MyFriend, "我的好友")
        , TabInfoData.Create((int) RecommendViewTab.GuildPlayer, "帮派成员")
        , TabInfoData.Create((int) RecommendViewTab.NearByPlayer, "附近玩家")
        , TabInfoData.Create((int) RecommendViewTab.NearByTeam, "附近队伍")
    };

    private TabbtnManager tabBtnMgr;

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        _disposable = new CompositeDisposable();
        
        IniePageTabBtns();
        InitTeamItems();
        InitPlayerItems();
        InitFriendItems();
        TurnPageView(_curTab);
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        _view.PlayerGrid_UIRecycledList.onUpdateItem = UpdatePlayerRecycledList;
        _view.TeamGrid_UIRecycledList.onUpdateItem = UpdateTeamRecycledList;
        _view.FriendGrid_UIRecycledList.onUpdateItem = UpdateFriendRecycledList;

        _disposable.Add(OnRefreshBtn_UIButtonClick.Subscribe(_ => OnRefreshPlayerOnHandler()));
        _disposable.Add(OnOneKeyInviteBtn_UIButtonClick.Subscribe(_ => OnOneKeyInviteBtnClickHandler())); 
        _disposable.Add(OneKeyApplyBtn_UIButtonClick.Subscribe(_ => OnOneKeyApplyClickHenalder()));
    }

    protected override void OnDispose()
    {
        _disposable = _disposable.CloseOnceNull();
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    private void InitTeamItems()
    {
        for (int i = 0; i < TeamDataMgr.CreateTeamItemMax; ++i)
        {
            var ctrl = AddChild<TeamEasyGroupItemController, TeamEasyGroupInfoItem>(
                _view.TeamGrid_UIRecycledList.gameObject
                , TeamEasyGroupInfoItem.NAME
                , "TeamItem" + i);

            _teamApplicationItemsDic.Add(ctrl.gameObject, ctrl);
            _disposable.Add(ctrl.OnItemClickEvent.Subscribe(dto => { OnTeamItemClickEvent(dto); }));
        }
    }

    private void InitPlayerItems()
    {
        for (int i = 0; i < TeamDataMgr.CreateTeamItemMax; ++i)
        {
            var ctrl = AddChild<TeamApplicationUnitedItemController, TeamApplicationUnitedItem>(
                _view.PlayerGrid_UIRecycledList.gameObject
                , TeamApplicationUnitedItem.NAME
                , "PlayerItem" + i);

            _playerApplicationItemDic.Add(ctrl.gameObject, ctrl);
            _disposable.Add(ctrl.GetSubject.Subscribe(index =>
            {
                var dto = TeamDataMgr.DataMgr.GetNearByPlayerDto(index);
                OnPlayerItemClickEvent(dto);
            }));
        }
    }
    
    private void InitFriendItems()
    {
        for (int i = 0; i < TeamDataMgr.CreateTeamItemMax; ++i)
        {
            var ctrl = AddChild<TeamApplicationUnitedItemController, TeamApplicationUnitedItem>(
                _view.FriendGrid_UIRecycledList.gameObject
                , TeamApplicationUnitedItem.NAME
                , "FriendItem" + i);

            _friendApplicationItemDic.Add(ctrl.gameObject, ctrl);
            _disposable.Add(ctrl.GetSubject.Subscribe(index =>
            {
                var dto = TeamDataMgr.DataMgr.GetFriendDto(index);
                OnPlayerItemClickEvent(dto);
            }));
        }
    }

    private void IniePageTabBtns()
    {
        var idx = TabName.FindElementIdx(
            s => s.EnumValue == (int)RecommendViewTab.MyFriend);

        tabBtnMgr = TabbtnManager.Create(
            TabName
            , delegate (int i)
            {
                return AddChild<TabBtnWidgetController, TabBtnWidget>(
                    _view.TabGrid_UIGrid.gameObject
                    , TabbtnPrefabPath.TabBtnWidget_S3.ToString()
                    , "TabBtn" + i);
            }, idx);

        tabBtnMgr.SetBtnLblFont(20, "2d2d2d", 18, ColorConstantV3.Color_VerticalUnSelectColor2_Str);

        tabBtnMgr.SetTabBtn(2);        //切换标签页颜色
        tabBtnMgr.SetTabBtn(0);

        _view.TabGrid_UIGrid.Reposition();

        _disposable.Add(tabBtnMgr.Stream.Subscribe(pageIndex =>
        {
            TurnPageView((RecommendViewTab)pageIndex);
        }));
    }

    public void UpdateTabBtn(int page)
    {
        tabBtnMgr.SetTabBtn(page < 0 ? 0 : page);
    }
    
    public void TurnPageView(RecommendViewTab pageIndex)
    {
        _curTab = pageIndex;
        ShowPlayerOrTeamGrid();
        switch (pageIndex)
        {
            case RecommendViewTab.MyFriend:
                TeamDataMgr.TeamNetMsg.RecommendPlayer();
                break;
            case RecommendViewTab.GuildPlayer:
                TeamDataMgr.TeamNetMsg.RecommendGuild();
                break;
            case RecommendViewTab.NearByPlayer:
                TeamDataMgr.TeamNetMsg.RecommendTeam();
                break;
            case RecommendViewTab.NearByTeam:
                TeamDataMgr.TeamNetMsg.RecommendTeam();
                break;
        }
        _view.OneKeyInviteBtn_UIButton.gameObject.SetActive(pageIndex != RecommendViewTab.NearByTeam);
        _view.OneKeyApplyBtn_UIButton.gameObject.SetActive(pageIndex == RecommendViewTab.NearByTeam);
    }

    //用特定接口类型数据刷新界面
    public void UpdateView(ITeamData data)
    {
        switch (_curTab)
        {
            case RecommendViewTab.MyFriend:
                UpdateFriendList(data);
                break;
            case RecommendViewTab.GuildPlayer:
                UpdateGuildPlayerList(data);
                break;
            case RecommendViewTab.NearByPlayer:                
                UpdateNeaybyPlayerList(data);
                break;
            case RecommendViewTab.NearByTeam:
                UpdateTeamList(data);
                break;
        }
    }

    #region 我的好友
    private void UpdateFriendList(ITeamData data)
    {        
        _view.FriendGrid_UIRecycledList.gameObject.SetActive(true);
        int num = Mathf.CeilToInt((float)data.RecommendData.GetFriendList.Count() / (float)_colNum);
        View.FriendGrid_UIRecycledList.UpdateDataCount(num, true);

        _view.Texture_UISprite.gameObject.SetActive(num == 0);
        _view.DescLb_UILabel.gameObject.SetActive(num == 0);
        _view.DescLb_UILabel.text = "暂无好友";
    }

    private void UpdateFriendRecycledList(GameObject go, int itemIndex, int dataIndex)
    {
        if (_friendApplicationItemDic == null)
            return;
        
        TeamApplicationUnitedItemController item = null;
        if (_friendApplicationItemDic.TryGetValue(go, out item))
        {
            var info = TeamDataMgr.DataMgr.GetTwoFriendDto(dataIndex * _colNum);
            item.UpdateItemInfo(info, dataIndex);
        }
    }
    #endregion 

    #region 帮派玩家
    private void UpdateGuildPlayerList(ITeamData data)
    {
        _view.Texture_UISprite.gameObject.SetActive(true);
        _view.DescLb_UILabel.gameObject.SetActive(true);
        _view.DescLb_UILabel.text = "请先加入公会";
    }
    #endregion
    
    #region 附近玩家
    private void UpdateNeaybyPlayerList(ITeamData data)
    {
        View.PlayerGrid_UIRecycledList.gameObject.SetActive(true);
        float num = Mathf.Ceil((float)data.RecommendData.GetNearByDto.players.Count / (float)_colNum);
        View.PlayerGrid_UIRecycledList.UpdateDataCount((int)num, true);
        _view.Texture_UISprite.gameObject.SetActive(num == 0);
        _view.DescLb_UILabel.gameObject.SetActive(num == 0);
        _view.DescLb_UILabel.text = "附近没有玩家";
    }
    
    private void UpdatePlayerRecycledList(GameObject go, int itemIndex, int dataIndex)
    {
        if (_playerApplicationItemDic == null)
            return;

        TeamApplicationUnitedItemController item = null;
        if (_playerApplicationItemDic.TryGetValue(go, out item))
        {
            var info = TeamDataMgr.DataMgr.GetTwoNearByPlayerDto(dataIndex * _colNum);
            item.UpdateItemInfo(info, dataIndex);
        }
    }
    #endregion
   
    #region 附近队伍
    private void UpdateTeamList(ITeamData data)
    {
        _view.TeamGrid_UIRecycledList.gameObject.SetActive(true);
        _view.TeamGrid_UIRecycledList.UpdateDataCount(data.RecommendData.GetNearByDto.teams.Count, true);
        _view.Texture_UISprite.gameObject.SetActive(data.RecommendData.GetNearByDto.teams.Count == 0);
        _view.DescLb_UILabel.gameObject.SetActive(data.RecommendData.GetNearByDto.teams.Count == 0);
        _view.DescLb_UILabel.text = "附近没有队伍";
    }
    
    private void UpdateTeamRecycledList(GameObject go, int itemIndex, int dataIndex)
    {
        if (_teamApplicationItemsDic == null)
            return;

        TeamEasyGroupItemController item = null;
        if (_teamApplicationItemsDic.TryGetValue(go, out item))
        {
            if (dataIndex < TeamDataMgr.DataMgr.GetTeamDto().Count)
                item.UpdateData(TeamDataMgr.DataMgr.GetTeamDtoByIdx(dataIndex), dataIndex);
        }
    }
    #endregion

    private void ShowPlayerOrTeamGrid()
    {
        _view.PlayerGrid_UIRecycledList.gameObject.SetActive(false);
        _view.TeamGrid_UIRecycledList.gameObject.SetActive(false);
        _view.FriendGrid_UIRecycledList.gameObject.SetActive(false);
    }

    private void OnTeamItemClickEvent(TeamDto dto)
    {
        if (TeamDataMgr.DataMgr.HasTeam())
        {
            TipManager.AddTip("已有队伍，无法申请加入");
            return;
        }
        ServiceRequestAction.requestServer(Services.Team_JoinTeam(dto.leaderPlayerId, ""), "InviteMember", (e) =>
        {
            TipManager.AddTip(string.Format("已申请加入{0}的队伍，请耐心等待回复", dto.leaderPlayerName.ToString()));
            //JSTimer.Instance.SetupCoolDown(dto.id.ToString(), 10f, null, null);
        });
    }

    private void OnPlayerItemClickEvent(TeamPlayerDto dto)
    {
        //四轮之塔不能组队
        if (TowerDataMgr.DataMgr.IsInTower())
            return;
        TeamDto teamDto = TeamDataMgr.DataMgr.GetSelfTeamDto();
        if (teamDto == null)
        {
            ServiceRequestAction.requestServer(Services.Team_InvitePlayer(dto.id, _targetId, 
                _minLv, _maxLv, ""), "OnPlayerItemClickEvent", (e) =>
            {
                TipManager.AddTip(string.Format("已邀请[2DC6F8]{0}[-]加入队伍，请耐心等待回复", dto.nickname));
                //JSTimer.Instance.SetupCoolDown(dto.id.ToString(), 10f, null, null);
            });
            return;
        }

        ServiceRequestAction.requestServer(Services.Team_InvitePlayer(dto.id, teamDto.formationId, 
            teamDto.minGrade, teamDto.maxGrade, ""), "OnPlayerItemClickEvent", (e) =>
        {
            TipManager.AddTip(string.Format("已邀请[2DC6F8]{0}[-]加入队伍，请耐心等待回复", dto.nickname));
            //JSTimer.Instance.SetupCoolDown(dto.id.ToString(), 10f, null, null);
        });
    }
    
    private void OnRefreshPlayerOnHandler()
    {
        if (!_isCanRefresh)
        {
            var remainTime = Mathf.Floor(JSTimer.Instance.GetRemainTime("RecommendTeam"));
            if (remainTime > 0)
                TipManager.AddTip(string.Format("刷新太急了，请等{0}秒后再试试吧.", remainTime));
            return;
        }

        _isCanRefresh = false;
        TipManager.AddTip("===刷新列表===");
        JSTimer.Instance.SetupCoolDown("RecommendTeam", 30f, null, () =>
        {
            _isCanRefresh = true;
        });

        TurnPageView(_curTab);
    }

    //一键邀请
    private void OnOneKeyInviteBtnClickHandler()
    {
        //四轮之塔不能组队
        if (TowerDataMgr.DataMgr.IsInTower())
            return;
        string list = GetPlayerIdList();
        if (list == "")
        {
            TipManager.AddTip("=====附近没有玩家=====");
            return;
        }
        TeamDataMgr.TeamNetMsg.OnkeyInvitation(list, "");
    }

    //一键申请
    private void OnOneKeyApplyClickHenalder()
    {
        string list = GetPlayerIdList();
        if (list == "")
        {
            TipManager.AddTip("=====附近没有队伍=====");
            return;
        }
        TeamDataMgr.TeamNetMsg.OnKeyApplication(list, "");
    }

    private string GetPlayerIdList()
    {
        StringBuilder str = new StringBuilder();
        switch (_curTab)
        {
            case RecommendViewTab.MyFriend:
                TeamDataMgr.DataMgr.GetPlayerDtoList().ForEach(dto => { str.Append(dto.id + ","); });
                break;
            case RecommendViewTab.GuildPlayer:
                
                break;
            case RecommendViewTab.NearByPlayer:
                TeamDataMgr.DataMgr.GetNearbyPlayerDtoList().ForEach(dto => { str.Append(dto.id + ","); });
                break;
            case RecommendViewTab.NearByTeam:
                TeamDataMgr.DataMgr.GetNearbyTeamList().ForEach(dto => { str.Append(dto.id + ","); });
                break;
        }
        return str.ToString();
    }
}
