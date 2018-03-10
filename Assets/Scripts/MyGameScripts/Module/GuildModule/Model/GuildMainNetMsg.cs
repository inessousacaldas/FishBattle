// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : DM-PC092
// Created  : 1/10/2018 2:11:21 PM
// **********************************************************************

using AppDto;
using AppServices;

public sealed partial class GuildMainDataMgr
{
    public static class GuildMainNetMsg
    {
        private static string DELAYOPENGUILDVIEW = "DelayOpenGuildView";

        public static void ReqEnterGuildInfo(System.Action completeAction)
        {
            GameUtil.GeneralReq(Services.Guild_QueryByPlayer(false), e => RespEnterGuildInfo(e, completeAction));
        }

        private static void RespEnterGuildInfo(GeneralResponse e, System.Action completeAction)
        {
            if (e is GuildBaseInfoDto)
            {
                var dto = e as GuildBaseInfoDto;;
                DataMgr._data.UpdateGuildList(dto);
            }
            else
                DataMgr._data.SetguildBaseInfoNull();
            GameUtil.SafeRun(completeAction);
        }

        /// <summary>
        /// 请求公会列表
        /// </summary>
        /// <param name="pageIndex" 第几页></param>
        /// <param name="pageSize" 每页显示数量></param>
        public static void ReqGuildList(int pageIndex, int pageSize)
        {
            GameUtil.GeneralReq<GuildBaseInfoListDto>(Services.Guild_List(pageIndex, pageSize), RespGuildList);
        }

        private static void RespGuildList(GuildBaseInfoListDto dto)
        {
            DataMgr._data.UpdateGuildList(dto);
            FireData();
        }

        /// <summary>
        /// 请求公会数量
        /// </summary>
        public static void ReqGuildCount()
        {
            //GameUtil.GeneralReq<GuildCountDto>(Services.Guild_Count(), RespGuildCount);

            //后期需要删除掉
            FireData();
        }

        private static void RespGuildCount(GuildCountDto dto)
        {
            DataMgr._data.UpdateGuildCount(dto);
            FireData();
        }

        /// <summary>
        /// 钻石创建
        /// </summary>
        /// <param name="guildName" 公会名称></param>
        /// <param name="memoContent" 公会宣言></param>
        public static void ReqCreateByDiamond(string guildName, string memoContent)
        {
            GameUtil.GeneralReq<PlayerGuildInfoDto>(Services.Guild_CreateByDiamond(guildName, memoContent), RespCreateByDiamond);
        }
        private static void RespCreateByDiamond(PlayerGuildInfoDto resp)
        {
            DataMgr._data.UpdateGuildState(resp);
            GuildMainViewLogic.CloseNotJoinView();
            JSTimer.Instance.SetupCoolDown(DELAYOPENGUILDVIEW, 0.2f, null, () => ProxyGuildMain.OpenPanel());
        }

        /// <summary>
        /// 金币创建
        /// </summary>
        /// <param name="guildName" 公会名称></param>
        /// <param name="memoContent" 公会宣言></param>
        public static void ReqCreateByGold(string guildName, string memoContent)
        {
            GameUtil.GeneralReq<PlayerGuildInfoDto>(Services.Guild_CreateByGold(guildName, memoContent), RespCreateByGold);
        }
        private static void RespCreateByGold(PlayerGuildInfoDto resp)
        {
            DataMgr._data.UpdateGuildState(resp);
            GuildMainViewLogic.CloseNotJoinView();
            JSTimer.Instance.SetupCoolDown(DELAYOPENGUILDVIEW, 0.2f, null, () => ProxyGuildMain.OpenPanel());
        }

        /// <summary>
        /// 通过id请求公会信息
        /// </summary>
        /// <param name="showId"></param>
        /// <param name="isDetail"></param>
        public static void ReqGuildInfo(int showId,bool isDetail,System.Action completeAction = null)
        {
            if (isDetail)
                GameUtil.GeneralReq<GuildDetailInfoDto>(Services.Guild_QueryByShowId(showId, isDetail),e=> RespGuildInfo(e,completeAction));
            else
                GameUtil.GeneralReq<GuildBaseInfoDto>(Services.Guild_QueryByShowId(showId, isDetail), e => RespGuildInfo(e, completeAction));
        }

        /// <summary>
        /// 通过名字请求公会信息
        /// </summary>
        /// <param name="name"></param>
        /// <param name="isDetail"></param>
        public static void ReqGuildInfo(string name,bool isDetail, System.Action completeAction = null)
        {
            if (isDetail)
                GameUtil.GeneralReq<GuildDetailInfoDto>(Services.Guild_QueryByName(name, isDetail), e => RespGuildInfo(e, completeAction));
            else
                GameUtil.GeneralReq<GuildBaseInfoDto>(Services.Guild_QueryByName(name, isDetail), e => RespGuildInfo(e, completeAction));
        }

        private static void RespGuildInfo(GuildDetailInfoDto dto, System.Action completeAction = null)
        {
            DataMgr._data.UpdateGuildDetailInfo(dto);
            GameUtil.SafeRun(completeAction);
        }

        private static void RespGuildInfo(GuildBaseInfoDto dto, System.Action completeAction = null)
        {
            DataMgr._data.UpdateGuildList(dto);
            GameUtil.SafeRun(completeAction);
        }

        /// <summary>
        /// 申请加入公会
        /// </summary>
        /// <param name="showId"></param>
        public static void ReqRequstGuild(int showId,string guildName)
        {
            if (!GuildMainViewLogic.CanRequstGuild(showId)) return;
            GameUtil.GeneralReq(Services.Guild_ApplyJoin(showId),e=> RespRequstGuild(showId, guildName));
        }

        private static void RespRequstGuild(int showId,string guildName)
        {
            TipManager.AddTip("已成功申请加入" + guildName);
            DataMgr._data.RepeatRequstHandler(showId);
        }

        /// <summary>
        /// 一键申请
        /// </summary>
        public static void ReqBatchRequstGuild()
        {
            GameUtil.GeneralReq(Services.Guild_BatchApply(), null, () => RespBatchRequstGuild());
        }

        private static void RespBatchRequstGuild()
        {
            TipManager.AddTip("一键申请成功");
            DataMgr._data.RepeatRequstHandler(BatchId);
        }

        /// <summary>
        /// 申请列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        public static void ReqRequestList(int pageIndex,int pageSize)
        {
            GameUtil.GeneralReq<GuildApprovalListDto>(Services.Guild_ApplyList(pageIndex, pageSize), e => { RespRequestList(e, true); });
        }

        private static void RespRequestList(GuildApprovalListDto dto,bool showRequestView)
        {
            DataMgr._data.UpdateApprovalList(dto,showRequestView);
            ReqGuildMemList(DataMgr._data.reqMemPageIndex, ReqMemListCount);
        }
       
        /// <summary>
        /// 拒绝
        /// </summary>
        /// <param name="applyId"></param>
        public static void ReqRefuse(string applyId)
        {
            GameUtil.GeneralReq(Services.Guild_RefuseApply(applyId));
        }

        /// <summary>
        /// 接收入会
        /// </summary>
        /// <param name="applyId"></param>
        public static void ReqApproval(long applyId)
        {
            GameUtil.GeneralReq<GuildApprovalDto>(Services.Guild_Approval(applyId), RespApproval);
        }
        
        private static void RespApproval(GuildApprovalDto dto)
        {
            DataMgr._data.UpdateApprovalList(dto);
            ReqGuildMemList(DataMgr._data.reqMemPageIndex, ReqMemListCount);
        }
        /// <summary>
        /// 关闭申请列表
        /// </summary>
        public static void ReqClearApplyItem()
        {
            GameUtil.GeneralReq(Services.Guild_ClearApplyItem(DataMgr._data.applyIdStr), null, RespClearApplyItem);
        }

        private static void RespClearApplyItem()
        {
            DataMgr._data.ClearApprovalList();
        }

        /// <summary>
        /// 清空列表
        /// </summary>
        public static void ReqClearApplyList()
        {
            GameUtil.GeneralReq(Services.Guild_ClearApplyList(), null, RespClearApplyList);
        }
        private static void RespClearApplyList()
        {
            DataMgr._data.ClearApprovalDataList();
        }

        /// <summary>
        /// 邀请
        /// </summary>
        /// <param name="playerId"></param>
        public static void ReqInvitation(long playerId)
        {
            GameDebuger.LogError("邀请入会： " + playerId);
            //GameUtil.GeneralReq(Services.Guild_Invitation(playerId));
        }

        /// <summary>
        /// 接受邀请
        /// </summary>
        /// <param name="inviterId"></param>
        public static void ReqAcceptInvite(long inviterId)
        {
            GameDebuger.LogError("接受邀请： " + inviterId);
            //GameUtil.GeneralReq(Services.Guild_Accept(inviterId));
        }

        /// <summary>
        /// 离开公会
        /// </summary>
        public static void ReqLeave()
        {
            GameUtil.GeneralReq(Services.Guild_Leave(),null, ResLeaveSuccess);
        }
        private static void ResLeaveSuccess()
        {
            PlayerGuildInfoDto dto = null;
            DataMgr._data.UpdateGuildState(dto);
            DataMgr._data.BackToMainCity();
            GuildMainViewLogic.CloseHasJoinView();
            TipManager.AddTip("你已成功脱离公会");
        }
        /// <summary>
        /// 开除成员
        /// </summary>
        /// <param name="memberId"></param>
        public static void ReqKickOut(long memberId)
        {
            GameUtil.GeneralReq(Services.Guild_Kickout(memberId), null, () => RespKickOut(memberId));
        }

        private static void RespKickOut(long memberId)
        {
            DataMgr._data.UpdateMemList(memberId);
            ReqGuildMemList(DataMgr._data.reqMemPageIndex, ReqMemListCount);
        }

        /// <summary>
        /// 更改宣言
        /// </summary>
        /// <param name="memo"></param>
        public static void ReqChangeManifesto(string memo)
        {
            GameUtil.GeneralReq(Services.Guild_UpdateMemo(memo), null, () =>
             {
                 RespGuildNoticChange(NoticeModificationViewController.NoticeType.guildManifesto,memo);
             });
        }

        /// <summary>
        /// 更改公告
        /// </summary>
        /// <param name="notice"></param>
        public static void ReqChangeNotice(string notice)
        {
            GameUtil.GeneralReq(Services.Guild_UpdateNotice(notice),null, () =>
            {
                RespGuildNoticChange(NoticeModificationViewController.NoticeType.guildNotic,notice);
            });
        }
        private static void RespGuildNoticChange(NoticeModificationViewController.NoticeType noticeType,string message)
        {
            DataMgr._data.ChangeNoticeMessage(noticeType,message);
        }

        /// <summary>
        /// 公会成员
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        public static void ReqGuildMemList(int pageIndex,int pageSize)
        {
            GameUtil.GeneralReq<GuildMemberListDto>(Services.Guild_MemberList(pageIndex, pageSize), RespGuildMemList);
        }

        private static void RespGuildMemList(GuildMemberListDto dto)
        {
            DataMgr._data.UpdateMemList(dto);
            FireData();
        }

        /// <summary>
        /// 开除成员
        /// </summary>
        /// <param name="memberId"></param>
        public static void ReqGuildKickOut(long memberId)
        {
            GameDebuger.LogError("开除成员");
            //GameUtil.GeneralReq(Services.Guild_Kickout(memberId));
        }

        /// <summary>
        /// 任免职位
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="position"></param>
        public static void ReqGuildAppoint(long memberId,int position,string memberName,string posName)
        {
            GameUtil.GeneralReq(Services.Guild_Appoint(memberId, position), null,()=> { RespGuildAppointSuc(memberId, position, memberName, posName); } );
        }
        private static void RespGuildAppointSuc(long memberId, int position, string memberName, string posName)
        {
            DataMgr._data.UpdateMemList(memberId, position);
            TipManager.AddTip("你成功任命" + memberName + "为" + posName);
        }

        /// <summary>
        /// 移交会长
        /// </summary>
        /// <param name="targetId"></param>
        public static void ReqTransferBoss(long targetId)
        {
            GameUtil.GeneralReq(Services.Guild_TransferBoss(targetId), null, () => { RespTransferBossSuc(targetId); });
        }

        private static void RespTransferBossSuc(long targetId)
        {
            DataMgr._data.TransferBoss(targetId);
        }

        /// <summary>
        /// 建筑升级
        /// </summary>
        /// <param name="building"></param>
        public static void ReqUpgradeBuilding(int building)
        {
            GameUtil.GeneralReq<OnUpgradeBuildingDto>(Services.Guild_UpgradeBuilding(building), e => RespUpgradeBuilding(e, building));
        }
        private static void RespUpgradeBuilding(OnUpgradeBuildingDto dto,int building)
        {
            DataMgr._data.UpdateBuildUpTime(building, dto);
            FireData();
        }
        /// <summary>
        /// 一键接收
        /// </summary>
        public static void ReqOnekeyAccept(int pageIndex,int pageSize)
        {
            GameUtil.GeneralReq<GuildApprovalListDto>(Services.Guild_BatchApproval(pageIndex, pageSize), e => { RespRequestList(e, false); });
        }

        /// <summary>
        /// 回到公会
        /// </summary>
        /// <param name="sceneId"></param>
        /// <param name="sceneName"></param>
        public static void ReqEnterGuildMap(int sceneId,string sceneName)
        {

            WorldManager.Instance.Enter(sceneId, false);
            GuildMainViewLogic.CloseHasJoinView();
        }
        
        /// <summary>
        /// 搜索
        /// </summary>
        /// <param name="str"></param>
        public static void ReqSearch(string str)
        {
            GameUtil.GeneralReq<GuildBaseInfoListDto>(Services.Guild_Query(str), RespSearch);
        }

        private static void RespSearch(GuildBaseInfoListDto dto)
        {
            DataMgr._data.UpdateSearchGuildList(dto);
        }

        /// <summary>
        /// 领取工资
        /// </summary>
        public static void ReqGainSalary()
        {
            GameUtil.GeneralReq(Services.Guild_GainSalary(), null, RespGainSalary);
        }

        private static void RespGainSalary()
        {
            DataMgr._data.SetGainSalary(true);
        }

        /// <summary>
        /// 福利界面数据
        /// </summary>
        public static void ReqGuildWelfareDto()
        {
            GameUtil.GeneralReq<GuildWelfareDto>(Services.Guild_Welfare(), RespGuildWelfareDto);
        }

        private static void RespGuildWelfareDto(GuildWelfareDto dto)
        {
            DataMgr._data.SetGuildWelfareDto(dto);
            FireData();
        }

        /// <summary>
        /// 宝箱数据
        /// </summary>
        public static void ReqTreasureBox()
        {
            GameUtil.GeneralReq<GuildBoxDto>(Services.Guild_SelPointBox(), RespTreasureBox);
        }
        private static void RespTreasureBox(GuildBoxDto dto)
        {
            DataMgr._data.UpdateTreasureBox(dto);
            FireData();
        }

        /// <summary>
        /// 打开宝箱
        /// </summary>
        public static void ReqOepnBox()
        {
            GameUtil.GeneralReq<GuildBoxDto>(Services.Guild_OpenPointBox(), RespTreasureBox);
        }

        /// <summary>
        /// 捐献可叠堆物资
        /// </summary>
        /// <param name="id"></param>
        /// <param name="count"></param>
        public static void ReqDonateByItemId(int id,int count)
        {
            GameUtil.GeneralReq(Services.Guild_DonateByItemId(id,count), null, RespDonate);
        }
        /// <summary>
        /// 捐献不可叠堆物资
        /// </summary>
        /// <param name="id"></param>
        /// <param name="count"></param>
        public static void ReqDonateByUniqueId(long id)
        {
            GameUtil.GeneralReq(Services.Guild_DonateByUniqueId(id), null, RespDonate);
        }

        private static void RespDonate()
        {
            TipManager.AddTip("捐献物资成功");
            DataMgr._data.UpdateDonateCount();
            FireData();
        }
    }
}
