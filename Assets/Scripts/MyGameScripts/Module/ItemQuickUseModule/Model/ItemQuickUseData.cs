// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : xush
// Created  : 3/3/2018 3:30:06 PM
// **********************************************************************

using System.Collections.Generic;
using AppDto;

public interface IItemQuickUseData
{
    IEnumerable<BagItemDto> ItemDtoList { get; }
    BagItemDto CurItemDto { get; }
}

public sealed partial class ItemQuickUseDataMgr
{
    public sealed partial class ItemQuickUseData:IItemQuickUseData
    {
        private List<BagItemDto> _itemDtoList = new List<BagItemDto>();
        public IEnumerable<BagItemDto> ItemDtoList { get { return _itemDtoList; } }

        private BagItemDto _curItemDto;

        public BagItemDto CurItemDto
        {
            get { return _curItemDto; }
            set { _curItemDto = value; }
        }

        public void InitData()
        {
        }

        public void Dispose()
        {
            ClearItemList();
            _curItemDto = null;
        }

        public void AddItemInItemList(BagItemDto dto)
        {
            _itemDtoList.Add(dto);
            _curItemDto = _itemDtoList[0];
        }

        public void RemoveItemList(BagItemDto dto)
        {
            var idx = _itemDtoList.FindIndex(d => d.index == dto.index);
            if(idx >= 0)
                _itemDtoList.RemoveAt(idx);
            _curItemDto = _itemDtoList.Count > 0 ? _itemDtoList[0] : null;
        }

        public void ClearItemList()
        {
            _itemDtoList.Clear();
        }
    }
}
