// **********************************************************************
// Copyright (c) 2015 cilugame. All rights reserved.
// File     : AnnouncementController.cs
// Author   : senkay <senkay@126.com>
// Created  : 05/25/2015 
// Porpuse  : 
// **********************************************************************
//
using UnityEngine;
using System.Collections.Generic;

public class AnnouncementController : MonoViewController<AnnouncementView>
{

    private List<AnnouncementInfo> _infoList;


    #region IViewController

    /// <summary>
    /// 从DataModel中取得相关数据对界面进行初始化
    /// </summary>

    /// <summary>
    /// Registers the event.
    /// DateModel中的监听和界面控件的事件绑定,这个方法将在InitView中调用
    /// </summary>
    protected override void RegistCustomEvent ()
    {
        EventDelegate.Set(View.OkButton_UIButton.onClick, OnCloseButtonClick);
        EventDelegate.Set(View.CloseButton_UIButton.onClick, OnCloseButtonClick);
        EventDelegate.Set(View.dragRegion_UIEventTrigger.onClick, OnContentClick);
    }

    private void OnContentClick()
    {
        for (int i = 0; i < _contentUiLabels.Count; i++)
        {
            string urlStr = _contentUiLabels[i].GetUrlAtPosition(UICamera.lastWorldPosition);
            if (!string.IsNullOrEmpty(urlStr))
            {
                GameDebuger.TODO("ModelManager.Chat.DecodeUrlMsg(urlStr, null);");
                break;
            }
        }
       
    }


    #endregion

    public void Open()
    {
        _infoList = AnnouncementDataManager.Instance.GetAnnouncementInfoList();

        View.Table_UITable.gameObject.RemoveChildren();

        if (_infoList != null)
        {
            for (int i = 0; i < _infoList.Count; i++)
            {
                AnnouncementInfo info = _infoList[i];
                ShowAnnouncementInfo(info);
            }
        }

        View.Table_UITable.Reposition();
    }

    private AnnouncementInfo GenerateAnnouncementInfo(string title, string content)
    {
        AnnouncementInfo info = new AnnouncementInfo();
        info.title = title;
        info.content = content;
        return info;
    }

    private List<UILabel> _contentUiLabels = new List<UILabel>();

    private void ShowAnnouncementInfo(AnnouncementInfo info)
    {
        GameObject announcementTitleCell = AddCachedChild(View.Table_UITable.gameObject, "AnnouncementTitleCell");
        GameObject announcementContentCell = AddCachedChild(View.Table_UITable.gameObject, "AnnouncementContentCell");

        announcementTitleCell.GetComponentInChildren<UILabel>().text = info.title;
        UILabel lbl = announcementContentCell.GetComponentInChildren<UILabel>();
        lbl.text = info.content;
        _contentUiLabels.Add(lbl);
    }

    private void OnCloseButtonClick()
    {
        TalkingDataHelper.OnEventSetp("Announcement", "Close");
        ProxyLoginModule.CloseAnnouncement();
    }
}

