// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  MsgRecordItemCellController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using UniRx;

public partial class MsgRecordItemCellController
{
    private Subject<Unit> onItemClickEvt;

    public UniRx.IObservable<Unit> OnItemClick{
        get { return onItemClickEvt; }
    }

    ChatRecordVo vo;
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
 
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        onItemClickEvt = _view.Content_UISprite.gameObject.OnClickAsObservable();
    }

    protected override void OnDispose()
    {
        onItemClickEvt = onItemClickEvt.CloseOnceNull();
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    //用特定接口类型数据刷新界面
    public void UpdateView(ChatRecordVo vo)
    {
        this.vo = vo;
        _view.MsgLbl_UILabel.text = vo.InputStr;
    }

    //public void SetSelect(bool select)
    //{
    //    _view.Content_UISprite.spriteName = select
    //        ? "found-team-under-lines-selected"
    //        : "found-team-under-lines";
    //}

    public ChatRecordVo GetContent()
    {
        return vo;
    }
}
