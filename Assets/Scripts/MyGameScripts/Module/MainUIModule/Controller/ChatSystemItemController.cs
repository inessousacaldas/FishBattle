// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  ChatSystemItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using AppDto;
using UniRx;
using ChatChannelEnum = AppDto.ChatChannel.ChatChannelEnum;

public interface IChatItemData{
    string UrlStr{ get;}
    ChatChannelEnum ChannelID{ get;}
}

public class ChatItemData : IChatItemData
{
    public string urlStr;
    public ChatChannelEnum channelID;

    public string UrlStr
    {
        get { return urlStr; }
    }

    public ChatChannelEnum ChannelID
    {
        get { return channelID; }
    }
}

public partial class ChatSystemItemController
{


    private const int labelWidth = 339;
    // 界面初始化完成之后的一些后续初始化工作

    private ChatItemData data;
    private Subject<IChatItemData> stream;
    private IDisposable _disposable;


    public UniRx.IObservable<IChatItemData> ItemClickOnClick {
        get { return stream; }
    }

    public UICamera.VectorDelegate onDragCallback;

    protected override void AfterInitView ()
    {
        stream = new Subject<IChatItemData>();
        data = new ChatItemData();
        //_view.content_UILabel.width = labelWidth;
        _view.eventListener.onDrag = (go, delta) =>
        {
            if (onDragCallback != null)
                onDragCallback(go, delta);
        };
        _disposable = _view.content.OnClickAsObservable().Subscribe(_ =>
        {
            data.urlStr = View.content_UILabel.GetUrlAtPosition(UICamera.lastWorldPosition);
            stream.OnNext(data);
        });
    }


    protected override void OnDispose()
    {
        _disposable.Dispose();
        _disposable = null;
        stream = stream.CloseOnceNull();
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    //用特定接口类型数据刷新界面
    public void UpdateView(ChatNotify notify)
    {
        if (notify == null)
        {
            this.gameObject.SetActive(false);
            return;
        }
        View.content_UILabel.overflowMethod = UILabel.Overflow.ResizeFreely;
        data.channelID = (ChatChannelEnum) notify.channelId;
        this.gameObject.SetActive(true);

        string msg = "";
        if (notify.channelId == (int)ChatChannel.ChatChannelEnum.Guild)
        {
            msg = ChatHelper.ChatInfoViewToChatBox1(ChatHelper.ParseChatBoxMsg(notify));
        }
        else
        {
            msg = ColorHelper.UpdateColorInDeepStyle(ChatHelper.ParseChatBoxMsg(notify));
        }
//        View.eventListener.onDrag = MainUIDataMgr.MainUIViewController.DataMgr.OnDragChatBox;
        View.content_UILabel.spacingY = View.content_UILabel.SetEmojiText(msg) ? 15 : 8;

        if (View.content_UILabel.printedSize.x > labelWidth)
        {
            View.content_UILabel.overflowMethod = UILabel.Overflow.ResizeHeight;
            View.content_UILabel.width = (int)labelWidth;
        }
        float width = View.content_UILabel.width;
        float height = Height;
        View.content_BocCollider.center = new UnityEngine.Vector3(width / 2, -height / 2);
        View.content_BocCollider.size = new UnityEngine.Vector3(width, height);
    }

    public float Height
    {
        get { return View.content_UILabel.printedSize.y; }
    }
}
