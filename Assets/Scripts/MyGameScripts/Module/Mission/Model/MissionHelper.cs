// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Created  : 07/15/2017 16:35:59
// **********************************************************************
using AppDto;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public enum MsgType
{
    Nothing = 0,
    TargetDesc = 1,
    Description = 2,
    DialogueFirst = 3
}

#region Npc头顶标示枚举
public enum NpcMissionMark
{
    //	没有标示
    Nothing = 0,
    //	可接标示
    Accept = 1,
    //	进行中标示
    Process = 2,
    //	可提交标示
    Submit = 3,
    //	主线可接
    MainAccept = 4,
    //	主线进行中
    MainProcess = 5,
    //	主线可提交
    MainSubmit = 6,
    //	战斗
    Battle = 7
}
#endregion

#region 一个对话选项的枚举
public enum DialogueOption
{
    /// <summary>
    /// 不存在选项菜单（初始状态）:0 -- The nothing option.
    /// </summary>
    NothingOption = 0,
    /// <summary>
    /// 只存在功能选项:1 -- The only function.
    /// </summary>
    OnlyFunction = 1,
    /// <summary>
    /// 存在功能选项和任务一级菜单:2 -- The function and main mission menu.
    /// </summary>
    Function_MainMission = 2,
    /// <summary>
    /// 只存在任务一级菜单:3 -- The only main mission menu.
    /// </summary>
    OnlyMainMission = 3,
    /// <summary>
    /// 只存在二级菜单（有且仅有一个任务）:4 -- The only sub mission menu.
    /// </summary>
    OnlySubMission = 4,
    /// <summary>
    /// 只存在二级菜单(有且仅有一个冥雷任务):5 -- The only monster menu.
    /// </summary>
    OnlyMonster = 5,
    /// <summary>
    /// 快速地任务快捷选项:6 -- The quickly menu.
    /// </summary>
    OnlyQuickly = 6
}
#endregion


#region 任务删除类型的枚举
public enum RemoveMissionType
{
    Drop = 0,
    Finish = 1,
    Notify = 2
}
#endregion


#region 虚拟NPC配置
public class FactionNpc:Npc
{

    /** 各门派对应的真实NPC编号数组,门派1对应的下标是0 */
    public List<int> npcIds;
}
#endregion

#region 任务bun
/// <summary>
/// 任务不可接取的原因
/// </summary>
public enum MissionUnAcceptableReason
{
    //	任务可接取
    Acceptable = 0,
    //	任务不可接，因为有前置任务未完成
    ConditionMission = 1,
    //	任务不可接，因为等级不足
    Lv = 2,
}
#endregion
public static class MissionHelper
{
    #region 任务类型枚举
    public enum SubmitDtoType
    {
        //	无类型
        Nothing = 0,
        //	对话
        Talk = 1,
        //	使用物品
        ApplyItem = 2,
        //	收集物品
        CollectionItem = 3,
        //	收集一类物品
        CollectionItemCategory = 4,
        //	收集宠物
        CollectionPet = 5,
        //	暗雷击杀
        HiddenMonster = 6,
        //	明雷击杀
        ShowMonster = 7,
        //	升级任务
        Upgrade = 8,
        //	引导任务
        Guide = 9,
        //	赏金任务
        Bounty = 10,
        //多人交谈任务
        Findtem = 11,
        //采集任务
        PickItem = 12,
        //指定怪掉落道具
        ShowMonsterItem = 13,
        //公会到坐标宣言
        GuildSpeak = 14
    }
    #endregion

    #region NPC对话选项
    private static DialogueOption _dialogueOption = DialogueOption.NothingOption;

    public static DialogueOption dialogueOption
    {
        get { return _dialogueOption; }
    }
    #endregion

    //====================================================信息处理====================================================
    #region 获取任务名字
    /// <summary>
    /// 获取任务名字
    /// </summary>
    /// <param name="mission"></param>
    /// <returns></returns>
    public static string GetMissionTitleName(Mission mission,bool isCell = false)
    {
        MissionType.MissionTypeEnum missionType = (MissionType.MissionTypeEnum)mission.type;
        string name = "";
        if (missionType >= MissionType.MissionTypeEnum.Faction && missionType < MissionType.MissionTypeEnum.Copy)
        {
            if (!isCell)
                name = mission.missionType.name;
            else
                name = "日常-" + mission.missionType.name;
        }
        else if(missionType >= MissionType.MissionTypeEnum.Copy && missionType <= MissionType.MissionTypeEnum.CopyExtra)
        {
            if (!isCell)
                name = mission.name;
            else
                name = "副本-" + mission.name;
        }
        else
        {
            if(!isCell)
                name = mission.name;
            else
                name = mission.missionType.name + "-" + mission.name;
        }
        return name;
    }
    #endregion

    #region 已接任务目标信息文字转换和详细内容文字转换
    public static string GetCurTargetContent(Mission mission,PlayerMissionDto playerMissionDto = null, MisViewTab misViewTab = MisViewTab.None,bool isCell = false)
    {
        string tStr = GetCurrentMissionStr(mission, playerMissionDto, MsgType.TargetDesc, misViewTab,isCell);
        if (tStr == "")
        {
            tStr = string.Format("{0}", mission.submitNpc == null ?
                                 string.Format("{0}-{1}完成", mission.missionType.name, mission.name)
                                 : string.Format("回复{0}[-]", GetNpcNameByNpc(mission.submitNpc, false, false, false).WrapColor(ColorConstantV3.Color_MissionGreen)));
        }
        return tStr;
    }

    public static string GetCurDescriptionContent(Mission mission,PlayerMissionDto playerMissionDto = null, MisViewTab misViewTab = MisViewTab.None)
    {
        string tStr = GetCurrentMissionStr(mission, playerMissionDto, MsgType.Description,misViewTab);
        return tStr;
    }
    static public string GetNpcNameByNpc(Npc npc, bool needScene, bool isMissionCell, bool withColorSta = true)
    {
        if (npc == null)
            return "";

        npc = NpcVirturlToEntity(npc);

        string tStr = npc.name;
        if (needScene)
        {
            tStr = string.Format("{0}的{1}", DataCache.getDtoByCls<SceneMap>(npc.sceneId).name, tStr);
        }
        return tStr;
    }

    #endregion

    #region 具体转换文字逻辑

    private static bool IsCell = false;
    /// <summary>
    /// 当前任务文字转换逻辑
    /// </summary>
    /// <returns></returns>
    private static string GetCurrentMissionStr(Mission mission,PlayerMissionDto playerMissionDto,MsgType msgType, MisViewTab misViewTab = MisViewTab.None, bool isCell = false)
    {
        IsCell = isCell;
        SubmitDto tSubmitDto = null;
        MissionDialog tCurMissionDialog = null;
        if(mission != null)
        {
            tSubmitDto = GetSubmitDtoByMission(mission);
            tCurMissionDialog = GetMissionSubmitDtoDialog(mission);
        }
        else if(playerMissionDto != null)
        {

        }

        if(tCurMissionDialog == null)
        {
            GameDebuger.YellowDebugLog(string.Format("当前没有该任务对话 | ID：{0}",tSubmitDto == null ? mission.dialogId : tSubmitDto.dialogId));
            return "";
        }
        string tStr = "";
        //TODO tSubmitDto的判断
        if (tSubmitDto != null)
        {
            if (msgType == MsgType.TargetDesc)
            {
                tStr = tCurMissionDialog.goalDesc;
            }
            else if (msgType == MsgType.Description)
            {
                tStr = tCurMissionDialog.description;
            }
        }
        else
        {
            if(misViewTab == MisViewTab.CanAccess)
            {
                if (msgType == MsgType.TargetDesc)
                {
                    if (mission.type >= (int)MissionType.MissionTypeEnum.Faction && mission.type != (int)MissionType.MissionTypeEnum.Copy)
                    {
                        if(mission.missionType.acceptNpc == null)
                            GameDebuger.Log("日常任务类型—missionType.acceptNpc为空，请策划配置");
                        else
                            tStr = "前往" + mission.missionType.acceptNpc.name + "处领取任务";
                    }
                    else if(mission.type == (int)MissionType.MissionTypeEnum.Copy)
                    {
                        if(mission.acceptNpc == null)
                            GameDebuger.Log("副本任务类型—mission.acceptNpc为空，请策划配置");
                        else
                            tStr = "前往" + mission.acceptNpc.name + "处领取任务";
                    }
                    else
                        tStr = tCurMissionDialog.goalDesc;
                }
            }
            if (msgType == MsgType.Description)
            {
                if (mission.type >= (int)MissionType.MissionTypeEnum.Faction)
                {
                    if (string.IsNullOrEmpty(mission.missionType.description)) 
                        GameDebuger.Log("日常任务类型—missionType.description没有配置，请策划配置");
                    else
                        tStr = mission.missionType.description;
                }
                else
                    tStr = tCurMissionDialog.description;
            }
        }
        if (tStr == string.Empty)
            return tStr;

        SubmitDtoType tSubmitDtoType = GetSubmitDtoTypeBySubmitDto(tSubmitDto);
        string submitscene = "", submitnpc = "", scene = "", item = "", count = "", quality = "",acceptnpc = "",grade = "";
        switch(tSubmitDtoType)
        {
            case SubmitDtoType.Nothing:
                tStr = RegexString(tStr, submitscene, submitnpc, scene, item, count, quality, acceptnpc);
                break;
            case SubmitDtoType.Talk:
                TalkSubmitDto tTalkSubmitDto = tSubmitDto as TalkSubmitDto;
                submitscene = tTalkSubmitDto.submitNpc.scene.name;
                submitnpc = tTalkSubmitDto.submitNpc.name;
                tStr = RegexString(tStr,submitscene,submitnpc);
                break;
            case SubmitDtoType.CollectionItem:
                CollectionItemSubmitDto tCollectionItemSubmitDto = tSubmitDto as CollectionItemSubmitDto;
                submitscene = tCollectionItemSubmitDto.submitNpc.scene.name;
                submitnpc = tCollectionItemSubmitDto.submitNpc.name;
                if(tCollectionItemSubmitDto.item != null) item = tCollectionItemSubmitDto.item.name;
                count = tSubmitDto.count + "/" + tSubmitDto.needCount;
                tStr = RegexString(tStr,submitscene,submitnpc,"",item,count);
                break;
            case SubmitDtoType.ShowMonster:
                ShowMonsterSubmitDto tShowMonsterSubmitDto = tSubmitDto as ShowMonsterSubmitDto;
                if(tShowMonsterSubmitDto.submitNpc.npc == null && tShowMonsterSubmitDto.dialog.submitDialogSequence.Count > 0)
                {
                    submitscene = tShowMonsterSubmitDto.acceptNpc.scene.name;
                    submitnpc = tShowMonsterSubmitDto.acceptNpc.name;
                }
                else
                {
                    if(tShowMonsterSubmitDto.submitNpc.id != 0)
                    {
                        submitscene = tShowMonsterSubmitDto.submitNpc.scene.name;
                        submitnpc = tShowMonsterSubmitDto.submitNpc.name;
                    }
                }
                acceptnpc = tShowMonsterSubmitDto.acceptNpc.name;
                tStr = RegexString(tStr, submitscene, submitnpc, "", "", "", "", acceptnpc);
                break;
            case SubmitDtoType.Findtem:
                FindtemSubmitInfoDto tFindtmSubmitDto = tSubmitDto as FindtemSubmitInfoDto;
                submitscene = tFindtmSubmitDto.submitNpc.scene.name;
                submitnpc = tFindtmSubmitDto.acceptNpc.name;
                tStr = RegexString(tStr,submitscene,submitnpc);
                break;
            case SubmitDtoType.ApplyItem:
                ApplyItemSubmitDto tApplyItemSubmitDto = tSubmitDto as ApplyItemSubmitDto;
                scene = tApplyItemSubmitDto.acceptScene.sceneMap == null ?
                    string.Format("场景ID错误：{0}", tApplyItemSubmitDto.acceptScene.id) : tApplyItemSubmitDto.acceptScene.sceneMap.name;
                item = tApplyItemSubmitDto.item == null?
                    string.Format("物品ID错误：{0}", tApplyItemSubmitDto.itemId) : tApplyItemSubmitDto.item.name;
                submitnpc = tApplyItemSubmitDto.submitNpc.name;
                tStr = RegexString(tStr, "", submitnpc, scene, item);
                break;
            case SubmitDtoType.PickItem:
                PickItemSubmitInfoDto tPickItemSubmitDto = tSubmitDto as PickItemSubmitInfoDto;
                if (tPickItemSubmitDto.item != null)
                {
                    item = tPickItemSubmitDto.item.name;
                    tStr = RegexString(tStr, "", "", "", item, "", "", "");
                }
                break;
            case SubmitDtoType.Upgrade:
                UpgradeSubmitDto tUpgradeSubmitDto = tSubmitDto as UpgradeSubmitDto;
                grade = tUpgradeSubmitDto.grade.ToString();
                tStr = RegexString(tStr, "", "", "", "", "", "", "", grade);
                break;
            case SubmitDtoType.ShowMonsterItem:
                ShowMonsterItemSubmitDto tShowMonsterItem = tSubmitDto as ShowMonsterItemSubmitDto;
                var showMonsterItem = ItemHelper.GetGeneralItemByItemId(tShowMonsterItem.itemId);
                if(showMonsterItem != null)
                {
                    item = showMonsterItem.name;
                    count = tShowMonsterItem.itemCount + "/" + tShowMonsterItem.needCount;
                }
                tStr = RegexString(tStr, "", "", "", item, count);
                break;
        }
        return tStr;
    }
    //字符匹配
    private static string RegexString(string tStr,string submitscene = "",string submitnpc = "",string scene = "",string item = "",string count = "",string quality = "",
                                        string acceptnpc = "",string grade = "")
    {
        UnityEngine.Color color = IsCell ? ColorConstantV3.Color_MissionGreen_2 : ColorConstantV3.Color_MissionGreen;
        tStr = Regex.Replace(tStr,"{submitscene}",submitscene.WrapColor(color));
        tStr = Regex.Replace(tStr,"{submitnpc}",submitnpc.WrapColor(color));
        tStr = Regex.Replace(tStr,"{scene}",scene.WrapColor(color));
        tStr = Regex.Replace(tStr,"{item}",item.WrapColor(color));
        tStr = Regex.Replace(tStr,"{count}",count.WrapColor(color));
        tStr = Regex.Replace(tStr,"{quality}",quality.WrapColor(color));
        tStr = Regex.Replace(tStr, "{acceptnpc}", acceptnpc.WrapColor(color));
        tStr = Regex.Replace(tStr, "{grade}", grade.WrapColor(color));
        return tStr;
    }
    #endregion

    //====================================================任务提交项相关====================================================
    #region SubmitDtoType 获取任务提交项类型By Mission
    /// <summary>
    /// 获取任务提交项类型 -- Gets the submit dto type by mission.
    /// </summary>
    /// <returns>The submit dto type by mission.</returns>
    /// <param name="mission">Mission.</param>
    static public SubmitDtoType GetSubmitDtoTypeByMission(Mission mission)
    {
        SubmitDtoType tSubmitDtoType = SubmitDtoType.Nothing;
        if(mission != null)
        {
            SubmitDto tSubmitDto = GetSubmitDtoByMission(mission);
            if(tSubmitDto != null)
            {
                tSubmitDtoType = GetSubmitDtoTypeBySubmitDto(tSubmitDto);
            }
        }

        return tSubmitDtoType;
    }
    #endregion
    #region 获取提交条件submitCondition对话内容
    public static MissionDialog GetMissionSubmitDtoDialog(Mission mission,int submitIndex = 0)
    {
        SubmitDto tSubMistDto = GetSubmitDtoByMission(mission,submitIndex);
        return tSubMistDto == null ? mission.dialog : tSubMistDto.dialog;
    }
    #endregion

    #region 获取任务提交具体条件

    public static SubmitDto GetSubmitDtoByMission(Mission mission,int submitIndex)
    {
        if(mission == null) return null;
        List<SubmitDto> tSubmitDtoList = GetSubmitDtoListByMission(mission);
        for(int i = 0, len = tSubmitDtoList.Count;i < len;i++)
        {
            SubmitDto tSubmitDto = tSubmitDtoList[i];
            if(tSubmitDto.index == submitIndex && !tSubmitDto.finish) return tSubmitDto;
        }
        return null;
    }

    static public SubmitDto GetSubmitDtoByMission(Mission mission,bool debugSta = false)
    {
        if(mission == null)
        {
            return null;
        }
        List<SubmitDto> tSubmitDtoList = GetSubmitDtoListByMission(mission);
        return GetSubmitDtoCommon(mission,tSubmitDtoList);
    }

    static public SubmitDto GetSubmitDtoCommon(Mission mission,List<SubmitDto> tSubmitDtoList,bool debugSta = false)
    {
        for(int i = 0, len = tSubmitDtoList.Count;i < len;i++)
        {
            SubmitDto tSubmitDto=tSubmitDtoList[i];
            if(!tSubmitDto.finish)
            {
                if(IsFindItem(tSubmitDto))
                {
                    //遇到的问题，1.如果三个submitdto都完成之后，如果三个submitdto的提交任务NPC都不一样，我改提交哪个人？
                    //由于代码问题tSubmitDto没有task这个字段，暂用count和neet来提交，看后续问题
                    if(tSubmitDto.count < tSubmitDto.needCount)
                    {
                        return tSubmitDto;
                    }
                }
                else
                {
                    return tSubmitDto;
                }
            }
        }
        if(debugSta)
        {
            GameDebuger.OrangeDebugLog(string.Format("####### | 已没有下一个提交项，可提交当前任务 | 当前任务ID:{0} | Name:{1}-{2} | Auto:{3} | SubmitNpc:{4} #######",
                                                 mission.id,mission.missionType.name,mission.name,mission.autoSubmit,mission.submitNpc == null ? "NULL" : mission.submitNpc.name));
        }
        return null;
    }
    #endregion

    #region 获取任务提交项列表（ByMission）
    public static List<SubmitDto> GetSubmitDtoListByMission(Mission mission)
    {
        List<SubmitDto> tSubmitDtoList = new List<SubmitDto>();
        PlayerMissionDto tPlayerMissionDto = MissionDataMgr.DataMgr.GetPlayerMissionDtoByMissionID(mission.id);
        if(tPlayerMissionDto != null) tSubmitDtoList = tPlayerMissionDto.completions;
        return tSubmitDtoList;
    }
    #endregion

    #region 是否需要手动提交任务
    /// <summary>
    /// 是否需要手动提交任务 -- Determines whether this instance is finish submit need self the specified tSubmitDto onlyJudgeCount.
    /// </summary>
    /// <returns><c>true</c> if this instance is finish submit need self the specified tSubmitDto onlyJudgeCount; otherwise, <c>false</c>.</returns>
    /// <param name="tSubmitDto">T submit dto.</param>
    /// <param name="onlyJudgeCount">If set to <c>true</c> only judge count.</param>
    static public bool IsFinishSubmitNeedSelf(SubmitDto tSubmitDto,bool onlyJudgeCount = false)
    {
        bool tIsFinish=false;
        if(tSubmitDto != null)
        {
            if(onlyJudgeCount)
            {
                return tSubmitDto.count >= tSubmitDto.needCount;
            }
            else
            {
                if(!tSubmitDto.auto && (tSubmitDto.finish || tSubmitDto.count >= tSubmitDto.needCount))
                {
                    tIsFinish = true;
                }
            }
        }
        return tIsFinish;
    }
    #endregion

    #region 获取玩家收集物品的任务需要的收集的物品
    static public Dictionary<int,GeneralItem> GetCollectionItemBySubmitDto(List<SubmitDto> submitDtoList)
    {
        Dictionary<int,GeneralItem> tGeneralItemDic = new Dictionary<int, GeneralItem>();
        for(int i = 0;i < submitDtoList.Count;i++)
        {
            SubmitDto submitDto = submitDtoList[i];
            if(MissionHelper.IsCollectionItem(submitDto))
            {
                CollectionItemSubmitDto tCollectionItemSubmitDto = submitDto as CollectionItemSubmitDto;
                tGeneralItemDic.Add(tCollectionItemSubmitDto.itemId,tCollectionItemSubmitDto.item);
            }
            else if(MissionHelper.IsCollectionItemOrItemCategory(submitDto))
            {
                CollectionItemSubmitDto tCollectionItemCategorySubmitDto = submitDto as CollectionItemSubmitDto;
                //for(int i = 0;len = tCollectionItemCategorySubmitDto.itemCategory.)
            }
        }
        return tGeneralItemDic;
    }
    #endregion

    //====================================================任务类型判断====================================================

    #region  判断是否主线任务
    static public bool IsMainMissionType(Mission mission)
    {
        return mission.type == (int)MissionType.MissionTypeEnum.Master;
    }
    #endregion

    #region  是否玩家对话任务
    static public bool IsTalkItem(SubmitDto submitDto)
    {
        return submitDto is TalkSubmitDto;
    }
    #endregion

    #region 是否是采集任务
    static public bool IsCollection(SubmitDto submitDto)
    {
        return submitDto is PickItemSubmitInfoDto;
    }
    #endregion

    #region 判断是否抓鬼任务
    static public bool IsBuffyMissionType(Mission mission)
    {
        return mission.type == (int)MissionType.MissionTypeEnum.Ghost;
    }
    #endregion


    #region 判断是否紧急委托
    static public bool IsUrgentMision(Mission mission) {
        return mission.type == (int)MissionType.MissionTypeEnum.Urgent;
    }
    #endregion

    #region 是否玩家明雷任务
    /// <summary>
    /// 是否玩家明雷任务 -- Determines whether this instance is show monster the specified submitDto.
    /// </summary>
    /// <returns><c>true</c> if this instance is show monster the specified submitDto; otherwise, <c>false</c>.</returns>
    /// <param name="submitDto">Submit dto.</param>
    static public bool IsShowMonster(SubmitDto submitDto)
    {
        return submitDto is ShowMonsterSubmitDto;
    }
    #endregion

    #region 是否 明雷Npc \ 动态Npc
    static public bool IsMonsterNpc(Npc npc)
    {
        return npc is NpcMonster;
    }
    #endregion

    #region 判断任务类型
    static public SubmitDtoType GetSubmitDtoTypeBySubmitDto(SubmitDto submitDto)
    {
        SubmitDtoType tSubmitDtoType = SubmitDtoType.Nothing;
        if(submitDto is TalkSubmitDto)
        {
            tSubmitDtoType = SubmitDtoType.Talk;
        }
        else if(submitDto is ApplyItemSubmitDto)
        {
            tSubmitDtoType = SubmitDtoType.ApplyItem;
        }
        else if(submitDto is CollectionItemSubmitDto)
        {
            tSubmitDtoType = SubmitDtoType.CollectionItem;
        }
        else if(submitDto is HiddenMonsterSubmitDto)
        {
            tSubmitDtoType = SubmitDtoType.HiddenMonster;
        }
        else if(submitDto is ShowMonsterSubmitDto)
        {
            tSubmitDtoType = SubmitDtoType.ShowMonster;
        }
        else if(submitDto is FindtemSubmitInfoDto)
        {
            tSubmitDtoType = SubmitDtoType.Findtem;
        }
        else if(submitDto is PickItemSubmitInfoDto)
        {
            tSubmitDtoType = SubmitDtoType.PickItem;
        }
        else if(submitDto is ShowMonsterItemSubmitDto)
        {
            tSubmitDtoType = SubmitDtoType.ShowMonsterItem;
        }
        else if(submitDto is CollectionItemCategorySubmitDto)
        {
            tSubmitDtoType = SubmitDtoType.CollectionItemCategory;
        }
        else if(submitDto is UpgradeSubmitDto)
        {
            tSubmitDtoType = SubmitDtoType.Upgrade;
        }
        else if(submitDto is SpeakSubmitDto) {
            tSubmitDtoType = SubmitDtoType.GuildSpeak;
        }
        //else if (submitDto is )
        //{
        //    tSubmitDtoType = SubmitDtoType.Guide;
        //}
        //else if(submitDto is BountyMissionSubmitDto)
        //{
        //    tSubmitDtoType = SubmitDtoType.Bounty;
        //}
        return tSubmitDtoType;
    }
    #endregion

    #region 判断是否支线任务
    static public bool IsExtensionMissionType(Mission mission)
    {
        return mission.type == (int)MissionType.MissionTypeEnum.Branch;
    }
    #endregion

    #region  是否主线 \ 支线任务
    /// <summary>
    /// 是否主线 \ 支线任务 -- Determines whether this instance is main or extension the specified mission.
    /// </summary>
    /// <returns><c>true</c> if this instance is main or extension the specified mission; otherwise, <c>false</c>.</returns>
    /// <param name="mission">Mission.</param>
    static public bool IsMainOrExtension(Mission mission)
    {
        return IsMainMissionType(mission) || IsExtensionMissionType(mission);
    }
    #endregion

    #region 是否收集物品任务
    /// <summary>
    /// 是否收集物品任务 -- Determines whether this instance is collection item or item category the specified submitDto.
    /// </summary>
    /// <returns><c>true</c> if this instance is collection item or item category the specified submitDto; otherwise, <c>false</c>.</returns>
    /// <param name="submitDto">Submit dto.</param>
    static public bool IsCollectionItemOrItemCategory(SubmitDto submitDto)
    {
        return IsCollectionItem(submitDto);
    }
    #endregion

    #region 是否玩家收集物品任务
    /// <summary>
    /// 是否玩家收集物品任务 -- Determines whether this instance is collection item the specified submitDto.
    /// </summary>
    /// <returns><c>true</c> if this instance is collection item the specified submitDto; otherwise, <c>false</c>.</returns>
    /// <param name="submitDto">Submit dto.</param>
    static public bool IsCollectionItem(SubmitDto submitDto)
    {
        return submitDto is CollectionItemSubmitDto;
    }
    #endregion

    #region 是否是玩家交谈获得道具任务
    static public bool IsFindItem(SubmitDto submitDto)
    {
        return submitDto is FindtemSubmitInfoDto;
    }
    #endregion

    #region 是否玩家暗雷任务
    static public bool IsHiddenMonster(SubmitDto submitDto){
        return submitDto is HiddenMonsterSubmitDto;
    }
    #endregion

    #region 是否玩家使用道具任务
    /// <summary>
    /// 是否玩家使用道具任务 -- Determines whether this instance is apply item the specified submitDto.
    /// </summary>
    /// <returns><c>true</c> if this instance is apply item the specified submitDto; otherwise, <c>false</c>.</returns>
    /// <param name="submitDto">Submit dto.</param>
    static public bool IsApplyItem(SubmitDto submitDto)
    {
        return submitDto is ApplyItemSubmitDto;
    }
    #endregion

    static public bool IsFactionMissionType(Mission mission)
    {
        return mission.type == (int)MissionType.MissionTypeEnum.Faction;
    }
    //=====================================================NPC是否能进行任务判断===================================================
    #region #获取当前场景
    static public int GetCurrentSceneID()
    {
        int tSceneID=WorldManager.Instance.GetModel()==null?ModelManager.Player.GetPlayer().sceneId:WorldManager.Instance.GetModel().GetSceneId();
        return tSceneID;
    }
    #endregion

    #region 判断NPC是否能直接进行任务
    /// <summary>
    /// 判断NPC是否能直接进行任务
    /// </summary>
    /// <param name="npc">npc信息</param>
    /// <param name="missionOptionList"></param>
    /// <param name="missionOptionType"></param>
    /// <returns></returns>
    static public bool IsOnlyMission(Npc npc,List<MissionOption> missionOptionList,DialogueOption missionOptionType)
    {
        if(missionOptionList != null && missionOptionList.Count > 0 && (missionOptionType == DialogueOption.OnlySubMission))
        {
            MissionOption tMissionOption=missionOptionList[0];
            SubmitDto tsubmitDot=GetSubmitDtoByMission(tMissionOption.mission);
            if(tMissionOption.isExis)
            {
                if(npc.type == (int)Npc.NpcType.General)
                {
                    return true;
                }

                if(tsubmitDot == null)
                {
                    return true;
                }

                if(npc.type == (int)Npc.NpcType.Monster && tsubmitDot is ShowMonsterSubmitDto)
                {
                    return true;
                }
            }
        }

        return false;
    }
    #endregion

    #region 生成NPC对话子菜单的菜单名
    static public string  GetMissionTitleNameInDialogue(Mission mission,bool isExist)
    {
        string tType=mission.missionType.name;
        tType += "任务";
        string tName="";
        tName = string.Format("{0}",mission.name);
        tName = string.Format("-{0}",tName);
        return string.Format("{0}{1}[-]",tType,tName);
    }
    #endregion

    #region 虚拟NPC转换为配置NPC
    public static Npc NpcVirturlToEntity(Npc npc)
    {
        Npc tNpc = npc;
        if(npc is FactionNpc)
        {
            FactionNpc tFactionNpc = npc as FactionNpc;
            int tFactionId = ModelManager.Player.GetPlayer().factionId;
            tNpc = DataCache.getDtoByCls<Npc>(tFactionNpc.npcIds[tFactionId]);
        }
        return tNpc;
    }
    #endregion
    //	==================================================任务上一次对话NPC引用Dic=======================================
    #region 获取NPC位置信息
    public static Npc GetNpcByNpcInfoDto(NpcInfoDto npcInfoDto)
    {
        Npc tNpc = null;

        if(npcInfoDto == null || npcInfoDto.id == 0)
        {
            tNpc = null;
        }
        else
        {
            tNpc = new Npc();

            //	寻路
            tNpc.id = npcInfoDto.id;
            tNpc.name = npcInfoDto.name;
            tNpc.sceneId = npcInfoDto.sceneId;
            tNpc.x = npcInfoDto.x;
            tNpc.y = npcInfoDto.y;
            tNpc.z = npcInfoDto.z;

            if(npcInfoDto.npc != null)
            {
                tNpc.type = npcInfoDto.npc.type;
                tNpc.modelId = npcInfoDto.npc.modelId;
                tNpc.diglogface = npcInfoDto.npc.diglogface;
            }
        }

        return tNpc;
    }
    #endregion

    #region  获取当前场景ID中存在的明雷怪物Npc
    public static NpcInfoDto GetNpcInfoDtoByNpc(Npc npc) {
        NpcInfoDto tNpcInfoDto = new NpcInfoDto();
        tNpcInfoDto.id = npc.id;
        tNpcInfoDto.name = npc.name;
        tNpcInfoDto.sceneId = npc.sceneId;
        tNpcInfoDto.x = npc.x;
        tNpcInfoDto.y = npc.y;
        tNpcInfoDto.z = npc.z;
        tNpcInfoDto.npcAppearanceId = 0;
        tNpcInfoDto.npc = npc;
        return tNpcInfoDto;
    }
    #endregion

    #region  获取对话Npc
    //	获取对话Npc
    /// <summary>
    /// 获取对话Npc -- Gets the npc talk by submit dto.
    /// </summary>
    /// <returns>The npc talk by submit dto.</returns>
    /// <param name="submitDto">Submit dto.</param>
    /// <param name="sceneID">Scene I.</param>
    static public NpcInfoDto GetNpcTalkBySubmitDto(SubmitDto submitDto,int sceneID)
    {
        NpcInfoDto tNpcInfoDto = null;
        TalkSubmitDto tTalkSubmitDto = submitDto as TalkSubmitDto;
        if(sceneID == tTalkSubmitDto.submitNpc.sceneId)
        {
            if(tTalkSubmitDto.submitNpc.npc != null)
            {
                tNpcInfoDto = tTalkSubmitDto.submitNpc;
            }
        }
        // 	当该NPC是虚拟NPC是转换为具体NPC
        if(tNpcInfoDto != null && tNpcInfoDto.npc != null && tNpcInfoDto.npc is FactionNpc)
        {
            tNpcInfoDto.npc = NpcVirturlToEntity(tNpcInfoDto.npc);
        }
        return tNpcInfoDto;
    }
    #endregion

    public static Mission CreateNewMissionByTypeId(int missionTypeId) {
        Mission mission = null;
        MissionType missionType = DataCache.getDtoByCls<MissionType>(missionTypeId);
        if(missionType != null) {
            mission = new Mission();
            mission.id = -missionTypeId;
            mission.type = missionTypeId;
            mission.name = mission.missionType.name;
        }
        return mission;
    }

    //不同任务类型,不同行为（等级）
    public static bool DoMissionSubmitTypeAction(Mission mission)
    {
        SubmitDto tSubmitDto = null;
        if (mission != null)
        {
            tSubmitDto = GetSubmitDtoByMission(mission);
        }
        if (tSubmitDto == null) return true;
        SubmitDtoType tSubmitDtoType = GetSubmitDtoTypeBySubmitDto(tSubmitDto);
        switch (tSubmitDtoType)
        {
            case SubmitDtoType.Upgrade:
                TipManager.AddTip("剧情暂时告一段落，请努力升级吧！");
                ProxyScheduleMainView.Open();
                return false;
        }
        return true;
    }
}

