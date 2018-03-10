// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : Zijian
// Created  : 8/17/2017 2:13:45 PM
// **********************************************************************
using System.Collections.Generic;
using System;
using System.Linq;
using AppDto;
using UnityEngine;

public interface IShopData
{
    int CurShopTypeTab { get;} //当前选择的ShopType
    Dictionary<int, List<ITabInfo>> ShopTypeIdMap { get; } 
    ShopItemVo CurSelectShopVo { get; } //当前选中
    int CurSelectCount { get; } //当前选择的数量
    int CurTotalPrice { get; }// 当前总价
    int CurSelectRemainCount { get; } //当前选择物品剩余数量
    long OwenerMoney { get; } 
    ShopInfoData CurShopInfo { get; } //当前的商店信息

    ShopMainType CurShopMainType { get; } //是否游戏商城
    int GetCurShopId { get; }
    int SelectGoodsId { get; set; }
}

public class ShopInfoData
{
    ShopInfoDto dto;
    List<ShopItemVo> vos = new List<ShopItemVo>();
    private Shop shopInfo;
    public Shop ShopInfo
    {
        get { return shopInfo; }
    }
    public int ShopId
    {
        get
        {
            return ShopInfo.id;
        }
    }

    public Shop.ResetRule ResetRule
    {
        get { return (Shop.ResetRule)ShopInfo.resetRule; }
    }

    public long RefreshTime_Raw
    {
        get { return dto == null ? 0 : dto.updateTime; }
    }

    public long RemainTime
    {
        get
        {
            var serverDateTime = SystemTimeManager.Instance.GetUTCTimeStamp();
            var timespan = RefreshTime_Raw - serverDateTime;
            return timespan;
        }
    }

    public string FormatRemainTime
    {
        get
        {
            return DateUtil.GetDayHourMinuteSecond(RemainTime / 1000);
        }
    }

    public IEnumerable<ShopItemVo> ShopItems
    {
        get { return vos; }
    }

    public string ResetItemIcon
    {
        get
        {
            var item = ItemHelper.GetGeneralItemByItemId(ShopInfo.resetItemId);
            return item.icon;
        }
    }

    public int ResetItemCount
    {
        get
        {
            return ShopInfo.resetItemCount;
        }
    }

    public ShopInfoData(Shop shop) {
        vos.Clear();
        this.shopInfo = shop;
        var AllShopGoods = DataCache.getArrayByCls<ShopGoods>();
        var resList = AllShopGoods.Filter(x => x.shopId == shop.id);
        resList.ForEach(x => vos.Add(new ShopItemVo(x)));
    }
    public ShopInfoData(ShopInfoDto dto)
    {
        this.shopInfo = dto.shop;
        UpdateShopInfo(dto);
    }
    public void UpdateShopInfo(ShopInfoDto dto)
    {
        this.dto = dto;
        this.shopInfo = dto.shop;
        vos.Clear();
        dto.shopGoodsDtoList.ForEach(x=> {
            UpdateShopGoodDto(x);
        });
        //引入排序
        //vos.Sort();
    }
    public void UpdateShopGoodDto(ShopGoodsDto dto)
    {
        var vo = vos.Find(v => v.Id == dto.goodsId);
        if (vo == null)
            vos.ReplaceOrAdd(v => v.Id == dto.goodsId, new ShopItemVo(dto));
        else
            vo.UpdateDto(dto);
    }
}

public class ShopItemVo 
{
    private ShopGoodsDto dto;
    private ShopGoods shopGoods;
    public ShopGoods ShopGood { get { return shopGoods; } }
    public int ExpendItemId
    {
        get
        {
            return ShopGood.expendItemId;
        }
    }
    public string ExpendItemIcon
    {
        get
        {
            var item = ItemHelper.GetGeneralItemByItemId(ExpendItemId);
            return item.icon;
        }
    }

    public int DiscountPrice
    {
        get
        {
            return dto == null?ShopGood.originalPrice : dto.buyPrice;
        }
    }
    public int Discount
    {
        get
        {
            return 0;
        }
    }
    public ExpandType ExpendType
    {
        get
        {
            return (ExpandType)ShopGood.lableId;
        }
    }

    public string Icon
    {
        get
        {
            var item = ItemHelper.GetGeneralItemByItemId(ShopGood.itemId);
            
            return item==null ? "": item.icon;
        }
    }
    public int quality
    {
        get
        {
            var item = ItemHelper.GetGeneralItemByItemId<AppItem>(ShopGood.itemId);

            return item == null ? 0 : item.quality;
        }
    }
    public int Id
    {
        get
        {
            return ShopGood.id;
        }
    }

    public string Name
    {
        get
        {
            return ShopGood.goodsName;
        }
    }

    public int Price
    {
        get
        {
            return ShopGood.originalPrice;
        }
    }

    public int RemainNumber
    {
        get
        {
            if (dto == null)
                return -1;
            if (dto.limitNum == 0)
                return -1;
            return dto.limitNum - dto.num;
        }
    }

    public string Des
    {
        get
        {
            var item = ItemHelper.GetGeneralItemByItemId(ShopGood.itemId);
            var prop = item as Props;
            string des = string.Empty;
            if (prop != null)
                des = prop.description;
            return string.IsNullOrEmpty(ShopGood.discribe) ? des: ShopGood.discribe;
        }
    }
    public ShopItemVo() { }

    
    public ShopItemVo(ShopGoods goods) {
        this.dto = null;
        this.shopGoods = goods;
    }
    public ShopItemVo(ShopGoodsDto dto)
    {
        this.dto = dto;
        this.shopGoods = dto.goods;
    }
    public void UpdateDto(ShopGoodsDto dto)
    {
        this.dto = dto;
        this.shopGoods = dto.goods;
    }
}
//对应Shop / 里面的shopType
//现在只有商城用来
public enum ShopTypeTab :int
{
    None = -1,
    LimitShop, //限购
    BinnianmondShop,//绑钻
    ScoreShop,//积分
    ArenaShop = 8,//竞技
    GuildShop = 9,//公会商店
    LimitShopId = 101,  //限购商城id
    BinnianmondShopId = 102,//绑钻商城id
    ScroeShopId = 103,//积分商城id
    ArenaScroeShopId = 107,//竞技商城id
    GuildShopId = 108,  //公会商店
}

//物品的出售状态
public enum ExpandType
{
    None = 0,
    New = 1,//
    Hot,
    Discount,//折扣
}

public enum ShopMainType
{
    Shop,//游戏商城
    SystemShop, //系统商城
}

public sealed partial class ShopDataMgr
{
    public sealed partial class ShopData:IShopData
    {
        public static List<ITabInfo> tabInfoList =new List<ITabInfo>();
        // ShopType 对应的Tab信息
        private Dictionary<int, List<ITabInfo>> _ShopTypeIdMap = new Dictionary<int, List<ITabInfo>>();
        private int curTab;
        private int curShopId;
        private ShopItemVo curSelectShopvo;
        private int curSelectCount = 1;
        private int _selectGoodsId = -1;

        public int CurSelectCount
        {
            get { return curSelectCount; }
            set { curSelectCount = value; }
        }

        private IPlayerModel playerInfo;
        Dictionary<int,Dictionary<int, ShopInfoData>> allShopDic = new Dictionary<int, Dictionary<int, ShopInfoData>>();

        public ShopInfoData CurShopInfo {
            get {
                if (curShopId == 0)
                    return null;
                if (!allShopDic.ContainsKey((int)CurShopTypeTab))
                    return null;
                if (!allShopDic[(int)CurShopTypeTab].ContainsKey(curShopId))
                    return null;
                return allShopDic[(int)CurShopTypeTab][curShopId];
            } }

        public int CurShopTypeTab
        {
            get
            {
                return curTab;
            }
        }
        public void SetCurShopTab(int tab)
        {
            curTab = tab;
        }
        public void SetCurShopId(int tab)
        {
            curShopId = tab;
        }

        public int SelectGoodsId
        {
            get { return _selectGoodsId; }
            set { _selectGoodsId = value; }
        }

        public int GetCurShopId { get { return curShopId; } }
       
        public Dictionary<int, List<ITabInfo>> ShopTypeIdMap
        {
            get
            {
                return _ShopTypeIdMap;
            }
        }

        public ShopItemVo CurSelectShopVo
        {
            get
            {
                if (curSelectShopvo == null)
                {
                    GameDebuger.LogError("Shop : CurSeectShopVo = null");
                }
                return curSelectShopvo;
            }
        }

        public void SetCurSelectShopVo(ShopItemVo vo)
        {
            var sameItem = curSelectShopvo == vo;
            curSelectShopvo = vo;
            if (vo == null)
                curSelectCount = 0;
            else if(!sameItem)
                curSelectCount = 1;
            else
            {
                if (curSelectCount + 1 <= CurSelectShopVo.RemainNumber
                    || CurSelectShopVo.RemainNumber < 0)
                    curSelectCount += 1;
                else
                {
                    if(sameItem)
                        TipManager.AddTip("超过物品数量上限~");
                }
            }
        }

        public int CurTotalPrice
        {
            get
            {
                if (CurSelectShopVo == null)
                    return 0;
                return CurSelectCount * CurSelectShopVo.DiscountPrice;
            }
        }
        public int CurSelectRemainCount
        {
            get
            {
                if (CurSelectShopVo == null)
                    return 0;
                return CurSelectShopVo.RemainNumber;
            }
        }

        public long OwenerMoney
        {
            get
            {
                if (CurSelectShopVo == null)
                    return 0;
                var money = playerInfo.GetPlayerWealth((AppDto.AppVirtualItem.VirtualItemEnum)CurSelectShopVo.ExpendItemId);
                return money;
            }
        }

        public ShopMainType CurShopMainType { get; set; }

        public void SetCurPlayerModel(IPlayerModel playerModel)
        {
            this.playerInfo = playerModel;
        }
        public void InitData()
        {

        }
        public void ClearData()
        {
            //GameDebuger.Log("ShopData Dispose");
            _ShopTypeIdMap.Clear();
            allShopDic.Clear();
            tabInfoList.Clear();
            curSelectShopvo = null;
            curTab = 0;
            curShopId = 0;
        }
        public void InitShopData()
        {
            allShopDic.Clear();
            ShopTypeIdMap.Clear();
            var shopTypes = DataCache.getArrayByCls<ShopType>();
            shopTypes.ForEach(x =>
            {
                tabInfoList.Add(TabInfoData.Create(x.id, x.name));
                GetShopIdTabInfo(x.id);
                allShopDic.Add(x.id, new Dictionary<int, ShopInfoData>());
            });
        }
        
        private void GetShopIdTabInfo(int shopType)
        {
            var shop_dic = DataCache.getDicByCls<Shop>();
            var tabInfos = new List<ITabInfo>();
            if (shop_dic.Count > 0)
                shop_dic.Values.Where(x => x.shopType == shopType).ForEach(x =>
                {
                    tabInfos.Add(TabInfoData.Create(x.id, x.name));
                });
            
            ShopTypeIdMap.Add(shopType, tabInfos);
        }
      
        #region 其他商城
        public void UpdateSystemShopData(int shopType,int selectShopId = -1,int selectCount = 1 )
        {
            ClearData();
            CurShopMainType = ShopMainType.SystemShop;
            var shopList = DataCache.getArrayByCls<Shop>();
            var resList = shopList.Filter(x => x.shopType == shopType);
            allShopDic.Clear();
            allShopDic.Add(shopType, new Dictionary<int, ShopInfoData>());
            resList.ForEach(x =>
            {
                allShopDic[shopType].Add(x.id, new ShopInfoData(x));
            });
            GetShopIdTabInfo(shopType);
            if (CurShopInfo == null)
            {
                SetCurShopTab(shopType);
                SetCurShopId(ShopTypeIdMap[shopType][0].EnumValue);
            }

            SetCurSelectShopVo(CurShopInfo.ShopItems.FirstOrDefault());
            ShopItemVo goodItem = CurShopInfo.ShopItems.FirstOrDefault();
            if (selectShopId != -1)
            {
                //注意默认选中的有没有分页哦！
                goodItem = CurShopInfo.ShopItems.Find(x => x.Id == selectShopId);
                if (goodItem == null)
                    goodItem = CurShopInfo.ShopItems.FirstOrDefault();
            }
            SetCurSelectShopVo(goodItem);
            curSelectCount = selectCount;
        }
        #endregion

        #region NetUpdate  变成商城部分的方法了
        public void UpdateAllShop(AllShopInfoDto dto,bool isClear = true)
        {
            ClearData();
            InitShopData();
            CurShopMainType = ShopMainType.Shop;

            //根据Type分组~
            var allShopInfoDic = dto.shopInfoDtoList.GroupBy(x => x.shop.shopType);
            foreach(var type in allShopInfoDic)
            {
                int shopType = type.Key;
                if (!allShopDic.ContainsKey(shopType))
                    allShopDic.Add(shopType, new Dictionary<int, ShopInfoData>());
                    
                foreach (var shop in type)
                {
                    int shopId = shop.shopId;
                    if(!allShopDic[shopType].ContainsKey(shopId))
                        allShopDic[shopType].AddOrReplace(shopId, new ShopInfoData(shop) ); 
                    else
                        allShopDic[shopType][shopId].UpdateShopInfo(shop);
                }   
            }
        }

        public void UpdateSingleShop(ShopInfoDto dto)
        {
            int shopType = dto.shop.shopType;
            int shopId = dto.shopId;
            if(allShopDic.ContainsKey(shopType) && allShopDic[shopType].ContainsKey(shopId))
                allShopDic[shopType][shopId].UpdateShopInfo(dto);
            SetCurSelectShopVo(CurShopInfo.ShopItems.First());
        }

        public void UpdateSingleShopGood(int shopType,ShopGoodsDto dto)
        {
            if(allShopDic.ContainsKey(shopType) && allShopDic[shopType].ContainsKey(dto.goods.shopId))
            {
                allShopDic[shopType][dto.goods.shopId].UpdateShopGoodDto(dto);
            }
        }
        #endregion
        
    }
}
