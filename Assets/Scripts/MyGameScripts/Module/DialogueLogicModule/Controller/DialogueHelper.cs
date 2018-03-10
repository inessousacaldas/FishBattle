using UnityEngine;
using System.Collections;
using AppDto;
using AppServices;

public class DialogueHelper  {
    public enum DialogType
    {
        None = 0,
        Shop = 1,
        EveryDayMission = 2,
        Fight = 3,
        Team = 4,
        CloseDialog = 5,
        WathcFight = 6,
        GhostMission = 7,
        PatrolResidue = 8,
        AcceptTreasury = 9,
        EnterTower = 10,        //进入四轮之塔
        ResetTower = 11,        //重置四轮之塔
        TowerRank = 12,         //四轮之塔排行榜
        GarandArena = 13,
        BackToMainScene = 14,   //四轮之塔回到洛连特
        GoOnNextTower = 15,      //四轮之塔继续下一层
        OpenCopyPanel = 16,     //打开副本界面
        EnterMarial = 17,        //参加武术大会
        LeaveMarial = 18,         //离开武术大会场景
        AcceptMissionGuild = 21,        //帮派任务
        ExpelGuildMonster = 22      //驱赶公会强盗

    }

    public static bool OpenDialogueFunction(SceneNpcDto npcStateDto,DialogFunction dialogFunction)
    {
        bool needClose = true;
        switch((DialogType)dialogFunction.type) {
            case DialogType.Shop:
                ProxyNpcDialogueView.Close();
                var shop = DataCache.getDtoByCls<Shop>(dialogFunction.logicId);
                if(shop != null) {
                    if(shop.shopType == 104)
                        ProxyShop.OpenShopByType(shop.shopType,MissionDataMgr.DataMgr.GetMissionShopItem().QuickShopItemID);
                    else if(shop.shopType == 105)
                        ProxyShop.OpenShopByType(shop.shopType,MissionDataMgr.DataMgr.GetMissionShopItem().WeaPonShopItemID);
                    else if(shop.shopType == 108)   //公会商店直接开商城界面,特殊处理
                        ProxyShop.OpenShop(ShopTypeTab.GuildShop, ShopTypeTab.GuildShopId);
                    else
                        ProxyShop.OpenShopByType(shop.shopType);
                }
                else
                    GameDebuger.LogError(string.Format("shop表找不到{0}商店,请检查", dialogFunction.logicId));
                needClose = false;
                break;
            case DialogType.EveryDayMission:
                ProxyNpcDialogueView.Close();
                ProxyEveryDayMission.Open();
                needClose = false;
                break;
            case DialogType.Fight:
            case DialogType.ExpelGuildMonster:
                //开战
                GameUtil.GeneralReq(Services.Scene_Battle(npcStateDto.id));
                break;
            case DialogType.Team:
                var teamAction = DataCache.getDtoByCls<TeamActionTarget>(dialogFunction.logicId);
                var teamTarget = TeamMatchTargetData.Create(dialogFunction.logicId, teamAction.maxGrade, teamAction.minGrade, true);
                TeamDataMgr.DataMgr.GetCurTeamMatchTargetData = teamTarget;
                ProxyTeamModule.OpenMainView(TeamMainViewTab.CreateTeam);
                TeamDataMgr.TeamNetMsg.AutoMatchTeam(teamTarget, true);
                //组队平台
                break;
            case DialogType.WathcFight:
                //观战
                GameUtil.GeneralReq(Services.Battle_Video(npcStateDto.battleId));
                break;
            case DialogType.GhostMission:
                //接受捉鬼任务
                Mission mission = new Mission();
                mission.id = -1;
                mission.type = (int)MissionType.MissionTypeEnum.Ghost;
                MissionDataMgr.DataMgr.AcceptMission(mission);
                needClose = false;
                break;
            case DialogType.PatrolResidue:
                //查看点数
                ProxyNpcDialogueView.Close();
                ProxyNpcDialogueView.OpenCustomPanel(npcStateDto.npc,string.Format("你今天还剩余{0}点安全巡查次数，每次巡查消耗1个点数，点数为0时不能获得巡查奖励。每天0点可获得60点次数，最多累积120点次数。",MissionDataMgr.MissionData.mGhostRingCount),null,null);
                needClose = false;
                break;
            case DialogType.AcceptTreasury:
                //接受宝图任务
                GameUtil.GeneralReq(Services.Mission_AcceptTreasury());
                break;
            case DialogType.EnterTower:
                if (!FunctionOpenHelper.isFuncOpen((int)FunctionOpen.FunctionOpenEnum.FUN_48, false))
                {
                    TipManager.AddTip("四轮之塔太危险了，还是先升级再来挑战吧！");
                    break;
                }
                else if (TeamDataMgr.DataMgr.IsLeader())
                {
                    TipManager.AddTip("组队状态不能够挑战");
                    break;
                }
                else if (TowerDataMgr.DataMgr.IsAllTowerClear)
                {
                    TipManager.AddTip("你已经完成所有挑战，请重置进度");
                    break;
                }
                TowerDataMgr.TowerNetMsg.EnterTower();
                break;
            case DialogType.ResetTower:
                int resetCount = TowerDataMgr.DataMgr.ResetCount;
                if (resetCount > 0)
                {
                    string des = "是否重置四轮之塔进度？";
                    string title = "";
                    var ctrl = ProxyBaseWinModule.Open();
                    BaseTipData data = BaseTipData.Create(title, des, 0, delegate
                    {
                        TowerDataMgr.TowerNetMsg.ResetTower();
                    }, null);
                    ctrl.InitView(data);
                }
                else
                    TipManager.AddTip("当前重置次数不足");
                break;
            case DialogType.TowerRank:
                TowerDataMgr.TowerNetMsg.TowerRank();
                break;
            case DialogType.GarandArena:
                if(FunctionOpenHelper.isFuncOpen((FunctionOpen.FunctionOpenEnum)dialogFunction.functionOpenId))
                    ProxyGarandArenaMainView.Open();
                break;
            case DialogType.BackToMainScene:
                TowerDataMgr.DataMgr.SetCanFireData();
                var mainNpc = TowerDataMgr.DataMgr.MainNpc;
                WorldManager.Instance.Enter(mainNpc.sceneId, false, false, true, null,WorldManager.Instance.MakeFlyPos(mainNpc.x - 2,mainNpc.z,false));
                break;
            case DialogType.GoOnNextTower:
                if (TowerDataMgr.DataMgr.IsAllTowerClear)
                {
                    TipManager.AddTip("你已经完成所有挑战，请重置进度");
                    break;
                }
                TowerDataMgr.TowerNetMsg.EnterTower();
                break;
            case DialogType.OpenCopyPanel:
                ProxyCopyPanel.Open();
                //打开副本界面
                break;
            case DialogType.EnterMarial:
                MartialDataMgr.MartialNetMsg.EnterKungfu();
                break;
            case DialogType.LeaveMarial:
                MartialDataMgr.MartialNetMsg.Exit();
                break;
            case DialogType.AcceptMissionGuild:
                Mission gmission = new Mission();
                gmission.id = -1;
                gmission.type = (int)MissionType.MissionTypeEnum.Guild;
                MissionDataMgr.DataMgr.AcceptMission(gmission);
                needClose = false;
                break;
        }
        return needClose;
    }
}
