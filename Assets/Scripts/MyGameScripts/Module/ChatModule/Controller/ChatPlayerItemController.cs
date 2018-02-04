// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  ChatPlayerItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using System.Text.RegularExpressions;
using AppDto;
using UnityEngine;
using UniRx;
using ChatChannelEnum = AppDto.ChatChannel.ChatChannelEnum;

public partial class ChatPlayerItemController
{
    private const float OffSet = 30f;
    private const float HasVoiceOffset = 25f;
    private const float HasVoiceLblOffset = 24f;
    private const float IconOffSet = 10f;
    private const int ChatLblWidth = 290;
    private const int FriendLblWidth = 350;
    private const float ContentLbl_Y = -47.0f;
    private const float ChatIconBg_Y = -37f;
    private ChatNotify _chatNotify;
    private FriendInfoDto _infoDto = null;
    private const string leftBgSpriteName = "the-orange-talking-under_1";
    private const string rightBgSpriteNaem = "the-orange-talking-under";
    private const float MinVoiceContentLblWidth = 200;
    private const float MaxVoiceContentLblWidth = 310;
    private const float MinVoiceProgressWidth = 140;
    private const float MaxVoiceProgressWidth = 240;

    //	聊天数据来源
    private bool _formFriend = false;
    private bool _isInFriendView;

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        View.ContentLbl_UILabel.text = "";
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        EventDelegate.Set(View.ContentBg_UIButton.onClick, OnClickContentBtn); //点击内容
        EventDelegate.Set(View.IconBg_UIButton.onClick, OnClickFaceBtn); //点击头像
        View.IconBg__UIEventListener.onPress = OnPressItem; // 长按头像
        VoiceRecognitionManager.Instance.OnPlayVoice -= DoSpeechAnimation;
        VoiceRecognitionManager.Instance.OnStopVoice -= StopAnimation;
        VoiceRecognitionManager.Instance.OnPlayVoice += DoSpeechAnimation;
        VoiceRecognitionManager.Instance.OnStopVoice += StopAnimation;
    }

    protected override void OnDispose()
    {
        ClearData();
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    //用特定接口类型数据刷新界面
    public void UpdateView(ChatNotify data, int idx=-1)
    {
        _chatNotify = data;
        //设置基本信息
        if (_chatNotify.fromPlayer.id != ModelManager.Player.GetPlayerId())
        {
            _infoDto = PrivateMsgDataMgr.DataMgr.GetInfoDtoById(_chatNotify.fromPlayer.id);
            
            if (_infoDto != null)
            {
                if(_infoDto.charactor as MainCharactor != null)
                    UIHelper.SetPetIcon(View.Icon_UISprite, (_infoDto.charactor as MainCharactor).gender == 1 ? "101" : "103");
                View.PlayerNameAndJob_UILabel.text = _infoDto.name;
                View.LvLbl_UILabel.text = _infoDto.grade.ToString();
            }
            else
            {
                UIHelper.SetPetIcon(View.Icon_UISprite, _chatNotify.fromPlayer.gender == 1 ? "101" : "103");
                View.PlayerNameAndJob_UILabel.text = _chatNotify.fromPlayer.nickname;
                View.LvLbl_UILabel.text = _chatNotify.fromPlayer.grade.ToString();
            }
        }
        else
        {
            View.PlayerNameAndJob_UILabel.pivot = UIWidget.Pivot.Right;
            View.PlayerNameAndJob_UILabel.text = ModelManager.Player.GetPlayerName();
            UIHelper.SetPetIcon(View.Icon_UISprite, ModelManager.Player.GetPlayerGender() == 1 ? "101" : "103");
            View.LvLbl_UILabel.text = ModelManager.Player.GetPlayerLevel().ToString();
        }  

        SetContent(idx);

        //动画
        StopAnimation(string.Empty, false);

        DoSpeechAnimation(VoiceRecognitionManager.Instance.GetCurrPlayVoiceKey());
    }

    public void UpdateContent(ChatNotify notify)
    {
        _chatNotify = notify;
        SetContent();
    }

    private void SetVoiceActive(bool isVoice,bool isLeft = true)
    {
        var voiceInfo = ChatHelper.ParseVoiceMsg(_chatNotify.content);
        View.VoiceSprite_UISprite.gameObject.SetActive(isVoice);
        View.VoiceProgress_UISprite.gameObject.SetActive(isVoice);
        View.ContentBg_Voice_UISprite.gameObject.SetActive(isVoice);
        View.VoiceTranslateLabel_UILabel.gameObject.SetActive(isVoice);
        View.VoiceTime_UILabel.gameObject.SetActive(isVoice);

        if(isLeft)
        {
            View.ContentBg_Voice_UISprite.pivot = UIWidget.Pivot.TopLeft;
            View.VoiceProgress_UISprite.pivot = UIWidget.Pivot.Left;
            View.ContentBg_Voice_UISprite.transform.localPosition = new Vector3(View.IconBg_UISprite.width + 15, -37, 0);
        }
        else
        {
            View.ContentBg_Voice_UISprite.pivot = UIWidget.Pivot.TopRight;
            View.VoiceProgress_UISprite.pivot = UIWidget.Pivot.Right;
            View.ContentBg_Voice_UISprite.transform.localPosition = new Vector3(View.ChatPlayerItemPrefab1_UIWidget.width - View.IconBg_UISprite.width - 15, -37);
        }

        View.ContentBg_Voice_UISprite.spriteName = isLeft ? leftBgSpriteName : rightBgSpriteNaem;
        if (isVoice)
        {

            View.VoiceTime_UILabel.text = string.Format("{0}秒", (int)voiceInfo.RecordTime);
            View.VoiceTranslateLabel_UILabel.text = voiceInfo.TranslateContent;
            if(View.VoiceTranslateLabel_UILabel.printedSize.x > 280)
            {
                View.VoiceTranslateLabel_UILabel.overflowMethod = UILabel.Overflow.ResizeHeight;
                View.VoiceTranslateLabel_UILabel.width = (int)280;
                View.ContentBg_Voice_UISprite.width = (int)MaxVoiceContentLblWidth;
            }
            else if(View.VoiceTranslateLabel_UILabel.printedSize.x < 180)
            {
                View.VoiceTranslateLabel_UILabel.overflowMethod = UILabel.Overflow.ResizeHeight;
                View.VoiceTranslateLabel_UILabel.width = (int)180;
                View.ContentBg_Voice_UISprite.width = (int)MinVoiceContentLblWidth;
            }
            else
            {
                View.ContentBg_Voice_UISprite.width = View.VoiceTranslateLabel_UILabel.width + 50;
            }
            View.ContentBg_Voice_UISprite.height = (int)(62 + View.VoiceTranslateLabel_UILabel.printedSize.y);
        }
    }

    private void SetContent(int idx=-1)
    {
        //	显示时间 (好友私聊)
        var lastChatTime = PrivateMsgDataMgr.DataMgr.GetCurChatLastTime(idx);
        View.TirmeLbl_UILabel.enabled = lastChatTime > 0 && idx > 0;//第一个记录不显示时间
        if (View.TirmeLbl_UILabel.enabled)
        {
            var lastChatDateTime = DateUtil.UnixTimeStampToDateTime(lastChatTime);
            var nowDateTime = SystemTimeManager.Instance.GetServerTime();
            //时间加\n 用于增加与上条聊天记录的距离
            if(nowDateTime.Day - lastChatDateTime.Day >= 2)
                View.TirmeLbl_UILabel.text = string.Format("\n{0}-{1} {2:D2}:{3:D2}", lastChatDateTime.Month, lastChatDateTime.Day, lastChatDateTime.Hour, lastChatDateTime.Minute);
            else if (nowDateTime.Day - lastChatDateTime.Day >= 1)
                View.TirmeLbl_UILabel.text = string.Format("\n昨天 {0:D2}:{1:D2}", lastChatDateTime.Hour, lastChatDateTime.Minute);
            else if (nowDateTime.Day == lastChatDateTime.Day)
                View.TirmeLbl_UILabel.text = string.Format("\n{0:D2}:{1:D2}", lastChatDateTime.Hour, lastChatDateTime.Minute);
        }

        string tContent = _chatNotify.content;
        if (_chatNotify.channelId == (int)ChatChannel.ChatChannelEnum.Guild && tContent.Contains("签到"))
        {
            //帮派签到信息特殊处理
            tContent = tContent.Replace("#", "#\b");
            tContent = tContent.Replace("[", "[\b");
            tContent = tContent.Replace("【签到】", "【签到】".WrapColor(ColorConstantV3.Color_Blue_Str));
        }

        View.ContentLbl_UILabel.overflowMethod = UILabel.Overflow.ResizeFreely;
        string content = ColorHelper.UpdateColorInLightStyle(tContent).WrapColor(ColorConstantV3.Color_White);
        bool hasEmoji = View.ContentLbl_UILabel.SetEmojiText(content);
        View.ContentLbl_UILabel.spacingX = 1;
        View.ContentBg_UISprite.topAnchor.absolute = 10;
        View.ContentBg_UISprite.bottomAnchor.absolute = hasEmoji ? 10 : -2;
        View.ContentLbl_UILabel.spacingY = hasEmoji ? 25 : 5;

        bool left = _chatNotify.fromPlayer.id != ModelManager.Player.GetPlayerId();
        bool isVoice = ChatHelper.IsVoiceMessage(_chatNotify.content);
        float lblWidth = isVoice ? ChatLblWidth - HasVoiceLblOffset : ChatLblWidth;
        //lblWidth = left ? lblWidth - 10 : lblWidth ;

        if (_isInFriendView)
        {
            lblWidth = isVoice ? FriendLblWidth - HasVoiceLblOffset : FriendLblWidth;
        }
        if (View.ContentLbl_UILabel.printedSize.x > lblWidth)
        {
            View.ContentLbl_UILabel.overflowMethod = UILabel.Overflow.ResizeHeight;
            View.ContentLbl_UILabel.width = (int)lblWidth;
        }
       
        View.ContentBg_UISprite.spriteName = left ? leftBgSpriteName : rightBgSpriteNaem;
        if (left)
        {
            View.IconBg.transform.localPosition = new Vector3(View.IconBg_UISprite.width * 0.5f + IconOffSet, ChatIconBg_Y, 0f);
            View.ContentLbl_UILabel.pivot = UIWidget.Pivot.TopLeft;
            View.ContentBg_UISprite.pivot = UIWidget.Pivot.TopLeft;
            //View.ContentBg_UISprite.flip = UIBasicSprite.Flip.Horizontally;
            View.ContentBg_UISprite.rightAnchor.absolute = 10;
            
            View.PlayerNameAndJob_UILabel.pivot = UIWidget.Pivot.Left;
            View.PlayerNameAndJob_UILabel.transform.localPosition = new Vector3(119.4f, -19);
            View.littleIconTable_UITable.pivot = UIWidget.Pivot.Left;
            View.littleIconTable_UITable.transform.localPosition = new Vector3(80.5f, -17.1f);
            View.littleIconTable_UITable.Reposition();

            if (isVoice)
            {
                View.ContentLbl_UILabel.gameObject.SetActive(false);
                SetVoiceActive(true, true);
            }
            else
            {
                View.ContentLbl_UILabel.transform.localPosition = new Vector3(95.5f, -42.1f, 0f);
                View.ContentBg_UISprite.leftAnchor.absolute = -15;
                View.ContentLbl_UILabel.gameObject.SetActive(true);
                SetVoiceActive(false);
            }
        }
        else
        {
            View.IconBg.transform.localPosition = new Vector3(365f, ChatIconBg_Y, 0f);
            View.ContentLbl_UILabel.pivot = UIWidget.Pivot.TopRight;
            View.ContentBg_UISprite.pivot = UIWidget.Pivot.TopRight;
            View.ContentBg_UISprite.leftAnchor.absolute = -10;
            View.PlayerNameAndJob_UILabel.pivot = UIWidget.Pivot.Right;
            View.PlayerNameAndJob_UILabel.transform.localPosition = new Vector3(286f, -17);
            View.littleIconTable_UITable.pivot = UIWidget.Pivot.Right;
            View.littleIconTable_UITable.transform.localPosition = new Vector3(327f, -17.1f);
            View.littleIconTable_UITable.Reposition();

            if (isVoice)
            {
                View.ContentLbl_UILabel.gameObject.SetActive(false);
                SetVoiceActive(true, false);
            }
            else
            {
                View.ContentLbl_UILabel.gameObject.SetActive(true);
                View.ContentLbl_UILabel.transform.localPosition = new Vector3(313.1f, -42.7f, 0);
                View.ContentBg_UISprite.rightAnchor.absolute = 15;
                SetVoiceActive(false);
            }
        }
        View.ContentBg_UIButton.GetComponent<UIWidget>().ResizeCollider();
        View.ContentBg_UISprite.UpdateAnchors();
    }

    private void OnClickFaceBtn()
    {
        //        if (ModelManager.CSPK.IsOptionForbiddenForCSPKSpecial())
        //            return;
        if (_chatNotify == null)
        {
            return;
        }
        if (_chatNotify.fromPlayer.id != ModelManager.Player.GetPlayerId())
        {
            if (_chatNotify.fromPlayer.id == 0) return;//点击的是NPC
            var ctrl = FriendDetailViewController.Show<FriendDetailViewController>(FriendDetailView.NAME, UILayerType.ThreeModule, false, true);
            ctrl.UpdateView(_chatNotify.fromPlayer);
//            UIPanel panel = UIPanel.Find(transform);
//            if (panel != null)
//            {
////                UILayerType layerType = LayerManager.Instance.GetLayerTypeByDepth(panel.depth);
////                ProxyMainUI.OpenPlayerInfoViewDynamic(_chatNotify.fromPlayer.id, layerType);
//            }
//            else
//            {
//                //ProxyMainUI.OpenPlayerInfoView(_chatNotify.fromPlayer.id, new Vector3(-35, -32, 0));
//            }
        }
    }

    private void OnClickContentBtn()
    {
        //if (ModelManager.Chat.IsOnlyVoiceTextMessage(_chatNotify.content))
        //{
        //    return;
        //}

        //string clickMsg = View.ContentLbl_UILabel.GetUrlAtPosition(UICamera.lastWorldPosition);
        //GameDebuger.Log("ChatPlayerItem OnClcikContentBtn : clickMsg" + clickMsg);
        //if (string.IsNullOrEmpty(clickMsg) == false)
        //{
        //    UIPanel panel = UIPanel.Find(transform);
        //    //ILayerType layerType = LayerManager.Instance.GetLayerTypeByDepth(panel.depth);
        //    //ModelManager.Chat.DecodeUrlMsg(clickMsg, UICamera.current.gameObject, layerType);
        //    ChatHelper.DecodeUrlMsg(clickMsg);
        //    return;
        //}


        string pattern = ChatHelper.UrlPattern;
        Match match = Regex.Match(_chatNotify.content, pattern);
        if (match.Success)
        {
            UIPanel panel = UIPanel.Find(transform);
            ChatHelper.DecodeUrlMsg(match.Groups[1].ToString(), UICamera.current.gameObject);
        }
    }

    private void DoSpeechAnimation(string fileName)
    {
        StopAnimation(string.Empty, false);

        if (View == null || _chatNotify == null)
        {
            return;
        }
        if(ChatHelper.IsVoiceMessage(_chatNotify.content))
        {
            string pattern = @"\[url=([^\]]+)\]";
            Match match = Regex.Match(_chatNotify.content, pattern);
            if (match.Success)
            {
                string[] param = match.Groups[1].ToString().Split(',');
                if (param.Length > 2 && param[2] == fileName)
                {
                    //做动画
                    JSTimer.Instance.AddCdUpdateHandler(VoiceRecognitionManager.PlayVoiceStop, (e) =>
                    {
                        if (View == null)
                            return;

                        View.VoiceSprite_UISprite.type = UIBasicSprite.Type.Filled;
                        View.VoiceSprite_UISprite.invert = true;
                        View.VoiceSprite_UISprite.fillDirection = UIBasicSprite.FillDirection.Horizontal;
                        View.VoiceSprite_UISprite.fillAmount += 0.15f;
                        if (View.VoiceSprite_UISprite.fillAmount >= 1.0f)
                        {
                            View.VoiceSprite_UISprite.fillAmount = 0;
                        }
                    });
                }
            }
        }
//        else
        {
            StopAnimation(string.Empty, false);
        }
    }

    #region @好友

    private Subject<string> longPress = new Subject<string>();
    public UniRx.IObservable<string> LongPress { get { return longPress; } }
    private bool hasPressed = false;
    private void OnPressItem(GameObject go, bool press)
    {
        if (_chatNotify.channelId != (int)ChatChannelEnum.Team) return;
        if (press && !hasPressed)
        {
            if (_chatNotify.fromPlayer.id != ModelManager.Player.GetPlayerId())
            {
                JSTimer.Instance.SetupCoolDown("ChatFacePressTimer", 1.5f, null, () =>
                {
                    longPress.OnNext(_chatNotify.fromPlayer.nickname);
                    hasPressed = true;
                });
            }
        }
        else
        {
            hasPressed = false;
            JSTimer.Instance.CancelCd("ChatFacePressTimer");
        }
    }

    public ChatNotify ChatNotify
    {
        get { return _chatNotify; }
    }
    #endregion

    private void StopAnimation(string fileName, bool needContinue)
    {
        if (View != null)
        {
            View.VoiceSprite_UISprite.fillAmount = 1f;
            View.VoiceSprite_UISprite.type = UIBasicSprite.Type.Simple;
        }
    }

    public void ClearData()
    {
        VoiceRecognitionManager.Instance.OnPlayVoice -= DoSpeechAnimation;
        VoiceRecognitionManager.Instance.OnStopVoice -= StopAnimation;

        _chatNotify = null;
    }

}
