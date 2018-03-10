// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : xjd
// Created  : 9/1/2017 2:37:22 PM
// **********************************************************************

using System.Collections.Generic;
using AppDto;

public interface IEngraveData
{
    IEnumerable<BagItemDto> GetMedallionDataList();
    IEnumerable<BagItemDto> GetRuneDataList();

    long CurSelMedallionId { get; }

    long CurSelRuneId { get; }

    int CurSelRuneIdForItemId { get; }

    bool isShowSaleBtn { get; set; }

    BagItemDto SelMedallionData { get; }
}

public sealed partial class EngraveDataMgr
{
    public sealed partial class EngraveData:IEngraveData
    {
        private IEnumerable<BagItemDto> _medallionDataList = new List<BagItemDto>();
        private IEnumerable<BagItemDto> _runeDataList = new List<BagItemDto>();
        private long _curSelMedallionId = 0;
        private long _curSelRuneId = 0;
        private int _curSelRunIdForItemId = 0;

        public void InitData()
        {
            UpdateMedallionData();
            UpdateRuneData();
        }

        public void UpdateData()
        {
            UpdateMedallionData();
            UpdateRuneData();
        }

        public void UpdateMedallionData()
        {
            _medallionDataList = BackpackDataMgr.DataMgr.GetMedallionItems();

            if (_medallionDataList.ToList().IsNullOrEmpty())
                _curSelMedallionId = 0;
            else if(_curSelMedallionId==0 || _medallionDataList.ToList().Find(item => item.uniqueId == _curSelMedallionId)==null)
                _curSelMedallionId = _medallionDataList.ToList()[0].uniqueId;

            isShowSaleBtn = _curSelMedallionId > 0;
        }

        public bool isShowSaleBtn { get; set; }

        public void UpdateRuneData()
        {
            _runeDataList = BackpackDataMgr.DataMgr.GetRuneItems();

            //铭刻符无唯一ID 默认为0 暂时设置为1~n标识该物体 方便列表显示、如果叠堆显示则不需要
            _runeDataList.ForEachI((item, index) =>
            {
                item.uniqueId = index + 1;
            });

            _curSelRuneId = _runeDataList.ToList().IsNullOrEmpty() ? 0 : _runeDataList.ToList()[0].uniqueId ;

            _curSelRunIdForItemId = _runeDataList.ToList().IsNullOrEmpty() ? 0 : _runeDataList.ToList()[0].itemId;
        }

        public IEnumerable<BagItemDto> GetMedallionDataList()
        {
            return BackpackDataMgr.DataMgr.GetMedallionItems();
        }
        public IEnumerable<BagItemDto> GetRuneDataList()
        {
            //铭刻符无唯一ID 默认为0 暂时设置为1~n标识该物体 方便列表显示、如果叠堆显示则不需要
            _runeDataList = BackpackDataMgr.DataMgr.GetRuneItems();
            _runeDataList.ForEachI((item, index) =>
            {
                item.uniqueId = index + 1;
            });

            return _runeDataList;
        }

        public long CurSelMedallionId
        {
            get { return _curSelMedallionId; }
            set { _curSelMedallionId = value; }
        }

        public long CurSelRuneId
        {
            get { return _curSelRuneId; }
            set
            {
                _curSelRuneId = value;

                GetRuneDataList().ForEach(item =>
                {
                    if (item.uniqueId == _curSelRuneId)
                        _curSelRunIdForItemId = item.itemId;
                });
            }
        }

        public int CurSelRuneIdForItemId
        {
            get { return _curSelRunIdForItemId; }
        }

        public BagItemDto SelMedallionData
        {
            get
            {
                return GetMedallionDataList().Find(x => x.uniqueId == _curSelMedallionId);
            }
        }

        public void Dispose()
        {

        }
    }
}
