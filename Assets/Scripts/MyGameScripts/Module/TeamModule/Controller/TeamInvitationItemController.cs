// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  TeamInvitationItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System.Collections.Generic;
using AppDto;

public partial class TeamInvitationItemController
{
    private List<HeroHeadItemController> itemCtrlList = new List<HeroHeadItemController>();

    private int _dataIdx = -1;
    private HeroHeadItemController headCtrl;
    public int DataIdx
    {
        get { return _dataIdx; }
        set { _dataIdx = value; }
    }

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        View.Label_UILabel.text = "同意";
        headCtrl = AddCachedChild<HeroHeadItemController, HeroHeadItem>(
            View.leaderAnchor.gameObject
            , HeroHeadItem.NAME);
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {

    }

    protected override void OnDispose()
    {

    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    public void UpdateView(TeamInvitationNotify notify)
    {
        if (notify == null)
        {
            this.gameObject.SetActive(false);
            return;
        }
        
        this.gameObject.SetActive(true);
        View.targetName_UILabel.text = "";

        TeamMemberSimpleDto dto = null;
        notify.inviteTeamMembers.TryGetValue(0, out dto);
        if (dto != null)
        {
            View.leaderName_UILabel.text = dto.nickname;
            headCtrl.UpdateData(dto);
            View.lvLbl_UILabel.text = "Lv." + dto.grade;
            View.factionLbl_UILabel.text = dto.faction.name;
            UIHelper.SetFactionIcon(View.factionInfo_UISprite, dto.factionId);
        }
        else
        {
            GameLog.LogTeam("there is an error in data from server");
        }

    }

    public void UpdateView(TeamInvitationNotify notify, int dataindex)
    {
        if (notify == null)
        {            
            this.gameObject.SetActive(false);
            return;
        }

        this.gameObject.SetActive(true);
        _dataIdx = dataindex;
        
        var set = new List<TeamMemberSimpleDto>();
        set = notify.inviteTeamMembers.GetRange(1, notify.inviteTeamMembers.Count - 1);
     
        UpdateHeroGrid(set);
        UpdateView(notify);
    }

    private void UpdateHeroGrid(List<TeamMemberSimpleDto> data)
    {
        data.ForEachI(delegate(TeamMemberSimpleDto member, int idx)
        {
            HeroHeadItemController ctrl = null;
            itemCtrlList.TryGetValue(idx, out ctrl);
            if (ctrl == null)
            {
                ctrl = AddCachedChild<HeroHeadItemController, HeroHeadItem>(
                    View.itemGrid_UIGrid.gameObject
                    , HeroHeadItem.NAME);

                itemCtrlList.Add(ctrl);
            }
            ctrl.UpdateData(member);
        });

        for (var i = data.Count;i < itemCtrlList.Count;++ i)
        {
            var ctrl = itemCtrlList[i];
            ctrl.UpdateWithNullData();
        }

    }
}
