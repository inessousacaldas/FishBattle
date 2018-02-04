// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  ChatInfoViewController.cs
// Author   : Zijian
// Created  : 10/12/2017 11:03:22 AM
// Porpuse  : 
// **********************************************************************

using System;
using UniRx;
using UnityEngine;
using AppDto;
public partial interface ISocialityViewController
{
    TabbtnManager PageTabMgr { get; }
    TabbtnManager ChannelTabMgr { get; }
    IChatViewController ChatViewCtrl { get; }
    EmailContentController EmailViewCtrl { get; }
    FriendViewController FriendCtrl { get; }
    PrivateMsgViewController PrivateMsgCtrl { get; }

    void TweenMoveUp();
    void TweenMoveDown();

    void SetTweenOffsetValue(float value);

    void OnChannelTabChange(IChatInfoViewData data);
}
public partial class SocialityViewController    {

    //聊天部分~
    private ChatViewController _chatViewCtrl;
    private EmailContentController _emailViewCtrl;
    private FriendViewController _friendCtrl;
    private PrivateMsgViewController _privateMsgCtrl;
    public IChatViewController ChatViewCtrl { get { return _chatViewCtrl; } }
    public EmailContentController EmailViewCtrl { get { return _emailViewCtrl; } }
    public FriendViewController FriendCtrl { get { return _friendCtrl; } }
    public PrivateMsgViewController PrivateMsgCtrl { get { return _privateMsgCtrl; } }

    private TabbtnManager _channelTabMgr;
    public TabbtnManager ChannelTabMgr { get { return _channelTabMgr; } }

    private TabbtnManager _pageTabMgr;
    public TabbtnManager PageTabMgr { get { return _pageTabMgr; } }


    bool isInitChatPage;
    bool isInitPageTab;
    private ChatPageTab _lastTab;

    new CompositeDisposable _disposable;

    private TweenPosition Content_TweenPosition;
    public float tweenOffset
    {
        set
        {
            if (Content_TweenPosition == null)
            {
                Content_TweenPosition = UITweener.Begin<TweenPosition>(View.Content, 0.25f);
                Content_TweenPosition.from = new Vector3(-568f, 1f, 0f);
            }
            Content_TweenPosition.to = new Vector3(-568f, 201f, 0f);
            EventDelegate.Set(Content_TweenPosition.onFinished, SetFaceAnchorVisible);
            Content_TweenPosition.enabled = false;
        }
    }

    private void SetFaceAnchorVisible()
    {
        if(View.Content.transform.localPosition.y <= 2)
            View.FaceAnchor.SetActive(false);
    }
    // 界面初始化完成之后的一些后续初始化工作
    private ITabBtnController AddChannelTabBtn(int i)
    {
        return AddTabBtn(i, _view.ChannelBtnGroup_UIGrid.gameObject, TabbtnPrefabPath.TabBtnWidget_ChatTab, "Tabbtn_");
    }

    private ITabBtnController AddTabBtn(int i, GameObject parent, TabbtnPrefabPath tabPath, string name)
    {
        // todo fish: tobe check 验证一下这个bug，应该不存在此问题
        //http://oa.cilugame.com/redmine/issues/12110
        var ctrl = AddChild<TabBtnWidgetController, TabBtnWidget>(
            parent
            , tabPath.ToString()
            , name + i);

        ctrl.SetBtnImages("betton_b","betton_a");
        ctrl.SetBtnLblFont(selectColor: "65480b", normalColor: "636262");
        return ctrl;
    }
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        _disposable = new CompositeDisposable();
        
        InitPageTab();
        InitChatPage();
        InitEmailPage();
        InitFriendPage();
        InitPrivateMsgPage();
        tweenOffset = 200;
        _view.ApplyGuideBtn_UILabel.text = "[333251]加入工会可以让你认识更多新朋友，还能参与抵御[a64e00]强盗入侵[-]、参加[a64e00]工会任务[-]和[a64e00]攻城掠地[-]等精彩的工会活动！此外还有[1d8e00]每周福利[-]、[1d8e00]工会宝箱[-]、[1d8e00]工会工资[-]、[1d8e00]工会商店[-]等丰富的福利等着你！[-]";
    }

	// 客户端自定义代码
	protected override void RegistCustomEvent ()
    {
        
    }

    protected override void RemoveCustomEvent ()
    {
        if(Content_TweenPosition != null)
            EventDelegate.Remove(Content_TweenPosition.onFinished, SetFaceAnchorVisible);
    }
        
    protected override void OnDispose()
    {
        base.OnDispose();
        _disposable = _disposable.CloseOnceNull();
    }

	//在打开界面之前，初始化数据
	protected override void InitData()
    {
            
    }

    // 业务逻辑数据刷新
    protected override void UpdateDataAndView(ISocialityData data){

        var idx = ChatDataMgr.ChatData.pageNames.FindElementIdx(x => x.EnumValue == (int)data.CurPageTab);
        _pageTabMgr.SetTabBtn(idx);

        //显示邮箱未读邮件数量
        View.EmailRedPot_UISprite.gameObject.SetActive(EmailDataMgr.DataMgr.GetNoReadCount() > 0);
        View.EmailLabel_UILabel.text = EmailDataMgr.DataMgr.GetNoReadCount().ToString();

        switch(data.CurPageTab)
        {
            case ChatPageTab.Chat:
                View.ChatInputGroup.SetActive(true);
                if(_lastTab != data.CurPageTab)
                    View.Input_UIInput.value = "";
                View.ChatChannelPanel.SetActive(true);
                View.FriendAnchor.SetActive(false);
                View.EmailAnchor.SetActive(false);
                View.PrivateMsgAnchor.SetActive(false);
                View.ChannelBtnGroup_UIGrid.gameObject.SetActive(true);
                OnChatPageTab(data.ChatData);
                break;
            case ChatPageTab.Email:
                View.ChatInputGroup.SetActive(false);
                View.ChatChannelPanel.SetActive(false);
                View.FriendAnchor.SetActive(false);
                View.EmailAnchor.SetActive(true);
                View.PrivateMsgAnchor.SetActive(false);
                View.ChannelBtnGroup_UIGrid.gameObject.SetActive(false);
                break;
            case ChatPageTab.Friend:
                View.ChatInputGroup.SetActive(false);
                View.ChatChannelPanel.SetActive(false);
                View.FriendAnchor.SetActive(true);
                View.EmailAnchor.SetActive(false);
                View.PrivateMsgAnchor.SetActive(false);
                View.ChannelBtnGroup_UIGrid.gameObject.SetActive(false);
                _friendCtrl.UpdateView(data.FriendData);
                break;
            case ChatPageTab.PrivateMsg:
                View.ChatInputGroup.SetActive(false);
                View.ChatChannelPanel.SetActive(false);
                View.FriendAnchor.SetActive(false);
                View.EmailAnchor.SetActive(false);
                View.PrivateMsgAnchor.SetActive(true);
                View.ChannelBtnGroup_UIGrid.gameObject.SetActive(false);
                if(_chatViewCtrl != null)
                    ChatViewCtrl.SetFacePanelVisible(false);
                UpdatePrivateMsg(data, data.PrivateMsgData);
                break;
        }

        _lastTab = data.CurPageTab;
    }
    private void OnChatPageTab(IChatInfoViewData data)
    {
        if (!isInitChatPage)
        {
            isInitChatPage = true;
        }
        UpdateChannelList(data);

        ChatViewCtrl.SetFacePanelVisible(false);

        //if (data.isMoveUpFaceContent)
        //    TweenMoveUp();
        //else
        //    TweenMoveDown();
    }
    public void OnChannelTabChange(IChatInfoViewData data)
    {
        //_chatViewCtrl.SetRightPanelHeight(data.chatViewData.isEnable);
    }

    //更新聊天窗口~
    private void UpdateChannelList(IChatInfoViewData data)
    {
        //更新聊天标签页
        var idx = ChatDataMgr.ChatData.ShowChannelBtnNames.FindElementIdx(s => s.EnumValue == (int)data.CurChannelId);
        _channelTabMgr.UpdateTabs(ChatDataMgr.ChatData.ShowChannelBtnNames, AddChannelTabBtn, idx);
        View.ApplyGuide.SetActive(data.CurChannelId == ChatChannel.ChatChannelEnum.Guild);
        //更新聊天面板
        _chatViewCtrl.UpdateView(data);
    }

    private void InitPageTab()
    {
        if (isInitPageTab)
            return;
        isInitPageTab = true;

        if (!FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum.FUN_7))
            _pageTabMgr.SetBtnHide((int)ChatPageTab.Email);
        if (!FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum.FUN_8))
            _pageTabMgr.SetBtnHide((int)ChatPageTab.PrivateMsg);
        if (!FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum.FUN_8))
            _pageTabMgr.SetBtnHide((int)ChatPageTab.Friend);
        _pageTabMgr = TabbtnManager.Create(
            ChatDataMgr.ChatData.pageNames
            , i => AddTabBtn(i, _view.TabGroup, TabbtnPrefabPath.TabBtnWidget_ChatTab, "Page_")
            , 0);
    }
    private void InitChatPage()
    {
        //var idx = data.ShowChannelBtnNames.FindElementIdx(s => s.EnumValue == (int)data.CurChannelId);
        _channelTabMgr = TabbtnManager.Create(
            ChatDataMgr.ChatData.ShowChannelBtnNames
            , AddChannelTabBtn
            , 0
        );
        View.ChannelBtnGroup_UIGrid.Reposition();
        _chatViewCtrl = AddController<ChatViewController, ChatView>(_view.ChatView);
        //tweenOffset = ChatViewCtrl.FaceCtrl.GetHight();
    }
    private void InitEmailPage()
    {
        //初始化邮箱
        _emailViewCtrl = AddChild<EmailContentController, EmailContent>(
            _view.EmailAnchor
            , EmailContent.NAME);

        //独立监听消息
        _disposable.Add(SocialityDataMgr.Stream.Subscribe(data => _emailViewCtrl.UpdateView(data.EmailData)));
    }

    private void InitFriendPage()
    {
        //初始化好友
        _friendCtrl = AddChild<FriendViewController, FriendView>(
            View.FriendAnchor
            , FriendView.NAME);

        _disposable.Add(FriendDataMgr.Stream.Subscribe(data => _friendCtrl.UpdateView(data)));
    }

    private void InitPrivateMsgPage()
    {
        //初始化私信
        _privateMsgCtrl = AddChild<PrivateMsgViewController, PrivateMsgView>(
            View.PrivateMsgAnchor
            , PrivateMsgView.NAME);

        _disposable.Add(PrivateMsgDataMgr.Stream.Subscribe(data => _privateMsgCtrl.UpdateView(data)));
        //tweenOffset = _privateMsgCtrl.FaceCtrl.GetHight();
    }

    private void UpdatePrivateMsg(ISocialityData Idata, IPrivateMsgData data)
    {
        if (_lastTab != Idata.CurPageTab)
        {
            _privateMsgCtrl.UpdateView(data);
            _privateMsgCtrl.ResetInput();
        }
    }


    /// <summary>
    /// 向上移动
    /// </summary>
    public void TweenMoveUp()
    {
        View.FaceAnchor.SetActive(true);
        Content_TweenPosition.PlayForward();
    }

    public void TweenMoveDown()
    {
        Content_TweenPosition.PlayReverse();
    }
    public void SetTweenOffsetValue (float value)
    {
        if (Content_TweenPosition == null)
        {
            Content_TweenPosition = UITweener.Begin<TweenPosition>(View.Content, 0.5f);
            Content_TweenPosition.from = Content_TweenPosition.value;
        }
        Content_TweenPosition.to = new Vector3(Content_TweenPosition.value.x, Content_TweenPosition.value.y + value, Content_TweenPosition.value.z);
        Content_TweenPosition.enabled = false;
    }

}
