// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  CreateTeamViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System.Collections.Generic;
using AppDto;
using UniRx;
using UnityEngine;

public partial class CreateTeamViewController
{
    private CompositeDisposable _disposable;

    private Dictionary<GameObject, TeamEasyGroupItemController> _memberItemList = null;
    private List<TeamDto> _teamsDto;
    
    private int _createTeamRefreshTime = 10;

    protected override void AfterInitView ()
    {
        _disposable = new CompositeDisposable();

        _memberItemList = new Dictionary<GameObject, TeamEasyGroupItemController>();

        InitMemberItem();
    }

    protected override void RegistCustomEvent ()
    {
        _view.MemberGrid_UIRecycledList.onUpdateItem = OnUpdateItemList;
    }

    protected override void OnDispose()
    {
        _disposable.Dispose();
        _disposable = null;
        
//        _teamsDto.Clear();
    }

    protected override void RemoveCustomEvent ()
    {
        
    }

    public void UpdateView(ITeamData data)
    {
        var teamData = data.TeamMainViewData;
        var hasTeam = teamData.HasTeam();
        var isLeader = teamData.IsLeader();
        var isAutoMatch = data.AutoBtnState;

        View.ApplyBtn_UIButton.gameObject.SetActive(hasTeam && isLeader);
        View.SpeechBtn_UIButton.gameObject.SetActive(hasTeam && isLeader);
        View.ApplicationBtn_UIButton.gameObject.SetActive(!hasTeam);
        View.CreateTeamBtn_UIButton.gameObject.SetActive(!hasTeam);
        View.LeaveBtn_UIButton.gameObject.SetActive(hasTeam);
        View.TargetLvLbl.gameObject.SetActive(hasTeam);
        View.CaptainBtn_UIButton.gameObject.SetActive(isLeader || !hasTeam);

        if (hasTeam)
        {
            var d = data.GetTeamDto.members.Find(s => s.id == ModelManager.Player.GetPlayerId());
            if (!isLeader)
            {
                View.AutoMatchBtn_UIButton.gameObject.SetActive(false);
                View.CancelBtn.gameObject.SetActive(false);
                View.BackTeamBtn_UIButton.gameObject.SetActive(d.memberStatus == (int)TeamMemberDto.TeamMemberStatus.Away);
                View.AwayBtn_UIButton.gameObject.SetActive(d.memberStatus != (int)TeamMemberDto.TeamMemberStatus.Away);
            }
            else
            {
                View.AwayBtn_UIButton.gameObject.SetActive(false);
                View.BackTeamBtn_UIButton.gameObject.SetActive(false);
                View.AutoMatchBtn_UIButton.gameObject.SetActive(!isAutoMatch);
                View.CancelBtn.gameObject.SetActive(isAutoMatch);
            }
        }
        else
        {
            View.AutoMatchBtn_UIButton.gameObject.SetActive(!isAutoMatch);
            View.CancelBtn.gameObject.SetActive(isAutoMatch);
            View.AwayBtn_UIButton.gameObject.SetActive(false);
            View.BackTeamBtn_UIButton.gameObject.SetActive(false);
        }

        _teamsDto = data.TeamMainViewData.GetCreatTeamsDto == null? null: data.TeamMainViewData.GetCreatTeamsDto.teams;
        View.MemberGrid_UIRecycledList.UpdateDataCount(_teamsDto == null? 0:_teamsDto.Count, true);

        View.MemberGroup_UIScrollView.UpdatePosition();
        UpdateTeamMatchLabel(teamData.GetMatchTargetData);
    }

    private void InitMemberItem()
    {
        for (int i = 0; i < TeamDataMgr.CreateTeamItemMax; ++i)
        {
            var controller = AddCachedChild<TeamEasyGroupItemController, TeamEasyGroupInfoItem>(
                _view.MemberGrid_UIRecycledList.gameObject
                , TeamEasyGroupInfoItem.NAME
                , "Item" + i);

            _disposable.Add(controller.OnItemClickEvent.Subscribe(dto =>
            {
                TeamDataMgr.TeamNetMsg.JoinTeam(dto.leaderPlayerId);
            }));

            _memberItemList.Add(controller.gameObject, controller);
        }
    }

    private void OnUpdateItemList(GameObject item, int itemIndex, int dataIndex)
    {
        if (_teamsDto == null) return;

        TeamEasyGroupItemController controller = null;
        _memberItemList.TryGetValue(item, out controller);

        if (controller == null) return;
        if (dataIndex >= _teamsDto.Count) return;

        controller.UpdateData(_teamsDto[dataIndex], dataIndex);
    }

    public void UpdateTeamMatchLabel(ITeamMatchTargetData data)
    {
        TeamActionTarget target = DataCache.getDtoByCls<TeamActionTarget>(data.GetActiveId);
        
        if (data.GetActiveId == 0)
        {
            _view.TargetInfoLbl_UILabel.text = "目标:全部";
            string _lv = string.Format("等级:{0}-{1}级", data.GetMinLv, data.GetMaxLv);
            _view.TargetLvLbl.text = _lv;
        }

        if (target != null)
        {
            string str = string.Format("目标:{0}", target.name);
            string lv = string.Format("等级:{0}-{1}级", data.GetMinLv, data.GetMaxLv);
            _view.TargetInfoLbl_UILabel.text = str;
            _view.TargetLvLbl.text = lv;
        }
        _view.GotoMissionBtn_UIButton.gameObject.SetActive(target != null && target.smartGuideId != -1);
    }
}
