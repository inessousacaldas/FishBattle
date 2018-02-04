// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : Zijian
// Created  : 10/12/2017 11:00:32 AM
// **********************************************************************

using AppDto;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UniRx;
using ChatChannelEnum = AppDto.ChatChannel.ChatChannelEnum;
using EmailNetMsg = EmailDataMgr.EmailNetMsg;
using System;
using UnityEngine;

public sealed partial class SocialityDataMgr
{
    
    public static partial class SocialityViewLogic
    {
        private static CompositeDisposable _disposable;
        public static void Open(ChatPageTab tab, ChatChannelEnum channel)
        {
            DataMgr._data.CurPageTab = tab;
            if (channel != ChatChannelEnum.Unknown)
                DataMgr._data.ChatData.CurChannelId = channel;
            // open的参数根据需求自己调整
            var layer = UILayerType.ChatModule;
            var ctrl = SocialityViewController.Show<SocialityViewController>(
                ChatInfoView.NAME
                , layer
                , true
                , true
                , Stream);
            InitReactiveEvents(ctrl);
        }
        private static void Dispose()
        {
            _disposable = _disposable.CloseOnceNull();
            ChatDataMgr.ChatPageViewLogic.Dispose();
            OnDispose();
        }

        // 如果有自定义的内容需要清理，在此实现
        private static void OnDispose()
        {

        }
        private static void InitReactiveEvents(ISocialityViewController ctrl)
        {
            if (ctrl == null) return;
            if (_disposable == null)
                _disposable = new CompositeDisposable();
            else
            {
                _disposable.Clear();
            }
            _disposable.Add(ctrl.CloseEvt.Subscribe(_ => Dispose()));

            _disposable.Add(ctrl.PageTabMgr.Stream.Subscribe(i =>
            {
                DataMgr._data.CurPageTab = (ChatPageTab)i;
                //邮件刷新机制 只有进入界面才会排序邮件
                EmailDataMgr.DataMgr.SetSortEmail();

                FireData();
            }));
            //隐藏按钮
            _disposable.Add(ctrl.OnHideBtn_UIButtonClick.Subscribe(_ => { ProxySociality.CloseChatInfoView(); }));

            ChatDataMgr.ChatPageViewLogic.InitReactiveEvents(ctrl);
            FriendDataMgr.FriendViewLogic.InitReactiveEvents(ctrl);
            PrivateMsgDataMgr.PrivateMsgViewLogic.InitReactiveEvents(ctrl);
            EmailDataMgr.EmailContentViewLogic.InitReactiveEvents(ctrl);

            _disposable.Add(ctrl.FriendCtrl.OnFriendChatStream.Subscribe(infoDto =>
            {
                ctrl.PageTabMgr.SetTabBtn((int)ChatPageTab.PrivateMsg);
                DataMgr._data.CurPageTab = ChatPageTab.PrivateMsg;
                PrivateMsgDataMgr.DataMgr.SetCurFriendDto(infoDto);

                FireData();
            }));
        }
    }
}

public sealed partial class ChatDataMgr
{
    public static partial class ChatPageViewLogic
    {
        private static CompositeDisposable _disposable;
        static ISocialityViewController _ctrl;
        public static string ChatHornInput = "ChatHornInput";
        public static void InitReactiveEvents(ISocialityViewController ctrl)
        {
            if (ctrl == null) return;
            if (_disposable == null)
                _disposable = new CompositeDisposable();
            else
            {
                _disposable.Clear();
            }

            _ctrl = ctrl;


            var chatData = DataMgr._data;
            //隐藏按钮
            _disposable.Add(ctrl.OnHideBtnClick.Subscribe(_ =>
            {
                chatData.LockState = true;
                ProxySociality.CloseChatInfoView();
            }));

            //_disposable.Add(ctrl.ChatViewCtrl.OnUnReadBtn_UIButtonClick.Subscribe(_ =>
            //{
            //    GameDebuger.Log("XASDFASDFSDAF");
            //    chatData.LockState = true;
            //    chatData.NewMsgCnt[chatData.CurChannelId] = 0;
            //    _ctrl.ChatViewCtrl.UpdateChatList(DataMgr._data.GetChannelNotifyQueue(DataMgr._data.CurChannelId));
            //    FireData();
            //}));

            _disposable.Add(ctrl.OnRedPacketRemind_UIButtonClick.Subscribe(_ => { }));
            //隐藏按钮
            _disposable.Add(ctrl.OnHideBtn_UIButtonClick.Subscribe(_ => { ProxySociality.CloseChatInfoView(); }));

            //=======================聊天部分===================
            UpdateChatList();
            InitVoiceEvent();
            //聊天频道Tab按钮
            _disposable.Add(ctrl.ChannelTabMgr.Stream.Subscribe(i =>
            {
                ctrl.ChatViewCtrl.InputFiledText = string.Empty;
                JSTimer.Instance.CancelTimer(ChatHornInput);
                chatData.CurChannelId = (ChatChannelEnum)ChatData.ShowChannelBtnNames[i].EnumValue;
                SetInputDeafaultText(chatData.chatViewData, chatData.CurChannelId);
                UpdateChatList();
                if (chatData.CurChannelId == ChatChannelEnum.Current || chatData.CurChannelId == ChatChannelEnum.Horn)
                {
                    chatData.chatViewData._speechMode = false;
                }
                FireData();
                ctrl.OnChannelTabChange(chatData);
            }));

            //发送按钮
            _disposable.Add(ctrl.ChatViewCtrl.OnSendBtn_UIButtonClick.Subscribe(_ =>
            {
                if(ChatHelper.CheckCanChatAndShowTip(chatData.CurChannelId))
                {
                    SendChatMessage(ctrl);
                }
            }));

            //语音/输入框切换按钮
            _disposable.Add(ctrl.ChatViewCtrl.OnSpeedOrInputBtn_UIButtonClick.Subscribe(_ =>
            {
                if (!ChatHelper.CheckCanChatAndShowTip(DataMgr._data.CurChannelId))
                    return;
                if (chatData.CurChannelId == ChatChannelEnum.Current || chatData.CurChannelId == ChatChannelEnum.Horn)
                {
                    TipManager.AddTip("当前频道无法发送语音");
                    chatData.chatViewData._speechMode = false;
                }
                else
                {
                    chatData.chatViewData._speechMode = !chatData.chatViewData._speechMode;
                }
                FireData();
            }));
            #region 喇叭
            _disposable.Add(ctrl.ChatViewCtrl.OnHornBtn_UIButtonClick.Subscribe(e =>
            {
                ctrl.ChatViewCtrl.UpdateHornPanel(DataMgr._data.ChatInfoViewData);
            }));
            _disposable.Add(ctrl.ChatViewCtrl.OnHornBtnClick.Subscribe(e => ctrl.ChatViewCtrl.OnHornTypeBtnClick(e)));
            _disposable.Add(ctrl.ChatViewCtrl.OnHornChooseBtn_UIButtonClick.Subscribe(e => 
            {
                ctrl.ChatViewCtrl.OnHornChooseBtnClick();
                chatData.CurHordId = ctrl.ChatViewCtrl.CurHornId;
                FireData();
            }));
            _disposable.Add(ctrl.ChatViewCtrl.OnHornCloseBtn_UIButtonClick.Subscribe(e => { ctrl.ChatViewCtrl.OnHornCloseBtnClick(); }));
            #endregion
            //输入框语音按钮
            ctrl.ChatViewCtrl.OnSpeechBtnPressEvt += OnSpeechBtnPress;
            //拖动结束
            _disposable.Add(ctrl.ChatViewCtrl.OnItemScrollStopMoving.Subscribe(isBottom =>
            {
                if (DataMgr._data.LockState != isBottom && isBottom)
                    UpdateChatList();
                DataMgr._data.LockState = isBottom;       
                FireData();
            }));
            //表情按钮
            _disposable.Add(ctrl.ChatViewCtrl.OnExpression_UIButtonClick.Subscribe(_ =>
            {
                
                chatData.isMoveUpFaceContent = !chatData.isMoveUpFaceContent;
                ctrl.ChatViewCtrl.ShowFaceView(true);
                if (chatData.isMoveUpFaceContent)
                {
                    //ctrl.SetTweenOffsetValue(ctrl.ChatViewCtrl.FaceCtrl.GetHight());
                    ctrl.TweenMoveUp();
                }         
                else
                    ctrl.TweenMoveDown();
                if(ctrl.ChatViewCtrl.FaceCtrl.EmojiItemList == null)
                {
                    ctrl.ChatViewCtrl.FaceCtrl.OnTabClickHandler(ExpressionType.Expression);
                }
               // FireData();
            }));

            _disposable.Add(ctrl.ChatViewCtrl.FaceCtrl.SelectStream.Subscribe(x => OnSelectByFacePanel(x, ctrl.ChatViewCtrl)));

            #region @好友功能

            _disposable.Add(ctrl.ChatViewCtrl.InputMsgHandle.Subscribe(e => AtFunction(e, ctrl))); //输入框
            _disposable.Add(ctrl.ChatViewCtrl.InputPress.Subscribe(e =>//点击输入框
            {
                if(!ctrl.ChatViewCtrl.FaceCtrl.IsShowFace)
                    ctrl.TweenMoveDown();
            }));
            _disposable.Add(TeamDataMgr.Stream.Subscribe(e => //监听队伍
            {
                ctrl.ChatViewCtrl.FaceCtrl.curChannel = ChatChannelEnum.Unknown;
                ctrl.ChatViewCtrl.FaceCtrl.UpdateTeamMember(e.GetTeamDto,chatData.CurChannelId);
            }));
            _disposable.Add(ctrl.ChatViewCtrl.FaceCtrl.AtItemClick.Subscribe(e => //下方面板人物选择
            {
                ctrl.TweenMoveDown();
                ctrl.ChatViewCtrl.InputFiledText += e.nickname + " ";
            }));
            _disposable.Add(ctrl.ChatViewCtrl.LongPress.Subscribe(e => //聊天界面人物长按
            {
                ctrl.ChatViewCtrl.InputFiledText += "@" + e + " ";
            }));
            _disposable.Add(ctrl.ChatViewCtrl.OnAtFuncBtn_UIButtonClick.Subscribe(e => //@提示点击
            {
                AtFuncBtnClick(ctrl.ChatViewCtrl, chatData.chatViewData.finalyAt);
                chatData.chatViewData.finalyAt = null;
            }));
            _disposable.Add(ctrl.ChatViewCtrl.OnUnReadBtn_UIButtonClick.Subscribe(e =>
            {
                UpdateChatList();
                FireData();
            }));
            _disposable.Add(ctrl.ChatViewCtrl.FaceCtrl.OnCloseBtn_UIButtonClick.Subscribe(e => { ctrl.TweenMoveDown(); }));//下方面板关闭点击
            _disposable.Add(ctrl.ChatViewCtrl.FaceCtrl.OnSearchBtn_UIButtonClick.Subscribe(e => { ctrl.ChatViewCtrl.FaceCtrl.OnSearchBtnClick(); }));//搜索按钮点击
            #endregion
            GameEventCenter.AddListener<ChatNotify>(GameEvent.CHAT_MSG_NOTIFY, OnChatMsgNotify);
        }
        private static void AtFuncBtnClick(IChatViewController ctrl,ChatNotify notify)
        {
            if (notify != null) 
                DataMgr._data.CurChannelId = (ChatChannelEnum)notify.channelId;
            UpdateChatList(false);
            FireData();
            ctrl.ScrollToPos(ctrl.GetAtScrollPos(notify));
            ctrl.HideAtFunc();
        }
        private static void AtFunction(string msg, ISocialityViewController ctrl)
        {
            var chatData = DataMgr._data;
            if (chatData.CurChannelId == ChatChannelEnum.Team)
            {
                if (msg.EndsWith("@"))
                {
                    DataMgr._data.isMoveUpFaceContent = false;
                    ctrl.ChatViewCtrl.FaceCtrl.UpdateTeamMember(chatData.GetTeamData(), chatData.CurChannelId);
                    ctrl.ChatViewCtrl.ShowFaceView(false);
                    ctrl.TweenMoveUp();
                }
            }
        }
        private static void OnSpeechBtnPress(GameObject go, bool state)
        {
            if (state)
            {
                SpeechFlagItemController.OpenSpeechFlag((int)ChatDataMgr.Stream.LastValue.ChatInfoViewData.CurChannelId);
            }
            else
            {
                SpeechFlagItemController.SendVoiceMessage();
            }
        }
        private static void OnChatMsgNotify(ChatNotify notify)
        {
            //如果是锁屏，同时等于当前频道，则更新
            if((DataMgr._data.LockState && notify.channelId == (int)DataMgr._data.CurChannelId) || notify.fromPlayer.id == ModelManager.Player.GetPlayerId())
            {
                UpdateChatList();
            }
        }
        private static void UpdateChatList(bool needDelay = true)
        {
            var chatData = DataMgr._data;
            chatData.LockState = true;
            chatData.NewMsgCnt[chatData.CurChannelId] = 0;
            _ctrl.ChatViewCtrl.UpdateChatList(DataMgr._data.GetChannelNotifyQueue(DataMgr._data.CurChannelId),needDelay);
        }
        private static void SendChatMessage(ISocialityViewController chatInfoCtrl)
        {
            var chatCtrl = chatInfoCtrl.ChatViewCtrl;
            var chatViewData = DataMgr._data.chatViewData;
            string inputStr = chatCtrl.InputFiledText;
            string _urlMsg = chatViewData._urlMsg;
            //DataMgr._data.isMoveUpFaceContent = false;
            //chatInfoCtrl.TweenMoveDown();
            AppStringHelper.FilterEmoji(inputStr);
            //string inputStr = _view.Input_UIInput.value;
            if (string.IsNullOrEmpty(inputStr) && string.IsNullOrEmpty(_urlMsg))
            {
                TipManager.AddTip("不能发送空消息");
                return;
            }
            if (inputStr.GetGBLength() > ChatDataMgr.ChatData.ChatMsgLen)
            {
                TipManager.AddTip("内容长度超出上限");
                return;
            }
            string pattern = ChatHelper.InputPattern;
            string finalMsg = Regex.Replace(inputStr, pattern, _urlMsg.WrapColor("1D8E00"));
            finalMsg = ChatHelper.ReplaceUrlToAddr(finalMsg);
            //喇叭使用其他接口
            if(DataMgr._data.CurChannelId == ChatChannelEnum.Horn)
            {
                if (!CheckChannelCanChat(DataMgr._data.CurChannelId, chatCtrl))
                    return;
                //CheckHornEnough(finalMsg, chatViewData, chatCtrl);
                SentHornMsg(DataMgr._data.CurHordId, finalMsg, chatViewData, chatCtrl);
            }
            else
            {
                ChatDataMgr.ChatNetMsg.ReqSendChatMsg(DataMgr._data.CurChannelId, 0, false, finalMsg, () =>
                {
                    //本次登陆的聊天记录
                    DataMgr._data.AddChatRecord(new ChatRecordVo(inputStr, _urlMsg));
                    //清空输入
                    chatViewData._urlMsg = string.Empty;
                    chatCtrl.InputFiledText = string.Empty;
                    FireData();
                });
            }
        }

        private static bool CheckChannelCanChat(ChatChannelEnum channel,IChatViewController ctrl)
        {
            string taskName = channel.ToString();
            switch (channel)
            {
                case ChatChannelEnum.Horn:
                    
                    if (JSTimer.Instance.IsCdExist(channel.ToString()))
                    {
                        var val = JSTimer.Instance.GetTimerTask(taskName);
                        if (val == null)
                            JSTimer.Instance.SetupTimer(ChatHornInput, delegate
                            {
                                int time = (int)JSTimer.Instance.GetRemainTime(taskName);
                                if (!ctrl.ViewIsNull)
                                {
                                    if (time != 0)
                                    {
                                        ctrl.InputFiledText = string.Format("请{0}秒后输入", time);
                                    }
                                    else
                                    {
                                        ctrl.InputFiledText = string.Empty;
                                        FireData();
                                    }
                                }
                            });
                        return false;
                    }
                    return true;
            }
            return true;
        }

        //检查喇叭道具是否足够
        private static void CheckHornEnough(string finalMsg,ChatViewData chatViewData,IChatViewController chatCtrl)
        {
            int curHorndId = DataMgr._data.CurHordId;
            int count = BackpackDataMgr.DataMgr.GetItemCountByItemID(curHorndId);
            if (count > 0)
            {
                SentHornMsg(curHorndId, finalMsg, chatViewData, chatCtrl);
            }
            else
            {
                var hornName = DataMgr._data.HornList.Find(e => e.id == curHorndId).name;
                var ctrl = ProxyBaseWinModule.Open();
                var title = "";
                var des = "当前" + hornName + "不足，扣除5元宝购买喇叭可以在喇叭频道发言";
                BaseTipData data = BaseTipData.Create(title, des, 0, delegate 
                {
                    var wealth = ModelManager.Player.GetPlayerWealth(AppVirtualItem.VirtualItemEnum.DIAMOND);
                    if (wealth >= 5)
                        SentHornMsg(curHorndId, finalMsg, chatViewData, chatCtrl);
                    else
                        TipManager.AddTip("元宝不足，购买失败");
                }, null);
                ctrl.InitView(data);
            }
        }

        private static void SentHornMsg(int curHornId, string finalMsg, ChatViewData chatViewData, IChatViewController chatCtrl)
        {
            ChatDataMgr.ChatNetMsg.ReqSendChatMsgWithProps((int)DataMgr._data.CurChannelId, curHornId, finalMsg,
                    () =>
                    {
                        //清空输入
                        chatViewData._urlMsg = string.Empty;
                        chatCtrl.InputFiledText = string.Empty;
                        DataMgr._data.SetUpChatTimer(ChatChannelEnum.Horn);
                        FireData();
                    });
        }
        
        public static void OnSelectByFacePanel(IFaceData faceData, IChatViewController chatCtrl)
        {
            var chatViewData = DataMgr._data.chatViewData;
            var str = faceData.Content;
            var msgType = faceData.Type;
            if (msgType == ChatMsgType.Emoji)
            {
                chatCtrl.InputFiledText += str;
            }
            else if (msgType == ChatMsgType.Record)
            {
                chatCtrl.InputFiledText = str;
                chatViewData._urlMsg = faceData.UrlMsg;
            }
            else
            {
                //string pattern = @"\[c]\[\w*]\[[\u4e00-\u9fa5]+-[\u4e00-\u9fa5]+\]\[-\]\[/c]"; //匹配 [XX-XXX] 带颜色
                string pattern = ChatHelper.InputPattern; //匹配 [XX-XXX]
                Match match = Regex.Match(chatCtrl.InputFiledText, pattern);
                if(match.Success)
                {
                   chatCtrl.InputFiledText =chatCtrl.InputFiledText.Replace(match.Value, str);
                }
                else
                {
                    chatCtrl.InputFiledText += str;
                }
                chatViewData._urlMsg = faceData.UrlMsg;
            }
            FireData();
        }
        /// <summary>
        /// 设置频道默认值
        /// </summary>
        /// <param name="data"></param>
        /// <param name="channelId"></param>
        private static void SetInputDeafaultText(ChatViewData data, ChatChannelEnum channelId)
        {
            switch (channelId)
            {
                case ChatChannelEnum.System:
                case ChatChannelEnum.Hearsay:
                    data.isEnable = false;
                    data._defalutInputText = "该频道为系统频道，不可以发言";
                    break;
                default:
                    data.isEnable = true;
                    data._defalutInputText = "请输入聊天文字";
                    break;
            }
        }

        #region 语音录制
        private static void InitVoiceEvent()
        {
            VoiceRecognitionManager.Instance.OnEndOfSpeechEvt += OnEndOfSpeechEvt;
            VoiceRecognitionManager.Instance.OnTranslateVoiceEndEvt += OnTranslateVoiceEndEvt;
            VoiceRecognitionManager.Instance.OnVoiceEndResultEvt += OnVoiceEndResultEvt;
        }


        private static void ClearVoiceEvent()
        {
            VoiceRecognitionManager.Instance.OnEndOfSpeechEvt -= OnEndOfSpeechEvt;
            VoiceRecognitionManager.Instance.OnTranslateVoiceEndEvt -= OnTranslateVoiceEndEvt;
            VoiceRecognitionManager.Instance.OnVoiceEndResultEvt -= OnVoiceEndResultEvt;
        }


        private static void OnVoiceEndResultEvt(VoiceEndResult endResult)
        {
            if (endResult.SaveState)
            {
                string sendContent = ChatHelper.GetVoiceUrl(endResult.VoiceKey, endResult.TranslateRes, endResult.RecordTime, endResult.fileUrl);
                sendContent = ChatHelper.ReplaceUrlToAddr(sendContent); //不明为何要转~~
                ChatNetMsg.ReqSendVoiceMsg((int)endResult.ChannelId, 0, sendContent, () =>
                {
                    TipManager.AddTopTip("发送语音消息成功");
                    //GameDebuger.Log("发送语音消息成功");
                });
                VoiceRecognitionManager.Instance.PlayVoice(endResult.VoiceKey, endResult.RecordTime);
            }
            else
            {
                TipManager.AddTopTip("语音发送失败");
            }

        }
        //语音转换文字结束现行展示
        private static void OnTranslateVoiceEndEvt(VoiceFinalResult result)
        {
            DataMgr.AddChatNotifySim(ChatHelper.GetNotify((int)result.ChannelId, ChatHelper.GetVoiceUrl(result.VoiceKey, result.Result, result.RecordTime)));
        }
        //客户端录音结束
        private static void OnEndOfSpeechEvt(long _currChannelId, string voiceKey, float _recordTime)
        {
            DataMgr.AddChatNotifySim(ChatHelper.GetNotify((int)_currChannelId,ChatHelper.GetVoiceUrl(voiceKey, "", _recordTime)));
        }
        #endregion
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="chatCtrl"></param>

        public static void Dispose()
        {
            _disposable = _disposable.CloseOnceNull();
            GameEventCenter.RemoveListener<ChatNotify>(GameEvent.CHAT_MSG_NOTIFY, OnChatMsgNotify);
            JSTimer.Instance.CancelTimer(ChatHornInput);
            //OnClose UI
            _ctrl.ChatViewCtrl.OnSpeechBtnPressEvt -= OnSpeechBtnPress;
            var chatData = DataMgr._data;
            chatData.isMoveUpFaceContent = false;
            ClearVoiceEvent();


            _ctrl = null;
        }

    }
}

public sealed partial class EmailDataMgr
{
    public sealed partial class EmailContentViewLogic
    {
        static CompositeDisposable _disposable;
        /// <summary>
        /// 注册邮箱的事件
        /// </summary>
        /// <param name="ctrl"></param>
        public static void InitReactiveEvents(ISocialityViewController ctrl)
        {
            if (ctrl == null) return;
            if (_disposable == null)
                _disposable = new CompositeDisposable();
            else
            {
                _disposable.Clear();
            }
            _disposable.Add(ctrl.EmailViewCtrl.OnOnKeyReceive_UIButtonClick.Subscribe(_ =>
            {
                var item = DataMgr._data.MailDtoList.Find(
                    s => !s.attachments.IsNullOrEmpty());
                if (item != null)
                {
                    //这里判断背包格子和附件数量的关系 待处理         TODO 
                    EmailNetMsg.ReqExtractAll(null, null);
                }
                else
                {
                    TipManager.AddTip("没有可领取的邮件".WrapColor(ColorConstantV3.Color_Blue));
                }
            }));

            _disposable.Add(ctrl.EmailViewCtrl.OnDelRead_UIButtonClick.Subscribe(_ =>
            {
                var item = DataMgr._data.MailDtoList.Find<PlayerMailDto>(
                    s => s.read && s.attachments.IsNullOrEmpty());
                if (item != null)
                {
                    EmailNetMsg.ReqDelAllRead(
                      () => TipManager.AddTip("已删除所有已读邮件".WrapColor(ColorConstantV3.Color_Blue))
                     , error => TipManager.AddTip("删除失败".WrapColor(ColorConstantV3.Color_Blue))
                       );
                }
                else
                {
                    TipManager.AddTip("没有找到已读邮件".WrapColor(ColorConstantV3.Color_Blue));
                }
            }));
        }
    }
}

//好友系统
public sealed partial class FriendDataMgr
{
    public static partial class FriendViewLogic
    {
        private static CompositeDisposable _disposable;
        public static void InitReactiveEvents(ISocialityViewController ctrl)
        {
            if (ctrl == null) return;
            if (_disposable == null)
                _disposable = new CompositeDisposable();
            else
            {
                _disposable.Clear();
            }

            var friendCtrl = ctrl.FriendCtrl;
            var friendData = DataMgr._data;

            _disposable.Add(friendCtrl.TabBtnMgr.Stream.Subscribe(i =>
            {
                SetCurTab((FriendViewTab)FriendData._TabInfos[i].EnumValue);
            }));

            //_disposable.Add(friendCtrl.OnFriendChatStream.Subscribe(id =>
            //{

            //}));
        }

        public static void SetCurTab(FriendViewTab i)
        {
            DataMgr._data.CurTab = i;
            FireData();
        }

        private static void Dispose()
        {
            _disposable = _disposable.CloseOnceNull();
        }
    }
}

//好友私信
public sealed partial class PrivateMsgDataMgr
{
    public static partial class PrivateMsgViewLogic
    {
        private static CompositeDisposable _disposable;
        public static void InitReactiveEvents(ISocialityViewController ctrl)
        {
            if (ctrl == null) return;
            if (_disposable == null)
                _disposable = new CompositeDisposable();
            else
            {
                _disposable.Clear();
            }

            var privateMsgData = DataMgr._data;
            var privateMsgCtrl = ctrl.PrivateMsgCtrl;

            _disposable.Add(privateMsgCtrl.OnExpression_UIButtonClick.Subscribe(_ =>
            {
                privateMsgData.isMoveUpContent = !privateMsgData.isMoveUpContent;
                ctrl.ChatViewCtrl.ShowFaceView(true);
                if (privateMsgData.isMoveUpContent)
                    ctrl.TweenMoveUp();
                else
                    ctrl.TweenMoveDown();

                if (ctrl.ChatViewCtrl.FaceCtrl.EmojiItemList == null)
                {
                    ctrl.ChatViewCtrl.FaceCtrl.OnTabClickHandler(ExpressionType.Expression);
                }

                FireData();
            }));

            _disposable.Add(privateMsgCtrl.OnSpeedOrInputBtn_UIButtonClick.Subscribe(_ =>
            {
                privateMsgData.isSpeechMode = !privateMsgData.isSpeechMode;
                FireData();
            }));

            _disposable.Add(privateMsgCtrl.OnSendBtn_UIButtonClick.Subscribe(_ =>
            {
                SendChatMessage(ctrl);
            }));

            _disposable.Add(privateMsgCtrl.OnUnReadBtn_UIButtonClick.Subscribe(_ =>
            {
                DataMgr._data.LockState = false;
                DataMgr._data.UnreadCnt = 0;
                DataMgr._data.isScrollToBot = true;

                FireData();
            }));

            //拖动结束
            _disposable.Add(privateMsgCtrl.OnItemScrollStopMoving.Subscribe(isBottom =>
            {
                DataMgr._data.LockState = !isBottom;
                if(isBottom)
                {
                    FireData();
                }
            }));

            _disposable.Add(privateMsgCtrl.OnFriendIdStream.Subscribe(infoDto =>
            {
                DataMgr._data.CurMsgId = infoDto.friendId;
                FireData();
            }));

            //_disposable.Add(ctrl.FriendCtrl.OnFriendChatStream.Subscribe(id =>
            //{
            //    DataMgr.SetCurFriendId(id);

            //    FireData();
            //}));

            _disposable.Add(ctrl.ChatViewCtrl.FaceCtrl.SelectStream.Subscribe(x => OnSelectByFacePanel(x, ctrl)));
        }

        private static void Dispose()
        {
            _disposable = _disposable.CloseOnceNull();
        }

        private static void OnSelectByFacePanel(IFaceData faceData, ISocialityViewController ctrl)
        {
            var str = faceData.Content;
            var msgType = faceData.Type;
            if (msgType == ChatMsgType.Emoji)
            {
                ctrl.PrivateMsgCtrl.InputFiledText += str;
            }
            else if (msgType == ChatMsgType.Record)
            {
                ctrl.PrivateMsgCtrl.InputFiledText = str;
                DataMgr._data.UrlMsg = faceData.UrlMsg;
            }
            else
            {
                //string pattern = @"\[c]\[\w*]\[[\u4e00-\u9fa5]+-[\u4e00-\u9fa5]+\]\[-\]\[/c]"; //匹配 [XX-XXX] 带颜色
                string pattern = ChatHelper.InputPattern; //匹配 [XX-XXX]
                Match match = Regex.Match(ctrl.PrivateMsgCtrl.InputFiledText, pattern);
                if (match.Success)
                {
                    ctrl.PrivateMsgCtrl.InputFiledText = ctrl.PrivateMsgCtrl.InputFiledText.Replace(match.Value, str);
                }
                else
                {
                    ctrl.PrivateMsgCtrl.InputFiledText += str;
                }
                DataMgr._data.UrlMsg = faceData.UrlMsg;
            }

            FireData();
        }

        private static void SendChatMessage(ISocialityViewController ctrl)
        {
            var friendCtrl = ctrl.FriendCtrl;
            var friendData = DataMgr._data;
            var privateMsgCtrl = ctrl.PrivateMsgCtrl;
            string inputStr = privateMsgCtrl.InputFiledText;
            string _urlMsg = friendData.UrlMsg;
            //DataMgr._data.isMoveUpContent = false;
            //ctrl.TweenMoveDown();
            AppStringHelper.FilterEmoji(inputStr);
            //string inputStr = _view.Input_UIInput.value;
            if(DataMgr._data.CurMsgId <= 0)
            {
                TipManager.AddTip("请选择一个好友玩家");
                return;
            }
            else if (string.IsNullOrEmpty(inputStr) && string.IsNullOrEmpty(_urlMsg))
            {
                TipManager.AddTip("不能发送空消息");
                return;
            }
            if (inputStr.GetGBLength() > ChatDataMgr.ChatData.ChatMsgLen)
            {
                TipManager.AddTip("内容长度超出上限");
                return;
            }

            string finalMsg = string.Format("{0}{1}",
                string.IsNullOrEmpty(inputStr) ? "" : inputStr.StripChatSymbols(),
                string.IsNullOrEmpty(_urlMsg) ? "" : _urlMsg.WrapColor("1D8E00"));


            PrivateMsgDataMgr.PrivateMsgNetMsg.ReqTalkFriend(DataMgr._data.CurMsgId, finalMsg, () =>
            {
                ChatNotify tempNotify = new ChatNotify();
                ShortPlayerDto tempDto = new ShortPlayerDto();
                tempDto.id = ModelManager.Player.GetPlayerId();
                tempNotify.channelId = (int)ChatChannelEnum.Friend;
                tempNotify.content = inputStr;
                tempNotify.fromPlayer = tempDto;
                tempNotify.fromTime = SystemTimeManager.Instance.GetUTCTimeStamp();
                tempNotify.toPlayerId = DataMgr._data.CurMsgId;
                tempNotify.itemId = 0;
                tempNotify.lableType = 0;
                tempNotify.actionType = 0;

                DataMgr._data.AddChatNotify(tempNotify);
                //DataMgr._data.AddChatRecord(tempNotify);

                //清空输入
                friendData.UrlMsg = string.Empty;
                privateMsgCtrl.InputFiledText = string.Empty;
                FireData();
            });
        }
    }
}