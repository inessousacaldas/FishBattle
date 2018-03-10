// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  ChatViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using AppDto;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using ChatChannelEnum = AppDto.ChatChannel.ChatChannelEnum;

/// <summary>
/// 左侧聊天框 以及聊天输入框
/// </summary>
public class ChatViewData
{
    public bool _speechMode = false; //是否在语音模式下
    public string _urlMsg="";//超链接的信息，额外的缓存起来了
    public string _defalutInputText; //默认的输入字符
    public bool isEnable = true;
    //public bool LockState = false;// 是否锁屏不刷新
    public int UnrealCnt;
    public Dictionary<ChatChannelEnum, int> newMsgCnt;
    public ChatNotify finalyAt;//最后一个at自己的人
}
public partial interface IChatViewController
{
    UniRx.IObservable<bool> OnItemScrollStopMoving { get; }
    event UIEventListener.BoolDelegate OnSpeechBtnPressEvt;
    FacePanelViewController FaceCtrl { get; }
    void SetFacePanelVisible(bool isVisible);

    string InputFiledText { get; set; } //提供输入框直接更改
    bool ViewIsNull { get; }
    /// <summary>
    /// 刷新聊天界面
    /// </summary>
    /// <param name="chatList"></param>
    void UpdateChatList(IChatData chatList,bool needDelay = true);

    void SetRightPanelHeight(bool isCanChat);
    UniRx.IObservable<string> InputMsgHandle { get; }
    UniRx.IObservable<Unit> InputPress { get; }
    void ShowFaceView(bool showFace);
    UniRx.IObservable<string> LongPress { get; }
    void HideAtFunc();
    List<Vector3> GetAtScrollPos(ChatNotify notify);
    void ScrollToPos(List<Vector3> v3List);
    #region 喇叭
    void UpdateHornPanel(IChatInfoViewData chatViewData);
    UniRx.IObservable<HornTypeBtnController> OnHornBtnClick { get; }
    void OnHornTypeBtnClick(HornTypeBtnController ctrl);
    void OnHornChooseBtnClick();
    void OnHornCloseBtnClick();
    int CurHornId { get; }
    #endregion
}
public partial class ChatViewController
{
    #region Event
    private Subject<bool> itemScrollViewStopMovingEvt = new Subject<bool>();
    public UniRx.IObservable<bool> OnItemScrollStopMoving
    {
        get { return itemScrollViewStopMovingEvt; }
    }
    public event UIEventListener.BoolDelegate OnSpeechBtnPressEvt;
    #endregion

    //玩家消息
    private List<ChatPlayerItemController> _playerChatItemList = new List<ChatPlayerItemController>();
    //系统消息
    private List<ChatSystemItem2Controller> _systemChatItemList = new List<ChatSystemItem2Controller>();

    CompositeDisposable _disposable;
    //表情等面板~
    FacePanelViewController faceCtrl;
    public FacePanelViewController FaceCtrl
    {
        get { return faceCtrl; }
    }

    public bool ViewIsNull
    {
        get { return View == null; }
    }

    public string InputFiledText {
        get { return View.Input_UIInput.value; }
        set { View.Input_UIInput.value = value; } }

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        _disposable = new CompositeDisposable();
        InitFacePanel();
        //TestUI();
    }
    private void InitFacePanel()
    {
        FacePanelViewController.CurShowType = FaceShowType.SHOW_TYPE_Chat;
        faceCtrl = AddChild<FacePanelViewController, FacePanelView>(
                _view.FaceAnchor
                , FacePanelView.NAME
            );
        _disposable.Add(FaceCtrl.SelectStream.Subscribe(x=> {

        }));
        
    }
    public void SetFacePanelVisible(bool isVisible)
    {
        //_view.FaceAnchor.SetActive(isVisible);
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        View.ItemScrollView_UIScrollView.onDragFinished += OnStopMoving;
        EventDelegate.Set(View.Input_UIInput.onChange, OnInputValueChange);
        UIEventListener.Get(View.Input_UIInput.gameObject).onPress += OnPressInput;
        View.SpeechBtn_UIEventListener.onPress = (go,state)=> { if(OnSpeechBtnPressEvt!=null) OnSpeechBtnPressEvt(go, state); };
        View.SpeechBtn_UIEventListener.onDrag = OnSpeechBtnDrag;
    }

    Subject<bool> isSel = new Subject<bool>();
    public UniRx.IObservable<bool> IsSel { get { return isSel; } }
   

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent()
    {
        _disposable.Clear();
        
        View.ItemScrollView_UIScrollView.onDragFinished -= OnStopMoving;
        View.SpeechBtn_UIEventListener.onPress = null;
        View.SpeechBtn_UIEventListener.onDrag = null;
        UIEventListener.Get(View.Input_UIInput.gameObject).onPress -= OnPressInput;
        EventDelegate.Remove(View.Input_UIInput.onChange, OnInputValueChange);

    }
   
    protected override void OnDispose()
    {
        _disposable.Dispose();
    }

    #region @好友功能

    private Subject<string> inputMsgHandle = new Subject<string>();
    public UniRx.IObservable<string> InputMsgHandle { get { return inputMsgHandle; } }
    private Subject<Unit> inputPress = new Subject<Unit>();
    public UniRx.IObservable<Unit> InputPress { get { return inputPress; } }
    private Subject<string> longPress = new Subject<string>();
    public UniRx.IObservable<string> LongPress { get { return longPress; } }

    private void OnPressInput(GameObject go,bool press)
    {
        if (press)
        {
            inputPress.OnNext(new Unit());
        }
    }
    public void HideAtFunc()
    {
        View.atFuncBtn_UIButton.gameObject.SetActive(false);
        View.UnReadPanel.gameObject.SetActive(false);
        View.UnReadPanelTabel_UITabel.Reposition();
    }
    #endregion
    #region 喇叭

    private List<HornTypeBtnController> hornTypeList = new List<HornTypeBtnController>();
    private HornTypeBtnController curHornType;
    private int hornId = -1;
    public int CurHornId { get { return hornId; } }
    private Subject<HornTypeBtnController> HornBtnEvt = new Subject<HornTypeBtnController>();
    public UniRx.IObservable<HornTypeBtnController> OnHornBtnClick { get { return HornBtnEvt; } }
    public void UpdateHornPanel(IChatInfoViewData chatViewData)
    {
        if (!View.HornPanel.activeSelf)
        {
            if (hornTypeList.Count == 0)
            {
                chatViewData.HornList.ForEach(e =>
                {
                    var ctrl = AddChild<HornTypeBtnController, HornTypeBtn>(View.hornBtnGrid.gameObject, HornTypeBtn.NAME);
                    ctrl.UpdateView(e);
                    _disposable.Add(ctrl.OnHornTypeBtn_UIButtonClick.Subscribe(g => HornBtnEvt.OnNext(ctrl)));
                    hornTypeList.Add(ctrl);
                });
            }
            OnHornTypeBtnClick(curHornType == null ? hornTypeList[0] : curHornType);
            View.HornPanel.SetActive(true);
            View.hornBtnGrid.Reposition();
        }
    }
    public void OnHornTypeBtnClick(HornTypeBtnController ctrl)
    {
        if (ctrl == null) return;
        if (curHornType!=null)
            if (curHornType.Props.id  == ctrl.Props.id) return;
        curHornType = ctrl;
        ChatPropsConsume props = ctrl.Props;
        //var item = ItemHelper.GetGeneralItemByItemId(props.id);
        //if (item != null)
            View.hornDesLabel.text = "使用该喇叭发言需要消耗" + props.name;
        //else
           // GameDebuger.Log("id: " + props.id + "该喇叭道具不存在");
        View.chooseLabel.text = props.name;
    }

    public void OnHornChooseBtnClick()
    {
        if (curHornType != null)
            hornId = curHornType.Props.id;
        OnHornCloseBtnClick();
    }
    public void OnHornCloseBtnClick()
    {
        View.HornPanel.SetActive(false);
    }

    #endregion
    #region 聊天输入框
    public void UpdateInputState(IChatInfoViewData chatViewData)
    {
        bool _speechMode = chatViewData.chatViewData._speechMode;
        _view.SpeechBtn_UIEventListener.gameObject.SetActive(_speechMode);
        _view.SpeedOrInputBg_UISprite.spriteName = _speechMode ? "icon_26" : "icon_26";
        _view.Input_UIInput.gameObject.SetActive(!_speechMode);
        _view.Input_UIInput.characterLimit = chatViewData.CurChannelId == ChatChannelEnum.Horn ? 31 : 51;
        View.SpeedOrInputBtn_UIButton.gameObject.SetActive(chatViewData.CurChannelId != ChatChannelEnum.Horn);
        View.HornBtn_UIButton.gameObject.SetActive(chatViewData.CurChannelId == ChatChannelEnum.Horn);

        int curHornId = chatViewData.CurHordId;
        var hornList = chatViewData.HornList;
        
        if(curHornId == -1)
            View.HornLabel_Label.text = hornList.ToArray()[0].name;
        else
            View.HornLabel_Label.text = hornList.Find(e => curHornId == e.id).name;
    }
    private void OnSpeechBtnDrag(GameObject go, Vector2 delta)
    {
        SpeechFlagItemController.HandleDragEvent(delta);
    }
    private void OnInputValueChange()
    {
        if (_view.Input_UIInput.characterLimit == _view.Input_UIInput.value.Length)
        {
            TipManager.AddTip("内容长度超出上限");
            return;
        }
        inputMsgHandle.OnNext(_view.Input_UIInput.value);
    }


    public void ShowFaceView(bool showFace)
    {
        if (faceCtrl != null)
        {
            faceCtrl.View.Face_Go.SetActive(showFace);
            faceCtrl.View.AtPanel_Go.SetActive(!showFace);
        }
    }
    #endregion

    public void UpdateView(IChatInfoViewData chatViewData)
    {
        UpdateInputState(chatViewData);
        View.WarnTip.SetActive(!chatViewData.chatViewData.isEnable);
        // SetRightPanelHeight(chatViewData.isEnable);
        View.ChatInputGroup.gameObject.SetActive(chatViewData.chatViewData.isEnable);
        //View.Input_UIInput.enabled = chatViewData.isEnable;
        //View.Input_UIInput.defaultText = chatViewData._defalutInputText;
        var unReadMsg = chatViewData.chatViewData.newMsgCnt;
        View.UnReadPanel.gameObject.SetActive(false);
        if (unReadMsg != null)
        {
            if (unReadMsg.ContainsKey(chatViewData.CurChannelId))
            {
                var unrealCnt = unReadMsg[chatViewData.CurChannelId];
                if (unrealCnt > 0)
                {
                    View.UnReadBtn_UIButton.gameObject.SetActive(true);
                    View.UnReadCountLbl_UILabel.text = string.Format("[FE514E]有[-][F4D068]{0}[-][FE514E]条新消息[-]", unrealCnt);
                    View.UnReadPanel.gameObject.SetActive(true);
                }
                else
                {
                    View.UnReadBtn_UIButton.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            View.UnReadBtn_UIButton.gameObject.SetActive(false);
        }
        var atFunc = chatViewData.chatViewData.finalyAt;
        if (atFunc != null)
        {
            if (atFunc.channelId != (int)ChatChannelEnum.Team) return;
            View.atFuncBtn_UIButton.gameObject.SetActive(true);
            View.atFuncBtnLbl_Label.text = string.Format("[64BCF1]有人在{0}频道[-][F4D068]@[-][64BCF1]了你[-]", ChatHelper.GetChannelName((ChatChannelEnum)atFunc.channelId));
            View.UnReadPanel.gameObject.SetActive(true);
        }
        else
        {
            View.atFuncBtn_UIButton.gameObject.SetActive(false);
        }
        View.UnReadPanelTabel_UITabel.Reposition();
    }

    /// <summary>
    /// 更新聊天信息
    /// </summary>
    /// <param name="chatList"></param>
    public void UpdateChatList(IChatData data,bool needDelay = true)
    {
        IEnumerable<ChatNotify> chatList = data.GetChannelNotifyQueue(data.CurChannelId);
        var playerMsgCnt = 0;
        var sysMsgCnt = 0;
        
        _playerChatItemList.GetElememtsByRange(playerMsgCnt, -1).ForEach(s => s.Hide());
        _systemChatItemList.GetElememtsByRange(sysMsgCnt, -1).ForEach(s => s.Hide());
        
        chatList.ForEachI((s, i) =>
        {
            //系统、公会系统推送
            if(s.channelId == (int)ChatChannelEnum.System || (s.channelId == (int)ChatChannelEnum.Guild) && s.fromPlayer == null)
            {
                var name = GetItemNameByIdx(i);
                var ctrl = AddSystemCellIfNotExist(sysMsgCnt, name);
                ctrl.UpdateView(s);
                sysMsgCnt++;
                ctrl.Show();
            }
            else
            {
                if (data.CurChannelId == ChatChannelEnum.Guild && !data.ChatInfoViewData.HasGuild) return;
                var name = GetItemNameByIdx(i);
                var ctrl = AddPlayerCellIfNotExist(playerMsgCnt, name);
                _disposable.Add(ctrl.LongPress.Subscribe(e => longPress.OnNext(e)));
                ctrl.UpdateView(s);
                playerMsgCnt++;
                ctrl.Show();
            }

        });
        if (!needDelay)
            View.ItemTable_UITable.Reposition();
        else
            View.ItemTable_UITable.RepositionDelay(() => { UpdateChatScrollView(); });
    }

    public void UpdateChatScrollView()
    {
        if (_view.ItemScrollView_UIScrollView.shouldMoveVertically)
        {
            _view.ItemScrollView_UIScrollView.SetDragAmount(0f,1f, false); //重置到底部
        }
        else
        {
            _view.ItemScrollView_UIScrollView.ResetPosition(); //重置到顶部
        }
    }

    public List<Vector3> GetAtScrollPos(ChatNotify notify)
    {
        var item = _playerChatItemList.Find(e => notify.fromTime == e.ChatNotify.fromTime);
        List<Vector3> v3List = new List<Vector3>();
        if (item != null)
        {
            Vector3 v3 = item.transform.localPosition;
            float rate = v3.y - 81;
            float sub = -454 - rate;
            v3 = new Vector3(v3.x, sub, v3.z);
            v3List.Add(v3);
            v3List.Add(item.transform.position);
        } 
        return v3List;
    }

    public void ScrollToPos(List<Vector3> v3List)
    {
        int count = v3List.Count;
        if(count > 1)
        {
            Vector3 localPos = v3List[0];
            Vector3 worldPos = v3List[1];
            bool isVisible = View.ItemScrollView_UIScrollView.panel.IsVisible(worldPos);
            if(!isVisible)
                SpringPanel.Begin(View.ItemScrollView_UIScrollView.gameObject, localPos, 8);
        }
    }
    private string GetItemNameByIdx(int itemIdx)
    {
        return itemIdx > 9 ? itemIdx.ToString() : string.Format("0{0}", itemIdx);
    }

    private ChatSystemItem2Controller AddSystemCellIfNotExist(int idx, string name)
    {
        ChatSystemItem2Controller com = null;
        _systemChatItemList.TryGetValue(idx, out com);
        if (com == null)
        {

            com = AddCachedChild<ChatSystemItem2Controller, ChatSystemItem2>(
                _view.ItemTable_UITable.gameObject
                , ChatSystemItem2.NAME
                , name);
            _systemChatItemList.Add(com);
        }
        return com;
    }

    private ChatPlayerItemController AddPlayerCellIfNotExist(int idx, string name)
    {
        ChatPlayerItemController com = null;
        _playerChatItemList.TryGetValue(idx, out com);
        if (com == null)
        {
            com = AddCachedChild<ChatPlayerItemController, ChatPlayerItem>(
                _view.ItemTable_UITable.gameObject
                , ChatPlayerItem.NAME
                , name);
            _playerChatItemList.Add(com);
        }
        return com;
    }


    private void OnStopMoving()
    {
        View.ItemScrollView_UIScrollView.UpdateScrollbars(true);
        Vector3 constraint = View.ItemScrollView_UIScrollView.panel.CalculateConstrainOffset(
            View.ItemScrollView_UIScrollView.bounds.min
            , View.ItemScrollView_UIScrollView.bounds.max);

        bool isLock = constraint.y <= -0.001f;
        GameDebuger.Log(string.Format("ChatViewController :isLock {0} ,constraint {1} => ",isLock,constraint));
        itemScrollViewStopMovingEvt.OnNext(isLock);
    }

    public void SetRightPanelHeight(bool isCanChat)
    {
        View.RightPanel_UIWidget.bottomAnchor.absolute = isCanChat ? 90 : 43;
        View.RightPanel_UIWidget.ResetAnchors();
    }
}
