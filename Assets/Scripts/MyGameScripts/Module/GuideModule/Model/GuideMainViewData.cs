// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : xjd
// Created  : 12/19/2017 11:25:11 AM
// **********************************************************************

using System.Collections.Generic;
using AppDto;

public interface IGuideMainViewData
{
    List<GuideInfoNotify> GuideInfoList { get; }
}

public sealed partial class GuideMainViewDataMgr
{
    public sealed partial class GuideMainViewData:IGuideMainViewData
    {

        public void InitData()
        {
        }

        public void Dispose()
        {

        }

        private List<GuideInfoNotify> _guideInfoList = new List<GuideInfoNotify>();
        public List<GuideInfoNotify> GuideInfoList
        {
            get
            {
                return _guideInfoList;
            }
        }

        public void UpdateData(GuideListDto dto)
        {
            if (dto == null || dto.info == null) return;
            _guideInfoList.Clear();
            _guideInfoList = dto.info;
        }

        public void SetGetId(int guideId)
        {
            if(_guideInfoList.Find(x=>x.guideId == guideId) != null)
            {
                _guideInfoList.Find(x => x.guideId == guideId).status = (int)GuideInfoNotify.GuideStatus.Finished;
            }
        }

        public void UpdateGuideInfo(GuideInfoNotify notify)
        {
            if (_guideInfoList.Find(x => x.guideId == notify.guideId) != null)
                _guideInfoList.ReplaceOrAdd(_guideInfoList.FindIndex(x => x.guideId == notify.guideId), notify);
            else
                _guideInfoList.Add(notify);
        }
    }
}
