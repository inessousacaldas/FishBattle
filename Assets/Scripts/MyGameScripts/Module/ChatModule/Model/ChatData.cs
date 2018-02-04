using System;
using System.Collections.Generic;
using AppDto;
using AssetPipeline;
using ChatChannelEnum = AppDto.ChatChannel.ChatChannelEnum;
using System.Text.RegularExpressions;
using LITJson;


// 聊天主界面的聊天信息分页
public  interface IChatInfoViewData
{
    /// <summary>
    /// 允许的频道列表~Todo:频道的显示有可能变化
    /// </summary>
    IEnumerable<ChatChannelEnum> AllowChannelIDList { get; }

    /// <summary>
    /// 根据频道获取消息
    /// </summary>
    /// <param name="dataCurChannelId"></param>
    /// <returns></returns>
    IEnumerable<ChatNotify> GetChannelNotifyQueue(ChatChannelEnum dataCurChannelId);
    /// <summary>
    /// 当前选中的标签页
    /// </summary>
    ChatChannelEnum CurChannelId { get; set; }
    //每个频道新消息的数量
    Dictionary<ChatChannelEnum, int> NewMsgCnt { get; }

    /// <summary>
    /// 缓存的输入框消息
    /// </summary>
    //Dictionary<ChatChannelEnum,ChatViewData> cacheInputMsg { get; }
    //是否锁屏
    bool LockState { get; }

    /// <summary>
    /// 获取最近发送消息的记录
    /// </summary>
    /// <returns></returns>
    IEnumerable<ChatRecordVo> GetChatRecordList();

    /// <summary>
    /// 添加自己发送消息的记录
    /// </summary>
    /// <param name="content"></param>
    void AddChatRecord(ChatRecordVo content);
    ChatViewData chatViewData { get; }

    bool isMoveUpFaceContent { get; }
    //当前正在使用的喇叭
    int CurHordId { get; set; }
    IEnumerable<ChatPropsConsume> HornList { get; }
}
public class ChatRecordVo
{
    public string InputStr;
    public string _UrlMsg;
    public ChatRecordVo(string inputStr,string urlMsg)
    {
        this.InputStr = inputStr;
        this._UrlMsg = urlMsg;
    }
}

/// <summary>
/// 聊天设定
/// </summary>
public class ChatSettingData
{
    public const string PLAYERPREF_ChatSettingData = "PLAYERPREF_ChatSettingData";
    public Dictionary<string, bool> channelShowDic = new Dictionary<string, bool>();
    public Dictionary<string, bool> channelAutoPlayVoice = new Dictionary<string, bool>();

    public static List<ChatChannel.ChatChannelEnum> ShowChannelList = new List<ChatChannel.ChatChannelEnum>() {
            //ChatChannel.ChatChannelEnum.Horn,
             ChatChannel.ChatChannelEnum.Country,
              ChatChannel.ChatChannelEnum.Guild,
            ChatChannel.ChatChannelEnum.Team,
             ChatChannel.ChatChannelEnum.Current,
             ChatChannel.ChatChannelEnum.System
        };
    public static List<ChatChannel.ChatChannelEnum> ChannelAutoPlayList = new List<ChatChannel.ChatChannelEnum>() {
            ChatChannel.ChatChannelEnum.Country,
            ChatChannel.ChatChannelEnum.Guild,
            ChatChannel.ChatChannelEnum.Team,
        };
    public static ChatSettingData LoadData()
    {
        PlayerPrefsExt.DeletePlayerKey(PLAYERPREF_ChatSettingData);
        var json = PlayerPrefsExt.GetPlayerString(PLAYERPREF_ChatSettingData);
        var data = JsonMapper.ToObject<ChatSettingData>(json);
        if (data == null)
        {
            data = new ChatSettingData();
            SetDefalutValue(data);
        }
            
        GameDebuger.Log("Load ChatSettingData" + json);
        
        return data;
    }

    /// <summary>
    /// 设置默认值为全部勾选
    /// </summary>
    /// <param name="data"></param>
    private static void SetDefalutValue(ChatSettingData data)
    {
        ShowChannelList.ForEach(x => {
            data.channelShowDic.Add(x.ToString(), true);
        });
        ChannelAutoPlayList.ForEach(x =>
        {
            data.channelAutoPlayVoice.Add(x.ToString(), true);
        });
    }
    public static void SaveData(ChatSettingData data)
    {
        var json = JsonMapper.ToJson(data);
        PlayerPrefsExt.SetPlayerString(PLAYERPREF_ChatSettingData, json);
        GameDebuger.Log("Save ChatSettingData" + json);
    }
}
public interface IChatBoxData
{
    IEnumerable<ChatNotify> GetChatBoxMsgs();
    /// <summary>
    /// 当前队伍语音
    /// </summary>
    TeamVoiceMode CurTeamVoiceMode { get; }
    Queue<ChatNotify> GetChatHornMsgs();
    int ChatBox_MaxCount { get; }
    bool LockState { get; set; }
    List<int> ChatBoxNewMsgCnt { get; set; }
}
public interface IChatData
{
    IChatBoxData ChatBoxData { get;}
    IChatInfoViewData ChatInfoViewData { get; }
    //聊天的配置
    ChatSettingData chatSettingData { get; }
}

/// <summary>
/// 实时语音聊天模式
/// </summary>
public enum TeamVoiceMode
{
    Mut,//静音，
    OnlyVoice, // 只有声音
    All, //全部开启
}
public sealed partial class ChatDataMgr
{
    /// <summary>
    /// 主界面的聊天框显示
    /// </summary>
    public class ChatBoxData : IChatBoxData
    {
        //记录上限//
        public const int CHATBOX_MAXCOUNT = 100;
        //喇叭上线
        public const int CHATHORN_MAXCOUNT = 100;
        //主界面聊天面板显示内容频道
        private Queue<ChatNotify> _chatBoxNotifyQueue;
        //主界面聊天面板喇叭
        private Queue<ChatNotify> _chatHornNotifyQueue;
        public TeamVoiceMode CurTeamVoiceMode { get; set; }
        private bool lockState;
        public bool LockState
        {
            get { return lockState; }
            set { lockState = value; }
        }
        //主界面新消息数量
        private List<int> _chatBoxNewMsgCnt;
        public List<int> ChatBoxNewMsgCnt
        {
            get { return _chatBoxNewMsgCnt; }
            set { _chatBoxNewMsgCnt = value; }
        }
        public IEnumerable<ChatNotify> GetChatBoxMsgs()
        {
            //DataMgr._data.chatSettingData.
            return _chatBoxNotifyQueue;
        }
        public int ChatBox_MaxCount
        {
            get { return CHATBOX_MAXCOUNT; }
        }
        public void AddChatBoxQueue(ChatNotify notify, bool delay = false)
        {
            //是否屏蔽
            var receive = DataMgr.CheckReceive((ChatChannelEnum)notify.channelId);
            if (receive)
            {
                if (_chatBoxNotifyQueue.Count >= CHATBOX_MAXCOUNT)
                {
                    _chatBoxNotifyQueue.Dequeue();
                }
                _chatBoxNotifyQueue.Enqueue(notify);
            }
            if ((ChatChannelEnum)notify.channelId == ChatChannelEnum.Horn)
            {
                if (_chatHornNotifyQueue.Count >= CHATHORN_MAXCOUNT)
                {
                    _chatHornNotifyQueue.Dequeue();
                }
                _chatHornNotifyQueue.Enqueue(notify);
            }
        }

        public Queue<ChatNotify> GetChatHornMsgs()
        {
            return _chatHornNotifyQueue;
        }

        public void InitData()
        {
            _chatBoxNotifyQueue = new Queue<ChatNotify>(CHATBOX_MAXCOUNT);
            _chatHornNotifyQueue = new Queue<ChatNotify>();
            _chatBoxNewMsgCnt = new List<int> { 0 };
            lockState = true;
        }
    }
    public sealed partial class ChatData
        : IChatData
            , IChatInfoViewData
    {
        #region ChatInfoView的Tab
        //所有频道名字,这里决定了客户端界面显示顺序
        public static readonly ITabInfo[] mBtnNames =
        {
            TabInfoData.Create((int) ChatChannelEnum.System, ChatHelper.GetChannelName(ChatChannelEnum.System)),
            TabInfoData.Create((int) ChatChannelEnum.Hearsay, ChatHelper.GetChannelName(ChatChannelEnum.Hearsay)),
            TabInfoData.Create((int) ChatChannelEnum.Horn, ChatHelper.GetChannelName(ChatChannelEnum.Horn)),
            //TabInfoData.Create((int) ChatChannelEnum.Country, ChatHelper.GetChannelName(ChatChannelEnum.Country)),
            //TabInfoData.Create((int) ChatChannelEnum.Guild, ChatHelper.GetChannelName(ChatChannelEnum.Guild)),
            TabInfoData.Create((int) ChatChannelEnum.Team, ChatHelper.GetChannelName(ChatChannelEnum.Team)),
            TabInfoData.Create((int) ChatChannelEnum.Current, ChatHelper.GetChannelName(ChatChannelEnum.Current)),
        };

        public static List<ITabInfo> pageNames = new List<ITabInfo>{
            TabInfoData.Create((int) ChatPageTab.Chat, "聊天"),
            TabInfoData.Create((int) ChatPageTab.PrivateMsg, "私信"),
            TabInfoData.Create((int) ChatPageTab.Friend, "好友"),
            TabInfoData.Create((int) ChatPageTab.Email, "邮箱"),
        };

        public static int GetPageIdx(ChatPageTab tab)
        {
            return pageNames.FindElementIdx(s => (ChatPageTab)s.EnumValue == tab);
        }
        #endregion

        //各频道记录上限//
        public const int MAX_COUNT = 100;
        public const int MAX_CHATRECORD_COUNT = 10; //聊天记录的个数
        public const int ChatMsgLen = 120; //聊天信息最大长度
        private const int MinPageCount = 18;
        private static readonly string CHAT_RECORD_DIR = GameResPath.persistentDataPath + "/chatRecord";
        
        //主界面的聊天框显示
        public ChatBoxData _chatBoxData;
        public IChatBoxData ChatBoxData
        {
            get { return _chatBoxData; }
        }

        public IChatInfoViewData ChatInfoViewData
        {
            get { return this; }
        }
        public ChatData()
        {

        }

        public void Dispose()
        {

        }

        public void InitData()
        {
            allowChannelIDList = IntEnumHelper.getEnums<ChatChannelEnum>().ToList();
            allowChannelIDList.RemoveAt(0);
            channelMsgDic = new Dictionary<ChatChannelEnum, Queue<ChatNotify>>(new GenericEnumComparer<ChatChannelEnum>());
            allowChannelIDList.ForEach(s => channelMsgDic.Add(s, new Queue<ChatNotify>(MAX_COUNT)));
            curChannelID = ChatChannelEnum.Current;
            _chatRecordQueue = new Queue<ChatRecordVo>(MAX_CHATRECORD_COUNT);
            _newMsgCnt = new Dictionary<ChatChannelEnum, int>(new GenericEnumComparer<ChatChannelEnum>())
            {
                {ChatChannelEnum.Guild, 0}
                , {ChatChannelEnum.Team, 0}
                , {ChatChannelEnum.Country, 0}
                , {ChatChannelEnum.Current, 0}
                , {ChatChannelEnum.Horn, 0}
                , {ChatChannelEnum.Friend,0 }
                , {ChatChannelEnum.System,0 }
            };
            _chatBoxData = new ChatDataMgr.ChatBoxData();
            _chatBoxData.InitData();
            _chatViewData = new ChatViewData();
            _chatSettingData = ChatSettingData.LoadData();
            var horn = HornList;
            curHordID = horn.ToArray()[0].id;
            SetUpChatTimer(ChatChannelEnum.Horn);
        }


        #region ServerData

        private Dictionary<ChatChannelEnum, Queue<ChatNotify>> channelMsgDic;

        public List<ChatChannelEnum> allowChannelIDList = new List<ChatChannelEnum>(10);
        #endregion

        #region ClientData
        // 最近发送消息记录
        public Queue<ChatRecordVo> _chatRecordQueue;

        private ChatChannelEnum curChannelID;

        public ChatChannelEnum CurChannelId
        {
            get { return curChannelID; }
            set
            {
                if (!curChannelID.Equals(value))
                {

                }
                curChannelID = value;
            }
        }
        
        private List<ChatPropsConsume> hornDic;
        public IEnumerable<ChatPropsConsume> HornList
        {
            get
            {
                if(hornDic == null)
                {
                    hornDic = DataCache.getArrayByCls<ChatPropsConsume>();
                }
                return hornDic;
            }
        }
        private int curHordID = -1;
        public int CurHordId
        {
            get
            {
                return curHordID;
            }
            set
            {
                curHordID = value;
            }
        }
        //各频道新消息数量
        private Dictionary<ChatChannelEnum, int> _newMsgCnt;

        public Dictionary<ChatChannelEnum, int> NewMsgCnt
        {
            get { return _newMsgCnt; }
            set
            {
                _newMsgCnt = value;
            }
        }

        private bool _lockState;

        public bool LockState
        {
            get { return _lockState; }
            set
            {
                _lockState = value;
            }
        }

        public IEnumerable<ChatRecordVo> GetChatRecordList()
        {
            return _chatRecordQueue;
        }

        #endregion

        public IEnumerable<ChatChannelEnum> AllowChannelIDList
        {
            get { return allowChannelIDList;}
        }
        private ChatViewData _chatViewData;
        public ChatViewData chatViewData
        {
            get
            {
                //if (_newMsgCnt.ContainsKey(CurChannelId))
                    //_chatViewData.UnrealCnt = _newMsgCnt[CurChannelId];
                _chatViewData.newMsgCnt = _newMsgCnt;
                return _chatViewData;
            }
            set
            {
                _chatViewData = value;
            }
        }
        private ChatSettingData _chatSettingData;
        public ChatSettingData chatSettingData { get { return _chatSettingData; } }
        public static ITabInfo[] ShowChannelBtnNames
        {
            get
            {
                return ChatDataMgr.ChatData.mBtnNames.Filter(s =>
                    DataMgr._data.AllowChannelIDList.FindElementIdx(id =>
                        id.Equals((ChatChannelEnum)s.EnumValue)) >= 0).ToArray();
            }
        }

        public bool isMoveUpFaceContent { get; set; }

        //Dictionary<ChatChannelEnum, ChatViewData> _cacheInputMsg;
        //public Dictionary<ChatChannelEnum, ChatViewData> cacheInputMsg
        //{
        //    get
        //    {
        //        return _cacheInputMsg;
        //    }
        //}

        public IEnumerable<ChatNotify> GetChannelNotifyQueue(ChatChannelEnum channelId)
        {
            Queue<ChatNotify> queue = null;
            channelMsgDic.TryGetValue(channelId, out queue);
            return queue.ToList().GetElememtsByRange(-1, MAX_COUNT);
        }

        public void AddChatNotify(ChatNotify notify)
        {
            if (notify == null) return;
            if (channelMsgDic.ContainsKey((ChatChannelEnum)notify.channelId))
            {
                //填充翻译内容
                if (ChatHelper.FillTranslationContent(notify, channelMsgDic[(ChatChannelEnum)notify.channelId]))
                {


                }
                else
                {
                    if (!CheckPlayerLv(notify)) return; //如果等级不够，不显示聊天信息
                    AddNotify(notify);
                    if (notify.actionType == (int)ChatChannel.ActionTypeEnum.Tip)
                    {
                        string tTip = string.Format("{0} {1}", "系统".WrapColor(ColorConstantV3.Color_Red_Str),
                        ColorHelper.UpdateColorInDeepStyle(notify.content));
                        TipManager.AddTip(tTip);
                    }
                    _chatBoxData.AddChatBoxQueue(notify);
                }
            }
        }

        //目前当前 喇叭频道 只有15级以上的人才能接收和发送
        private bool CheckPlayerLv(ChatNotify notify)
        {
            int lv = ModelManager.Player.GetPlayerLevel();
            ChatChannelEnum channel = (ChatChannelEnum)notify.channelId;
            if (channel == ChatChannelEnum.Current || channel == ChatChannelEnum.Horn)
                return lv >= 15;
            return true;
        }

        private void AddNotify(ChatNotify notify)
        {
            Queue<ChatNotify> queue = null;
            channelMsgDic.TryGetValue((ChatChannelEnum)notify.channelId, out queue);

            if (queue != null)
            {
                if (queue.Count >= MAX_COUNT)
                {
                    queue.Dequeue();
                }
                queue.Enqueue(notify);
            }

            //1.如果锁屏状态下,收到该频道消息
            //2.如果收到别的频道的消息

            if((!LockState && notify.channelId == (int)CurChannelId) || notify.channelId != (int)CurChannelId)
            {
                if (!_newMsgCnt.ContainsKey((ChatChannelEnum)notify.channelId))
                {
                    GameDebuger.Log("不存在此频道 " + (ChatChannelEnum)notify.channelId);
                }
                else
                {
                    var msgCount = _newMsgCnt[(ChatChannelEnum)notify.channelId];
                    _newMsgCnt[(ChatChannelEnum)notify.channelId] = ++msgCount;
                }
            }
            if (!ChatBoxData.LockState)
            {
                var msgCnt = _chatBoxData.ChatBoxNewMsgCnt[0];
                _chatBoxData.ChatBoxNewMsgCnt[0] = ++msgCnt;
            }
            CheckAtFunc(notify);
        }

        private void CheckAtFunc(ChatNotify notify)
        {
            string msg = notify.content;
            string pattern = @"@" + ModelManager.Player.GetPlayerName();
            Match m = Regex.Match(msg, pattern);
            if (m.Success)
            {
                if(_chatViewData != null)
                {
                    _chatViewData.finalyAt = notify;
                }
            }
        }
        
        public void AddChatRecord(ChatRecordVo chatMsg)
        {
            if (chatMsg == null || string.IsNullOrEmpty(chatMsg.InputStr))
                return;

            if (_chatRecordQueue.Contains(chatMsg))
                return;

            if (_chatRecordQueue.Count >= MAX_CHATRECORD_COUNT)
            {
                _chatRecordQueue.Dequeue();
            }

            _chatRecordQueue.Enqueue(chatMsg);
        }
        public TeamDto GetTeamData()
        {
            return TeamDataMgr.DataMgr.GetSelfTeamDto();
        }

        //设置各频道定时器
        public void SetUpChatTimer(ChatChannelEnum chatChannel)
        {
            JSTimer.CdTask cdTask = null;
            switch (chatChannel)
            {
                case ChatChannelEnum.Horn:
                    cdTask = JSTimer.Instance.SetupCoolDown(ChatChannelEnum.Horn.ToString(), GetHornChatTimer, null, delegate { JSTimer.Instance.CancelTimer(ChatPageViewLogic.ChatHornInput); });
                    break;
            }
        }

        private float GetHornChatTimer
        {
            get
            {
                float playerLv = ModelManager.Player.GetPlayerLevel();
                float severLv = ModelManager.Player.ServerGrade;
                float sub = Math.Max(30, (severLv - playerLv + 20));
                return sub;
            }
        }
    }

    public IEnumerable<ChatRecordVo> GetChatRecordList()
    {
        return _data._chatRecordQueue;
    }
    
    //模拟消息
    public void MakeChatNotify(object obj)
    {
        if(obj is ItemTipsNotify)
        {
            var itemTipsNotify = obj as ItemTipsNotify;
            MakeItemTipsNotify(itemTipsNotify);
        }
        else if(obj is FriendOnlineNotify)
        {
            var fOnlineNotify = obj as FriendOnlineNotify;
            MakeFriendOnlineNotify(fOnlineNotify);
        }
        else if (obj is TeamShoutNotify)
        {
            var teamShoutNotify = obj as TeamShoutNotify;
            MakeTeamShoutNotify(teamShoutNotify);
        }
        else if (obj is FriendFlowersNotify)
        {
            var flowerShoutNotify = obj as FriendFlowersNotify;
            MakeFlowerShoutNotify(flowerShoutNotify);
        }
        FireData();
    }

    //获得奖励提示，模拟消息（主界面聊天框显示）
    private void MakeItemTipsNotify(ItemTipsNotify notify)
    {
        var reward = notify.itemDtos;
        string des = "";
        reward.ForEach(e =>
        {
            var item = ItemHelper.GetGeneralItemByItemId(e.itemId);
            if (item is AppVirtualItem)
            {
                des = "获得" + item.name + e.count;
            }
            else
            {
                des = "获得" + item.name + "*" + e.count;
            }
            ChatNotify chatNotify = new ChatNotify() { channelId = (int)ChatChannelEnum.System, content = des, lableType = 1 };
            _data.AddChatNotify(chatNotify);
        });
    }

    //好友上线
    private void MakeFriendOnlineNotify(FriendOnlineNotify notify)
    {
        if (notify.online)
        {
            var msg = FriendDataMgr.DataMgr.GetFriendDtoById(notify.id);
            if (msg != null)
            {
                string content = "";
                string res = "";
                res = ChatHelper.CombineToUrl(ChatMsgType.ChatFriend, "[打个招呼]".WrapColor(ColorConstantV3.Color_ChatGreen), notify.id, out content);
                string des = "你的好友" + msg.name.WrapColor(ColorConstantV3.Color_ChatGreen) + "上线了，赶紧去打个招呼吧！"+ res;
                ChatNotify chatNotify = new ChatNotify() { channelId = (int)ChatChannelEnum.System, content = des, lableType = 1 };
                _data.AddChatNotify(chatNotify);
            }
        }
    }

    private void MakeTeamShoutNotify(TeamShoutNotify notify)
    {
        ShortPlayerDto playerDto = new ShortPlayerDto();
        playerDto.id = notify.leaderId;
        playerDto.nickname = notify.leaderName;
        playerDto.grade = notify.grade;
        playerDto.factionId= notify.factionId;
        playerDto.charactorId = notify.dress.charactorId;
        playerDto.charactor = notify.dress.charactor as MainCharactor;
        ChatNotify chatNotify = new ChatNotify()
        {
            channelId = (int) ChatChannelEnum.Team,
            content = notify.content,
            fromPlayer = playerDto,
            lableType = 1,
        };
        _data.AddChatNotify(chatNotify);
    }

    //收到鲜花
    private void MakeFlowerShoutNotify(FriendFlowersNotify notify)
    {
        if(notify.toId == ModelManager.Player.GetPlayerId())
        {
            var itemData = ItemHelper.GetGeneralItemByItemId(notify.itemId);
            if (itemData == null) return;
            var des = string.Format("{0}向你赠送{1}朵{2}，并对你说：{3}", notify.fromName, notify.flowersCount, itemData.name, notify.content);
            ChatNotify chatNotify = new ChatNotify() { channelId = (int)ChatChannelEnum.System, content = des, lableType = 1 };
            _data.AddChatNotify(chatNotify);
        }
    }
}
