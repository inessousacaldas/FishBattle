// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  TeamInfoTabContentViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using UnityEngine;
using System.Collections.Generic;
using System.Text;
using AppDto;
using UniRx;
using UniRx.Triggers;

public partial class TeamInfoTabContentViewController
{
    private CompositeDisposable _disposable;
    
    // 界面初始化完成之后的一些后续初始化工作
    private List<TeamPlayerInfoItemController> _memberItemList = null;
    private TeamMemberDto _curMemberDto;

    private ITeamMainViewData data = null;

    private int[] _posIdx = {0, 1, 2, 3};
    private const int ButtonHigth = 52;

    private List<Transform> _itemPosList = new List<Transform>(); 

    private Dictionary<int, Crew> _crewList = new Dictionary<int, Crew>();

    private Subject<Unit> _addPlayerEvt = new Subject<Unit>();
    public UniRx.IObservable<Unit> AddPlayerHandler { get { return _addPlayerEvt; } }

    private S3PopupListController teamPopupListCtrl;
    List<PopUpItemInfo> nameList = new List<PopUpItemInfo>();

    enum TeamPopupClickEnum
    {
        CheckInfo = 0,
        TranTeamLeader,
        KickOutMember,
        CallMember,
        SetCommander,
        CancelCommander,
        AddFriend,
        Message
    }

    private void InitData()
    {
        var list = DataCache.getDicByCls<GeneralCharactor>();
        list.ForEach(data =>
        {
            if(data.Value is Crew)
                _crewList.Add((data.Value as Crew).id, data.Value as Crew);
        });

        InitTeamPopupList();
    }

    private void TeamPopupListCtrl_OnClickOtherEvt()
    {
        teamPopupListCtrl.Hide();
    }

    private void InitTeamPopupList()
    {
        teamPopupListCtrl = AddChild<S3PopupListController, S3PopupList>(View.gameObject, S3PopupList.PREFAB_WAREHOUSE);
        teamPopupListCtrl.InitData(S3PopupItem.PREFAB_TEAMBTN, nameList, 
            isClickHide: false, isShowList:true, isShowBg:false);
        teamPopupListCtrl.Hide();
        teamPopupListCtrl.OnClickOtherEvt += TeamPopupListCtrl_OnClickOtherEvt;
        teamPopupListCtrl.OnChoiceIndexStream.Subscribe(item =>
        {
            switch ((TeamPopupClickEnum)item.EnumValue)
            {
                case TeamPopupClickEnum.CheckInfo:
                    ProxyMainUI.OpenPlayerInfoView(_curMemberDto.id, Vector3.zero);
                    break;
                case TeamPopupClickEnum.TranTeamLeader:
                    TeamDataMgr.TeamNetMsg.AssignLeader(_curMemberDto);
                    break;
                case TeamPopupClickEnum.KickOutMember:
                    TeamDataMgr.TeamNetMsg.KickOutMember(_curMemberDto);
                    break;
                case TeamPopupClickEnum.CallMember:
                    TeamDataMgr.TeamNetMsg.SummonAwayTeamMember(_curMemberDto.id);
                    break;
                case TeamPopupClickEnum.SetCommander:
                    TeamDataMgr.TeamNetMsg.SetCommander(_curMemberDto.id);
                    break;
                case TeamPopupClickEnum.CancelCommander:
                    TeamDataMgr.TeamNetMsg.SetCommander(_curMemberDto.id);
                    break;
                case TeamPopupClickEnum.AddFriend:
                    FriendDataMgr.FriendNetMsg.ReqAddFriend(_curMemberDto.id);
                    break;
                case TeamPopupClickEnum.Message:
                    break;
            }
            TeamPopupListCtrl_OnClickOtherEvt();
        });
    }

    protected override void AfterInitView ()
    {
        //队形按钮开放
        View.ArrayBtn_UIButton.gameObject.SetActive(FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum.FUN_11));

        _disposable = new CompositeDisposable();
        _memberItemList = new List<TeamPlayerInfoItemController>(TeamDataMgr.TeamDefaultMemberCnt);
        InitData();

        InitPosAnchor();
        InitMemberItem();

        View.SecondPanel.gameObject.SetActive(false);
    }

    private void InitPosAnchor()
    {

    }

    private void InitMemberItem()
    {
        for (var i = 0; i < TeamDataMgr.TeamDefaultMemberCnt; ++i)
        {
            var ctrl = AddChild<TeamPlayerInfoItemController, TeamPlayerInfoItem>(
                _view.memberGrid_UIGrid.gameObject
                , TeamPlayerInfoItem.NAME
                , "TeamMemberInfo_" + i);

            ctrl.EnableDrag = false;
            _disposable.Add(ctrl.OnTeamPlayerInfoItem_UIButtonClick.Subscribe(_ => OnSelectMemberInfoItem(ctrl.MemberDto, ctrl.Anchor)));
            _disposable.Add(ctrl.GetAddEvt.Subscribe(_=> _addPlayerEvt.OnNext(new Unit())));
            ctrl.ResetItem();
            ctrl.OnDragDropReleaseCallBack = OnDragDrogRelease;
            _memberItemList.Add(ctrl);
            _itemPosList.Add(ctrl.transform);
            var pos = ctrl.transform.localPosition;
            ctrl.transform.localPosition = new Vector3(i * 20f, pos.y, pos.z);
        }
        _view.memberGrid_UIGrid.Reposition();
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        UICamera.onClick += OnCameraClick;
    }

    protected override void OnDispose()
    {
        UICamera.onClick -= OnCameraClick;
        teamPopupListCtrl.OnClickOtherEvt -= TeamPopupListCtrl_OnClickOtherEvt;
        teamPopupListCtrl = null;
        _memberItemList.Clear();
        _disposable = _disposable.CloseOnceNull();
        base.OnDispose();
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    //用特定接口类型数据刷新界面
    public void UpdateView(ITeamData teamdata)
    {
        data = teamdata.TeamMainViewData;
        var hasTeam = data.HasTeam();
        var isLeader = data.IsLeader();
        var isAutoMatch = teamdata.AutoBtnState;

        View.ApplicationBtn_UIButton.gameObject.SetActive(isLeader);
        View.InviteBtn_UIButton.gameObject.SetActive(!hasTeam);
        View.CreateTeamBtn_UIButton.gameObject.SetActive(!hasTeam);
        View.LeaveBtn_UIButton.gameObject.SetActive(hasTeam);
        
        View.CaptainBtn_UIButton.gameObject.SetActive(isLeader || !hasTeam);
        View.CaptainIcon_UISprite.gameObject.SetActive(isLeader);
        View.targetLvLbl.gameObject.SetActive(hasTeam);
        
        
        if (hasTeam)
        {
            var d = teamdata.GetTeamDto.members.Find(s => s.id == ModelManager.Player.GetPlayerId());
            if (!isLeader)
            {
                View.AutoMatchBtn_UIButton.gameObject.SetActive(false);
                View.CanCelAutoBtn_UIButton.gameObject.SetActive(false);
                View.BackTeamBtn_UIButton.gameObject.SetActive(d.memberStatus == (int)TeamMemberDto.TeamMemberStatus.Away);
                View.AwayBtn_UIButton.gameObject.SetActive(d.memberStatus != (int)TeamMemberDto.TeamMemberStatus.Away);
            }
            else
            {
                View.AwayBtn_UIButton.gameObject.SetActive(false);
                View.BackTeamBtn_UIButton.gameObject.SetActive(false);
                View.AutoMatchBtn_UIButton.gameObject.SetActive(!isAutoMatch);
                View.CanCelAutoBtn_UIButton.gameObject.SetActive(isAutoMatch);
            }
        }
        else
        {
            View.AutoMatchBtn_UIButton.gameObject.SetActive(!isAutoMatch);
            View.CanCelAutoBtn_UIButton.gameObject.SetActive(isAutoMatch);
            View.AwayBtn_UIButton.gameObject.SetActive(false);
            View.BackTeamBtn_UIButton.gameObject.SetActive(false);
        }

        UpdateMemberGrid(teamdata);
        UpdateTeamMatchLabel(data.GetMatchTargetData);
        SetCurFormatiom(teamdata);
    }

    public void SetCurFormatiom(ITeamData teamdata)
    {
        string str = "";
        Formation formation;
        if (teamdata.TeamMainViewData.HasTeam())
        {
            formation = DataCache.getDtoByCls<Formation>(teamdata.GetTeamDto.formation.formationId);
            var show = formation.id != (int)Formation.FormationType.Regular;
            str = FormationHelper.GetFormationNameAndLevel(formation.name, teamdata.GetTeamDto.formation.level, show);
        }
        else
        {
            var data = teamdata.TeamMainViewData.CurFormationInfo;
            formation = DataCache.getDtoByCls<Formation>(data.formationId);
            var lv = data.formationId == (int) Formation.FormationType.Regular ? 0 : data.level;
            str = FormationHelper.GetFormationNameAndLevel(formation.name, lv);
        }
        UpdateFomatinInfo(str);
    }

    private void UpdateMemberGrid(ITeamData teamData)
    {
        if (View.memberGrid_UIGrid == null)
        {
            GameDebuger.LogError("Error: can not find the correct grid: View.memberGrid_UIGrid");
            return;
        }

        _memberItemList.ForEach(item =>
        {
            item.ResetItem();
        });
        var mainPos = -1;    //玩家位置
        if (teamData.GetTeamDto != null)
        {
            var memberDto = teamData.GetTeamDto.members;
            memberDto.ForEachI((dto, idx) =>
            {
                var pos = dto.position % TeamDataMgr.TeamDefaultMemberCnt;
                if (dto.id == teamData.GetTeamDto.leaderPlayerId)
                {
                    mainPos = pos;
                    if (dto.crewPositionNotifys.Count != 0)
                    {
                        dto.crewPositionNotifys.ForEach(d =>
                        {
                            var i = d.position%TeamDataMgr.TeamDefaultMemberCnt;
                            if (i != mainPos)
                            {
                                _memberItemList[i].EnableDrag = true;
                                _memberItemList[i].UpdateView(_crewList[d.crewId], d.grade, d.slotsElementLimit);
                            }
                            else
                            {
                                _memberItemList[mainPos].UpdateView(dto,
                                    TeamPlayerInfoItemController.MemberPos.Leader,
                                    teamData.TeamMainViewData.IsCommander(dto.id));
                                _memberItemList[mainPos].EnableDrag = true;
                            }
                        });
                    }
                    _memberItemList[mainPos].UpdateView(dto,
                                    TeamPlayerInfoItemController.MemberPos.Leader,
                                    teamData.TeamMainViewData.IsCommander(dto.id));
                    _memberItemList[mainPos].EnableDrag = true;
                }
                else
                {
                    _memberItemList[pos].EnableDrag = true;
                    _memberItemList[pos].UpdateView(dto, 
                        TeamPlayerInfoItemController.MemberPos.Member, teamData.TeamMainViewData.IsCommander(dto.id));
                }
                _memberItemList[pos].HideDragDropItem = teamData.TeamMainViewData.IsLeader();
            });
        }
        else
            SetCrewList(teamData);

        View.memberGrid_UIGrid.Reposition();
    }

    private void SetCrewList(ITeamData teamData)
    {
        var crewInfo = teamData.TeamMainViewData.CurFormationInfo.crewInfoDtos;
        var mainPos = -1;    //玩家位置
        crewInfo.ForEach(dto =>
        {
            var pos = dto.position % TeamDataMgr.TeamDefaultMemberCnt;
            _memberItemList[pos].EnableDrag = true;

            if (dto.crewId == ModelManager.Player.GetPlayerId())
            {
                var mainCrew = crewInfo.Find(d => d.position % TeamDataMgr.TeamDefaultMemberCnt == pos && d.crewId != ModelManager.Player.GetPlayerId());
                if (mainCrew == null)
                    _memberItemList[pos].UpdateView(ModelManager.Player.GetPlayer(), 0, dto.grade);
                else
                    _memberItemList[pos].UpdateView(ModelManager.Player.GetPlayer(),
                        mainCrew.crewSufaceId, dto.grade, mainCrew.quality, mainCrew.slotsElementLimit);


                mainPos = pos;
            }
            else if (pos != mainPos)
                _memberItemList[pos].UpdateView(_crewList[dto.crewSufaceId], dto.grade, dto.slotsElementLimit);
        });
    }

    private void OnSelectMemberInfoItem(TeamMemberDto memberDto, GameObject go)
    {
        if (memberDto == null
            || memberDto.id == ModelManager.Player.GetPlayerId())
            return;

        _curMemberDto = memberDto;
        nameList.Clear();

        var infoBtn = new PopUpItemInfo("查看信息", (int)TeamPopupClickEnum.CheckInfo);
        nameList.Add(infoBtn);

        if (memberDto.id != ModelManager.Player.GetPlayerId())
        {
            var MseeageBtn = new PopUpItemInfo("发送消息", (int) TeamPopupClickEnum.Message);
            nameList.Add(MseeageBtn);
        }

        if (!FriendDataMgr.DataMgr.IsMyFriend(memberDto.id))
        {
            var AddFriendBtn = new PopUpItemInfo("添加好友", (int) TeamPopupClickEnum.AddFriend);
            nameList.Add(AddFriendBtn);
        }

        if (data.IsLeader())
        {
            var ChangeLeaderBtn = new PopUpItemInfo("移交队长", (int)TeamPopupClickEnum.TranTeamLeader);
            nameList.Add(ChangeLeaderBtn);

            var kickOutMemberBtn = new PopUpItemInfo("请离队伍", (int) TeamPopupClickEnum.KickOutMember);
            nameList.Add(kickOutMemberBtn);

            var callbackBtn = new PopUpItemInfo("召唤归队", (int) TeamPopupClickEnum.CallMember);
            nameList.Add(callbackBtn);

            //if (TeamDataMgr.DataMgr.GetCommanderId() == memberDto.id)
            //{
            //    var CancelCommanderBtn = new PopUpItemInfo("取消指挥", (int) TeamPopupClickEnum.CancelCommander);
            //    nameList.Add(CancelCommanderBtn);
            //}
            //else
            //{
            //    var SetCommanderBtn = new PopUpItemInfo("委托指挥", (int) TeamPopupClickEnum.SetCommander);
            //    nameList.Add(SetCommanderBtn);
            //}
        }
        teamPopupListCtrl.UpdateView(nameList);
        teamPopupListCtrl.Show();
        teamPopupListCtrl.SetPos(go.transform.position);
        teamPopupListCtrl.SetListBgDimensions(126, ButtonHigth *nameList.Count + 5);
    }

    public void UpdateFomatinInfo(string str)
    {
        _view.ArrayLabel.text = str;
    }

    public void UpdateTeamMatchLabel(ITeamMatchTargetData data)
    {
        TeamActionTarget target = DataCache.getDtoByCls<TeamActionTarget>(data.GetActiveId);
        
        if (data.GetActiveId == 0)
        {
            _view.targetInfoLbl_UILabel.text = "目标:全部";
            string lv = string.Format("等级:{0}-{1}级", data.GetMinLv, data.GetMaxLv);
            _view.targetLvLbl.text = lv;
        }

        if (target != null)
        {
            string str = string.Format("目标:{0}", target.name);
            string lv = string.Format("等级:{0}-{1}级", data.GetMinLv, data.GetMaxLv);
            _view.targetInfoLbl_UILabel.text = str;
            _view.targetLvLbl.text = lv;
        }
        _view.GotoMissionBtn_UIButton.gameObject.SetActive(target != null && target.smartGuideId != -1);
    }

    #region 拖拽

    private void OnDragDrogRelease(
        TeamPlayerInfoItemController item, 
        GameObject go, 
        Vector3 pos)
    {


        //如果pos > go.localpos,则在go的右边,否则在左边
        int start = 0;
        int end = 0;
        _itemPosList.ForEachI((m, idx) =>
        {
            if (m.gameObject.name == item.gameObject.name)
                start = idx;

            if (pos.x > m.transform.localPosition.x)
            {
                end += 1;
            }
        });
       
        PosSort(start, end);
        StringBuilder str = new StringBuilder();
        _posIdx.ForEach(i =>
        {
            str.Append(i + ",");
        });

        _posIdx = new[] {0, 1, 2, 3};
        MemberSort();
        MemberListSort();
        if(data.HasTeam())
            TeamFormationDataMgr.TeamFormationNetMsg.Team_ExchangeColumn(str.ToString());
        else
            TeamDataMgr.TeamNetMsg.TeamMenuPosition(str.ToString());
    }

    private Comparison<Transform> _comparison = null;
    private void MemberSort()
    {
        if (_comparison == null)
        {
            _comparison = (a, b) =>
            {
                return a.localPosition.x.CompareTo(b.localPosition.x);
            };
        }
        _itemPosList.Sort(_comparison);
        
    }

    private Comparison<TeamPlayerInfoItemController> _comp = null;

    private void MemberListSort()
    {
        if (_comp == null)
        {
            _comp = (a, b) =>
            {
                return a.transform.localPosition.x.CompareTo(b.transform.localPosition.x);
            };
        }
        _memberItemList.Sort(_comp);
    }

    private void PosSort(int start, int end)
    {
        var endPos = _posIdx[start];
        //左移
        if (start > end)
        {
            _posIdx.ForEach(i =>
            {
                if (i <= start && i > end)
                    _posIdx[i] -= 1;
            });
        }  
        else if (start < end)
        {
            _posIdx.ForEach(i =>
            {
                if (i >= start && i < end)
                    _posIdx[i] += 1;
            });
        }
        _posIdx[end] = endPos;
    }
    #endregion

    private void OnCameraClick(GameObject go)
    {
        UIPanel panel = UIPanel.Find(go.transform);
        if (_view.SecondPanel.gameObject.activeSelf 
            && panel != _view.SecondPanel_UIPanel)
            _view.SecondPanel.gameObject.SetActive(false);
    }
}

