// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  TeamChallengeConfirmViewController.cs
// Author   : DM-PC092
// Created  : 1/22/2018 3:17:47 PM
// Porpuse  : 
// **********************************************************************

using AppDto;
using System.Collections.Generic;
using UniRx;

public partial interface ITeamChallengeConfirmViewController
{

}
public partial class TeamChallengeConfirmViewController {
    private string timeName = " TeamChallengeConfirm";
    private int timer = 30;
    private Dictionary<long,TeamConfirmHeadController> tTeamConfirmHeadControllerDic = new Dictionary<long, TeamConfirmHeadController>();
    private System.IDisposable _disposables;
    private bool isSendServer = true;
    //保留第一打开界面的队伍人数
    private int TeamCount =0;
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView()
    {
        //队长默认同意
        if(TeamDataMgr.DataMgr.IsLeader())
        {
            View.SurelBtn_UIButton.gameObject.SetActive(false);
            View.CanelBtn_UIButton.gameObject.SetActive(false);
        }
        timer = 32;
        JSTimer.Instance.CancelTimer(timeName);
        JSTimer.Instance.SetupTimer(timeName,() =>
        {
            timer--;
            View.TimerLabel_UILabel.text = "倒计时 " + (timer - 2).ToString();
            if(timer <= 2 && timer >= 0) {
                View.TimerLabel_UILabel.gameObject.SetActive(false);
                if(!TeamDataMgr.DataMgr.IsLeader() && isSendServer)
                {
                    JSTimer.Instance.CancelTimer(timeName);
                    TremChallengeConfirmDataMgr.TremChallengeConfirmNetMsg.Team_PlayerConfirm(false);
                }
                //如果超过30自己还没有确定，就自动拒绝，如果已经确定了就是最有连接超时，自动关闭界面
            }
            else if(timer <= 0) {
                View.TimerLabel_UILabel.gameObject.SetActive(false);
                JSTimer.Instance.CancelTimer(timeName);
                TipManager.AddTip("有队友断线，需要重新确认");
                ProxyTremChallengeConfirm.Close();
            }
        },1f);
    }

    // 客户端自定义代码
    protected override void RegistCustomEvent()
    {
        _disposables = TeamDataMgr.Stream.Select((data) => data.GetTeamMember).Subscribe(stateTuple => TeamStatus(stateTuple));
    }

    protected override void RemoveCustomEvent()
    {
    }

    protected override void OnDispose()
    {
        base.OnDispose();
        TeamCount = 0;
        JSTimer.Instance.CancelCd("CloseTeamChallengeUITimr");
        JSTimer.Instance.CancelTimer(timeName);
        tTeamConfirmHeadControllerDic.ForEach(e => { e.Value.OnClose(); });
        tTeamConfirmHeadControllerDic.Clear();
        isSendServer = true;
        _disposables.Dispose();
    }

    //在打开界面之前，初始化数据
    protected override void InitData()
    {

    }

    // 业务逻辑数据刷新
    protected override void UpdateDataAndView(ITremChallengeConfirmData data) {
        if(data.GetServerType() == ServerType.Init)
            SetInfoData(data.GetTitleTex(),data.GetneedConfirmIds());
        else if(data.GetServerType() == ServerType.UpDate)
            SetPlayerState(data.GetPlayer(),data.GetTremmembers());
        else
            GameDebuger.LogError("未知状态:" + data.GetPlayer());
    }


    void SetInfoData(string title,IEnumerable<long> GetMissionCellData) {
        View.TitleLabel_UILabel.text = title;
        TeamDto tTeamDto = TeamDataMgr.DataMgr.GetSelfTeamDto();
        TeamCount = tTeamDto.members.Count;
        List<long> MissionCellData = GetMissionCellData.ToList();
        for(int i = 0;i < MissionCellData.Count;i++) {
            TeamMemberDto tTeamMemberDto = tTeamDto.members.Find(e => e.id == MissionCellData[i]);
            TeamConfirmHeadController _cell = AddCachedChild<TeamConfirmHeadController,TeamConfirmHeadView>(_view.Grid_UIGrid.gameObject,TeamConfirmHeadView.NAME);
            _cell.Init(tTeamMemberDto.playerDressInfo.charactor.texture.ToString(),tTeamMemberDto.nickname);
            //队长默认要显示钩钩
            if(tTeamDto.leaderPlayerId == tTeamMemberDto.id)
                _cell.SetStaue("Anser_Right");
            tTeamConfirmHeadControllerDic.Add(tTeamMemberDto.id,_cell);
        }
        _view.Grid_UIGrid.enabled = true;
    }

    void SetPlayerState(long playerID,Dictionary<long,int> Tremmembers) {
        int isClose = 0;
        Tremmembers.ForEach(e => {
            if(tTeamConfirmHeadControllerDic.ContainsKey(e.Key)) {
                string iconName="";
                if(e.Value == 1)
                {
                    iconName = "Anser_Right";
                    isClose++;
                }
                else if(e.Value == 2)
                {
                    iconName = "Anser_wrong";
                    TipManager.AddTip(string.Format("{0}拒绝进入副本",tTeamConfirmHeadControllerDic[e.Key].GetName()));
                    JSTimer.Instance.CancelCd("CloseTeamChallengeUITimr");
                    JSTimer.Instance.SetupCoolDown("CloseTeamChallengeUITimr",1,null,() => { ProxyTremChallengeConfirm.Close(); });
                }
                else {

                }
                tTeamConfirmHeadControllerDic[e.Key].SetStaue(iconName);
            }
        });
        if(ModelManager.Player.GetPlayerId() == playerID)
        {
            isSendServer = false;
            View.SurelBtn_UIButton.gameObject.SetActive(false);
            View.CanelBtn_UIButton.gameObject.SetActive(false);
        }
        if(isClose >= tTeamConfirmHeadControllerDic.Count) {
            ProxyTremChallengeConfirm.Close();
        }
    }


    /// <summary>
    /// 队伍状态发生改变的时候
    /// </summary>
    private void TeamStatus(IEnumerable<TeamMemberDto> tITeamData) {
        List<TeamMemberDto> tITeamDataList = tITeamData.ToList();
        if(TeamCount != tITeamDataList.Count)
            ProxyTremChallengeConfirm.Close();
        else {
            tITeamDataList.ForEach(e =>
            {
                if(e.memberStatus != 1 && e.memberStatus != 2)
                {
                    ProxyTremChallengeConfirm.Close();
                }
                else {
                    //GameDebuger.LogError("组队状态:"+ e.memberStatus);
                }
            });
        }
    }
}
