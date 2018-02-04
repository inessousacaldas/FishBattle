// **********************************************************************
// Copyright (c) 2015 cilugame. All rights reserved.
// File     : AnnouncementDataManager.cs
// Author   : senkay <senkay@126.com>
// Created  : 10/29/2015 
// Porpuse  : 
// **********************************************************************
//

using System.Collections.Generic;

public class AnnouncementDataManager
{
	private static readonly AnnouncementDataManager instance = new AnnouncementDataManager();
	public static AnnouncementDataManager Instance
	{
		get
		{
			return instance;
		}
	}

	private AnnouncementInfoList _announcementInfoList;

	public const string Type_AnnouncementData = "gonggaoData.txt"; //oldType announcementData.txt


	public void CheckUpdate()
	{
		string oldVersion = GameStaticConfigManager.Instance.GetLocalStaticDataVer(Type_AnnouncementData);

        GameStaticConfigManager.Instance.LoadStaticConfig(Type_AnnouncementData, delegate (string str) {
            _announcementInfoList = JsHelper.ToObject<AnnouncementInfoList>(str);

            if (_announcementInfoList != null)
            {
                if (_announcementInfoList.version != oldVersion)
                {
                    ProxyLoginModule.OpenAnnouncement();
                }
            }
        }, tips =>
        {
            TipManager.AddTip(tips);
        });
	}

	public List<AnnouncementInfo> GetAnnouncementInfoList()
	{
		if (_announcementInfoList != null)
		{
			return _announcementInfoList.infoList;
		}
		else
		{
			return null;
		}
	}
}

