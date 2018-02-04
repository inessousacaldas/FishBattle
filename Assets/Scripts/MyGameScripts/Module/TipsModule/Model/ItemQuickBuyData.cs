// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : xush
// Created  : 1/3/2018 3:27:06 PM
// **********************************************************************

using AppDto;

public interface IItemQuickBuyData
{
    ShopGoodsListDto ShopGoodsList { get; }
    int ItemId { get; }
}

public sealed partial class ItemQuickBuyDataMgr
{
    public sealed partial class ItemQuickBuyData:IItemQuickBuyData
    {
        private int _itemId;

        public int ItemId
        {
            get { return _itemId;}
            set { _itemId = value; }
        }

        private ShopGoodsListDto _dto;
        public ShopGoodsListDto ShopGoodsList
        {
            get { return _dto; }
            set { _dto = value; }
        }

        public void InitData()
        {
        }

        public void Dispose()
        {

        }
    }
}
