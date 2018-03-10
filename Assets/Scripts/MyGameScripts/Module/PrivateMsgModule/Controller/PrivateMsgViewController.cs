// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  PrivateMsgViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using UnityEngine;
using UniRx;
using System.Collections.Generic;
using AppDto;

public partial interface IPrivateMsgViewController
{
    UniRx.IObservable<bool> OnItemScrollStopMoving { get; }

    string InputFiledText { get; set; }
}
public partial class PrivateMsgViewController
{
    public string InputFiledText
    {
        get { return View.Input_UIInput.value; }
        set { View.Input_UIInput.value = value; }
    }

    private Subject<bool> itemScrollViewStopMovingEvt = new Subject<bool>();
    public UniRx.IObservable<bool> OnItemScrollStopMoving
    {
        get { return itemScrollViewStopMovingEvt; }
    }

    //好友
    private List<ChatFriendItemController> _friendItemList = new List<ChatFriendItemController>();
    //消息
    private List<ChatPlayerItemController> _chatItemList = new List<ChatPlayerItemController>();

    private CompositeDisposable _disposable = null;
    Subject<FriendInfoDto> friendIdStream = new Subject<FriendInfoDto>();

    public UniRx.IObservable<FriendInfoDto> OnFriendIdStream { get { return friendIdStream; } }
    private long _curId;

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        if (_disposable == null)
            _disposable = new CompositeDisposable();

        //InitFacePanel();
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        View.RightScrollView_UIScrollView.onDragFinished += OnStopMoving;
        EventDelegate.Set(View.Input_UIInput.onChange, OnInputValueChange);
        View.SpeechBtn_UIEventListener.onPress = OnSpeechBtnPress;

        DeleteBtn_UIButtonEvt.Subscribe(_ =>
        {
            PrivateMsgDataMgr.DataMgr.ClearRecordById(_curId);
        });

        IconBtn_UIButtonEvt.Subscribe(_ =>
        {
            ProxyTips.OpenTextTips(22, new Vector3(-270, 188));
        });
    }

    protected override void OnDispose()
    {
        if (_disposable != null)
            _disposable.Dispose();
        _disposable = null;
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        View.RightScrollView_UIScrollView.onDragFinished -= OnStopMoving;
        View.SpeechBtn_UIEventListener.onPress = null;
    }

    public void UpdateView(IPrivateMsgData data)
    {
        UpdateLeftTabel(data);
        UpdateRightTabel(data);
        UpdateTopPanel(data);

        _curId = data.CurMsgId;
    }

    public void ResetInput()
    {
        View.Input_UIInput.value = "";
    }

    public void UpdateLeftTabel(IPrivateMsgData data)
    {
        _disposable.Clear();
        PrivateMsgDataMgr.DataMgr.OnReadFriendMsg(data.CurMsgId);

        //队列靠前 显示靠后
        _friendItemList.ForEach(x => x.Hide());
        int index = 0;
        var tempList = data.ChatFriendQueue.ToList();
        var needRefresh = false;
        for(int i=data.ChatFriendQueue.Count-1; i>=0; i--)
        {
            long id = tempList[i];
            var dto = data.GetInfoDtoById(id);
            if (dto != null)
            {
                var ctrl = AddChatFriendItemIfNotExist(index);
                var showRed = data.NoReadQueue.Contains(id);
                ctrl.UpdateView(dto, showRed, data.CurMsgId==dto.friendId);
                ctrl.Show();

                if (data.CurMsgId == dto.friendId && index == 0)
                    needRefresh = true;

                _disposable.Add(ctrl.OnClickItemStream.Subscribe(infoDto =>
                {
                    friendIdStream.OnNext(infoDto);
                }));

                index++;
            }
        }

        if (data.ChatFriendQueue.ToList().IsNullOrEmpty())
            SetDegree();

        View.LeftGird_UIGrid.Reposition();
        if(needRefresh)
            View.LeftScrollView_UIScrollView.ResetPosition();
    }

    private void SetDegree(int degree=0, bool isFriend=false)
    {
        View.Label_UILabel.text = degree.ToString();
    }

    public void UpdateRightTabel(IPrivateMsgData data)
    {
        if (View.RightScrollView_UIScrollView.shouldMoveVertically && data.LockState && data.UnreadCnt > 0)
        {
            View.UnReadPanel_UIPanel.gameObject.SetActive(true);
            View.UnReadCountLbl_UILabel.text = string.Format("有{0}条新消息".WrapColor(ColorConstantV3.Color_Red), data.UnreadCnt.ToString().WrapColor(ColorConstantV3.Color_Yellow));

            return;
        }

        View.UnReadPanel_UIPanel.gameObject.SetActive(false);
        var playerMsgCnt = 0;
        _chatItemList.GetElememtsByRange(playerMsgCnt, -1).ForEach(s => s.Hide());

        data.CurChatList.ForEachI((s, i) =>
        {
            var name = GetItemNameByIdx(i);
            var ctrl = AddPlayerCellIfNotExist(playerMsgCnt, name);
            ctrl.UpdateView(s, i);
            playerMsgCnt++;
            ctrl.Show();
        });

        View.RightTable_UITable.Reposition();
        //重设scrollview会重新计算mbound 下面才能获得正确的shouldMoveVertically值
        View.RightScrollView_UIScrollView.ResetPosition();

        //fish: 如果打开界面的时候table的位置不对，就是anchor计算后于Update,具体可以看shouldMoveVertically的size
        if (View.RightScrollView_UIScrollView.shouldMoveVertically)
        {
            if(data.isScrollToBot)
            {
                View.RightScrollView_UIScrollView.SetDragAmount(0f, 1f, false);
                data.isScrollToBot = false;
            }
            else
            {
                View.RightScrollView_UIScrollView.ResetPosition();
                View.RightScrollView_UIScrollView.SetDragAmount(0f, 1f, false);
            }
        }
        else
        {
            View.RightScrollView_UIScrollView.ResetPosition();
        }
    }

    public void UpdateTopPanel(IPrivateMsgData data)
    {
        if(data.CurMsgId > 0)
        {
            View.Degree.SetActive(true);
            if (FriendDataMgr.DataMgr.IsMyFriend(data.CurMsgId))
                SetDegree(FriendDataMgr.DataMgr.GetFriendDtoById(data.CurMsgId).degree, true);
            else
                SetDegree();
        }
        else
            View.Degree.SetActive(false);
    }

    private string GetItemNameByIdx(int itemIdx)
    {
        return itemIdx > 9 ? itemIdx.ToString() : string.Format("0{0}", itemIdx);
    }

    private ChatPlayerItemController AddPlayerCellIfNotExist(int idx, string name)
    {
        ChatPlayerItemController com = null;
        _chatItemList.TryGetValue(idx, out com);
        if (com == null)
        {
            com = AddChild<ChatPlayerItemController, ChatPlayerItem>(View.RightTable_UITable.gameObject, ChatPlayerItem.NAME);
            _chatItemList.Add(com);
        }

        return com;
    }

    private ChatFriendItemController AddChatFriendItemIfNotExist(int idx)
    {
        ChatFriendItemController com = null;
        _friendItemList.TryGetValue(idx, out com);
        if (com == null)
        {
            com = AddChild<ChatFriendItemController, ChatFriendItem>(View.LeftGird_UIGrid.gameObject, ChatFriendItem.NAME);
            _friendItemList.Add(com);
        }

        return com;
    }

    private void OnInputValueChange()
    {

        if (View.Input_UIInput.characterLimit == View.Input_UIInput.value.Length)
        {
            TipManager.AddTip("内容长度超出上限");
            return;
        }
    }

    private void OnSpeechBtnPress(GameObject go, bool state)
    {
        if (state)
        {
            SpeechFlagItemController.OpenSpeechFlag((int)PrivateMsgDataMgr.Stream.LastValue.CurMsgId);
        }
        else
        {
            SpeechFlagItemController.SendVoiceMessage();
        }
    }

    private void OnStopMoving()
    {
        View.RightScrollView_UIScrollView.UpdateScrollbars(true);
        Vector3 constraint = View.RightScrollView_UIScrollView.panel.CalculateConstrainOffset(
            View.RightScrollView_UIScrollView.bounds.min
            , View.RightScrollView_UIScrollView.bounds.min);

        itemScrollViewStopMovingEvt.OnNext(constraint.y <= 0.001f);
    }
}
