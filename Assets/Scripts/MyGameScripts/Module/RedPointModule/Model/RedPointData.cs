// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : xjd
// Created  : 1/25/2018 5:23:19 PM
// **********************************************************************

using System.Collections.Generic;
using AppDto;

public enum RedPointTypeEnum : int
{
    UnknowType = 0,
    one = 1,
}

public interface IRedPointData
{
    RedPointDataMgr.RedPointSingleData PushSingleData { get; }
}

public sealed partial class RedPointDataMgr
{
    public class RedPointSingleData
    {
        readonly public int redPointType;
        public bool isShow;
        public bool isOpen;
        public int num;
        public object detailMsg;
        public List<int> curChildRenTypeList = new List<int>();

        public RedPointSingleData(int type, bool show, bool open=true, int n=0, object msg=null, List<int> list=null)
        {
            redPointType = type;
            isShow = show;
            isOpen = open;
            num = n;
            detailMsg = msg;
        }

        public void UpdateInfluenceList(int id, bool isTrue)
        {
            if (isTrue && !curChildRenTypeList.Contains(id))
                curChildRenTypeList.Add(id);
            else if(!isTrue && curChildRenTypeList.Contains(id))
                curChildRenTypeList.Remove(id);

            isShow = !curChildRenTypeList.IsNullOrEmpty();
            DataMgr._data.UpdateSingleData(redPointType, isShow);
        }
    }

    public sealed partial class RedPointData:IRedPointData
    {
        public void InitData()
        {
            _allStaticDataDic = DataCache.getDicByCls<RedPoint>();
            _allStaticDataDic.ForEach(itemData =>
            {
                _allDataDic.Add(itemData.Value.id, new RedPointSingleData(itemData.Value.id, false));
            });
        }

        public void Dispose()
        {

        }

        //替换读表的dto
        private Dictionary<int, RedPoint> _allStaticDataDic = new Dictionary<int, RedPoint>();
        private Dictionary<int, RedPointSingleData> _allDataDic = new Dictionary<int, RedPointSingleData>();
        private RedPointSingleData _pushSingleData = new RedPointSingleData(0, false);
        public RedPointSingleData PushSingleData { get { return _pushSingleData; } }

        public void UpdateSingleData(int type, bool isShow, int n=0)
        {
            if (!_allDataDic.ContainsKey(type)) return;

            //type的红点状态
            var singleData = _allDataDic.Find(x => x.Key == type);
            singleData.Value.isShow = isShow;
            singleData.Value.num = n;
            _pushSingleData = singleData.Value;
            //先firedata 当前红点的状态  因为之后牵连的红点状态还会firedata多次（次数依据红点影响深度 查看RedPoint表）
            FireData();

            //type影响的红点状态
            var singlStaticData = _allStaticDataDic.Find(x => x.Key == type);
            singlStaticData.Value.parent.ForEach(id =>
            {
                var influenceData = _allDataDic.Find(x => x.Key == id);
                influenceData.Value.UpdateInfluenceList(type, isShow);
            });
        }

        public RedPointSingleData GetRedPointData(int type)
        {
            if (!_allDataDic.ContainsKey(type))
                return null;
            return _allDataDic.Find(x => x.Key == type).Value;
        }
    }
}
