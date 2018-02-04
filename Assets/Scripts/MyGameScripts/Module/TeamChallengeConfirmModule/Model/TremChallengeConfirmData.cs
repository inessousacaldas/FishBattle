// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : DM-PC092
// Created  : 1/22/2018 3:17:47 PM
// **********************************************************************

using AppDto;
using AppServices;
using System;
using System.Collections.Generic;

public interface ITremChallengeConfirmData
{
    string GetTitleTex();
    IEnumerable<long> GetneedConfirmIds();
    long GetPlayer();
    ServerType GetServerType();
    Dictionary<long,int> GetTremmembers();
}


public sealed partial class TremChallengeConfirmDataMgr
{
    public sealed partial class TremChallengeConfirmData:ITremChallengeConfirmData
    {
        private string titleTex;
        private List<long> needConfirmIds = new List<long>();
        //0 - 未知 1 - 确定 2 - 拒绝
        private Dictionary<long,int> mTremmembers = new Dictionary<long, int>();
        //需要改变状态的队员
        private long playerID;
        private bool playerstate;
        public ServerType mServerType = ServerType.None;
        public void InitData()
        {

        }

        public void Dispose()
        {

        }

        /// <summary>
        /// 接受组队消息通知
        /// </summary>
        /// <param name="tTeamChallengeConfirmNotify"></param>
        public void TeamChallengeConfirmNotify(TeamChallengeConfirmNotify tTeamChallengeConfirmNotify)
        {
            TeamChallengeConfirmViewLogic.OnClose();
            titleTex = tTeamChallengeConfirmNotify.targerText;
            needConfirmIds = SortList(tTeamChallengeConfirmNotify.needConfirmIds);
            TeamDto tTeamDto = TeamDataMgr.DataMgr.GetSelfTeamDto();
            for(int i = 0; i< needConfirmIds.Count;i++) {
                TeamMemberDto tTeamMemberDto = tTeamDto.members.Find(e => e.id == needConfirmIds[i]);
                if(tTeamMemberDto.memberStatus == (int)TeamMemberDto.TeamMemberStatus.Leader)
                {
                    mTremmembers.Add(tTeamMemberDto.id,1);
                }
                else {
                    mTremmembers.Add(tTeamMemberDto.id,0);
                }
            }
            mServerType = ServerType.Init;
            TeamChallengeConfirmViewLogic.Open();
        }


        public void TeamMemberConfirmNotify(TeamMemberConfirmNotify tTeamMemberConfirmNotify)
        {
            playerID = tTeamMemberConfirmNotify.playerId;
            playerstate = tTeamMemberConfirmNotify.accept;
            if(mTremmembers.ContainsKey(playerID)) {
                if(playerstate) {
                    mTremmembers[playerID] = 1;
                }
                else {
                    mTremmembers[playerID] = 2;
                }
            }
            mServerType = ServerType.UpDate;
            FireData();
            mServerType = ServerType.None;
        }

        public string GetTitleTex()
        {
            return titleTex;
        }

        public IEnumerable<long> GetneedConfirmIds()
        {
            return needConfirmIds;
        }

        /// <summary>
        /// 获得最新改变那个
        /// </summary>
        /// <returns></returns>
        public long GetPlayer() {
            return playerID;
        }

        public ServerType GetServerType()
        {
            return mServerType;
        }

        public Dictionary<long,int> GetTremmembers() {
            return mTremmembers;
        }

        public void OnCloseData() {
            playerID = 0;
            mTremmembers.Clear();
            needConfirmIds.Clear();
            mServerType = ServerType.None;
            titleTex = string.Empty;
        }


        //排序方法
        List<long> SortList(List<long> tNeetSortList) {
            List<long> needConfirmIds = new List<long>();
            needConfirmIds.Add(TeamDataMgr.DataMgr.GetSelfTeamDto().leaderPlayerId);
            tNeetSortList.ForEach(e => {
                if(e != TeamDataMgr.DataMgr.GetSelfTeamDto().leaderPlayerId)
                    needConfirmIds.Add(e);
            });
            return needConfirmIds;
        }


        /// <summary>
        /// 需要同步任务弹窗
        /// </summary>
        public void CopySyncConfirmPanel(CopySyncConfirmNotify tCopySyncConfirmNotify) {
            if(tCopySyncConfirmNotify.type == (int)CopySyncConfirmNotify.ConfirmTypeEnum.Request) {
                //System.Text.StringBuilder strBuilder = new System.Text.StringBuilder();
                //TeamDto tTeamDto = TeamDataMgr.DataMgr.GetSelfTeamDto();
                //for(int i = 0;i < tCopySyncConfirmNotify.playerId.Count;i++)
                //{
                //    TeamMemberDto tTeamMemberDto = tTeamDto.members.Find(e => e.id == needConfirmIds[i]);
                //    strBuilder.Append(tTeamMemberDto.nickname.WrapColor(ColorConstantV3.Color_ChatNameBlue));
                //    strBuilder.Append(",");

                //}
                //if(strBuilder.Length > 0)
                //    strBuilder.Remove(strBuilder.Length - 1,1);
                long tplayerId = ModelManager.Player.GetPlayerId();
                if(tCopySyncConfirmNotify.playerId.Contains(tplayerId)) {
                    var controller = ProxyBaseWinModule.Open();
                    var title = "同步副本任务";
                    var txt = "进入副本需要同步副本任务，是否同步？";
                    BaseTipData data = BaseTipData.Create(title, txt, 30, () =>
                    {
                        //防止关闭的时候再发一次协议
                        GameUtil.GeneralReq(Services.Copy_SyncConfirm(tCopySyncConfirmNotify.copyId,true),e =>
                        {
                        });
                    },
                    ()=>
                    {
                        GameUtil.GeneralReq(Services.Copy_SyncConfirm(tCopySyncConfirmNotify.copyId,false),e =>
                        {
                        });
                    }, "否", "是");
                    //controller.SetCloseAction = () =>
                    //{
                    //    GameUtil.GeneralReq(Services.Copy_SyncConfirm(tCopySyncConfirmNotify.copyId,false),e =>
                    //    {
                    //    });
                    //};
                    Action CloseAction = () =>
                    {
                        GameUtil.GeneralReq(Services.Copy_SyncConfirm(tCopySyncConfirmNotify.copyId,false),e =>
                        {
                        });
                    };
                    controller.InitView(data,false,CloseAction);
                }
                if(TeamDataMgr.DataMgr.IsLeader())
                {
                    TipManager.AddTip("等待队员确定副本进度");
                }
            }
            else if(tCopySyncConfirmNotify.type == (int)CopySyncConfirmNotify.ConfirmTypeEnum.Respond) {
                if(!tCopySyncConfirmNotify.accept) {
                    TeamDto tTeamDto = TeamDataMgr.DataMgr.GetSelfTeamDto();
                    if(tCopySyncConfirmNotify.playerId.Count > 0)
                    {
                        TeamMemberDto tTeamMemberDto  = tTeamDto.members.Find(e =>  e.id == tCopySyncConfirmNotify.playerId[0]);
                        TipManager.AddTip(string.Format("{0}拒绝同步",tTeamMemberDto.nickname));
                    }
                    else {
                        GameDebuger.LogError("队员拒绝没有队员数据");
                    }
                }
            }
        }
    }
}
