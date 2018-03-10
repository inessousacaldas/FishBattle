// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : xjd
// Created  : 10/18/2017 3:02:07 PM
// **********************************************************************

using System.Collections.Generic;
using AppDto;

public interface IPrivateMsgData
{
    int UnreadCnt { get; }

    bool LockState { get; }

    bool isScrollToBot { get; set; }

    long CurMsgId { get; set; }

    string UrlMsg { get; set; }

    Queue<long> ChatFriendQueue { get; }

    IEnumerable<ChatNotify> CurChatList { get; }

    //IEnumerable<ChatNotify> GetFriendNotifyQueueById(long friendId);

    FriendInfoDto GetInfoDtoById(long id);

    Queue<long> NoReadQueue { get; }
}

public sealed partial class PrivateMsgDataMgr
{
    public sealed partial class PrivateMsgData:IPrivateMsgData
    {

        public void InitData()
        {
            //_friendMsgDic = new Dictionary<long, Queue<ChatNotify>>();
            //for(int i=0; i<MAX_FRIEND_COUNT;i++)
            //{
            //    _friendMsgDic.Add(-1-i, new Queue<ChatNotify>(MAX_CHATRECORD_COUNT));
            //}
        }

        public void Dispose()
        {

        }

        private Dictionary<long, Queue<ChatNotify>> _friendMsgDic = new Dictionary<long, Queue<ChatNotify>>();
        public Dictionary<long, Queue<ChatNotify>> FriendMsgDic
        {
            get
            {
                return _friendMsgDic;
            }
        }

        public const int MAX_CHATRECORD_COUNT = 100;
        private const int MinPageCount = 18;
        private const int MAX_FRIEND_COUNT = 20;

        //保存陌生人玩家信息
        private List<FriendInfoDto> _strangerInfoDtoList = new List<FriendInfoDto>();
        public IEnumerable<FriendInfoDto> StrangerInfoDtoList
        {
            get
            {
                return _strangerInfoDtoList;
            }
        }
        public FriendInfoDto GetInfoDtoById(long id)
        {
            if (FriendDataMgr.DataMgr.GetFriendDtoById(id) != null)
                return FriendDataMgr.DataMgr.GetFriendDtoById(id);
            else if(_strangerInfoDtoList.Find(x => x.friendId == id) != null)
                return _strangerInfoDtoList.Find(x => x.friendId == id);

            return null;
        }

        public void AddFriendInfoDto(FriendInfoDto dto)
        {
            if (_strangerInfoDtoList.Find(x => x.friendId == dto.friendId) == null
                && FriendDataMgr.DataMgr.GetFriendDtoById(dto.friendId) == null)
            {
                _strangerInfoDtoList.Add(dto);
            }
        }

        private Queue<long> _chatFriendQueue = new Queue<long>();
        public Queue<long> ChatFriendQueue
        {
            get
            {
                return _chatFriendQueue;
            }
        }

        public bool _lockState = false;
        public bool LockState
        {
            get { return _lockState; }
            set
            {
                _lockState = value;
            }
        }

        public int UnreadCnt
        { get; set; }

        public bool _isScrollToBot = false;
        public bool isScrollToBot
        {
            get { return _isScrollToBot; }
            set
            {
                _isScrollToBot = value;
            }
        }

        public string UrlMsg { get; set; }

        // 记录最新收到消息的频道，为了减少不必要频繁刷新
        private long latestMsgId;
        public long LatestMsgId
        {
            get { return latestMsgId; }
        }

        private long curMsgId;
        public long CurMsgId
        {
            get { return curMsgId; }
            set
            {
                if (!curMsgId.Equals(value))
                {
                    UnreadCnt = 0;
                    LockState = false;
                }
                latestMsgId = curMsgId;
                curMsgId = value;
            }
        }

        public bool isMoveUpContent { get; set; }
        public bool isSpeechMode { get; set; } //是否在语音模式下

        public Queue<long> _noReadQueue = new Queue<long>();
        public Queue<long> NoReadQueue
        {
            get
            {
                return _noReadQueue;
            }
        }

        public IEnumerable<ChatNotify> CurChatList
        {
            get
            {
                return GetFriendNotifyQueueById(curMsgId);
            }
        }

        public IEnumerable<ChatNotify> GetFriendNotifyQueueById(long friendId)
        {
            Queue<ChatNotify> queue = null;
            _friendMsgDic.TryGetValue(friendId, out queue);

            if(queue == null)
                queue = new Queue<ChatNotify>();

            return queue.ToList().GetElememtsByRange(-1, MinPageCount);
        }

        private long GetLongestNoTalkId()
        {
            long id = -1;
            var date = SystemTimeManager.Instance.GetUTCTimeStamp();
            _friendMsgDic.ForEach(item =>
            {
                long time = 0;
                while (item.Value.Count > 0)
                {
                    var queueItem = item.Value.Dequeue();
                    time = queueItem.fromTime;
                }

                if(time < date)
                {
                    date = time;
                    id = item.Key;
                }
            });

            return id;
        }

        //收到消息 保存私信收发者的信息 包括陌生人
        public void NotifyAddFriendInfo(ChatNotify notify)
        {
            //陌生人第一次发送信息给我 保存陌生人信息
            if (!FriendDataMgr.DataMgr.IsMyFriend(notify.fromPlayer.id))
            {
                FriendInfoDto dto = new FriendInfoDto();
                dto.friendId = notify.fromPlayer.id;
                dto.name = notify.fromPlayer.nickname;
                dto.grade = notify.fromPlayer.grade;
                dto.factionId = notify.fromPlayer.factionId;
                dto.countryId = (int)notify.fromPlayer.guildId;
                dto.degree = 0;
                dto.online = true;
                dto.charactor = notify.fromPlayer.charactor;
                dto.charactorId = notify.fromPlayer.charactorId;
                AddFriendInfoDto(dto);
            }
        }

        //收到消息notify
        public void AddChatNotify(ChatNotify notify)
        {
            if (notify == null) return;

            if (notify.fromPlayer.id == ModelManager.Player.GetPlayerId())
            {
                latestMsgId = notify.toPlayerId;
                //锁屏未读消息 自己发送 直接刷新聊天界面
                if (latestMsgId == curMsgId)
                {
                    LockState = false;
                    UnreadCnt = 0;
                    _isScrollToBot = true;
                }
            }
            else if(notify.toPlayerId == ModelManager.Player.GetPlayerId())
            {
                latestMsgId = notify.fromPlayer.id;
                AddNoReadMsgId(latestMsgId);
                NotifyAddFriendInfo(notify);
                //锁屏未读消息
                if (LockState && curMsgId == latestMsgId)
                {
                    UnreadCnt++;
                }
            }

            AddChatFriendQueueId(latestMsgId);

            if (!_chatFriendQueue.Contains(curMsgId))
                curMsgId = latestMsgId;

            if (!_friendMsgDic.ContainsKey(latestMsgId))
            {
                var queue = new Queue<ChatNotify>();
                //queue.Enqueue(notify);
                while(_friendMsgDic.Count >= MAX_FRIEND_COUNT)
                {
                    _friendMsgDic.Remove(GetLongestNoTalkId());
                }

                _friendMsgDic.Add(latestMsgId, queue);
            }

            if (ChatHelper.FillTranslationContent(notify, _friendMsgDic[latestMsgId]))
            {
            }
            else
            {
                AddNotify(notify);
                //chatBoxData.AddChatBoxQueue(notify);
            }
        }

        private void AddNotify(ChatNotify notify)
        {
            Queue<ChatNotify> queue = null;
            _friendMsgDic.TryGetValue(latestMsgId, out queue);

            if (queue != null)
            {
                if (queue.Count >= MAX_CHATRECORD_COUNT)
                {
                    queue.Dequeue();
                }
                queue.Enqueue(notify);
            }
        }

        //增加聊天好友id列表
        public void AddChatFriendQueueId(long id)
        {
            if (!_chatFriendQueue.Contains(id))
            {
                if(_chatFriendQueue.Count >= MAX_FRIEND_COUNT)
                {
                    _chatFriendQueue.Dequeue();
                }
                _chatFriendQueue.Enqueue(id);
            }
            else
            {
                Queue<long> tempQueue = new Queue<long>();

                while (_chatFriendQueue.Count > 0)
                {
                    var tempId = _chatFriendQueue.Dequeue();
                    if (tempId != id)
                        tempQueue.Enqueue(tempId);
                }
                tempQueue.Enqueue(id);
                _chatFriendQueue = tempQueue;
            }
        }
        //清除某好友聊天记录
        public void ClearRecordById(long friendId)
        {
            if (_friendMsgDic.ContainsKey(friendId))
                _friendMsgDic.Remove(friendId);
            //_data.FriendMsgDic[friendId] = new Queue<ChatNotify>();
            if (_chatFriendQueue.Contains(friendId))
            {
                Queue<long> tempQueue = new Queue<long>();

                while (_chatFriendQueue.Count > 0)
                {
                    var tempId = _chatFriendQueue.Dequeue();
                    if (tempId != friendId)
                        tempQueue.Enqueue(tempId);
                }

                _chatFriendQueue = tempQueue;
                curMsgId = tempQueue.ToList().Count > 0 ? tempQueue.ToList()[tempQueue.ToList().Count - 1] : 0;
            }
        }

        //保存未读取信息标记
        private void AddNoReadMsgId(long id)
        {
            if (!_noReadQueue.Contains(id))
                _noReadQueue.Enqueue(id);
            else
            {
                Queue<long> tempQueue = new Queue<long>();
                
                while(_noReadQueue.Count > 0)
                {
                    var tempId = _noReadQueue.Dequeue();
                    if (tempId != id)
                        tempQueue.Enqueue(tempId);
                }
                tempQueue.Enqueue(id);
                _noReadQueue = tempQueue;
            }
        }

        #region 登录后设置信息列表、当前选中聊天好友和未读信息
        public void ResetChatFriendQueue(string str)
        {
            GameDebuger.Log("~~~~~~~~DataHashCode" + this.GetHashCode());
            if (str == null)
                return;
            _chatFriendQueue.Clear();
            var strArr = str.Split(';');
            strArr.ForEach(idStr =>
            {
                if (!string.IsNullOrEmpty(idStr) && _chatFriendQueue.Count<MAX_FRIEND_COUNT)
                    _chatFriendQueue.Enqueue(long.Parse(idStr));
            });

            _chatFriendQueue.ToList().TryGetValue(_chatFriendQueue.Count - 1, out curMsgId);
        }
        public void ResetNoReadQueue(string str)
        {
            _noReadQueue.Clear();
            var strArr = str.Split(';');
            strArr.ForEach(idStr =>
            {
                if (!string.IsNullOrEmpty(idStr))
                    _noReadQueue.Enqueue(long.Parse(idStr));
            });
        }
        #endregion

        public void OnReadFriendMsg(long id)
        {
            if (!_noReadQueue.Contains(id))
                return;
            else
            {
                Queue<long> tempQueue = new Queue<long>();

                while (_noReadQueue.Count > 0)
                {
                    var tempId = _noReadQueue.Dequeue();
                    if (tempId != id)
                        tempQueue.Enqueue(tempId);
                }

                _noReadQueue = tempQueue;
            }
        }

        public void SetCurFriendId(long id)
        {
            curMsgId = id;
            Queue<ChatNotify> queue = null;
            _friendMsgDic.TryGetValue(id, out queue);

            if (queue == null)
            {
                queue = new Queue<ChatNotify>();
                _friendMsgDic.Add(curMsgId, queue);
            }
        }
    }
}
