// Author: fish
using System;
using System.Collections.Generic;
using AppDto;
using BagEnum = AppDto.AppItem.BagEnum;
using  CirculationType = AppDto.BagItemDto.CirculationType;

public class tempdto  // todo fish
{
    public int id;
    public int num;
}

public enum CompositeTabType
{
    Composite  // 合成
    , DeComposite // 分解
}
//分解物品类型
public enum DecompositeTabType
{
    Material   //材料
    ,Equip     //装备
    , Circuit   //回路
}
public interface ICompositeItem
{
    AppItem Item { get; }
    int Cnt { get; set; }
    int CirType { get; }
    int ItemID { get; }
}

public interface IDeCompositeItem
{
    AppItem Item { get; }
    int Cnt { get; set; }
}
//合成
public class CompositeItem : ICompositeItem
{
    private AppItem item;

    public AppItem Item
    {
        get { return item; }
    }

    public int Cnt
    {
        get { return cnt; }
        set { cnt = value; }
    }

    private int cnt;

    private int cirType;
    public int CirType {
        get { return cirType;}
    }

    public int ItemID {
        get { return item == null ? 0 : item.id; }
    }

    public static CompositeItem Create(AppItem item, int cnt, int cir = -1)
    {
        var tmp = new CompositeItem();
        tmp.item = item;
        tmp.cnt = cnt;
        tmp.cirType = cir;
        return tmp;
    }

}
//分解
public class DeCompositeItem : IDeCompositeItem
{
    private AppItem item;
    public AppItem Item
    {
        get { return item; }
    }

    private int cnt;
    public int Cnt {
        get { return cnt; }
        set { cnt = value; }
    }

    public static DeCompositeItem Create(AppItem items, int cnt, int cir = -1)
    {
        var tmp = new DeCompositeItem();
        tmp.item = items;
        tmp.cnt = cnt;
        return tmp;
    }
}


//分解合成统一用
public interface ICompositViewData
{
    CompositeTabType CurCompositTab { get; }
    // 合成分页
    IEnumerable<BagItemDto> GetCompositeItems(out int len);
    
    bool IsConvinienceComposite { get; }
    int CompositeTimes { get; } // 设定合成次数
    ICompositeItem CurMaterialItem { get; }
    ICompositeItem OutcomeItem { get; }

    int CompositeSelIdx { get; }

    //分解分页
    DecompositeTabType CurDecomposeTab { get; }

    IEnumerable<BagItemDto> GetDeComposeItems(
        DecompositeTabType tab
        , out int len);
    IDeCompositeItem CurDecomposeItem { get; }
    List<GeneralItem> DecAcquireItems { get; }         //分解所得物品类型
    List<ResolveGainDto> GainDtoList { get; }          //分解后服务器返回的获得物品及数量
    int DecomposeSelIdx { get; }
    int DecomposeTimes { get; }

    List<ITabInfo> Composite_itemsPageTab { get; }
}
//仓库
public interface IWareHouseViewData{
    BagDto WareHouseDto { get; }
    /// <summary>
    /// 仓库的名字集
    /// </summary>
    IEnumerable<string> wareHouseNameStrSet { get; }
    int CurWareHousePage { get; }
    int SelectWarehouseItemIdx { get; }
    int WareHousePageNum { get; }
    int WareHouseBagCapability { get; }
    int BagPageNum { get; }
    int ItemBagCapability { get; }
    IEnumerable<BagItemDto> GetBagItems();
    IEnumerable<BagItemDto> GetWarehouseItems();

    DateTime lastSortBagTime { get; }
}

public interface IMainUIViewData{
    bool IsTempPackNotNull{ get;}    
}
//临时背包
public interface ITempBackpackViewData
{
    int CurTempPageNum { get; }
    int PageNum { get; }
    IEnumerable<BagItemDto> GetTempBagItems();
    IEnumerable<BagItemDto> GetBagItems();
    int ItemBagCapability { get; }
    int expandConsumeNum { get; }
    int BagItemsCnt { get; }
}
//背包
public interface IBackpackViewData
{
    BackpackViewTab CurTab { get; } //仓库/背包
  
    int CurPageNum { get; } //当前页数
    int PageNum { get; } //总页数
    int BagItemsCnt { get; } //背包物品的数量
    int TaskItemsCnt { get; } //任务物品的数量
    int PetItemsCnt { get; } //伙伴物品的数量
    IEnumerable<BagItemDto> GetBagItems();
    IEnumerable<CrewChipDto> GetCrewChips();
    IEnumerable<ItemDto> GetMissionItem();
    int ItemBagCapability { get; } //背包的容量
    int expandConsumeNum { get; } //扩展背包需要的数量
    ItemTypeTab CurBagTab { get; }
    int CurBagTabIndex { get; }  
    IEnumerable<ITabInfo> CurBagTabInfos { get; }
    int CurBackPackSelectIdx { get; } //背包当前选中的Idx 

    /// <summary>
    /// 是否展示整理按钮
    /// </summary>
    bool isShowSortBtn { get; } //是否展示整理按钮
    bool isShowComposit { get; } // 是否展示合成按钮
    bool isShowDeComposit { get; } // 是否展示分解按钮
    /// <summary>
    /// 是否能进行整理
    /// </summary>
    DateTime lastSortBagTime { get; }


    IEnumerable<EquipmentDto> equipmentCellsData { get; }
}

public interface IBackpackData
{
    IBackpackViewData BackpackViewData { get; }
    IWareHouseViewData WareHouseViewData { get; }
    ITempBackpackViewData TempBackpackViewData { get; }
    IMainUIViewData MainUIViewData{ get;}
    ICompositViewData CompositeViewData { get;}
}

public enum BackpackViewTab
{
    Backpack
    , Warehouse
}

public enum ItemTypeTab
{
    Item  // 物品
    , Task // 任务
    , Pet  //武将
}

public class EnumEqualityComparer<T> : IEqualityComparer<T> where T:struct
{
    public bool Equals(T x, T y)
    {
        return x.Equals(y);
    }

    public int GetHashCode(T type)
    {
        return type.GetHashCode();throw new NotImplementedException();
    }
}

public sealed partial class BackpackDataMgr
{
    public sealed partial class BackpackData
        : IBackpackData
        , IBackpackViewData
        , ITempBackpackViewData
        , IMainUIViewData
        , IWareHouseViewData
        , ICompositViewData
    {
        #region ServerData

        public Dictionary<BagEnum, BagDto> _dtoDic = new Dictionary<BagEnum, BagDto>(new EnumEqualityComparer<BagEnum>());
// 已删除可以被找回的物品不再以index为负数的形式存在背包里
        public BagDto backPackDto{get {return GetDtoByBagID(BagEnum.Backpack);}}

        public BagDto tempDto {get {return GetDtoByBagID(BagEnum.Temppack);}}

        public BagDto wareHousePackDto{get {return GetDtoByBagID(BagEnum.Warehouse);}}
        public List<string> _wareHouseNameStrSet;

        public List<CrewChipDto> _crewChips = new List<CrewChipDto>();
        public List<ItemDto> _missionItem = new List<ItemDto>();

        #endregion ServerData

        #region ClientData

        public List<BagItemDto> AddedBagItems = new List<BagItemDto>();
        public List<BagItemDto> AddedTemBagItems = new List<BagItemDto>();
        public List<ItemDto> AddedTaskItems = new List<ItemDto>();
        public List<tempdto> AddedPetItems = new List<tempdto>();

        private IEnumerable<TradeGoods> _tradeGoods = new List<TradeGoods>(); 

        // main view
        private ItemTypeTab curBagTab;
        public ItemTypeTab CurBagTab {
            get { return curBagTab; }
            set
            {
                curBagTab = value;
            }
        }
        

        public BackpackViewTab curTab = BackpackViewTab.Backpack;
        public int itemPageNum = 0;
        public int taskPageNum = 0;
        public int petPageNum = 0;

        public int CurBackPackSelectIdx {
            get { 
                return curBackPackSelectIdx; 
            }
            set { 
                curBackPackSelectIdx = value; 
            }
        }

        private int curBackPackSelectIdx = -1;

// 临时包裹
        public int curTempPageNum = 0;

        // 仓库
        public BagDto WareHouseDto {
            get { return wareHousePackDto; }
        }
        public IEnumerable<string> wareHouseNameStrSet {
            get { return _wareHouseNameStrSet; }
        }

        private int _curWareHousePage = 0;
        public int CurWareHousePage
        {
            get { 
                return _curWareHousePage; 
            }
            set { 
                _curWareHousePage = value; 
            }
        }

        private int _selectWarehouseIdx = -1;

        public int SelectWarehouseItemIdx
        {
            get { return _selectWarehouseIdx; }
            set {_selectWarehouseIdx = value; }
        }

        private BagDto GetDtoByBagID(BagEnum bagID){
            BagDto dto = null;
            _dtoDic.TryGetValue(bagID, out dto);
            return dto;
        }

        private int _wareHousePageNum;
        public int WareHousePageNum {
            get
            {
                int curPageNum = wareHousePackDto == null
                    ? 0
                    : TransCapabilityToPageNum(wareHousePackDto.capability, wareHousePackDto.maxCapability);
                if (curPageNum > _wareHousePageNum)
                {
                    _wareHouseNameStrSet.Add("仓库" + curPageNum);
                    _wareHousePageNum = curPageNum;
                }
                return _wareHousePageNum;
            }
        }

        public int WarehouseItemsCnt {
            get { return wareHousePackDto.items.TryGetLength(); }
        }

        public int WareHouseBagCapability {
            get { return wareHousePackDto.capability; }
        }

        private int _bagPageNum;
        public int BagPageNum {
            get
            {
                int curPageNum = backPackDto == null ? 0 : TransCapabilityToPageNum(backPackDto.capability, backPackDto.maxCapability);
                if(curPageNum > _bagPageNum)
                {
                    _bagPageNum = curPageNum;
                }
                return _bagPageNum;
            }
        }

        // 合成分页
        private CompositeTabType curCompositTab;
        public bool isConvinienceComposite ;
        public int compositeTimes;
        private int compositeSelIdx;
        public int CompositeSelIdx {
            get { return compositeSelIdx; }
            set { compositeSelIdx = value; }
        }

        public bool IsShowComposeTips = true;   // 记录合成物品提示的显示，本次登录有效
        // 分解分页
        private DecompositeTabType curDecomposeTab = DecompositeTabType.Material;
        
        
        private int decomposeTimes;   //分解次数
        private int decomposeSelIdx;
        public int DecomposeSelIdx
        {
            get { return decomposeSelIdx; }
            set
            {
                decomposeSelIdx = value;
            }
        }

        #endregion

        #region StaticData

        public static readonly ITabInfo[] ItemTabNameSet =
        {
            TabInfoData.Create((int)ItemTypeTab.Item, "物品")
            , TabInfoData.Create((int)ItemTypeTab.Task, "任务")
            , TabInfoData.Create((int)ItemTypeTab.Pet, "伙伴")
        } ;

        #endregion StaticData


        public BackpackData()
        {

        }

        public void Dispose()
        {
            AddedBagItems.Clear();
            AddedTemBagItems.Clear();
            AddedTaskItems.Clear();
            AddedPetItems.Clear();
        }


        public IBackpackViewData BackpackViewData {
            get { return this; }
        }

        public ITempBackpackViewData TempBackpackViewData {
            get { return this; }
        }

        public IMainUIViewData MainUIViewData {
            get { return this;}
        }

        public IWareHouseViewData WareHouseViewData
        {
            get { return this; }
        }

        public ICompositViewData CompositeViewData {
            get { return this; }
        }

        public void UpdateAddedItems(BagItemNotify noti){
            if (noti == null)
                return;
            List<BagItemDto> set = null;
            List<BagItemDto> src = null;
            if (noti.bagId == (int) AppItem.BagEnum.Backpack)
            {
                set = AddedBagItems;
                src = backPackDto.items;
            }
            else if (noti.bagId == (int) AppItem.BagEnum.Temppack)
            {
                set = AddedTemBagItems;
                src = tempDto.items;
            }
            if (set != null && src == null)
            {
                noti.deleteItems.ForEach(i =>
                {
                    var item = src.TryGetValue(i);
                    if (item != null)
                        set.RemoveItem(item);
                });

                noti.updateItems.ForEach(item =>
                {
                    var _item = src.Find(s => s.index == item.index);
                    if (_item == null ||item.count > _item.count)
                        noti.updateItems.AddIfNotExist(item);
                });
            }
        }

        #region MainViewRegion

        public BackpackViewTab CurTab {
            get { return curTab; }
        }

        public int CurBagTabIndex {
            get
            {
                return ItemTabNameSet.FindElementIdx(s => s.EnumValue == (int) curBagTab);
            }
        }

        public int CurPageNum {
            get
            {
                switch (curBagTab)
                {
                    case ItemTypeTab.Item:
                        return itemPageNum;
                    case ItemTypeTab.Task:
                        return taskPageNum;
                    case ItemTypeTab.Pet:
                        return petPageNum;
                    default:
                        return 0;
                }
            }
            set
            {
                switch (curBagTab)
                {
                    case ItemTypeTab.Item:
                        itemPageNum = value;
                        break;
                        ;
                    case ItemTypeTab.Task:
                        taskPageNum = value;
                        break;
                    case ItemTypeTab.Pet:
                        petPageNum = value;
                        break;
                }
            }
        }

        public int CurTempPageNum {
            get { return curTempPageNum; }
        }

        public int PageNum {
            get
            {
                switch (curBagTab)
                {
                    case ItemTypeTab.Item:
                        return TransCapabilityToPageNum(backPackDto.capability, backPackDto.maxCapability);
                    case ItemTypeTab.Task:
                        return (_missionItem.TryGetLength() + ItemsContainerConst.PageCapability)/ItemsContainerConst.PageCapability;
                    case ItemTypeTab.Pet:
                        return (_crewChips.TryGetLength() +  + ItemsContainerConst.PageCapability)/ItemsContainerConst.PageCapability;
                    default:
                        return 1;
                }
            }
        }

        public int BagItemsCnt {
            get { return backPackDto.items.TryGetLength(); }
        }
        public int TaskItemsCnt {
            get { return _missionItem.TryGetLength(); }
        }
        public int PetItemsCnt {
            get { return _crewChips.TryGetLength(); }
        }

        public IEnumerable<BagItemDto> GetTempBagItems()
        {
            return tempDto == null ? null : tempDto.items;
        }

        public BagItemDto GetTempBagItemByIndex(int index)
        {
            return tempDto == null ? null: tempDto.items.Find(x=>x.index == index);
        }

        public IEnumerable<CrewChipDto> GetCrewChips()
        {
            return _crewChips;
        }

        public IEnumerable<ItemDto> GetMissionItem() {
            return _missionItem;
        }

        public void UpdateCrewChips(IEnumerable<CrewChipDto> chipDtos)
        {
            chipDtos.ForEach(dto =>
            {
                if (dto.chipAmount == 0)
                {
                    var index = _crewChips.FindIndex(d => d.chipId == dto.chipId);
                    if(index >= 0)
                        _crewChips.RemoveAt(index);
                }
                else
                    _crewChips.ReplaceOrAdd(d => d.chipId == dto.chipId, dto);
            });
        }

        public void UpdateMissionItem(IEnumerable<ItemDto> taskDtos)
        {
            taskDtos.ForEach(dto =>
            {
                _missionItem.ReplaceOrAdd(d => d.itemId == dto.itemId,dto);
            });
            List<int> tRemoMission = new List<int>();
            _missionItem.ForEach(dto => {
                if(dto.count <= 0)
                {
                    _missionItem.RemoveItem(dto);
                }
            });
        }

        public IEnumerable<BagItemDto> GetBagItems()
        {
            return backPackDto == null ? null : backPackDto.items; 
        }

        public IEnumerable<BagItemDto> GetQuartzItems()
        {
            if (backPackDto == null || backPackDto.items.Count == 0)
                return null;
            var list = backPackDto.items.Filter(s => s != null && s.extra != null && s.extra is QuartzExtraDto);
            return list;
        }

        public IEnumerable<BagItemDto> GetTradeItems()
        {
            if (backPackDto == null || backPackDto.items.Count == 0)
                return null;

            List<BagItemDto> list = new List<BagItemDto>();
            backPackDto.items.ForEach(d =>
            {
                var item = _tradeGoods.Find(goods => goods.id == d.itemId);
                var tradeGoods = DataCache.getDtoByCls<TradeGoods>(d.itemId);
                if (item != null 
                    && tradeGoods != null 
                    && tradeGoods.sell
                    && d.circulationType != (int)CirculationType.Bind)
                    list.Add(d);
            });
            return list;
        }

        public IEnumerable<BagItemDto> GetStallDtos()
        {
            if (backPackDto == null || backPackDto.items.Count == 0)
                return null;

             return backPackDto.items.Filter(d => d.stallable && d.circulationType != (int)CirculationType.Bind);
        }

        public BagItemDto GetBagItemByIndex(int index)
        {
            if (backPackDto == null || backPackDto.items.IsNullOrEmpty())
                return null;

            return backPackDto.items.Find(i=>i.index == index);
        }
        public object GetBagItemByIndex(int index,ItemTypeTab tab)
        {
            switch(tab)
            {
                case ItemTypeTab.Item:
                    return backPackDto.items.Find(i => i.index == index);
                case ItemTypeTab.Pet:
                    {
                        CrewChipDto crew_dto = null;
                        _crewChips.TryGetValue(index, out crew_dto);
                        return crew_dto;
                    }
                    
                case ItemTypeTab.Task:
                    {
                        ItemDto temp_dto = null;
                        _missionItem.TryGetValue(index, out temp_dto);
                        return temp_dto;
                    }
            }
            return null;
            
        }

        public BagItemDto GetBagItemByTabAndIndex(int index)
        {
            return backPackDto.items.Find<BagItemDto>(item=>item.index == index);
        }

        public int ItemBagCapability
        {
            get { return backPackDto == null ? 0 : backPackDto.capability; }
        }

        //一次扩展背包消耗的道具数量
        public int expandConsumeNum
        {
            get
            {
                return ExpressionManager.UnlockBagSlots(backPackDto.capability);
            }
        }
        //是否展示排序按钮
        public bool isShowSortBtn
        {
            get
            {
                return CurBagTab == ItemTypeTab.Item ;
            }
        }
        public bool isShowComposit
        {
            get
            {
                return CurBagTab == ItemTypeTab.Item && CurTab == BackpackViewTab.Backpack;
            }
        }
        public bool isShowDeComposit
        {
            get
            {
                return CurBagTab == ItemTypeTab.Item && CurTab == BackpackViewTab.Backpack;
            }
        }
        public DateTime lastSortBagTime { get; set; }
        public DateTime lastSortWareHouseTime { get; set; }
        #endregion MainViewRegion

        public bool IsTempPackNotNull {
            get { return tempDto != null && !tempDto.items.IsNullOrEmpty(); }
        }

        public bool CheckTempItemNull(int idx)
        {
            return tempDto != null && tempDto.items.Find(x=>x.index == idx) != null;
        }
        /// <summary>
        /// 获取背包页数~（有自动增加一页的操作~）
        /// </summary>
        /// <param name="cap"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        private int TransCapabilityToPageNum(int cap, int max)
        {
//            背包仓库每次扩容格子数
            var expandSize = DataCache.GetStaticConfigValue(AppStaticConfigs.PACK_EXPAND_SIZE, 5);
            var pageSize = DataCache.GetStaticConfigValue(AppStaticConfigs.WAREHOUSE_PAGE_SIZE, 25);
            var len = Math.Min(cap + expandSize, max);
            
            return len % pageSize > 0
                ? len / pageSize + 1
                : len / pageSize;
        }
        public bool CheckWareHousePageIsOpen(int page)
        {
            var _BagDto = DataMgr._data.wareHousePackDto;
            var pageSize = DataCache.GetStaticConfigValue(AppStaticConfigs.WAREHOUSE_PAGE_SIZE, 25);
            //如果最后一页没有任何一行解锁~则不允许改名
            if ((page >= _BagDto.capability / pageSize)
                && _BagDto.capability % pageSize == 0)
            {
                return false; 
            }
            return true;
        }
        public IEnumerable<BagItemDto> GetWarehouseItems()
        {
            if (wareHousePackDto == null)
                return null;

            return wareHousePackDto.items;
        }

        public BagItemDto GetWarehouseItemsByIndex(int idx){
            if (wareHousePackDto == null)
                return null;

            //return wareHousePackDto.items.TryGetValue(idx);
            return wareHousePackDto.items.Find(x => x.index == idx);
        }

        public bool UpdateBagDto(BagDto dto)
        {
            if (dto == null) return false;
            GameLog.Log_BAG("BagDtoNotify bagid = "+((AppItem.BagEnum)dto.bagId).ToString());
            _dtoDic.AddOrReplace((AppItem.BagEnum)dto.bagId, dto);
            return true;
        }

        public void UpdateWarehouseNameSet(List<WarehousePageNotify> dtoWarehousePageDtos)
        {
            var pageNum = WareHousePageNum;
            _wareHouseNameStrSet.Clear();
            for (var i = 0; i < pageNum; i++)
            {
                _wareHouseNameStrSet.Add(string.Format("仓库{0}", i + 1));
            }
            
            dtoWarehousePageDtos.ForEach<WarehousePageNotify>(s=> {
                if(!string.IsNullOrEmpty(s.pageName))
                    _wareHouseNameStrSet[s.pageIndex] = s.pageName;
            });
        }

        public void InitData()
        {
            _wareHouseNameStrSet = new List<string>(10);
            compositeSelIdx = -1;
            _tradeGoods = DataCache.getArrayByCls<TradeGoods>();
        }

        #region Composite Page
        
        public CompositeTabType CurCompositTab
        {
            get { return curCompositTab; }
            set
            {
                curCompositTab = value;
            }
        }

        public IEnumerable<BagItemDto> GetAllItemInBagAndTemp()
        {
            IEnumerable<BagItemDto> a = backPackDto == null ? null : backPackDto.items;
            IEnumerable<BagItemDto> b = tempDto == null ? null : tempDto.items;
            return a.ConcatWithNull(b);
        }

        public IEnumerable<BagItemDto> GetCompositeItems(out int len) {
            var temp = GetAllItemInBagAndTemp().Filter(s =>
                s != null && s.item != null && s.item.mixable).ToList();
            temp.Sort(PackItemDtoComparer.ComposeItemSorter);
            len = temp.TryGetLength();
            return temp;
        }
        
        public bool IsConvinienceComposite {
            get { return isConvinienceComposite; }
        }

        public int CompositeTimes {
            get { return compositeTimes; }
        }

        public ICompositeItem CurMaterialItem
        {
            get
            {
                int len = 0;
                var items = GetCompositeItems(out len);

                var curMaterialItem = items.TryGetValue(compositeSelIdx);
                if (curMaterialItem == null)
                    return null;
                
                // 如果是便捷合成，选全部，否，选当前流通类型
                var cnt = 0;
                items.ForEachI((s, idx) =>
                {
                    if (s.item != null
                        && s.itemId == curMaterialItem.itemId
                        && (isConvinienceComposite
                            || s.circulationType == curMaterialItem.circulationType))
                        cnt += s.count;
                });
                return CompositeItem.Create(curMaterialItem.item, cnt, curMaterialItem.circulationType);
            }
        }

        public ICompositeItem OutcomeItem
        {
            get
            {
                var item = CurMaterialItem;
                if (item == null)
                    return null;

                try
                {
                    var _compositeProps = DataCache.getDtoByCls<CompositeProps>(item.Item.id);
                    var gainItem = ItemHelper.GetGeneralItemByItemId<AppItem>(_compositeProps.gainId);
                    return CompositeItem.Create(gainItem, item.Cnt / _compositeProps.consumeCount * _compositeProps.gainCount);
                }
                catch(Exception e){
                    GameDebuger.LogError(string.Format("道具合成表读取错误 ,e:{1}", e));
                    return null;
                }
            }
        }

        #endregion

        #region Decomposite Page

        public DecompositeTabType CurDecomposeTab
        {
            get { return curDecomposeTab; }
            set
            {
                curDecomposeTab = value;
            }
        }

        //获取type类所有可以分解的物品
        public IEnumerable<BagItemDto> GetDeComposeItems(
            DecompositeTabType type
            ,  out int len) {
            //  材料分页
            var temp = GetAllItemInBagAndTemp().Filter(s => {
                if (s != null && s.item != null && s.item.resolveable && GetItemTypes(s.item.itemType) == type)
                {
                    return true;
                }
                    
                return false;
            }).ToList();
            //var temp = GetAllItemInBagAndTemp().Filter(s =>
            //    s != null && s.item != null && s.item.resolveable && s.item.itemType == _type)
            //    .ToList();
            
            temp.Sort(PackItemDtoComparer.DecomposeMaterialSorter);
            len = temp.TryGetLength();
            return temp;
        }
        /// <summary>
        /// 根据Item_type 转换为 材料/装备/结晶贿赂~ （待优化）
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private DecompositeTabType GetItemTypes(int item_type)
        {
            DecompositeTabType res = DecompositeTabType.Material;
            if((item_type /100) == (int)AppItem.ItemTypeEnum.Props/100)
            {
                res = DecompositeTabType.Material;
            }else if(item_type == (int)AppItem.ItemTypeEnum.Equipment)
            {
                res = DecompositeTabType.Equip;
            }else if(item_type == (int)AppItem.ItemTypeEnum.Quartz)
            {
                res = DecompositeTabType.Circuit;
            }
            return res;
        }
        public IDeCompositeItem CurDecomposeItem {
            get
            {
                int len = 0;
                var items = GetDeComposeItems(CurDecomposeTab, out len);
                var curDecMaterialItem = items.TryGetValue(decomposeSelIdx);
                if (curDecMaterialItem == null)
                return null;

                var cnt = 0;
                items.ForEach(s =>
                {
                    if (s.item != null
                        && s.itemId == curDecMaterialItem.itemId)
                        cnt += s.count;
                });
                return DeCompositeItem.Create(curDecMaterialItem.item, cnt, curDecMaterialItem.circulationType);
            }
        }

        private List<GeneralItem> acuqireList = new List<GeneralItem>();
        public List<GeneralItem> DecAcquireItems
        {
            get
            {
                var item = CurDecomposeItem;
                if (item == null)
                    return null;

                try
                {
                    acuqireList.Clear();
                    var decProps = DataCache.getDtoByCls<ResolveProps>(item.Item.id);
                    decProps.gainItems.ForEach(id =>
                    {
                        var acquireItem = ItemHelper.GetGeneralItemByItemId(id);
                        acuqireList.Add(acquireItem);
                    });
                    return acuqireList;
                }
                catch (Exception e)
                {
                    GameDebuger.LogError(string.Format("道具合成表读取错误 ,e:{1}", e));
                    return null;
                }
            }
        }

        public int DecomposeTimes
        {
            get { return decomposeTimes; }
            set
            {
                decomposeTimes = value;
            }
        }

        private List<ResolveGainDto> gainDtoList;
        public List<ResolveGainDto> GainDtoList
        {
            get { return gainDtoList; }
            set
            {
                gainDtoList = value;
            }
        }

        public IEnumerable<ITabInfo> CurBagTabInfos
        {
            get
            {
                if (CurTab == BackpackViewTab.Warehouse)
                {
                    ItemTabNameSet.GetElememtsByRange(0, 1);
                    return ItemTabNameSet.GetElememtsByRange(0, 0);
                }
                else
                {
                    return ItemTabNameSet;
                }
            }
        }
        private static List<ITabInfo> DecItemTabName = new List<ITabInfo>()
        {
            TabInfoData.Create((int) DecompositeTabType.Material, "材料")
            //暂时隐藏
            //, TabInfoData.Create((int) DecompositeTabType.Equip, "装备")
            //,TabInfoData.Create((int)DecompositeTabType.Circuit, "回路")
        };
        public List<ITabInfo> Composite_itemsPageTab
        {
            get
            {
                if (CurCompositTab == CompositeTabType.Composite)
                    return DecItemTabName.GetRange(0, 0);
                else if (CurCompositTab == CompositeTabType.DeComposite)
                    return DecItemTabName;
                return DecItemTabName.GetRange(0, 0);
            }
        }

        public IEnumerable<EquipmentDto> equipmentCellsData
        {
            get
            {
                return EquipmentMainDataMgr.DataMgr.GetEquipmentDtoList(EquipmentMainDataMgr.EquipmentHoldTab.Equip);
            }
        }

        public void ClearCompositeViewData()
        {
            isConvinienceComposite = false;
            compositeTimes = 0;
            compositeSelIdx = -1;
        }

        public void ClearDecomposeViewData()
        {
            decomposeSelIdx = -1;
            decomposeTimes = 0;
        }

        #endregion


        /// <summary>
        /// 获取背包/临时背包剩余的格子~
        /// </summary>
        /// <returns></returns>
        public int GetLeftSlotsInBagAndTemp()
        {
            var i = 0;
            if (backPackDto != null)
                i += backPackDto.capability - backPackDto.items.TryGetLength();
            if (tempDto != null)
                i += tempDto.capability - tempDto.items.TryGetLength();
            return i;
        }
    }
}
