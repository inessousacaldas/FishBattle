using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class HotKeyManager
{
    #region UI 相关
    public class OpenUIModule
    {
        public KeyCode[] HotKeyList;
        public string ModuleName;
        public Action OnOpened;
        public Action OnClosed;

        public void Trigger()
        {
            // 判断当前是打开状态还是关闭状态
            if (!UIModuleManager.Instance.IsModuleOpened(ModuleName))
            {
                if (OnOpened != null)
                {
                    OnOpened();
                }
            }
            else
            {
                if (OnClosed != null)
                {
                    OnClosed();
                }
            }

        }
    }

    public static readonly OpenUIModule[] OpenUIModuleList;
    #endregion

    #region 战斗相关

    public class BattleAction
    {
        public KeyCode[] HotKeyList;
        public Action OnTrigger;

        public void Trigger()
        {
            if (OnTrigger != null)
            {
                OnTrigger();
            }
        }
    }

    public static BattleAction[] BattleActionList;
    #endregion

    static HotKeyManager()
    {
        GameDebuger.TODO(@"OpenUIModuleList = new OpenUIModule[]
        {
             // 大地图
             new OpenUIModule() {HotKeyList = new []{KeyCode.LeftAlt, KeyCode.M}, ModuleName = ProxyWorldMapModule.NAME_MINIWORLDMAP, OnOpened = () => ProxyWorldMapModule.OpenMiniWorldMap(), OnClosed = () => ProxyWorldMapModule.CloseMiniWorldMap()},
             // 小地图
             new OpenUIModule() {HotKeyList = new []{KeyCode.Tab}, ModuleName = ProxyWorldMapModule.NAME_MINIMAP, OnOpened = () => ProxyWorldMapModule.OpenMiniMap(), OnClosed = () => ProxyWorldMapModule.CloseMiniMap()},
             // 指引
             new OpenUIModule() {HotKeyList = new []{KeyCode.LeftAlt, KeyCode.H}, ModuleName = ProxyGameGuideModule.NAME, OnOpened = () => ProxyGameGuideModule.Open(), OnClosed = () => ProxyGameGuideModule.Close()},
             // 日程
             new OpenUIModule() {HotKeyList = new []{KeyCode.LeftAlt, KeyCode.C}, ModuleName = ProxySchedulePushModule.NAME, OnOpened = () => ProxySchedulePushModule.Open(), OnClosed = () => ProxySchedulePushModule.Close()},
             // 挂机
             new OpenUIModule() {HotKeyList = new []{KeyCode.LeftAlt, KeyCode.G}, ModuleName = ProxyAutoFramModule.NAME, OnOpened = () => ProxyAutoFramModule.Open(), OnClosed = () => ProxyAutoFramModule.Close()},
             // 排行版
             new OpenUIModule() {HotKeyList = new []{KeyCode.LeftAlt, KeyCode.R}, ModuleName = ProxyRankingModule.NAME, OnOpened = () => ProxyRankingModule.Open(), OnClosed = () => ProxyRankingModule.Close()},
             // 交易
             new OpenUIModule() {HotKeyList = new []{KeyCode.LeftAlt, KeyCode.S}, ModuleName = ProxyTradeModule.NAME_TTADEBASEVIEW_PATH, OnOpened = () => ProxyTradeModule.Open(), OnClosed = () => ProxyTradeModule.Close()},
             // 商城
             new OpenUIModule() {HotKeyList = new []{KeyCode.LeftAlt, KeyCode.A}, ModuleName = ProxyShopModule.NAME, OnOpened = () => ProxyShopModule.OpenMallShopping(), OnClosed = () => ProxyShopModule.Close()},
             // 好友
             new OpenUIModule() {HotKeyList = new []{KeyCode.LeftAlt, KeyCode.F}, ModuleName = ProxyFriendModule.NAME, OnOpened = () => ProxyFriendModule.Open(), OnClosed = () => ProxyFriendModule.Close()},
             // 聊天
             new OpenUIModule() {HotKeyList = new []{KeyCode.LeftAlt, KeyCode.X}, ModuleName = ProxyChatModule.NAME, OnOpened = () => ProxyChatModule.Open(), OnClosed = () => ProxyChatModule.Close()},
             // 宠物
             new OpenUIModule() {HotKeyList = new []{KeyCode.LeftAlt, KeyCode.Q}, ModuleName = ProxyPetPropertyModule.PETPROPERTY_MAINVIEW, OnOpened = () => ProxyPetPropertyModule.Open(), OnClosed = () => ProxyPetPropertyModule.Close()},
             // 人物
             new OpenUIModule() {HotKeyList = new []{KeyCode.LeftAlt, KeyCode.W}, ModuleName = ProxyPlayerPropertyModule.BASEINFO_VIEW, OnOpened = () => ProxyPlayerPropertyModule.Open(), OnClosed = () => ProxyPlayerPropertyModule.Close()},
             // 任务
             new OpenUIModule() {HotKeyList = new []{KeyCode.LeftAlt, KeyCode.Y}, ModuleName = ProxyMissionModule.NAME_MISSION_PATH, OnOpened = () => ProxyMissionModule.Open(), OnClosed = () => ProxyMissionModule.Close()},
             // 技能
             new OpenUIModule() {HotKeyList = new []{KeyCode.LeftAlt, KeyCode.T}, ModuleName = ProxySkillModule.NAME, OnOpened = () => ProxySkillModule.Open(), OnClosed = () => ProxySkillModule.Close()},
             // 包裹
             new OpenUIModule() {HotKeyList = new []{KeyCode.LeftAlt, KeyCode.E}, ModuleName = ProxyBackpackModule.NAME, OnOpened = () => ProxyBackpackModule.Open(), OnClosed = () => ProxyBackpackModule.Close()},
             // 打造
             new OpenUIModule() {HotKeyList = new []{KeyCode.LeftAlt, KeyCode.V}, ModuleName = ProxyEquipmentOptModule.NAME, OnOpened = () => ProxyEquipmentOptModule.Open(), OnClosed = () => ProxyEquipmentOptModule.Close()},
             // 系统
             new OpenUIModule() {HotKeyList = new []{KeyCode.LeftAlt, KeyCode.J}, ModuleName = ProxySystemSettingModule.NAME_SystemSettingView, OnOpened = () => ProxySystemSettingModule.Open(), OnClosed = () => ProxySystemSettingModule.Close()},
             // 帮派
             new OpenUIModule() {HotKeyList = new []{KeyCode.LeftAlt, KeyCode.B}, ModuleName = ProxyGuildModule.NAME_GuildInfoList, OnOpened = () => ProxyGuildModule.OpenGuildInfoList(), OnClosed = () => ProxyGuildModule.CloseGuildInfoList()},
            // 伙伴
             new OpenUIModule() {HotKeyList = new []{KeyCode.LeftAlt, KeyCode.D}, ModuleName = ProxyCrewModule.MAIN_VIEW, OnOpened = () => ProxyCrewModule.Open(), OnClosed = () => ProxyCrewModule.Close()},         
             // 队伍
             new OpenUIModule() {HotKeyList = new []{KeyCode.LeftAlt, KeyCode.Z}, ModuleName = ProxyTeamModule.MAIN_VIEW, OnOpened = () => ProxyTeamModule.Open(), OnClosed = () => ProxyTeamModule.Close()},
             // 奖励
             new OpenUIModule() {HotKeyList = new []{KeyCode.LeftAlt, KeyCode.N}, ModuleName = ProxyRewardModule.PATH_NAME, OnOpened = () => ProxyRewardModule.Open(), OnClosed = () => ProxyRewardModule.Close()},
             // 精简模式
             new OpenUIModule() {HotKeyList = new []{KeyCode.LeftAlt, KeyCode.P}, ModuleName = ProxyMainUI.MAINUI_VIEW, OnOpened = null, OnClosed = () =>{ProxyMainUI.TogleDisplayModel();}},
        };

        BattleActionList = BattleController.GetBattleHotKeyList();
            ");
    }
}
