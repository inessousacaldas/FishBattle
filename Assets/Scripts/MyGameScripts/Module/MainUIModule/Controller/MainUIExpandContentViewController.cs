// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  MainUIExpandContentViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using System.Collections.Generic;
using AppDto;
using UniRx;
using UnityEngine;
using TeamNetMsg = TeamDataMgr.TeamNetMsg;

public partial interface IMainUIExpandContentViewController
{
    void HideTeamRequestBtn();

    void UpdateView(UIMode mode);
    void UpdateView(ITeamData teamData);

    void UpdateExpandContentState(bool isOpen, bool isMainUIState = true);
    void HideGameObject(bool b);
    void UpdateMissionUICell(IEnumerable<object> list);
    UniRx.IObservable<bool> GetTeamBtnClick { get; }
}
public partial class MainUIExpandContentViewController
{
    public enum ExpandContentType
    {
        nothing = 0,
        mission = 1,
        team = 2
    }

    public enum OperateTeamType
    {
        NONE = 0,
        LEAVETEAM = 1,
        BACKTEAM = 2,
        CHECKINFO = 3,
        LEADBYOTHER = 4,
        PLEASELEAVE = 5,
        SUMMON = 6,
        LEAVELITTLE = 7,
        APPLYLEADTEAM = 8,
        SAMERIDING = 9,
        RIDEOFF = 10,
        KICKRIDING = 11,
    }

    OperateTeamType operateTeamType = OperateTeamType.NONE;
    private const string OPERATE_ITEM = "OperateBtnItem";

    private ExpandContentType _expandContentType = ExpandContentType.nothing;

    private CompositeDisposable _disposable = new CompositeDisposable();
    private List<ExpandContentTeamItemController> _teamItemList = new List<ExpandContentTeamItemController>(5);
    private List<IMissionUICellController> _taskCellList = new List<IMissionUICellController>();

    //记录最近一次玩家操作任务面板的状态值
    private bool _lastOpExpandState = false;

    //主界面上右侧面板滑动状态
    private bool _mainUIExpandState = true;

    //主界面上右侧栏是否选中组队栏
    private bool _isSelectTeamState = false;

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        UICamera.onClick += ClickEventHandler;
    }

    private Subject<bool> _onTeamBtnClick = new Subject<bool>();
    public UniRx.IObservable<bool> GetTeamBtnClick { get { return _onTeamBtnClick; } }  

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        _disposable.Add(TeamDataMgr.Stream.SubscribeAndFire(data =>
        {
            //重登录后 Data拿到为空
            if(data != null)
            _view.TeamRequestBtn.gameObject.SetActive(data.GetApplicationCnt() > 0);
        }));
        _disposable.Add(OnteamMsgBtn_UIButtonClick.Subscribe(_ =>
        {
            _onTeamBtnClick.OnNext(_lastOpExpandState);
            UpdateExpandContentState(_lastOpExpandState);
        }));
        _disposable.Add(OnTabBtn_Task_UIButtonClick.Subscribe(_ => { OnTaskBtnClick(); }));
        _disposable.Add(OnTabBtn_Team_UIButtonClick.Subscribe(_ => { OnTeamBtnClick(); }));
    }

    protected override void OnDispose()
    {
        UICamera.onClick -= ClickEventHandler;
        _teamItemList.Clear();
        _taskCellList.Clear();
        _disposable = _disposable.CloseOnceNull();
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {

    }
    #region 任务跟踪面板
    public void UpdateMissionUICell(IEnumerable<object> allList)
    {
        if (allList == null) return;
        int poolCount = _taskCellList.Count;
        int index = 0;
        
        allList.ForEach(e =>
        {
            if (index < poolCount)
            {
                _taskCellList[index].ShowView();
                _taskCellList[index].UpdateView(e);
            }
            else
            {
                IMissionUICellController ctrl = AddChild<MissionUICellController, MissionUICell>(
                        View.MissionTable_UITable.gameObject,
                        MissionUICell.NAME
                    );
                ctrl.UpdateView(e);
                _taskCellList.Add(ctrl);
                _disposable.Add(ctrl.MissionCell_UIButtonClick.Subscribe(_ => OnMissionCellClick(ctrl.Obj)));
            }
            index++;
        });
        for (; index < poolCount; index++)
        {
            _taskCellList[index].HideView();
        }
        View.MissionTable_UITable.Reposition();
        SpringPanel.Begin(View.itemScrollView_UIScrollView.gameObject, new Vector3(-117f, -119f, 0f),8);
    }
    
    private void OnMissionCellClick(object obj)
    {
        if (ModelManager.Player.IsPlayerBandMode()) return;
        if (TowerDataMgr.DataMgr.IsInTowerAndCheckLeave(delegate
         {
             FindToMis(obj);
         })) return;

        FindToMis(obj);
    }

    private void FindToMis(object obj)
    {
        if (obj is Mission)
        {
            Mission tMission = obj as Mission;
            MissionDataMgr.DataMgr.FindToMissionNpcByMission(tMission, true);
        }
    }
    #endregion
    

    //用特定接口类型数据刷新界面
    public void UpdateView(UIMode mode)
    {
        var state = UIMode.GAME == mode && _lastOpExpandState;
        var mis = UIMode.GAME == mode && !MissionDataMgr.DataMgr.BodyHasMission();
        UpdateExpandContentState(state && mis);
    }

    private IExpandTeamData _data = null;

    public void UpdateView(ITeamData teamData)
    {
        _data = teamData != null ? teamData.ExpandTeamViewData : null;
        if (_data == null || !_data.HasTeam())
        {
            UpdataHasNoTeam();
        }
        else
        {
            UpdateTeamItemView(_data);
            UpdateTeamGroupBtnState(_data.HasTeam(), _data.IsLeader());
        }
    }

    public void UpdateExpandContentState(bool isOpen, bool isMainUIState = true)
    {
        _mainUIExpandState = isMainUIState;
        ToggleContentRoot(isOpen);
        
        _view.MissionTable_UITable.gameObject.SetActive(!_isSelectTeamState);
        _view.TeamGrid_UIGrid.gameObject.SetActive(_isSelectTeamState && _data.HasTeam());
        _view.teamBtnGroup.SetActive(_isSelectTeamState);
    }

    private void ToggleContentRoot(bool isOpen)
    {
        if (!_mainUIExpandState)
        {
            View.TabContentRoot_TweenPosition.Play(false);  //false收起,true展开
            _view.Arrow_UISprite.flip = UIBasicSprite.Flip.Horizontally;
            return;
        }

        _lastOpExpandState = !isOpen;
        View.TabContentRoot_TweenPosition.Play(!isOpen);  //false收起,true展开
        _view.Arrow_UISprite.flip = isOpen ?
            UIBasicSprite.Flip.Horizontally : UIBasicSprite.Flip.Vertically;

        ToggleMissionEffect(isOpen);
    }

    private void ChangeTab(bool b)
    {
        SpringPanel.Begin(View.itemScrollView_UIScrollView.gameObject, new Vector3(-117f, -119f, 0f), 8);
        _view.MissionTable_UITable.gameObject.SetActive(b);
        _view.TeamGrid_UIGrid.gameObject.SetActive(!b && _data.HasTeam());
        _view.teamBtnGroup.SetActive(!b);

        _view.MissonSelect.SetActive(b);
        _view.MissonLb_UILabel.text = b ? "任务".WrapColor("515048") : "任务".WrapColor("ffffd6");

        _view.TeamSelect.SetActive(!b);
        _view.TeamLb_UILabel.text = !b ? "组队".WrapColor("515048") : "组队".WrapColor("ffffd6");

        _isSelectTeamState = !b;
    }

    private void OnTaskBtnClick()
    {
        if(!_isSelectTeamState)
            ProxyMission.OpenMissionMainUI();

        ChangeTab(true);
    }

    private void OnTeamBtnClick()
    {
        if (_isSelectTeamState)
            ProxyTeamModule.OpenMainView();

        ChangeTab(false);
    }

    private void ToggleMissionEffect(bool isOpen)
    {
        // todo fish
//        if (_missionCellEffectDic.Count > 0)
//        {
//            foreach (int tKey in _missionCellEffectDic.Keys)
//            {
//                if (tKey == MissionType.MissionTypeEnum_Guide)
//                {
//                    foreach (MissionCellController tSurroundUIEffect in _missionCellEffectDic[tKey].Values)
//                    {
//                        tSurroundUIEffect.SetEffectActive(isOpen);
//                    }
//                }
//                else
//                {
//                    if (_missionCellEffectDic[tKey][0] != null)
//                    {
//                        _missionCellEffectDic[tKey][0].SetEffectActive(isOpen);
//                    }
//                    else
//                    {
//                        Dictionary<int, MissionCellController> tSurroundUIEffectDic = _missionCellEffectDic[tKey];
//                        foreach (int key in tSurroundUIEffectDic.Keys)
//                        {
//                            GameDebuger.OrangeDebugLog(string.Format("当前key的值：{0}", key));
//                        }
//                    }
//                }
//            }
//        }
    }

    ///目前最多4种情况
    private Dictionary<OperateTeamType, string> operateTeamDic = new Dictionary<OperateTeamType, string>();
    private List<OperateBtnItemController> _operateBtnItemList = new List<OperateBtnItemController>(4);

    public void OnSelectTeamItem(int index)
    {
        if (_data == null) return;

        View.directSprite_UISprite.gameObject.SetActive(true);
        var OnclickMember = _data.TeamMembers.Find(m=>m.index == index);   ///点击的头像的信息

//        if (_selectTeamMemberId == OnclickMember.id)
//        {
//            PlayerView playerView = WorldManager.DataMgr.GetView().GetPlayerView(TeamDataMgr.DataMgr.leaderPlayerId);
//            if (playerView != null && playerView.HasSameRidingMount())
//            {
//                // 当前队长拥有坐骑是共乘坐骑,着重新刷新菜单,因为状态控制太麻烦了 by willson
//            }
//            else
//            {
//                return;
//            }
//        }

        // 重新设置菜单
        operateTeamDic.Clear();

        //为队长时
        if (_data.IsLeader())
        {
            if (OnclickMember.id != _data.LeaderID)
            {
                //点击其他队员时
                if (OnclickMember.memberStatus == (int)TeamMemberDto.TeamMemberStatus.Away)  ///暂离
                {
                    operateTeamDic.Add(OperateTeamType.SUMMON, "召唤队员");
                    operateTeamDic.Add(OperateTeamType.CHECKINFO, "查看信息");
                    operateTeamDic.Add(OperateTeamType.PLEASELEAVE, "请离队伍");
                }
                else if (OnclickMember.memberStatus == (int)TeamMemberDto.TeamMemberStatus.Offline)  ///离线
                {
                    operateTeamDic.Add(OperateTeamType.CHECKINFO, "查看信息");
                    operateTeamDic.Add(OperateTeamType.PLEASELEAVE, "请离队伍");
                }
                else  ///在线
                {
                    operateTeamDic.Add(OperateTeamType.CHECKINFO, "查看信息");
                    operateTeamDic.Add(OperateTeamType.LEADBYOTHER, "移交队长");
                    operateTeamDic.Add(OperateTeamType.PLEASELEAVE, "请离队伍");

//                    if (ModelManager.Team.IsOnTeamLeaderRide(OnclickMember.playerId))
//                    {
//                        // 踢人
//                        operateTeamDic.Add(OperateTeamType.KICKRIDING, "解除共乘");
//                    }
//                    else
//                    {
//                        if (ModelManager.Mount.CanSameRiding())
//                        {
//                            operateTeamDic.Add(OperateTeamType.SAMERIDING, "邀请共乘");
//                        }
//                    }
                }
            }
            else
            {
                operateTeamDic.Add(OperateTeamType.LEAVETEAM, "退出队伍");
            }
        }
        ///暂离时
        else if (ModelManager.IPlayer.GetPlayerId() == OnclickMember.id)
        {
            if (OnclickMember.memberStatus == (int) TeamMemberDto.TeamMemberStatus.Away)
            {
                operateTeamDic.Add(OperateTeamType.BACKTEAM, "回归队伍");
            }
            else
            {
                operateTeamDic.Add(OperateTeamType.LEAVELITTLE, "暂时离队");
            }
            operateTeamDic.Add(OperateTeamType.LEAVETEAM, "退出队伍");
        }
        else
        {
            operateTeamDic.Add(OperateTeamType.CHECKINFO, "查看信息");
        }
            // 共乘状态下
//            if (ModelManager.Team.IsOnTeamLeaderRide())
//            {
//                operateTeamDic.Add(OperateTeamType.RIDEOFF, "不再共乘");
//            }

        List<OperateTeamType> typesList = new List<OperateTeamType>(operateTeamDic.Keys);
        if (_operateBtnItemList.Count < operateTeamDic.Count)
        {
            for (int i = _operateBtnItemList.Count; i < operateTeamDic.Count; i++)
            {
                var con = AddChild<OperateBtnItemController, OperateBtnItem>(
                    View.operateBtnGrid_UIGrid.gameObject
                    , OPERATE_ITEM);
                _operateBtnItemList.Add(con);
            }
        }
        else
        {
            ///多余的隐藏
            for (int i = operateTeamDic.Count; i < _operateBtnItemList.Count; i++)
            {
                _operateBtnItemList[i].gameObject.SetActive(false);
            }
        }

        ///现有的数据Update
        for (int i = 0; i < operateTeamDic.Count; i++)
        {
            _operateBtnItemList[i].Open(_data.GetMainRoleTeamMemberDto(), OnclickMember, OnClickOperateItemEvent);
            _operateBtnItemList[i].SetTypeItem(typesList[i]);
            _operateBtnItemList[i].SetLabel(operateTeamDic[typesList[i]]);
            //_operateBtnItemList[i].gameObject.name = operateTeamDic[typesList[i]];
            _operateBtnItemList[i].gameObject.SetActive(true);
        }

        Vector3 v3 = View.directSprite_UISprite.transform.localPosition;
        v3 = new Vector3(v3.x, 0 - index * 64, v3.z);
        View.directSprite_UISprite.transform.localPosition = v3;

        View.itemOperateBtnBg_UISprite.height = 75 + (operateTeamDic.Count - 1) * 63;

        View.operateBtnGrid_UIGrid.Reposition();
    }

    private void UpdataHasNoTeam()
    {
        View.TeamGrid_UIGrid.gameObject.SetActive(false);
        UpdateTeamGroupBtnState(false);
    }

    private void UpdateTeamItemView(IExpandTeamData data)
    {
        _view.TeamGrid_UIGrid.gameObject.SetActive(_isSelectTeamState);
        var teamMemberInfo = data.TeamMembers;
        var cnt = data.GetMemberCount();
        if (_teamItemList.Count < cnt)
        {
            for (int i = _teamItemList.Count; i < cnt; ++i)
            {
                var ctrl = AddCachedChild<ExpandContentTeamItemController, ExpandContentTeamItem>(
                    View.TeamGrid_UIGrid.gameObject, ExpandContentTeamItem.NAME);
                var idx = i;
                var d =  ctrl.OnExpandContentTeamItem_UIButtonClick.Subscribe(_ => OnSelectTeamItem(idx));
                _disposable.Add(d);
                _teamItemList.Add(ctrl);
            }
        }

        _teamItemList.ForEachI(delegate(ExpandContentTeamItemController item, int i)
        {
            var dto = teamMemberInfo.Find(m=>m.index == i);
            item.UpdateView(dto);
        });

        View.TeamGrid_UIGrid.Reposition();
    }

    /// <summary>
    /// 更新主界面队伍界面
    /// </summary>
    private void UpdateTeamGroupBtnState(bool hasteam, bool isLeader = false)
    {
        View.directSprite_UISprite.gameObject.SetActive(false);
        View.cancelMatchBtn_UIButton.gameObject.SetActive(false);

        if (hasteam)
        {
            if (isLeader)
                View.callOrAwayBtnLbl_UILabel.text = "召回";
            else
            {
                var statue = TeamDataMgr.DataMgr.GetTeamberStatueById(ModelManager.Player.GetPlayerId());
                View.callOrAwayBtnLbl_UILabel.text = statue == TeamMemberDto.TeamMemberStatus.Away ? "归队" : "暂离";
            }
            
            View.joinOrLeaveBtnLbl_UILabel.text = "离队";
            View.contentBgLbl_UILabel.cachedGameObject.SetActive(false);
        }
        else
        {
            View.callOrAwayBtnLbl_UILabel.text = "创建";
            View.joinOrLeaveBtnLbl_UILabel.text = "寻队";

            View.contentBgLbl_UILabel.text = "点击创建或加入队伍";
            View.contentBgLbl_UILabel.cachedGameObject.SetActive(true);
        }
    }

    private void OnClickOperateItemEvent(OperateTeamType type, TeamMemberDto playerDto, TeamMemberDto onClickDto)
    {
        GameDebuger.Log(type + "操作ID" + playerDto.id + "被点击ID" + onClickDto.id);
        switch (type)
        {
            case OperateTeamType.LEAVETEAM:
                TeamNetMsg.LeaveTeam();
                break;
            case OperateTeamType.LEAVELITTLE:
                TeamNetMsg.AwayTeam();
                break;
            case OperateTeamType.BACKTEAM:
                TeamNetMsg.BackTeam();
                break;
            /// case OperateTeamType.APPLYLEADTEAM: break;  ///申请带队暂时先不做
            case OperateTeamType.LEADBYOTHER:
                TeamNetMsg.AssignLeader(onClickDto);
                break;
            case OperateTeamType.CHECKINFO:
                ProxyMainUI.OpenPlayerInfoView(onClickDto.id, Vector3.zero);
                break;
            case OperateTeamType.PLEASELEAVE:
                TeamNetMsg.KickOutMember(onClickDto);
                break;
            case OperateTeamType.SUMMON: ///召唤归队
                TeamNetMsg.SummonAwayTeamMember(onClickDto.id);
                break;
//            case OperateTeamType.SAMERIDING: // 邀请共乘
//                TeamNetMsg.InviteRide(onClickDto.playerId);
//                break;
//            case OperateTeamType.RIDEOFF: // 下车
//                ModelManager.Team.RideOff();
//                break;
//            case OperateTeamType.KICKRIDING:
//                ModelManager.Team.KickRide(onClickDto.playerId);
//                break;
                default:break;
        }
        View.directSprite_UISprite.gameObject.SetActive(false);
    }

    public void ClickEventHandler(GameObject clickObj)
    {
        if (!View.directSprite_UISprite.gameObject.activeSelf)
            return;
        Transform _gridTransform = View.operateBtnGrid_UIGrid.transform;
        int operateBtnNum = _gridTransform.childCount;
        UIPanel panel = UIPanel.Find(clickObj.transform);
        if (operateBtnNum > 0)
        {
            if (panel != _view.itemOperateGroup_UIPanel)
            {
                while (operateBtnNum > 0)
                {
                    if (clickObj == _gridTransform.GetChild(operateBtnNum - 1).gameObject)
                        return;
                    operateBtnNum--;
                }
                View.directSprite_UISprite.gameObject.SetActive(false);
            }
        }
    }

    public void HideTeamRequestBtn()
    {
        _view.TeamRequestBtn.gameObject.SetActive(false);
    }

    public void HideGameObject(bool b)
    {
        gameObject.SetActive(b);
    }
}
