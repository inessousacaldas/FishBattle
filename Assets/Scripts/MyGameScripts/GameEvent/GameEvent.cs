using AppDto;
using UnityEngine;
using System.Collections.Generic;

public static class GameEvent
{
    #region Battle

//    public static readonly GameEvents.Event<MonsterController> BATTLE_FIGHT_CHOOSETARGETPET = new GameEvents.Event<MonsterController>("BATTLE_FIGHT_CHOOSETARGETPET");
    
    public static readonly GameEvents.Event BATTLE_FIGHT_EXITBATTLE = new GameEvents.Event("BATTLE_FIGHT_EXITBATTLE");
    public static readonly GameEvents.Event<Transform> BATTLE_UI_UPDATE_POSITION = new GameEvents.Event<Transform>("BATTLE_UI_UPDATE_POSITION");
    public static readonly GameEvents.Event BATTLE_FIGHT_DESTROY = new GameEvents.Event("BATTLE_FIGHT_DESTROY");
    //    public static readonly GameEvents.Event BATTLE_FIGHT_ORDERUPDATE = new GameEvents.Event("BATTLE_FIGHT_ORDERUPDATE");//调整为从队列中增加及减少
    //    public static readonly GameEvents.Event<VideoRound> BATTLE_FIGHT_ROUNDINFO = new GameEvents.Event<VideoRound>("BATTLE_FIGHT_ROUNDINFO");//既然是单例的方法，就不用事件了。
    //public static readonly GameEvents.Event<ActionQueueAddNotify> BATTLE_FIGHT_ADD_TO_QUEUE_UI = new GameEvents.Event<ActionQueueAddNotify>("BATTLE_FIGHT_ADD_TO_QUEUE");
    public static readonly GameEvents.Event<long,string > BATTLE_FIGHT_REMOVE_FROM_QUEUE_UI = new GameEvents.Event<long,string>("BATTLE_FIGHT_REMOVE_FROM_QUEUE");
    public static readonly GameEvents.Event<VideoRound> BATTLE_FIGHT_VIDEOROUND = new GameEvents.Event<VideoRound>("BATTLE_FIGHT_VIDEOROUND");
    public static readonly GameEvents.Event<string,float> BATTLE_FIGHT_UPDATE_REMAIN_TIME = new GameEvents.Event<string, float>("BATTLE_FIGHT_UPDATE_REMAIN_TIME");
    
    public static readonly GameEvents.Event<long,float,float> BATTLE_UI_SKILL_COMMON_CD = new GameEvents.Event<long,float,float>("BATTLE_UI_SKILL_COMMON_CD");
    //（名字）队列变化（添加或移除、开始或结束），参数：回合数据，是否添加/开始
    public static readonly GameEvents.Event<long> BATTLE_FIGHT_QUEUE_UPDATE = new GameEvents.Event<long>("BATTLE_FIGHT_QUEUE_UPDATE");
    
    public static readonly GameEvents.Event<long> BATTLE_UI_SELECTED_OPTION_ROLE_CHANGED = new GameEvents.Event<long>("BATTLE_UI_SELECTED_OPTION_ROLE_CHANGED");

    public static readonly GameEvents.Event BATTLE_UI_ACTION_REQUEST_SUCCESS = new GameEvents.Event("BATTLE_UI_ACTION_REQUEST_SUCCESS");
    public static readonly GameEvents.Event BATTLE_UI_HIDE_AUTO_ROUND_TIME_LABEL = new GameEvents.Event("BATTLE_UI_HIDE_AUTO_ROUND_TIME_LABEL");
    public static readonly GameEvents.Event<BattlePosition.MonsterSide,int> BATTLE_UI_UPDATE_ACTION_TOTAL_DAMAGE_OR_HEAL = new GameEvents.Event<BattlePosition.MonsterSide,int>("BATTLE_UI_UPDATE_ACTION_TOTAL_DAMAGE_OR_HEAL");
    
    public static readonly GameEvents.Event<bool> BATTLE_UI_BTN_AUTO_CLICKED = new GameEvents.Event<bool>("BATTLE_UI_BTN_AUTO_CLICKED");
    public static readonly GameEvents.Event BATTLE_UI_ONORDERLISTUPDATE = new GameEvents.Event("BATTLE_UI_ONORDERLISTUPDATE");
    public static readonly GameEvents.Event<long,int> BATTLE_UI_SP_UPDATED = new GameEvents.Event<long,int>("BATTLE_UI_SP_UPDATED");
    //有行动者死亡
   
    public static readonly GameEvents.Event<long,MonsterOptionStateManager.MonsterOptionState> BATTLE_FIGHT_MONSTER_OPTION_STATE_CHANGED = new GameEvents.Event<long,MonsterOptionStateManager.MonsterOptionState>("BATTLE_FIGHT_MONSTER_OPTION_STATE_CHANGED");
    public static readonly GameEvents.Event<long,SkillBuff,bool> BATTLE_FIGHT_BUFF_STATUS_CHANGED = new GameEvents.Event<long,SkillBuff,bool>("BATTLE_FIGHT_BUFFED");
    public static readonly GameEvents.Event<bool,bool> BATTLE_UI_ON_OPTION_BUTTON_HIDE = new GameEvents.Event<bool,bool>("BATTLE_UI_ON_OPTION_BUTTON_HIDE");
    public static readonly GameEvents.Event<MonsterController,Skill> BATTLE_UI_SKILL_SELECTED = new GameEvents.Event<MonsterController,Skill>("BATTLE_UI_SKILL_SELECTED");
    //当前操作目标选定。
    
    public static readonly GameEvents.Event<Skill,string> BATTLE_UI_SHOW_TARGET_SELECT = new GameEvents.Event<Skill,string>("BATTLE_UI_SHOW_TARGET_SELECT");
    public static readonly GameEvents.Event<Skill,bool> BATTLE_UI_SHOW_SKILL_TIP = new GameEvents.Event<Skill,bool>("BATTLE_UI_SHOW_SKILL_TIP");
    public static readonly GameEvents.Event<MonsterController> BATTLE_UI_CD_STATUS_UPDATE = new GameEvents.Event<MonsterController>("BATTLE_UI_CD_STATUS_UPDATE");
    public static readonly GameEvents.Event BATTLE_FIGHT_AUTO_FIGHT_CONFIG_UPDATE = new GameEvents.Event("BATTLE_FIGHT_AUTO_FIGHT_TIME_UPDATE");

    #endregion

    #region World

    public static readonly GameEvents.Event<ScenePlayerDto> World_OnAddPlayer = new GameEvents.Event<ScenePlayerDto>("World_OnAddPlayer");
    public static readonly GameEvents.Event<ScenePlayerDto> World_OnUpdatePlayer = new GameEvents.Event<ScenePlayerDto>("World_OnUpdatePlayer");
    public static readonly GameEvents.Event<long> World_OnRemovePlayer = new GameEvents.Event<long>("World_OnRemovePlayer");
    public static readonly GameEvents.Event<long, float, float> World_OnUpdatePlayerPos = new GameEvents.Event<long, float, float>("World_OnUpdatePlayerPos");
    public static readonly GameEvents.Event<long, float, float> World_OnChangePlayerPos = new GameEvents.Event<long, float, float>("World_OnChangePlayerPos");
    public static readonly GameEvents.Event<long, bool> World_OnChangeBattleStatus = new GameEvents.Event<long, bool>("World_OnChangeBattleStatus");
    public static readonly GameEvents.Event<long, int> World_OnChangeWeapon = new GameEvents.Event<long, int>("World_OnChangeWeapon");
    public static readonly GameEvents.Event<long, int> World_OnChangeWeaponEff = new GameEvents.Event<long, int>("World_OnChangeWeaponEff");
    public static readonly GameEvents.Event<long> World_OnChangePlayerTitle = new GameEvents.Event<long>("World_OnChangePlayerTitle");
    public static readonly GameEvents.Event<long> World_OnChangePlayerDye = new GameEvents.Event<long>("World_OnChangePlayerDye");
    public static readonly GameEvents.Event<long, long, float> World_OnChangePlayerScale = new GameEvents.Event<long, long, float>("World_OnChangePlayerScale");
    public static readonly GameEvents.Event<long> World_OnChangePlayerModel = new GameEvents.Event<long>("World_OnChangePlayerModel");
    public static readonly GameEvents.Event<long, float, float> World_OnUpdateNpcPos = new GameEvents.Event<long, float, float>("World_OnUpdateNpcPos");
    public static readonly GameEvents.Event<long,int> World_OnHallowSpriteNotify = new GameEvents.Event<long,int>("World_OnHallowSpriteNotify");
    public static readonly GameEvents.Event<long> World_OnChangePlayerMoveSpeed = new GameEvents.Event<long>("World_OnChangePlayerMoveSpeed");
    public static readonly GameEvents.Event<long> World_OnChangeMaster = new GameEvents.Event<long>("World_OnChangeMaster");
    public static readonly GameEvents.Event<long, long, bool> World_OnChangeTeamRide = new GameEvents.Event<long, long, bool>("World_OnChangeTeamRide");
    public static readonly GameEvents.Event<long> World_OnChangeTeamStatus = new GameEvents.Event<long>("World_OnChangeTeamStatus");
    //    public static readonly GameEvents.Event<long, PlayerView.DetailRideStatus> World_OnChangePlayerRide = new GameEvents.Event<long, PlayerView.DetailRideStatus>("World_OnChangePlayerRide");

    #region Team

    public static readonly GameEvents.Event Team_OnTeamStateUpdate = new GameEvents.Event("Team_OnTeamStateUpdate");
    //    OnPlayerViewMoveUpdate
    //    GameEventCenter.AddListener(GameEvent.Player_OnPlayerNicknameUpdate, UpdatePlayerNickName);
    //    GameEventCenter.AddListener(GameEvent.Player_OnPlayerGradeUpdate, OnHeroGradeUpdate);
    //    GameEventCenter.AddListener(GameEvent.Backpack_OnWeaponModelChange, UpdateWeapon);
    //    GameEventCenter.AddListener(GameEvent.Backpack_OnHallowSpriteChange

    #endregion

    #endregion World

    #region Friend
    public static readonly GameEvents.Event AddFriend = new GameEvents.Event("AddFriend");
    public static readonly GameEvents.Event AddBlackFriend = new GameEvents.Event("AddBlackFriend");
    #endregion

    #region Team
    public static readonly GameEvents.Event<List<long>> SCENE_TEAM_NOTIFY = new GameEvents.Event<List<long>>("SCENE_TEAM_NOTIFY");
    #endregion
    #region 工具迁移事件

    #region Scene

    public static readonly GameEvents.Event OnSceneChangeEnd = new GameEvents.Event("OnSceneChangeEnd");
    //切换场景结束，指模型 音频 地图 等资源已发起Load

    #endregion

    #endregion 工具迁移事件

    #region 技能相关
    public static readonly GameEvents.Event<RoleSkillTalentSingleItemController> ROLE_SKILL_SHOW_TALENT_GRADE_VIEW = new GameEvents.Event<RoleSkillTalentSingleItemController>("ROLE_SKILL_SHOW_TALENT_GRADE_VIEW");
    public static readonly GameEvents.Event ROLE_SKILL_RESET_MAIN_VIEW_DEPTH = new GameEvents.Event("ROLE_SKILL_RESET_MAIN_VIEW_DEPTH");
    #endregion

    #region 背包相关
    //背包物品有变更
    public static readonly GameEvents.Event BACK_PACK_ITEM_CHANGE = new GameEvents.Event("BACK_PACK_ITEM_CHANGE");
    #endregion

    #region 聊天系统相关
    //聊天部分有新消息
    public static readonly GameEvents.Event<ChatNotify> CHAT_MSG_NOTIFY = new GameEvents.Event<ChatNotify>("CHAT_MSG_NOTIFY");
    //私有聊天信息
    public static readonly GameEvents.Event PRIVATE_CHATMSG_NOTIFY = new GameEvents.Event("PRIVATE_CHATMSG_NOTIFY");
    #endregion

    #region 主战伙伴
    //更换主战伙伴
    public static readonly GameEvents.Event<long> CHANGE_MAIN_CREW = new GameEvents.Event<long>("CHANGE_MAIN_CREW");
    #endregion
}
