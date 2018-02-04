// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  EmailItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using AppDto;

public partial class EmailItemController
{
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
 
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        EventDelegate.Set(View.Bg_UIButton.onClick, OnClick);
    }

    protected override void OnDispose()
    {

    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        EventDelegate.Remove(View.Bg_UIButton.onClick, OnClick);
    }

    //用特定接口类型数据刷新界面
    public void UpdateView(IEmailData data)
    {

    }


    private Action<EmailItemController> OnSelect;
    private long ID;


    public void Init()
    {

    }

    public void UpdateView(PlayerMailDto dto, Action<EmailItemController> onSelect = null)
    {
        ID = dto.id;
        OnSelect = onSelect;
        Select(false);

        View.EmailTitleLabel_UILabel.text = dto.title;
        View.EmailIconNotRead_UISprite.enabled = !dto.read;
        View.NotRead_UISprite.enabled = !dto.read;
        View.EmailIconReaded_UISprite.enabled = dto.read;
        View.Readed_UISprite.enabled = dto.read;
        View.RedPot_UISprite.gameObject.SetActive(!dto.read);
        View.ItemBg_UISprite.gameObject.SetActive(dto.hasAttachments);

        if (dto.mailType.saveDate == 0)
        {
            View.EmailTimeLabel_UILabel.text = "时效：永久";
        }
        else
        {
            long outTime = DateUtil.DateTimeToUnixTimestamp((DateUtil.UnixTimeStampToDateTime(dto.sendTime)).AddDays(dto.mailType.saveDate));
            View.EmailTimeLabel_UILabel.text = DateUtil.GetDateStr(outTime);
        }
    }

    public void Select(bool b)
    {
        View.ChoseBg_UISprite.enabled = b;
        View.Bg_UISprite.enabled = !b;
    }

    private void OnClick()
    {
        if ( OnSelect != null)
            OnSelect(this);
    }

}
