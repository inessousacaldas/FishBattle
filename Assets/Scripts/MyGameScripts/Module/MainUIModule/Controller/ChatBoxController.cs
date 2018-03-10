// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  ChatBoxController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using System.Collections.Generic;
using AppDto;
using UniRx;
using UnityEngine;
using ChatData = ChatDataMgr.ChatData;

public partial interface IChatBoxController
{
    void SetQuizariumState(bool b);
    UniRx.IObservable<IChatItemData> ChatItemClickEvt { get; }
    void OnClickChatBoxExpandBtn(bool _isUp);
    bool IsUp { get; set; }
}

public partial class ChatBoxController
{
    // 界面初始化完成之后的一些后续初始化工作
    private CompositeDisposable _disposable;

    private bool isUp = false;
    public bool IsUp { get { return isUp; }set { isUp = value; } }

    private List<ChatSystemItemController> _chatBoxItemList;

    private Subject<IChatItemData> stream;
    public UniRx.IObservable<IChatItemData> ChatItemClickEvt {
        get { return stream; }
    }

    private Subject<bool> scrollStopDragEvt = new Subject<bool>();
    public UniRx.IObservable<bool> OnScrollStopDrag
    {
        get { return scrollStopDragEvt; }
    }

    //private Subject<Unit> _refreshAnchor;

    //public UniRx.IObservable<Unit> RefreshAnchor
    //{
    //    get { return _refreshAnchor; }
    //}

    public S3PopupListController TeamVoicePopUpListContrllor;
    private ChatHornItemController chatHornItemCtrl;

    private const string CHATBOXDELAYREPOSITION = "ChatBoxDelayReposition";
    protected override void AfterInitView ()
    {
        _disposable = new CompositeDisposable();
        stream = new Subject<IChatItemData>();
        //_refreshAnchor = new Subject<Unit>();

        _chatBoxItemList = new List<ChatSystemItemController>(ChatDataMgr.ChatBoxData.CHATBOX_MAXCOUNT);

        for (int i = 0; i < ChatDataMgr.ChatBoxData.CHATBOX_MAXCOUNT; i++)
        {
            var ctrl = AddCachedChild<ChatSystemItemController, ChatSystemItem>(
                View.ChatTable_UITable.gameObject, ChatSystemItem.NAME);
            ctrl.transform.SetAsFirstSibling();
            //ctrl.onDragCallback = OnDragChatBox;
            _disposable.Add(ctrl.ItemClickOnClick.Subscribe(item =>
            {
                stream.OnNext(item);
            }));
            ctrl.Hide();
            _chatBoxItemList.Add(ctrl);
        }
        CreatHornItem();
        InitRealTimeVoiceBtn();
        View.ChatContent_UIPanel.GetComponent<UIScrollView>().UpdatePosition();
    }
    List<PopUpItemInfo> teamPopupNameList = new List<PopUpItemInfo>() {
        new PopUpItemInfo("静音模式", (int)TeamVoiceMode.Mut),
        new PopUpItemInfo("收听模式", (int)TeamVoiceMode.OnlyVoice),
        new PopUpItemInfo("开麦模式", (int)TeamVoiceMode.All) };
    S3PopUpItemData onChoiceData = new S3PopUpItemData() { rect = new Vector2(150, 30), bgSprite = "dropbox_btn_name1", fontSize = 20 };
    S3PopUpItemData onNormalData = new S3PopUpItemData() { rect = new Vector2(150, 30), bgSprite = "dropbox_btn_name2", fontSize = 18 };
    //队伍实时语音按钮
    private void InitRealTimeVoiceBtn()
    {
        ///语音按钮
        TeamVoicePopUpListContrllor = AddController<S3PopupListController, S3PopupList>(View.TeamBtnGameObject);
        TeamVoicePopUpListContrllor.InitData(S3PopupItem.NAME,teamPopupNameList, onChoiceData,onNormalData);
    }
    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
       // _view.ChatBoxEventListener.onDrag = OnDragChatBox;
        _view.ChatBoxEventListener.onClick = OnChatBoxEventClick;
        View.ChatContent_Scroll.onDragFinished += OnStopingDrag;
        EventDelegate.Add(_view.QuizariumBtn_UIButton.onClick, () =>
        {
            FunctionOpen functionOpen = DataCache.getDtoByCls<FunctionOpen>((int)FunctionOpen.FunctionOpenEnum.FUN_73);
            if (ModelManager.Player.GetPlayerLevel() < functionOpen.grade)
            {
                TipManager.AddTip(string.Format("需要{0}级才能参与知识竞答玩法",40));
                return;
            }
            ProxyQuestion.OpenQuizariumView();
        });
        
    }
    private void OnChatBoxEventClick(GameObject go)
    {
        stream.OnNext(null);
    }
    private void OnTeamBtnClick(GameObject go)
    {

    }
    protected override void OnDispose()
    {
        //_refreshAnchor = _refreshAnchor.CloseOnceNull()
        _disposable = _disposable.CloseOnceNull();
        JSTimer.Instance.CancelCd(CHATBOXDELAYREPOSITION);
        JSTimer.Instance.CancelCd("ChatBoxQuizarium");
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        View.ChatContent_Scroll.onDragFinished -= OnStopingDrag;
        _view.ChatBoxEventListener.onClick = null;
    }

    //用特定接口类型数据刷新界面
    public void UpdateView(IChatData data)
    {
        var chatBoxData = data.ChatBoxData;
        var notifyQueue = chatBoxData.GetChatBoxMsgs();
        
        int c = data.ChatBoxData.ChatBox_MaxCount - 1;
        notifyQueue.ForEach(item =>
        {
            _chatBoxItemList[c].UpdateView(item);
            c--;
        });
        View.ChatTable_UITable.RepositionDelay(delegate { View.ChatContent_Scroll.SetDragAmount(0f, 1f, false); });
        //View.ChatTable_UITable.Reposition();
        //JSTimer.Instance.SetupCoolDown(CHATBOXDELAYREPOSITION, 0.1f, null, );
        TeamVoicePopUpListContrllor.SetChoice((int)data.ChatBoxData.CurTeamVoiceMode);
    }

    public void UpdateChatBoxUnRead(IChatBoxData data)
    {
        var unReadMsg = data.ChatBoxNewMsgCnt;
        if(unReadMsg.Count > 0)
        {
            var count = unReadMsg[0];
            if(count > 0)
            {
                View.UnReadBtn_Go.SetActive(true);
                View.UnReadCountLbl_UILabel.text = string.Format("[FE514E]有[-][F4D068]{0}[-][FE514E]条新消息[-]", count);
            }
            else
                View.UnReadBtn_Go.SetActive(false);
        }
    }

    public void SetQuizariumState(bool b)
    {
        _view.QuizariumBtn_UIButton.gameObject.SetActive(b);
    }

    public void SetQuizraiumTime(int time)
    {
        if (time <= 0)
            return; 

        JSTimer.Instance.SetupCoolDown("ChatBoxQuizarium", time + 1, e =>
        {
            _view.QuizariumTimeLb_UILabel.text = string.Format("{0}", time);
            time -=1;
        }, () =>
        {
            _view.QuizariumTimeLb_UILabel.text = string.Format("{0}", time);
            SetQuizariumState(false);
            JSTimer.Instance.CancelCd("ChatBoxQuizarium");
        }, 1f);
    }

    #region 喇叭

    private void CreatHornItem()
    {
        chatHornItemCtrl = AddChild<ChatHornItemController, ChatHornItem>(View.Horn_Go, ChatHornItem.NAME);
        chatHornItemCtrl.Hide();
    }
    //刷新喇叭
    public void UpdateHornView(IChatData data)
    {
        var chatBoxData = data.ChatBoxData;
        var notifyQueue = chatBoxData.GetChatHornMsgs();
        var hornList = data.ChatInfoViewData.HornList;
        var cdTask = JSTimer.Instance.GetCdTask(chatHornItemCtrl.GetTaskName);
        if (notifyQueue.Count > 0)
        {
            if(cdTask==null)
                UpdateHornView(notifyQueue, hornList);
            else
            {
                if (cdTask.isValid == false)
                    UpdateHornView(notifyQueue, hornList);
            }
        }
    }
    private void UpdateHornView(Queue<ChatNotify> notifyQueue, IEnumerable<ChatPropsConsume> hornList)
    {
        chatHornItemCtrl.UpdateView(notifyQueue, hornList);
    }
    
    #endregion

    #region ChatBox size change

    public void OnClickChatBoxExpandBtn(bool _isUp)
    {
        RefreshChatBoxAnchor(_isUp);
        UpdateChatPanelAnchor(_isUp);
    }

    private void RefreshChatBoxAnchor(bool _isUp)
    {
        View.UpBtn_UISprite.flip = _isUp
            ? UIBasicSprite.Flip.Vertically
            : UIBasicSprite.Flip.Nothing;
    }

    //更新聊天面板移动后的相关对象位置
    private void UpdateChatPanelAnchor(bool _isUp)
    {
        View.ChatBox_UISprite.topAnchor.absolute = _isUp
            ? 24
            : -24;
        View.ChatBox_UISprite.UpdateAnchors();
        if (LayerManager.Instance.CurUIMode != UIMode.BATTLE)
        {
//            View.NewestActivityBtn.gameObject.GetComponent<UISprite>().UpdateAnchors();
        }
        View.UpBtn_UIButton.gameObject.GetComponent<UISprite>().UpdateAnchors();
        //View.SetUpBtn_UIButton.gameObject.GetComponent<UISprite>().UpdateAnchors();

        View.Horn_Widget.UpdateAnchors();
        View.ChatContent_UIPanel.UpdateAnchors();
        View.Interception_UIWidget.UpdateAnchors();
        View.SpeakBtnGrid_UIWidget.UpdateAnchors();
        View.Trumpet_UIWidget.UpdateAnchors();
        //View.UpBtn_UIButton.sprite.UpdateAnchors();
        View.QuizariumBtn_UISprite.UpdateAnchors();
        View.ChatTable_UIWidget.UpdateAnchors();
        View.ChatContent_Scroll.ResetPosition();
        View.ChatContent_Scroll.SetDragAmount(0f, 1f, false);
    }

    private const float ChatBoxDragLimit = 1f;
    private void OnDragChatBox(GameObject go, Vector2 delta)
    {
        if (delta.y > 0
            && Mathf.Abs(delta.y) > ChatBoxDragLimit)
        {
            //手势操作，如果向上滑划大于 scrollLen 正值 就往上 负值 就往下
            isUp = true;
        }
        else if(delta.y < 0
        && Mathf.Abs(delta.y) > ChatBoxDragLimit)
        {
            isUp = false;
        }
        RefreshChatBoxAnchor(isUp);
        UpdateChatPanelAnchor(isUp);
    }

    #endregion

    private void OnStopingDrag()
    {
        UIScrollView sroll = View.ChatContent_Scroll;
        Vector3 constrain = sroll.panel.CalculateConstrainOffset(sroll.bounds.min, sroll.bounds.max);
        bool isLock = constrain.y <= -0.001f;

        if (!isLock)
        {
            var chatItem = _chatBoxItemList.Filter(e => e.IsActive());
            var count = chatItem.ToList().Count;
            if(count <= 6)
            {
                int height = 0;
                chatItem.ForEach(e =>
                {
                    height += e.View.content_UILabel.height;
                });
                isLock = height <= View.ChatTable_UIWidget.height;
            }
        }
        scrollStopDragEvt.OnNext(isLock);
    }

}
