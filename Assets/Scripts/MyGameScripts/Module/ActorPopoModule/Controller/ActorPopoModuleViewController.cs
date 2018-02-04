// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  ActorPopoModuleViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using UnityEngine;

public partial class ActorPopoModuleViewController
{
    private long _id;
    private EmojiAnimationController _emojiLbl;
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        _emojiLbl = _view.MsgLabel_UILabel.cachedGameObject.GetMissingComponent<EmojiAnimationController>();
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {

    }

    protected override void OnDispose()
    {

    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    public void Open(long id,Transform target,string msg,Camera gameCamera,float offsetY,float delayToClose)
    {
        if(_view == null)
        {
            AfterInitView();
        }

        _id = id;
        _view.MsgLabel_UILabel.overflowMethod = UILabel.Overflow.ResizeFreely;
        bool hasEmoji = _emojiLbl.SetEmojiText (msg,_view.MsgLabel_UILabel);
        //播放剧情的时候，会影响对话的坐标，设置完表情取消启用
        _emojiLbl.enabled = false;

        if(_view.MsgLabel_UILabel.printedSize.x > 200f)
        {
            _view.MsgLabel_UILabel.overflowMethod = UILabel.Overflow.ResizeHeight;
            _view.MsgLabel_UILabel.width = 200;
            _view.MsgLabel_UILabel.spacingY = 3;
        }

        if(hasEmoji)
        {
            _view.MsgLabel_UILabel.spacingY = 15;
        }
        else
        {
            _view.MsgLabel_UILabel.spacingY = 3;
        }

        CancelInvoke("DelayClose");
        Invoke("DelayClose",delayToClose);

        _view.ActorPopoModuleView_UIFollowTarget.gameCamera = gameCamera;
        _view.ActorPopoModuleView_UIFollowTarget.uiCamera = LayerManager.Root.UICamera.cachedCamera;
        //_view.ActorPopoModuleView_UIFollowTarget.uiCamera = LayerManager.Root.SceneHUDCamera;
        _view.ActorPopoModuleView_UIFollowTarget.target = target;
        _view.ActorPopoModuleView_UIFollowTarget.offset = new Vector3(0f,offsetY,0f);
        _view.MsgLabel_UILabel.depth = ProxyActorPopoModule.ActorPopoCount * 5 + 2;
        _view.Background_UISprite.depth = ProxyActorPopoModule.ActorPopoCount * 5;
    }

    private void DelayClose()
    {
        ProxyActorPopoModule.Close(_id);
    }
}
