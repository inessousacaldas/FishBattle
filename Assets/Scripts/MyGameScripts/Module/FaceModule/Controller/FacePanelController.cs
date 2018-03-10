// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  FacePanelViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UniRx;
using UnityEngine;
using AppDto;
using AppServices;
using ChatChannelEnum = AppDto.ChatChannel.ChatChannelEnum;
#region Nested type: ExpressionType

public enum ExpressionType
{
    Expression,
    Good,
    Pet,
    Crew,
    Skill,
    Mission,
    Appellation,
    Achievement,
    GiftMoney,
    FashionDress,
    Record,
    Mount,
    PutAwayGood,//上架
    RedPack
}

#endregion

public enum FaceShowType
{
    SHOW_TYPE_Chat,
    SHOW_TYPE_SELFZONE_SEND,
    SHOW_TYPE_SELFZONE_COMMENT
}

public partial class FacePanelViewController
{
    private const int LeftTypeBtnCount = 12;

    private static readonly ITabInfo[] TabInfos = new ITabInfo[10]{
        TabInfoData.Create((int) ExpressionType.Expression,"表情")
        , TabInfoData.Create((int) ExpressionType.Good, "物品")
        , TabInfoData.Create((int) ExpressionType.Crew, "伙伴")
        , TabInfoData.Create((int) ExpressionType.Mission, "任务")
        , TabInfoData.Create((int) ExpressionType.Skill, "技能")
        , TabInfoData.Create((int) ExpressionType.PutAwayGood, "上架")
        , TabInfoData.Create((int) ExpressionType.RedPack, "红包")
        , TabInfoData.Create((int) ExpressionType.Appellation, "称谓")
        , TabInfoData.Create((int) ExpressionType.Achievement, "成就")
        , TabInfoData.Create((int) ExpressionType.Record, "记录")
    };

    private static readonly ExpressionType[] SelfZoneShow = new ExpressionType[4]
    {
        ExpressionType.Skill
        , ExpressionType.Mission
        , ExpressionType.GiftMoney
        , ExpressionType.Record
    };
    private static readonly ExpressionType[] CommentShow = new ExpressionType[1]{ExpressionType.Expression};
    
    private const string ExpressionItemPath = "EmojiItemCell"; //表情 prefab
    private const string FriendPartnerItemCellPath = "FriendPartnerItemCell"; //宠物、伙伴prefab
    private const string HyperlinkItemCellPath = "HyperlinkItemCell"; //称谓 成就 任务 prefab
    
    private static FaceShowType _curShowType;

    public static FaceShowType CurShowType
    {
        get { return _curShowType; }
        set { _curShowType = value; }
    }

    private ChatMsgType _curType = ChatMsgType.TEXT;
    private List<EmojiItemController> _emojiItemList;
    public IEnumerable<EmojiItemController> EmojiItemList { get { return _emojiItemList; } }
    private List<ItemCellController> _backpackItemList;
    private List<Face_PetItemController> _crewItemList; //伙伴
    private List<MsgRecordItemCellController> _recordItemList;
    // 最近聊天信息列表
    private IEnumerable<ChatRecordVo> _recordList;

    private TabbtnManager faceTabMgr;
    private List<int> fashionList = new List<int>();

    private CompositeDisposable _disposable;

    #region Event
    private Subject<IFaceData> selectStream;
    public UniRx.IObservable<IFaceData> SelectStream
    {
        get { return selectStream; }
    }
    
    private Subject<Unit> submitStream;
    
    public UniRx.IObservable<Unit> SubmitStream
    {
        get { return submitStream; }
    }
    #endregion

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        _recordItemList = new List<MsgRecordItemCellController>(ChatDataMgr.ChatData.MAX_CHATRECORD_COUNT);
        _disposable = new CompositeDisposable();
        selectStream = new Subject<IFaceData>();
        submitStream = new Subject<Unit>();
        InitTabBtns();
    }
    private void InitTabBtns()
    {
        var info = GetTabNames();
        faceTabMgr = TabbtnManager.Create(
            info
            , delegate(int i) { 
                var com = AddChild<ExpressionBtnController, ExpressionBtn>(
                    View.Grid_UIGrid.gameObject
                    , TabbtnPrefabPath.ExpressionBtn.ToString());
                com.SetBtnImages(i.ToString());
                return com;
            });
        
        _disposable.Add(faceTabMgr.Stream.SubscribeAndFire(i =>
        {
            var temp = info.TryGetValue(i);
            if (temp != null)
            {
                OnTabClickHandler((ExpressionType)temp.EnumValue);
            }    
        }));
        
        View.Grid_UIGrid.Reposition();
        //OnClickEmojiBtn();
    }

    private IEnumerable<ITabInfo> GetTabNames()
    {
        IEnumerable<ITabInfo> tabs = null;
        switch (CurShowType)
        {
            case FaceShowType.SHOW_TYPE_Chat:
                tabs = TabInfos.Filter(s => IsFuncOpen((ExpressionType)s.EnumValue));
                break;
            case FaceShowType.SHOW_TYPE_SELFZONE_COMMENT:
                tabs = TabInfos.Filter(s => SelfZoneShow.Contains((ExpressionType)s.EnumValue) && IsFuncOpen((ExpressionType)s.EnumValue));
                break;
            case FaceShowType.SHOW_TYPE_SELFZONE_SEND:
                tabs = TabInfos.Filter(s => CommentShow.Contains((ExpressionType)s.EnumValue) && IsFuncOpen((ExpressionType)s.EnumValue));
                break;

        }
        return tabs;
    }
    private bool IsFuncOpen(ExpressionType value)
    {
        switch (value)
        {
//            case ExpressionType.Achievement:
//                return FunctionOpenHelper.isFuncOpen(
//                    FunctionOpen.FunctionOpenEnum_ChatAchievement, false);
//            case ExpressionType.GiftMoney:
//                return FunctionOpenHelper.isFuncOpen(
//                    FunctionOpen.FunctionOpenEnum_ChatRedPackets, false); 
            default: return true;
        }        
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        EventDelegate.Set(View.Search_UIInput.onChange, OnInputValueChange);
    }

    protected override void OnDispose()
    {
        JSTimer.Instance.CancelCd("ShowSendTips");
        _disposable.Dispose();
        _disposable = null;
        selectStream = selectStream.CloseOnceNull();
        submitStream = submitStream.CloseOnceNull();
        atMemberItemList.Clear();
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        EventDelegate.Remove(View.Search_UIInput.onChange, OnInputValueChange);
    }

    private void ShowSendTips()
    {
        View.Tips.SetActive(true);
        JSTimer.Instance.SetupCoolDown("ShowSendTips", 2f, null, () => { View.Tips.SetActive(false); });
    }

    private bool UpdateGridState(ChatMsgType type)
    {
        if (_curType == type)
            return false;
        _curType = type;
        SetActive(type);
        return true;
    }

    private void SetActive(ChatMsgType type)
    {
        if (type == ChatMsgType.Emoji)
        {
            View.emojiGrid_PageScrollView.gameObject.SetActive(true);
            View.packItemGrid_PageScrollView.gameObject.SetActive(false);
            View.petItemGrid_PageScrollView.gameObject.SetActive(false);
            View.missionItemGrid_PageScrollView.gameObject.SetActive(false);
            View.RecordGrid_PageScrollView.gameObject.SetActive(false);
        }
        else if (type == ChatMsgType.Item || type == ChatMsgType.Skill)
        {
            View.emojiGrid_PageScrollView.gameObject.SetActive(false);
            View.packItemGrid_PageScrollView.gameObject.SetActive(true);
            View.petItemGrid_PageScrollView.gameObject.SetActive(false);
            View.missionItemGrid_PageScrollView.gameObject.SetActive(false);
            View.RecordGrid_PageScrollView.gameObject.SetActive(false);
        }
        else if (type == ChatMsgType.Pet
                 || type == ChatMsgType.Crew
                 || type == ChatMsgType.Mount)
        {
            View.emojiGrid_PageScrollView.gameObject.SetActive(false);
            View.packItemGrid_PageScrollView.gameObject.SetActive(false);
            View.petItemGrid_PageScrollView.gameObject.SetActive(true);
            View.missionItemGrid_PageScrollView.gameObject.SetActive(false);
            View.RecordGrid_PageScrollView.gameObject.SetActive(false);
        }
        else if (type == ChatMsgType.Mission
                 || type == ChatMsgType.Appellation
                 || type == ChatMsgType.Achievement
                 || type == ChatMsgType.FasionDress)
        {
            View.emojiGrid_PageScrollView.gameObject.SetActive(false);
            View.packItemGrid_PageScrollView.gameObject.SetActive(false);
            View.petItemGrid_PageScrollView.gameObject.SetActive(false);
            View.missionItemGrid_PageScrollView.gameObject.SetActive(true);
            View.RecordGrid_PageScrollView.gameObject.SetActive(false);
        }
        else if (type == ChatMsgType.Record)
        {
            View.emojiGrid_PageScrollView.gameObject.SetActive(false);
            View.packItemGrid_PageScrollView.gameObject.SetActive(false);
            View.petItemGrid_PageScrollView.gameObject.SetActive(false);
            View.missionItemGrid_PageScrollView.gameObject.SetActive(false);
            View.RecordGrid_PageScrollView.gameObject.SetActive(true);
        }

        View.FashionPreview.SetActive(false);
    }

    private void OnClickSendBtn()
    {
        submitStream.OnNext(new Unit());
    }

    private void SendUrlMsg(string name, long itemId)
    {
        string content;
        string urlMsg = ChatHelper.CombineToUrl(_curType, name, itemId,out content);
        //变更输入栏内容
        selectStream.OnNext(FaceData.Create(_curType,  urlMsg, content));
        //发送消息
        //submitStream.OnNext(new Unit());
    }

    private void OnClickGiftMoneyBtn()
    {
//        ProxyChatModule.CloseFacePanel();
//        ProxyRedPacketModule.OpenMainView();

        //        UpdateGridState(ChatMsgType.Achievement);
        ////        if (!UpdateGridState(ChatMsgType.Achievement))
        ////            return;
        //
        //        if (_hyperlinkItemList != null)
        //        {
        //            for (int i = 0; i < _hyperlinkItemList.Count; i++)
        //            {
        //                _hyperlinkItemList[i].gameObject.SetActive(false);
        //            }
        //
                    View.missionItemGrid_PageScrollView.PageGrid.Reposition();
                    View.missionItemGrid_PageScrollView.SkipToPage(1, false);
        //        }
    }

    private void OnClickFashionDressBtn()
    {
        if (!UpdateGridState(ChatMsgType.FasionDress))
            return;

//        _wardrobeDto = ModelManager.Fashion.GetData();
//        if (_wardrobeDto != null)
//        {
//            if (_wardrobeDto != null)
//            {
//                for (int i = 0; i < _wardrobeDto.fashionDressDtos.Count; i++)
//                {
//                    if (_wardrobeDto.fashionDressDtos[i].wearDressList.Count > 0)
//                    {
//                        fashionList.Add(i);
//                    }
//                }
//            }
//            ShowTip(5, "");
//            //            if (fashionList.Count > 0)
//            //{
//            if (_hyperlinkItemList == null)
//                _hyperlinkItemList = new List<HyperlinkItemCellController>(fashionList.Count);
//
//            if (_hyperlinkItemList.Count < 5)
//            {
//                
//
//                for (int i = _hyperlinkItemList.Count; i < 5; i++)
//                { 
//                    GameObject item = AddCachedChild(View.missionItemGrid_PageScrollView.gameObject,HyperlinkItemCellPath);
//                    HyperlinkItemCellController com = new HyperlinkItemCellController(item);
//                    _hyperlinkItemList.Add(com);
//                }
//            }
//
//            for (int i = 0; i < 5; i++)
//            {
//                _hyperlinkItemList[i].gameObject.SetActive(true);
//                _hyperlinkItemList[i].SetSelected(false);
//                _hyperlinkItemList[i].SetData(i, string.Format("方案{0}", i + 1), OnSelectHyperlinkItem);
//                if (!fashionList.Contains(i))
//                {
//                    _hyperlinkItemList[i].SetGray(true);
//                }
//            }
//
//            for (int i = 5; i < _hyperlinkItemList.Count; i++)
//            {
//                _hyperlinkItemList[i].gameObject.SetActive(false);
//            }

            View.missionItemGrid_PageScrollView.PageGrid.Reposition();
            View.missionItemGrid_PageScrollView.SkipToPage(1, false);
            //}
//        }
    }

    private void OnClickMountBtn()
    {
        if (!UpdateGridState(ChatMsgType.Mount))
            return;

//        if (_mounts == null)
//        {
//            _mounts = ModelManager.Mount.GetAllMountsOnPlayer();
//            //            _crewInfoList.Sort((a, b) =>
//            //            {
//            //                bool aBattle = ModelManager.Crew.isCrewInBattle(a.crewDto.crewUniqueId);
//            //                bool bBattle = ModelManager.Crew.isCrewInBattle(b.crewDto.crewUniqueId);
//            //                return aBattle == bBattle ? a.crewDto.crewId.CompareTo(b.crewDto.crewId) : bBattle.CompareTo(aBattle);
//            //            });
//        }
//
//        if (ShowTip(_mounts.Count, "坐骑")) return;
//
//        if (_mounts != null)
//        {
//            if (_petItemList == null)
//                _petItemList = new List<FriendPartnerItemCellController>(_mounts.Count);
//
//            if (_petItemList.Count < _mounts.Count)
//            {
//                
//
//                for (int i = _petItemList.Count; i < _mounts.Count; i++)
//                { 
//                    GameObject item = AddCachedChild(View.petItemGrid_PageScrollView.gameObject,FriendPartnerItemCellPath);
//                    FriendPartnerItemCellController com = new FriendPartnerItemCellController(item);
//                    com.InitItem(i, OnSelectPetOrCrewItem);
//                    _petItemList.Add(com);
//                }
//            }
//
//            int playerLv = ModelManager.Player.GetPlayerLevel();
//            for (int i = 0; i < _mounts.Count; i++)
//            {
//                _petItemList[i].gameObject.SetActive(true);
//                _petItemList[i].ResetItem();
//                var mountInfo = _mounts[i];
//                _petItemList[i].SetIcon(mountInfo.mount.texture.ToString());
//                _petItemList[i].SetNameLbl(mountInfo.mount.name);
//                string typeStr = "";
//                if (_mounts[i].mount.mountType == Mount.MountType_Fly)
//                {
//                    typeStr = "飞行坐骑";
//                }
//                else if (_mounts[i].mount.mountType == Mount.MountType_Land)
//                {
//                    typeStr = "陆地坐骑";
//                }
//                else
//                {
//                    typeStr = "未知";
//                }
//                _petItemList[i].SetMountType(typeStr);
//               
//                _petItemList[i].SetBattleFlag(ModelManager.Mount.GetBattleMountId() == _mounts[i].mountId);
//            }
//
//            for (int i = _mounts.Count; i < _petItemList.Count; i++)
//            {
//                _petItemList[i].gameObject.SetActive(false);
//            }

            View.petItemGrid_PageScrollView.PageGrid.Reposition();
            View.petItemGrid_PageScrollView.SkipToPage(1, false);
//        }
        
    }
    private void OnClickRedPackBtn()
    {
        if (!UpdateGridState(ChatMsgType.RedPacket))
            return;
        
    }
    #region 聊天记录
    private void OnClickRecordBtn()
    {
        if (!UpdateGridState(ChatMsgType.Record))
            return;
        UpdateRecordPanel();
    }

    private void UpdateRecordPanel()
    {
        var i = 0;
        var cnt = _recordItemList.Count();
        _recordList = ChatDataMgr.Stream.LastValue.ChatInfoViewData.GetChatRecordList().ToList();
        //_recordList.Reverse();
        _recordList.ForEachI((x,idx)=>
        {
            MsgRecordItemCellController com = null;
            if (idx >= cnt)
            {
                com = AddCachedChild<MsgRecordItemCellController, MsgRecordItemCell>(
                    View.RecordGrid_PageScrollView.gameObject
                    ,MsgRecordItemCell.NAME);
                _recordItemList.Add(com);
                _disposable.Add(com.OnItemClick.Subscribe(_=>OnSelectRecordItem(idx)));
            }
            else
            {
                com = _recordItemList[idx];
            }
            com.Show();
            //com.SetSelect(false);
            com.UpdateView(x);
            
            i++;
        });

        ShowTip(i <= 0 , "记录");
        
        for (; i < cnt; i++)
        {
            _recordItemList[i].Hide();
        }
        
        View.RecordGrid_PageScrollView.PageGrid.Reposition();
        View.RecordGrid_PageScrollView.SkipToPage(1, false);
    }

    //private int _lastSelectRecord = -1;
    private void OnSelectRecordItem(int index)
    {
        //UpdateRecordPanel();
        var item = _recordItemList[index];
        var content = item.GetContent();
        selectStream.OnNext(FaceData.Create(_curType, content._UrlMsg, content.InputStr));
    }
    #endregion
    //    public static string GetMissionTitle(PlayerMissionDto playerMissionDto)
    //    {
    //        string txt = "";
    //        switch (playerMissionDto.mission.missionType.id)
    //        {
    //            case MissionType.MissionTypeEnum_Chain:
    //                PlayerChainMissionDto playerChainMissionDto = (PlayerChainMissionDto)playerMissionDto;
    //                txt = string.Format("{0}-{1}({2}/{3})", playerMissionDto.mission.missionType.name,
    //                    playerMissionDto.mission.name, playerChainMissionDto.curRings,
    //                    DataCache.GetStaticConfigValue(AppStaticConfigs.CHAIN_MISSION_ONE_ROUND_RINGS_COUNT, 300));
    //                break;
    //            case MissionType.MissionTypeEnum_Faction:
    //                PlayerFactionMissionDto factionMissionDto = (PlayerFactionMissionDto)playerMissionDto;
    //                txt = string.Format("{0}-{1}({2}/{3})", playerMissionDto.mission.missionType.name,
    //                    playerMissionDto.mission.name, factionMissionDto.curRings,
    //                    DataCache.GetStaticConfigValue(AppStaticConfigs.FACTION_MISSION_ONE_ROUND_RINGS_COUNT, 10));
    //                break;
    //            case MissionType.MissionTypeEnum_FactionTrial:
    //                PlayerFactionTrialMissionDto playerFactionTrialMissionDto =
    //                    (PlayerFactionTrialMissionDto)playerMissionDto;
    //                txt = string.Format("{0}-{1}({2}/{3})", playerMissionDto.mission.missionType.name,
    //                    playerMissionDto.mission.name, playerFactionTrialMissionDto.curRings,
    //                    DataCache.GetStaticConfigValue(AppStaticConfigs.FACTIONTRIAL_MISSION_ONE_ROUND_RINGS_COUNT, 9));
    //                break;
    //            case MissionType.MissionTypeEnum_Ghost:
    //                PlayerGhostMissionDto playerGhostMissionDto = (PlayerGhostMissionDto)playerMissionDto;
    //                txt = string.Format("{0}({1}/{2})", playerMissionDto.mission.missionType.name,
    //                    playerGhostMissionDto.curRings,
    //                    DataCache.GetStaticConfigValue(AppStaticConfigs.GHOST_MISSION_ONE_ROUND_RINGS_COUNT, 10));
    //                break;
    //            default:
    //                txt = string.Format("{0}-{1}", playerMissionDto.mission.missionType.name, playerMissionDto.mission.name);
    //                break;
    //        }
    //
    //        return txt;
    //    }

    private void OnlyShowTypePanel(ExpressionType pType)
    {
        View.Grid_UIGrid.transform.parent.gameObject.SetActive(false);
        UIRect tUIRect = View.ItemScrollView_UIScrollView.transform.parent.gameObject.GetComponent<UIRect>();
        tUIRect.leftAnchor.Set(0.5f,-307f);
        tUIRect.rightAnchor.Set(0.5f,307); 
        tUIRect.bottomAnchor.Set(0f, 9);
        tUIRect.topAnchor.Set(0f, 299);
        OnTabClickHandler(pType);
    }

    public void OnTabClickHandler(ExpressionType type)
    {
        switch (type)
        {
            case ExpressionType.Expression:
                OnClickEmojiBtn();
                break;
            case ExpressionType.Good:
                OnClickPackItemBtn();
                break;
            case ExpressionType.Crew:
                if (!FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum.FUN_10, true)) return;
                OnClickCrewBtn();
                break;
            case ExpressionType.Skill:
                //OnClickSkillBtn();
                break;
            case ExpressionType.Mission:
                OnClickMissionBtn();
                break;
            case ExpressionType.Appellation:
                OnClickAppellationBtn();
                break;
            case ExpressionType.Achievement:
                OnClickAchievementBtn();
                break;
            case ExpressionType.GiftMoney:
                OnClickGiftMoneyBtn();
                break;
            case ExpressionType.FashionDress:
                OnClickFashionDressBtn();
                break;
            case ExpressionType.Record:
                OnClickRecordBtn();
                break;
            case ExpressionType.Mount:
                OnClickMountBtn();
                break;
            case ExpressionType.RedPack:
                OnClickRedPackBtn();
                break;
            case ExpressionType.PutAwayGood:
            
                return;
        }
    }



    #region 表情

    private void OnClickEmojiBtn()
    {
        if (!UpdateGridState(ChatMsgType.Emoji))
            return;
        if (_emojiItemList == null)
        {
            InitEmojiItems();
        }
        if (_emojiItemList.IsNullOrEmpty())
        {
            ShowTip(true, "表情");
        }
        else
        {
            ShowTip(false);
            View.emojiGrid_PageScrollView.SkipToPage(1, false);
            View.emojiGrid_PageScrollView.UpdatePageFlagCount();
        }
        //SetActive(ChatMsgType.Emoji);
    }

    private void InitEmojiItems()
    {
        var itemPrefab = AssetPipeline. ResourcePoolManager.Instance.SpawnUIGo(EmojiItem.NAME) as GameObject;
        UISprite itemSprite = itemPrefab.GetComponent<UISprite>();
        UIAtlas emojiAtlas = itemSprite.atlas;
        if (emojiAtlas != null)
        {
            EmojiDataUtil.InitEmojiInfo(emojiAtlas);
            var emojiInfoList = new List<string>(EmojiDataUtil.GetEmojiInfo().Keys);
            var tList = new List<int>();
            for (int i = 0; i < emojiInfoList.Count; i++)
            {
                string s = emojiInfoList[i];
                int index = s.LastIndexOf('#');
                string str = s.Substring(index + 1, s.Length - index - 1);
                tList.Add(StringHelper.ToInt(str));
            }
            tList.Sort();
            emojiInfoList.Clear();
        
            for (int i = 0; i < tList.Count; i++)
            {
                emojiInfoList.Add("#" + tList[i]);
            }
            _emojiItemList = new List<EmojiItemController>(emojiInfoList.Count);

            //for (int i = 0; i < emojiInfoList.Count; i++)
            //{
            //    var prefix = emojiInfoList[i];
            //    var com = AddChild<EmojiItemController, EmojiItem>(
            //        View.emojiGrid_PageScrollView.gameObject
            //        , EmojiItem.NAME
            //        , prefix);
            //    com.UpdateView(prefix);
            //    _emojiItemList.Add(com);
            //    com.ClickHandler.Subscribe(_=>OnSelectEmojiItem(com.Prefix));
            //}
            _infoList = emojiInfoList;
            StartTimer(true);
        }            
    }

    private static readonly string TimerName = "addEmoji";
    private int _idx = 0;
    private List<string> _infoList = new List<string>();
    private bool _isFirst = true;
    private void StartTimer(bool isAni=false)
    {
        //加载完毕
        if(_idx >= _infoList.Count)
        {
            JSTimer.Instance.CancelTimer(TimerName + this.GetHashCode());

            if (_emojiItemList.IsNullOrEmpty())
            {
                ShowTip(true, "表情");
            }
            else
            {
                ShowTip(false);
                if (View.emojiGrid_PageScrollView.PageGrid != null)
                    View.emojiGrid_PageScrollView.PageGrid.Reposition();
            }

            return;
        }

        var timeGap = 0.016f;
        if (isAni)
            timeGap = 0.25f;//表情框有向上动画0.25s
        if (View.emojiGrid_PageScrollView.PageGrid == null) return;
        var maxRow = View.emojiGrid_PageScrollView.PageGrid.maxRow;
        var maxCol = View.emojiGrid_PageScrollView.PageGrid.maxCol;
        var maxCount = 8;
        var onceCount = maxCount > _infoList.Count - _idx ? _infoList.Count - _idx : maxCount;

        JSTimer.Instance.CancelTimer(TimerName + this.GetHashCode());
        JSTimer.Instance.SetupCoolDown(TimerName + this.GetHashCode(), timeGap, null, delegate
        {
            ShowTip(false);
            for (int i = 0; i < onceCount; i++)
            {
                var prefix = _infoList[i + _idx];
                var com = AddChild<EmojiItemController, EmojiItem>(
                    View.emojiGrid_PageScrollView.gameObject
                    , EmojiItem.NAME
                    , prefix);
                com.UpdateView(prefix);
                //设位置 否则能看见表情加载的动态
                com.transform.localPosition = new Vector3(1000, 0);
                _emojiItemList.Add(com);
                com.ClickHandler.Subscribe(_ => OnSelectEmojiItem(com.Prefix));
            }
            
            _idx += onceCount;
            //满一页之后刷新第一页
            if(_idx > maxRow * maxCol && _isFirst)
            {
                View.emojiGrid_PageScrollView.PageGrid.Reposition();
                _isFirst = false;
            }
                
            StartTimer();
        });
    }

    private void OnSelectEmojiItem(string prefix)
    {
        selectStream.OnNext(FaceData.Create(_curType, prefix));
    }

    #endregion

    #region 物品

    private void OnClickPackItemBtn()
    {
        if (!UpdateGridState(ChatMsgType.Item))
            return;
        if(_backpackItemList == null)
            InitPackItemListBefore();
        else
        {
            View.packItemGrid_PageScrollView.SkipToPage(1, false);
            View.packItemGrid_PageScrollView.UpdatePageFlagCount();
        }
        ShowTip(false);
    }
    private void InitPackItemListBefore()
    {
        //部分数据需要先请求
        GameUtil.GeneralReq(Services.Chat_WearItem(), resp =>
        {
            var wearItemList = resp as BagItemListDto;
            var equipmentItems = EquipmentMainDataMgr.DataMgr.GetEquipmentDtoList(EquipmentMainDataMgr.EquipmentHoldTab.Equip);
            var bagItems = BackpackDataMgr.DataMgr.GetBagItems().ToList();
            InitPackItemList(wearItemList, equipmentItems, bagItems);
            View.packItemGrid_PageScrollView.PageGrid.Reposition();
            View.packItemGrid_PageScrollView.SkipToPage(1, false);
            View.packItemGrid_PageScrollView.UpdatePageFlagCount();
        });
    }
    private void InitPackItemList(BagItemListDto wearItemList, List<EquipmentDto> equipmentItems,List<BagItemDto> bagItems) 
    {
        var tipsPos = new Vector3(-143.0f, 159.0f, 0.0f);
        _backpackItemList = new List<ItemCellController>();


        //装备的物品
        equipmentItems.ForEachI((x, i) => {
            ItemCellController itemCellCtrl = null;
            itemCellCtrl = AddCachedChild<ItemCellController, ItemCell>(View.packItemGrid_PageScrollView.gameObject, ItemCell.Prefab_BagItemCell);
            _backpackItemList.Add(itemCellCtrl);
            //装备身上的
            var tempData = x;
            itemCellCtrl.SetShowTips(false);
            bool isEquip = EquipmentMainDataMgr.DataMgr.IsEquipmentEquip(tempData);
            itemCellCtrl.UpdateEquipView(x, isEquip);
            itemCellCtrl.OnCellClick.Subscribe(evt => {
                var tipsCtrl = ProxyTips.OpenEquipmentTips(tempData,null);
                tipsCtrl.ReSetAllPos(null);
                tipsCtrl.SetBtnView("", "");
                tipsCtrl.SetTipsPosition(tipsPos);
                SendUrlMsg(tempData.equip.name, tempData.equipUid);
            });
        });

        //装备身上的结晶回路
        wearItemList.items.ForEachI((x, i) => {
            ItemCellController itemCellCtrl = null;
            itemCellCtrl = AddCachedChild<ItemCellController, ItemCell>(View.packItemGrid_PageScrollView.gameObject, ItemCell.Prefab_BagItemCell);
            _backpackItemList.Add(itemCellCtrl);  
            var tempData = x;
            itemCellCtrl.SetShowTips(false);
            itemCellCtrl.UpdateView(x);
            itemCellCtrl.OnCellClick.Subscribe(evt =>
            {
                var tipsCtrl = ProxyTips.OpenTipsWithBagItemDto(tempData, tipsPos);
                tipsCtrl.ReSetAllPos(null);
                tipsCtrl.SetBtnView("", "");
                tipsCtrl.SetTipsPosition(tipsPos);
                SendUrlMsg(tempData.item.name, tempData.uniqueId);
            });
        });
        //装备先放前面
        bagItems.Sort((a, b) =>
        {
            int av = 0;
            int bv = 0;
            if (a.item.itemType == (int)AppItem.ItemTypeEnum.Equipment)
                av = 1;
            if (b.item.itemType == (int)AppItem.ItemTypeEnum.Equipment)
                bv = 1;
            if (av != bv)
                return bv - av;
            else
                return a.index - b.index;
        });
        //背包物品
        bagItems.ForEachI((x, i) => {
            var itemCellCtrl = AddCachedChild<ItemCellController, ItemCell>(View.packItemGrid_PageScrollView.gameObject, ItemCell.Prefab_BagItemCell);
            _backpackItemList.Add(itemCellCtrl);
            itemCellCtrl.UpdateView(x);
            itemCellCtrl.SetShowTips(false);
            
            var tempData = x;
            itemCellCtrl.OnCellClick.Subscribe(evt => {
                ProxyTips.OpenTipsWithBagItemDto(tempData, tipsPos);
                //生成链接，同时弹出Tips
                if (tempData.uniqueId > 0)
                {
                    SendUrlMsg(tempData.item.name, tempData.uniqueId);
                }
                else
                {
                    SendUrlMsg(tempData.item.name, -tempData.item.id);
                }
            });   
        });
    }

    #endregion

    #region 伙伴
    private void OnClickCrewBtn()
    {
        if (!UpdateGridState(ChatMsgType.Crew))
            return;
        InitCrweListBefore();
    }

    /// <summary>
    /// 向服务器请求数据
    /// </summary>
    private void InitCrweListBefore()
    {
        if(_crewItemList == null)
        {
            if (FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum.FUN_10, true))
                GameUtil.GeneralReq<CrewShortListDto>(Services.Chat_CrewList(), resp =>
                {
                    InitCrewList(resp);
                });
        }
        else
        {
            View.petItemGrid_PageScrollView.PageGrid.Reposition();
            View.petItemGrid_PageScrollView.SkipToPage(1, false);
        }
    }
    private void InitCrewList(CrewShortListDto dtos)
    {
        _crewItemList = new List<Face_PetItemController>();
        dtos.crewShortDtos.ForEachI((x,i)=> {
            var ctrl = AddCachedChild<Face_PetItemController, Face_PetItem>(View.petItemGrid_PageScrollView.gameObject, Face_PetItem.NAME);
            _crewItemList.Add(ctrl);
           // var crewConfig = DataCache.getDtoByCls<Crew>(x.crewId);
            var crew = DataCache.getDtoByCls<GeneralCharactor>(x.crewId);
            if(crew is Crew)
            {
                var crewConfig = crew as Crew;
                ctrl.UpdateView(x, crewConfig);
                ctrl.OnFace_PetItem_UIButtonClick.Subscribe(evt =>
                {
                    SendUrlMsg(crewConfig.name, x.id);
                });
            }
            View.petItemGrid_PageScrollView.PageGrid.Reposition();
            View.petItemGrid_PageScrollView.SkipToPage(1, false);
            //ctrl.OnIconBg_UIButtonClick.Subscribe(evt =>
            //{
            //    TipManager.AddTopTip("打开伙伴详细界面");
            //});
        });
    }
    #endregion

    #region 任务 称谓 成就

    private void OnClickAppellationBtn()
    {
        if (!UpdateGridState(ChatMsgType.Appellation))
            return;

//        if (_titleInfoList == null)
//        {
//            _titleInfoList = ModelManager.Player.GetTitleList();
//        }
//
//        if (ShowTip(_titleInfoList.Count, "称谓")) return;
//
//        if (_titleInfoList != null)
//        {
//            if (_hyperlinkItemList == null)
//                _hyperlinkItemList = new List<HyperlinkItemCellController>(_titleInfoList.Count);
//
//            if (_hyperlinkItemList.Count < _titleInfoList.Count)
//            {
//                
//
//                for (int i = _hyperlinkItemList.Count; i < _titleInfoList.Count; i++)
//                { 
//                    GameObject item = AddCachedChild(View.missionItemGrid_PageScrollView.gameObject,HyperlinkItemCellPath);
//                    HyperlinkItemCellController com = new HyperlinkItemCellController(item);
//                    _hyperlinkItemList.Add(com);
//                }
//            }
//
//            for (int i = 0; i < _titleInfoList.Count; i++)
//            {
//                _hyperlinkItemList[i].gameObject.SetActive(true);
//                _hyperlinkItemList[i].SetSelected(false);
//                _hyperlinkItemList[i].SetData(i, _titleInfoList[i].titleName.StripChatSymbols(), OnSelectHyperlinkItem);
//            }
//
//            for (int i = _titleInfoList.Count; i < _hyperlinkItemList.Count; i++)
//            {
//                _hyperlinkItemList[i].gameObject.SetActive(false);
//            }
//
            View.missionItemGrid_PageScrollView.PageGrid.Reposition();
            View.missionItemGrid_PageScrollView.SkipToPage(1, false);
//        }
    }

    private void ShowTip(bool show, string str = "")
    {
        View.NoTips_UILabel.gameObject.SetActive(show);
        View.pageFlagGrid_Go.SetActive(!show);
        if (show)
        {
            View.NoTips_UILabel.text = string.Format("你暂时没有{0}", str);    
        }
    }

    private void OnClickMissionBtn()
    {
        if (!UpdateGridState(ChatMsgType.Mission))
            return;

//        if (_missionInfoList == null)
//        {
//            _missionInfoList = ModelManager.MissionData.GetPlayerMissionDtoList();
//        }
//
//        if (ShowTip(_missionInfoList.Count, "任务")) return;
//
//        if (_missionInfoList != null)
//        {
//            if (_hyperlinkItemList == null)
//                _hyperlinkItemList = new List<HyperlinkItemCellController>(_missionInfoList.Count);
//
//            if (_hyperlinkItemList.Count < _missionInfoList.Count)
//            {
//                
//
//                for (int i = _hyperlinkItemList.Count; i < _missionInfoList.Count; i++)
//                { 
//                    GameObject item = AddCachedChild(View.missionItemGrid_PageScrollView.gameObject,HyperlinkItemCellPath);
//                    HyperlinkItemCellController com = new HyperlinkItemCellController(item);
//                    _hyperlinkItemList.Add(com);
//                }
//            }
//
//            for (int i = 0; i < _missionInfoList.Count; i++)
//            {
//                _hyperlinkItemList[i].gameObject.SetActive(true);
//                _hyperlinkItemList[i].SetSelected(false);
//                _hyperlinkItemList[i].SetData(i,
//                    MissionHelper.GetMissionTitleNameInDialogue(_missionInfoList[i].mission, true),
//                    OnSelectHyperlinkItem);
//                //(string.Format("{0}-{1}", _missionInfoList[i].mission.missionType.name, ModelManager.MissionData.IsBuffyMissionType(_missionInfoList[i].mission)? "" : _missionInfoList[i].mission.name));
//            }
//
//            for (int i = _missionInfoList.Count; i < _hyperlinkItemList.Count; i++)
//            {
//                _hyperlinkItemList[i].gameObject.SetActive(false);
//            }
//
//            View.missionItemGrid_PageScrollView.PageGrid.Reposition();
//            View.missionItemGrid_PageScrollView.SkipToPage(1, false);
//        }
    }

//    private HyperlinkItemCellController _lastHyperlinkItem;

    private void OnSelectHyperlinkItem(int index)
    {
//        if (_lastHyperlinkItem != null)
//            _lastHyperlinkItem.SetSelected(false);
//
//        if (_lastHyperlinkItem != _hyperlinkItemList[index])
//        {
//            _lastHyperlinkItem = _hyperlinkItemList[index];
//            _lastHyperlinkItem.SetSelected(true);
//
//
//            if (_curType == ChatMsgType.FasionDress)
//            {
//                if (!fashionList.Contains(index))
//                {
//                    TipManager.AddTip("该套方案没有时装");
//                    View.FashionPreview.SetActive(false);
//                    return;
//                }
//                ShowSendTips();
//                if (_modelController == null)
//                {
//                    _modelController = ModelDisplayController.GenerateUICom(View.ModelAnchor_Transform);
//                    _modelController.Init(300, 300);
//                }
//
//                View.FashionPreview.SetActive(true);
//                List<int> fashionIds = new List<int>();
//                for (int i = 0; i < _wardrobeDto.fashionDressDtos[index].wearDressList.Count; i++)
//                {
//                    fashionIds.Add(_wardrobeDto.fashionDressDtos[index].wearDressList[i].itemId);
//                }
//
//                _modelController.SetupModelByFashionIds(
//                    ModelManager.Player.GetPlayer().charactor,
//                    fashionIds,
//                    ModelManager.Backpack.GetCurrentWeaponModel(),
//                    ModelManager.Player.GetPlayer().dressInfoDto.weaponEffect,
//                    ModelManager.Backpack.GetCurrentHallowSpriteId());
//                _modelController.SetupFasionAnimationList(ModelHelper.Anim_idle);
//            }
//            else
//                ShowSendTips();
//        }
//        else
//        {
//            if (_curType == ChatMsgType.Mission)
//            {
//                if (index < _missionInfoList.Count)
//                {
//                    PlayerMissionDto missionDto = _missionInfoList[index];
//                    SendUrlMsg(GetMissionTitle(missionDto), missionDto.missionId);
//                }
//            }
//            else if (_curType == ChatMsgType.Appellation)
//            {
//                if (index < _titleInfoList.Count)
//                {
//                    PlayerTitleDto titleDto = _titleInfoList[index];
//                    SendUrlMsg(titleDto.titleName.StripChatSymbols(), titleDto.titleId);
//                }
//            }
//            else if (_curType == ChatMsgType.Achievement)
//            {
//                if (index < _achievementStageList.Count)
//                {
//                    if (index == 0)
//                    {
//                        SendUrlMsg("成就总览", -1);
//                    }
//                    else
//                    {
//                        PlayerAchievementStageDto stageDto = _achievementStageList[index];
//                        SendUrlMsg(stageDto.achievementStage.name, stageDto.achievementStageId);
//                    }
//                }
//            }
//            else if (_curType == ChatMsgType.FasionDress)
//            {
//                if (!fashionList.Contains(index)) //TODO
//                {
//                    TipManager.AddTip("该套方案没有时装");
//                }
//                else
//                {
//                    SendUrlMsg("我的时装方案", index);
//                }
//            }
//        }
    }

    private void OnClickAchievementBtn()
    {
//        if (ModelManager.Achievement.Dto() == null)
//        {
//            GameEventCenter.AddListener(GameEvent.Achievement_backData, ShowAchievementListInfo);
//            ModelManager.Achievement.GetMyAchievementInfo();
//        }
//        else
//        {
            ShowAchievementListInfo();
//        }
    }
//
    private void ShowAchievementListInfo()
    {
//        GameEventCenter.RemoveListener(GameEvent.Achievement_backData, ShowAchievementListInfo);
//        if (_curIndex != (int)System.Linq.Expressions.ExpressionType.Achievement)
//        {
//            return;
//        }
//        if (!UpdateGridState(ChatMsgType.Achievement))
//            return;
//
//        _achievementStageList = ModelManager.Achievement.GetLatestCompleteStageDtoList();
//
//        if (ShowTip(_achievementStageList.Count, "成就")) return;
//
//        if (_achievementStageList.Count > 0)
//        {
//            if (_hyperlinkItemList == null)
//                _hyperlinkItemList = new List<HyperlinkItemCellController>(_achievementStageList.Count);
//
//            if (_hyperlinkItemList.Count < _achievementStageList.Count)
//            {
//                
//
//                for (int i = _hyperlinkItemList.Count; i < _achievementStageList.Count; i++)
//                { 
//                    GameObject item = AddCachedChild(View.missionItemGrid_PageScrollView.gameObject,HyperlinkItemCellPath);
//                    HyperlinkItemCellController com = new HyperlinkItemCellController(item);
//                    _hyperlinkItemList.Add(com);
//                }
//            }
//
//            for (int i = 0; i < _achievementStageList.Count; i++)
//            {
//                _hyperlinkItemList[i].gameObject.SetActive(true);
//                _hyperlinkItemList[i].SetSelected(false);
//
//                string LinkName = i == 0 ? "成就总览" : _achievementStageList[i].achievementStage.name;
//                _hyperlinkItemList[i].SetData(i, LinkName, OnSelectHyperlinkItem);
//            }
//
//            for (int i = _achievementStageList.Count; i < _hyperlinkItemList.Count; i++)
//            {
//                _hyperlinkItemList[i].gameObject.SetActive(false);
//            }
//
            //View.missionItemGrid_PageScrollView.PageGrid.Reposition();
           // View.missionItemGrid_PageScrollView.SkipToPage(1, false);
//        }
    }

    #endregion

    public float GetHight()
    {
        return 200;
    }
    #region @功能
    private List<ChatAtItemController> atMemberItemList = new List<ChatAtItemController>();
    public ChatChannelEnum curChannel = ChatChannelEnum.Unknown;
    public bool IsShowFace
    {
        get { return View.Face_Go.activeSelf; }
    }

    private Subject<TeamMemberDto> atItemClick = new Subject<TeamMemberDto>();
    public UniRx.IObservable<TeamMemberDto> AtItemClick{ get { return atItemClick; } }
    public void UpdateTeamMember(TeamDto teamdto, ChatChannelEnum channel)
    {
        if (teamdto == null || curChannel == channel) return;
        curChannel = channel;
        int poolCount = atMemberItemList.Count;
        var list = teamdto.members;
        list = list.Filter(e => e.id != ModelManager.Player.GetPlayerId()).ToList();//剔除自己
        int count = list.Count;
        for(int i = 0; i < poolCount; i++)
        {
            atMemberItemList[i].gameObject.SetActive(i < count);
            if (i < count)
            {
                var val = list[i];
                atMemberItemList[i].UpdateView(val);
            }
        }
        for (; poolCount < count; poolCount++)
        {
            var val = list[poolCount];
            var ctrl = AddChild<ChatAtItemController, ChatAtItem>(
                View.atGrid_UIGrid.gameObject,
                ChatAtItem.NAME
                );
            ctrl.UpdateView(val);
            _disposable.Add(ctrl.Onicon_UIButtonClick.Subscribe(e => atItemClick.OnNext(ctrl.teamMemDto)));
            atMemberItemList.Add(ctrl);
        }
        View.atGrid_UIGrid.Reposition();
    }

    public void OnSearchBtnClick()
    {
        string msg = View.Search_UIInput.value;
        atMemberItemList.ForEach(e =>
        {
            if (!e.teamMemDto.nickname.Contains(msg))
            {
                e.View.Hide();
            }
            else
            {
                e.View.Show();
            }
        });
    }
    private void OnInputValueChange()
    {
        string msg = View.Search_UIInput.value;
        if (string.IsNullOrEmpty(msg))
        {
            atMemberItemList.ForEach(e =>
            {
                e.View.Show();
            });
        }
    }

    #endregion
}

public interface IFaceData
{
    ChatMsgType Type { get; }
    string Content { get; }
    
    string UrlMsg { get; }
}

public class FaceData : IFaceData
{
    public ChatMsgType _type;

    public ChatMsgType Type
    {
        get { return _type; }
    }

    public string Content
    {
        get { return _content; }
    }


    public string UrlMsg
    {
        get
        {
            return _urlMsg;
        }
    }

    public string _content;
    public string _urlMsg;

    public static IFaceData Create(ChatMsgType curType, string prefix)
    {
        var data = new FaceData();
        data._type = curType;
        data._content = prefix;
        return data;
    }
    public static IFaceData Create(ChatMsgType curType,string urlMsg,string content)
    {
        var data = new FaceData();
        data._type = curType;
        data._urlMsg = urlMsg;
        data._content = content;
        return data;
    }

   
}