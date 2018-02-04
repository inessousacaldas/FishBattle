using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using AppDto;
using UnityEngine;
using ChatChannelEnum = AppDto.ChatChannel.ChatChannelEnum;
using LableTypeEnum = AppDto.ChatChannel.LableTypeEnum;
using LITJson;
using AppServices;

public static class ChatDataHelper
{
    public static bool IsSystemNotify(this ChatNotify chat)
    {
        return chat.fromPlayer == null;
    }
}

public static class ChatHelper
{
    public const string VoicePartten = @"\[url=([0-9-]*),([0-9-]*),([0-9a-zA-Z-]*),($|.*),(\d?\d+[\.]?\d+)\]($|.*)\["; 
    public const string UrlPattern = @"\[url=([^\]]+)\]"; //匹配获取 Url里面的信息
    public const string InputPattern = @"\[[\u4e00-\u9fa5]+-[\u4e00-\u9fa5a-zA-Z0-9]+\]"; //匹配输入框的文字 [XX-XXXX]


    public static string GetChannelName(ChatChannelEnum channelId)
    {
        switch (channelId)
        {
            case ChatChannelEnum.Hearsay: return "传闻";
            case ChatChannelEnum.System: return "系统";
            case ChatChannelEnum.Current: return "当前";
            case ChatChannelEnum.Team: return "队伍";
            case ChatChannelEnum.Guild: return "公会";
            case ChatChannelEnum.Country: return "阵营";
            case ChatChannelEnum.Friend: return "私聊";
            case ChatChannelEnum.Horn:   return "喇叭";
            default: return string.Empty;
        }
    }
    //这个方法用于特殊控制帮派频道玩家名字颜色
    public static string ChatInfoViewToChatBox1(string str)
    {
        return str.Replace("[0081AB]", "[FFF9E3]")
            .Replace("[A52D00]", "[FFF9E3]")
            .Replace("[A64E00]", "[F7E423]")
            .Replace("[1D8E00]", "[0FFF32]")
            .Replace("[8130A7]", "[C368E9]")
            .Replace("[0081AA]", "[FFF9E2]")
            .Replace("[C30000]", "[FD614C]");
    }

    public static string ParseChatBoxMsg(ChatNotify notify)
    {
        string finalMsg = "";
        if (notify.channelId == (int)ChatChannel.ChatChannelEnum.System)
        {
            ChatNotify systemNotify = notify as ChatNotify;
            if (systemNotify.lableType == (int)ChatChannel.LableTypeEnum.System
                || systemNotify.lableType == (int)ChatChannel.LableTypeEnum.Unknown)
            {
                finalMsg = string.Format("{0} {1}", "系统".WrapColor(ColorConstantV3.Color_Red_Str),
                    ColorHelper.UpdateColorInDeepStyle(notify.content));
            }
            else if (systemNotify.lableType == (int)ChatChannel.LableTypeEnum.Help)
            {
                finalMsg = string.Format("[E8E8E8]{0} {1}[-]", "帮助", ColorHelper.UpdateColorInDeepStyle(notify.content));
            }
            else if (systemNotify.lableType == (int)ChatChannel.LableTypeEnum.Prompt)
            {
                finalMsg = string.Format("[E8E8E8]{0}[-] {1}", "提示", ColorHelper.UpdateColorInDeepStyle(notify.content));
            }
            else if (systemNotify.lableType == (int)ChatChannel.ActionTypeEnum.Tip)
            {
                finalMsg = string.Format("{0} {1}", "系统".WrapColor(ColorConstantV3.Color_Red_Str), ColorHelper.UpdateColorInDeepStyle(notify.content));            }
            else if (systemNotify.lableType == (int)ChatChannel.ChatChannelEnum.Friend)
            {
                //好友上下线
                finalMsg = string.Format("[FB2929]{0}[-] {1}", "系统", ColorHelper.UpdateColorInDeepStyle(notify.content));
            }
            //if(systemNotify.actionType == (int)ChatChannel.ActionTypeEnum.Tip)
            //    TipManager.AddTip(finalMsg);
        }
//        else if (notify.channelId == (int)ChatChannel.ChatChannelEnum.CampBattle)
//        {
//            finalMsg = string.Format("{0} {1}", "阵营".WrapColor(ColorConstantV3.Color_Red_Str), ColorHelper.UpdateColorInDeepStyle(notify.content));
//        }
        else
        {
            bool isVoiceMsg = false;
            string playerName = "";
            if (notify is ChatNotify)
            {
                isVoiceMsg = IsVoiceMessage(notify.content);
                playerName = string.Format("[E8E8E8]【{0}】[-]", (notify as ChatNotify).fromPlayer.nickname);
            }
            else
            {
                playerName = " ";
            }
            if (notify.channelId == (int)ChatChannel.ChatChannelEnum.Country)
            {
                finalMsg = string.Format("{0}{1}{2}{3}",
                    "世界".WrapColor(ColorConstantV3.Color_Green_Str),
                    playerName,
                    isVoiceMsg ? "#leftV" : "",
                    ColorHelper.UpdateColorInDeepStyle(notify.content).WrapColor("0FFF32"));
            }
            else if (notify.channelId == (int)ChatChannel.ChatChannelEnum.Guild)
            {
                string tContent = notify.content;
                if (notify.content.Contains("签到") == true)
                {
                    tContent = tContent.Replace("[", "[\b");
                    tContent = tContent.Replace("#", "#\b");
                }

                finalMsg = string.Format("{0}{1}{2}{3}", "帮派".WrapColor("2DC6F8"),
                    playerName,
                    isVoiceMsg ? "#leftV" : "",
                    ChatInfoViewToChatBox1(tContent).WrapColor(ColorConstantV3.Color_Blue_Str));
            }
            else if (notify.channelId == (int)ChatChannel.ChatChannelEnum.Team)
            {
                finalMsg = string.Format("{0}{1}{2}{3}", "队伍".WrapColor("FF7633"),
                    playerName,
                    isVoiceMsg ? "#leftV" : "",
                    ColorHelper.UpdateColorInDeepStyle(notify.content).WrapColor("FF7633"));
            }
            else if (notify.channelId == (int)ChatChannel.ChatChannelEnum.Current)
            {
                finalMsg = string.Format("{0}{1}{2}{3}", "当前".WrapColor("E8E8E8"),
                    playerName,
                    isVoiceMsg ? "#leftV" : "",
                    ColorHelper.UpdateColorInDeepStyle(notify.content).WrapColor("E8E8E8"));
            }
        }
        return finalMsg;
    }

    //判断是否是语音消息
    public static bool IsVoiceMessage(string msg)
    {
        if (string.IsNullOrEmpty(msg))
            return false;
        return msg.Contains(string.Format("url={0}", (int)ChatMsgType.VoiceText));
    }

    //[url=msgType,playerId,itemId or uniqueId][name][/url]		普通超链接
    //[url=msgType,playerId,fileName,voiceUrl,recordTime][703F1B]{4}[-][/url]	语音
    public static void DecodeUrlMsg(string urlMsg, GameObject anchor = null, UILayerType layer = UILayerType.ThreeModule)
    {
        if (ModelManager.Player.IsPlayerBandMode())
            return;
        string[] param = urlMsg.Split(',');
        if (param.Length == 0)
            return;
        ChatMsgType msgType = (ChatMsgType)StringHelper.ToInt(param[0]);
        long playerId = 0L;
        switch (msgType)
        {
            case ChatMsgType.VoiceText:
                string fileName = param[2];
                float recordTime = float.Parse(param[4]);
                string fileUrl = param[3];
                VoiceRecognitionManager.Instance.PlayVoice(fileName, recordTime, fileUrl);
                break;
            case ChatMsgType.Item:
                var tipsPos = new Vector3(-143.0f, 159.0f, 0.0f);
                long.TryParse(param[1], out playerId);
                long itemUID;
                long.TryParse(param[2], out itemUID);
                if(itemUID > 0)
                {
                    //TODO:改成缓存，避免多次请求
                    GameUtil.GeneralReq(Services.Player_ShowPackItem(playerId, itemUID), resp =>
                    {
                        var dto = resp as BagItemDto;
                        ProxyTips.OpenTipsWithBagItemDto(dto, tipsPos);
                    });
                }
                else
                {
                    var item = DataCache.getDtoByCls<GeneralItem>((int)-itemUID);
                    var tipCtrl = ProxyTips.OpenGeneralItemTips(item);
                    tipCtrl.SetTipsPosition(tipsPos);
                    tipCtrl.ReSetAllPos(null);
                    tipCtrl.SetBtnView("", "");
                }
                break;
            case ChatMsgType.Crew:
                long.TryParse(param[1], out playerId);
                long crewUid;
                long.TryParse(param[2], out crewUid);
                GameUtil.GeneralReq<CrewInfoDto>(Services.Player_ShowCrew(playerId, crewUid), resp =>
                {
                    TipManager.AddTopTip("查看他们伙伴");
                });
                break;
            case ChatMsgType.ChatFriend:
                long.TryParse(param[2],out playerId);
                var _infoDto = PrivateMsgDataMgr.DataMgr.GetInfoDtoById(playerId);
                if (_infoDto != null)
                {
                    PrivateMsgDataMgr.DataMgr.SetCurFriendDto(_infoDto);
                    ProxySociality.OpenChatMainView(ChatPageTab.PrivateMsg);
                }
                break;
            default:
                TipManager.AddTip("没有相应的解析类型");
                break;
        }
    }
    /// <summary>
    /// 获取
    /// </summary>
    /// <param name="msgType"></param>
    /// <param name="name"></param>
    /// <param name="itemId"></param>
    /// <param name="content"></param>
    /// <returns></returns>
    public static string CombineToUrl(ChatMsgType msgType, string name, long itemId, out string content)
    {
        string res = string.Empty;
        content = string.Empty;
        switch (msgType)
        {
            case ChatMsgType.Item:
                content = string.Format("[道具-{0}]", name);
                res = string.Format("[url={0},{1},{2}]{3}[/url]", (int)msgType, ModelManager.Player.GetPlayerId(), itemId, content);
                break;
            case ChatMsgType.Crew:
                content = string.Format("[伙伴-{0}]", name);
                res = string.Format("[url={0},{1},{2}]{3}[/url]", (int)msgType, ModelManager.Player.GetPlayerId(), itemId, content);
                break;
            case ChatMsgType.ChatFriend:
                content = name;
                res = string.Format("[url={0},{1},{2}]{3}[/url]", (int)msgType, ModelManager.Player.GetPlayerId(), itemId, content);
                break;
        }

        return res;
    }
    /// <summary>
    /// 生成语音的Url格式
    /// </summary>
    /// <param name="fileName">文件名</param>
    /// <param name="translateContent">转换文字</param>
    /// <param name="recordTime">录音时长</param>
    /// <param name="fileUrl">文件的地址</param>
    /// <returns></returns>
    public static string GetVoiceUrl(string fileName, string translateContent, float recordTime, string fileUrl = "")
    {
        translateContent = CheckHasSeisitiveWord(translateContent);

        ChatMsgType msgType = ChatMsgType.VoiceText;
        return string.Format("[url={0},{1},{2},{3},{4}]{5}[/url]",
            (int)msgType,
            ModelManager.Player.GetPlayerId(),
            fileName,
            fileUrl,
            recordTime,
            translateContent);
    }
    //敏感词过滤
    public static string CheckHasSeisitiveWord(string str)
    {
        //var wordList = SensitiveWordFilter.Instance.ChatWordTrieFilter.FindAll(str);
        //if (wordList.Count > 0)
        //{
        //    string s = "";
        //    for (int i = 0; i < wordList.Count; i++)
        //    {
        //        int begin = wordList[i].IndexOf("#");
        //        s += wordList[i].Substring(begin == -1 ? 0 : begin + 1, wordList[i].Length) + ",";
        //    }
        //    GameDebuger.LogError("含有敏感字:" + s);
        //    string transWord = DataCache.GetStaticStringValue(AppStaticStrings.CHAT_SENSITIVE_WORD_REPLACER, "");
        //    str = string.IsNullOrEmpty(transWord)
        //        ? SensitiveWordFilter.Instance.ChatWordTrieFilter.Replace(str, '*')
        //        : transWord;
        //}
        return str;
    }
    public static IParseVoiceData ParseVoiceMsg(string msg)
    {
        string pattern = VoicePartten;
        Match match = Regex.Match(msg, pattern);
        if (match.Success)
        {
            ChatMsgType msgType = (ChatMsgType)StringHelper.ToInt(match.Groups[1].ToString());
            long playerId = StringHelper.ToInt(match.Groups[2].ToString());
      
            string fileName = match.Groups[3].ToString();
            string fileUrl = match.Groups[4].ToString();
            float recordTime = 0;
            float.TryParse(match.Groups[5].ToString(), out recordTime);
            string translateContent = match.Groups[6].ToString();
            return new ParseVoiceData(fileName, recordTime, fileUrl, translateContent, playerId);
        }
        else
            return null;

    }

    //找到相应的notify，加上翻译好的内容
    public static bool FillTranslationContent(ChatNotify notify, IEnumerable<ChatNotify> notifyQueue)
    {
        if (!IsVoiceMessage(notify.content))
            return false;
        var voiceData = ParseVoiceMsg(notify.content);
        string newFileName = voiceData.FileName;
        GameDebuger.Log("重复语音匹配判断,新语音id:" + newFileName);
        if (string.IsNullOrEmpty(newFileName))
            return false;

        var noti = notifyQueue.Find<ChatNotify>(item =>
        {
            string oldFileName = DecodeVoiceFileName(item.content);
            return !string.IsNullOrEmpty(oldFileName) && oldFileName == newFileName;
        });

        if (noti == null)
            return false;
        else
        {
            GameDebuger.Log("重复语音匹配判断,旧旧旧语音id:" + newFileName);
            if (notify.content.Length > noti.content.Length)
            {
                // 长的一定是含有转文字的
                noti.content = notify.content;
                GameDebuger.Log("长的一定是含有转文字的: " + noti.content);
            }
            return true;
        }

    }

    public static string DecodeVoiceFileName(string msg)
    {
        if (!IsVoiceMessage(msg))
            return "";
        string pattern = VoicePartten;
        Match match = Regex.Match(msg, pattern);
        if (match.Success)
        {
            return match.Groups[3].ToString();
        }
        return "";
    }

    public static float DecodeVoiceFileTime(string msg)
    {
        float time = 0f;
        if (!IsVoiceMessage(msg))
            return time;
        string pattern = VoicePartten;
        ;
        Match match = Regex.Match(msg, pattern);
        if (match.Success)
        {
            if (float.TryParse(match.Groups[5].ToString(), out time))
                return time;
        }
        return time;
    }
    #region addr url 互转
    //-------------------addr url 互转
    //发送到服务器使用addr, label上使用url
    public static string ReplaceAddrToUrl(string str)
    {
        return str.Replace("[addr=", "[url=")
            .Replace(@"[/addr]", @"[/url]");
    }

    public static string ReplaceUrlToAddr(string str)
    {
        return str.Replace("[url=", "[addr=")
            .Replace("[/url]", @"[/addr]");
    }
    #endregion
    /// <summary>
    /// 聊天显示文本转换颜色
    ///
    /// 活动/道具/链接绿0fff32 -> 1d8e00
    /// 场景黄 f7e423 -> a64e00
    /// 文本褐 fff9e3 -> 333251
    /// 名字蓝 2dc6f8 -> 0081ab
    ///
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static  string ChangeStrColor(string str)
    {
        return str.Replace(ColorConstantV3.Color_Green_Str, ColorConstantV3.Color_ChatGreen_Str)
            .Replace(ColorConstantV3.Color_Yellow_Str, ColorConstantV3.Color_Orange2_Str)
            .Replace(ColorConstantV3.Color_White_Str, ColorConstantV3.Color_SealBrown_Str)
            .Replace(ColorConstantV3.Color_Blue_Str,ColorConstantV3.Color_ChatBlue_Str)
            .Replace("[E7BD37]", "[D08D05]")
            .Replace("[C368E9]", "[8130A7]");
    }

    /// <summary>
    /// 检查是否该聊天频道的Cd
    /// </summary>
    /// <param name="channelId"></param>
    /// <param name="cdTask"></param>
    /// <returns></returns>
    public static bool CheckChatCd(int channelId,out JSTimer.CdTask cdTask)
    {
        bool res = true;
        cdTask = null;
        string TimerName = "CHATTIMER_CHANNEL" + channelId;
        if (JSTimer.Instance.IsCdExist(TimerName))
        {
            cdTask = JSTimer.Instance.GetCdTask(TimerName);
            res = false;
        }
        return res;
    }
    /// <summary>
    /// 检查该频道是否能聊天，并且弹出提示
    /// </summary>
    /// <param name="channelId"></param>
    /// <returns></returns>
    public static bool CheckCanChatAndShowTip(ChatChannelEnum channel)
    {
        var playerGrade = ModelManager.Player.GetPlayerLevel();
        switch(channel)
        {
            case ChatChannelEnum.Current:
                if(playerGrade < 15)
                {
                    TipManager.AddTopTip("当前发言需要人物等级15级");
                    return false;
                }
                break;
            case ChatChannelEnum.Horn:
                if (playerGrade < 15)
                {
                    TipManager.AddTopTip("喇叭发言需要人物等级15级");
                    return false;
                }
                break;
            case ChatChannelEnum.Country:
                TipManager.AddTopTip("你还没有阵营，不能在阵营频道发言");
                return false;
                break;
            case ChatChannelEnum.Guild:
                TipManager.AddTopTip("你还没有公会，不能在公会频道发言");
                return false;
                break;
            default:
                return true;
        }
        return true;
    }
    public static string GetLabelName(LableTypeEnum lableTypeEnum)
    {
        switch (lableTypeEnum)
        {
            case ChatChannel.LableTypeEnum.System:
                return "ChanelBG_systems";
            case ChatChannel.LableTypeEnum.Help:
                return "ChanelBG_systems";
            case ChatChannel.LableTypeEnum.Prompt:
                return "ChanelBG_systems";
            default :
                return string.Empty;
                            
        }
    }

    public static string GetChannelNameColor(ChatChannelEnum channelId, LableTypeEnum lableType)
    {
        if (channelId == ChatChannelEnum.System)
        {
            switch (lableType)
            {
                case LableTypeEnum.System:
                    return ColorConstantV3.Color_Orange_Str;
                case LableTypeEnum.Help:
                    return ColorConstantV3.Color_White_Str;
                case LableTypeEnum.Prompt:
                    return ColorConstantV3.Color_White_Str;
                default: return ColorConstantV3.Color_White_Str;
            }
        }
        else
        {
            switch (channelId)
            {
                case ChatChannelEnum.Guild:
                    return ColorConstantV3.Color_Blue_Str;                    
                case ChatChannelEnum.Team:
                    return ColorConstantV3.Color_Orange_Str;
                case ChatChannelEnum.Country:
                    return ColorConstantV3.Color_Green_Str;
                case ChatChannelEnum.Hearsay:
                    return ColorConstantV3.Color_White_Str;
                case ChatChannelEnum.Current:
                    return ColorConstantV3.Color_White_Str;
//                case ChatChannel.ChatChannelEnum_CampBattle:
//                    _view.ChannelNameLbl_UILabel.text = string.Format("{0}", "阵营").WrapColor(ColorConstantV3.Color_White_Str);
//                    content = string.Format("{0}", ChangeStrColor(notify.content).WrapColor(ColorConstantV3.Color_SealBrown_Str));
//                    break;
                default: return string.Empty;
            }
        }
    }


    public static ChatNotify GetNotify(int channelId, string content, int lableType = 0)
    {
        return new ChatNotify
        {
            channelId = channelId,
            content = content,
            lableType = lableType,
            fromPlayer = GetSelfShortPlayerDtoInfo()
        };
    }
    public static ShortPlayerDto GetSelfShortPlayerDtoInfo()
    {
        return new ShortPlayerDto
        {
            id = ModelManager.Player.GetPlayerId(),
            charactorId = ModelManager.Player.GetPlayer().charactorId,
            nickname = ModelManager.Player.GetPlayer().name,
            grade = ModelManager.Player.GetPlayerLevel(),
        };
    }
}

public interface IUrlChatData
{
    ChatMsgType type { get; }
}
public interface IParseVoiceData : IUrlChatData
{
    string FileName { get; }
    float RecordTime { get; }

    string VoiceUrl { get; }

    string TranslateContent { get; }

    long PlayerId { get; }
}
public class ParseVoiceData : IParseVoiceData
{
    public string FileName { get; set; }

    public float RecordTime { get; set; }

    public string VoiceUrl { get; set; }

    public string TranslateContent { get; set; }

    public long PlayerId { get; set; }

    public ChatMsgType type
    {
        get
        {
            return ChatMsgType.VoiceText;
        }
    }

    public ParseVoiceData(string fileName, float recordTime, string voiceUrl, string translateContent, long playerId)
    {
        FileName = fileName;
        RecordTime = recordTime;
        this.VoiceUrl = voiceUrl;
        this.TranslateContent = translateContent;
        this.PlayerId = playerId;
    }

    public override string ToString()
    {
        return string.Format("[url={0},{1},{2},{3},{4}]{5}[/url]",
            (int)ChatMsgType.VoiceText,
            PlayerId,
            FileName,
            VoiceUrl,
            RecordTime,
            TranslateContent);
    }
}

public class ParseItemData : IUrlChatData
{
    private ChatMsgType _type;
    public ChatMsgType type
    {
        get
        {
            return _type;
        }
    }

    public int PlayerId;
    public int ItemId;
    public ParseItemData(ChatMsgType type,int playerId,int ItemId)
    {

        this._type = type;
        this.PlayerId = playerId;
        this.ItemId = ItemId;
    }
}
