// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  EmailContentController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using System.Collections.Generic;
using AppDto;
using AppServices;
using UniRx;
using UnityEngine;
using EmailNetMsg = EmailDataMgr.EmailNetMsg;

public partial class EmailContentController
{
    public IEmailData _emailData;
    private CompositeDisposable _disposable;
    private List<EmailItemController> _itemCtlList;
    private int _mailItemHeight = 82;  //EmailItemController 高度

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        //SetAnchorTarget(transform.parent);
        _disposable = new CompositeDisposable();
        InitEmailView();
    }

    //设置锚点
    //private void SetAnchorTarget(Transform trans)
    //{
    //    View.EmailContent_UIWidget.SetAnchor(trans);
    //}

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        //_disposable.Add(EmailDataMgr.Stream.Subscribe(data => UpdateView(data)));
      //  _disposable.Add(OnOnKeyReceive_UIButtonClick.Subscribe(_ => OnOneKeyReceiveRequest()));
      //  _disposable.Add(OnDelRead_UIButtonClick.Subscribe(_ => OnDelReadRequest()));
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
        
    }

    //用特定接口类型数据刷新界面
    public void UpdateView(IEmailData data)
    {
        //邮件刷新机制 只有进入界面才会排序邮件 所以在SocialityViewLogic中处理（EmailDataMgr.DataMgr.SetSortEmail();）

        _emailData = data;
        if (data.MailDtoList == null) return;

        if (_itemCtlList == null)
        {
            _itemCtlList = new List<EmailItemController>();
            _emailData.CurMailIdx = 0;
        }


        EmailItemController item;
        int dtoCount = 0;
        data.MailDtoList.ForEachI((dto, idx) =>
        {
            if (_itemCtlList.Count <= idx)      //列表中的item数少于dto数时
            {
                EmailItemController com = AddChild<EmailItemController, EmailItem>(
                _view.MailPos_UITable.gameObject, EmailItem.NAME, string.Format("EmailItem{0}", idx));
                _itemCtlList.Add(com);
            }

            _itemCtlList.TryGetValue(idx, out item);
            if (item != null)
            {
                item.UpdateView(dto, OnItemCellSelect);
                item.Show();
                if(data.CurMailDto.id == dto.id)
                    item.Select(true);
            }
            dtoCount++;
        });

        if (_itemCtlList.Count > dtoCount)     //隐藏多余的item
        {
            for (int i = dtoCount; i < _itemCtlList.Count; i++)
            {
                _itemCtlList[i].Hide();
            }
        }

        if (!data.IsDeleteAll)
        {
            //AutoSelectnextEmail();
        }
        else
        {
            data.IsDeleteAll = false;
            _emailViewCtl.Hide();
            View.ScrollView_UIScrollView.ResetPosition();
        }

        View.MailPos_UITable.Reposition();

        //判断scrollview是不是拉到最下面
        if (data.MailDtoList.ToList().Count > 6 && View.ScrollView_UIScrollView.panel.clipOffset.y < _mailItemHeight * (6 - data.MailDtoList.ToList().Count))
        {
            _view.ScrollView_UIScrollView.ResetPosition();
            View.ScrollView_UIScrollView.SetDragAmount(0f, 1f, false);
        }
    }

    #region 邮件列表


    //初始化邮件信息界面
    private void InitEmailView()
    {
        if (_emailViewCtl == null)       //打开邮件面板，展示详细信息
        {
            _emailViewCtl = AddChild<EmailViewController, EmailView>(
                gameObject, EmailView.NAME
                );
            _emailViewCtl.transform.localPosition = new Vector3(570, -2, 0);
            _disposable.Add(_emailViewCtl.OnCloseBtn_UIButtonClick.Subscribe(_ => _emailViewCtl.Hide()));
            _emailViewCtl.Hide();
        }
    }

    #region 点击回调 

    private PlayerMailDto _curSelectDto;
    private EmailViewController _emailViewCtl;
    private void OnItemCellSelect(EmailItemController ctl)
    {
        if (ctl == null) return;
        for (var i = 0; i < _itemCtlList.Count; i++)
        {
            var select = (_itemCtlList[i] == ctl);
            _itemCtlList[i].Select(@select);

            if (!select) continue;
            _curSelectDto = EmailDataMgr.DataMgr.SetAndGetCurMail(i);
            _emailViewCtl.Show();
            _emailViewCtl.SetData(_curSelectDto);
            _emailViewCtl.UpdateView(_emailData);
        }

        if (!_curSelectDto.read)
        {
            EmailNetMsg.ReadMailRequest(_curSelectDto.id);
        }
    }

    #endregion


    #region 选中下一封邮件

    public void AutoSelectnextEmail()
    {
        if (!_emailViewCtl.IsActive())
            return;

        var ctrl = _itemCtlList.TryGetValue(_emailData.CurMailIdx);
        OnItemCellSelect(ctrl);
    }


    #endregion

    #endregion

}
