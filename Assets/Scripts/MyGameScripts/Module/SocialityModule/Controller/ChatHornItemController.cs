// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  ChatHornItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using AppDto;
using System.Collections.Generic;
using UnityEngine;

public partial class ChatHornItemController
{
    private const string TaskName = "ChatHornItemInMainUI";
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
 
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {

    }

    protected override void OnDispose()
    {
        JSTimer.Instance.CancelTimer(TaskName);
        base.OnDispose();
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    public string GetTaskName { get { return TaskName; } }

    public void UpdateView(Queue<ChatNotify> notifyQueue, IEnumerable<ChatPropsConsume> hornList)
    {
        UIWidget parent = View.transform.parent.GetComponent<UIWidget>();
        var notify = notifyQueue.Dequeue();
        string content = "[94D0E5]【" + notify.fromPlayer.nickname + "】[-][F1CE57]" + notify.content + "[-]";
        bool hasEmoji = View.content_EmojiAnimationController.SetEmojiText(content);

        if(View.content_EmojiAnimationController.width > 306)
        {
            View.content_EmojiAnimationController.overflowMethod = UILabel.Overflow.ResizeHeight;
            View.content_EmojiAnimationController.width = 306;
        }
        View.content_EmojiAnimationController.spacingY = hasEmoji ? 25 : 8;

        float yHeight = View.content_EmojiAnimationController.printedSize.y;
        float rate = hasEmoji ? 43 : 26;
        float res = yHeight / rate;
        
        int s = (int)res;
        parent.bottomAnchor.absolute = s > 2 ? s * 40 + 10 : s * 40;
        int absoulut = parent.bottomAnchor.absolute;
        parent.bottomAnchor.absolute = hasEmoji ? absoulut + 50 * s : absoulut;

        View.Show();
        float durationTime = hornList.Find(e => e.id == notify.itemId).duration;
        durationTime = notifyQueue.Count == 0 ? durationTime : durationTime * 0.75f;
        JSTimer.Instance.SetupCoolDown(TaskName, durationTime, null, delegate 
        {
            if(notifyQueue.Count > 0)
            {
                UpdateView(notifyQueue, hornList);
            }
            else
            {
                View.Hide();
            }
            if(notifyQueue.Count == 0 && !View.gameObject.activeSelf)
            {
                JSTimer.Instance.CancelTimer(TaskName);
            }
        });
    }
}
